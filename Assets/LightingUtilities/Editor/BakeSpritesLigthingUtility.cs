using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class BakeSpritesLigthingEditor
{
	public static string dataHolderPath 
	{
		get {
			return BakeSpritesMeshExporter.dataHolderPath;
		}
	}

	[MenuItem("Lighting/Export Packed Meshes")]
	public static void ExportPackedMeshes()
	{
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();

		BakeSpritesEditorDataHolder data = BakeSpritesEditorDataHolder.Create ();
//		data.scenes.Clear ();
//		data.sprites.Clear ();
//		data.meshes.Clear ();
		
		for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
		{
			Scene scene = EditorSceneManager.GetSceneAt(i);
			data.scenes.Add (scene.path);

			foreach (var root in scene.GetRootGameObjects())
			{
				foreach (var node in TraverseTransformTree(root.transform))
				{
					StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(node.gameObject);
					if ((flags & StaticEditorFlags.LightmapStatic) == 0) continue;

					SpriteRenderer spriteRenderer = node.GetComponent<SpriteRenderer>();
					if (!spriteRenderer) continue;

					Sprite sprite = spriteRenderer.sprite;
					if (!sprite) continue;

					data.sprites.Add(sprite);
				}
			}
		}

		FileUtil.DeleteFileOrDirectory (dataHolderPath);
		AssetDatabase.CreateAsset (data, dataHolderPath);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();

		// Closes all current scenes
		EditorSceneManager.NewScene (NewSceneSetup.EmptyScene, NewSceneMode.Single);

		GameObject gameObject = new GameObject ("BakeSpritesMeshExporter");
		var exporter = gameObject.AddComponent<BakeSpritesMeshExporter>();
		exporter.dataHolder = data;

		// Force atlas to be loaded
		EditorApplication.isPlaying = true;
	}

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

					Debug.LogFormat("{2} Lightmap .Index = {0}; .ScaleOffset = {1}",
						spriteRenderer.lightmapIndex, spriteRenderer.lightmapScaleOffset, spriteRenderer.name);
                }
            }
        }
    }

    private static IEnumerable<Transform> TraverseTransformTree(Transform root)
    {
        yield return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform a = root.GetChild(i);
            foreach (var b in TraverseTransformTree(a))
            {
                yield return b;
            }
        }
    }
}
