using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] private GameObject _optionsScreen;
    public void OnSettings()
    {
        Time.timeScale = 0;
        _optionsScreen.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        _optionsScreen.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
