using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealthAndRespawn : MonoBehaviour
{
    [Header("Pengaturan Nyawa & Air")]
    public int maxLives = 3;
    public Transform waterTransform;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gameOverSound;
    public AudioClip damageSound;

    [Header("UI (Opsional - Bisa Diisi Nanti)")]
    public TextMeshProUGUI livesText;
    public GameObject[] heartIcons;
    public GameObject gameOverPanel;

    private int currentLives;
    private Vector3 respawnPosition;
    private CharacterController characterController;
    private bool isGameOver = false;

    void Start()
    {
        currentLives = maxLives;
        respawnPosition = transform.position; // Titik awal game menjadi checkpoint pertama
        characterController = GetComponent<CharacterController>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateLivesUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return;

        // Menyentuh Checkpoint baru
        if (other.CompareTag("Checkpoint"))
        {
            respawnPosition = other.transform.position + Vector3.up * 1.5f;
            Debug.Log("Checkpoint tersimpan!");
        }
        // Menyentuh Air
        else if (other.CompareTag("Water"))
        {
            HandleWaterHit();
        }
    }

    
void HandleWaterHit()
{
    // Cek apakah checkpoint terakhir sudah terendam
    bool isCheckpointSubmerged =
        waterTransform != null &&
        respawnPosition.y <= (waterTransform.position.y + 0.5f);

    // Jika checkpoint sudah terendam, langsung Game Over
    // tanpa mengurangi nyawa.
    if (isCheckpointSubmerged)
    {
        TriggerGameOver();
        return;
    }

    // Checkpoint masih aman, kurangi satu nyawa
    currentLives--;

    if (audioSource != null && damageSound != null)
    {
        audioSource.PlayOneShot(damageSound);
    }

    UpdateLivesUI();

    // Jika nyawa habis, Game Over
    if (currentLives <= 0)
    {
        TriggerGameOver();
    }
    else
    {
        RespawnPlayer();
    }
}

    void RespawnPlayer()
    {
        // Matikan CharacterController sesaat agar posisi bisa dipindahkan secara paksa
        if (characterController != null) characterController.enabled = false;

        transform.position = respawnPosition;

        if (characterController != null) characterController.enabled = true;
        Debug.Log("Kena air! Respawn di checkpoint. Sisa nyawa: " + currentLives);
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER!");

        SpeedrunTimer timer = GetComponent<SpeedrunTimer>();

        if (timer != null)
        {
            timer.StopTimer();
        }

        RuntimeController runtimeTimer = FindFirstObjectByType<RuntimeController>();

        if (runtimeTimer != null)
        {
            runtimeTimer.StopTimer();
        }

        // Hentikan kenaikan air
        if (waterTransform != null)
        {
            RisingWater waterScript = waterTransform.GetComponent<RisingWater>();
            if (waterScript != null) waterScript.isRising = false;
        }

        // Munculkan kursor & panel Game Over jika sudah dipasang
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Putar suara Game Over
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // Munculkan panel Game Over
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Nyawa: " + currentLives;

        if (heartIcons != null)
        {
            for (int i = 0; i < heartIcons.Length; i++)
            {
                heartIcons[i].SetActive(i < currentLives);
            }
        }
    }

    // Fungsi untuk tombol Ulangi (Restart) di UI Game Over
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}