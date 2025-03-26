using System;
using UnityEngine;

namespace SignageHADO
{
    public class WitMotionDevice : MonoBehaviourCustom
    {
        private bool _isActive = false;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;

                if (!_isActive)
                {
                    integratedImu?.Parse(null);
                    batteryData?.Parse(null);
                }
            }
        }

        public float BatteryLevel => batteryData != null ? batteryData.Level : 0f;
        public Quaternion Angle => integratedImu != null ? integratedImu.Angle : Quaternion.identity;
        public Vector3 Accelerometer => integratedImu != null ? integratedImu.Accelerometer : Vector3.zero;
        public Vector3 AngularVelocity => integratedImu != null ? integratedImu.AngularVelocity : Vector3.zero;

        protected struct ByteReader
        {
            private int currentIndex;
            private readonly byte[] bytes;

            public ByteReader(byte[] bytes)
            {
                this.bytes = bytes;
                currentIndex = 0;
            }

            public short readShort()
            {
                short result;
                if (BitConverter.IsLittleEndian)
                {
                    result = (short)((bytes[currentIndex + 1] << 8) | bytes[currentIndex]);
                }
                else
                {
                    result = (short)((bytes[currentIndex] << 8) | bytes[currentIndex + 1]);
                }

                currentIndex += 2;
                return result;
            }

            public byte readByte()
            {
                var result = bytes[currentIndex];
                currentIndex += 1;
                return result;
            }

            public int readInt()
            {
                int result;

                if (BitConverter.IsLittleEndian)
                {
                    result = bytes[currentIndex] |
                             (bytes[currentIndex + 1] << 8) |
                             (bytes[currentIndex + 2] << 16) |
                             (bytes[currentIndex + 3] << 24);
                }
                else
                {
                    result = (bytes[currentIndex] << 24) |
                             (bytes[currentIndex + 1] << 16) |
                             (bytes[currentIndex + 2] << 8) |
                             (bytes[currentIndex + 3]);
                }

                currentIndex += 4;
                return result;
            }
        }

        [Serializable]
        protected class IntegratedImu
        {
            public Vector3 Accelerometer { get; private set; } = Vector3.zero;
            public Vector3 AngularVelocity { get; private set; } = Vector3.zero;
            public Quaternion Angle { get; private set; } = Quaternion.identity;

            private const float AccelerometerScale = 32768.0f;
            private const float AngularVelocityScale = 32768f;
            private const float AngleScale = 32768.0f;

            [HideInInspector] private ByteReader reader = default;
            [HideInInspector] private byte header = 0;
            [HideInInspector] private byte flag = 0;
            [HideInInspector] private Vector3 accelerometer = Vector3.zero;
            [HideInInspector] private Vector3 angularVelocity = Vector3.zero;
            [HideInInspector] private Vector3 angle = Vector3.zero;

            public void Parse(byte[] bytes)
            {
                if (bytes == null || bytes.Length <= 0)
                {
                    Empty();
                    return;
                }

                reader = new ByteReader(bytes);

                header = reader.readByte();
                if (header != 0x55)
                {
                    Empty();
                    return;
                }

                flag = reader.readByte();
                if (flag != 0x61)
                {
                    Empty();
                    return;
                }

                // ‡”Ô’ˆÓI
                accelerometer.x = -(reader.readShort() / AccelerometerScale * 16);
                accelerometer.z = -(reader.readShort() / AccelerometerScale * 16);
                accelerometer.y = -(reader.readShort() / AccelerometerScale * 16);

                // ‡”Ô’ˆÓI
                angularVelocity.x = -(reader.readShort() / AngularVelocityScale * 2000);
                angularVelocity.z = -(reader.readShort() / AngularVelocityScale * 2000);
                angularVelocity.y = -(reader.readShort() / AngularVelocityScale * 2000);

                // ‡”Ô’ˆÓI
                angle.x = -(reader.readShort() / AngleScale * 180);
                angle.z = -(reader.readShort() / AngleScale * 180);
                angle.y = -(reader.readShort() / AngleScale * 180);

                Set(angle, accelerometer, angularVelocity);
            }

            private void Empty() => Set(Vector3.zero, Vector3.zero, Vector3.zero);

            private void Set(Vector3 angle, Vector3 accelerometer, Vector3 angularVelocity)
            {
                Angle = Quaternion.Euler(angle);
                Accelerometer = accelerometer;
                AngularVelocity = angularVelocity;
            }
        }

        [Serializable]
        protected class BatteryData
        {
            public float Level { get; private set; } = 0f;

            [HideInInspector] private ByteReader reader = default;
            [HideInInspector] private byte header = 0;
            [HideInInspector] private byte flag = 0;
            [HideInInspector] private byte startRegisterLowByte = 0;
            [HideInInspector] private byte startRegisterHighByte = 0;
            [HideInInspector] private int val = 0;

            public void Parse(byte[] bytes)
            {
                if (bytes == null || bytes.Length <= 0)
                {
                    Empty();
                    return;
                }

                reader = new ByteReader(bytes);

                header = reader.readByte();
                if (header != 0x55)
                {
                    Empty();
                    return;
                }

                flag = reader.readByte();
                if (flag != 0x71)
                {
                    Empty();
                    return;
                }

                startRegisterLowByte = reader.readByte();
                if (startRegisterLowByte != 0x64)
                {
                    Empty();
                    return;
                }

                startRegisterHighByte = reader.readByte();
                if (startRegisterHighByte != 0x00)
                {
                    Empty();
                    return;
                }

                val = BitConverter.ToInt16(bytes, 4);

                Set(val);
            }

            private void Empty() => Set(0);

            private void Set(float val)
            {
                if (val > 830)
                {
                    Level = 1f;
                }
                else if (val <= 830 && val > 750)
                {
                    Level = 0.75f;
                }
                else if (val <= 750 && val > 715)
                {
                    Level = 0.5f;
                }
                else if (val <= 715 && val > 675)
                {
                    Level = 0.25f;
                }
                else
                {
                    Level = 0f;
                }
            }
        }

        private IntegratedImu integratedImu = new IntegratedImu();
        private BatteryData batteryData = new BatteryData();

        private void Start()
        {
            IsActive = false;
        }

        public void UpdateWitMotion(byte[] buffer)
        {
            try
            {
                integratedImu?.Parse(buffer);
                batteryData?.Parse(buffer);
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "UpdateWitMotion", $"{ex.Message}");
            }

            Log(DebugerLogType.Info, "UpdateWitMotion", $"BatteryLevel={BatteryLevel}, Angle={Angle}, Accelerometer={Accelerometer}, AngularVelocity={AngularVelocity}");
        }
    }
}
