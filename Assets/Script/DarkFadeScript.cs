using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkFadeScript : MonoBehaviour {


    float exposure;
    public float startExposure = 3.0f;
    public float exposureReductionByDecimal = 0.005f;
    public float currentExposure;

    private string currentRoomName;
    public static bool isInSafeZone = true;

    void Start()
    {
        exposure = startExposure;
        RenderSettings.skybox.SetFloat("_Exposure", startExposure);
        DynamicGI.UpdateEnvironment();
        currentRoomName = GetComponent<PortalManager>().CurrentRoom.name;

        InvokeRepeating("ReduceExposure", 1.0f, 0.1f);
    }

    void Update()
    {
        currentExposure = RenderSettings.skybox.GetFloat("_Exposure");
    }


    // Mise à jour de l'exposition, pour le fondu au noir
    void ReduceExposure()
    {        
        currentRoomName = GetComponent<PortalManager>().CurrentRoom.name;

        // Cas de la Safe Zone (ou de la salle tuto)
        if(currentRoomName.Equals("RoomTuto") || currentRoomName.Equals("Room1"))
        {
            if (exposure > startExposure)
                return;
            exposure += exposureReductionByDecimal * 10.0f;
        }
        else
        {
            if (exposure <= 0.0f)
                return;
            exposure -= exposureReductionByDecimal;
        }

        RenderSettings.skybox.SetFloat("_Exposure", exposure);
        DynamicGI.UpdateEnvironment();
    }

}
