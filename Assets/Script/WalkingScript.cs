using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class WalkingScript : MonoBehaviour {


    [EventRef]
    public string walking_state;

    public float soundDetectionTolerance = 0.01f;
    private FMOD.Studio.EventInstance walking_fmod;

    private bool StartSound = false;
    private bool StopSound = false;
    private bool isSoundPlaying = false;

    private Vector3 previousPos;

    private float[] lastPositions = new float[10];

    private int i = 0;

    
    void Start ()
    {
        walking_fmod = RuntimeManager.CreateInstance(walking_state);
        previousPos = transform.position;
    }
	

	void Update ()
    {
        float medianValue = GetMedianPosition();
        walking_fmod.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));       

        if (!isSoundPlaying && medianValue > soundDetectionTolerance)
        {
            StartSound = true;
            StopSound = false;
        }
        else if(isSoundPlaying && medianValue < 0.00001f)
        {
            StartSound = false;
            StopSound = true;
        }
              
        previousPos = transform.position;
        ManageSound();
    }


    float GetMedianPosition()
    {
        if (i >= lastPositions.Length) i = 0;
        lastPositions[i++] = Vector3.Distance(previousPos, transform.position);
      
        float median = lastPositions[0];
        for (int j = 1; j < lastPositions.Length; j++)
            median += lastPositions[j];

        return (median / lastPositions.Length);
    }


    void ManageSound()
    {
        if(StartSound && !isSoundPlaying)
        {
            //print("Playing walk sound");
            walking_fmod.start();
            isSoundPlaying = true;
            StartSound = false;
        }
        else if (StopSound && isSoundPlaying)
        {
            //print("Stoping walk sound");
            walking_fmod.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            isSoundPlaying = false;
            StopSound = false;
        }
    }

}
