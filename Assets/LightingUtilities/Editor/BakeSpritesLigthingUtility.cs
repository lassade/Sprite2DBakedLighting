using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public static class BakeSpritesLigthingEditor
{
    [MenuItem("Lighting/Prepare Sprites")]
    public static void PrepareSprites()
    {
        for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
        {
            Scene scene = EditorSceneManager.GetSceneAt(i);
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var node in TraverseTransformTree(root.transform))
                {
                    GameObject gameObject = node.gameObject;
                    StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(gameObject);
                    if ((flags & StaticEditorFlags.LightmapStatic) == 0) continue;

                    SpriteRenderer spriteRenderer = node.GetComponent<SpriteRenderer>();
                    if (!spriteRenderer) continue;

                    // Create if not find a BakedLigthingSpriteData
                    BakedLigthingSpriteData data = gameObject.GetComponent<BakedLigthingSpriteData>();
                    if (!data) data = gameObject.AddComponent<BakedLigthingSpriteData>();
                    data.BuildMesh();
                }
            }
        }

        EditorSceneManager.MarkAllScenesDirty();
    }

    [MenuItem("Lighting/Clean Sprites")]
    public static void CleanLightingSpriteData()
    {
        for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
        {
            Scene scene = EditorSceneManager.GetSceneAt(i);
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var node in TraverseTransformTree(root.transform))
                {
                    BakedLigthingSpriteData data = node.GetComponent<BakedLigthingSpriteData>();
                    if (!data) continue;
                    data.TransferLightmapData();
                    data.Clear();
                }
            }
        }

        EditorSceneManager.MarkAllScenesDirty();
    }

    [MenuItem("Lighting/Log Sprites Light Data")]
    public static void LogLightingSpriteData()
    {
        for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
        {
            Scene scene = EditorSceneManager.GetSceneAt(i);
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var node in TraverseTransformTree(root.transform))
                {
                    GameObject gameObject = node.gameObject;
                    StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(gameObject);
                    if ((flags & StaticEditorFlags.LightmapStatic) == 0) continue;

                    SpriteRenderer spriteRenderer = node.GetComponent<SpriteRenderer>();
                    if (!spriteRenderer) continue;

                    Debug.Log(string.Format("Lightmap .Index = {0}; .ScaleOffset = {1}", spriteRenderer.lightmapIndex, spriteRenderer.lightmapScaleOffset));
                }
            }
        }

        EditorSceneManager.MarkAllScenesDirty();
    }

    private static IEnumerable<Transform> TraverseTransformTree(Transform root)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform a = root.GetChild(i);
            yield return a;
            foreach (var b in TraverseTransformTree(a))
            {
                yield return b;
            }
        }
    }
}
