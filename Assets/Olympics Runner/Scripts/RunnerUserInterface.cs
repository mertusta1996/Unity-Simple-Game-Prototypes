using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MertUsta.OlympicsRunner
{
    public class RunnerUserInterface : MonoBehaviour
    {
        [Header("UI")] public Slider sourceBar;
        public Image fillImage;

        [Header("Slider Colors for Runner's tempo")]
        public Color successColor;

        public Color overForceColor;
        public Color underForceColor;

        [Header("Color Limits for Runner")] 
        public float maxTempoColorLimit = 100f;
        public float overForceColorLimit = 85f;
        public float successColorLimit = 60f;

        [Header("Tempo UI Texts")] 
        public TextMeshProUGUI tempoText;
        
        private void Update()
        {
            CheckBarStatus();
        }

        private void CheckBarStatus()
        {
            // UI changes when user input changes.
            if (sourceBar.value <= maxTempoColorLimit && sourceBar.value >= overForceColorLimit)
                SetUserInterface(overForceColor, "OverForced Tempo -", Color.red);
            else if (sourceBar.value < overForceColorLimit && sourceBar.value >= successColorLimit)
                SetUserInterface(successColor, "Good Tempo +", Color.green);
            else
                SetUserInterface(underForceColor, "Slow Tempo -", Color.yellow);
        }

        private void SetUserInterface(Color color, string tempo, Color tempoColor)
        {
            fillImage.color = color;
            tempoText.text = tempo;
            tempoText.color = tempoColor;
        }
    }
}