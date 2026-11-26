
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
    [Header("Countdown UI")]
    public Image countdownImage;
    public Sprite[] countdownSprites;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip countdownSound;

    [Header("Settings")]
    public float duration = 1f;

    void Start()
    {
        Debug.Log("COUNTDOWN START! TimeScale: " + Time.timeScale);

        if (countdownImage == null ||
            countdownSprites == null ||
            countdownSprites.Length == 0)
        {
            Debug.LogError("Countdown belum dikonfigurasi!");
            return;
        }

        Time.timeScale = 0f;
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        countdownImage.gameObject.SetActive(true);

        for (int i = 0; i < countdownSprites.Length; i++)
        {
            countdownImage.sprite = countdownSprites[i];

            // Bunyi satu kali setiap angka muncul
            if (audioSource != null && countdownSound != null)
            {
                audioSource.PlayOneShot(countdownSound);
            }

            yield return new WaitForSecondsRealtime(duration);
        }

        countdownImage.gameObject.SetActive(false);
        Time.timeScale = 1f;

        Debug.Log("COUNTDOWN SELESAI! GAME START!");
    }
}
