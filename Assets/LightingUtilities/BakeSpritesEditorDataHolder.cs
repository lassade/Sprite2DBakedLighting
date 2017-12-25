using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BakeSpritesEditorDataHolder : ScriptableObject
{
	[System.Serializable]
	public enum StateType
	{
		ExportSprites,
		ReplaceSpritesForMeshes,
		BakeLigths
	}

	public StateType pipelineState = StateType.ExportSprites;
	public List<string> scenes = new List<string>();
	public List<Sprite> sprites = new List<Sprite> ();
	public List<Mesh> meshes = new List<Mesh>();

	public static BakeSpritesEditorDataHolder Create()
	{
		return CreateInstance<BakeSpritesEditorDataHolder> ();
	}
}