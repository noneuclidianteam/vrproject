using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

public class Chalk : MonoBehaviour {

    [SerializeField]
    private Brush brush = null;

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

            if (PortalManager.instance.textureDrawing)
            {
                InkCanvas canvasObject = hit.collider.gameObject.GetComponent<InkCanvas>();
                if (canvasObject != null && brush != null)
                {
                    canvasObject.Paint(brush, hit.point);
                }
            } else
            {
                if (hit.collider.gameObject.tag == "Portal")
                {
                    return;
                }
                emitter.transform.position = hit.point - transform.forward * 0.01f;
                emitter.Play();
                return;
            }
		}
        emitter.Pause();
    }

	public void OnDrawGizmos() {
		Vector3 size = transform.forward * lineCastSize;
		Vector3 lineCastStart = transform.position - size;
		Vector3 lineCastEnd = transform.position + size;
		Gizmos.color = Color.red;
		Gizmos.DrawLine (lineCastStart, lineCastEnd);
	}
}
