using UnityEngine;
using System.Collections;
using UnityEngine.XR;

public class PortalCamera : MonoBehaviour {

	public int textureSize = 1024;

	public Camera _vrEye;

	private Camera _cameraForPortal;
	private RenderTexture _leftEyeRenderTexture;
	private RenderTexture _rightEyeRenderTexture;

	private void Awake() { 
		//_vrEye = SteamVR.instance.
		_cameraForPortal = GetComponent<Camera>();
		_cameraForPortal.enabled = false;
		_leftEyeRenderTexture = new RenderTexture(textureSize, textureSize, 24);
		_rightEyeRenderTexture = new RenderTexture(textureSize, textureSize, 24);
	}

	public void RenderIntoMaterial(Material material) {
		if (Camera.current == _vrEye) {

		}
			
			transform.localRotation = _vrEye.transform.localRotation; // left eye
			Vector3 eyeOffset = SteamVR.instance.eyes[0].pos;
			transform.localPosition = _vrEye.transform.position + _vrEye.transform.TransformVector(eyeOffset);
			_cameraForPortal.projectionMatrix = HMDMatrix4x4ToMatrix4x4(
				SteamVR.instance.hmd.GetProjectionMatrix(
					Valve.VR.EVREye.Eye_Left,
					_vrEye.nearClipPlane,
					_vrEye.farClipPlane
				)
			);
			_cameraForPortal.targetTexture = _leftEyeRenderTexture;
			_cameraForPortal.Render();


			material.SetTexture("_LeftTex", _leftEyeRenderTexture); // right eye
			eyeOffset = SteamVR.instance.eyes[1].pos;
			transform.localPosition = _vrEye.transform.position + _vrEye.transform.TransformVector(eyeOffset);
			_cameraForPortal.projectionMatrix = HMDMatrix4x4ToMatrix4x4(
				SteamVR.instance.hmd.GetProjectionMatrix(
					Valve.VR.EVREye.Eye_Right,
					_vrEye.nearClipPlane,
					_vrEye.farClipPlane
				)
			);
			_cameraForPortal.targetTexture = _rightEyeRenderTexture;
			_cameraForPortal.Render();
			material.SetTexture("_RightTex", _rightEyeRenderTexture);
		//} 
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
}