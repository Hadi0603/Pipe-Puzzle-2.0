using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    public static int levelToLoad;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject pauseBtn;
    [FormerlySerializedAs("uiManager")] public UIController uiController;

    private void Start()
    {
        levelToLoad = PlayerPrefs.GetInt("levelToLoad", 1);
    }

    public void Play()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void ReplayBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseBtn()
    {
        uiController.ShowPauseMenu();
    }

    public void ResumeBtn()
    {
        uiController.DisablePauseMenu();
    }

    public void MenuBtn()
    {
        SceneManager.LoadScene(0);
    }

    public void ClearBtn()
    {
        PlayerPrefs.DeleteAll();
    }

    public void QuitBtn()
    {
        Application.Quit();
    }
}