using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAreaFollow : MonoBehaviour {

    //attach to main camera
    public Transform player;
	public float KillElevation = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, KillElevation, player.position.z);
    }
}
