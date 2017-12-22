using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalParameters : MonoBehaviour {

	public static PortalParameters instance = null;

	public bool EnableVR = false;
	public bool PortalBlur = false;
	public Camera Camera;
	public Camera VRCamera;
	public GameObject VRCameraRig;
	public GameObject Player;

	void Awake()
	{
		if (instance == null)
			instance = this;

		if (EnableVR) {
			VRCameraRig.SetActive (true);
			Player.SetActive (false);
		} else {
			VRCameraRig.SetActive (false);
			Player.SetActive (true);
		}
	}

	public Camera getUsedCamera() {
		return EnableVR ? VRCamera : Camera;
	}

	public GameObject getPlayerObject() {
		return EnableVR ? VRCameraRig : Player; 
	}
}
