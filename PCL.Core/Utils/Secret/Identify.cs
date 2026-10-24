using System;
using System.Management;
using System.Text;
using PCL.Core.Logging;
using PCL.Core.Utils.Exts;
using PCL.Core.Utils.Hash;

namespace PCL.Core.Utils.Secret;

public class Identify
{
    /// <summary>识别码替换种子（无限制中心"重新生成识别码"用）：非空时覆盖机器指纹派生值。</summary>
    private static byte[]? _overrideRawId;

    /// <summary>LauncherId 缓存字段（配合半自动属性手动管理，支持重新生成后失效）。</summary>
    private static string? _launcherIdCache;

    public static byte[] RawId => _overrideRawId ??= _GetRawId();

    public static string LauncherId => _launcherIdCache ??= _getLauncherId();

    /// <summary>
    ///     重新生成识别码（无限制中心）。用随机数替换机器指纹派生的原始 ID，
    ///     LauncherId 随之改变（今日人品种子、匿名标识等都会换新）。
    /// </summary>
    public static void Regenerate()
    {
        var random = new byte[64];
        System.Security.Cryptography.RandomNumberGenerator.Fill(random);
        _overrideRawId = System.Text.Encoding.UTF8.GetBytes(
            PCL.Core.Utils.Hash.SHA512Provider.Instance.ComputeHash(random).ToHexString());
        _launcherIdCache = null; // 清除缓存，下次访问按新 RawId 重算
    }

    private static byte[] _GetRawId()
    {
        var code = new StringBuilder();
        try
        {
            code.Append("UUID:").Append(_GetWmiProperty("Win32_ComputerSystemProduct", "UUID"))
                .Append("|MB_Prod:").Append(_GetWmiProperty("Win32_BaseBoard", "Product"))
                .Append("|MB_SN:").Append(_GetWmiProperty("Win32_BaseBoard", "SerialNumber"))
                .Append("|CPU:").Append(_GetWmiProperty("Win32_Processor", "ProcessorId"));
        }
        catch (Exception ex)
        {
            LogWrapper.Error(ex, "Identify", "获取设备基础信息失败");
        }

        return Encoding.UTF8.GetBytes(SHA512Provider.Instance.ComputeHash(code.ToString()).ToHexString());
    }

    private static string _GetWmiProperty(string className, string propertyName)
    {
        try
        {
            using var searcher =
                new ManagementObjectSearcher($"SELECT {propertyName} FROM {className}");
            using var results = searcher.Get();
            foreach (var obj in results)
            {
                if (obj[propertyName] is not null)
                    return (obj[propertyName].ToString() ?? string.Empty).Trim();
            }
        }
        catch { /* Ignore */ }
        return string.Empty;
    }

    private static string _getLauncherId()
    {
        try
        {
            var prefix = "PCL-CE|"u8.ToArray();
            var ctx = RawId;
            var suffix = "|LauncherId"u8.ToArray();

            var buffer = new byte[prefix.Length + ctx.Length + suffix.Length];
            var bufferSpan = buffer.AsSpan();
            prefix.CopyTo(bufferSpan[..prefix.Length]);
            ctx.CopyTo(bufferSpan.Slice(prefix.Length, ctx.Length));
            suffix.CopyTo(bufferSpan.Slice(prefix.Length + ctx.Length, suffix.Length));

            Array.Clear(ctx);
            var sample = SHA512Provider.Instance.ComputeHash(bufferSpan).ToHexString();
            bufferSpan.Clear();

            // 16 in length, 8 bytes, 64 bits, enough for us
            return sample.Substring(64, 16)
                .ToUpper()
                .Insert(4, "-")
                .Insert(9, "-")
                .Insert(14, "-");
        }
        catch (Exception ex)
        {
            LogWrapper.Error(ex, "Identify", "无法获取识别码");
            return "PCL2-CECE-GOOD-2025";
        }
    }
}