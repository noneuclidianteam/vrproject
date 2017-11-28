using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour {

	public Camera Camera;

	public float WalkSpeed, RunSpeed;
	public float CameraSpeed;

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");

		bool isRunning = Input.GetAxis ("Run") > 0;

		float speed = isRunning ? RunSpeed : WalkSpeed;

		this.transform.position += speed * v * transform.forward * Time.deltaTime;
		this.transform.position += speed * h * Vector3.Cross (transform.forward, transform.up) * Time.deltaTime;

		float cameraH = Input.GetAxis ("Mouse X");
		float cameraV = -Input.GetAxis ("Mouse Y");
		this.transform.Rotate (0f, CameraSpeed * cameraH, 0f);
		Camera.transform.Rotate (CameraSpeed * cameraV, 0f, 0f);
	}
}
