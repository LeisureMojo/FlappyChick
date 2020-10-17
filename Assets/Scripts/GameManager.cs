using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // create a event to notify other scripts
    public delegate void GameDelegate();  
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;
    
    // this game manager has to be singleton
    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countDownPage;
    public Text scoreText;

    public AudioSource bgm;

    public int Score {get {return score;}}
    private enum PageState {
        None,
        Start,
        GameOver,
        CountDown
    }

    int score = 0;

    bool gameOver = true;

    public bool GameOver 
    {get {return gameOver;}}

    void Awake() 
    {
        Instance = this;
    }

    void Update(){}
    private void OnEnable() 
    {
        // subsribe to these events
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
    }

    private void OnDisable() 
    {
        // unsubscribe these events
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted();  // event sent to TapController
        score = 0;
        gameOver = false;
        bgm.Play();
    }

    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        bgm.Stop();
        SetPageState(PageState.GameOver);
    }

    void OnPlayerScored(){
        score++;
        scoreText.text = $"Score: {score}";
    }

    void SetPageState(PageState state) {
        startPage.SetActive(false);
        gameOverPage.SetActive(false);
        countDownPage.SetActive(false);
        switch(state){
            case PageState.None:
                break;
            case PageState.GameOver:
                gameOverPage.SetActive(true);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                break;
            case PageState.CountDown:
                countDownPage.SetActive(true);
                break;
        }
    }
    
    public void ConfirmGameOver()
    {
        // activated when replay button is hit
        OnGameOverConfirmed(); //event sent to TapController
        scoreText.text = $"Score: 0";
        SetPageState(PageState.Start);
    }

    public void StartGame(){
        // activated when play button is hit
        SetPageState(PageState.CountDown);
    }
    
}
