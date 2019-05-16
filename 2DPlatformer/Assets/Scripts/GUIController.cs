using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIController : MonoBehaviour {

    private TMP_Text scoreLabel;
    private Image heart1, heart2, heart3;

    

    // Use this for initialization
    void Start () {
        scoreLabel = GameObject.Find("Score Label").GetComponent<TMP_Text>();
        heart1 = GameObject.Find("Heart1").GetComponent<Image>();
        heart2 = GameObject.Find("Heart2").GetComponent<Image>();
        heart3 = GameObject.Find("Heart3").GetComponent<Image>();
        heart1.enabled = true;
        heart2.enabled = true;
        heart3.enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
        scoreLabel.SetText(GameManager.instance.GetScore().ToString());
        int health = GameManager.instance.GetHealth();
        if (health < 3)
        {
            heart3.enabled = false;
            if (health < 2)
            {
                heart2.enabled = false;
                if (health < 1)
                {
                    heart1.enabled = false;
                }
                else
                {
                    heart1.enabled = true;
                }
            }
            else
            {
                heart2.enabled = true;
            }
        }
        else
        {
            heart3.enabled = true;
        }
    }
}
