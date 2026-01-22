using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Highlightable : MonoBehaviour
{
    [Header("Highlight")]
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Outline (mesh-based)")]
    [SerializeField] private float outlineWidth = 0.02f; // world units (vertex extrusion in object space)
    [SerializeField] private float outlineScale = 0.03f; // small uniform scale inflation for silhouette
    [SerializeField] private bool updateEveryFrame = false; // recalc skinned meshes each frame while highlighted

    [Header("Optional UI")]
    [SerializeField] private GameObject uiPanel; // assign the Canvas (or a child panel) to open on click

    [SerializeField] private string uiPanelTagFallback = "ConversationUI";
    private GameObject uiPanelFallback;

    private ConversationManager conversationManager;

    private Renderer[] _renderers;

    // Outline objects created per-mesh
    private readonly List<GameObject> _outlineGOs = new List<GameObject>();
    private readonly List<MeshRenderer> _outlineRenderers = new List<MeshRenderer>();
    private readonly List<MeshFilter> _outlineMeshFilters = new List<MeshFilter>();

    // For skinned meshes we keep the source skinned renderer and a baked mesh
    private readonly List<SkinnedMeshRenderer> _skinnedSources = new List<SkinnedMeshRenderer>();
    private readonly List<Mesh> _bakedMeshes = new List<Mesh>();

    private bool _isHighlighted;
    private const string OutlineShaderName = "Custom/OutlineUnlit";

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(includeInactive: true);

        conversationManager = FindFirstObjectByType<ConversationManager>();

        if (conversationManager == null)
            Debug.LogError("[Highlightable] ConversationManager not found in scene (or it's disabled).");

        if (uiPanel == null && !string.IsNullOrEmpty(uiPanelTagFallback))
        {
            uiPanelFallback = GameObject.FindGameObjectWithTag(uiPanelTagFallback);
            if (uiPanelFallback == null)
                Debug.LogWarning($"[Highlightable] uiPanel not assigned and no object found with tag '{uiPanelTagFallback}'.");
        }

        // Create outline duplicates for MeshFilters
        var meshFilters = GetComponentsInChildren<MeshFilter>(includeInactive: true);
        foreach (var mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;

            var go = new GameObject("Outline_Mesh_" + mf.name);
            // parent to the original mesh transform so local transforms match (handles per-mesh local offsets)
            go.transform.SetParent(mf.transform, worldPositionStays: false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            // apply small uniform inflation so thin surfaces get a visible rim
            go.transform.localScale = Vector3.one * (1.0f + outlineScale);

            var outMF = go.AddComponent<MeshFilter>();
            outMF.sharedMesh = mf.sharedMesh;

            var outMR = go.AddComponent<MeshRenderer>();
            outMR.material = CreateOutlineMaterial();
            outMR.shadowCastingMode = ShadowCastingMode.Off;
            outMR.receiveShadows = false;
            outMR.enabled = false;

            _outlineGOs.Add(go);
            _outlineMeshFilters.Add(outMF);
            _outlineRenderers.Add(outMR);
        }

        // Create outline duplicates for SkinnedMeshRenderers (we will bake their meshes)
        var skinned = GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
        foreach (var smr in skinned)
        {
            if (smr.sharedMesh == null) continue;

            var go = new GameObject("Outline_Skinned_" + smr.name);
            go.transform.SetParent(smr.transform, worldPositionStays: false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one * (1.0f + outlineScale);

            var outMF = go.AddComponent<MeshFilter>();
            var outMR = go.AddComponent<MeshRenderer>();
            outMR.material = CreateOutlineMaterial();
            outMR.shadowCastingMode = ShadowCastingMode.Off;
            outMR.receiveShadows = false;
            outMR.enabled = false;

            // prepare baked mesh
            var baked = new Mesh();
            smr.BakeMesh(baked);
            outMF.sharedMesh = baked;

            _outlineGOs.Add(go);
            _outlineMeshFilters.Add(outMF);
            _outlineRenderers.Add(outMR);

            _skinnedSources.Add(smr);
            _bakedMeshes.Add(baked);
        }

        // Ensure initial material properties
        UpdateOutlineProperties();
    }

    void OnValidate()
    {
        // update material properties in editor when values change
        UpdateOutlineProperties();

        // update scales of any existing outline GOs (editor)
        for (int i = 0; i < _outlineGOs.Count; i++)
        {
            var go = _outlineGOs[i];
            if (go != null)
                go.transform.localScale = Vector3.one * (1.0f + outlineScale);
        }
    }

    // Toggle highlight on/off. Uses mesh duplicates rendered with a small extrusion shader.
    public void SetHighlighted(bool on)
    {
        if (on == _isHighlighted) return;
        _isHighlighted = on;

        for (int i = 0; i < _outlineRenderers.Count; i++)
        {
            var r = _outlineRenderers[i];
            if (r != null)
                r.enabled = on;
        }

        if (on)
        {
            // Ensure skinned meshes are baked immediately
            BakeSkinnedMeshes();
            UpdateOutlineProperties();
        }
    }

    void Update()
    {
        if (_isHighlighted && updateEveryFrame)
        {
            // Re-bake skinned meshes each frame (if any) so outline follows animation
            BakeSkinnedMeshes();
        }
    }

    // Update material color/width properties for all outline renderers
    private void UpdateOutlineProperties()
    {
        for (int i = 0; i < _outlineRenderers.Count; i++)
        {
            var mat = _outlineRenderers[i]?.sharedMaterial;
            if (mat == null) continue;
            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", highlightColor);
            if (mat.HasProperty("_OutlineWidth"))
                mat.SetFloat("_OutlineWidth", outlineWidth);

            // Ensure it's drawn before normal geometry (so original mesh occludes the inflated backfaces)
            mat.renderQueue = (int)RenderQueue.Geometry - 1; // 1999
        }
    }

    // Bake skinned meshes into the corresponding mesh filters' shared meshes
    private void BakeSkinnedMeshes()
    {
        for (int i = 0; i < _skinnedSources.Count; i++)
        {
            var src = _skinnedSources[i];
            var baked = _bakedMeshes[i];
            if (src == null || baked == null) continue;
            // reuse the same Mesh instance to avoid allocations
            src.BakeMesh(baked);
            // MeshFilter already references this baked mesh
        }
    }

    // Called by the interactor when the object is clicked to open associated UI.
    public void OpenUI()
    {
        Debug.Log($"[Highlightable] OpenUI called on {gameObject.name}");

        var panel = uiPanel;

        if (panel == null)
        {
            Debug.Log($"[Highlightable] uiPanel is null, trying UIRegistry. Instance = {(UIRegistry.Instance == null ? "NULL" : "OK")}");
            panel = UIRegistry.Instance?.ConversationPanel;
        }

        Debug.Log($"[Highlightable] panel resolved = {(panel == null ? "NULL" : panel.name)}");

        if (panel != null)
            panel.SetActive(true);

        var actor = GetComponentInParent<PatientActor>();
        if (actor != null && actor.Patient != null && conversationManager != null)
            conversationManager.SetActivePatient(actor.Patient);
    }

    void OnDisable()
    {
        foreach (var r in _outlineRenderers)
            if (r != null)
                r.enabled = false;
    }

    private Material CreateOutlineMaterial()
    {
        var shader = Shader.Find(OutlineShaderName);
        if (shader == null)
        {
            // fallback to a simple sprite shader (doesn't support extrusion) — but primary shader is recommended.
            shader = Shader.Find("Sprites/Default");
        }

        var mat = new Material(shader);
        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", highlightColor);
        if (mat.HasProperty("_OutlineWidth"))
            mat.SetFloat("_OutlineWidth", outlineWidth);

        // ensure it's drawn before geometry so the original mesh reads depth and occludes center
        mat.renderQueue = (int)RenderQueue.Geometry - 1;
        return mat;
    }

    void OnDestroy()
    {
        // Clean up baked meshes we created for skinned renderers
        for (int i = 0; i < _bakedMeshes.Count; i++)
        {
            if (_bakedMeshes[i] != null)
            {
#if UNITY_EDITOR
                // avoid leaking in editor
                DestroyImmediate(_bakedMeshes[i]);
#else
                Destroy(_bakedMeshes[i]);
#endif
            }
        }
    }
}