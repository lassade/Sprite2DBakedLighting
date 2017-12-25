using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEditor;

[CustomEditor(typeof(BakedLigthingSpriteData))]
public class BakedLigthingSpriteDataEditor : Editor {

	BakedLigthingSpriteData bakedLigthingSpriteData;
	SpriteRenderer spriteRenderer;
	Sprite sprite { get { return spriteRenderer.sprite; } }

	void OnEnable()
	{
		bakedLigthingSpriteData = target as BakedLigthingSpriteData;
		spriteRenderer = bakedLigthingSpriteData.GetComponent<SpriteRenderer> ();
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		if (sprite) {
			var msg = string.Format ("Current sprite OuterUV = {0},\nInnerUV = {1}",
				DataUtility.GetOuterUV (sprite), DataUtility.GetInnerUV (sprite));
			EditorGUILayout.HelpBox (msg, MessageType.Info);
		}
	}

	public override bool HasPreviewGUI ()
	{
		return sprite;
	}

	public override void DrawPreview (Rect previewArea)
	{
		//var outer = DataUtility.GetOuterUV (sprite);
		var outer = Application.isPlaying ? DataUtility.GetOuterUV (sprite) : bakedLigthingSpriteData.spriteOuterUV;
		//EditorGUI.DrawTextureTransparent(previewArea, null, ScaleMode.ScaleToFit, outer.z / outer.w);
		GUI.DrawTextureWithTexCoords(previewArea, sprite.texture, new Rect (outer.x, outer.y, outer.z, outer.w), true);
	}
}
