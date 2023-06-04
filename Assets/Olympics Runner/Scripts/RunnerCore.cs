using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MertUsta.OlympicsRunner
{
    public class RunnerCore : MonoBehaviour
    {
        [Header("Components")] 
        public Slider sourceBar;
        public Animator animatorAthlete;
        public Rigidbody rigidbodyAthlete;
        
        [Header("Speed and Distance UI Texts")] 
        public TextMeshProUGUI speedText;
        public TextMeshProUGUI distanceText;

        [Header("Runner Values")] 
        public float minRunningSpeed = 3f;
        public float maxRunningSpeed = 24f;
        
        [Header("Runner Values Curves")] 
        public AnimationCurve initialSpeedValuesCurve;
        public AnimationCurve dividerPerValuesCurve;
        public AnimationCurve maxSpeedValues;

        [Header("Runner Effects")] 
        public ParticleSystem speedEffect;

        private static readonly int Run = Animator.StringToHash("run");
        private static readonly int Speed = Animator.StringToHash("speed");
        private Vector3 _cameraDefaultPos;
        private Vector3 _runnerDefaultPos;
        private bool _isMainCameraNotNull;
        private Camera _mainCamera => Camera.main;

        private void Awake()
        {
            _isMainCameraNotNull = _mainCamera != null;
            if (Camera.main != null) _cameraDefaultPos = Camera.main.transform.localPosition;
            if(rigidbodyAthlete != null) _runnerDefaultPos = rigidbodyAthlete.transform.position;
        }
        
        private void Update()
        {
            RunnerLogic(sourceBar.value);
        }

        private void RunnerLogic(float sValue)
        {
            var velocityMagnitude = rigidbodyAthlete.velocity.magnitude;
            
            // Set the head bobbing values in main camera with velocity magnitude.
            SetHeadBobbingValues(velocityMagnitude);

            // Set the warp-drive speed effect's radius via runner's velocity magnitude.
            var speedEffectShape = speedEffect.shape;
            speedEffectShape.radius = SpeedEffect.CalculateSpeedEffectValue(velocityMagnitude, maxRunningSpeed, 4f, 10f);
            
            // Shows the speed value in UI.
            SetSpeedUI(velocityMagnitude);
            
            // Shows the speed value in UI.
            distanceText.text = "Total Distance : " + DistanceConversion.Distance((rigidbodyAthlete.transform.position - _runnerDefaultPos).magnitude);

            // Calculates and sets animator speed and rigidbody speed.
            RunnerCalculation(sValue, velocityMagnitude);
        }

        private void RunnerCalculation(float sValue, float velocityMagnitude)
        {
            var sValueOnCurve = sValue / 100f;
            if (sValue > 0)
            {
                // Sets Rigidbody speed with input.
                SetRigidbodySpeed(maxSpeedValues.Evaluate(sValueOnCurve), 0.5f, 2f);

                // Sets Animator speed with input.
                if (sValueOnCurve > 0.05f || velocityMagnitude > minRunningSpeed)
                    SetAnimatorSpeed(initialSpeedValuesCurve.Evaluate(sValueOnCurve), maxSpeedValues.Evaluate(sValueOnCurve) / maxRunningSpeed, dividerPerValuesCurve.Evaluate(sValueOnCurve));
                else
                    SetAnimatorSpeed(1f, 0, 1f);
            }
            else
            {
                // Sleep Rigidbody when no sValue from input.
                rigidbodyAthlete.Sleep();
                
                // Slowdown the Animator speed when no sValue from input.
                SetAnimatorSpeed(1f, 0, 1f);
            }
        }
        
        private void SetSpeedUI(float velocityMagnitude)
        {
            speedText.text = "Speed : " + velocityMagnitude.ToString("F1") + "km/h";
        }
        
        private void SetHeadBobbingValues(float velocityMagnitude)
        {
            var pos = new Vector3(
                Mathf.Sin(Time.fixedTime * 3) * velocityMagnitude / 750,
                Mathf.Sin(Time.fixedTime * 4) * velocityMagnitude / 300,
                Mathf.Cos(Time.fixedTime * 3) * velocityMagnitude / 360);
            
            if (_isMainCameraNotNull)
                _mainCamera.transform.localPosition = _cameraDefaultPos + pos;
        }

        private void SetAnimatorSpeed(float initialSpeed, float addingValue, float dividerPer)
        {
            animatorAthlete.speed = initialSpeed +  addingValue / dividerPer;
            animatorAthlete.SetFloat(Speed, rigidbodyAthlete.velocity.magnitude);
        }

        private void SetRigidbodySpeed(float maxSpeed, float accelerationConstant, float decelerationConstant)
        {
            rigidbodyAthlete.AddForce(rigidbodyAthlete.velocity.z < maxSpeed ? Vector3.forward * accelerationConstant : Vector3.back * decelerationConstant, ForceMode.Acceleration);
        }
    }
}