using System.Collections.Generic;
using Tetris;
using Unity.Rendering;
using UnityEditor;
using UnityEngine;

public class SerializePrefabToEcs : Editor
{
    [MenuItem("Tools/Serialize Selection")]
    public static void SerializeToSO()
    {
        var selection = Selection.activeGameObject;
        var dataModelContainer = ScriptableObject.CreateInstance<TetrisTransformContainer>();
        dataModelContainer.transformDatas = new List<TetrisTransform>();
        var renderers = selection.GetComponentsInChildren<MeshRenderer>();
        var renderer = renderers[0].GetComponent<MeshRenderer>();
        var mesh = renderer.GetComponent<MeshFilter>();
        dataModelContainer.renderer = new MeshInstanceRenderer()
        {
            mesh = mesh.sharedMesh,
            material = renderer.sharedMaterial,
            castShadows = renderer.shadowCastingMode,
            receiveShadows = renderer.receiveShadows
        };
        foreach (var meshRenderer in renderers)
        {
            AddToDataFromGameObject(meshRenderer.gameObject, ref dataModelContainer);
        }
        var name = selection.gameObject.name;
        var path =  AssetDatabase.GetAssetPath(selection).Replace($"{name}.prefab",string.Empty);
        AssetDatabase.CreateAsset(dataModelContainer, $"{path}Resources/{name}.asset");
    }

    private static void AddToDataFromGameObject(GameObject selection, ref TetrisTransformContainer tetrisTransformContainer)
    {
        var dataModel = new TetrisTransform
        {
            position = selection.transform.localPosition, 
            rotation = selection.transform.rotation 
        };
        tetrisTransformContainer.transformDatas.Add(dataModel);
    }
}
