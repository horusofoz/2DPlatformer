using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
	}
	public void LoadNextScene()
	{
		int thisScene = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(thisScene + 1);
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
