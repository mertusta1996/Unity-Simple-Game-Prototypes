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
        
        [Header("Runner Values Curves")] 
        public AnimationCurve initialSpeedValuesCurve;
        public AnimationCurve dividerPerValuesCurve;
        public AnimationCurve maxSpeedValues;

        private static readonly int Run = Animator.StringToHash("run");
        private static readonly int Speed = Animator.StringToHash("speed");

        private void Update()
        {
            RunnerLogic(sourceBar.value);
        }

        private void RunnerLogic(float sValue)
        {
            var velocity = rigidbodyAthlete.velocity;
            speedText.text = velocity.magnitude.ToString("F1") + "km/h";

            var sValueOnCurve = sValue / 100f;
            SetAnimatorBoolStatus(sValueOnCurve > 0.05f || velocity.z > minRunningSpeed);
            SetRigidbodySpeed(maxSpeedValues.Evaluate(sValueOnCurve));

            if (sValueOnCurve > 0.05f || rigidbodyAthlete.velocity.z > minRunningSpeed)
            {
                SetAnimatorSpeed(initialSpeedValuesCurve.Evaluate(sValueOnCurve), sValue, dividerPerValuesCurve.Evaluate(sValueOnCurve));
            }
            else
            {
                SetAnimatorSpeed(1f, 0, 1);
            }
        }
        
        private void SetAnimatorBoolStatus(bool isRun)
        {
            animatorAthlete.SetBool(Run, isRun);
        }
        
        private void SetAnimatorSpeed(float initialSpeed, float sValue, float dividerPer)
        {
            animatorAthlete.speed = initialSpeed +  sValue / dividerPer;
            animatorAthlete.SetFloat(Speed, rigidbodyAthlete.velocity.magnitude);
        }

        private void SetRigidbodySpeed(float maxSpeed)
        {
            rigidbodyAthlete.AddForce(rigidbodyAthlete.velocity.z < maxSpeed ? Vector3.forward * 0.5f : Vector3.back * 2f, ForceMode.Acceleration);
        }
    }
}