#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[ExecuteInEditMode]
public class BakeSpritesMeshExporter : MonoBehaviour
{
	public BakeSpritesEditorDataHolder dataHolder;

	public const string dataHolderPath = "Assets/LightingUtilities/Editor/BakeSpritesEditorDataHolder.asset";

	void Start ()
	{
		if (!dataHolder)
			return;
		
		switch(dataHolder.pipelineState)
		{
		case BakeSpritesEditorDataHolder.StateType.ExportSprites:
			if (EditorApplication.isPlaying)
			{
				StartCoroutine (ExportSpritesDelayed ());
			}
			break;

		case BakeSpritesEditorDataHolder.StateType.ReplaceSpritesForMeshes:
			if (!EditorApplication.isPlaying)
			{
				OpenSceneMode mode = OpenSceneMode.Single;
				foreach (var scene in dataHolder.scenes)
				{
					EditorSceneManager.OpenScene (scene, mode);
					mode = OpenSceneMode.Additive; // Append next scenes to the current open one
				}

				ReplaceSpritesForMeshes ();
			}
			break;

		default:
			break;
		}
	}

	IEnumerator ExportSpritesDelayed ()
	{
		yield return new WaitForFixedUpdate();

		foreach (var sprite in dataHolder.sprites) {
			GameObject gameObject = new GameObject (sprite.name);
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer> ();
			spriteRenderer.sprite = sprite;
			BakedLigthingSpriteData data = gameObject.AddComponent<BakedLigthingSpriteData> ();
			data.BuildMesh();

			// Save mesh created from sprites atlas (if packed)
			var mesh = data.mesh;
			dataHolder.meshes.Add (mesh);
			AssetDatabase.AddObjectToAsset (mesh, dataHolderPath);
		}

		dataHolder.pipelineState = BakeSpritesEditorDataHolder.StateType.ReplaceSpritesForMeshes; // Next stage of the script
		AssetDatabase.SaveAssets ();

		EditorApplication.isPlaying = false;
	}

	void ReplaceSpritesForMeshes()
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

					Sprite sprite = spriteRenderer.sprite;
					if (!sprite) continue;

					var index = dataHolder.sprites.IndexOf (sprite);
					if (index < 0)
					{
						Debug.LogErrorFormat ("Could not find sprite {1} for {0}", gameObject.name, sprite.name);
					}
					else
					{
						// Create if not find a BakedLigthingSpriteData
						BakedLigthingSpriteData data = gameObject.GetComponent<BakedLigthingSpriteData>();
						if (!data) data = gameObject.AddComponent<BakedLigthingSpriteData>();
						data.BuildMesh();

						DestroyImmediate (data.mesh);
						MeshFilter meshFilter = data.content.GetComponent<MeshFilter> ();
						meshFilter.sharedMesh = data.mesh = dataHolder.meshes [index];
					}
				}
			}
		}

		EditorSceneManager.MarkAllScenesDirty();
	}

	static IEnumerable<Transform> TraverseTransformTree(Transform root)
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
#endif