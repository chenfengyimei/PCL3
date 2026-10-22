using PCL.Core.Utils;

namespace PCL;

/// <summary>
///     占位更新源：PCL3 尚未接入自有更新服务器时使用。
///     始终报告“已是最新版本”，公告与下载均为空，确保用户不会被更新回 PCL CE 官方版本。
///     接入自有更新源后，请在 UpdateManager.remoteServer 中替换本类（参考其中的注释）。
/// </summary>
public class UpdatesNoopModel : IUpdateSource
{
    public string SourceName { get; set; } = "未配置（占位更新源）";

    public bool IsAvailable()
    {
        return true;
    }

    public bool RefreshCache()
    {
        return true;
    }

    public VersionDataModel GetLatestVersion(UpdateChannel channel, UpdateArch arch)
    {
        return new VersionDataModel
        {
            VersionName = ModBase.versionBaseName,
            VersionCode = ModBase.versionCode,
            Changelog = "",
            Sha256 = null,
            Source = SourceName
        };
    }

    public bool IsLatest(UpdateChannel channel, UpdateArch arch, SemVer currentVersion, int currentVersionCode)
    {
        return true;
    }

    public VersionAnnouncementDataModel GetAnnouncementList()
    {
        return new VersionAnnouncementDataModel { Content = [] };
    }

    public List<ModLoader.LoaderBase> GetDownloadLoader(UpdateChannel channel, UpdateArch arch, string output)
    {
        return [];
    }
}
