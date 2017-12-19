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
	private static Camera renderCamera = null;
	private Vector3 lastCamPos;

	private RenderTexture _leftEyeRenderTexture;
	private RenderTexture _rightEyeRenderTexture;

	Material mat;

	private Queue<Vector3> cameraPositions = new Queue<Vector3>();
	private Vector3 cameraDirection = Vector3.zero;

	private void enableLayer(Camera cam, int layer) {
		cam.cullingMask = cam.cullingMask | 1 << layer;
	}

	private void disableLayer(Camera cam, int layer) {
		cam.cullingMask = cam.cullingMask & ~(1 << layer);
	}

	// Use this for initialization
	void Start () {
		if (renderCamera == null) {
			GameObject cameraGameObject = new GameObject ();
			renderCamera = (Camera)cameraGameObject.AddComponent<Camera> ();
			renderCamera.tag = "Untagged";
			renderCamera.useOcclusionCulling = true;
		}

		enableLayer (renderCamera, DestinationLayer);
		disableLayer (renderCamera, SourceLayer);

		enableLayer (MainCamera, SourceLayer);
		disableLayer (MainCamera, DestinationLayer);

		_leftEyeRenderTexture = new RenderTexture(textureSize, textureSize, 24);
		_rightEyeRenderTexture = new RenderTexture(textureSize, textureSize, 24);

		mat = new Material(Shader.Find("Custom/PortalShaderVR"));
		GetComponent<Renderer> ().material = mat;
		mat.mainTexture = renderCamera.targetTexture;
	}

	int cpt = 0;

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

		print("test");

		if (!collider.gameObject.CompareTag (MainCamera.tag)) {
			return;
		}

		Vector3 currCamPos = MainCamera.transform.position;

		print(string.Format("{0} {1} {2}", cameraDirection, transform.parent.forward, Vector3.Dot (cameraDirection, transform.parent.forward)));
			
		if (Vector3.Dot (cameraDirection, transform.parent.forward) > 0f) {
			return;
		}

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

		this.gameObject.transform.parent.Rotate (0f, 180f, 0f);

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
			renderCamera.transform.localRotation = MainCamera.transform.localRotation;

			// left eye
			Vector3 eyeOffset = SteamVR.instance.eyes[0].pos;
			renderCamera.transform.localPosition = MainCamera.transform.position + MainCamera.transform.TransformVector(eyeOffset);
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
			renderCamera.transform.localPosition = MainCamera.transform.position + MainCamera.transform.TransformVector(eyeOffset);
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
