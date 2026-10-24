using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PCL.Core.App;
using PCL.Core.App.Localization;

namespace PCL;

/// <summary>
///     无限制中心。PCL CY Edition 将启动器内所有隐藏参数、彩蛋、状态开关集中暴露，
///     给用户最大的开放性：今日人品可指定、计数可改、所有"不再提示"可重置、
///     愚人节可强制、主页预设源可查看复制、外部内容文件可直达。
/// </summary>
public partial class PageUnlimited
{
    public PageUnlimited()
    {
        InitializeComponent();
        Loaded += (_, _) => Reload();
    }

    public void Reload()
    {
        // 构建密钥注入
        TextSecretMsClient.Text = Config.Unlimited.MicrosoftClientId;
        TextSecretCfKey.Text = Config.Unlimited.CurseforgeApiKey;
        TextSecretLinkServers.Text = Config.Unlimited.LinkServers;

        // 调试选项
        CheckDebugMode.SetChecked(Config.Debug.Enabled, false);
        CheckDebugDelay.SetChecked(Config.Debug.AddRandomDelay, false);
        CheckDebugSkipCopy.SetChecked(Config.Debug.DontCopy, false);
        CheckDebugRestricted.SetChecked(Config.Debug.AllowRestrictedFeature, false);

        // 下载极限
        SliderDlThreads.Value = Config.Download.ThreadLimit;
        SliderDlConnections.Value = Config.Download.FileConnectionLimit;
        SliderDlSpeed.Value = Config.Download.SpeedLimit;

        // 识别码
        LabLauncherId.Text = PCL.Core.Utils.Secret.Identify.LauncherId;

        // 今日人品
        var luckValue = new Random(PageToolsTest.GenerateDailySeed()).Next(0, 101);
        LabLuckToday.Text = $"{luckValue} ({PageToolsTest.GetRating(luckValue)})";
        TextLuckForce.Text = _nextLuckValue?.ToString() ?? "";

        // 运行数据
        LabStartupCount.Text = States.System.StartupCount.ToString();
        LabLaunchCount.Text = States.System.LaunchCount.ToString();

        // 提示与确认状态
        BuildHintFlags();

        // 愚人节
        CheckFoolForce.Checked = PCL.Core.App.Basics.UnlimitedForce.ForceAprilFool == true;

        // 主页预设源
        BuildPresetUrls();
    }

    #region 构建密钥注入

    /// <summary>
    ///     把持久化的密钥覆盖预热进运行时覆盖层（优先级最高）。
    ///     在 FormMain 启动早期与每次页面保存时调用。
    /// </summary>
    public static void WarmupSecrets()
    {
        try
        {
            var u = Config.Unlimited;
            PCL.Core.Utils.OS.EnvironmentInterop.SetRuntimeSecret("MS_CLIENT_ID", u.MicrosoftClientId);
            PCL.Core.Utils.OS.EnvironmentInterop.SetRuntimeSecret("CURSEFORGE_API_KEY", u.CurseforgeApiKey);
            PCL.Core.Utils.OS.EnvironmentInterop.SetRuntimeSecret("LINK_SERVER_ROOT", u.LinkServers);
        }
        catch
        {
            // 配置尚未就绪时跳过，页面打开后会再次预热
        }
    }

    private void BtnSecretMsClient_Click(object sender, MouseButtonEventArgs e)
    {
        Config.Unlimited.MicrosoftClientId = TextSecretMsClient.Text.Trim();
        WarmupSecrets();
        HintService.Hint(Lang.Text("Unlimited.Secrets.Applied"), HintType.Success);
    }

    private void BtnSecretCfKey_Click(object sender, MouseButtonEventArgs e)
    {
        Config.Unlimited.CurseforgeApiKey = TextSecretCfKey.Text.Trim();
        WarmupSecrets();
        HintService.Hint(Lang.Text("Unlimited.Secrets.Applied"), HintType.Success);
    }

    private void BtnSecretLinkServers_Click(object sender, MouseButtonEventArgs e)
    {
        Config.Unlimited.LinkServers = TextSecretLinkServers.Text.Trim();
        WarmupSecrets();
        HintService.Hint(Lang.Text("Unlimited.Secrets.Applied"), HintType.Success);
    }

    #endregion

    #region 调试选项

    private void DebugFlag_Change(object sender, bool user)
    {
        if (!user) return;
        Config.Debug.Enabled = CheckDebugMode.Checked == true;
        Config.Debug.AddRandomDelay = CheckDebugDelay.Checked == true;
        Config.Debug.DontCopy = CheckDebugSkipCopy.Checked == true;
        Config.Debug.AllowRestrictedFeature = CheckDebugRestricted.Checked == true;
        HintService.Hint(Lang.Text("Unlimited.Debug.Applied"), HintType.Success);
    }

    #endregion

    #region 下载极限

    private void DlSlider_Change(object sender, bool user)
    {
        if (!user) return;
        switch ((string)((MySlider)sender).Tag)
        {
            case "DlThreads":
                Config.Download.ThreadLimit = (int)SliderDlThreads.Value;
                break;
            case "DlConnections":
                Config.Download.FileConnectionLimit = (int)SliderDlConnections.Value;
                break;
            case "DlSpeed":
                Config.Download.SpeedLimit = (int)SliderDlSpeed.Value;
                break;
        }
    }

    #endregion

    #region 识别码

    private void BtnRegenId_Click(object sender, MouseButtonEventArgs e)
    {
        if (ModMain.MyMsgBox(Lang.Text("Unlimited.Id.Regen.Confirm.Message"), Lang.Text("Unlimited.Id.Regen.Confirm.Title"),
                Lang.Text("Common.Action.Confirm"), Lang.Text("Common.Action.Cancel"), isWarn: true) != 1) return;
        // 识别码基于机器指纹派生，重新生成等价于换一台"新机器"：今日人品种子随之改变
        PCL.Core.Utils.Secret.Identify.Regenerate();
        LabLauncherId.Text = PCL.Core.Utils.Secret.Identify.LauncherId;
        HintService.Hint(Lang.Text("Unlimited.Id.Regen.Done"), HintType.Success);
        Reload();
    }

    #endregion

    #region 今日人品

    /// <summary>下一次今日人品的强制值（null = 正常随机）。今日人品为每日固定随机，强制值仅生效一次。</summary>
    public static int? _nextLuckValue;

    private void BtnLuckRoll_Click(object sender, MouseButtonEventArgs e)
    {
        PageToolsTest.Jrrp();
        // 抽过之后清除强制值（一次性）
        _nextLuckValue = null;
        Reload();
    }

    private void BtnLuckForce_Click(object sender, MouseButtonEventArgs e)
    {
        if (!int.TryParse(TextLuckForce.Text.Trim(), out var value) || value is < 0 or > 100)
        {
            HintService.Hint(Lang.Text("Unlimited.Luck.Force.Invalid"), HintType.Error);
            return;
        }

        _nextLuckValue = value;
        HintService.Hint(Lang.Text("Unlimited.Luck.Force.Success", value), HintType.Success);
    }

    #endregion

    #region 运行数据

    private void BtnStartupEdit_Click(object sender, MouseButtonEventArgs e)
    {
        var input = ModMain.MyMsgBoxInput(Lang.Text("Unlimited.Stats.StartupCount"),
            Lang.Text("Unlimited.Stats.InputHint"), States.System.StartupCount.ToString());
        if (int.TryParse(input, out var value) && value >= 0)
        {
            States.System.StartupCount = value;
            HintService.Hint(Lang.Text("Unlimited.Stats.Saved"), HintType.Success);
            Reload();
        }
        else if (input is not null)
        {
            HintService.Hint(Lang.Text("Unlimited.Stats.Invalid"), HintType.Error);
        }
    }

    private void BtnLaunchEdit_Click(object sender, MouseButtonEventArgs e)
    {
        var input = ModMain.MyMsgBoxInput(Lang.Text("Unlimited.Stats.LaunchCount"),
            Lang.Text("Unlimited.Stats.InputHint"), States.System.LaunchCount.ToString());
        if (int.TryParse(input, out var value) && value >= 0)
        {
            States.System.LaunchCount = value;
            HintService.Hint(Lang.Text("Unlimited.Stats.Saved"), HintType.Success);
            Reload();
        }
        else if (input is not null)
        {
            HintService.Hint(Lang.Text("Unlimited.Stats.Invalid"), HintType.Error);
        }
    }

    #endregion

    #region 提示与确认状态

    /// <summary>所有"不再提示"类的布尔状态（名称 → 读写器）。</summary>
    private static readonly (string Name, Func<bool> Get, Action<bool> Set)[] HintFlags =
    [
        ("LaunchWithProfile", () => States.Hint.LaunchWithProfile, v => States.Hint.LaunchWithProfile = v),
        ("NonAsciiGamePath", () => States.Hint.NonAsciiGamePath, v => States.Hint.NonAsciiGamePath = v),
        ("LargeDownloadThread", () => States.Hint.LargeDownloadThread, v => States.Hint.LargeDownloadThread = v),
        ("Renderer", () => States.Hint.Renderer, v => States.Hint.Renderer = v),
        ("DebugLog4j2Config", () => States.Hint.DebugLog4j2Config, v => States.Hint.DebugLog4j2Config = v),
        ("InstallPageBack", () => States.Hint.InstallPageBack, v => States.Hint.InstallPageBack = v),
        ("HideGameInstance", () => States.Hint.HideGameInstance, v => States.Hint.HideGameInstance = v),
        ("ManualInstall", () => States.Hint.ManualInstall, v => States.Hint.ManualInstall = v),
        ("UpdateMod", () => States.Hint.UpdateMod, v => States.Hint.UpdateMod = v),
        ("HomepageCommand", () => States.Hint.HomepageCommand, v => States.Hint.HomepageCommand = v),
        ("UntrustedHomepage", () => States.Hint.UntrustedHomepage, v => States.Hint.UntrustedHomepage = v),
        ("MoreInstanceSetup", () => States.Hint.MoreInstanceSetup, v => States.Hint.MoreInstanceSetup = v),
        ("IndieSetup", () => States.Hint.IndieSetup, v => States.Hint.IndieSetup = v),
        ("ExportConfig", () => States.Hint.ExportConfig, v => States.Hint.ExportConfig = v),
        ("MaxGameLog", () => States.Hint.MaxGameLog, v => States.Hint.MaxGameLog = v),
        ("SchematicFirstTime", () => States.Hint.SchematicFirstTime, v => States.Hint.SchematicFirstTime = v),
        ("FunctionDatapackUpdate", () => States.Hint.FunctionDatapackUpdate, v => States.Hint.FunctionDatapackUpdate = v),
    ];

    private void BuildHintFlags()
    {
        PanHintFlags.Children.Clear();
        foreach (var flag in HintFlags)
        {
            var target = flag;
            var check = new MyCheckBox
            {
                Height = 22,
                Margin = new Thickness(-1, 0, 0, 5),
                Text = target.Name,
            };
            check.SetChecked(target.Get(), false);
            check.Change += (_, _) => target.Set(check.Checked == true);
            PanHintFlags.Children.Add(check);
        }
    }

    private void BtnHintsResetAll_Click(object sender, MouseButtonEventArgs e)
    {
        foreach (var flag in HintFlags)
            flag.Set(false);
        HintService.Hint(Lang.Text("Unlimited.Hints.ResetDone"), HintType.Success);
        Reload();
    }

    private void BtnReplayNotice_Click(object sender, MouseButtonEventArgs e)
    {
        UpdateManager.ShowCEAnnounce();
    }

    private void BtnReplayEula_Click(object sender, MouseButtonEventArgs e)
    {
        States.System.LauncherEula = false;
        HintService.Hint(Lang.Text("Unlimited.Hints.EulaReset"), HintType.Success);
    }

    private void BtnReplayUpdateLog_Click(object sender, MouseButtonEventArgs e)
    {
        ModMain.frmMain.ShowUpdateLog();
    }

    #endregion

    #region 愚人节

    private void CheckFoolForce_Change(object sender, bool user)
    {
        if (!user) return;
        PCL.Core.App.Basics.UnlimitedForce.ForceAprilFool = CheckFoolForce.Checked;
        // 立即刷新标题栏样式
        ModSetup.UiLogoType((int)Config.Preference.WindowTitleType);
        HintService.Hint(Lang.Text("Unlimited.Fool.Applied"), HintType.Success);
    }

    #endregion

    #region 主页预设源

    /// <summary>主页预设的名称与 URL（与 PageLaunchRight 的 switch 保持一致）。</summary>
    internal static readonly (string Name, string? Url)[] HomepagePresets =
    [
        ("你知道吗", null),
        ("回声洞", null),
        ("Minecraft 新闻", "https://news.bugjump.net"),
        ("每日整合包推荐", "https://pclsub.sodamc.com/"),
        ("Minecraft 皮肤推荐", "https://forgepixel.com/pcl_sub_file"),
        ("OpenBMCLAPI 仪表盘 Lite", "https://pcl-bmcl.milu.ink/"),
        ("PCL 新功能说明书", "https://raw.gitcode.com/WForst-Breeze/WhatsNewPCL/raw/main/Custom.xaml"),
        ("杂志主页", "https://pclhomeplazaoss.lingyunawa.top:26994/d/Homepages/Ext1nguisher/Custom.xaml"),
        ("PCL GitHub 仪表盘", "https://ddf.pcl-community.org/Custom.xaml"),
        ("Minecraft 更新摘要", "https://raw.gitcode.com/ENC_Euphony/PCL-AI-Summary-HomePage/raw/master/Custom.xaml"),
        ("今日新闻热点", "https://pcl.wyc-w.top/index.xaml"),
        ("Minecraft 芝士站", "https://www.xxag.top/mkss"),
        ("整合包推荐引擎", "https://qawsedrftgyhujiko.fun/pcl2/Custom.xaml"),
        ("Bangumi 番剧主页", "https://bangumi.p.kaphia.qzz.io"),
        ("PCL CE 公告栏", "https://s3.pysio.online/pcl2-ce/apiv2/pages/announce.xaml"),
        ("Minecraft 信息流", null),
        ("简单主页", "https://pclhomeplazaoss.lingyunawa.top:26994/d/Homepages/MFn233/Custom.xaml"),
        ("PCL 主页市场", "https://pclhomeplazaoss.lingyunawa.top:26994/d/Homepages/JingHai-Lingyun/Custom.xaml"),
        ("PCL 新闻速报", "https://pclhomeplazaoss.lingyunawa.top:26994/d/Homepages/Joker2184/UpdateHomepage.xaml"),
        ("OpenMCIM 仪表盘", "https://files.mcimirror.top/PCL"),
    ];

    private void BuildPresetUrls()
    {
        PanPresetUrls.Children.Clear();
        for (var index = 0; index < HomepagePresets.Length; index++)
        {
            var preset = HomepagePresets[index];
            var row = new Grid { Margin = new Thickness(0, 1, 0, 1) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var name = new TextBlock
            {
                Text = $"{index}. {preset.Name}",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 12, 0),
            };
            Grid.SetColumn(name, 0);
            var url = new TextBlock
            {
                Text = preset.Url ?? Lang.Text("Unlimited.Presets.Local"),
                Opacity = 0.5,
                TextTrimming = TextTrimming.CharacterEllipsis,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 12, 0),
            };
            Grid.SetColumn(url, 1);
            var copy = new MyButton
            {
                Padding = new Thickness(13, 0, 13, 0),
                Text = Lang.Text("Unlimited.Presets.Copy"),
                Tag = preset.Url,
            };
            copy.IsEnabled = preset.Url is not null;
            var presetUrl = preset.Url;
            copy.Click += (_, _) => ModBase.ClipboardSet(presetUrl);
            Grid.SetColumn(copy, 2);

            row.Children.Add(name);
            row.Children.Add(url);
            row.Children.Add(copy);
            PanPresetUrls.Children.Add(row);
        }
    }

    #endregion

    #region 外部内容文件

    private void BtnOpenCaves_Click(object sender, MouseButtonEventArgs e)
    {
        ModBase.OpenExplorer(ModBase.exePath + @"PCL\");
        HintService.Hint(Lang.Text("Unlimited.Files.CavesHint"), HintType.Info);
    }

    private void BtnOpenHints_Click(object sender, MouseButtonEventArgs e)
    {
        ModBase.OpenExplorer(ModBase.exePath + @"PCL\");
        HintService.Hint(Lang.Text("Unlimited.Files.HintsHint"), HintType.Info);
    }

    #endregion
}
