using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalkScript : MonoBehaviour {

    Vector3 previousPos;

    [Range(0.01f, 1.0f)]
    public float detectionTolerance = 0.05f;

    public GameObject lineRendererPrefab;
    private LineRenderer lr;

    public bool isDrawing = false, startDrawing = false;


	void Start ()
    {
        previousPos = transform.position;
        
	}
	

	void Update () {

        Vector3 diffPos = transform.position - previousPos;
        if (isDrawing && (diffPos.x > detectionTolerance || diffPos.y > detectionTolerance || diffPos.z > detectionTolerance))
        {           
            //print("Updating line points.");
            lr.SetPosition(lr.positionCount++, transform.position);
            previousPos = transform.position;
        }

	}


    void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.tag.Equals("Wall"))
            return;

        print("Trigger object : " + col.gameObject.name);

        if (!isDrawing)
        {
            print("Start drawing (" + col.contacts[0] + ")");
            lr = GameObject.Instantiate(lineRendererPrefab).GetComponent<LineRenderer>();
            lr.SetPositions(new Vector3[] { col.transform.position });
        }

        isDrawing = true;
    }


    void OnCollisionExit(Collision col)
    {
        print("End drawing");
        isDrawing = false;
    }

}
