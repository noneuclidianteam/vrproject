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

        if (direction.Equals(DirectionVector.Forward))
            directionVector = transform.forward;
        else if (direction.Equals(DirectionVector.Backward))
            directionVector = -transform.forward ;
        else
            directionVector = transform.right;
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
        
        hasToEnablePillar = Vector3.Dot(currCamPos - lastCamPos, directionVector) < 0f;

        print(directionVector + " DOT " + Vector3.Dot(currCamPos - lastCamPos, directionVector));
    }
}


public enum DirectionVector
{
    Forward,
    Right,
    Backward
}
