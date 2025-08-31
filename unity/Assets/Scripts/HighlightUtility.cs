using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Static helper for screen-space highlights using HighlightManager.
/// </summary>
public static class HighlightUtility {
    private static HighlightManager manager;

    /// <summary>
    /// Internal getter to find the manager in the scene.
    /// </summary>
    private static HighlightManager Manager {
        get {
            if (manager == null) {
                manager = GameObject.FindObjectOfType<HighlightManager>();
                if (manager == null)
                    Debug.LogError("HighlightManager not found in scene!");
            }
            return manager;
        }
    }

    /// <summary>
    /// Check if a single GameObject (includes all child renderers) is highlighted.
    /// </summary>
    public static bool IsHighlighted(GameObject obj) {
        if (obj == null || Manager == null) return false;
        return Manager.IsHighlighted(obj);
    }

    /// <summary>
    /// Highlight a single GameObject (includes all child renderers)
    /// </summary>
    public static void Highlight(GameObject obj) {
        if (obj == null || Manager == null) return;
        Manager.Highlight(obj);
    }

    /// <summary>
    /// Remove highlight from a single GameObject
    /// </summary>
    public static void Unhighlight(GameObject obj) {
        if (obj == null || Manager == null) return;
        Manager.Unhighlight(obj);
    }

    /// <summary>
    /// Highlight multiple GameObjects at once
    /// </summary>
    public static void Highlight(IEnumerable<GameObject> objects) {
        if (objects == null) return;
        foreach (var obj in objects) Highlight(obj);
    }

    /// <summary>
    /// Remove highlight from multiple GameObjects at once
    /// </summary>
    public static void Unhighlight(IEnumerable<GameObject> objects) {
        if (objects == null) return;
        foreach (var obj in objects) Unhighlight(obj);
    }

    public static void Clear() {
        if (Manager == null) return;
        Manager.Clear();
    }
}
