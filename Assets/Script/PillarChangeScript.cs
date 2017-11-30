using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarChangeScript : MonoBehaviour
{
    public GameObject currentPillar;

    public GameObject nextPillar;

    private bool isAltPillarActive = false;


    void OnTriggerEnter(Collider collider)
    {

       // print("hasToEnablePillar = " + PillarDetectionScript.hasToEnablePillar);

        if (!currentPillar.activeInHierarchy && !nextPillar.activeInHierarchy)
            return;

        if (PillarDetectionScript.hasToEnablePillar)
        {
           // print("Ready to enable");

            currentPillar.SetActive(isAltPillarActive);
            nextPillar.SetActive(!isAltPillarActive);

            isAltPillarActive = !isAltPillarActive;
            PillarDetectionScript.hasToEnablePillar = false;
        }
    }
    

}
