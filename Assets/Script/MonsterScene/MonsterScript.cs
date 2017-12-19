using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MonsterScript : MonoBehaviour
{

    public bool isDebug;

    [Range(0.01f, 1.0f)]
    public float monsterSpeed = 0.2f;

    public float playerDetectionTolerance = 2.0f;
    private float playerHeadSpeed;

    public Transform playerTransform;

    Vector3 prevPlayerHeadPos = Vector3.zero;

    public Hand hand1;    public Hand hand2;


    void Start()
    {
        if (isDebug)
            playerTransform = GameObject.Find("Player_debug").transform;

        prevPlayerHeadPos = playerTransform.position;
    }


    void Update ()
    {
                // VERSION VR

        //if(hand1.controller != null)
        //    hand1.controller.velocity;
        //if(hand2.controller != null)
        //    hand2.controller.velocity;
 

        playerHeadSpeed = (playerTransform.position - prevPlayerHeadPos).magnitude / Time.deltaTime;

        // Contrôle sur la vitesse du joueur + déplacement du monstre si tolérance dépassée
        if (playerHeadSpeed > playerDetectionTolerance)
        {
            print("Monster is moving ...");
            transform.LookAt(playerTransform);
            transform.Translate((playerTransform.position - transform.position) * Time.deltaTime);
        }
        else
        {
            // Déplacement du monstre
            if ((int)(Time.time * 100) % 20 == 0)
            {
                transform.Translate(new Vector3(Random.Range(-1.0f, 1.0f) * monsterSpeed, 0.0f, Random.Range(-1.0f, 1.0f) * monsterSpeed));
            }
        }

        prevPlayerHeadPos = playerTransform.position;
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player"))
        {
            print("You died !");           
        }
    }
}
