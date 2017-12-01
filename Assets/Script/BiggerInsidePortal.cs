using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiggerInsidePortal : MonoBehaviour {

	public Camera MainCamera;
	public int SourceLayer, DestinationLayer;

    public GameObject pillar;
    public GameObject pillar_alt;

	private bool crossed = false;
	private Camera renderCamera;
	private Vector3 lastCamPos;

	private void enableLayer(Camera cam, int layer) {
		cam.cullingMask = cam.cullingMask | 1 << layer;
	}

	private void disableLayer(Camera cam, int layer) {
		cam.cullingMask = cam.cullingMask & ~(1 << layer);
	}

	// Use this for initialization
	void Start ()
    {
		renderCamera = (Camera) Camera.Instantiate(
			MainCamera.GetComponent<Camera>(),
			MainCamera.transform.position,
			MainCamera.transform.rotation,
			MainCamera.transform
		);

		renderCamera.tag = "Untagged";

		enableLayer (renderCamera, DestinationLayer);
		disableLayer (renderCamera, SourceLayer);

		enableLayer (MainCamera, SourceLayer);
		disableLayer (MainCamera, DestinationLayer);

		// renderCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
		renderCamera.targetTexture = new RenderTexture(2000, 2000, 24);
		Material mat = new Material(Shader.Find("Hidden/PortalEffectShader"));
		GetComponent<Renderer> ().material = mat;
		mat.mainTexture = renderCamera.targetTexture;
	}
	

	void Update ()
    {
		lastCamPos = MainCamera.transform.position;
	}

	void OnTriggerEnter(Collider collider)
    {

		print("Colliding");

		if (!collider.gameObject.CompareTag (MainCamera.tag)) 
			return;
		
		Vector3 currCamPos = MainCamera.transform.position;

		if (crossed)
        {
			if (Vector3.Dot (currCamPos - lastCamPos, transform.forward) > 0f) 
				return;
			
			disableLayer (MainCamera, DestinationLayer);
			enableLayer (MainCamera, SourceLayer);

			disableLayer (renderCamera, SourceLayer);
			enableLayer (renderCamera, DestinationLayer);           

            gameObject.layer = SourceLayer;
		}
        else
        {
			if (Vector3.Dot (currCamPos - lastCamPos, transform.forward) < 0f) 
				return;		

			disableLayer (MainCamera, SourceLayer);
			enableLayer (MainCamera, DestinationLayer);

			disableLayer (renderCamera, DestinationLayer);
			enableLayer (renderCamera, SourceLayer);

            gameObject.layer = DestinationLayer;
		}

		gameObject.transform.Rotate (0f, 180f, 0f, Space.World);

        pillar.SetActive(crossed);
        pillar_alt.SetActive(!crossed);

        crossed = !crossed;
	}
}
