using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MertUsta.OlympicsRunner
{
    public class RunnerSourceBar : MonoBehaviour
    {
        [Header("Slider UI")]
        public Slider sourceBar;

        [Header("Source Values")]
        public float maxSource = 100;
        public float minSource = 0;
        public float currentSource;
        public float sourceIncreaseRate = 0.025f;

        [Header("Tempo Values")]
        public float tempoStepValue = 45f;
        public float increasingTempoValue = 16f;
        public float decreasingTempoValue = 8f;

        private readonly WaitForSecondsRealtime _regenTick = new WaitForSecondsRealtime(0.1f);
        private Coroutine _regen;
        
        private void Start()
        {
            currentSource = minSource;
            sourceBar.minValue = minSource;
            sourceBar.maxValue = maxSource;
            sourceBar.value = minSource;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                UseSource(currentSource < tempoStepValue ? increasingTempoValue : decreasingTempoValue);
        }

        private void UseSource(float amount)
        {
            // Tempo and Source usage mechanisms.
            if (!(currentSource + amount <= maxSource)) return;
            
            currentSource += amount;
            sourceBar.value = currentSource;

            if (_regen != null)
                StopCoroutine(_regen);

            _regen = StartCoroutine(RegenSource());
        }

        private IEnumerator RegenSource()
        {
            yield return new WaitForSecondsRealtime(0.01f);

            while (currentSource > minSource)
            {
                currentSource -= maxSource * sourceIncreaseRate;
                sourceBar.value = currentSource;
                yield return _regenTick;
            }
        }
    }
}