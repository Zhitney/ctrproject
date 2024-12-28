using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{   

    public void SelectStage(string stageName)
    {
        if (GameManager.Instance == null)
    {
        Debug.LogError("GameManager.Instance is null. Ensure GameManager is in the scene.");
        return;
    }

        GameManager.Instance.selectedStage = stageName;
        SceneManager.LoadScene("CarSelect");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
