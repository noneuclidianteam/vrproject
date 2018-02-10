using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chalk : MonoBehaviour {

	[SerializeField]
	private ParticleSystem emitter;

	[SerializeField]
	[Range(0.001f, 0.3f)]
	private float lineCastSize = 1.0f;

	private Vector3 lineCastStart;
	private Vector3 lineCastEnd;

	public void Awake()
	{
		emitter.Pause();
	}

	public void Update()
	{
		RaycastHit hit;

		Vector3 size = transform.forward * lineCastSize;
		Vector3 lineCastStart = transform.position - size;
		Vector3 lineCastEnd = transform.position + size;

		if (Physics.Linecast(lineCastStart, lineCastEnd, out hit)) {
			emitter.transform.position = hit.point;
			emitter.Play();
		} else {
			emitter.Pause();
		}
	}

	public void OnDrawGizmos() {
		Vector3 size = transform.forward * lineCastSize;
		Vector3 lineCastStart = transform.position - size;
		Vector3 lineCastEnd = transform.position + size;
		Gizmos.color = Color.red;
		Gizmos.DrawLine (lineCastStart, lineCastEnd);
	}
}
