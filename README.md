# Unity Sprite2D Bake Lighting

Unity Enlighten does not bake lighting into sprites. This script reaplace (temporarily) every sprite in the open scenes by a mesh this will trick Enlighten to bake the lighting for each sprite, afterwards you can call a function to transfer all the ligth data into the original SpriteRenderer.

This code supposedly works if you have any external lighting solution, but is not tested yet.

Pull Requests are welcomed!

Use at your own risk, be smart and create a **backup** of all your work before using this code !!!

### Sprite Atlas Support (Beta)

I mananged to get the bake working with sprites packed using the new SpriteAtlas. To experiment with this feature
use instead of using Lighting/Prepare Sprites in step 3 call the Lighting/Export Packed Meshes, let the editor
do his thing, them folow the steps as usual. Notice that it may look strange outsite the play mode but will
look right as soon as you hit play.

Limitations:
1. Looks strange when not oin play mode
2. You can not use different SpriteAtlase that you used to bake the lighting. It may cause some problems

Please set the Sprite Packer mode in "Edit/Project Settings/Editor" to be "Always Enabled"

### Usage

1. Set at least LightmapStatic if you want to bake the sprite lights;

![Set LightmapStatic if you want to bake the sprite lights](https://raw.githubusercontent.com/lassade/Sprite2DBakedLighting/master/Extra/1.png)

2. Remember to configure the light Baking property into Baked or Mixed;
 
![Set LightmapStatic if you want to bake the sprite lights](https://raw.githubusercontent.com/lassade/Sprite2DBakedLighting/master/Extra/3.png)

3. Go to Lighting/Prepare Sprites in the toolbar. This will replace every sprite in your scene by a mesh, but do not worry this change is temporarily and is only made to Unity Enlighten into bake lighting for the sprites;

![Set LightmapStatic if you want to bake the sprite lights](https://raw.githubusercontent.com/lassade/Sprite2DBakedLighting/master/Extra/2.png)

4. Bake your ligths as usual
5. Hit Lighting/Clean Sprites in the toolbar, to copy the light data into your sprites.
6. I found out that the Sprites-Diffuse shader works with baked lights, but you can use any shader that works lightmaping.
7. Your are done!

### Demo

You can see it working by open the scene demo scene an following the steps above. The demo uses a modified version of the [2D Platformer](https://www.assetstore.unity3d.com/en/#!/content/11228).

### Updates

Latest Unity builds changed the Sprites-Diffuse shader and removed the ligthmaps support, the new shader Custom/Sprites-Ligthmap adds this function back. Thanks Coresi7 for pointing it out and for providing the shader.

### TODO

The baked lightmaps have too much redundant parts, this causes alot of video memory been used for the same parts of the static lighting. A possible fix for this maybe combine all the meshes into a single one, then spliting into multiple squares,
