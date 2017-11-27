using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour {

	public Camera camera;

	public Material renderMat;

	// When game starts remove current camera textures and set new textures with the dimensions of the players screen
	void Start()
	{
		if (camera.targetTexture != null)
		{
			camera.targetTexture.Release();
		}

		camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
		renderMat.mainTexture = camera.targetTexture;
	}
}
