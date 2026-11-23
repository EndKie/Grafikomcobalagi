using TMPro;
using UnityEngine;

public class SpeedrunTimer : MonoBehaviour
{
    private const string BestTimeKey = "BestSpeedrunTime";

    [SerializeField] private TMP_Text leaderboardText;
    [SerializeField] private string finishCheckpointName = "Checkpoint_3";

    private float elapsedTime;
    private bool finished;
    private bool stopped;

    private void Start()
    {
#if UNITY_EDITOR
        Debug.Assert(FormatTime(65.43f) == "01:05.43");
        Debug.Assert(FormatTime(59.999f) == "01:00.00");
#endif
        UpdateBoard();
    }

    private void Update()
    {
        if (finished || stopped)
            return;

        elapsedTime += Time.deltaTime;
        UpdateBoard();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!finished && !stopped &&
            other.CompareTag("Checkpoint") &&
            other.name == finishCheckpointName)
        {
            Finish();
        }
    }

    private void Finish()
    {
        finished = true;

        if (!PlayerPrefs.HasKey(BestTimeKey) ||
            elapsedTime < PlayerPrefs.GetFloat(BestTimeKey))
        {
            PlayerPrefs.SetFloat(BestTimeKey, elapsedTime);
            PlayerPrefs.Save();
        }

        UpdateBoard();
    }

    public void StopTimer()
    {
        if (finished || stopped)
            return;

        stopped = true;
        UpdateBoard();
    }

    private void UpdateBoard()
    {
        if (leaderboardText == null)
            return;

        string best = PlayerPrefs.HasKey(BestTimeKey)
            ? FormatTime(PlayerPrefs.GetFloat(BestTimeKey))
            : "--:--.--";

        string status = finished
            ? "FINISH: "
            : stopped
                ? "GAME OVER: "
                : "WAKTU: ";

        leaderboardText.text =
            "<b>LEADERBOARD</b>\n" +
            status + FormatTime(elapsedTime) + "\n" +
            "TERBAIK: " + best;
    }

    internal static string FormatTime(float seconds)
    {
        int hundredths = Mathf.RoundToInt(Mathf.Max(0f, seconds) * 100f);
        int minutes = hundredths / 6000;
        int secondsPart = hundredths / 100 % 60;

        return $"{minutes:00}:{secondsPart:00}.{hundredths % 100:00}";
    }
}
