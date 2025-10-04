using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private CanvasGroup levelWonUI;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private CanvasGroup pauseUI;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private CanvasGroup levelLostUI;
    [SerializeField] private GameObject lostPanel;
    [SerializeField] private GameObject pauseBtn;
    [SerializeField] private Text timerText;
    [SerializeField] private float gameTime = 30f;
    [SerializeField] Text levelText;
    private bool isBlinking = false;
    private bool isPaused = false;
    [FormerlySerializedAs("pipeController")] [SerializeField] GameObject pipeManager;

    private void Awake()
    {
        levelWonUI.alpha = 0f;
        winPanel.transform.localPosition = new Vector2(0, +Screen.height);
        pauseUI.alpha = 0f;
        pausePanel.transform.localPosition = new Vector2(0, +Screen.height);
        levelLostUI.alpha = 0f;
        lostPanel.transform.localPosition = new Vector2(0, +Screen.height);
        StartCoroutine(TimerCountdown());

        int cycleCount = PlayerPrefs.GetInt("levelCycleCount", 0);
        int currentLevel = GameController.levelToLoad;
        if (currentLevel == 0) currentLevel = 1;

        int displayLevel = cycleCount * 10 + currentLevel;
        levelText.text = "LEVEL " + displayLevel;
    }


    public void GameWon()
    {
        levelWonUI.gameObject.SetActive(true);
        levelWonUI.LeanAlpha(1, 0.5f);
        pauseBtn.SetActive(false);
        Destroy(timerText);
        winPanel.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;

        if (GameController.levelToLoad < 10)
        {
            GameController.levelToLoad++;
            PlayerPrefs.SetInt("levelToLoad", GameController.levelToLoad);
        }
        else
        {
            GameController.levelToLoad = 1;
            PlayerPrefs.SetInt("levelToLoad", GameController.levelToLoad);

            int cycleCount = PlayerPrefs.GetInt("levelCycleCount", 0);
            PlayerPrefs.SetInt("levelCycleCount", cycleCount + 1);
        }

        PlayerPrefs.Save();
    }

    public void ShowPauseMenu()
    {
        pauseUI.gameObject.SetActive(true);
        pauseUI.LeanAlpha(1, 0.5f);
        pauseBtn.SetActive(false);
        pausePanel.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
        isPaused = true;
        pipeManager.SetActive(false);
    }

    public void DisablePauseMenu()
    {
        pauseUI.LeanAlpha(0, 0.5f);
        pausePanel.LeanMoveLocalY(+Screen.height, 0.5f).setEaseInExpo();
        pauseBtn.SetActive(true);
        isPaused = false;
        pipeManager.SetActive(true);
        Invoke(nameof(DisablePauseUI), 0.5f);
    }

    private void DisablePauseUI()
    {
        pauseUI.gameObject.SetActive(false);
    }
    private IEnumerator TimerCountdown()
    {
        while (gameTime > 0)
        {
            if (!isPaused) // Only update the timer if the game is not paused
            {
                gameTime -= Time.deltaTime;
                UpdateTimerUI();

                if (gameTime <= 15f && !isBlinking)
                {
                    StartCoroutine(BlinkTime());
                }
            }
            yield return null;
        }

        GameOver();
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private IEnumerator BlinkTime()
    {
        isBlinking = true;
        Color originalColor = timerText.color;
    
        while (gameTime <= 15f && gameTime > 0)
        {
            timerText.color = (timerText.color == Color.red) ? Color.white : Color.red;
            yield return new WaitForSeconds(0.5f);
        }

        timerText.color = originalColor;
        isBlinking = false;
    }
    public void GameOver()
    {
        pauseBtn.SetActive(false);
        levelLostUI.gameObject.SetActive(true);
        timerText.enabled = false;
        pipeManager.SetActive(false);
        levelLostUI.LeanAlpha(1, 0.5f);
        lostPanel.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
    }
}
