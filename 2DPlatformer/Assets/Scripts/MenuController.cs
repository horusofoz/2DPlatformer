using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static MenuController instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void Exit()
	{
    #if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
    #elif (UNITY_STANDALONE) 
        Application.Quit();
    #elif (UNITY_WEBGL)
        Application.OpenURL("about:blank");
    #endif
	}
}
