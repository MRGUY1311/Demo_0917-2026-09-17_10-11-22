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
}
