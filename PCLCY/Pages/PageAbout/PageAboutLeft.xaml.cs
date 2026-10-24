using System.Windows;
using System.Windows.Controls;
using PCL.Core.App;
using PCL.Core.App.Localization;

namespace PCL;

/// <summary>
///     关于页左栏。PCL CY Edition 将原设置页"关于"分类下的四个子模块
///     （软件信息/查看日志/反馈/软件更新）独立为主导航"关于"页的左栏。
///     子页面实例与设置页共享（ModMain.frmSetup*），页面 Tag 值沿用 PageSubType 枚举。
/// </summary>
public partial class PageAboutLeft
{
    private bool isLoad;
    private bool isPageSwitched; // 如果在 Loaded 前切换到其他页面，会导致触发 Loaded 时再次切换一次

    private void PageAboutLeft_Loaded(object sender, RoutedEventArgs e)
    {
        // 同步隐藏配置（与设置页"个性化-功能隐藏"一致，修复首次进入关于页不刷新可见性的遗漏）
        RefreshHidden(PageSetupUI.HiddenForceShow);
        if (isLoad)
            return;
        isLoad = true;
        if (isPageSwitched)
            return;
        // 选择第一个未被隐藏的子页面
        var hideCfg = Config.Preference.Hide;
        if (!hideCfg.SetupAbout)
            ItemAbout.SetChecked(true, false, false);
        else if (!hideCfg.SetupUpdate)
            ItemUpdate.SetChecked(true, false, false);
        else if (!hideCfg.SetupFeedback)
            ItemFeedback.SetChecked(true, false, false);
        else if (!hideCfg.SetupLog)
            ItemLog.SetChecked(true, false, false);
        else
            ItemAbout.SetChecked(true, false, false);
    }

    /// <summary>
    ///     按隐藏配置刷新四个子项与分类标题的可见性（供本页加载与设置页 HiddenRefresh 调用）。
    /// </summary>
    public void RefreshHidden(bool forceShow)
    {
        var conf = Config.Preference.Hide;
        ItemAbout.Visibility = !forceShow && conf.SetupAbout ? Visibility.Collapsed : Visibility.Visible;
        ItemUpdate.Visibility = !forceShow && conf.SetupUpdate ? Visibility.Collapsed : Visibility.Visible;
        ItemFeedback.Visibility = !forceShow && conf.SetupFeedback ? Visibility.Collapsed : Visibility.Visible;
        ItemLog.Visibility = !forceShow && conf.SetupLog ? Visibility.Collapsed : Visibility.Visible;

        var aboutVisible = !(conf.SetupAbout && conf.SetupUpdate && conf.SetupFeedback && conf.SetupLog) || forceShow;
        TextAboutCategory.Visibility = aboutVisible ? Visibility.Visible : Visibility.Collapsed;
        if (aboutVisible)
            TextAboutCategory.Opacity = 0.6d;

        var aboutCount = 0;
        if (!conf.SetupAbout)
            aboutCount += 1;
        if (!conf.SetupUpdate)
            aboutCount += 1;
        if (!conf.SetupFeedback)
            aboutCount += 1;
        if (!conf.SetupLog)
            aboutCount += 1;
        PanItem.Visibility = aboutCount < 2 && !forceShow ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PageAboutLeft_Unloaded(object sender, RoutedEventArgs e)
    {
        isPageSwitched = false;
    }

    public static void TryFeedback() // Handles ItemFeedback.Click
    {
        ModBase.RunInNewThread(() =>
        {
            if (!ModBase.CanFeedback(true))
                return;
            switch (ModMain.MyMsgBox(Lang.Text("Setup.Left.Feedback.Message"), Lang.Text("Setup.Left.Feedback.Title"),
                        Lang.Text("Setup.Left.Feedback.SubmitNew"), Lang.Text("Setup.Left.Feedback.ViewList"), Lang.Text("Common.Action.Cancel")))
            {
                case 1:
                {
                    ModBase.Feedback();
                    break;
                }
                case 2:
                {
                    ModBase.OpenWebsite("https://github.com/chenfengyimei/PCLCY/issues/");
                    break;
                }
            }
        });
    }

    public void Refresh(object sender, EventArgs e) // 由边栏按钮匿名调用
    {
        switch (ModBase.Val(((MyIconButton)sender).Tag))
        {
            case (double)FormMain.PageSubType.SetupFeedback:
            {
                if (ModMain.frmSetupFeedback is not null) ModMain.frmSetupFeedback.Loader.Start(isForceRestart: true);
                ItemFeedback.Checked = true;
                break;
            }
        }

        HintService.Hint(Lang.Text("Setup.Left.Refreshing"), log: false);
    }

    #region 页面切换

    /// <summary>
    ///     当前页面的编号。
    /// </summary>
    public FormMain.PageSubType pageID;

    public PageAboutLeft()
    {
        InitializeComponent();
        // 与 Loaded 中保持一致的默认子页面选择
        var hideCfg = Config.Preference.Hide;
        if (!hideCfg.SetupAbout)
            pageID = FormMain.PageSubType.SetupAbout;
        else if (!hideCfg.SetupUpdate)
            pageID = FormMain.PageSubType.SetupUpdate;
        else if (!hideCfg.SetupFeedback)
            pageID = FormMain.PageSubType.SetupFeedback;
        else if (!hideCfg.SetupLog)
            pageID = FormMain.PageSubType.SetupLog;
        else
            pageID = FormMain.PageSubType.SetupAbout;
        AnimatedControl = PanItem;
        Loaded += PageAboutLeft_Loaded;
        Unloaded += PageAboutLeft_Unloaded;
    }

    /// <summary>
    ///     勾选事件改变页面。
    /// </summary>
    private void PageCheck(object senderRaw, ModBase.RouteEventArgs e)
    {
        var sender = (MyListItem)senderRaw;
        if (sender.Tag is not null)
            PageChange((FormMain.PageSubType)ModBase.Val(sender.Tag));
    }

    /// <summary>
    ///     获取当前导航指定的右页面。
    /// </summary>
    public object PageGet(FormMain.PageSubType? id = null)
    {
        var targetID = id ?? pageID;
        switch (targetID)
        {
            case FormMain.PageSubType.SetupAbout:
            {
                ModMain.frmSetupAbout ??= new PageSetupAbout();
                return ModMain.frmSetupAbout;
            }
            case FormMain.PageSubType.SetupLog:
            {
                ModMain.frmSetupLog ??= new PageSetupLog();
                return ModMain.frmSetupLog;
            }
            case FormMain.PageSubType.SetupFeedback:
            {
                ModMain.frmSetupFeedback ??= new PageSetupFeedback();
                return ModMain.frmSetupFeedback;
            }
            case FormMain.PageSubType.SetupUpdate:
            {
                ModMain.frmSetupUpdate ??= new PageSetupUpdate();
                return ModMain.frmSetupUpdate;
            }

            default:
            {
                throw new Exception("未知的关于子页面种类：" + (int)targetID);
            }
        }
    }

    /// <summary>
    ///     切换现有页面。
    /// </summary>
    public void PageChange(FormMain.PageSubType id)
    {
        if (pageID == id)
            return;
        ModAnimation.AniControlEnabled += 1;
        isPageSwitched = true;
        try
        {
            PageChangeRun((MyPageRight)PageGet(id));
            pageID = id;
        }
        catch (Exception ex)
        {
            ModBase.Log(
                ex,
                $"切换分页面失败（ID {(int)id}）",
                ModBase.LogLevel.Feedback,
                userSummary: Lang.Text("Setup.Error.OperationFailed"));
        }
        finally
        {
            ModAnimation.AniControlEnabled -= 1;
        }
    }

    private static void PageChangeRun(MyPageRight target)
    {
        ModAnimation.AniStop("FrmMain PageChangeRight"); // 停止主页面的右页面切换动画，防止它与本动画一起触发多次 PageOnEnter
        if (target.Parent is not null)
            target.SetValue(ContentPresenter.ContentProperty, null);
        ModMain.frmMain.pageRight = target;
        ((MyPageRight)ModMain.frmMain.PanMainRight.Child).PageOnExit();
        ModAnimation.AniStart(new[]
        {
            ModAnimation.AaCode(() =>
            {
                ((MyPageRight)ModMain.frmMain.PanMainRight.Child).PageOnForceExit();
                ModMain.frmMain.PanMainRight.Child = ModMain.frmMain.pageRight;
                ModMain.frmMain.pageRight.Opacity = 0d;
            }, 130),
            ModAnimation.AaCode(() =>
            {
                // 延迟触发页面通用动画，以使得在 Loaded 事件中加载的控件得以处理
                ModMain.frmMain.pageRight.Opacity = 1d;
                ModMain.frmMain.pageRight.PageOnEnter();
            }, 30, true)
        }, "PageLeft PageChange");
    }

    #endregion
}
