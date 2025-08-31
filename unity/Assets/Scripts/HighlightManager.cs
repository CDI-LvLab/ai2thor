using UnityEngine;
using System.Collections.Generic;

public class HighlightManager : MonoBehaviour {
    [Header("Materials")]
    public Material maskMaterial;   // HighlightMask shader
    public Material postMaterial;   // HighlightPost shader

    [Header("Optional Debug")]
    [SerializeField] private UnityEngine.UI.RawImage debugMask;

    [Header("Highlight Settings")]
    [Tooltip("Temporary layer name used for highlighted objects.")]
    public string highlightLayerName = "Highlight";

    private Camera maskCam;
    private RenderTexture maskRT;
    private HashSet<Renderer> highlightedRenderers = new HashSet<Renderer>();

    void Start() {
        // Ensure mask camera is created after main camera exists
        InitializeMaskCamera(Camera.main);
    }

    /// <summary>
    /// Creates or updates the mask camera to match the given main camera
    /// </summary>
    public void InitializeMaskCamera(Camera mainCam) {
        if (mainCam == null) {
            Debug.LogError("HighlightManager: No main camera found!");
            return;
        }

        if (GameObject.Find("HighlightMaskCamera")) {
            return;
        }

        if (maskCam == null) {
            GameObject camObj = new GameObject("HighlightMaskCamera");
            camObj.transform.SetParent(mainCam.transform);
            camObj.transform.localPosition = Vector3.zero;
            camObj.transform.localRotation = Quaternion.identity;
            maskCam = camObj.AddComponent<Camera>();
        }

        maskCam.CopyFrom(mainCam);
        maskCam.clearFlags = CameraClearFlags.SolidColor;
        maskCam.backgroundColor = Color.black; // fully transparent
        maskCam.cullingMask = 0; // temporarily empty
        maskCam.depth = mainCam.depth + 1;
        maskCam.enabled = false;
        maskCam.allowHDR = false;
        maskCam.allowMSAA = false;

        // Create or resize maskRT
        if (maskRT == null || maskRT.width != Screen.width || maskRT.height != Screen.height) {
            if (maskRT != null) maskRT.Release();
            maskRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.R8);
        }

        if (postMaterial != null)
            postMaterial.SetTexture("_MaskTex", maskRT);
    }

    void LateUpdate() {
        // Sync mask camera with main camera transform (in case main camera moves)
        if (maskCam != null && Camera.main != null) {
            maskCam.transform.position = Camera.main.transform.position;
            maskCam.transform.rotation = Camera.main.transform.rotation;
            maskCam.fieldOfView = Camera.main.fieldOfView;
            maskCam.orthographic = Camera.main.orthographic;
            maskCam.orthographicSize = Camera.main.orthographicSize;
        }
    }

    /// <summary>
    /// Add a GameObject (and children) to the highlighted set
    /// </summary>
    public void Highlight(GameObject obj) {
        if (obj == null) return;
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
            highlightedRenderers.Add(r);
    }

    /// <summary>
    /// Remove a GameObject (and children) from the highlighted set
    /// </summary>
    public void Unhighlight(GameObject obj) {
        if (obj == null) return;
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
            highlightedRenderers.Remove(r);
    }

    /// <summary>
    /// Remove a GameObject (and children) from the highlighted set
    /// </summary>
    public void Clear() {
        highlightedRenderers.Clear();
    }

    /// <summary>
    /// Check if a single GameObject (includes all child renderers) is highlighted.
    /// </summary>
    public bool IsHighlighted(GameObject obj) {
        if (obj == null) return false;
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>()) {
            if (highlightedRenderers.Contains(r)) {
                return true;
            }
        }
        return false;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if (highlightedRenderers.Count == 0) {
            Graphics.Blit(src, dest);
            return;
        }

        int highlightLayer = LayerMask.NameToLayer(highlightLayerName);
        if (highlightLayer == -1) {
            Debug.LogError($"HighlightManager: Layer '{highlightLayerName}' does not exist!");
            Graphics.Blit(src, dest);
            return;
        }

        // Backup original layers
        Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();

        // Assign highlighted objects (and children) to temp layer
        foreach (var r in highlightedRenderers) {
            if (r == null) continue;
            AssignLayerRecursive(r.gameObject, highlightLayer, originalLayers);
        }

        Dictionary<Renderer, Material[]> originalMats = new Dictionary<Renderer, Material[]>();
        foreach (var r in highlightedRenderers) {
            originalMats[r] = r.sharedMaterials;
            Material[] mats = new Material[r.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = maskMaterial; // your flat unlit mask
            r.materials = mats;
        }

        // Configure mask camera to render only highlight layer
        maskCam.cullingMask = 1 << highlightLayer;
        maskCam.targetTexture = maskRT;
        maskCam.Render();

        foreach (var kvp in originalMats)
            kvp.Key.materials = kvp.Value;

        // Optional debug mask display
        if (debugMask != null)
            debugMask.texture = maskRT;

        // Restore original layers
        foreach (var kvp in originalLayers)
            kvp.Key.layer = kvp.Value;

        // Apply post-process overlay + outline
        postMaterial.SetTexture("_MainTex", src);
        postMaterial.SetTexture("_MaskTex", maskRT);
        Graphics.Blit(src, dest, postMaterial);
    }

    /// <summary>
    /// Recursively assign layer to a GameObject and all children, recording original layers
    /// </summary>
    private void AssignLayerRecursive(GameObject go, int layer, Dictionary<GameObject, int> originalLayers) {
        if (!originalLayers.ContainsKey(go))
            originalLayers.Add(go, go.layer);

        go.layer = layer;

        foreach (Transform child in go.transform)
            AssignLayerRecursive(child.gameObject, layer, originalLayers);
    }
}
