using System;
using UnityEngine;

namespace SignageHADO.Game
{
    [Serializable]
    public class PlayerHandler : MonoBehaviourCustom
    {
        [SerializeField] private ReticleView reticleViewLeft = null;
        [SerializeField] private ReticleView reticleViewRight = null;
        [Header("PlayerObject")]
        [SerializeField] private GameObject playerObject = null;
        [SerializeField] private PlayerReticle playerReticleLeft = null;
        [SerializeField] private PlayerReticle playerReticleRight = null;
        [SerializeField] private Transform playerHead = null;
        [SerializeField] private Vector3 slotOffset = new Vector3(0.3f, -0.25f, 0.125f);
        [SerializeField] private Transform slotLeft = null;
        [SerializeField] private Transform slotRight = null;
        [Header("Debug")]
        [SerializeField] private ReticleView reticleViewCenter = null;
        [SerializeField] private PlayerReticle playerReticleCenter = null;
        [SerializeField] private float manualControlPower = 20f;

        protected bool isHeadTracking => TrackingHandler.IsHeadTracking;

        protected bool isLeftHandTracking => TrackingHandler.IsLeftHandTracking;
        protected HandPoseEnum leftHandPose => TrackingHandler.LeftHandPose;
        protected Vector2 leftHandViewPos => TrackingHandler.LeftHandViewPos;

        protected bool isRightHandTracking => TrackingHandler.IsRightHandTracking;
        protected HandPoseEnum rightHandPose => TrackingHandler.RightHandPose;
        protected Vector2 rightHandViewPos => TrackingHandler.RightHandViewPos;

        protected bool activeControl { get; private set; } = false;
        protected bool activeReticle { get; private set; } = false;

        private Vector3 slotCenterPos => playerHead != null ? playerHead.position : Vector3.zero;
        private Vector3 slotLeftPos => slotLeft != null ? slotLeft.position : Vector3.zero;
        private Vector3 slotRightPos => slotRight != null ? slotRight.position : Vector3.zero;

        protected float rayRadius { get; private set; } = 0.52f;
        protected float rayMaxLength { get; private set; } = 20f;
        protected float attackInterval { get; private set; } = 0.3f;
        protected int attackPower { get; private set; } = 1000;

        public bool KeepShotAction { get; private set; } = false;

        public bool ActiveManualAction { get; private set; } = false;

        private bool manualAction = false;
        private float manualVertical = 0f;
        private float manualHorizontal = 0f;

        [Serializable]
        protected struct Reticle
        {
            public bool Active;
            public Vector2 MarkingViewPos;
            public Vector3 WorldPos;
            public Vector2 ReticleViewPos;
            public bool LockOn;

            public void Clear()
            {
                Active = false;
                MarkingViewPos = TrackingHandler.DefaultViewPos;
                WorldPos = Vector3.zero;
                ReticleViewPos = Vector2.zero;
                LockOn = false;
            }
        }

        [HideInInspector] private Reticle _reticleLeft = new Reticle();
        [HideInInspector] private Reticle _reticleRight = new Reticle();
        [HideInInspector] private Reticle _reticleCenter = new Reticle();
        [HideInInspector] private Vector3 _lockValue = Vector3.zero;
        [HideInInspector] private Vector3 _localEulerAngles = Vector3.zero;

        [HideInInspector] private HandPoseEnum _beforeLeftHandPose = HandPoseEnum.Unknown;
        [HideInInspector] private HandPoseEnum _beforeRightHandPose = HandPoseEnum.Unknown;
        [HideInInspector] private float _attackIntervalManual = 0f;
        [HideInInspector] private float _attackIntervalLeft = 0f;
        [HideInInspector] private float _attackIntervalRight = 0f;

        private OutputLogFile _outputLogFile = null;

        private void Start()
        {
            if (slotLeft != null)
                slotLeft.transform.localPosition = new Vector3(-slotOffset.x, slotOffset.y, slotOffset.z);

            if (slotRight != null)
                slotRight.transform.localPosition = slotOffset;

            KeepShotAction = ConfigSignageHADO.KeepShotAction;

            ClearManualAction();

#if UNITY_EDITOR
            _outputLogFile = new OutputLogFile($"{Application.dataPath}/../../Logs");
#endif
        }

        public void Setup(float rayRadius, float rayMaxLength, float attackInterval, int attackPower)
        {
            this.rayRadius = rayRadius;
            this.rayMaxLength = rayMaxLength;
            this.attackInterval = attackInterval;
            this.attackPower = attackPower;
        }

        public void ResetPlayer()
        {
            _reticleLeft.Clear();
            _reticleRight.Clear();
            _reticleCenter.Clear();

            playerReticleLeft?.Set(rayRadius, rayMaxLength);
            playerReticleRight?.Set(rayRadius, rayMaxLength);
            playerReticleCenter?.Set(rayRadius, rayMaxLength);

            playerReticleLeft?.ClearReticle();
            playerReticleRight?.ClearReticle();
            playerReticleCenter?.ClearReticle();

            _beforeLeftHandPose = HandPoseEnum.Unknown;
            _beforeRightHandPose = HandPoseEnum.Unknown;

            _attackIntervalManual = 0f;
            _attackIntervalLeft = 0f;
            _attackIntervalRight = 0f;
        }

        public void UpdatePlayer(GameStatus gameStatus)
        {
            try
            {
                activeControl = (gameStatus == GameStatus.Entry || gameStatus == GameStatus.Playing);
                activeReticle = (gameStatus == GameStatus.Entry || gameStatus == GameStatus.Start || gameStatus == GameStatus.Playing);

                if (gameStatus != GameStatus.None)
                {
                    // 移動・レティクル操作
                    {
                        _reticleLeft.Active = false;
                        _reticleRight.Active = false;
                        _reticleCenter.Active = false;

                        if (ActiveManualAction)
                        {
                            ManualViewAction();
                        }
                        else
                        {
                            TrackingViewAction();
                        }

                        UpdateViewMove();

                        _reticleLeft.ReticleViewPos = reticleViewLeft.UpdateReticle(_reticleLeft.MarkingViewPos, leftHandPose, _reticleLeft.LockOn, (activeReticle && _reticleLeft.Active));
                        _reticleRight.ReticleViewPos = reticleViewRight.UpdateReticle(_reticleRight.MarkingViewPos, rightHandPose, _reticleRight.LockOn, (activeReticle && _reticleRight.Active));
                        _reticleCenter.ReticleViewPos = reticleViewCenter.UpdateReticle(_reticleCenter.MarkingViewPos, leftHandPose, _reticleCenter.LockOn, (activeReticle && _reticleCenter.Active));

                        _reticleLeft.LockOn = playerReticleLeft.GetReticleTargetPosition(slotLeft, _reticleLeft.ReticleViewPos, out _reticleLeft.WorldPos);
                        _reticleRight.LockOn = playerReticleRight.GetReticleTargetPosition(slotRight, _reticleRight.ReticleViewPos, out _reticleRight.WorldPos);
                        _reticleCenter.LockOn = playerReticleCenter.GetReticleTargetPosition(playerHead, _reticleCenter.ReticleViewPos, out _reticleCenter.WorldPos);
                    }

                    // 攻撃アクション
                    {
                        if (!ActiveManualAction)
                        {
                            TrackingShotAction();
                        }

                        UpdateAttackInterval(Time.deltaTime);
                    }
                }

#if UNITY_EDITOR
                // デバッグ
                {
                    if (Input.GetKey(KeyCode.R))
                        _outputLogFile?.Rec();

                    if (Input.GetKey(KeyCode.S))
                        _outputLogFile?.Stop();

                    _outputLogFile?.UpdateLog($"{_reticleRight.MarkingViewPos.x:0.000},{_reticleRight.MarkingViewPos.y:0.000},{_reticleRight.ReticleViewPos.x:0.000},{_reticleRight.ReticleViewPos.y:0.000}");
                }
#endif
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "UpdateAsync", $"[{gameStatus}]: {ex.Message}");
            }
        }

        private void UpdateViewMove()
        {
            ClearManualAction();

            if (playerObject == null || playerHead == null)
                return;

            playerHead.localRotation = Quaternion.Slerp(playerHead.localRotation, Quaternion.Euler(_localEulerAngles), 0.85f);
        }

        private void UpdateAttackInterval(float deltaTime)
        {
            _attackIntervalManual = (_attackIntervalManual - deltaTime <= 0f) ? 0f : _attackIntervalManual - deltaTime;

            _attackIntervalLeft = (_attackIntervalLeft - deltaTime <= 0f) ? 0f : _attackIntervalLeft - deltaTime;

            _attackIntervalRight = (_attackIntervalRight - deltaTime <= 0f) ? 0f : _attackIntervalRight - deltaTime;
        }

        private void TrackingViewAction()
        {
            if (playerObject == null)
                return;

            if (isLeftHandTracking)
            {
                _reticleLeft.MarkingViewPos.x = leftHandViewPos.x + 0.5f;
                _reticleLeft.MarkingViewPos.y = leftHandViewPos.y + 0.5f;

                // MarkingViewPosがレンジ外は処理させない
                if ((_reticleLeft.MarkingViewPos.x >= -0.15f && _reticleLeft.MarkingViewPos.x <= 1.15f) &&
                    (_reticleLeft.MarkingViewPos.y >= -0.15f && _reticleLeft.MarkingViewPos.y <= 1.15f))
                {
                    _reticleLeft.Active = true;
                }
            }

            if (isRightHandTracking)
            {
                _reticleRight.MarkingViewPos.x = rightHandViewPos.x + 0.5f;
                _reticleRight.MarkingViewPos.y = rightHandViewPos.y + 0.5f;

                // MarkingViewPosがレンジ外は処理させない
                if ((_reticleRight.MarkingViewPos.x >= -0.15f && _reticleRight.MarkingViewPos.x <= 1.15f) &&
                    (_reticleRight.MarkingViewPos.y >= -0.15f && _reticleRight.MarkingViewPos.y <= 1.15f))
                {
                    _reticleRight.Active = true;
                }
            }
        }

        private void ManualViewAction()
        {
            if (playerObject == null || playerHead == null)
                return;

            if (manualAction)
            {
                _lockValue = (-playerObject.transform.right * manualVertical + playerObject.transform.up * manualHorizontal) * manualControlPower * Time.deltaTime;
                _localEulerAngles = playerHead.localEulerAngles + _lockValue;
            }

            _reticleCenter.Active = true;
        }

        private void ClearManualAction()
        {
            manualAction = false;
            manualVertical = 0f;
            manualHorizontal = 0f;
        }

        private bool CheckActiveShot(HandPoseEnum beforePose, HandPoseEnum nowPose)
        {
            if (KeepShotAction)
            {
                // 連射許可
                if (nowPose == HandPoseEnum.Paper)
                    return true;
            }
            else
            {
                // パーにしたら発射
                if (beforePose != nowPose && nowPose == HandPoseEnum.Paper)
                    return true;
            }
            return false;
        }

        private void TrackingShotAction()
        {
            if (isLeftHandTracking)
            {
                if (_attackIntervalLeft <= 0f && _reticleLeft.Active)
                {
                    if (CheckActiveShot(_beforeLeftHandPose, leftHandPose))
                    {
                        OnShotBullet(attackPower, slotLeftPos, _reticleLeft.WorldPos, false);
                        _attackIntervalLeft = attackInterval;
                    }
                }
            }

            if (isRightHandTracking)
            {
                if (_attackIntervalRight <= 0f && _reticleRight.Active)
                {
                    if (CheckActiveShot(_beforeRightHandPose, rightHandPose))
                    {
                        OnShotBullet(attackPower, slotRightPos, _reticleRight.WorldPos, true);
                        _attackIntervalRight = attackInterval;
                    }
                }
            }

            _beforeLeftHandPose = leftHandPose;
            _beforeRightHandPose = rightHandPose;
        }

        private void OnShotBullet(int attack, Vector3 slotPos, Vector3 reticlePos, bool right)
        {
            if (!activeControl)
                return;

            GameHandler.ShotBullet(attack, slotPos, (reticlePos - slotPos).normalized, right);
        }

        public void SetKeepShotAction(bool value)
        {
            if (KeepShotAction != value)
            {
                KeepShotAction = value;

                ConfigSignageHADO.KeepShotAction = KeepShotAction;
                ConfigSignageHADO.Save();
            }
        }

        #region ManualControl

        public void SetManualControl(float vertical, float horizontal)
        {
            if (manualVertical != vertical || manualHorizontal != horizontal)
                manualAction = true;

            manualVertical = vertical;
            manualHorizontal = horizontal;
        }

        public void ActiveManualControl(bool active)
        {
            if (ActiveManualAction == active)
                return;

            ActiveManualAction = active;

            if (ActiveManualAction)
                _localEulerAngles = Vector3.zero;
        }

        public void ShotBullet()
        {
            if (!ActiveManualAction || _attackIntervalManual > 0f)
                return;

            OnShotBullet(attackPower, slotCenterPos, _reticleCenter.WorldPos, false);

            _attackIntervalManual = attackInterval;
        }

        #endregion
    }
}
