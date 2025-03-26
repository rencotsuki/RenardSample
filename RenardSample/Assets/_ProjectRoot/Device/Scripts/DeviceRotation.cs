using System;
using UnityEngine;

namespace SignageHADO
{
    [Serializable]
    public class DeviceRotation : MonoBehaviourCustom
    {
        public bool GyroActive { get; private set; } = false;

        /// <summary>左右軸まわりの回転(縦)</summary>
        public float Pitch { get; private set; } = 0f;

        /// <summary>上下軸まわりの回転(横)</summary>
        public float Yaw { get; private set; } = 0f;

        /// <summary>前後軸まわりの回転(画面を回す)</summary>
        public float Roll { get; private set; } = 0f;

        public Vector3 EulerAngles { get; private set; } = Vector3.zero;

        protected bool isMobile
        {
            get
            {
#if !UNITY_EDITOR
                if (Application.platform == RuntimePlatform.IPhonePlayer ||
                    Application.platform == RuntimePlatform.Android)
                {
                    return true;
                }
#endif
                return false;
            }
        }

        private Quaternion _gyroAsUnity = Quaternion.identity;
        private Quaternion _calculatedRotation = Quaternion.identity;

        private void Awake()
        {
            SetGyroActive(false);
        }

        private void Update()
        {
            if (isMobile)
            {
                if (Input.gyro.enabled)
                {
                    EulerAngles = GetCameraGyroEuler(Input.gyro.attitude);
                }
                else
                {
                    EulerAngles = Vector3.zero;
                }
            }

            Pitch = EulerAngles.x;
            Yaw = EulerAngles.y;
            Roll = EulerAngles.z;
        }

        public void SetGyroActive(bool value)
        {
            GyroActive = isMobile ? value : false;

            if (isMobile)
                Input.gyro.enabled = GyroActive;
        }

        public void SetEulerAngles(Vector3 value)
        {
            EulerAngles = value;
        }

        private Vector3 GetCameraGyroEuler(Quaternion gyro)
        {
            _gyroAsUnity = new Quaternion(gyro.x, gyro.y, -gyro.z, -gyro.w);
            _calculatedRotation = Quaternion.Euler(90f, 0f, 0f) * _gyroAsUnity;
            return _calculatedRotation.eulerAngles;
        }
    }
}
