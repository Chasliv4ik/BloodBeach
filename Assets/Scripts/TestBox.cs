using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestBox : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (!isLocalPlayer) return;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * 10);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * 10);
        }
	}
}
