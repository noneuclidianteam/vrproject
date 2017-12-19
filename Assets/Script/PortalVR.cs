using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PortalVR : MonoBehaviour {

	public Camera MainCamera;

	public int SourceLayer, DestinationLayer;

	public int textureSize = 1024;

	public int cameraPositionHistorySize = 20;

	private bool crossed = false;
	private Camera renderCamera;
	private Vector3 lastCamPos;

	private RenderTexture _leftEyeRenderTexture;
	private RenderTexture _rightEyeRenderTexture;

	Material mat;

	private Queue<Vector3> cameraPositions;
	private Vector3 cameraDirection = Vector3.zero;

	private void enableLayer(Camera cam, int layer) {
		cam.cullingMask = cam.cullingMask | 1 << layer;
	}

	private void disableLayer(Camera cam, int layer) {
		cam.cullingMask = cam.cullingMask & ~(1 << layer);
	}

	// Use this for initialization
	void Start () {
		GameObject cameraGameObject = new GameObject ();
		renderCamera = (Camera)cameraGameObject.AddComponent<Camera> ();
		renderCamera.tag = "Untagged";

		enableLayer (renderCamera, DestinationLayer);
		disableLayer (renderCamera, SourceLayer);

		enableLayer (MainCamera, SourceLayer);
		disableLayer (MainCamera, DestinationLayer);

		_leftEyeRenderTexture = new RenderTexture(textureSize, textureSize, 24);
		_rightEyeRenderTexture = new RenderTexture(textureSize, textureSize, 24);

		Material mat = new Material(Shader.Find("Custom/PortalShaderVR"));
		GetComponent<Renderer> ().material = mat;
		mat.mainTexture = renderCamera.targetTexture;
	}

	// Update is called once per frame
	void Update () {
		cameraPositions.Enqueue(MainCamera.transform.position);
		cameraDirection += MainCamera.transform.position;

		if (cameraPositions.Count >= cameraPositionHistorySize) {
			cameraDirection -= cameraPositions.Dequeue();
		}

		cameraDirection.Normalize();
	}

	void OnTriggerEnter(Collider collider) {

		if (!collider.gameObject.CompareTag (MainCamera.tag)) {
			return;
		}

		Vector3 currCamPos = MainCamera.transform.position;

		if (crossed) {
			if (Vector3.Dot (cameraDirection, transform.forward) > 0f) {
				return;
			}
			disableLayer (MainCamera, DestinationLayer);
			enableLayer (MainCamera, SourceLayer);

			disableLayer (renderCamera, SourceLayer);
			enableLayer (renderCamera, DestinationLayer);

			this.gameObject.layer = SourceLayer;
		} else {
			if (Vector3.Dot (cameraDirection, transform.forward) < 0f) {
				return;
			}

			disableLayer (MainCamera, SourceLayer);
			enableLayer (MainCamera, DestinationLayer);

			disableLayer (renderCamera, DestinationLayer);
			enableLayer (renderCamera, SourceLayer);

			this.gameObject.layer = DestinationLayer;
		}

		this.gameObject.transform.Rotate (0f, 180f, 0f, Space.World);

		crossed = !crossed;
	}

	private Matrix4x4 HMDMatrix4x4ToMatrix4x4(Valve.VR.HmdMatrix44_t input) {
		var m = Matrix4x4.identity;
		m[0, 0] = input.m0;
		m[0, 1] = input.m1;
		m[0, 2] = input.m2;
		m[0, 3] = input.m3;
		m[1, 0] = input.m4;
		m[1, 1] = input.m5;
		m[1, 2] = input.m6;
		m[1, 3] = input.m7;
		m[2, 0] = input.m8;
		m[2, 1] = input.m9;
		m[2, 2] = input.m10;
		m[2, 3] = input.m11;
		m[3, 0] = input.m12;
		m[3, 1] = input.m13;
		m[3, 2] = input.m14;
		m[3, 3] = input.m15;
		return m; 
	}

	public void OnWillRenderObject() {
		if (Camera.current == MainCamera) {
			transform.localRotation = MainCamera.transform.localRotation;

			// left eye
			Vector3 eyeOffset = SteamVR.instance.eyes[0].pos;
			transform.localPosition = MainCamera.transform.position + MainCamera.transform.TransformVector(eyeOffset);
			renderCamera.projectionMatrix = HMDMatrix4x4ToMatrix4x4(
				SteamVR.instance.hmd.GetProjectionMatrix(
					Valve.VR.EVREye.Eye_Left,
					MainCamera.nearClipPlane,
					MainCamera.farClipPlane
				)
			);
			renderCamera.targetTexture = _leftEyeRenderTexture;
			renderCamera.Render();
			mat.SetTexture("_LeftEyeTexture", _leftEyeRenderTexture);

			// right eye
			eyeOffset = SteamVR.instance.eyes[1].pos;
			transform.localPosition = MainCamera.transform.position + MainCamera.transform.TransformVector(eyeOffset);
			renderCamera.projectionMatrix = HMDMatrix4x4ToMatrix4x4(
				SteamVR.instance.hmd.GetProjectionMatrix(
					Valve.VR.EVREye.Eye_Right,
					MainCamera.nearClipPlane,
					MainCamera.farClipPlane
				)
			);
			renderCamera.targetTexture = _rightEyeRenderTexture;
			renderCamera.Render();
			mat.SetTexture("_RightEyeTexture", _rightEyeRenderTexture);
		} 
	}
		
}
