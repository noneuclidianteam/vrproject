using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

	public Portal DestinationPortal = null;

	public int RenderCameraIgnoredLayer = -1;

	private Camera playerCamera;
	private static Camera renderCamera;

	private Queue<Vector3> cameraDirections = new Queue<Vector3>();
	private Vector3 cameraDirection = Vector3.zero;

	private Material portalMaterial;

	private RenderTexture _leftEyeRenderTexture;
	private RenderTexture _rightEyeRenderTexture;

	public Texture2D displacementMap;

	private Vector3 lastCameraPosition;

	private static int cameraPositionHistorySize = 0;

	private Room room;

	private static float lastCross = 0f;

	public Room getRoom() {
		return this.room;
	}

	public void setRoom(Room room) {
		this.room = room;
	}

	private void createRenderCamera() {
		if (renderCamera == null) {
			GameObject cameraGameObject = new GameObject ();
			renderCamera = (Camera)cameraGameObject.AddComponent<Camera> ();
			renderCamera.tag = "Untagged";
			renderCamera.name = "RenderCamera";
			renderCamera.useOcclusionCulling = true;
			renderCamera.nearClipPlane = 0.01f;
			//renderCamera.cullingMask = renderCamera.cullingMask & ~(1 << LayerMask.NameToLayer ("Layer1"));
			renderCamera.enabled = false;
		}
	}

	private void createRenderTextures() {
		if (PortalManager.instance.EnableVR) {
			_leftEyeRenderTexture = new RenderTexture ((int)SteamVR.instance.sceneWidth, (int)SteamVR.instance.sceneWidth, 24);
			_rightEyeRenderTexture = new RenderTexture ((int)SteamVR.instance.sceneWidth, (int)SteamVR.instance.sceneWidth, 24);
		} else {
			_leftEyeRenderTexture = new RenderTexture (Screen.currentResolution.width, Screen.currentResolution.height, 24);
			//_leftEyeRenderTexture = new RenderTexture (256, 256, 24);
		}
	}

	private void assignPortalMaterial() {
		portalMaterial = new Material(Shader.Find("Custom/PortalShader"));
		GetComponent<Renderer> ().material = portalMaterial;
	}

	private void updateCameraDirection() {
		if (lastCameraPosition == null) {
			return;
		}

		Vector3 currentCameraPosition = playerCamera.transform.position;
		Vector3 currentCameraDirection = currentCameraPosition - lastCameraPosition;

		cameraDirections.Enqueue(currentCameraDirection);

		if (cameraDirections.Count >= cameraPositionHistorySize) {
			cameraDirections.Dequeue();
		}

		cameraDirection = Vector3.zero;

		foreach (Vector3 vec in cameraDirections) {
			cameraDirection += vec;
		}

		cameraDirection.Normalize();

		lastCameraPosition = currentCameraPosition;
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

	public void renderPortalTextureStereo(Vector3 renderCameraPosition) {
		renderCamera.transform.rotation = playerCamera.transform.rotation;

		// left eye
		Vector3 eyeOffset = SteamVR.instance.eyes[0].pos;
		renderCamera.transform.position = renderCameraPosition + playerCamera.transform.TransformVector(eyeOffset);
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
		renderCamera.transform.position = renderCameraPosition + playerCamera.transform.TransformVector(eyeOffset);
		renderCamera.projectionMatrix = HMDMatrix4x4ToMatrix4x4(
			SteamVR.instance.hmd.GetProjectionMatrix(
				Valve.VR.EVREye.Eye_Right,
				playerCamera.nearClipPlane,
				playerCamera.farClipPlane
			)
		);
		renderCamera.targetTexture = _rightEyeRenderTexture;

		renderCamera.Render();

		portalMaterial.SetInt ("_VREnabled", 1);
		portalMaterial.SetTexture("_RightEyeTexture", _rightEyeRenderTexture);
	}

	private void renderPortalTexture(Vector3 renderCameraPosition) {
		renderCamera.transform.rotation = playerCamera.transform.rotation;
		renderCamera.transform.position = renderCameraPosition;

		renderCamera.projectionMatrix = playerCamera.projectionMatrix;
		renderCamera.targetTexture = _leftEyeRenderTexture;
		portalMaterial.SetInt ("_VREnabled", 0);

		renderCamera.Render();

		portalMaterial.SetTexture ("_LeftEyeTexture", _leftEyeRenderTexture);
	}

	public void preparePortalRender(int depth, Vector3 origin) {
		if (DestinationPortal == null) {
			return;
		}

		Vector3 portalOffset = 
			DestinationPortal.gameObject.transform.position - transform.position;

		Vector3 newOrigin = origin + portalOffset;

		if (depth != 0) {

			foreach (Portal portal in DestinationPortal.getRoom().getPortals()) {
				if (portal != DestinationPortal)
					portal.preparePortalRender(depth - 1, newOrigin);
			}
		}

		if (RenderCameraIgnoredLayer != -1) {
			renderCamera.cullingMask = renderCamera.cullingMask & ~(1 << RenderCameraIgnoredLayer);
		}

		if (PortalManager.instance.EnableVR) {
			renderPortalTextureStereo(newOrigin);
		} else {
			renderPortalTexture(newOrigin);
		}

		if (RenderCameraIgnoredLayer != -1) {
			renderCamera.cullingMask = renderCamera.cullingMask | 1 << RenderCameraIgnoredLayer;
		}
	}

	// Use this for initialization
	void Awake () {
		if (cameraPositionHistorySize != PortalManager.instance.CameraPositionHistorySize) {
			cameraPositionHistorySize = PortalManager.instance.CameraPositionHistorySize;
		}

		if (RenderCameraIgnoredLayer == -1) {
			RenderCameraIgnoredLayer = PortalManager.instance.RenderCameraIgnoredLayer;
		}

		createRenderCamera();
		createRenderTextures();
		assignPortalMaterial();
		playerCamera = PortalManager.instance.getUsedCamera ();

		if (DestinationPortal != null) {
			if (DestinationPortal.DestinationPortal == null) {
					DestinationPortal.DestinationPortal = this;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		updateCameraDirection();
	}

	void OnTriggerEnter(Collider collider) {
		if (DestinationPortal == null) {
			return;
		}

		if (!collider.gameObject.CompareTag (playerCamera.tag)) {
			return;
		}

		float now = Time.time;

		if (now - lastCross < 0.1f) {
			return;
		}

		float dotProduct = Vector3.Dot (cameraDirection, transform.parent.forward);

		Debug.Log ("Dot product : " + dotProduct);

		if (dotProduct > 0f) {
			return;
		}

		Debug.Log ("Portal passed");

		lastCross = now;

		PortalManager.instance.getPlayerObject().transform.position += 
			DestinationPortal.transform.position - transform.position;
		PortalManager.instance.CurrentRoom = DestinationPortal.getRoom();
	}


	public void OnWillRenderObject() {
		if (DestinationPortal == null) {
			return;
		}

		//Si nous sommes dans une passe de pre-rendu, on ne fait rien.
		//Si la camera courante n'est pas la camera du joueur (la camera de rendu).
		if (Camera.current != playerCamera) {
			return;
		} else {
			//Si nous sommes dans une passe de rendu final, et que ce portail
			//n'est pas dans la salle ou se trouve le joueur, on ne fait rien.
			if (getRoom() != PortalManager.instance.CurrentRoom) {
				return;
			}
		}

		preparePortalRender(1, playerCamera.transform.position);
	}

	public void disableRender() {
		GetComponent<Renderer>().enabled = false;
	}

	public void enableRender() {
		GetComponent<Renderer> ().enabled = true;
	}

	void OnDrawGizmos() {
		
		if(DestinationPortal != null) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine (gameObject.transform.position, DestinationPortal.transform.position);
		}

		if (cameraDirection != null && playerCamera != null) {
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(playerCamera.transform.position, playerCamera.transform.position + cameraDirection);
		}

		Gizmos.color = Color.green;
		Vector3 forward_2 = transform.parent.forward / 2.0f;
		Vector3 forward_4 = forward_2 / 2.0f;
		Vector3 perpendicular = new Vector3(-forward_4.z, forward_4.y, forward_4.x) / 2.0f;
		Vector3 endArrow = transform.position + forward_2;
		Gizmos.DrawLine(transform.position, endArrow);
		Gizmos.DrawLine(endArrow, endArrow - forward_4 + perpendicular);
		Gizmos.DrawLine(endArrow, endArrow - forward_4 - perpendicular);
		Gizmos.DrawLine(transform.position - perpendicular, transform.position + perpendicular);
	}
}
