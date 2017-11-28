using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

	public Camera MainCamera;
	public int SourceLayer, DestinationLayer;

	private bool crossed = false;
	private Camera renderCamera;
	private MeshRenderer renderer;

	private void enableLayer(Camera cam, int layer) {
		cam.cullingMask = cam.cullingMask | 1 << layer;
	}

	private void disableLayer(Camera cam, int layer) {
		cam.cullingMask = cam.cullingMask & ~(1 << layer);
	}

	// Use this for initialization
	void Start () {
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

		renderCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
		Material mat = new Material(Shader.Find("Hidden/PortalEffectShader"));
		renderer = GetComponent<MeshRenderer> ();
		renderer.material = mat;
		mat.mainTexture = renderCamera.targetTexture;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collider) {

		if (!collider.gameObject.CompareTag (MainCamera.tag)) {
			return;
		}

		Vector3 camToPortal = this.transform.position - MainCamera.transform.position;

		if (crossed) {
			disableLayer (MainCamera, DestinationLayer);
			enableLayer (MainCamera, SourceLayer);

			disableLayer (renderCamera, SourceLayer);
			enableLayer (renderCamera, DestinationLayer);

			this.gameObject.layer = SourceLayer;
		} else {
			disableLayer (MainCamera, SourceLayer);
			enableLayer (MainCamera, DestinationLayer);

			disableLayer (renderCamera, DestinationLayer);
			enableLayer (renderCamera, SourceLayer);

			this.gameObject.layer = DestinationLayer;
		}

		this.gameObject.transform.Rotate (0f, 180f, 0f, Space.World);
			
		crossed = !crossed;
	}
}
