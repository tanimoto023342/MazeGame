using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// Used to handle the load of new Scenes in the game and to configure the static default values.
/// </summary>
public static class SceneHandler
{
    public static int LevelSelectLevelCount = 8;
    //public static int LevelSelectLevelCount = Resources.LoadAll("LevelSelectLevels", typeof(TextAsset)).Length;
    public static int FreeWorldLevelCount = Resources.LoadAll("FreeWorldLevels", typeof(TextAsset)).Length;
    static bool[] tutorialsPlayed = new bool[3];

    private static string iscanplayfilepath;
    public  static int iscanplaynum = 1;

    public static void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public static void LoadCreditScene()
    {
        SceneManager.LoadScene("Credit", LoadSceneMode.Single);
    }

    public static void LoadLevelSelectScene(bool isFreeWorldMode)
    {
        LevelData.IsFreeWorldMode = /*!isFreeWorldMode*/false;
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
    }

    public static void LoadArcadeGameScene()
    {
        GUIHandler.IsEndGame = false;
        PauseControl.GameIsPaused = false;
        LevelData.IsFreeWorldMode = false;
        LevelData.IsArcadeMode = true;
        LevelData.Starts = new();
        LevelData.Ends = new();
        LevelData.defaultStart = null;
        LevelData.defaultEnd = null;
        LevelData.GamePieces = null;
        LevelData.BoardSize = 10;
        LevelData.TimeLimit = LevelData.Difficulty == Difficulty.Normal ? 40 : 
            (LevelData.Difficulty == Difficulty.Hard ? 30 : 60);

        //if (true)//チュートリアルに分岐しないよう変更
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        /*
        else
        {
            tutorialsPlayed[1] = true;
            SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
        }
        */
    }

    public static void LoadLevel(int levelNumber)
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
        if (iscanplaynum < levelNumber) return;
        GUIHandler.IsEndGame = false;
        PauseControl.GameIsPaused = false;
        LevelData.IsArcadeMode = false;
        LevelData.LevelNumber = levelNumber;
        LevelData.Starts = new();
        LevelData.Ends = new();
        LevelData.defaultStart = null;
        LevelData.defaultEnd = null;
        LevelData.GamePieces = null;

        if (/*LevelData.IsFreeWorldMode &&*/ false)
            LevelData.lvlData = Resources.Load<TextAsset>("FreeWorldLevels/Level" + levelNumber.ToString())
                .text.Split(Environment.NewLine);
        else
            LevelData.lvlData = Resources.Load<TextAsset>("LevelSelectLevels/Level" + levelNumber.ToString())
                .text.Split(Environment.NewLine);

        LevelData.BoardSize = int.Parse(LevelData.lvlData[0]);

        int[] difficulties = Array.ConvertAll(LevelData.lvlData[1].Split(';'), int.Parse);
        LevelData.TimeLimit = LevelData.Difficulty == Difficulty.Normal ? difficulties[1] :
            (LevelData.Difficulty == Difficulty.Hard ? difficulties[2] : difficulties[0]);

        if ((LevelData.IsFreeWorldMode && true) || (!LevelData.IsFreeWorldMode && true))//チュートリアルに分岐しないよう変更
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        else
        {
            if (/*LevelData.IsFreeWorldMode &&*/ false)
            {
                tutorialsPlayed[2] = true;
                SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
            }
            else
            {
                tutorialsPlayed[0] = true;
                SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
            }
        }
    }
}
