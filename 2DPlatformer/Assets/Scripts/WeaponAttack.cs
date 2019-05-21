using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("I hit a " + collider.gameObject.name);
        if (collider.gameObject.tag == "enemy")
        {
            // TODO Call function for enemy hit/die animation
            // TODO Call function for player animation from successful hit e.g. SFX,VFX
            // Score change?
            GameManager.instance.AddScore(100);
            Debug.Log(GameManager.instance.GetScore());
			collider.gameObject.SetActive(false);
        }
    }
}
