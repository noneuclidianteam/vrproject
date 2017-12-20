using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MonsterScript : MonoBehaviour
{

    public bool isDebug;

    [Range(0.01f, 1.0f)]
    public float monsterSpeed = 0.3f;

    public float followTime = 3.0f;
    public float playerDetectionTolerance = 2.0f;
    private float playerHeadSpeed;
    private float startFollowTime;

    public Transform playerTransform;

    private Vector3 prevPlayerHeadPos = Vector3.zero;

    public Hand hand1, hand2;

    public bool isFollowingPlayer = false;


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
                   
        // Le personnage est détecté par le monstre
        if (playerHeadSpeed > playerDetectionTolerance)
        {
            startFollowTime = Time.time;
            isFollowingPlayer = true;
        }
        
        if (isFollowingPlayer)
            MoveToPlayer();       
        else
        {
            // Déplacement aléatoire du monstre
            if ((int)(Time.time * 100) % 20 == 0)
            {
                transform.Translate(new Vector3(Random.Range(-15, 15) * monsterSpeed * Time.deltaTime, 0.0f, Random.Range(-15, 15) * monsterSpeed) * Time.deltaTime);
                transform.Rotate(new Vector3(0.0f, Random.Range(-45.0f, 45.0f), 0.0f));
            }
        }

        prevPlayerHeadPos = playerTransform.position;
    }


    private void MoveToPlayer()
    {
        if (Time.time - startFollowTime > followTime)
        {
            isFollowingPlayer = false;
            return;
        }

        print("Monster is moving ...");
        transform.LookAt(playerTransform);
        transform.Translate((playerTransform.position - transform.position) * Time.deltaTime * monsterSpeed);
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player") && isFollowingPlayer)
        {
            print("You died !");           
        }
    }
}
