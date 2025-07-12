using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

/// <summary>
/// TurorialPlayerを元に新クラスを定義しました
/// </summary>
public class CreditPlayer : MonoBehaviour
{
    private void Start()
    {
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
