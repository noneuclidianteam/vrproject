using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalkScript : MonoBehaviour {

    Vector3 previousPos;

    [Range(0.001f, 0.01f)]
    public float detectionTolerance;

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
            print("Updating line points.");
            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 1, transform.position);
        }

	}


    void OnTriggerEnter(Collider col)
    {
        
    }

}
