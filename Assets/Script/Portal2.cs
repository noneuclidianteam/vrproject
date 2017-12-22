using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal2 : MonoBehaviour {

	public GameObject DestinationPortal;
	public GameObject DestinationPortal2;

	private GameObject currentPortal;

	public bool DoubleSided = false;

	public int CameraPositionHistorySize = 20;

	public int RenderCameraIgnoredLayer = -1;

	private Camera playerCamera;
	private static Camera renderCamera;

	private Queue<Vector3> cameraPositions = new Queue<Vector3>();
	private Vector3 cameraDirection = Vector3.zero;

	private Material portalMaterial;

	private RenderTexture _leftEyeRenderTexture;
	private RenderTexture _rightEyeRenderTexture;

	private void createRenderCamera() {
		if (renderCamera == null) {
			GameObject cameraGameObject = new GameObject ();
			renderCamera = (Camera)cameraGameObject.AddComponent<Camera> ();
			renderCamera.tag = "Untagged";
			renderCamera.useOcclusionCulling = true;
			renderCamera.nearClipPlane = 0.01f;
			//renderCamera.cullingMask = renderCamera.cullingMask & ~(1 << LayerMask.NameToLayer ("Layer1"));
			renderCamera.enabled = false;
		}
	}

	private void createRenderTextures() {
		if (PortalParameters.instance.EnableVR) {
			_leftEyeRenderTexture = new RenderTexture ((int)SteamVR.instance.sceneWidth, (int)SteamVR.instance.sceneWidth, 24);
			_rightEyeRenderTexture = new RenderTexture ((int)SteamVR.instance.sceneWidth, (int)SteamVR.instance.sceneWidth, 24);
		} else {
			_leftEyeRenderTexture = new RenderTexture (Screen.currentResolution.width, Screen.currentResolution.height, 24);
		}
	}

	private void assignPortalMaterial() {
		portalMaterial = new Material(Shader.Find("Custom/PortalShader"));
		GetComponent<Renderer> ().material = portalMaterial;
	}

	private void updateCameraDirection() {
		cameraPositions.Enqueue(playerCamera.transform.position);
		cameraDirection += playerCamera.transform.position;

		if (cameraPositions.Count >= CameraPositionHistorySize) {
			cameraDirection -= cameraPositions.Dequeue();
		}

		cameraDirection.Normalize();
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

	private Vector3 portalOffset;

	private void computePortalOffset() {
		portalOffset = currentPortal.transform.position - transform.position;
	}

	public void preparePortalRenderStereo() {
		renderCamera.transform.rotation = playerCamera.transform.rotation;

		// left eye
		Vector3 eyeOffset = SteamVR.instance.eyes[0].pos;
		renderCamera.transform.position = playerCamera.transform.position + portalOffset + playerCamera.transform.TransformVector(eyeOffset);
		renderCamera.projectionMatrix = HMDMatrix4x4ToMatrix4x4(
			SteamVR.instance.hmd.GetProjectionMatrix(
				Valve.VR.EVREye.Eye_Left,
				playerCamera.nearClipPlane,
				playerCamera.farClipPlane
			)
		);
		renderCamera.targetTexture = _leftEyeRenderTexture;
		renderCamera.Render();
		portalMaterial.SetTexture("_LeftEyeTexture", _leftEyeRenderTexture);

		// right eye
		eyeOffset = SteamVR.instance.eyes[1].pos;
		renderCamera.transform.position = playerCamera.transform.position + portalOffset + playerCamera.transform.TransformVector(eyeOffset);
		renderCamera.projectionMatrix = HMDMatrix4x4ToMatrix4x4(
			SteamVR.instance.hmd.GetProjectionMatrix(
				Valve.VR.EVREye.Eye_Right,
				playerCamera.nearClipPlane,
				playerCamera.farClipPlane
			)
		);
		renderCamera.targetTexture = _rightEyeRenderTexture;
		portalMaterial.SetInt ("_VREnabled", 1);
		renderCamera.Render();

		portalMaterial.SetTexture("_RightEyeTexture", _rightEyeRenderTexture);
	}

	public void preparePortalRenderStandart() {
		//Vector3 playerForwardRelativeToPortal = PortalRoot.transform.InverseTransformDirection (playerCamera.transform.forward);
		//lastForward = DestinationPortal.transform.TransformDirection (playerForwardRelativeToPortal);
		//renderCamera.transform.forward = lastForward;
		//renderCamera.transform.localPosition = transform.position - playerCamera.transform.position;

		//print(string.Format("{0} {1}", playerCamera.transform.localPosition, renderCamera.transform.localPosition));

		renderCamera.transform.rotation = playerCamera.transform.rotation;
		renderCamera.transform.position = playerCamera.transform.position + portalOffset;

		renderCamera.projectionMatrix = playerCamera.projectionMatrix;
		renderCamera.targetTexture = _leftEyeRenderTexture;
		portalMaterial.SetInt ("_VREnabled", 0);

		renderCamera.Render();

		portalMaterial.SetTexture ("_LeftEyeTexture", _leftEyeRenderTexture);
	}

	void handleDoubleSided() {
		if (Vector3.Dot (PortalParameters.instance.getUsedCamera ().transform.forward, transform.parent.forward) > 0) {
			this.gameObject.transform.parent.Rotate (0f, 180f, 0f);
			if (DestinationPortal2 != null) {
				currentPortal = currentPortal == DestinationPortal ? DestinationPortal2 : DestinationPortal;
			}
		}
	}

	// Use this for initialization
	void Start () {
		if (DestinationPortal != null) {
			currentPortal = DestinationPortal;
		} else {
			currentPortal = DestinationPortal2;
		}

		if (RenderCameraIgnoredLayer == -1) {
			RenderCameraIgnoredLayer = PortalParameters.instance.RenderCameraIgnoredLayer;
		}

		createRenderCamera();
		createRenderTextures();
		assignPortalMaterial ();
		playerCamera = PortalParameters.instance.getUsedCamera ();
	}
	
	// Update is called once per frame
	void Update () {
		updateCameraDirection();

		if (DoubleSided) {
			handleDoubleSided();
		}

		computePortalOffset ();
	}

	void OnTriggerEnter(Collider collider) {
		if (!collider.gameObject.CompareTag (playerCamera.tag)) {
			return;
		}

		if (Vector3.Dot (cameraDirection, transform.parent.forward) > 0f) {
			return;
		}

		//TODO : Teleport player here.
		PortalParameters.instance.getPlayerObject().transform.position += portalOffset;

		StartCoroutine (SwitchActivePortal());

		print("Portal passed");
	}

	IEnumerator SwitchActivePortal()
	{
		yield return new WaitForSeconds (0.05f);
		currentPortal.SetActive (true);
		yield return new WaitForSeconds (0.05f);
		gameObject.SetActive (false);
	}


	public void OnWillRenderObject() {
		if (Camera.current != playerCamera) {
			return;
		}

		portalMaterial.SetInt("_BluredPortal", PortalParameters.instance.PortalBlur ? 1 : 0);

		if (RenderCameraIgnoredLayer != -1) {
			renderCamera.cullingMask = renderCamera.cullingMask & ~(1 << RenderCameraIgnoredLayer);
		}

		if (PortalParameters.instance.EnableVR) {
			preparePortalRenderStereo ();
		} else {
			preparePortalRenderStandart ();
		}

		if (RenderCameraIgnoredLayer != -1) {
			renderCamera.cullingMask = renderCamera.cullingMask | 1 << RenderCameraIgnoredLayer;
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine (gameObject.transform.position, DestinationPortal.transform.position);
	}
}
