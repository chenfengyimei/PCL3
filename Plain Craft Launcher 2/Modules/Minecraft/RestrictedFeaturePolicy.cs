namespace PCL;

/// <summary>
///     区域限制策略。PCL3 是完全开放的公开版本，不按国家或地区限制任何功能。
/// </summary>
internal static class RestrictedFeaturePolicy
{
    internal static bool IsRestricted => false;

    internal static bool Check(string feature, bool showHint = true)
    {
        _ = feature;
        _ = showHint;
        return true;
    }

    internal static void ThrowIfRestricted(string feature)
    {
        _ = feature;
    }
}
