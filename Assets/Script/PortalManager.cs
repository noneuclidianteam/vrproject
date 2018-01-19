using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour {

	public static PortalManager instance = null;

	public bool EnableVR = false;
	public bool PortalBlur = false;
	public Camera Camera;
	public Camera VRCamera;
	public GameObject VRCameraRig;
	public GameObject Player;
	public int RenderCameraIgnoredLayer = 31;
	public Room CurrentRoom;

	private List<Room> rooms;

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

	public void Start() {
		rooms = new List<Room>(FindObjectsOfType<Room> ());
	}

	IEnumerator enablePortalDelayed(Portal portal)
	{
		yield return new WaitForSeconds (0.05f);
		portal.gameObject.SetActive (true);
	}

	public Camera getUsedCamera() {
		return EnableVR ? VRCamera : Camera;
	}

	public GameObject getPlayerObject() {
		return EnableVR ? VRCameraRig : Player; 
	}
}
