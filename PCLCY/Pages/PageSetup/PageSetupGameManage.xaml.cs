using System.Windows;
using System.Windows.Controls;
using PCL.Core.App;
using PCL.Core.App.Configuration;
using PCL.Core.App.Localization;
using PCL.Core.Utils;
using PCL.Network;

namespace PCL;

public partial class PageSetupGameManage
{
    private new bool isLoaded;

    public PageSetupGameManage()
    {
        InitializeComponent();
        Loaded += PageSetupSystem_Loaded;
    }

    private void PageSetupSystem_Loaded(object sender, RoutedEventArgs e)
    {
        // 重复加载部分
        PanBack.ScrollToHome();

        // 非重复加载部分
        if (isLoaded)
            return;
        isLoaded = true;

        ModAnimation.AniControlEnabled += 1;
        Reload();
        SliderLoad();

        ModAnimation.AniControlEnabled -= 1;
    }

    public void Reload()
    {
        // 下载
        SliderDownloadThread.Value = Config.Download.ThreadLimit;
        SliderDownloadFileConnection.Value = Config.Download.FileConnectionLimit;
        SliderDownloadSpeed.Value = Config.Download.SpeedLimit;
        ComboDownloadHttpMode.SelectedIndex = (int)Config.Download.HttpMode;
        ComboDownloadSource.SelectedIndex = Config.Download.FileSource;
        ComboDownloadVersion.SelectedIndex = Config.Download.VersionListSource;
        CheckDownloadAutoSelectVersion.Checked = Config.Download.AutoSelectInstance;
        CheckFixAuthlib.Checked = Config.Download.FixAuthLib;

        // Mod 与整合包
        ComboDownloadTranslateV2.SelectedIndex = Config.Download.Comp.NameFormatV2;
        ComboDownloadMod.SelectedIndex = Config.Download.Comp.CompSourceSolution;
        ComboModLocalNameStyle.SelectedIndex = Config.Download.Comp.UiCompNameSolution;
        ComboDownloadQuickBehavior.SelectedIndex = Config.Download.Comp.QuickDownloadBehavior;
        CheckDownloadIgnoreQuilt.Checked = Config.Download.Comp.IgnoreQuilt;
        CheckDownloadAutoInstallDependencies.Checked = Config.Download.Comp.AutoInstallDependencies;
        CheckDownloadClipboard.Checked = Config.Download.Comp.ReadClipboard;

        // Minecraft 更新提示
        CheckUpdateRelease.Checked = Config.Tool.ReleaseNotification;
        CheckUpdateSnapshot.Checked = Config.Tool.SnapshotNotification;

        // 辅助设置
        CheckHelpLauncherLanguage.Checked = Config.Tool.AutoChangeLanguage;
    }

    // 初始化
    public void Reset()
    {
        try
        {
            Config.Download.Reset();
            Config.Tool.Reset();
            ModBase.Log("[Setup] 已初始化其他页设置");
            HintService.Hint(Lang.Text("Setup.GameManage.Initialized"), HintType.Success, false);
        }
        catch (Exception ex)
        {
            ModBase.Log(
                ex,
                Lang.Text("Setup.GameManage.Error.InitFailed"),
                ModBase.LogLevel.Msgbox,
                userSummary: Lang.Text("Setup.GameManage.Error.InitFailed"));
        }

        Reload();
    }

    // 将控件改变路由到设置改变
    private void CheckBoxChange(object senderRaw, bool user)
    {
        var sender = (MyCheckBox)senderRaw;
        if (ModAnimation.AniControlEnabled == 0)
            SetByTag(sender.Tag?.ToString(), sender.Checked);
    }

    private void SliderChange(object senderRaw, bool user)
    {
        var sender = (MySlider)senderRaw;
        if (ModAnimation.AniControlEnabled == 0)
            SetByTag(sender.Tag?.ToString(), sender.Value);
    }

    private void ComboChange(object senderRaw, SelectionChangedEventArgs e)
    {
        var sender = (MyComboBox)senderRaw;
        if (ModAnimation.AniControlEnabled == 0)
            SetByTag(sender.Tag?.ToString(), sender.SelectedIndex);
    }

    private static void SetByTag(string tag, object value)
        => ConfigService.TrySetValue(tag, value);

    // 滑动条
    private void SliderLoad()
    {
        SliderDownloadThread.getHintText = new Func<object, object>(v => (int)v + 1);
        SliderDownloadFileConnection.getHintText = new Func<object, object>(v => (int)v + 1);
        SliderDownloadSpeed.getHintText = new Func<object, object>(v =>
        {
            int value = (int)v;
            switch (value)
            {
                case <= 14:
                    return Lang.Number((value + 1) * 0.1d, "N1") + " MiB/s";
                case <= 31:
                    return Lang.Number((value - 11) * 0.5d, "N1") + " MiB/s";
                case <= 41:
                    return Lang.Number(value - 21, "N0") + " MiB/s";
                default:
                    return Lang.Text("Setup.GameManage.Download.Unlimited");
            }
        });
    }

    private void SliderDownloadThread_PreviewChange(object sender, ModBase.RouteEventArgs e)
    {
        if (SliderDownloadThread.Value < 64)
            return;
        if (!States.Hint.LargeDownloadThread)
        {
            States.Hint.LargeDownloadThread = true;
            ModMain.MyMsgBox(
                Lang.Text("Setup.GameManage.Download.Threads.TooManyWarning.Message"),
                Lang.Text("Common.Dialog.Warning"),
                Lang.Text("Setup.GameManage.Download.Threads.TooManyWarning.Confirm"), isWarn: true);
        }
    }

    // ================= 网络诊断：下载源健康状态 =================

    /// <summary>参与诊断展示的下载源（host → 展示名键）。</summary>
    private static readonly (string Host, string NameKey)[] DiagnosticSources =
    [
        ("api.curseforge.com", "Setup.GameManage.NetworkDiagnosis.Source.CurseForge"),
        ("api.modrinth.com", "Setup.GameManage.NetworkDiagnosis.Source.Modrinth"),
        ("mod.mcimirror.top", "Setup.GameManage.NetworkDiagnosis.Source.Mcim"),
        ("bmclapi2.bangbang93.com", "Setup.GameManage.NetworkDiagnosis.Source.Bmclapi"),
        ("piston-meta.mojang.com", "Setup.GameManage.NetworkDiagnosis.Source.Mojang"),
    ];

    /// <summary>轻量探测 URL：每个源取一个低成本稳定端点（只看连通性，不校验业务内容）。</summary>
    private static string ProbeUrlOf(string host) => host switch
    {
        "api.curseforge.com" => "https://api.curseforge.com/v1/games",
        "api.modrinth.com" => "https://api.modrinth.com/",
        "mod.mcimirror.top" => "https://mod.mcimirror.top/",
        "bmclapi2.bangbang93.com" => "https://bmclapi2.bangbang93.com/mc/game/versionmanifest.json",
        "piston-meta.mojang.com" => "https://piston-meta.mojang.com/mc/game/version_manifest.json",
        _ => $"https://{host}/",
    };

    private void RefreshSourceHealthPanel()
    {
        var snapshot = SourceHealthTracker.Snapshot();
        PanSourceHealth.Children.Clear();
        foreach (var (host, nameKey) in DiagnosticSources)
        {
            snapshot.TryGetValue(host, out var state);
            var failures = state.Failures;
            var cooling = state.CooldownRemainingMs > 0;
            var row = new Grid { Margin = new Thickness(0, 2, 0, 2) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var icon = new TextBlock
            {
                // 🟢 正常 / 🟡 冷却中 / 🔴 连续异常（阈值内失败过但未冷却不亮红）
                Text = failures == 0 ? "🟢" : cooling ? "🟡" : "🔴",
                Margin = new Thickness(0, 0, 8, 0),
            };
            var name = new TextBlock { Text = Lang.Text(nameKey) };
            Grid.SetColumn(icon, 0);
            Grid.SetColumn(name, 1);
            row.Children.Add(icon);
            row.Children.Add(name);
            if (cooling)
            {
                var status = new TextBlock
                {
                    Text = Lang.Text("Setup.GameManage.NetworkDiagnosis.Cooldown",
                        Math.Ceiling(state.CooldownRemainingMs / 1000d)),
                    Opacity = 0.6,
                    HorizontalAlignment = HorizontalAlignment.Right,
                };
                Grid.SetColumn(status, 1);
                row.Children.Add(status);
            }
            else if (failures > 0)
            {
                var status = new TextBlock
                {
                    Text = Lang.Text("Setup.GameManage.NetworkDiagnosis.FailCount", failures),
                    Opacity = 0.6,
                    HorizontalAlignment = HorizontalAlignment.Right,
                };
                Grid.SetColumn(status, 1);
                row.Children.Add(status);
            }
            PanSourceHealth.Children.Add(row);
        }
    }

    private void BtnSourceHealthReset_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        SourceHealthTracker.Reset();
        RefreshSourceHealthPanel();
        HintService.Hint(Lang.Text("Setup.GameManage.NetworkDiagnosis.Reset.Success"), HintType.Success);
    }

    private void BtnSourceHealthCheck_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        HintService.Hint(Lang.Text("Setup.GameManage.NetworkDiagnosis.Checking"));
        // 各源并行轻量探测；只更新记账，不动用户配置
        ModBase.RunInThread(() =>
        {
            foreach (var (host, _) in DiagnosticSources)
            {
                try
                {
                    Requester.FetchString(ProbeUrlOf(host), new RequestParam
                    {
                        Timeout = 8000,
                        UseBrowserUserAgent = true,
                    });
                    SourceHealthTracker.RecordSuccess($"https://{host}/");
                }
                catch
                {
                    SourceHealthTracker.RecordFailure($"https://{host}/");
                }
            }
            ModBase.RunInUi(RefreshSourceHealthPanel);
            HintService.Hint(Lang.Text("Setup.GameManage.NetworkDiagnosis.CheckDone"), HintType.Success);
        });
    }
}
