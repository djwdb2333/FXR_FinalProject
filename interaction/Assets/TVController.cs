using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class TVController : MonoBehaviour
{
    [Header("Video Setup")]
    public VideoPlayer videoPlayer;

    [Header("UI Elements")]
    public TextMeshPro timerText; // assign your TMP text (UI or 3D)

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Pause(); // start paused
        }

        if (timerText != null)
            timerText.gameObject.SetActive(false); // hide timer initially
    }

    void Update()
    {
        if (videoPlayer == null || timerText == null)
            return;

        if (videoPlayer.isPlaying)
        {
            // show timer and update
            if (!timerText.gameObject.activeSelf)
                timerText.gameObject.SetActive(true);

            UpdateTimer();
        }
        else
        {
            // hide timer if paused
            if (timerText.gameObject.activeSelf)
                timerText.gameObject.SetActive(false);
        }
    }

    public void PlayVideo()
    {
        if (videoPlayer == null) return;

        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
        else
            videoPlayer.Play();
    }

    private void UpdateTimer()
    {
        if (videoPlayer.clip == null) return;

        double current = videoPlayer.time;
        double total = videoPlayer.clip.length;

        timerText.text = $"{FormatTime(current)} / {FormatTime(total)}";
    }

    private string FormatTime(double time)
    {
        int minutes = Mathf.FloorToInt((float)(time / 60));
        int seconds = Mathf.FloorToInt((float)(time % 60));
        return $"{minutes:00}:{seconds:00}";
    }
}