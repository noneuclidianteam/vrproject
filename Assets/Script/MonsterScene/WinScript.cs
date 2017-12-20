using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour {
    

	private float initialDepth;


		void Start()
		{
			initialDepth = transform.localScale.x;
		}

    void OnTriggerEnter(Collider col)
    {
        print("Victory !!!!");

				float newDepth = (transform.localScale.x == initialDepth ) ? initialDepth /2 : initialDepth;
				transform.localScale = new Vector3(newDepth, transform.localScale.y, transform.localScale.z);
    }
}
