using System;
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

        private void Update()
        {
            RunnerLogic(sourceBar.value);
        }

        private void RunnerLogic(float sValue)
        {
            var velocity = rigidbodyAthlete.velocity;
            var sValueOnCurve = sValue / 100f;
            
            // Shows the speed value in UI.
            speedText.text = velocity.magnitude.ToString("F1") + "km/h";

            if (sValue > 0)
            {
                // Sets Rigidbody speed with input.
                SetRigidbodySpeed(maxSpeedValues.Evaluate(sValueOnCurve), 0.5f, 2f);

                // Sets Animator speed with input.
                if (sValueOnCurve > 0.05f || velocity.z > minRunningSpeed)
                {
                    SetAnimatorSpeed(true, initialSpeedValuesCurve.Evaluate(sValueOnCurve), maxSpeedValues.Evaluate(sValueOnCurve) / maxRunningSpeed, dividerPerValuesCurve.Evaluate(sValueOnCurve));
                }
                else
                {
                    SetAnimatorSpeed(false,1f, 0, 1f);
                }
            }
            else
            {
                // Sleep Rigidbody when no sValue from input.
                rigidbodyAthlete.Sleep();
                
                // Slowdown the Animator speed when no sValue from input.
                SetAnimatorSpeed(false,1f, 0, 1f);
            }
        }
        
        private void SetAnimatorSpeed(bool isRun, float initialSpeed, float addingValue, float dividerPer)
        {
            animatorAthlete.SetBool(Run, isRun);
            animatorAthlete.speed = initialSpeed +  addingValue / dividerPer;
            animatorAthlete.SetFloat(Speed, rigidbodyAthlete.velocity.magnitude);
        }

        private void SetRigidbodySpeed(float maxSpeed, float accelerationConstant, float decelerationConstant)
        {
            rigidbodyAthlete.AddForce(rigidbodyAthlete.velocity.z < maxSpeed ? Vector3.forward * accelerationConstant : Vector3.back * decelerationConstant, ForceMode.Acceleration);
        }
    }
}