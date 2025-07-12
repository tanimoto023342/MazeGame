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
    [SerializeField] string videoFileName;

    private void Start()
    {
        PlayVideo();
    }

    public void PlayVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        
        if (videoPlayer)
        {
            videoFileName = LevelData.IsArcadeMode ? "arcade-tutorial.mp4" : 
                (LevelData.IsFreeWorldMode ? "free-world-tutorial.mp4" : "level-select-tutorial.mp4");
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
    }

    public void SkipTutorial()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
