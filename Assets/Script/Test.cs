using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public GameObject renderCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnWillRenderObject() {
		PortalCamera c = renderCamera.GetComponent<PortalCamera> ();
		c.RenderIntoMaterial (GetComponent<Renderer>().material);
	}
}
