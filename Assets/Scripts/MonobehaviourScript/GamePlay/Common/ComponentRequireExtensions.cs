using UnityEngine;

public static class ComponentRequireExtensions
{
    public static bool TryRequireComponent<T>(this Component owner, out T component)
        where T : Component
    {
        if (owner.TryGetComponent(out component))
            return true;

        Debug.LogError($"Require {typeof(T).Name}.", owner);
        return false;
    }

    // searchChildren 为 true 时，查找自身及后代节点上的组件。
    public static bool TryRequireComponent<T>(this Component owner, out T component,
        bool searchChildren, bool includeInactive = false)
        where T : Component
    {
        if (!searchChildren)
            return owner.TryRequireComponent(out component);

        component = owner.GetComponentInChildren<T>(includeInactive);
        if (component != null)
            return true;

        Debug.LogError($"Require {typeof(T).Name} on self or children.", owner);
        return false;
    }
}
