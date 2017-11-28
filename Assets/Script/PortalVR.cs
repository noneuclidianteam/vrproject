using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PortalVR : MonoBehaviour {

	public Camera RightCamera, LeftCamera;

	public int SourceLayer, DestinationLayer, 
		RightOnlySourceLayer, LeftOnlySourceLayer,
		RightOnlyDestinationLayer, LeftOnlyDestinationLayer;

	private bool crossed = false;
	private Camera renderCameraRight, renderCameraLeft;
	private GameObject portalRightOnly, portalLeftOnly;
	private Vector3 lastCamPos;

	private void enableLayer(Camera cam, int layer) {
		cam.cullingMask = cam.cullingMask | 1 << layer;
	}

	private void disableLayer(Camera cam, int layer) {
		cam.cullingMask = cam.cullingMask & ~(1 << layer);
	}

	private void swapLayers(ref int layer1, ref int layer2) {
		int tmp = layer1;
		layer1 = layer2;
		layer2 = tmp;
	}

	private void invertLayers() {
		swapLayers(ref SourceLayer, ref DestinationLayer);
		swapLayers(ref RightOnlySourceLayer, ref RightOnlyDestinationLayer);
		swapLayers(ref LeftOnlySourceLayer, ref LeftOnlyDestinationLayer);

		enableLayer(LeftCamera, SourceLayer);
		disableLayer(LeftCamera, DestinationLayer);
		enableLayer(LeftCamera, LeftOnlySourceLayer);
		disableLayer(LeftCamera, LeftOnlyDestinationLayer);

		enableLayer(RightCamera, SourceLayer);
		disableLayer(RightCamera, DestinationLayer);
		enableLayer(RightCamera, RightOnlySourceLayer);
		disableLayer(RightCamera, RightOnlyDestinationLayer);
	}

	Camera createRenderCameraFrom(Camera cam) {
		Camera renderCamera = (Camera) Camera.Instantiate(
			cam.GetComponent<Camera>(),
			cam.transform.position,
			cam.transform.rotation,
			cam.transform
		);

		renderCamera.tag = "Untagged";

		enableLayer(renderCamera, DestinationLayer);
		disableLayer(renderCamera, SourceLayer);

		renderCamera.targetTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24);

		return renderCamera;
	}

	// Use this for initialization
	void Start () {

		if (this.gameObject.name.Contains("Clone")) {
			return;
		}

		renderCameraRight = createRenderCameraFrom(RightCamera);
		enableLayer (renderCameraRight, RightOnlyDestinationLayer);
		disableLayer (renderCameraRight, RightOnlySourceLayer);

		renderCameraLeft = createRenderCameraFrom(LeftCamera);
		enableLayer (renderCameraLeft, LeftOnlyDestinationLayer);
		disableLayer (renderCameraLeft, LeftOnlySourceLayer);

		portalRightOnly = this.gameObject;
		portalLeftOnly = Instantiate(portalRightOnly);

		Material matRight = new Material(Shader.Find("Hidden/PortalEffectShader"));
		GetComponent<Renderer> ().material = matRight;
		matRight.mainTexture = renderCameraRight.targetTexture;
		portalLeftOnly.layer = RightOnlySourceLayer;

		Material matLeft = new Material(Shader.Find("Hidden/PortalEffectShader"));
		portalLeftOnly.GetComponent<Renderer>().material = matLeft;
		matLeft.mainTexture = renderCameraLeft.targetTexture;
		portalLeftOnly.layer = RightOnlySourceLayer;
	}

	// Update is called once per frame
	void Update () {
		lastCamPos = RightCamera.transform.position;
	}

	void OnTriggerEnter(Collider collider) {

		if (!collider.gameObject.CompareTag (RightCamera.tag)) {
			return;
		}

		Vector3 currCamPos = RightCamera.transform.position;

		if (crossed) {
			if (Vector3.Dot (currCamPos - lastCamPos, transform.forward) > 0f) {
				return;
			}

			invertLayers();
		} else {
			if (Vector3.Dot (currCamPos - lastCamPos, transform.forward) < 0f) {
				return;
			}

			invertLayers();
		}

		this.gameObject.transform.Rotate (0f, 180f, 0f, Space.World);

		crossed = !crossed;
	}
}
