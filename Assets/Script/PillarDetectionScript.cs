using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarDetectionScript : MonoBehaviour
{

    public static bool hasToEnablePillar;
    
    private Camera mainCamera;
    private Vector3 lastCamPos; 

    void Start ()
    {
        mainCamera = Camera.main;
        hasToEnablePillar = false;
	}
	

	void Update ()
    {
        lastCamPos = mainCamera.transform.position;
    }


    void OnTriggerEnter(Collider collider)
    {
        //if (!collider.gameObject.CompareTag(mainCamera.tag))
        //   return;

        Vector3 currCamPos = mainCamera.transform.position;

        print(Vector3.Dot(currCamPos - lastCamPos, transform.forward));

        hasToEnablePillar = Vector3.Dot(currCamPos - lastCamPos, transform.forward) < 0f;
    }
}


enum directionVector
{
    Forward,

}
