using UnityEngine;
using UnityEngine.UI; // Use this for UI.Text
using TMPro; // Use this if you're using TextMeshPro

public class HeartbeatSimulator : MonoBehaviour
{
    [Header("UI Component (Text or TMP_Text)")]
    public Text uiText; // Assign if using UnityEngine.UI
    public TMP_Text tmpText; // Assign if using TextMeshPro

    [Header("Heartbeat Settings")]
    public float normalMinBPM = 70f;
    public float normalMaxBPM = 85f;
    public float updateInterval = 1f;

    [Header("Anesthesia Settings")]
    public float anesthesiaDuration = 60f; // Seconds before effects start to wear off
    public float decreaseRate = 0.3f; // How much bpm drops per second after that
    public float minBPM = 30f; // Lower cap for bpm

    private float currentBPM;
    private float timer;
    private float timeSinceStart;
    private bool anesthesiaWearingOff = false;

    void Start()
    {
        currentBPM = Random.Range(normalMinBPM, normalMaxBPM);
        timer = updateInterval;
    }

    void Update()
    {
        timeSinceStart += Time.deltaTime;

        if (timeSinceStart >= anesthesiaDuration)
        {
            anesthesiaWearingOff = true;
        }

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if (anesthesiaWearingOff)
            {
                // Simulate a gradual drop
                currentBPM -= decreaseRate * updateInterval;
                currentBPM = Mathf.Max(currentBPM, minBPM);
            }
            else
            {
                // Normal small variations
                currentBPM += Random.Range(-2f, 2f);
                currentBPM = Mathf.Clamp(currentBPM, normalMinBPM, normalMaxBPM);
            }

            // Display the heartbeat
            string heartbeatText = $"❤️ Heartbeat: {Mathf.RoundToInt(currentBPM)} bpm";

            if (uiText != null)
                uiText.text = heartbeatText;

            if (tmpText != null)
                tmpText.text = heartbeatText;

            timer = updateInterval;
        }
    }
}
