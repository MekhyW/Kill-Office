Exmples:
	Contains an example scene ExampleImage.unity that:
	1) Contains the requried AdinmoManager in the root of the scene
	2) Applies an AdinmoTexture to an image that will be replaced with an ad
	3) Includes an ExampleCallbacks.cs script that shows examples of how the callbacks can be used to notify that game when 	textures have been applied.

Plugins:
	AdinmoPlugin.dll -- a small pure .net plugin that handles all communication with the Adinmo Servers

Prefabs:
	AdinmoManager.prefab must be populated in the root of your scene. You need to add your game key obtained from our website.

Scripts:
	AdinmoTexture.cs is a component that you will add to textures that need to be replaced,  It needs to conain a placement key obtained from our website.

Resources:
	Contains prefabs and textures uesed for debugging placement sizes


