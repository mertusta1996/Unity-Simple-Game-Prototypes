using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MertUsta.OlympicsRunner
{
    public class UserInput : MonoBehaviour
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
        

        public void Update()
        {
            CheckBarStatus();
        }

        private void CheckBarStatus()
        {
            if (sourceBar.value <= maxTempoColorLimit && sourceBar.value >= overForceColorLimit)
            {
                fillImage.color = overForceColor;
                tempoText.text = "OverForced Tempo -";
                tempoText.color = Color.red;
            }
            else if (sourceBar.value < overForceColorLimit && sourceBar.value >= successColorLimit)
            {
                fillImage.color = successColor;
                tempoText.text = "Good Tempo +";
                tempoText.color = Color.green;
            }
            else
            {
                fillImage.color = underForceColor;
                tempoText.text = "Slow Tempo -";
                tempoText.color = Color.yellow;
            }
        }
    }
}