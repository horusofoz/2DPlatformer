using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    //attach to main camera
    public Transform player;
    public Vector3 offset;

	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z); //set offset values to x=0 y=3 z=-10
	}
}
