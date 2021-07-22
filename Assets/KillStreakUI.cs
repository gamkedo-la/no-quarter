using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillStreakUI : MonoBehaviour
{
    public TMP_Text text;
    public Image background;

    private void OnEnable()
    {
        PlayerStatsManager.OnKill += UpdateStreakText;
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnKill -= UpdateStreakText;
    }

    private void Start()
    {
        background.enabled = false;
        text.enabled = false;
    }

    private void UpdateStreakText(int streak)
    {
        if (streak > 10) // streak doesn't award extra points until after 10
        {
            var digits = streak.ToString().Length;
            var marks = new string('!', digits);
            var fullText = $"{streak} kills{marks}";
            text.text = fullText;
            background.enabled = true;
            text.enabled = true;
        }
        else
        {
            background.enabled = false;
            text.enabled = false;
        }
    }
}
