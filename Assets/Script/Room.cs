using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

	private List<Portal> portals;

	public List<Portal> getPortals() {
		return portals;
	}

	// Use this for initialization
	void Start () {
		portals = new List<Portal> (GetComponentsInChildren<Portal> ());

		foreach (Portal portal in portals) {
			portal.setRoom (this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
