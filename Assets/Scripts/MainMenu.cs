using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartSimulator()
    {
        Debug.Log("Scene changed!");
        SceneManager.LoadScene("ARScene");
    }
    public void QuitSimulator()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
