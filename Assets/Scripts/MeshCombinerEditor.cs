using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshCombinerEditor : EditorWindow
{
    // The parent GameObject (e.g. a prefab instance) containing all the modular pieces.
    GameObject sourceParent;

    // Dropdown of detected material groups.
    int selectedMaterialGroupIndex = 0;
    List<string> materialGroupKeys = new List<string>();

    // Overwrite confirmation.
    bool allowOverwrite = false;
    bool assetExists = false;

    // Inline status message.
    string statusMessage = "";

    // Cached asset name/path.
    string assetName = "";
    string assetPath = "";
    // Fixed save folder.
    string assetFolder = "Assets/CombinedMeshes/";

    // Flag to indicate that a combine operation has been completed.
    bool combineCompleted = false;

    [MenuItem("Tools/Mesh Combiner")]
    public static void ShowWindow()
    {
        GetWindow<MeshCombinerEditor>("Mesh Combiner");
    }

    void OnGUI()
    {
        GUILayout.Label("Mesh Combiner", EditorStyles.boldLabel);

        // Parent Object: defaults to the currently selected object.
        if (sourceParent == null)
            sourceParent = Selection.activeGameObject;
        sourceParent = (GameObject)EditorGUILayout.ObjectField("Parent Object", sourceParent, typeof(GameObject), true);

        if (sourceParent != null)
        {
            UpdateMaterialGroupKeys();

            if (materialGroupKeys.Count == 0)
            {
                EditorGUILayout.HelpBox("No MeshRenderers with materials were found in children.", MessageType.Warning);
            }
            else
            {
                selectedMaterialGroupIndex = EditorGUILayout.Popup("Material Group", selectedMaterialGroupIndex, materialGroupKeys.ToArray());
            }

            // Compute target asset name based solely on parent's name.
            assetName = sourceParent.name + "_CombinedMesh";
            assetPath = assetFolder + assetName + ".asset";

            // Display read-only information.
            EditorGUILayout.LabelField("Asset Name:", assetName);
            EditorGUILayout.LabelField("Save Path:", assetFolder);

            // Only show the overwrite warning if a combine hasn't just completed.
            if (!combineCompleted)
            {
                Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
                assetExists = (existingMesh != null);
                if (assetExists)
                {
                    EditorGUILayout.HelpBox("WARNING: An asset with this name already exists! Overwriting it may be irreversible.", MessageType.Error);
                    allowOverwrite = EditorGUILayout.Toggle("Allow Overwrite", allowOverwrite);
                }
                else
                {
                    allowOverwrite = false;
                }
            }
        }

        // Disable the Combine Meshes button after a successful combine.
        bool combineButtonEnabled = (sourceParent != null && materialGroupKeys.Count > 0 && (!assetExists || allowOverwrite)) && !combineCompleted;
        GUI.enabled = combineButtonEnabled;
        if (GUILayout.Button("Combine Meshes"))
        {
            statusMessage = "";
            string selectedKey = materialGroupKeys[selectedMaterialGroupIndex];
            CombineMeshes(sourceParent, selectedKey);
            combineCompleted = true;
            Repaint();
        }
        GUI.enabled = true;

        if (GUILayout.Button("Close"))
        {
            this.Close();
        }

        // Display concise feedback.
        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
        }
    }

    // Scans children of sourceParent for unique material combination keys.
    void UpdateMaterialGroupKeys()
    {
        materialGroupKeys.Clear();
        MeshFilter[] meshFilters = sourceParent.GetComponentsInChildren<MeshFilter>();
        HashSet<string> keys = new HashSet<string>();
        foreach (MeshFilter mf in meshFilters)
        {
            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr == null || mr.sharedMaterials == null || mr.sharedMaterials.Length == 0)
                continue;
            string key = GetMaterialCombinationKey(mr.sharedMaterials);
            keys.Add(key);
        }
        materialGroupKeys.AddRange(keys);
        if (selectedMaterialGroupIndex >= materialGroupKeys.Count)
            selectedMaterialGroupIndex = 0;
    }

    // Returns a string key representing the combination of materials.
    string GetMaterialCombinationKey(Material[] mats)
    {
        if (mats == null || mats.Length == 0) return "";
        List<string> names = new List<string>();
        foreach (Material m in mats)
        {
            if (m != null)
                names.Add(m.name);
        }
        return string.Join(", ", names.ToArray());
    }

    // Combines only those MeshFilters whose material combination matches selectedKey.
    void CombineMeshes(GameObject parent, string selectedKey)
    {
        MeshFilter[] allMeshFilters = parent.GetComponentsInChildren<MeshFilter>();
        List<MeshFilter> matchingFilters = new List<MeshFilter>();

        foreach (MeshFilter mf in allMeshFilters)
        {
            // Skip the parent's own MeshFilter.
            if (mf.gameObject == parent)
                continue;

            MeshRenderer mr = mf.GetComponent<MeshRenderer>();
            if (mr == null)
                continue;

            Material[] mats = mr.sharedMaterials;
            string key = GetMaterialCombinationKey(mats);
            if (key == selectedKey)
            {
                if (mf.sharedMesh == null || mf.sharedMesh.vertexCount == 0)
                    continue;
                matchingFilters.Add(mf);
            }
        }

        if (matchingFilters.Count == 0)
        {
            statusMessage = "No matching MeshFilters found for the selected material group.";
            return;
        }

        int submeshCount = matchingFilters[0].GetComponent<MeshRenderer>().sharedMaterials.Length;
        Mesh finalMesh = null;
        if (submeshCount == 1)
        {
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            foreach (MeshFilter mf in matchingFilters)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = mf.sharedMesh;
                // Pre-transform each mesh into parent's local space.
                ci.transform = parent.transform.worldToLocalMatrix * mf.transform.localToWorldMatrix;
                combineInstances.Add(ci);
            }
            finalMesh = new Mesh();
            finalMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            finalMesh.CombineMeshes(combineInstances.ToArray(), true, true);
        }
        else
        {
            finalMesh = CombineMultipleSubmeshes(matchingFilters, parent.transform.worldToLocalMatrix);
        }

        // Save the combined mesh as an asset.
        finalMesh.name = assetName;
        if (!AssetDatabase.IsValidFolder(assetFolder))
            AssetDatabase.CreateFolder("Assets", "CombinedMeshes");
        AssetDatabase.CreateAsset(finalMesh, assetPath);
        AssetDatabase.SaveAssets();
        Mesh savedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

        // Replace parent's mesh with the combined mesh.
        MeshFilter combinedMF = parent.GetComponent<MeshFilter>();
        if (combinedMF == null)
            combinedMF = parent.AddComponent<MeshFilter>();
        combinedMF.sharedMesh = savedMesh;
        MeshRenderer combinedMR = parent.GetComponent<MeshRenderer>();
        if (combinedMR == null)
            combinedMR = parent.AddComponent<MeshRenderer>();
        Material[] commonMats = matchingFilters[0].GetComponent<MeshRenderer>().sharedMaterials;
        combinedMR.sharedMaterials = commonMats;

        // Disable original objects.
        foreach (MeshFilter mf in matchingFilters)
        {
            if (mf.gameObject != parent)
                mf.gameObject.SetActive(false);
        }

        // Simplified feedback.
        statusMessage = matchingFilters.Count + " total meshes were successfully combined!";
    }

    // Manually combine meshes that use multiple submeshes.
    Mesh CombineMultipleSubmeshes(List<MeshFilter> meshFilters, Matrix4x4 parentMatrix)
    {
        List<Vector3> finalVertices = new List<Vector3>();
        List<Vector3> finalNormals = new List<Vector3>();
        List<Vector4> finalTangents = new List<Vector4>();
        List<Vector2> finalUVs = new List<Vector2>();

        int submeshCount = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterials.Length;
        List<List<int>> submeshTriangles = new List<List<int>>();
        for (int i = 0; i < submeshCount; i++)
            submeshTriangles.Add(new List<int>());
        int vertexOffset = 0;
        foreach (MeshFilter mf in meshFilters)
        {
            Mesh mesh = mf.sharedMesh;
            Matrix4x4 localMatrix = parentMatrix * mf.transform.localToWorldMatrix;
            Vector3[] verts = mesh.vertices;
            for (int i = 0; i < verts.Length; i++)
                finalVertices.Add(localMatrix.MultiplyPoint3x4(verts[i]));
            Vector3[] norms = mesh.normals;
            if (norms != null && norms.Length == verts.Length)
            {
                for (int i = 0; i < norms.Length; i++)
                    finalNormals.Add(localMatrix.MultiplyVector(norms[i]).normalized);
            }
            Vector2[] uvs = mesh.uv;
            if (uvs != null && uvs.Length == verts.Length)
                finalUVs.AddRange(uvs);
            for (int sub = 0; sub < submeshCount; sub++)
            {
                int[] tris = mesh.GetTriangles(sub);
                for (int t = 0; t < tris.Length; t++)
                    submeshTriangles[sub].Add(tris[t] + vertexOffset);
            }
            vertexOffset += verts.Length;
        }
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.SetVertices(finalVertices);
        if (finalNormals.Count == finalVertices.Count)
            combinedMesh.SetNormals(finalNormals);
        if (finalUVs.Count == finalVertices.Count)
            combinedMesh.SetUVs(0, finalUVs);
        combinedMesh.subMeshCount = submeshCount;
        for (int sub = 0; sub < submeshCount; sub++)
            combinedMesh.SetTriangles(submeshTriangles[sub], sub);
        combinedMesh.RecalculateBounds();
        if (finalNormals.Count != finalVertices.Count)
            combinedMesh.RecalculateNormals();
        return combinedMesh;
    }
}
