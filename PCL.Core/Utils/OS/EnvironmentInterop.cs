using System;
using PCL.Core.Logging;
using PCL.Core.Utils.Exts;

namespace PCL.Core.Utils.OS;

public static partial class EnvironmentInterop
{
    private const string LogModule = "Environment";

    /// <summary>
    ///     运行时密钥覆盖（无限制中心）。优先级最高：非空值直接生效，允许用户在不重新构建的情况下
    ///     注入微软 Client ID、CurseForge Key、联机服务器等构建期密钥。
    /// </summary>
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> RuntimeSecrets = new();

    /// <summary>
    ///     写入一条运行时密钥覆盖（空值删除覆盖，回落到构建期注入值）。
    /// </summary>
    public static void SetRuntimeSecret(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            RuntimeSecrets.TryRemove(key, out _);
        else
            RuntimeSecrets[key] = value;
    }

    /// <summary>
    ///     读取运行时覆盖的密钥（不存在返回 null）。
    /// </summary>
    public static string? GetRuntimeSecret(string key)
        => RuntimeSecrets.TryGetValue(key, out var value) ? value : null;

    /// <summary>
    /// 读取环境变量并使用 <see cref="StringExtension.Convert{T}"/> 将其转换为指定类型并写入目标引用。
    /// </summary>
    /// <param name="key">环境变量名</param>
    /// <param name="target">需要写入的目标引用 (不存在该环境变量或转换失败时不会写入)</param>
    /// <param name="detailLog">是否在日志中输出变量值</param>
    /// <typeparam name="TValue">目标引用的类型</typeparam>
    /// <returns>是否成功写入目标引用</returns>
    public static bool ReadVariable<TValue>(string key, ref TValue target, bool detailLog = true)
    {
        var envValue = Environment.GetEnvironmentVariable(key);
        if (envValue is null) return false;
        var valueLog = detailLog ? $" = {envValue}" : string.Empty;
        LogWrapper.Debug(LogModule, $"读取到环境变量 {key}{valueLog}");
        var value = envValue.Convert<TValue>();
        if (value is null)
        {
            LogWrapper.Warn(LogModule, $"环境变量 {key} 类型转换失败");
            return false;
        }
        target = value;
        return true;
    }

    public static string? GetSecret(string key, bool readEnv = true, bool readEnvDebugOnly = false)
    {
        // 无限制中心的运行时覆盖优先于构建期注入与环境变量
        if (RuntimeSecrets.TryGetValue(key, out var runtimeValue))
            return runtimeValue;
        if (!SecretDictionary.TryGetValue(key, out var value) &&
            readEnv &&
#if !DEBUG
            !readEnvDebugOnly &&
#endif
            ReadVariable($"PCL_{key}", ref value, false)
        ) SecretDictionary[key] = value;
        return value;
    }
    
    /// <summary>
    /// 获取当前操作系统名称。
    /// </summary>
    /// <returns>返回小写的操作系统名称，如 "windows", "linux", "osx"。</returns>
    public static string GetCurrentOsName() {
        if (OperatingSystem.IsWindows())
            return "windows";
        if (OperatingSystem.IsLinux())
            return "linux";
        return OperatingSystem.IsMacOS() 
            ? "osx"
            : "unknown";
    }
}
