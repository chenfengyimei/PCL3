using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PCL.Core.Utils;

/// <summary>
///     下载源健康记账器：按 host 记录连续失败与冷却窗口，供多源重试排序。
///     <list type="bullet">
///         <item>成功一次即清零该 host 的失败状态</item>
///         <item>连续失败达到阈值后进入冷却：冷却期内该源在候选排序中被压后（不禁止，只降权）</item>
///         <item>纯内存、线程安全；启动器重启后自然复位</item>
///     </list>
/// </summary>
public static class SourceHealthTracker
{
    /// <summary>
    ///     连续失败多少次后进入冷却。
    /// </summary>
    public const int FailureThreshold = 2;

    /// <summary>
    ///     冷却时长（毫秒）。冷却结束后源自动回到正常行列（半开：下次结果决定是否立即再冷却）。
    /// </summary>
    public const int CooldownMs = 60_000;

    private sealed record HealthState
    {
        public int ConsecutiveFailures { get; init; }
        public long LastFailureAt { get; init; }
    }

    private static readonly ConcurrentDictionary<string, HealthState> States = new();

    private static string HostOf(string url)
    {
        try
        {
            return new Uri(url).Host;
        }
        catch
        {
            return url;
        }
    }

    /// <summary>
    ///     记录一次成功：清除该 host 的失败状态。
    /// </summary>
    public static void RecordSuccess(string url)
    {
        States.TryRemove(HostOf(url), out _);
    }

    /// <summary>
    ///     记录一次失败：累计连续失败，达到阈值即进入冷却。
    /// </summary>
    public static void RecordFailure(string url)
    {
        var host = HostOf(url);
        States.AddOrUpdate(host,
            new HealthState { ConsecutiveFailures = 1, LastFailureAt = Environment.TickCount64 },
            (_, old) => new HealthState
            {
                ConsecutiveFailures = old.ConsecutiveFailures + 1,
                LastFailureAt = Environment.TickCount64,
            });
    }

    /// <summary>
    ///     该 host 当前是否处于冷却期（连续失败达阈值且冷却未过）。
    /// </summary>
    public static bool IsCoolingDown(string url)
    {
        if (!States.TryGetValue(HostOf(url), out var state)) return false;
        if (state.ConsecutiveFailures < FailureThreshold) return false;
        return Environment.TickCount64 - state.LastFailureAt < CooldownMs;
    }

    /// <summary>
    ///     排序分：健康源 0，冷却源 +1（只用于稳定排序的降权，不作为硬性禁用）。
    /// </summary>
    public static int Score(string url) => IsCoolingDown(url) ? 1 : 0;

    /// <summary>
    ///     各源健康快照（UI 展示用）。返回 host → (连续失败数, 冷却剩余毫秒)。
    /// </summary>
    public static IReadOnlyDictionary<string, (int Failures, int CooldownRemainingMs)> Snapshot()
    {
        var result = new Dictionary<string, (int, int)>();
        foreach (var (host, state) in States)
        {
            var cooling = state.ConsecutiveFailures >= FailureThreshold;
            var remaining = cooling
                ? Math.Max(0, CooldownMs - (int)(Environment.TickCount64 - state.LastFailureAt))
                : 0;
            result[host] = (state.ConsecutiveFailures, remaining);
        }
        return result;
    }

    /// <summary>
    ///     清空全部健康状态（「重置健康状态」按钮 / 手动检测前用）。
    /// </summary>
    public static void Reset()
    {
        States.Clear();
    }
}
