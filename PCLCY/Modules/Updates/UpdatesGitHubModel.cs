using PCL.Core.App.Localization;
using PCL.Core.Utils;
using PCL.Network;
using PCL.Network.Loaders;

namespace PCL;

/// <summary>
///     GitHub Releases 更新源：以 PCL CY Edition 公开仓库的 Release 作为自有更新渠道。
///     发布流水线打 tag（如 v3.0.1）并发布 Release 后，所有用户即可通过本源收到更新，无需自建服务器。
///     <para>通道约定：stable 取 <c>/releases/latest</c>（正式 Release），beta 取最新的 prerelease。</para>
///     <para>资产命名约定与发布流水线一致：<c>PCLCY_Release_x64.exe</c>、<c>PCLCY_Beta_ARM64.exe</c> 等。</para>
///     <para>版本代号约定：tag <c>vX.Y.Z</c> → code <c>X*1000 + Y*10 + Z</c>（如 v3.0.0 → 3000）。</para>
/// </summary>
public class UpdatesGitHubModel : IUpdateSource
{
    /// <summary>
    ///     公开仓库 slug。若 GitHub 仓库更名，仅需调整此常量。
    /// </summary>
    private const string RepoSlug = "chenfengyimei/PCL3";

    private const string LatestUrl = $"https://api.github.com/repos/{RepoSlug}/releases/latest";
    private const string ListUrl = $"https://api.github.com/repos/{RepoSlug}/releases?per_page=20";

    public string SourceName { get; set; } = "GitHub Releases";

    public bool IsAvailable()
    {
        // 匿名访问公共仓库 API 即可（限流 60 次/小时/来源 IP，远高于更新检查频率），无需任何密钥
        return true;
    }

    public bool RefreshCache()
    {
        return true;
    }

    public VersionDataModel GetLatestVersion(UpdateChannel channel, UpdateArch arch)
    {
        var release = _GetRelease(channel);
        var tagName = (string?)release["tag_name"];
        if (string.IsNullOrWhiteSpace(tagName) || !tagName.StartsWithF("v"))
            throw new Exception("GitHub Release 的 tag 不符合 vX.Y.Z 约定");
        var versionName = tagName[1..];
        var semVer = SemVer.Parse(versionName);
        return new VersionDataModel
        {
            Source = SourceName,
            VersionName = versionName,
            VersionCode = semVer.Major * 1000 + semVer.Minor * 10 + semVer.Patch,
            Changelog = (string?)release["body"] ?? "",
            Sha256 = _GetAssetSha256(release, arch, channel)
        };
    }

    public bool IsLatest(UpdateChannel channel, UpdateArch arch, SemVer currentVersion, int currentVersionCode)
    {
        var latest = GetLatestVersion(channel, arch);
        return currentVersion >= SemVer.Parse(latest.VersionName);
    }

    public VersionAnnouncementDataModel GetAnnouncementList()
    {
        // GitHub Releases 没有公告结构；公告可后续通过仓库 Discussion / 独立接口补充
        return new VersionAnnouncementDataModel { Content = [] };
    }

    public List<ModLoader.LoaderBase> GetDownloadLoader(UpdateChannel channel, UpdateArch arch, string output)
    {
        var loaders = new List<ModLoader.LoaderBase>();
        loaders.Add(new ModLoader.LoaderTask<int, List<DownloadFile>>(Lang.Text("Update.Task.GetDownloadInfo"), load =>
        {
            var release = _GetRelease(channel);
            var asset = _GetAsset(release, arch, channel);
            var dlUrl = (string?)asset["browser_download_url"];
            if (string.IsNullOrWhiteSpace(dlUrl))
                throw new Exception("GitHub Release 中未找到对应架构的下载资产");
            load.output = new List<DownloadFile> { new([dlUrl], output) };
        }));
        loaders.Add(new LoaderDownload(Lang.Text("Update.Task.DownloadUpdateFile"), new List<DownloadFile>()));
        return loaders;
    }

    private static JsonObject _GetRelease(UpdateChannel channel)
    {
        if (channel == UpdateChannel.stable)
        {
            var latest = Requester.FetchJson(LatestUrl, RequestParam.WithRetry) as JsonObject;
            if (latest is null || latest["tag_name"] is null)
                throw new Exception("GitHub Releases 暂无正式版");
            return latest;
        }

        // beta 通道：取最新的 prerelease；仓库尚无 prerelease 时回退到正式版
        var list = Requester.FetchJson(ListUrl, RequestParam.WithRetry) as JsonArray;
        if (list is not null)
            foreach (var node in list.OfType<JsonObject>())
                if (node["prerelease"]?.GetValue<bool>() == true)
                    return node;
        return _GetRelease(UpdateChannel.stable);
    }

    private static string _AssetSuffix(UpdateChannel channel, UpdateArch arch)
    {
        var configuration = channel == UpdateChannel.stable ? "Release" : "Beta";
        var architecture = arch == UpdateArch.arm64 ? "ARM64" : "x64";
        return $"_{configuration}_{architecture}.exe";
    }

    private static JsonObject _GetAsset(JsonObject release, UpdateArch arch, UpdateChannel channel)
    {
        if (release["assets"]?.AsArray() is not { } assets)
            throw new Exception("GitHub Release 中没有下载资产");
        var asset = assets.OfType<JsonObject>()
            .FirstOrDefault(a => ((string?)a["name"])?.EndsWithF(_AssetSuffix(channel, arch), ignoreCase: true) == true);
        if (asset is null)
            throw new Exception($"GitHub Release 中未找到匹配 {_AssetSuffix(channel, arch)} 的资产");
        return asset;
    }

    private static string _GetAssetSha256(JsonObject release, UpdateArch arch, UpdateChannel channel)
    {
        try
        {
            var digest = (string?)_GetAsset(release, arch, channel)["digest"];
            return digest?.StartsWithF("sha256:") == true ? digest["sha256:".Length..] : digest;
        }
        catch
        {
            // 缺少 digest 时返回空，由调用方跳过校验
            return null;
        }
    }
}
