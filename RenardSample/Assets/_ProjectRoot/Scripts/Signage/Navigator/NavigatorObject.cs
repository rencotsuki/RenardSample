using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniVRM10;
using UniHumanoid;

namespace SignageHADO
{
    [Serializable]
    public struct FacialExpression
    {
        public ExpressionEnum Expression;
        public float Weight;

        public void Clear()
        {
            Expression = ExpressionEnum.Neutral;
            Weight = 1f;
        }

        public void Copy(FacialExpression facial)
        {
            Expression = facial.Expression;
            Weight = facial.Weight;
        }
    }

    [SerializeField]
    public class NavigatorObject : MonoBehaviourCustom
    {
        [SerializeField] private Vrm10Instance _vrmInstance = null;
        [SerializeField] private AnimationClip _poseResetClip = null;

        public bool Init { get; private set; } = false;

        protected Vrm10Instance vrmInstance
        {
            get => _vrmInstance;
            private set => _vrmInstance = value;
        }

        public Transform ModelRoot => vrmInstance != null ? vrmInstance.transform : transform;

        protected AnimationClip poseResetClip
        {
            get => _poseResetClip;
            private set => _poseResetClip = value;
        }

        public float LookAtHeight { get; protected set; } = 1.3f;

        protected Humanoid humanoid = null;
        protected Animator anim = null;
        protected Vrm10Runtime vrm10Runtime = null;
        protected Vrm10RuntimeControlRig controlRig = null;
        protected Vrm10RuntimeExpression expression = null;

        protected Transform haed => humanoid != null ? humanoid.Head : null;

        protected bool visibleModel = false;
        public bool Visible => Init && visibleModel;

        public TimelineMotionHandler TimelineMotion { get; protected set; } = null;

        protected float referenceScale = 1f;

        protected virtual float bodyBoneSlerp => 0.95f;

        [HideInInspector] protected float rootHeight = 0f;
        [HideInInspector] private Vector3 _rootLocalPos = Vector3.zero;

        ///<summary>指</summary>
        protected enum FingerBones
        {
            ///<summary>親指</summary>
            Thumb,
            ///<summary>人差し指</summary>
            Index,
            ///<summary>中指</summary>
            Middle,
            ///<summary>薬指</summary>
            Ring,
            ///<summary>小指</summary>
            Little
        }

        ///<summary>指関節</summary>
        protected enum KnuckleBones
        {
            ///<summary>第1関節</summary>
            Distal,
            ///<summary>第2関節</summary>
            Intermediate,
            ///<summary>第3関節</summary>
            Proximal
        }

        protected virtual float fingerBoneSlerp => 0.35f;

        [HideInInspector] private float _fingerRotValue = 0f;
        [HideInInspector] private float _fingerRotValueP = 0f;
        [HideInInspector] private Quaternion _fingerRot = Quaternion.identity;

        [HideInInspector] private float[] _blinkWaitTime = new float[] { 1.0f, 3.0f, 10.0f, 30.0f };
        [HideInInspector] private bool _blinkWait = false;
        [HideInInspector] private float _weightValue = 1 / 0.3f;
        [HideInInspector] private float _weight = 0f;
        private IDisposable _onBlink = null;

        protected LipSyncScript lipSync = null;

        protected ExpressionKey nowExpression { get; private set; } = ExpressionKey.Neutral;
        protected float expressionValue = 0f;
        protected float lipValueAa = 0f;
        protected float lipValueIh = 0f;
        protected float lipValueOu = 0f;
        protected float lipValueEe = 0f;
        protected float lipValueOh = 0f;

        protected virtual float expressionLerp => 0.85f;

        [HideInInspector] protected Vector3 playerScale = Vector3.one;
        [HideInInspector] protected Vector3 actorScale = Vector3.one;
        [HideInInspector] private float _beforeExpression = 0f;

        public static async UniTask<NavigatorObject> CreateVRMRuntimeLoadAsync(CancellationToken token, string filePath, Transform parent, AnimationClip poseResetClip = null, bool isDebugLog = false)
        {
            NavigatorObject result = null;

            try
            {
                if (string.IsNullOrEmpty(filePath))
                    throw new Exception("null or empty filePath.");

                if (!File.Exists(filePath))
                    throw new Exception($"not found filePath. path={filePath}");

                var obj = new GameObject("NavigatorObject");

                obj.transform.SetParent(parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;

                result = obj.AddComponent<NavigatorObject>();
                result.poseResetClip = poseResetClip;

                var instance = await Vrm10.LoadPathAsync(filePath, controlRigGenerationOption: ControlRigGenerationOption.Generate, ct: token);

                if (instance == null)
                    throw new Exception("failed load vrm file.");

                instance.transform.SetParent(obj.transform);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;

                result.vrmInstance = instance;

                if (!result.OnSetup())
                    throw new Exception("failed setup.");

                return result;
            }
            catch (Exception ex)
            {
                if (isDebugLog)
                    Debug.Log($"{typeof(NavigatorObject).Name}::CreateVRMRuntimeLoadAsync - {ex.Message}");
            }

            if (result != null)
                Destroy(result.gameObject);

            return null;
        }

        private void Start()
        {
            OnSetup();
        }

        public void SetVisible(bool value)
        {
            if (visibleModel != value)
                OnSetVisible(value);
        }

        #region Load/Setup

        protected bool OnSetup()
        {
            Init = false;

            try
            {
                if (vrmInstance == null)
                    throw new Exception("not found vrm instance.");

                vrm10Runtime = vrmInstance.Runtime;
                controlRig = vrm10Runtime != null ? vrm10Runtime.ControlRig : null;
                expression = vrm10Runtime != null ? vrm10Runtime.Expression : null;

                if (transform.parent != null)
                    ChangeObjectLayer(gameObject, transform.parent.gameObject.layer);

                OnSetScaleObjects(1f, 1f);
                OnSuccessVrmInstance();
                OnSetVisible(visibleModel);

                Init = true;
            }
            catch (Exception ex)
            {
                Log(DebugerLogType.Info, "OnSetup", $"{ex.Message}");
            }
            return Init;
        }

        protected void ChangeObjectLayer(GameObject obj, int layer)
        {
            if (obj == null) return;

            obj.layer = layer;
            foreach (Transform t in obj.transform)
            {
                ChangeObjectLayer(t.gameObject, layer);
            }
        }

        protected virtual void OnSuccessVrmInstance()
        {
            anim = vrmInstance?.GetComponentInChildren<Animator>();

            if (anim != null)
            {
                anim.applyRootMotion = true;

                TimelineMotion = anim.gameObject.GetComponent<TimelineMotionHandler>();

                if (TimelineMotion == null)
                    TimelineMotion = anim.gameObject.AddComponent<TimelineMotionHandler>();

                TimelineMotion?.Setup(poseResetClip);
            }

            humanoid = vrmInstance?.GetComponentInChildren<Humanoid>();
            rootHeight = GetRootHeight();

            LookAtHeight = rootHeight;// (0.9f * rootHeight - 0.8f * rootHeight);
        }

        protected virtual void OnSetScaleObjects(float scale, float scaleOffset)
        {
            playerScale = (referenceScale > 0 && scaleOffset > 0) ? Vector3.one * (scaleOffset / referenceScale) : Vector3.one;

            // 必ず、先に変更すること！
            if (transform.localScale != playerScale)
                transform.localScale = playerScale;

            actorScale = scale > 0 ? Vector3.one * scale : Vector3.one;

            // スケールは親であるmodelRootに適用させる
            if (transform.localScale != actorScale)
                transform.localScale = actorScale;

            // 必ず読込みモデルはスケール１で展開させる
            if (vrmInstance != null && vrmInstance.transform.localScale != Vector3.one)
                vrmInstance.transform.localScale = Vector3.one;
        }

        protected float GetRootHeight()
        {
            if (humanoid != null && humanoid.Hips != null)
                return humanoid.Hips.localPosition.y;
            return 0f;
        }

        protected virtual void OnSetVisible(bool value)
        {
            visibleModel = value;

            if (vrmInstance != null)
                vrmInstance.gameObject.SetActive(visibleModel);

            OnActiveVrmInstance(visibleModel);
        }

        protected void OnActiveVrmInstance(bool value)
        {
            if (vrmInstance == null)
                return;

            vrmInstance.UpdateType = value ? Vrm10Instance.UpdateTypes.Update : Vrm10Instance.UpdateTypes.None;
            vrmInstance.enabled = value;

            if (value)
                SetBlinkWait();
        }

        public void SetLookAtTarget(Transform target)
        {
            if (vrmInstance != null)
                vrmInstance.LookAtTarget = target;
        }

        public void SetLipSync(LipSyncScript target)
        {
            lipSync = target;
        }

        #endregion

        #region Control

        public void ResetFacialExpression()
        {
            SetExpressionWeight(ToExpressionKey(ExpressionEnum.Happy), 0f);
            SetExpressionWeight(ToExpressionKey(ExpressionEnum.Angry), 0f);
            SetExpressionWeight(ToExpressionKey(ExpressionEnum.Sad), 0f);
            SetExpressionWeight(ToExpressionKey(ExpressionEnum.Relaxed), 0f);
            SetExpressionWeight(ToExpressionKey(ExpressionEnum.Surprised), 0f);
            SetExpressionWeight(ToExpressionKey(ExpressionEnum.Blink), 0f);
            SetExpressionWeight(ToExpressionKey(ExpressionEnum.WinkLeft), 0f);
            SetExpressionWeight(ToExpressionKey(ExpressionEnum.WinkRight), 0f);
            SetExpressionWeight(ToExpressionKey(ExpressionEnum.Neutral), 1f);
        }

        public void UpdateFacialExpression(FacialExpression facial, FacialExpression preFacial)
        {
            if (GetExpressionWeight(ExpressionKey.Neutral) <= 0.1f)
                SetExpressionWeight(ExpressionKey.Blink, 0f);

            SetExpressionWeight(ToExpressionKey(preFacial.Expression), preFacial.Weight);
            SetExpressionWeight(ToExpressionKey(facial.Expression), facial.Weight);
        }

        public void UpdateLipSync()
        {
            if (lipSync != null)
            {
                if (lipSync.transform.position != haed.position ||
                    lipSync.transform.rotation != haed.rotation)
                {
                    lipSync.transform.SetPositionAndRotation(haed.position, haed.rotation);
                }

                lipValueAa = lipSync.LipSuncAa;
                lipValueIh = lipSync.LipSuncIh;
                lipValueOu = lipSync.LipSuncOu;
                lipValueEe = lipSync.LipSuncEe;
                lipValueOh = lipSync.LipSuncOh;
            }
            else
            {
                lipValueAa = 0f;
                lipValueIh = 0f;
                lipValueOu = 0f;
                lipValueEe = 0f;
                lipValueOh = 0f;
            }

            OnUpdateHumanoidLip();
        }

        /// <summary>※通信による連動を想定した反映処理</summary>
        protected virtual void OnUpdateHumanoidBones(IVrmModel model)
        {
            if (model == null)
                return;

            OnSetScaleObjects(model.OriginScale(), model.ScaleOffset());

            if (ModelRoot != null)
                ModelRoot.localPosition = model.AnchorPos();

            OnSetPositionBone(HumanBodyBones.Hips, model.RootPos(), ModelRoot);
            OnSetQuaternionBone(HumanBodyBones.Hips, model.RootRot(), bodyBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.Spine, model.Spine(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.Chest, model.Chest(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.UpperChest, model.UpperChest(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.Neck, model.Neck(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.Head, model.Head(), bodyBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.LeftShoulder, model.Shoulder_Left(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftUpperArm, model.Arm_Left(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftLowerArm, model.Elbow_Left(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftHand, model.Wrist_Left(), bodyBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.RightShoulder, model.Shoulder_Right(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightUpperArm, model.Arm_Right(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightLowerArm, model.Elbow_Right(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightHand, model.Wrist_Right(), bodyBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.LeftUpperLeg, model.Hip_Left(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftLowerLeg, model.Knee_Left(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftFoot, model.Foot_Left(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftToes, model.Toe_Left(), bodyBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.RightUpperLeg, model.Hip_Right(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightLowerLeg, model.Knee_Right(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightFoot, model.Foot_Right(), bodyBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightToes, model.Toe_Right(), bodyBoneSlerp);
        }

        /// <summary>※通信による連動を想定した反映処理</summary>
        protected virtual void OnUpdateHumanoidFingers(IVrmModel model)
        {
            if (model == null)
                return;

            // Left

            OnSetQuaternionBone(HumanBodyBones.LeftThumbProximal, GetFingerLocalRot(FingerBones.Thumb, KnuckleBones.Proximal, model.ThumbFinger_Left()), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftThumbIntermediate, GetFingerLocalRot(FingerBones.Thumb, KnuckleBones.Intermediate, model.ThumbFinger_Left()), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftThumbDistal, GetFingerLocalRot(FingerBones.Thumb, KnuckleBones.Distal, model.ThumbFinger_Left()), fingerBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.LeftIndexProximal, GetFingerLocalRot(FingerBones.Index, KnuckleBones.Proximal, model.IndexFinger_Left()), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftIndexIntermediate, GetFingerLocalRot(FingerBones.Index, KnuckleBones.Intermediate, model.IndexFinger_Left()), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftIndexDistal, GetFingerLocalRot(FingerBones.Index, KnuckleBones.Distal, model.IndexFinger_Left()), fingerBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.LeftMiddleProximal, GetFingerLocalRot(FingerBones.Middle, KnuckleBones.Proximal, model.MiddleFinger_Left()), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftMiddleIntermediate, GetFingerLocalRot(FingerBones.Middle, KnuckleBones.Intermediate, model.MiddleFinger_Left()), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftMiddleDistal, GetFingerLocalRot(FingerBones.Middle, KnuckleBones.Distal, model.MiddleFinger_Left()), fingerBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.LeftRingProximal, GetFingerLocalRot(FingerBones.Ring, KnuckleBones.Proximal, model.RingFinger_Left()), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftRingIntermediate, GetFingerLocalRot(FingerBones.Ring, KnuckleBones.Intermediate, model.RingFinger_Left()), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftRingDistal, GetFingerLocalRot(FingerBones.Ring, KnuckleBones.Distal, model.RingFinger_Left()), fingerBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.LeftLittleProximal, GetFingerLocalRot(FingerBones.Little, KnuckleBones.Proximal, model.LittleFinger_Left()), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftLittleIntermediate, GetFingerLocalRot(FingerBones.Little, KnuckleBones.Intermediate, model.LittleFinger_Left()), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.LeftLittleDistal, GetFingerLocalRot(FingerBones.Little, KnuckleBones.Distal, model.LittleFinger_Left()), fingerBoneSlerp);

            // Right

            OnSetQuaternionBone(HumanBodyBones.RightThumbProximal, GetFingerLocalRot(FingerBones.Thumb, KnuckleBones.Proximal, model.ThumbFinger_Right(), true), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightThumbIntermediate, GetFingerLocalRot(FingerBones.Thumb, KnuckleBones.Intermediate, model.ThumbFinger_Right(), true), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightThumbDistal, GetFingerLocalRot(FingerBones.Thumb, KnuckleBones.Distal, model.ThumbFinger_Right(), true), fingerBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.RightIndexProximal, GetFingerLocalRot(FingerBones.Index, KnuckleBones.Proximal, model.IndexFinger_Right(), true), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightIndexIntermediate, GetFingerLocalRot(FingerBones.Index, KnuckleBones.Intermediate, model.IndexFinger_Right(), true), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightIndexDistal, GetFingerLocalRot(FingerBones.Index, KnuckleBones.Distal, model.IndexFinger_Right(), true), fingerBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.RightMiddleProximal, GetFingerLocalRot(FingerBones.Middle, KnuckleBones.Proximal, model.MiddleFinger_Right(), true), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightMiddleIntermediate, GetFingerLocalRot(FingerBones.Middle, KnuckleBones.Intermediate, model.MiddleFinger_Right(), true), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightMiddleDistal, GetFingerLocalRot(FingerBones.Middle, KnuckleBones.Distal, model.MiddleFinger_Right(), true), fingerBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.RightRingProximal, GetFingerLocalRot(FingerBones.Ring, KnuckleBones.Proximal, model.RingFinger_Right(), true), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightRingIntermediate, GetFingerLocalRot(FingerBones.Ring, KnuckleBones.Intermediate, model.RingFinger_Right(), true), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightRingDistal, GetFingerLocalRot(FingerBones.Ring, KnuckleBones.Distal, model.RingFinger_Right(), true), fingerBoneSlerp);

            OnSetQuaternionBone(HumanBodyBones.RightLittleProximal, GetFingerLocalRot(FingerBones.Little, KnuckleBones.Proximal, model.LittleFinger_Right(), true), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightLittleIntermediate, GetFingerLocalRot(FingerBones.Little, KnuckleBones.Intermediate, model.LittleFinger_Right(), true), fingerBoneSlerp);
            OnSetQuaternionBone(HumanBodyBones.RightLittleDistal, GetFingerLocalRot(FingerBones.Little, KnuckleBones.Distal, model.LittleFinger_Right(), true), fingerBoneSlerp);
        }

        /// <summary>※通信による連動を想定した反映処理</summary>
        protected virtual void OnUpdateHumanoidFace(IVrmModel model)
        {
            if (model == null)
                return;

            OnSetExpression((ExpressionEnum)model.Expression());
            expressionValue = model.ExpressionValue();

            OnUpdateHumanoidExpression();

            lipValueAa = model.Lip_Aa();
            lipValueIh = model.Lip_Ih();
            lipValueOu = model.Lip_Ou();
            lipValueEe = model.Lip_Ee();
            lipValueOh = model.Lip_Oh();

            OnUpdateHumanoidLip();
        }

        protected void OnUpdateHumanoidExpression()
        {
            if (GetExpressionWeight(ExpressionKey.Neutral) <= 0.6f)
                SetExpressionWeight(ExpressionKey.Blink, 0f);

            SetExpressionWeight(nowExpression, expressionValue);
        }

        protected void OnUpdateHumanoidLip()
        {
            if (GetExpressionWeight(ExpressionKey.Aa) != lipValueAa)
                SetExpressionWeight(ExpressionKey.Aa, lipValueAa);

            if (GetExpressionWeight(ExpressionKey.Ih) != lipValueIh)
                SetExpressionWeight(ExpressionKey.Ih, lipValueIh);

            if (GetExpressionWeight(ExpressionKey.Ou) != lipValueOu)
                SetExpressionWeight(ExpressionKey.Ou, lipValueOu);

            if (GetExpressionWeight(ExpressionKey.Ee) != lipValueEe)
                SetExpressionWeight(ExpressionKey.Ee, lipValueEe);

            if (GetExpressionWeight(ExpressionKey.Oh) != lipValueOh)
                SetExpressionWeight(ExpressionKey.Oh, lipValueOh);
        }

        protected void OnSetPositionBone(HumanBodyBones bone, Vector3 pos, float slerp) => OnSetPositionBone(bone, pos, slerp, null);
        protected void OnSetPositionBone(HumanBodyBones bone, Vector3 pos, Transform rootParent) => OnSetPositionBone(bone, pos, 1f, rootParent);
        protected virtual void OnSetPositionBone(HumanBodyBones bone, Vector3 pos, float slerp = 1f, Transform rootParent = null)
        {
            if (controlRig == null || controlRig.Bones == null)
                return;

            if (controlRig.Bones.ContainsKey(bone))
            {
                if (controlRig.Bones[bone].ControlBone != null)
                {
                    if (rootParent != null)
                    {
                        if (slerp >= 1f)
                        {
                            controlRig.Bones[bone].ControlBone.position = rootParent.TransformPoint(pos);
                        }
                        else if (slerp > 0f)
                        {
                            controlRig.Bones[bone].ControlBone.position = Vector3.Slerp(controlRig.Bones[bone].ControlBone.position, rootParent.TransformPoint(pos), slerp);
                        }
                    }
                    else
                    {
                        if (slerp >= 1f)
                        {
                            controlRig.Bones[bone].ControlBone.localPosition = pos;
                        }
                        else if (slerp > 0f)
                        {
                            controlRig.Bones[bone].ControlBone.localPosition = Vector3.Slerp(controlRig.Bones[bone].ControlBone.localPosition, pos, slerp);
                        }
                    }
                }
            }
        }

        protected virtual void OnSetQuaternionBone(HumanBodyBones bone, Quaternion rot, float slerp = 1f)
        {
            if (controlRig == null || controlRig.Bones == null)
                return;

            if (controlRig.Bones.ContainsKey(bone))
            {
                if (controlRig.Bones[bone].ControlBone != null)
                {
                    if (slerp >= 1f)
                    {
                        controlRig.Bones[bone].ControlBone.localRotation = rot;
                    }
                    else if (slerp > 0f)
                    {
                        controlRig.Bones[bone].ControlBone.localRotation = Quaternion.Slerp(controlRig.Bones[bone].ControlBone.localRotation, rot, slerp);
                    }
                }
            }
        }

        protected Vector3 GetRootLocalPos(float heightRate)
        {
            if (humanoid != null)
            {
                if (humanoid.Hips != null && rootHeight > 0f)
                {
                    if (ModelRoot != null)
                    {
                        _rootLocalPos = ModelRoot.transform.InverseTransformPoint(humanoid.Hips.position);
                        _rootLocalPos.y = rootHeight * heightRate;
                        return _rootLocalPos;
                    }
                    else
                    {
                        _rootLocalPos = humanoid.Hips.localPosition;
                        _rootLocalPos.y = rootHeight * heightRate;
                        return _rootLocalPos;
                    }
                }
            }
            return Vector3.zero;
        }

        protected Quaternion GetFingerLocalRot(FingerBones finger, KnuckleBones knuckle, float value, bool right = false)
        {
            _fingerRot = Quaternion.identity;

            /*
             * 割合の応じて指関節の回転量と曲げる方向は
             * ココで決めている
             * 数値範囲：0-1
             */

            if (finger == FingerBones.Thumb)
            {
                _fingerRotValue = (value < 0 ? 0f : value > 1 ? 1f : value);

                if (knuckle == KnuckleBones.Proximal)
                {
                    _fingerRot.x = 1f * _fingerRotValue;
                    _fingerRot.y = 0f;
                    _fingerRot.z = 0f;
                }

                if (knuckle == KnuckleBones.Intermediate)
                {
                    _fingerRot.x = 1f * _fingerRotValue;
                    _fingerRot.y = 0f;
                    _fingerRot.z = 0f;
                }

                if (knuckle == KnuckleBones.Distal)
                {
                    _fingerRot.x = 1f * _fingerRotValue;
                    _fingerRot.y = 0f;
                    _fingerRot.z = 1f * (right ? -_fingerRotValue : _fingerRotValue);
                }
            }
            else
            {
                _fingerRotValue = (value < 0 ? 0f : value > 1 ? 1f : value);
                _fingerRotValueP = (value >= 0 ? 0f : Mathf.Abs(value));

                if (finger == FingerBones.Index)
                {
                    if (knuckle == KnuckleBones.Proximal)
                    {
                        _fingerRot.x = 0f;
                        _fingerRot.y = 0.1f * (right ? -_fingerRotValueP : _fingerRotValueP);
                        _fingerRot.z = 1f * (right ? -_fingerRotValue : _fingerRotValue);
                    }
                    else
                    {
                        _fingerRot.x = 0f;
                        _fingerRot.y = 0f;
                        _fingerRot.z = 1f * (right ? -_fingerRotValue : _fingerRotValue);
                    }
                }

                if (finger == FingerBones.Middle)
                {
                    if (knuckle == KnuckleBones.Proximal)
                    {
                        _fingerRot.x = 0f;
                        _fingerRot.y = 0f;
                        _fingerRot.z = 1f * (right ? -_fingerRotValue : _fingerRotValue);
                    }
                    else
                    {
                        _fingerRot.x = 0f;
                        _fingerRot.y = 0f;
                        _fingerRot.z = 1f * (right ? -_fingerRotValue : _fingerRotValue);
                    }
                }

                if (finger == FingerBones.Ring)
                {
                    if (knuckle == KnuckleBones.Proximal)
                    {
                        _fingerRot.x = 0f;
                        _fingerRot.y = -0.1f * (right ? -_fingerRotValueP : _fingerRotValueP);
                        _fingerRot.z = 1f * (right ? -_fingerRotValue : _fingerRotValue);
                    }
                    else
                    {
                        _fingerRot.x = 0f;
                        _fingerRot.y = 0f;
                        _fingerRot.z = 1f * (right ? -_fingerRotValue : _fingerRotValue);
                    }
                }

                if (finger == FingerBones.Little)
                {
                    if (knuckle == KnuckleBones.Proximal)
                    {
                        _fingerRot.x = 0f;
                        _fingerRot.y = -0.2f * (right ? -_fingerRotValueP : _fingerRotValueP);
                        _fingerRot.z = 1f * (right ? -_fingerRotValue : _fingerRotValue);
                    }
                    else
                    {
                        _fingerRot.x = 0f;
                        _fingerRot.y = 0f;
                        _fingerRot.z = 1f * (right ? -_fingerRotValue : _fingerRotValue);
                    }
                }
            }

            return _fingerRot;
        }

        protected void SetBlinkWait()
        {
            OnDispose();

            _blinkWait = true;

            var time = _blinkWaitTime[UnityEngine.Random.Range(0, _blinkWaitTime.Length - 1)];
            _onBlink = Observable.Interval(TimeSpan.FromSeconds(time))
                .Subscribe(_ =>
                {
                    _weight = 1f;
                });
        }

        protected void OnDispose()
        {
            _onBlink?.Dispose();
            _onBlink = null;
            _blinkWait = false;
        }

        public void UpdateBlink(float deltaTime)
        {
            if (_weight > 0f)
            {
                _weight -= _weightValue * deltaTime;
            }
            else if (_weight < 0f)
            {
                _blinkWait = false;
                _weight = 0f;
                SetBlinkWait();
            }

            if (GetExpressionWeight(ExpressionKey.Neutral) >= 0.95f)
                SetExpressionWeight(ExpressionKey.Blink, _weight);
        }

        protected void OnSetExpression(ExpressionEnum expression)
        {
            if (!nowExpression.Equals(ToExpressionKey(expression)))
                ResetExpressionWeight(nowExpression);

            nowExpression = ToExpressionKey(expression);
        }

        protected ExpressionKey ToExpressionKey(ExpressionEnum expression)
        {
            switch (expression)
            {
                case ExpressionEnum.Happy: return ExpressionKey.Happy;
                case ExpressionEnum.Angry: return ExpressionKey.Angry;
                case ExpressionEnum.Sad: return ExpressionKey.Sad;
                case ExpressionEnum.Relaxed: return ExpressionKey.Relaxed;
                case ExpressionEnum.Surprised: return ExpressionKey.Surprised;
                case ExpressionEnum.Blink: return ExpressionKey.Blink;
                case ExpressionEnum.WinkLeft: return ExpressionKey.BlinkLeft;
                case ExpressionEnum.WinkRight: return ExpressionKey.BlinkRight;

                default: break;
            }

            return ExpressionKey.Neutral;
        }

        protected void ResetExpressionWeight(ExpressionKey key)
            => expression?.SetWeight(key, 0f);

        protected float GetExpressionWeight(ExpressionKey key)
        {
            return expression != null ? expression.GetWeight(key) : 0f;
        }

        protected void SetExpressionWeight(ExpressionKey key, float value)
        {
            _beforeExpression = expression != null ? expression.GetWeight(key) : 0f;
            expression?.SetWeight(key, ToLerp(_beforeExpression, value, expressionLerp));
        }

        protected float ToLerp(float start, float end, float lerp)
            => start + (end - start) * Mathf.Clamp01(lerp);

        #endregion
    }
}
