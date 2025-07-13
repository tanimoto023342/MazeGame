using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;

/// <summary>
/// Handles Game scene GUI Components, End Game Menu, Total Score calculaction
/// and Timer and its mechanism
/// </summary>
public class GUIHandler : MonoBehaviour
{
    /// <summary>
    /// Used for Total Score calculation, can be changed for custom calculations
    /// </summary>
    public const int MAXIMUM_SCORE = 10000;

    public GameObject endGameMenu;
    public GameObject endGameText;
    public TMP_Text totalScore;
    public LevelHandler levelHandler;

    public Button restartButton;
    public Button quitButton;
//    public Button nextLevelButton;
    public Button pauseButton;
    public Button skipButton;
    public Button startFlowButton;

    public TMP_Text timerText;
    public Slider timerSlider;//???
    public GameObject gameover;
    public AudioSource gameBGM; // ゲームBGM用AudioSource（ステージ1-4用）
    public AudioSource gameBGMStage5to8; // ゲームBGM用AudioSource（ステージ5-8用）

    // After the Flow starts EndGame starts
    public static bool IsEndGame { get; set; } = false;

    public bool isDebug;
    [SerializeField]
    [Range(5, 30)]
    int defaultTimeLimit = 20; // To be edited in Editor
    int currentTime;

    private TextAsset iscanplayfile;
    private string iscanplayfilepath;
    private int iscanplaynum = 1;

    GameManager gm;

    void Awake()
    {
        gm = GetComponent<GameManager>();
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(GetBackToMainMenu);
   //     nextLevelButton.onClick.AddListener(GoToNextLevel);
        skipButton.onClick.AddListener(AccelerateFlow);
        startFlowButton.onClick.AddListener(gm.StartFlow);
    }

    void Start()
    {
        iscanplayfilepath = Environment.CurrentDirectory + "/Assets/Scripts/iscanplay.txt";
        if (!File.Exists(iscanplayfilepath))
        {
            File.WriteAllText(iscanplayfilepath, iscanplaynum.ToString());
        }
        else
        {
            iscanplaynum = int.Parse(File.ReadAllText(iscanplayfilepath));
        }
        Debug.Log(iscanplaynum);
        if (isDebug)
            LevelData.TimeLimit = defaultTimeLimit;

        bool isLastLevel = LevelData.LevelNumber ==
            (LevelData.IsFreeWorldMode ? SceneHandler.FreeWorldLevelCount : SceneHandler.LevelSelectLevelCount);
        if (LevelData.IsArcadeMode || isLastLevel)
     //       nextLevelButton.gameObject.SetActive(false);

        timerText.text = LevelData.TimeLimit.ToString();
        StartCoroutine("CountdownTimer");
        timerSlider.maxValue = LevelData.TimeLimit;
        timerSlider.value = 0;

        // ステージに応じてBGMを切り替え
        SwitchBGMByStage();
    }

    void Update()
    {
        if(!gm.flowsStarted)timerSlider.value += 1 * Time.deltaTime;
    }

    /// <summary>
    /// Coroutine that starts counting down the Timer every second until it reaches 0
    /// after which it's Game Over
    /// </summary>
    IEnumerator CountdownTimer()
    {
        currentTime = LevelData.TimeLimit;
        while (currentTime != 0)
        {
            yield return new WaitForSeconds(1);
            currentTime -= 1;
            timerText.text = currentTime.ToString();
        }
        ShowEndGameMenu(isWon: false);
        StopCoroutine("CountdownTimer");
    }

    void OnDestroy()
    {
        restartButton.onClick.RemoveListener(RestartGame);
        quitButton.onClick.RemoveListener(GetBackToMainMenu);
        skipButton.onClick.RemoveListener(AccelerateFlow);
        startFlowButton.onClick.RemoveListener(gm.StartFlow);
    }

    /// <summary>
    /// A popup GUI that shows the End Game menu with the Total Score, Restart, Quit and Next Level buttons
    /// </summary>
    /// <param name="isWon">Used to determine if the player won or lost the game.
    /// The End Game Menu changes accordingly.</param>
    public void ShowEndGameMenu(bool isWon)
    {
        // ゲーム終了時にBGMを停止
        if (gameBGM != null && gameBGM.isPlaying)
        {
            gameBGM.Stop();
        }
        if (gameBGMStage5to8 != null && gameBGMStage5to8.isPlaying)
        {
            gameBGMStage5to8.Stop();
        }

        gm.StopAllCoroutines();
        pauseButton.enabled = false;
        skipButton.gameObject.SetActive(false);
        // Pauses the game to prevent GUI interaction and player Input
        Time.timeScale = 0f;
        PauseControl.GameIsPaused = true;

        if (isWon)
        {
            endGameText.name = "You Won";
            endGameText.GetComponent<TMP_Text>().text = "YOU WON!";
            string score = CalculateTotalScore();
            totalScore.text = score;
            levelHandler.PlayWinningAudio();
            //?X?R?A???
            int level = LevelData.LevelNumber;
            int currentScore = int.Parse(score);
            int previousScore = PlayerPrefs.GetInt("HighScore_Level" + level, 0);
            if (currentScore > previousScore)
            {
                PlayerPrefs.SetInt("HighScore_Level" + level, currentScore);
                PlayerPrefs.Save();
            }
        }
        else
        {
            endGameText.name = "You Lost";
            endGameText.GetComponent<TMP_Text>().text = "YOU LOST!";
            totalScore.text = "0"; // If the player looses the remaining Timer is unnecessary
            gameover.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("UI/ゲームオーバー");
            levelHandler.PlayGameOverAudio(); // �Q�[���I�[�o�[����BGM���Đ�
        }

        endGameMenu.SetActive(true);
    }

    /// <summary>
    /// Resumes the Game, restores defaults, restarts and shuffles the current level
    /// </summary>
    public void RestartGame()
    {
        if (PauseControl.GameIsPaused) // Resumes the game
        {
            PauseControl.GameIsPaused = false;
            Time.timeScale = 1;
        }
        endGameMenu.SetActive(false);

        IsEndGame = false;
        skipButton.gameObject.SetActive(false);
        pauseButton.enabled = true;
        startFlowButton.enabled = true;
        levelHandler.ResetLevel();
        timerText.text = LevelData.TimeLimit.ToString();

        if (LevelData.IsFreeWorldMode ||true)
        {
            SceneHandler.LoadLevel(LevelData.LevelNumber);
        }

        StartCoroutine("CountdownTimer"); // Starts the CountdownTimer coroutine for the new game
    }

    /// <summary>
    /// Resumes the Game, restores defaults and changes scene to MainMenu
    /// </summary>
    void GetBackToMainMenu()
    {
        if (PauseControl.GameIsPaused)
        {
            PauseControl.GameIsPaused = false;
            Time.timeScale = 1;
        }
        endGameMenu.SetActive(false);

        SceneHandler.LoadMainMenuScene();
    }

    /// <summary>
    /// Navigation to the next level
    /// </summary>
    void GoToNextLevel()
    {
        if (PauseControl.GameIsPaused)
        {
            PauseControl.GameIsPaused = false;
            Time.timeScale = 1;
        }
        endGameMenu.SetActive(false);

        SceneHandler.LoadLevel(LevelData.LevelNumber + 1);
    }

    /// <summary>
    /// Setup the EndGame routine and sets IsEndGame
    /// </summary>
    public void SetEndGameScene()
    {
        IsEndGame = true;
        StopCoroutine("CountdownTimer");
        skipButton.gameObject.SetActive(true);
        startFlowButton.enabled = false;
    }

    /// <summary>
    /// Basic Total Score calculation using set maximum metric
    /// </summary>
    /// <returns>Total Score string</returns>
    string CalculateTotalScore()
    {
        // Minimum Score: 0
        // Maximum Score: 10000

        //double weight = LevelData.IsArcadeMode ? 1.0 : LevelSelectHandler.MaxTimeLimit / (double)LevelData.TimeLimit;
        double weight = LevelData.IsArcadeMode ? 1.0 : 1.0 + (LevelData.LevelNumber - 1.0) / 10.0;
        //double notNormalizedScore = currentTime / (double)LevelData.TimeLimit / weight;
        double notNormalizedScore = 500 * (LevelData.TimeLimit - timerSlider.value) * weight;
        int score = Mathf.RoundToInt((float)(notNormalizedScore * MAXIMUM_SCORE));

        return score.ToString();
    }

    /// <summary>
    /// Used for flow acceleration using the "Skip" button
    /// </summary>
    void AccelerateFlow()
    {
        Time.timeScale = 4;
    }

    /// <summary>
    /// ステージに応じてBGMを切り替えるメソッド
    /// </summary>
    void SwitchBGMByStage()
    {
        // 現在のステージを取得
        int currentStage = LevelData.LevelNumber;

        // ステージ1-4の場合
        if (currentStage >= 1 && currentStage <= 4)
        {
            // ステージ5-8のBGMが再生中なら停止
            if (gameBGMStage5to8 != null && gameBGMStage5to8.isPlaying)
            {
                Debug.Log("5-8停止");
                gameBGMStage5to8.Stop();
            }

            // ステージ1-4のBGMを再生
            if (gameBGM != null && !gameBGM.isPlaying)
            {
                gameBGM.Play();
            }
        }
        // ステージ5-8の場合
        else if (currentStage >= 5 && currentStage <= 8)
        {
            // ステージ1-4のBGMが再生中なら停止
            if (gameBGM != null && gameBGM.isPlaying)
            {
                Debug.Log("1-4停止");
                gameBGM.Stop();
            }

            // ステージ5-8のBGMを再生
            if (gameBGMStage5to8 != null && !gameBGMStage5to8.isPlaying)
            {
                gameBGMStage5to8.Play();
            }
        }
        else{
            Debug.Log("例外に分岐しました");
        }
    }
}
