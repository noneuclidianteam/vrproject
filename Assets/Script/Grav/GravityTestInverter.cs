using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTestInverter : MonoBehaviour {

	public GameObject Normal;

	public GameObject Innverted;

	bool IsInverted =false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider ObjectCol)
	{
		if (ObjectCol.tag == "InverterPortal") 
		{
			print("Trigger reached");
			if (IsInverted == false) {
				Normal.SetActive (false);
				Innverted.SetActive (true);
			} else {
				Normal.SetActive (true);
				Innverted.SetActive (false);
			}

			IsInverted = !IsInverted;
		}
	}
}
