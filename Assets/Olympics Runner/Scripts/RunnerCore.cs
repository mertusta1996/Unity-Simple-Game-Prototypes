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
        
        [Header("Speed UI Texts")] 
        public TextMeshProUGUI speedText;

        [Header("Runner Values")] 
        public float minRunningSpeed = 3f;
        public float maxRunningSpeed = 24f;
        
        [Header("Runner Values Curves")] 
        public AnimationCurve initialSpeedValuesCurve;
        public AnimationCurve dividerPerValuesCurve;
        public AnimationCurve maxSpeedValues;

        private static readonly int Run = Animator.StringToHash("run");
        private static readonly int Speed = Animator.StringToHash("speed");
        private Vector3 _cameraDefaultPos;
        
        private void Awake()
        {
            if (Camera.main != null) _cameraDefaultPos = Camera.main.transform.localPosition;
        }
        
        private void Update()
        {
            RunnerLogic(sourceBar.value);
        }

        private void RunnerLogic(float sValue)
        {
            var velocity = rigidbodyAthlete.velocity;
            var sValueOnCurve = sValue / 100f;
            HeadCamera(velocity.z);
            // Shows the speed value in UI.
            speedText.text = velocity.magnitude.ToString("F1") + "km/h";

            if (sValue > 0)
            {
                // Sets Rigidbody speed with input.
                SetRigidbodySpeed(maxSpeedValues.Evaluate(sValueOnCurve), 0.5f, 2f);

                // Sets Animator speed with input.
                if (sValueOnCurve > 0.05f || velocity.z > minRunningSpeed)
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

        private void HeadCamera(float speed)
        {
            var posY = Mathf.Sin(Time.fixedTime * 3) * speed / 200;
            var posX = Mathf.Cos(Time.fixedTime * 3) * speed / 800;

            var pos = new Vector3(posX, posY,0);
            if (Camera.main != null)
                Camera.main.transform.localPosition = _cameraDefaultPos + pos;
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