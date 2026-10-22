using System;
using PCL.Core.App;
using PCL.Core.App.Configuration;
using PCL.Core.App.IoC;

namespace PCL.Core.UI.Theme;

/// <summary>
/// 配色模式更改事件。
/// </summary>
/// <param name="isDarkMode">当前是否为深色模式</param>
/// <param name="theme">当前配色主题</param>
public delegate void ColorModeChangedEvent(bool isDarkMode, ColorTheme theme);

/// <summary>
/// 配色主题更改事件。
/// </summary>
/// <param name="theme">当前配色主题</param>
public delegate void ColorThemeChangedEvent(ColorTheme theme);

[LifecycleScope("theme", "主题", false)]
[LifecycleService(LifecycleState.WindowCreating)]
public sealed partial class ThemeService
{
    [AnyConfigItem<ToneProfileConfig>("UiToneProfiles", ConfigSource.Local)]
    public static partial ToneProfileConfig ToneProfiles { get; set; }

    [RegisterConfigEvent]
    public static ConfigEventRegistry OnColorModeConfigChanged => new(
        scope: Config.Preference.Theme.ColorModeConfig,
        trigger: ConfigEvent.Update,
        handler: _ => RefreshColorMode()
    );

    [RegisterConfigEvent]
    public static ConfigEventRegistry OnColorThemeConfigChanged => new(
        scope: [Config.Preference.Theme.DarkColorConfig, Config.Preference.Theme.LightColorConfig],
        trigger: ConfigEvent.Update,
        handler: e =>
        {
            // ignore no change or non-current color theme change
            if (Equals(e.OldValue, e.Value)) return;
            if (IsDarkMode) { if (e.Item == Config.Preference.Theme.LightColorConfig) return; }
            else { if (e.Item == Config.Preference.Theme.DarkColorConfig) return; }
            // trigger color refresh
            if (Lifecycle.CurrentState > LifecycleState.Loading)
            {
                Lifecycle.CurrentApplication.Dispatcher.BeginInvoke(() =>
                {
                    ApplyColorResources();
                    ColorThemeChanged?.Invoke(CurrentTheme);
                    _AprilFoolLogic();
                });
            }
        }
    );

    [RegisterConfigEvent]
    public static ConfigEventRegistry OnCustomThemeArgsConfigChanged => new(
        scope: [
            Config.Preference.Theme.WindowHueConfig,
            Config.Preference.Theme.WindowSatConfig,
            Config.Preference.Theme.WindowLightConfig,
            Config.Preference.Theme.WindowDeltaConfig
        ],
        trigger: ConfigEvent.Update,
        handler: e =>
        {
            if (Equals(e.OldValue, e.Value)) return;
            if (CurrentTheme != ColorTheme.Custom) return;
            if (Lifecycle.CurrentState > LifecycleState.Loading)
            {
                Lifecycle.CurrentApplication.Dispatcher.BeginInvoke(() =>
                {
                    ApplyColorResources();
                    ColorThemeChanged?.Invoke(CurrentTheme);
                });
            }
        }
    );

    [LifecycleStart]
    private static void _Start()
    {
        IsDarkMode = _IsDarkMode();
        _LogStatus();
        _RefreshAll();
    }

    private static void _LogStatus()
    {
        Context.Debug($"当前状态: {(IsDarkMode ? "Dark" : "Light")}, {CurrentTheme}");
    }

    private static bool _IsDarkMode() => Config.Preference.Theme.ColorMode switch
    {
        ColorMode.Light => false,
        ColorMode.Dark => true,
        ColorMode.System => SystemThemeHelper.IsSystemInDarkMode(),
        _ => false
    };

    /// <summary>
    /// 当前是否为深色主题。
    /// </summary>
    public static bool IsDarkMode { get; private set; }

    /// <summary>
    /// 配色模式更改事件。
    /// </summary>
    public static event ColorModeChangedEvent? ColorModeChanged;

    /// <summary>
    /// 配色主题更改事件。
    /// </summary>
    public static event ColorThemeChangedEvent? ColorThemeChanged;

    private static void _RefreshAll()
    {
        ApplyGrayResources();
        ApplyColorResources();
        ColorModeChanged?.Invoke(IsDarkMode, CurrentTheme);
        _AprilFoolLogic();
    }

    private static void _AprilFoolLogic()
    {
        // 愚人节切换窗口标题样式；PCL CY Edition 不再限制 HMCL 主题的使用时间
        if (Basics.IsAprilFool) Config.Preference.WindowTitleTypeConfig.TriggerEvent(ConfigEvent.Changed, null);
    }

    /// <summary>
    /// 刷新配色模式，若检测到当前配色模式有实际更改，则会触发主题刷新。
    /// </summary>
    public static void RefreshColorMode()
    {
        var isDarkMode = _IsDarkMode();
        if (IsDarkMode == isDarkMode) return;
        Context.Info("正在更改配色模式");
        IsDarkMode = isDarkMode;
        _LogStatus();
        if (Lifecycle.CurrentState > LifecycleState.Loading)
        {
            Lifecycle.CurrentApplication.Dispatcher.BeginInvoke(_RefreshAll);
        }
    }

    /// <summary>
    /// 当前使用的色彩属性。
    /// </summary>
    public static ToneProfile CurrentTone => IsDarkMode ? ToneProfiles.Dark : ToneProfiles.Light;

    /// <summary>
    /// 当前使用的主题色。
    /// </summary>
    public static ColorTheme CurrentTheme
    {
        get
        {
            var theme = Config.Preference.Theme;
            return IsDarkMode ? theme.DarkColor : theme.LightColor;
        }
        set
        {
            var theme = Config.Preference.Theme;
            var config = IsDarkMode ? theme.DarkColorConfig : theme.LightColorConfig;
            config.SetValue(value);
        }
    }

    /// <summary>
    /// 获取当前色彩主题对应的各种参数。
    /// </summary>
    public static (int Hue, double LightAdjust, double ChromaAdjust, int Delta) GetCurrentThemeArgs()
    {
        var theme = CurrentTheme;
        return theme switch
        {
            ColorTheme.SkyBlue => (235, 0.36, 0.2, 0),
            ColorTheme.CatBlue => (255, 0, -0.2, 0),
            ColorTheme.DeathBlue => (268, -0.05, -0.1, 0),
            ColorTheme.HmclBlue => (275, -0.03, -0.35, 0),
            ColorTheme.Custom => (
                ((Config.Preference.Theme.WindowHue % 360) + 360) % 360,
                (Math.Clamp(Config.Preference.Theme.WindowLight, 0, 100) - 20) / 100d,
                (Math.Clamp(Config.Preference.Theme.WindowSat, 0, 100) - 80) / 100d,
                Math.Clamp(Config.Preference.Theme.WindowDelta, 0, 180)),
#if DEBUG
            _ => ((int)theme, 0, 0, 0)
#else
            _ => throw new IndexOutOfRangeException($"Invalid theme index: {(int)theme}")
#endif
        };
    }

    private static double _AdjustLinear(double value, double adjustment)
    {
        if (adjustment == 0) return value;
        // 确保输入在合理范围内
        value = Math.Clamp(value, 0.0, 1.0);
        adjustment = Math.Clamp(adjustment, -1.0, 1.0);
        // 非对称线性插值
        return adjustment switch
        {
            > 0 => value + (1.0 - value) * adjustment,
            _ => value + value * adjustment
        };
    }

    private static CatColorResource[] _CalculateGrays(ToneProfile tone) => [
        LabColor.FromLch(tone.L1).ToCatColor("Gray1"),
        LabColor.FromLch(tone.L2).ToCatColor("Gray2"),
        LabColor.FromLch(tone.L3).ToCatColor("Gray3"),
        LabColor.FromLch(tone.L4).ToCatColor("Gray4"),
        LabColor.FromLch(tone.L5).ToCatColor("Gray5"),
        LabColor.FromLch(tone.L6).ToCatColor("Gray6"),
        LabColor.FromLch(tone.L7).ToCatColor("Gray7"),
        LabColor.FromLch(tone.L8).ToCatColor("Gray8"),
        LabColor.FromLch(tone.LWhite, alpha:tone.AHalfWhite).ToCatColor("HalfWhite", false),
        LabColor.FromLch(tone.LWhite, alpha:tone.ASemiWhite).ToCatColor("SemiWhite", false),
        LabColor.FromLch(tone.LWhite).ToCatColor("White", false),
        LabColor.FromLch(tone.LWhite, alpha:tone.ATransparent).ToCatColor("Transparent", false),
        LabColor.FromLch(tone.LBackground, alpha:tone.ABackground).ToCatColor("TransparentBackground", false),
        LabColor.FromLch(tone.LBackground).ToCatColor("Background", false),
        LabColor.FromLch(tone.LBackground, alpha:tone.AToolTip).ToCatColor("ToolTip", false),
        LabColor.FromLch(tone.L7, 0.25, 30, tone.AHalfTransparent).ToCatColor("RedBack", false),
        LabColor.FromLch(tone.LForeground).ToCatColor("Memory", false),
    ];

    private static CatColorResource[] _CalculateColors(ToneProfile tone,
        (int Hue, double LightAdjust, double ChromaAdjust, int Delta) args)
    {
        double Hue(double offset) => args.Hue + args.Delta * offset;
        return [
            LabColor.FromLch(_AdjustLinear(tone.L1, args.LightAdjust * 0.1), _AdjustLinear(tone.C1, args.ChromaAdjust * 0.25), Hue(-0.25)).ToCatColor("1"),
            LabColor.FromLch(_AdjustLinear(tone.L2, args.LightAdjust), _AdjustLinear(tone.C2, args.ChromaAdjust), Hue(-0.15)).ToCatColor("2"),
            LabColor.FromLch(_AdjustLinear(tone.L3, args.LightAdjust), _AdjustLinear(tone.C3, args.ChromaAdjust), Hue(-0.08)).ToCatColor("3"),
            LabColor.FromLch(_AdjustLinear(tone.L4, args.LightAdjust), _AdjustLinear(tone.C4, args.ChromaAdjust), Hue(0)).ToCatColor("4"),
            LabColor.FromLch(_AdjustLinear(tone.L5, args.LightAdjust), _AdjustLinear(tone.C5, args.ChromaAdjust), Hue(0)).ToCatColor("5"),
            LabColor.FromLch(_AdjustLinear(tone.L6, args.LightAdjust), _AdjustLinear(tone.C6, args.ChromaAdjust), Hue(0.08)).ToCatColor("6"),
            LabColor.FromLch(_AdjustLinear(tone.L7, args.LightAdjust), _AdjustLinear(tone.C7, args.ChromaAdjust), Hue(0.15)).ToCatColor("7"),
            LabColor.FromLch(_AdjustLinear(tone.L8, args.LightAdjust), _AdjustLinear(tone.C8, args.ChromaAdjust), Hue(0.25)).ToCatColor("8"),
            LabColor.FromLch(_AdjustLinear(tone.L8, args.LightAdjust), _AdjustLinear(tone.C8, args.ChromaAdjust), Hue(0.25), tone.ASemiTransparent).ToCatColor("SemiTransparent", false),
            LabColor.FromLch(_AdjustLinear(tone.L5, args.LightAdjust), _AdjustLinear(tone.C5, args.ChromaAdjust), Hue(0)).ToCatColor("Bg0"),
            LabColor.FromLch(_AdjustLinear(tone.L7, args.LightAdjust), _AdjustLinear(tone.C7, args.ChromaAdjust), Hue(0.15), tone.ASemiWhite).ToCatColor("Bg1"),
        ];
    }

    private static CatColorResource[] LightGrayCache { get => field ??= _CalculateGrays(ToneProfiles.Light); set; } = null!;

    private static CatColorResource[] DarkGrayCache { get => field ??= _CalculateGrays(ToneProfiles.Dark); set; } = null!;

    /// <summary>
    /// 清除灰度配色的计算缓存。
    /// </summary>
    public static void InvalidateGrayCache()
    {
        LightGrayCache = null!;
        DarkGrayCache = null!;
    }

    /// <summary>
    /// 应用灰度配色到 WPF 资源字典。
    /// </summary>
    public static void ApplyGrayResources()
    {
        var cache = IsDarkMode ? DarkGrayCache : LightGrayCache;
        foreach (var c in cache) c.Apply();
    }

    /// <summary>
    /// 应用彩色配色到 WPF 资源字典。
    /// </summary>
    public static void ApplyColorResources()
    {
        var colors = _CalculateColors(CurrentTone, GetCurrentThemeArgs());
        foreach (var c in colors) c.Apply();
    }
}
