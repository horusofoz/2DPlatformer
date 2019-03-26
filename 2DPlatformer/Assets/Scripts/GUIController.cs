using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIController : MonoBehaviour {

    private TMP_Text scoreLabel;
    private TMP_Text healthLabel;

    // Use this for initialization
    void Start () {
        scoreLabel = GameObject.Find("Score Label").GetComponent<TMP_Text>();
        healthLabel = GameObject.Find("Health Label").GetComponent<TMP_Text>();
    }
	
	// Update is called once per frame
	void Update () {
        scoreLabel.SetText(GameManager.instance.getScore().ToString());
        healthLabel.SetText(GameManager.instance.getHealth().ToString());
    }
}
