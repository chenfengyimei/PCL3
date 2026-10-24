using System;
using System.Collections.Generic;
using System.Linq;

namespace PCL.Core.App.IoC;

public static partial class DependencyGroups
{
    private static readonly AttributeTargets[] _TargetsToTry = [
        AttributeTargets.Method,
        AttributeTargets.Property,
        AttributeTargets.Class
    ];
    
    public static bool InvokeInjection(Delegate injection, string identifier, AttributeTargets targets)
    {
        if (!_GroupMap.TryGetValue(identifier, out var groups)) return false;
        var targetGroups = new List<DependencyGroup>();
        foreach (var targetToTry in _TargetsToTry.Where(t => targets.HasFlag(t)))
        {
            // 该目标的收集组不存在 = 声明了收集器但当前没有任何匹配项（空集），跳过而非失败，
            // 避免"Dependency injection failed"对合法空集的误报（如 rpc-property 暂无属性用法）
            if (groups.GetValueOrDefault(targetToTry) is not { } group) continue;
            targetGroups.Add(group);
        }
        foreach (var group in targetGroups) group.InvokeInjection(injection);
        return true;
    }
}
