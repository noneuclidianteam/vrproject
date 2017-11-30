using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarDetectionScript : MonoBehaviour
{

    public static bool hasToEnablePillar;
    
    public DirectionVector direction;

    private Camera mainCamera;
    private Vector3 lastCamPos;
    private Vector3 directionVector;

    void Start ()
    {
        mainCamera = Camera.main;
        hasToEnablePillar = false;

        //directionVector = direction.Equals(DirectionVector.Forward) ? transform.forward : (direction.Equals(DirectionVector.Backward) ? -transform.forward : transform.right);

        directionVector = direction.Equals(DirectionVector.Forward) ? transform.forward :  -transform.forward ;

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
        

        hasToEnablePillar = Vector3.Dot(currCamPos - lastCamPos, directionVector) <= 0f;


        print(Vector3.Dot(currCamPos - lastCamPos, directionVector) + " to enable : " + hasToEnablePillar);
    }
}


public enum DirectionVector
{
    Forward,
    Right,
    Backward
}
