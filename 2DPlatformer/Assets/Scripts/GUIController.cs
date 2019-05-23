using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIController : MonoBehaviour {

    private TMP_Text scoreLabel;
    private GameObject heart1, heart2, heart3;

    

    // Use this for initialization
    void Start () {
        scoreLabel = GameObject.Find("Score Label").GetComponent<TMP_Text>();




        heart1 = GameObject.Find("Heart1");
        heart2 = GameObject.Find("Heart2");
        heart3 = GameObject.Find("Heart3");
        heart1.SetActive(true);
        heart2.SetActive(true);
        heart3.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        scoreLabel.SetText(GameManager.instance.GetScore().ToString());
        int health = GameManager.instance.GetHealth();

        switch (health)
        {
            case 1:
                heart1.SetActive(true);
                heart2.SetActive(false);
                heart3.SetActive(false);
                break;
            case 2:
                heart1.SetActive(true);
                heart2.SetActive(true);
                heart3.SetActive(false);
                break;
            case 3:
                heart1.SetActive(true);
                heart2.SetActive(true);
                heart3.SetActive(true);
                break;
            default:
                heart1.SetActive(false);
                heart2.SetActive(false);
                heart3.SetActive(false);
                break;
        }
    }
}
