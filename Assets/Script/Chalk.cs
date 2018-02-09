using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chalk : MonoBehaviour {

	[SerializeField]
	ParticleSystem emitter;

	[SerializeField]
	private int wait = 3;

	private int waitCount;

	public void Awake()
	{
		//GetComponent<MeshRenderer>().material.color = brush.Color;
		emitter.Pause();
	}

	public void FixedUpdate()
	{
		++waitCount;
		emitter.Pause();
	}

	public void OnCollisionStay(Collision collision)
	{
		if(waitCount < wait)
			return;
		waitCount = 0;

		foreach(var p in collision.contacts)
		{
			emitter.transform.position = p.point;
			emitter.Play ();
			//Debug.Log ("Test1");
		}
	}
}
