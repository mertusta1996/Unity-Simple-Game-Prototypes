using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MertUsta.DroneMechanics
{
    public class DroneCore : MonoBehaviour
    {
        [Header("Drone")] 
        public Rigidbody droneRigidbody;
        public Camera droneCamera;

        [Header("Propellers")] 
        public List<Transform> propellerList = new List<Transform>();
        
        [Header("UI")] 
        public TextMeshProUGUI speedText;
        public TextMeshProUGUI altitudeText;
        
        [Header("Drone Effects")] 
        public ParticleSystem speedEffect;

        // pitch, yaw, roll
        private Vector3 _attitude;
        private Vector3 _orientation;
        private float _thrustForce;

        private void Update()
        {
            ApplyUserInputs();
            SetCameraPositionAndRotation();
        }
        
        private void FixedUpdate()
        {
            DroneLogic();
        }

        private void ApplyUserInputs()
        {
            // pitch
            _attitude.x = CalculateValueByUserInput(_attitude.x, 36.75f,1.25f, 0.2f,KeyCode.W,KeyCode.S);
            
            // yaw
            _attitude.y = CalculateValueByUserInput(_attitude.y, 2f,0.8f, 0.2f,KeyCode.RightArrow,KeyCode.LeftArrow);
            
            // roll
            _attitude.z = CalculateValueByUserInput(_attitude.z, 36.75f,1.25f, 0.2f,KeyCode.D,KeyCode.A);
            
            // thrustForce
            _thrustForce = CalculateValueByUserInput(_thrustForce, 5f,1f, 0.2f,KeyCode.UpArrow,KeyCode.DownArrow);

            SetSpeedUI();
            SetAltitudeUI();
            SetSpeedEffect(droneRigidbody.velocity.magnitude);
        }
        
        private void SetSpeedUI()
        {
            speedText.text = "Speed : " + droneRigidbody.velocity.magnitude.ToString("F1") + "km/h";
        }
        
        private void SetAltitudeUI()
        {
            var altitudeValueByM = droneRigidbody.transform.position.y - 0.2f;
            if (altitudeValueByM < 1000f)
            {
                altitudeText.text = "Altitude : " + altitudeValueByM.ToString("F1") + "m";
            }
            else
            {
                var altitudeValueByKm = altitudeValueByM / 1000f;
                altitudeText.text = "Altitude : " + altitudeValueByKm.ToString("F3") + "km";
            }
        }
        
        //TODO: Create "SpeedEffect.cs" and use same methods for Runner and Drone.
        private void SetSpeedEffect(float velocityMagnitude)
        {
            var sh = speedEffect.shape;
            sh.radius = 10f - ((velocityMagnitude / 15f) * 6f);
        }

        private float CalculateValueByUserInput(float actualValue, float limitValue, float diffPerFrameValue, float dragMultiplier, KeyCode upperKeyCode, KeyCode lowerKeyCode)
        {
            // drag force (air friction)
            actualValue = Mathf.Lerp(actualValue, 0, Time.deltaTime / dragMultiplier);

            // applying user's input
            if (Input.GetKey(upperKeyCode) && actualValue < limitValue) actualValue += diffPerFrameValue;
            if (Input.GetKey(lowerKeyCode) && actualValue > -limitValue) actualValue -= diffPerFrameValue;
        
            return actualValue;
        }
        
        
        private void SetCameraPositionAndRotation()
        {
            var movingCameraVector = transform.position - transform.forward * 1.5f + Vector3.up * 0.36f;
            droneCamera.transform.position = droneCamera.transform.position * 0.75f + movingCameraVector * 0.25f;
            droneCamera.transform.LookAt(transform.position + transform.forward * 30.0f);
        }

        private void DroneLogic()
        {
            var propellerTurnSpeed = Mathf.Max(_attitude.magnitude, Mathf.Abs(_thrustForce));
            foreach (var propeller in propellerList)
            {
                // propeller rotate around yaw axis
                propeller.Rotate(Vector3.up, propellerTurnSpeed * 22.5f);
                
                // apply thrust force to rigidbody
                droneRigidbody.AddForce(transform.up * (droneRigidbody.mass * 9.81f + _thrustForce) / 5f, ForceMode.Force);
            }
            
            // yaw
            _orientation.y += _attitude.y;

            // pitch
            _orientation.x = Mathf.Lerp(_orientation.x, _attitude.x, Time.deltaTime * 2f);
            
            // roll
            _orientation.z = Mathf.Lerp(_orientation.z, -_attitude.z, Time.deltaTime * 2f);

            // apply orientation to rigidbody
            droneRigidbody.MoveRotation(Quaternion.Euler(_orientation.x, _orientation.y, _orientation.z));
        }
    }
}