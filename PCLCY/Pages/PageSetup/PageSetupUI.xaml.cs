using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PCL.Core.App;
using PCL.Core.App.Configuration;
using PCL.Core.UI;
using PCL.Core.UI.Theme;
using PCL.Core.Utils;
using PCL.Core.App.Localization;

namespace PCL;

public partial class PageSetupUI
{
    public string[] ThemeColors =>
    [
        Lang.Text("Setup.Ui.Theme.Color.SkyBlue"),
        Lang.Text("Setup.Ui.Theme.Color.CatBlue"),
        Lang.Text("Setup.Ui.Theme.Color.CrashBlue"),
        Lang.Text("Setup.Ui.Theme.Color.Hmcl"),
        Lang.Text("Setup.Ui.Theme.Color.Custom")
    ];

    public ColorTheme[] ThemeColorValues =>
    [
        ColorTheme.SkyBlue,
        ColorTheme.CatBlue,
        ColorTheme.DeathBlue,
        ColorTheme.HmclBlue,
        ColorTheme.Custom
    ];
    
    public new bool isLoaded;

    public PageSetupUI()
    {
        InitializeComponent();
        Loaded += PageSetupUI_Loaded;
        Loaded += (_, _) => HiddenRefresh();
    }

    private void PageSetupUI_Loaded(object sender, RoutedEventArgs e)
    {
        // 重复加载部分
        PanBack.ScrollToHome();

        ModAnimation.AniControlEnabled += 1;
        Reload(); // #4826，在每次进入页面时都刷新一下
        ModAnimation.AniControlEnabled -= 1;

        // 非重复加载部分
        if (isLoaded)
            return;
        isLoaded = true;

        SliderLoad();
    }

    public void Reload()
    {
        try
        {
            // 启动器
            SliderLauncherOpacity.Value = Config.Preference.Theme.WindowOpacity;
            CheckLauncherLogo.Checked = Config.Preference.ShowStartupLogo;
            ComboDarkMode.SelectedIndex = (int)Config.Preference.Theme.ColorMode;
            // 主题预览卡（首次构建 + 选中态/自定义色带刷新）
            EnsureThemeCards();
            RefreshThemeCards();
            // 自定义参数：默认编辑当前配色模式（浅/深各自独立一套四参）
            _customEditingDark = ThemeManager.IsDarkMode;
            ComboCustomTarget.SelectedIndex = _customEditingDark ? 1 : 0;
            LoadCustomSliders();
            UpdateCustomThemePanel();
            CheckShowLaunchingHint.Checked = Config.Preference.ShowLaunchingHint;

            // 字体设置
            ComboUiFont.SelectedFontTag = Config.Preference.Font;
            ComboUiMotdFont.SelectedFontTag = Config.Preference.MotdFont;

            CheckBlur.Checked = Config.Preference.Blur.IsEnabled;
            SliderBlurValue.Value = Config.Preference.Blur.Radius;
            SliderBlurSamplingRate.Value = Config.Preference.Blur.SamplingRate;
            ComboBlurType.SelectedIndex = Config.Preference.Blur.KernelType;
            PanBlurValue.Visibility = CheckBlur.Checked == true ? Visibility.Visible : Visibility.Collapsed;
            CheckLockWindowSize.Checked = Config.Preference.LockWindowSize;

            // 背景图片
            SliderBackgroundOpacity.Value = Config.Preference.Background.WallpaperOpacity;
            SliderBackgroundBlur.Value = Config.Preference.Background.WallpaperBlurRadius;
            ComboBackgroundSuit.SelectedIndex = Config.Preference.Background.WallpaperSuitMode;
            CheckBackgroundColorful.Checked = Config.Preference.Background.BackgroundColorful;
            var autoPauseVideo = Config.Preference.Background.AutoPauseVideo;
            CheckAutoPauseVideo.Checked = autoPauseVideo;
            if (ModVideoBack.IsGaming)
                if (autoPauseVideo)
                    BtnBackgroundRefresh.IsEnabled = false;

            BackgroundRefresh(false, false);

            // 标题栏
            ((MyRadioBox)FindName("RadioLogoType" + (int)Config.Preference.WindowTitleType))
                .Checked = true;
            CheckLogoLeft.Visibility = RadioLogoType0.Checked ? Visibility.Visible : Visibility.Collapsed;
            PanLogoText.Visibility = RadioLogoType2.Checked ? Visibility.Visible : Visibility.Collapsed;
            PanLogoChange.Visibility = RadioLogoType3.Checked ? Visibility.Visible : Visibility.Collapsed;
            TextLogoText.Text = Config.Preference.WindowTitleCustomText;
            CheckLogoLeft.Checked = Config.Preference.TopBarLeftAlign;

            // 背景音乐
            CheckMusicRandom.Checked = Config.Preference.Music.ShufflePlayback;
            CheckMusicAuto.Checked = Config.Preference.Music.StartOnStartup;
            CheckMusicStop.Checked = Config.Preference.Music.StopInGame;
            CheckMusicStart.Checked = Config.Preference.Music.StartInGame;
            CheckMusicSMTC.Checked = Config.Preference.Music.EnableSMTC;
            SliderMusicVolume.Value = Config.Preference.Music.Volume;
            MusicRefreshUI();

            // 主页
            try
            {
                ComboCustomPreset.SelectedIndex = Config.Preference.Homepage.SelectedPreset;
            }
            catch
            {
                Config.Preference.Homepage.SelectedPresetConfig.Reset();
            }

            ((MyRadioBox)FindName("RadioCustomType" + Config.Preference.Homepage.Type)).Checked = true;
            TextCustomNet.Text = Config.Preference.Homepage.CustomUrl;
            ModSetup.UiCustomType(Config.Preference.Homepage.Type);

            // 功能隐藏
            // 获取配置组引用
            var uiHidden = Config.Preference.Hide;

            // 主页面
            CheckHiddenPageDownload.Checked = uiHidden.PageDownload;
            CheckHiddenPageSetup.Checked = uiHidden.PageSetup;
            CheckHiddenPageTools.Checked = uiHidden.PageTools;

            // 子页面 设置
            CheckHiddenSetupLaunch.Checked = uiHidden.SetupLaunch;
            CheckHiddenSetupUI.Checked = uiHidden.SetupUi;
            CheckHiddenSetupLauncherLanguage.Checked = uiHidden.SetupLauncherLanguage;
            CheckHiddenSetupGameManage.Checked = uiHidden.SetupGameManage;
            CheckHiddenSetupJava.Checked = uiHidden.SetupJava;
            CheckHiddenLauncherMisc.Checked = uiHidden.SetupLauncherMisc;
            CheckHiddenSetupUpdate.Checked = uiHidden.SetupUpdate;
            CheckHiddenSetupGameLink.Checked = uiHidden.SetupGameLink;
            CheckHiddenSetupAbout.Checked = uiHidden.SetupAbout;
            CheckHiddenSetupFeedback.Checked = uiHidden.SetupFeedback;
            CheckHiddenSetupLog.Checked = uiHidden.SetupLog;

            // 子页面 工具
            CheckHiddenToolsGameLink.Checked = uiHidden.ToolsGameLink;
            CheckHiddenToolsTest.Checked = uiHidden.ToolsTest;

            // 子页面 实例设置
            CheckHiddenVersionEdit.Checked = uiHidden.InstanceEdit;
            CheckHiddenVersionExport.Checked = uiHidden.InstanceExport;
            CheckHiddenVersionSave.Checked = uiHidden.InstanceSave;
            CheckHiddenVersionScreenshot.Checked = uiHidden.InstanceScreenshot;
            CheckHiddenVersionMod.Checked = uiHidden.InstanceMod;
            CheckHiddenVersionResourcePack.Checked = uiHidden.InstanceResourcePack;
            CheckHiddenVersionShader.Checked = uiHidden.InstanceShader;
            CheckHiddenVersionSchematic.Checked = uiHidden.InstanceSchematic;
            CheckHiddenVersionServer.Checked = uiHidden.InstanceServer;

            // 特定功能
            CheckHiddenFunctionSelect.Checked = uiHidden.FunctionSelect;
            CheckHiddenFunctionModUpdate.Checked = uiHidden.FunctionModUpdate;
            CheckHiddenFunctionHidden.Checked = uiHidden.FunctionHidden;
        }
        catch (NullReferenceException ex)
        {
            ModBase.Log(
                ex,
                Lang.Text("Setup.Ui.Error.ConfigReset"),
                ModBase.LogLevel.Msgbox,
                userSummary: Lang.Text("Setup.Ui.Error.ConfigReset"));
            Reset();
        }
        catch (Exception ex)
        {
            ModBase.Log(
                ex,
                Lang.Text("Setup.Ui.Error.LoadFailed"),
                ModBase.LogLevel.Feedback,
                userSummary: Lang.Text("Setup.Ui.Error.LoadFailed"));
        }
    }

    // 初始化
    public void Reset()
    {
        try
        {
            Config.Preference.Reset();
            ModBase.Log("[Setup] 已初始化个性化设置！");
            HintService.Hint(Lang.Text("Setup.Ui.Initialized"), HintType.Success, false);
        }
        catch (Exception ex)
        {
            ModBase.Log(
                ex,
                Lang.Text("Setup.Ui.Error.InitFailed"),
                ModBase.LogLevel.Msgbox,
                userSummary: Lang.Text("Setup.Ui.Error.InitFailed"));
        }

        Reload();
    }

    // 将控件改变路由到设置改变
    private void SliderChange(object senderRaw, bool user)
    {
        var sender = (MySlider)senderRaw;
        if (ModAnimation.AniControlEnabled == 0)
        {
            var tag = sender.Tag?.ToString();
            SetByTag(tag, sender.Value);
            // 自定义主题参数：预览卡色带随滑条实时刷新
            if (tag is "UiLauncherHue" or "UiLauncherSat" or "UiLauncherLight" or "UiLauncherDelta" or
                "UiDarkHue" or "UiDarkSat" or "UiDarkLight" or "UiDarkDelta")
                RefreshCustomThemeCards();
        }
    }

    private void ComboChange(object senderRaw, SelectionChangedEventArgs e)
    {
        var sender = (MyComboBox)senderRaw;
        if (ModAnimation.AniControlEnabled == 0)
            SetByTag(sender.Tag?.ToString(), sender.SelectedIndex);
    }

    private void CheckBoxChange(object senderRaw, bool user)
    {
        var sender = (MyCheckBox)senderRaw;
        if (ModAnimation.AniControlEnabled == 0)
            SetByTag(sender.Tag?.ToString(), sender.Checked);
    }

    private void TextBoxChange(object senderRaw, RoutedEventArgs e)
    {
        var sender = (MyTextBox)senderRaw;
        if (ModAnimation.AniControlEnabled == 0)
            SetByTag(sender.Tag?.ToString(), sender.Text);
    }

    private void RadioBoxChange(object senderRaw, ModBase.RouteEventArgs e)
    {
        var sender = (MyRadioBox)senderRaw;
        var gotCfg = sender.Tag?.ToString()?.Split("/") ?? Array.Empty<string>();
        if (ModAnimation.AniControlEnabled == 0 && gotCfg.Length >= 2)
            SetByTag(gotCfg[0], int.Parse(gotCfg[1]));
    }

    private static void SetByTag(string tag, object value)
    {
        ConfigService.TrySetValue(tag, value);
    }

    private void ComboFontChange(object sender, SelectionChangedEventArgs e)
    {
        if (ModAnimation.AniControlEnabled == 0) Config.Preference.Font = ComboUiFont.SelectedFontTag;
    }

    private void ComboMotdFontChange(object sender, SelectionChangedEventArgs e)
    {
        if (ModAnimation.AniControlEnabled == 0) Config.Preference.MotdFont = ComboUiMotdFont.SelectedFontTag;
    }

    // 背景图片
    private void BtnUIBgOpen_Click(object sender, MouseButtonEventArgs e)
    {
        ModBase.OpenExplorer(ModBase.exePath + @"PCL\Pictures\");
    }

    private void BtnBackgroundRefresh_Click(object sender, MouseButtonEventArgs e)
    {
        BackgroundRefresh(true, true);
    }

    public void BackgroundRefreshUI(bool show, int count)
    {
        if (PanBackgroundOpacity is null)
            return;
        if (show)
        {
            PanBackgroundOpacity.Visibility = Visibility.Visible;
            PanBackgroundBlur.Visibility = Visibility.Visible;
            PanBackgroundSuit.Visibility = Visibility.Visible;
            BtnBackgroundClear.Visibility = Visibility.Visible;
            CheckAutoPauseVideo.Visibility = Visibility.Visible;
            CardBackground.Title = Lang.Text("Setup.Ui.Background.TitleWithCount", count);
        }
        else
        {
            PanBackgroundOpacity.Visibility = Visibility.Collapsed;
            PanBackgroundBlur.Visibility = Visibility.Collapsed;
            PanBackgroundSuit.Visibility = Visibility.Collapsed;
            BtnBackgroundClear.Visibility = Visibility.Collapsed;
            CheckAutoPauseVideo.Visibility = Visibility.Collapsed;
            CardBackground.Title = Lang.Text("Setup.Ui.Background.TitleDefault");
        }

        CardBackground.TriggerForceResize();
    }

    private void BtnBackgroundClear_Click(object sender, MouseButtonEventArgs e)
    {
        if (ModMain.MyMsgBox(Lang.Text("Setup.Ui.Background.Clear.Confirm.Message"),
                Lang.Text("Common.Dialog.Warning"), button2: Lang.Text("Common.Action.Cancel"),
                isWarn: true) == 1)
        {
            ModBase.DeleteDirectory(ModBase.exePath + @"PCL\Pictures");
            BackgroundRefresh(false, true);
            HintService.Hint(Lang.Text("Setup.Ui.Background.Clear.Success"), HintType.Success);
        }
    }

    /// <summary>
    ///     刷新背景图片及设置页 UI。
    /// </summary>
    /// <param name="isHint">是否显示刷新提示。</param>
    /// <param name="refresh">是否刷新图片显示。</param>
    public static void BackgroundRefresh(bool isHint, bool refresh)
    {
        try
        {
            // 获取可用的图片文件
            Directory.CreateDirectory(ModBase.exePath + @"PCL\Pictures\");
            var pic = ModBase.EnumerateFiles(ModBase.exePath + @"PCL\Pictures\").Where(file =>
                    !(file.Extension.Equals(".ini", StringComparison.OrdinalIgnoreCase) ||
                      file.Extension.Equals(".db", StringComparison.OrdinalIgnoreCase))).Select(file => file.FullName)
                .ToList();

            // 视频加载异常处理

            EventHandler<ExceptionRoutedEventArgs> videoHandler = (sender, e) =>
            {
                var videoEx = e.ErrorException;
                var videoAddress = ModMain.frmMain.VideoBack.Source.ToString();
                if (ModMain.frmMain.VideoBack.Source is not null)
                {
                    ModVideoBack.VideoStop();

                    if (videoEx.Message.Contains("0xC00D109B"))
                        ModBase.Log(
                            $"""
                             刷新背景内容失败，该视频文件可能并非 H.264（AVC）格式。
                             你可以尝试使用视频转码工具打开视频文件并设定目标格式为 H.264（AVC），然后转码该视频。
                             文件：{videoAddress}
                             """,
                            ModBase.LogLevel.Msgbox,
                            userSummary: Lang.Text("Setup.Ui.Error.BackgroundVideoUnsupported"));
                    else
                        ModBase.Log(
                            videoEx,
                            $"刷新背景内容失败（{videoAddress}）",
                            ModBase.LogLevel.Msgbox,
                            userSummary: Lang.Text("Setup.Ui.Error.OperationFailed"));
                }
            };
            ModMain.frmMain.VideoBack.MediaFailed -= videoHandler;
            ModVideoBack.GamingStateChanged -= ModVideoBack.OnGamingStateChanged;
            ModVideoBack.ForcePlayChanged -= ModVideoBack.OnForcePlayChanged;
            ModVideoBack.GamingStateChanged += ModVideoBack.OnGamingStateChanged;
            ModVideoBack.ForcePlayChanged += ModVideoBack.OnForcePlayChanged;
            if (!Config.Preference.Background.AutoPauseVideo)
                ModVideoBack.ForcePlay = true;
            // 加载
            if (pic.Count == 0)
            {
                if (refresh)
                {
                    if (ModMain.frmMain.ImgBack.Visibility == Visibility.Collapsed)
                    {
                        if (isHint)
                            HintService.Hint(Lang.Text("Setup.Ui.Background.NoAvailableContent"), HintType.Error);
                    }
                    else
                    {
                        ModMain.frmMain.ImgBack.Visibility = Visibility.Collapsed;
                        if (isHint)
                            HintService.Hint(Lang.Text("Setup.Ui.Background.Cleared"), HintType.Success);
                    }
                }

                if (ModMain.frmSetupUI is not null)
                    ModMain.frmSetupUI.BackgroundRefreshUI(false, 0);
            }
            else
            {
                if (refresh)
                {
                    var address = RandomUtils.PickRandom(pic);
                    try
                    {
                        ModMain.frmMain.ImgBack.Background = null;
                        ModVideoBack.VideoStop();
                        ModBase.Log("[UI] 加载背景内容：" + address);
                        ModMain.frmMain.ImgBack.Background = new MyBitmap(address);
                        _ = Config.Preference.Background.WallpaperSuitMode;
                        ModMain.frmMain.ImgBack.Visibility = Visibility.Visible;
                        if (isHint)
                                HintService.Hint(Lang.Text("Setup.Ui.Background.Refresh.Success", ModBase.GetFileNameFromPath(address)), HintType.Success,
                                false);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            ModMain.frmMain.VideoBack.MediaFailed += videoHandler;
                            ModBase.Log(ex, "[UI] 加载背景图片失败" + address);
                            if (ModBase.modeDebug)
                                HintService.Hint(Lang.Text("Setup.Ui.Background.ImageLoadFailed", address));
                            ModMain.frmMain.ImgBack.Visibility = Visibility.Visible;
                            ModMain.frmMain.VideoBack.Source = new Uri(address, UriKind.Absolute);
                            ModVideoBack.VideoPlay();
                            if (isHint)
                            HintService.Hint(Lang.Text("Setup.Ui.Background.Refresh.Success", ModBase.GetFileNameFromPath(address)), HintType.Success,
                                    false);
                        }
                        catch (Exception playEx)
                        {
                            ModBase.Log(playEx, "播放背景内容时出现未知错误：");
                        }
                    }
                }

                if (ModMain.frmSetupUI is not null)
                    ModMain.frmSetupUI.BackgroundRefreshUI(true, pic.Count);
            }
        }

        catch (Exception ex)
        {
            ModBase.Log(
                ex,
                "刷新背景内容时出现未知错误",
                ModBase.LogLevel.Feedback,
                userSummary: Lang.Text("Setup.Ui.Error.OperationFailed"));
        }
    }

    // 顶部栏
    private void BtnLogoChange_Click(object sender, MouseButtonEventArgs e)
    {
        var fileName = SystemDialogs.SelectFile(
            Lang.Text("Setup.Ui.ImageFile.Filter"),
            Lang.Text("Setup.Ui.ImageFile.SelectTitle"));
        if (string.IsNullOrEmpty(fileName))
            return;
        try
        {
            // 拷贝文件
            File.Delete(ModBase.exePath + @"PCL\Logo.png");
            ModBase.CopyFile(fileName, ModBase.exePath + @"PCL\Logo.png");
            // 设置当前显示
            ModMain.frmMain.ImageTitleLogo.Source = null; // 防止因为 Source 属性前后的值相同而不更新 (#5628)
            ModMain.frmMain.ImageTitleLogo.Source = ModBase.exePath + @"PCL\Logo.png";
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("参数无效"))
                ModBase.Log(
                    """
                    改变标题栏图片失败，该图片文件可能并非标准格式。
                    你可以尝试使用画图打开该文件并重新保存，这会让图片变为标准格式。
                    """,
                    ModBase.LogLevel.Msgbox,
                    userSummary: Lang.Text("Setup.Ui.Error.TitleImageInvalidFormat"));
            else
                ModBase.Log(
                    ex,
                    "设置标题栏图片失败",
                    ModBase.LogLevel.Msgbox,
                    userSummary: Lang.Text("Setup.Ui.Error.OperationFailed"));
            ModMain.frmMain.ImageTitleLogo.Source = null;
        }
    }

    private void RadioLogoType3_Check(object sender, ModBase.RouteEventArgs e)
    {
        if (!(ModAnimation.AniControlEnabled == 0 && e.raiseByMouse))
            return;
        Refresh: ;

        // 已有图片则不再选择
        if (File.Exists(ModBase.exePath + @"PCL\Logo.png"))
        {
            try
            {
                ModMain.frmMain.ImageTitleLogo.Source = null; // 防止因为 Source 属性前后的值相同而不更新 (#5628)
                ModMain.frmMain.ImageTitleLogo.Source = ModBase.exePath + @"PCL\Logo.png";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("参数无效"))
                    ModBase.Log(
                        """
                        调整标题栏图片失败，该图片文件可能并非标准格式。
                        你可以尝试使用画图打开该文件并重新保存，这会让图片变为标准格式。
                        """,
                        ModBase.LogLevel.Msgbox,
                        userSummary: Lang.Text("Setup.Ui.Error.TitleImageResizeInvalidFormat"));
                else
                    ModBase.Log(
                        ex,
                        "调整标题栏图片失败",
                        ModBase.LogLevel.Msgbox,
                        userSummary: Lang.Text("Setup.Ui.Error.OperationFailed"));
                ModMain.frmMain.ImageTitleLogo.Source = null;
                e.handled = true;
                try
                {
                    File.Delete(ModBase.exePath + @"PCL\Logo.png");
                }
                catch (Exception exx)
                {
                    ModBase.Log(
                        exx,
                        "清理错误的标题栏图片失败",
                        ModBase.LogLevel.Msgbox,
                        userSummary: Lang.Text("Setup.Ui.Error.OperationFailed"));
                }
            }

            return;
        }

        // 没有图片则要求选择
        var fileName = SystemDialogs.SelectFile(Lang.Text("Setup.Ui.ImageFile.Filter"), Lang.Text("Setup.Ui.ImageFile.SelectTitle"));
        if (string.IsNullOrEmpty(fileName))
        {
            ModMain.frmMain.ImageTitleLogo.Source = null;
            e.handled = true;
        }
        else
        {
            try
            {
                // 拷贝文件
                File.Delete(ModBase.exePath + @"PCL\Logo.png");
                ModBase.CopyFile(fileName, ModBase.exePath + @"PCL\Logo.png");
                goto Refresh;
            }
            catch (Exception ex)
            {
                ModBase.Log(
                    ex,
                    "复制标题栏图片失败",
                    ModBase.LogLevel.Msgbox,
                    userSummary: Lang.Text("Setup.Ui.Error.OperationFailed"));
            }
        }
    }

    private void BtnLogoDelete_Click(object sender, MouseButtonEventArgs e)
    {
        try
        {
            File.Delete(ModBase.exePath + @"PCL\Logo.png");
            RadioLogoType1.SetChecked(true, true);
            HintService.Hint(Lang.Text("Setup.Ui.Logo.Clear.Success"), HintType.Success);
        }
        catch (Exception ex)
        {
            ModBase.Log(
                ex,
                "清空标题栏图片失败",
                ModBase.LogLevel.Msgbox,
                userSummary: Lang.Text("Setup.Ui.Error.OperationFailed"));
        }
    }

    // 背景音乐
    private void BtnMusicOpen_Click(object sender, MouseButtonEventArgs e)
    {
        ModBase.OpenExplorer(ModBase.exePath + @"PCL\Musics\");
    }

    private void BtnMusicRefresh_Click(object sender, MouseButtonEventArgs e)
    {
        ModMusic.MusicRefreshPlay(true);
    }

    public void MusicRefreshUI()
    {
        if (PanBackgroundOpacity is null)
            return;
        if (ModMusic.musicAllList.Any())
        {
            PanMusicVolume.Visibility = Visibility.Visible;
            PanMusicDetail.Visibility = Visibility.Visible;
            BtnMusicClear.Visibility = Visibility.Visible;
            CardMusic.Title = Lang.Text("Setup.Ui.Music.TitleWithCount", ModBase.EnumerateFiles(ModBase.exePath + @"PCL\Musics\").Count());
        }
        else
        {
            PanMusicVolume.Visibility = Visibility.Collapsed;
            PanMusicDetail.Visibility = Visibility.Collapsed;
            BtnMusicClear.Visibility = Visibility.Collapsed;
            CardMusic.Title = Lang.Text("Setup.Ui.Music.Title");
        }

        CardMusic.TriggerForceResize();
    }

    private void BtnMusicClear_Click(object sender, MouseButtonEventArgs e)
    {
        if (ModMain.MyMsgBox(Lang.Text("Setup.Ui.Music.Clear.Confirm.Message"),
                Lang.Text("Common.Dialog.Warning"), button2: Lang.Text("Common.Action.Cancel"),
                isWarn: true) == 1)
            ModBase.RunInThread(() =>
            {
                HintService.Hint(Lang.Text("Setup.Ui.Music.Deleting"));
                // 停止播放音乐
                ModMusic.musicNAudio = null;
                ModMusic.musicWaitingList = new List<string>();
                ModMusic.musicAllList = new List<string>();
                Thread.Sleep(200);
                // 删除文件
                try
                {
                    ModBase.DeleteDirectory(ModBase.exePath + @"PCL\Musics");
                    // DisableSMTCSupport()
                    HintService.Hint(Lang.Text("Setup.Ui.Music.Delete.Success"), HintType.Success);
                }
                catch (Exception ex)
                {
                    ModBase.Log(
                        ex,
                        "删除背景音乐失败",
                        ModBase.LogLevel.Msgbox,
                        userSummary: Lang.Text("Setup.Ui.Error.OperationFailed"));
                }

                try
                {
                    Directory.CreateDirectory(ModBase.exePath + @"PCL\Musics");
                    ModBase.RunInUi(() => ModMusic.MusicRefreshPlay(false));
                }
                catch (Exception ex)
                {
                    ModBase.Log(
                        ex,
                        "重建背景音乐文件夹失败",
                        ModBase.LogLevel.Msgbox,
                        userSummary: Lang.Text("Setup.Ui.Error.OperationFailed"));
                }
            });
    }

    private void CheckMusicStart_Change(object sender, bool user)
    {
        CheckBoxChange(sender, user);
        if (ModAnimation.AniControlEnabled != 0)
            return;
        if (CheckMusicStart.Checked == true)
            CheckMusicStop.Checked = false;
    }

    private void CheckMusicStop_Change()
    {
        if (ModAnimation.AniControlEnabled != 0)
            return;
        if (CheckMusicStop.Checked == true)
            CheckMusicStart.Checked = false;
    }

    // 主页

    private void BtnCustomRefresh_Click(object sender, MouseButtonEventArgs e)
    {
        ModMain.frmLaunchRight.ForceRefresh();
        HintService.Hint(Lang.Text("Setup.Ui.Homepage.Refresh.Success"), HintType.Success);
    }

    private void BtnCustomTutorial_Click(object sender, MouseButtonEventArgs e)
    {
        ModBase.OpenWebsite("https://docs.pclc.cc/ce/customization/xaml-format");
    }

    // 主题预览卡
    private bool _themeCardsBuilt;
    private bool _customEditingDark;
    private readonly Dictionary<(ColorTheme Theme, bool IsDark), Border> _themeCards = [];

    /// <summary>构建浅色/深色两排主题预览卡（每排 5 张，色阶渐变 + 名称）。</summary>
    private void EnsureThemeCards()
    {
        if (_themeCardsBuilt) return;
        _themeCardsBuilt = true;
        BuildThemeCards(PanLightThemeCards, isDark: false);
        BuildThemeCards(PanDarkThemeCards, isDark: true);
    }

    private void BuildThemeCards(Panel host, bool isDark)
    {
        host.Children.Clear();
        for (var i = 0; i < ThemeColorValues.Length; i++)
            host.Children.Add(CreateThemeCard(ThemeColorValues[i], ThemeColors[i], isDark));
    }

    private Border CreateThemeCard(ColorTheme theme, string name, bool isDark)
    {
        var strip = new Border
        {
            Height = 24,
            CornerRadius = new CornerRadius(4, 4, 0, 0),
            Margin = new Thickness(6, 6, 6, 0),
            Background = BuildThemeStripBrush(theme, isDark),
        };
        var label = new TextBlock
        {
            Text = name,
            FontSize = 11,
            Foreground = (Brush)FindResource("ColorBrushGray5"),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(2, 4, 2, 5),
            TextTrimming = TextTrimming.CharacterEllipsis,
        };
        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(strip, 0);
        Grid.SetRow(label, 1);
        grid.Children.Add(strip);
        grid.Children.Add(label);
        var card = new Border
        {
            CornerRadius = new CornerRadius(6),
            BorderThickness = new Thickness(2),
            BorderBrush = Brushes.Transparent,
            Background = (Brush)FindResource("ColorBrushGray4"),
            Margin = new Thickness(3, 0, 3, 0),
            Cursor = Cursors.Hand,
            Child = grid,
        };
        _themeCards[(theme, isDark)] = card;
        card.MouseLeftButtonUp += (_, _) => ThemeCard_Click(theme, isDark);
        card.MouseEnter += (_, _) => ApplyThemeCardVisual(card, theme, isDark, true);
        card.MouseLeave += (_, _) => ApplyThemeCardVisual(card, theme, isDark, false);
        return card;
    }

    /// <summary>按主题与配色模式生成 8 档色阶渐变条（与 ThemeService 实际渲染同源）。</summary>
    private static Brush BuildThemeStripBrush(ColorTheme theme, bool isDark)
    {
        var colors = ThemeService.GetThemePreviewColors(theme, isDark);
        var brush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
        };
        for (var i = 0; i < colors.Length; i++)
            brush.GradientStops.Add(new GradientStop(colors[i], i / (double)(colors.Length - 1)));
        brush.Freeze();
        return brush;
    }

    private static bool IsThemeSelected(ColorTheme theme, bool isDark)
        => isDark
            ? Config.Preference.Theme.DarkColor == theme
            : Config.Preference.Theme.LightColor == theme;

    /// <summary>刷新全部预览卡的选中态与自定义卡色带（自定义参数随滑条实时变化）。</summary>
    private void RefreshThemeCards()
    {
        if (!_themeCardsBuilt) return;
        foreach (var ((theme, isDark), card) in _themeCards)
        {
            ApplyThemeCardVisual(card, theme, isDark, false);
            if (theme == ColorTheme.Custom)
            {
                var strip = FindThemeStrip(card);
                if (strip != null)
                    strip.Background = BuildThemeStripBrush(theme, isDark);
            }
        }
    }

    private void RefreshCustomThemeCards()
    {
        if (!_themeCardsBuilt) return;
        foreach (var isDark in new[] { false, true })
        {
            if (_themeCards.TryGetValue((ColorTheme.Custom, isDark), out var card))
            {
                var strip = FindThemeStrip(card);
                if (strip != null)
                    strip.Background = BuildThemeStripBrush(ColorTheme.Custom, isDark);
            }
        }
    }

    private static Border? FindThemeStrip(Border card)
        => (card.Child as Grid)?.Children.OfType<Border>().FirstOrDefault();

    private void ApplyThemeCardVisual(Border card, ColorTheme theme, bool isDark, bool hovered)
    {
        var selected = IsThemeSelected(theme, isDark);
        if (selected)
        {
            card.BorderBrush = new SolidColorBrush(ThemeService.GetThemePreviewColors(theme, isDark)[4]);
            card.Opacity = 1;
        }
        else
        {
            card.BorderBrush = hovered
                ? (Brush)FindResource("ColorBrushGray5")
                : Brushes.Transparent;
            card.Opacity = hovered ? 1 : 0.85;
        }
    }

    private void ThemeCard_Click(ColorTheme theme, bool isDark)
    {
        if (ModAnimation.AniControlEnabled != 0) return;
        if (isDark)
            Config.Preference.Theme.DarkColor = theme;
        else
            Config.Preference.Theme.LightColor = theme;
        // 配置事件负责当前模式的配色资源；ThemeRefresh 兜底灰阶缓存与彩色背景渐变
        ThemeManager.ThemeRefresh();
        RefreshThemeCards();
        UpdateCustomThemePanel();
    }

    /// <summary>自定义参数编辑目标（浅色/深色）切换：滑条 Tag 与显示值随目标重载。</summary>
    private void CustomTarget_Change(object senderRaw, SelectionChangedEventArgs e)
    {
        if (ModAnimation.AniControlEnabled != 0) return;
        if (ComboCustomTarget.SelectedIndex < 0) return;
        _customEditingDark = ComboCustomTarget.SelectedIndex == 1;
        LoadCustomSliders();
    }

    private void LoadCustomSliders()
    {
        ModAnimation.AniControlEnabled += 1;
        var t = Config.Preference.Theme;
        SliderCustomHue.Tag = _customEditingDark ? "UiDarkHue" : "UiLauncherHue";
        SliderCustomSaturation.Tag = _customEditingDark ? "UiDarkSat" : "UiLauncherSat";
        SliderCustomLightness.Tag = _customEditingDark ? "UiDarkLight" : "UiLauncherLight";
        SliderCustomDelta.Tag = _customEditingDark ? "UiDarkDelta" : "UiLauncherDelta";
        SliderCustomHue.Value = _customEditingDark ? t.WindowDarkHue : t.WindowHue;
        SliderCustomSaturation.Value = _customEditingDark ? t.WindowDarkSat : t.WindowSat;
        SliderCustomLightness.Value = _customEditingDark ? t.WindowDarkLight : t.WindowLight;
        SliderCustomDelta.Value = _customEditingDark ? t.WindowDarkDelta : t.WindowDelta;
        ModAnimation.AniControlEnabled -= 1;
    }

    private void UpdateCustomThemePanel()
    {
        if (PanCustomTheme is null) return;
        var isCustom = Config.Preference.Theme.LightColor == ColorTheme.Custom ||
                       Config.Preference.Theme.DarkColor == ColorTheme.Custom;
        PanCustomTheme.Visibility = isCustom ? Visibility.Visible : Visibility.Collapsed;
    }

    // 主题预设：导入 / 导出
    private sealed class ThemePresetParams
    {
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Lightness { get; set; }
        public int Delta { get; set; }
    }

    private sealed class ThemePreset
    {
        public string Format { get; set; } = "";
        public int Version { get; set; }
        public string LightColor { get; set; } = "";
        public string DarkColor { get; set; } = "";
        public ThemePresetParams? Light { get; set; }
        public ThemePresetParams? Dark { get; set; }
    }

    private void BtnThemeExport_Click(object sender, MouseButtonEventArgs e)
    {
        var t = Config.Preference.Theme;
        var preset = new ThemePreset
        {
            Format = "pclcy-theme",
            Version = 1,
            LightColor = t.LightColor.ToString(),
            DarkColor = t.DarkColor.ToString(),
            Light = new ThemePresetParams
            {
                Hue = t.WindowHue, Saturation = t.WindowSat,
                Lightness = t.WindowLight, Delta = t.WindowDelta,
            },
            Dark = new ThemePresetParams
            {
                Hue = t.WindowDarkHue, Saturation = t.WindowDarkSat,
                Lightness = t.WindowDarkLight, Delta = t.WindowDarkDelta,
            },
        };
        var fileName = $"PCLCY-Theme-{preset.LightColor}-{preset.DarkColor}.pcltheme.json";
        var path = SystemDialogs.SelectSaveFile(
            Lang.Text("Setup.Ui.Theme.Preset.Export.Title"),
            fileName,
            Lang.Text("Setup.Ui.Theme.Preset.FileFilter"));
        if (string.IsNullOrEmpty(path)) return;
        try
        {
            File.WriteAllText(path, JsonSerializer.Serialize(preset, new JsonSerializerOptions { WriteIndented = true }));
            HintService.Hint(Lang.Text("Setup.Ui.Theme.Preset.Export.Success"), HintType.Success);
        }
        catch (Exception ex)
        {
            ModBase.Log(ex, "导出主题预设失败", ModBase.LogLevel.Msgbox,
                userSummary: Lang.Text("Setup.Ui.Theme.Preset.Export.Failed"));
        }
    }

    private void BtnThemeImport_Click(object sender, MouseButtonEventArgs e)
    {
        var path = SystemDialogs.SelectFile(
            Lang.Text("Setup.Ui.Theme.Preset.FileFilter"),
            Lang.Text("Setup.Ui.Theme.Preset.Import.Title"));
        if (string.IsNullOrEmpty(path)) return;
        try
        {
            var preset = JsonSerializer.Deserialize<ThemePreset>(File.ReadAllText(path));
            if (preset is null || preset.Format != "pclcy-theme")
                throw new JsonException("不是有效的 PCL CY Edition 主题预设文件");
            if (!Enum.TryParse(preset.LightColor, ignoreCase: true, out ColorTheme light) ||
                !Enum.TryParse(preset.DarkColor, ignoreCase: true, out ColorTheme dark))
                throw new JsonException("预设中的主题名称无法识别");
            var t = Config.Preference.Theme;
            t.LightColor = light;
            t.DarkColor = dark;
            if (preset.Light != null)
            {
                t.WindowHue = Math.Clamp(preset.Light.Hue, 0, 360);
                t.WindowSat = Math.Clamp(preset.Light.Saturation, 0, 100);
                t.WindowLight = Math.Clamp(preset.Light.Lightness, 0, 100);
                t.WindowDelta = Math.Clamp(preset.Light.Delta, 0, 180);
            }
            if (preset.Dark != null)
            {
                t.WindowDarkHue = Math.Clamp(preset.Dark.Hue, 0, 360);
                t.WindowDarkSat = Math.Clamp(preset.Dark.Saturation, 0, 100);
                t.WindowDarkLight = Math.Clamp(preset.Dark.Lightness, 0, 100);
                t.WindowDarkDelta = Math.Clamp(preset.Dark.Delta, 0, 180);
            }
            Reload();
            HintService.Hint(Lang.Text("Setup.Ui.Theme.Preset.Import.Success"), HintType.Success);
        }
        catch (Exception ex)
        {
            ModBase.Log(ex, "导入主题预设失败", ModBase.LogLevel.Msgbox,
                userSummary: Lang.Text("Setup.Ui.Theme.Preset.Import.Failed"));
        }
    }

    // 滑动条
    private void SliderLoad()
    {
        SliderMusicVolume.getHintText = new Func<object, object>(v =>
            Lang.Number(Math.Ceiling(Convert.ToDouble(v) * 0.1d) / 100d, "P0"));
        SliderLauncherOpacity.getHintText = new Func<object, object>(v =>
            Lang.Number(Math.Round(40 + Convert.ToDouble(v) * 0.1d) / 100d, "P0"));
        SliderBackgroundOpacity.getHintText = new Func<object, object>(v =>
            Lang.Number(Math.Round(Convert.ToDouble(v) * 0.1d) / 100d, "P0"));
        SliderBackgroundBlur.getHintText = new Func<object, object>(v => Lang.Text("Setup.Ui.Slider.Pixel", Lang.Number(Convert.ToDouble(v), "N0")));
        SliderBlurValue.getHintText = new Func<object, object>(v => Lang.Text("Setup.Ui.Slider.Pixel", Lang.Number(Convert.ToDouble(v), "N0")));
        SliderBlurSamplingRate.getHintText = new Func<object, object>(v => Lang.Number(Convert.ToDouble(v) / 100d, "P0"));
    }

    private void CheckMusicStart_OnChange(object sender, bool user)
    {
        CheckBoxChange(sender, user);
        CheckMusicStart_Change(sender, user);
    }

    private void CheckMusicStop_OnChange(object sender, bool user)
    {
        CheckBoxChange(sender, user);
        CheckMusicStop_Change();
    }

    #region 功能隐藏

    /// <summary>
    ///     是否强制显示被禁用的功能。
    /// </summary>
    public static bool HiddenForceShow
    {
        get => field;
        set
        {
            field = value;
            HiddenRefresh();
        }
    }

    /// <summary>
    ///     更新功能隐藏带来的显示变化。
    /// </summary>
    public static void HiddenRefresh()
    {
        if (ModMain.frmMain.PanTitleSelect is null || !ModMain.frmMain.PanTitleSelect.IsLoaded)
            return;
        try
        {
            // 获取配置组引用以缩短代码
            var conf = Config.Preference.Hide;

            // 顶部栏：下载、设置、工具可按配置隐藏；启动与关于常驻
            // （关于页承载上游署名与许可信息，保持可达；启动是主页面入口，隐藏会导致无法导航）
            ModMain.frmMain.PanTitleSelect.Visibility = Visibility.Visible;
            ModMain.frmMain.BtnTitleSelect1.Visibility = !HiddenForceShow && conf.PageDownload
                ? Visibility.Collapsed
                : Visibility.Visible;
            ModMain.frmMain.BtnTitleSelect2.Visibility =
                !HiddenForceShow && conf.PageSetup ? Visibility.Collapsed : Visibility.Visible;
            ModMain.frmMain.BtnTitleSelect3.Visibility =
                !HiddenForceShow && conf.PageTools ? Visibility.Collapsed : Visibility.Visible;

            // 功能隐藏设置卡片
            if (ModMain.frmSetupUI is not null)
            {
                ModMain.frmSetupUI.CardSwitch.Visibility = !HiddenForceShow && conf.FunctionHidden
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                ModMain.frmSetupUI.CardSwitch.Title = HiddenForceShow ? Lang.Text("Setup.Ui.FeatureHide.TitleTemporarilyDisabled") : Lang.Text("Setup.Ui.FeatureHide.Title");
            }

            // 设置子页面 (FrmSetupLeft)
            if (ModMain.frmSetupLeft is not null)
            {
                ModMain.frmSetupLeft.ItemLaunch.Visibility =
                    !HiddenForceShow && conf.SetupLaunch ? Visibility.Collapsed : Visibility.Visible;
                ModMain.frmSetupLeft.ItemUI.Visibility =
                    !HiddenForceShow && conf.SetupUi ? Visibility.Collapsed : Visibility.Visible;
                ModMain.frmSetupLeft.ItemLauncherLanguage.Visibility = !HiddenForceShow && conf.SetupLauncherLanguage
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                ModMain.frmSetupLeft.ItemGameManage.Visibility = !HiddenForceShow && conf.SetupGameManage
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                ModMain.frmSetupLeft.ItemLauncherMisc.Visibility = !HiddenForceShow && conf.SetupLauncherMisc
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                ModMain.frmSetupLeft.ItemJava.Visibility =
                    !HiddenForceShow && conf.SetupJava ? Visibility.Collapsed : Visibility.Visible;
                ModMain.frmSetupLeft.ItemGameLink.Visibility = !HiddenForceShow && conf.SetupGameLink
                    ? Visibility.Collapsed
                    : Visibility.Visible;

                var categories = new[]
                {
                    (ModMain.frmSetupLeft.TextGameCategory,
                        !(conf.SetupLaunch && conf.SetupJava && conf.SetupGameManage)),
                    (ModMain.frmSetupLeft.TextToolsCategory, !conf.SetupGameLink),
                    (ModMain.frmSetupLeft.TextLauncherCategory, !(conf.SetupUi && conf.SetupLauncherLanguage && conf.SetupLauncherMisc))
                };

                foreach (var category in categories)
                {
                    var isVisible = category.Item2 || HiddenForceShow;
                    category.Item1.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                    if (isVisible)
                        category.Item1.Opacity = 0.6d;
                }

                // 统计设置页可用项数量
                var setupCount = 0;
                if (!conf.SetupLaunch)
                    setupCount += 1;
                if (!conf.SetupUi)
                    setupCount += 1;
                if (!conf.SetupLauncherLanguage)
                    setupCount += 1;
                if (!conf.SetupGameManage)
                    setupCount += 1;
                if (!conf.SetupLauncherMisc)
                    setupCount += 1;
                if (!conf.SetupJava)
                    setupCount += 1;
                if (!conf.SetupGameLink)
                    setupCount += 1;
                ModMain.frmSetupLeft.PanItem.Visibility =
                    setupCount < 2 && !HiddenForceShow ? Visibility.Collapsed : Visibility.Visible;
            }

            // 关于子页面 (FrmAboutLeft)：PCL CY Edition 将关于分类独立为主导航页
            ModMain.frmAboutLeft?.RefreshHidden(HiddenForceShow);

            // 工具子页面 (FrmToolsLeft)
            if (ModMain.frmToolsLeft is not null)
            {
                ModMain.frmToolsLeft.ItemGameLink.Visibility = !HiddenForceShow && conf.ToolsGameLink
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                ModMain.frmToolsLeft.ItemTest.Visibility =
                    !HiddenForceShow && conf.ToolsTest ? Visibility.Collapsed : Visibility.Visible;
                
                // 处理分类标题
                var isGameLinkVisible = (!HiddenForceShow && !conf.ToolsGameLink) || HiddenForceShow;
                ModMain.frmToolsLeft.TextGameLinkCategory.Visibility = isGameLinkVisible ? Visibility.Visible : Visibility.Collapsed;
                if (isGameLinkVisible) ModMain.frmToolsLeft.TextGameLinkCategory.Opacity = 0.6;

                var isToolsVisible = (!HiddenForceShow && !conf.ToolsTest) || HiddenForceShow;
                ModMain.frmToolsLeft.TextToolsCategory.Visibility = isToolsVisible ? Visibility.Visible : Visibility.Collapsed;
                if (isToolsVisible) ModMain.frmToolsLeft.TextToolsCategory.Opacity = 0.6;
                
                // 统计工具页可用项数量
                var toolsCount = 0;
                if (!conf.ToolsGameLink)
                    toolsCount += 1;
                if (!conf.ToolsTest)
                    toolsCount += 1;
                ModMain.frmToolsLeft.PanItem.Visibility =
                    toolsCount < 2 && !HiddenForceShow ? Visibility.Collapsed : Visibility.Visible;
            }

            // 其他入口刷新
            if (ModMain.frmMain.pageCurrent == FormMain.PageType.InstanceSelect)
                ModMain.frmSelectRight.BtnEmptyDownload_Loaded();
            if (ModMain.frmMain.pageCurrent == FormMain.PageType.Launch)
                ModMain.frmLaunchLeft.RefreshButtonsUI();
            if (ModMain.frmMain.pageCurrent == FormMain.PageType.InstanceSetup &&
                ModMain.frmInstanceModDisabled is not null)
                ModMain.frmInstanceModDisabled.BtnDownload_Loaded();
        }

        catch (Exception ex)
        {
            ModBase.Log(
                ex,
                "刷新功能隐藏项目失败",
                ModBase.LogLevel.Feedback,
                userSummary: Lang.Text("Setup.Ui.Error.OperationFailed"));
        }
    }

    // ================= 设置页面协同 =================
    private void HiddenSetupMain()
    {
        var isChecked = (bool)CheckHiddenPageSetup.Checked;
        CheckHiddenSetupLaunch.Checked = isChecked;
        CheckHiddenSetupUI.Checked = isChecked;
        CheckHiddenSetupLauncherLanguage.Checked = isChecked;
        CheckHiddenSetupGameManage.Checked = isChecked;
        CheckHiddenLauncherMisc.Checked = isChecked;
        CheckHiddenSetupJava.Checked = isChecked;
        CheckHiddenSetupUpdate.Checked = isChecked;
        CheckHiddenSetupGameLink.Checked = isChecked;
        CheckHiddenSetupAbout.Checked = isChecked;
        CheckHiddenSetupFeedback.Checked = isChecked;
        CheckHiddenSetupLog.Checked = isChecked;
    }

    // ================= 设置页面协同 =================
    private void HiddenSetupMain(object sender, bool user)
    {
        if (!user)
            return; // 仅处理用户点击，防止死循环
        var isChecked = (bool)CheckHiddenPageSetup.Checked;
        CheckHiddenSetupLaunch.Checked = isChecked;
        CheckHiddenSetupUI.Checked = isChecked;
        CheckHiddenSetupLauncherLanguage.Checked = isChecked;
        CheckHiddenSetupGameManage.Checked = isChecked;
        CheckHiddenLauncherMisc.Checked = isChecked;
        CheckHiddenSetupJava.Checked = isChecked;
        CheckHiddenSetupUpdate.Checked = isChecked;
        CheckHiddenSetupGameLink.Checked = isChecked;
        CheckHiddenSetupAbout.Checked = isChecked;
        CheckHiddenSetupFeedback.Checked = isChecked;
        CheckHiddenSetupLog.Checked = isChecked;
    }

    private void HiddenSetupSub(object sender, bool user)
    {
        if (!user)
            return;
        var conf = Config.Preference.Hide;
        // 判断是否全部勾选
        var allChecked = conf.SetupLaunch && conf.SetupUi && conf.SetupLauncherLanguage && conf.SetupJava &&
                         conf.SetupUpdate && conf.SetupGameLink && conf.SetupAbout && conf.SetupFeedback &&
                         conf.SetupLog && conf.SetupLauncherMisc && conf.SetupGameManage;
        CheckHiddenPageSetup.Checked = allChecked;
    }

    // ================= 工具页面协同 =================
    private void HiddenToolsMain(object sender, bool user)
    {
        if (!user)
            return;
        var isChecked = (bool)CheckHiddenPageTools.Checked;
        CheckHiddenToolsGameLink.Checked = isChecked;
        CheckHiddenToolsTest.Checked = isChecked;
    }

    private void HiddenToolsSub(object sender, bool user)
    {
        if (!user)
            return;
        var conf = Config.Preference.Hide;
        var allChecked = conf.ToolsGameLink && conf.ToolsTest;
        CheckHiddenPageTools.Checked = allChecked;
    }

    // 警告提示
    private void HiddenHint(object sender, bool user)
    {
        if (ModAnimation.AniControlEnabled == 0 && sender is MyCheckBox checkBox && checkBox.Checked == true)
            HintService.Hint(Lang.Text("Setup.Ui.FeatureHide.TemporaryHint"));
    }

    #endregion
}
