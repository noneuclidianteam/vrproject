using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalkScript : MonoBehaviour {

    Vector3 previousPos;

    [Range(0.01f, 1.0f)]
    public float detectionTolerance = 0.05f;

    public LineRenderer lr;


	void Start ()
    {
        previousPos = transform.position;
        lr.SetPositions(
            new Vector3[] {
                transform.position,
                transform.position
            });
	}
	

	void Update () {

        Vector3 diffPos = transform.position - previousPos;
        if (diffPos.x > detectionTolerance || diffPos.y > detectionTolerance || diffPos.z > detectionTolerance)
        {
            //print("Updating line points.");
            lr.SetPosition(lr.positionCount++, transform.position);
            previousPos = transform.position;
        }

	}


    void OnTriggerEnter(Collider col)
    {
        
    }

}
