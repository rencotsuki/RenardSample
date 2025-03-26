using System;
using UnityEngine;

[Serializable]
public class CPUPanelModel : CPUModel
{
    [Serializable]
    protected class CustomModel
    {
        public Animator Animator = default;

        [SerializeField] private MeshRenderer meshBase = default;
        [SerializeField] private MeshRenderer meshFace = default;
        [SerializeField] private MeshRenderer meshBack = default;
        [SerializeField] private MeshRenderer meshInFace = default;
        [SerializeField] private MeshRenderer meshInBack = default;
        [SerializeField] private float[] inPanelOffsets = new float[] { 0.02f, 0.015f, 0.01f, 0.005f, 0f, -0.005f, -0.01f, -0.015f, -0.02f };

        [HideInInspector] public Material[] MaterialBase = null;
        [HideInInspector] public Material MaterialFace = null;
        [HideInInspector] public Material MaterialBack = null;
        [HideInInspector] public Material MaterialShadow = null;
        [HideInInspector] public int InPanelNum = 0;
        [HideInInspector] public Material[] MaterialInFace = null;
        [HideInInspector] public Material[] MaterialInBack = null;

        public void SetVisible(bool value)
        {
            if (meshBase != null && meshBase.enabled != value)
                meshBase.enabled = value;

            if (meshFace != null && meshFace.enabled != value)
                meshFace.enabled = value;

            if (meshBack != null && meshBack.enabled != value)
                meshBack.enabled = value;

            if (meshInFace != null && meshInFace.enabled != value)
                meshInFace.enabled = value;

            if (meshInBack != null && meshInBack.enabled != value)
                meshInBack.enabled = value;
        }

        public void CreateMaterials()
        {
            /*
             * 素材に設定されているMaterialを
             * 複製して利用するので
             * プレハブ設定に気を付ける
             */

            if (meshBase != null)
            {
                MaterialBase = new Material[meshBase.materials.Length];

                for (int i = 0; i < MaterialBase.Length; i++)
                {
                    CreateMaterial(meshBase.materials[i], out MaterialBase[i]);
                }

                meshBase.materials = MaterialBase;
            }

            if (meshFace != null)
            {
                if (CreateMaterial(meshFace.material, out MaterialFace))
                    meshFace.material = MaterialFace;
            }

            if (meshBack != null)
            {
                if (CreateMaterial(meshBack.material, out MaterialBack))
                    meshBack.material = MaterialBack;
            }

            InPanelNum = inPanelOffsets != null ? inPanelOffsets.Length : 0;

            if (meshInFace != null)
            {
                MaterialInFace = new Material[InPanelNum];

                for (int i = 0; i < InPanelNum; i++)
                {
                    CreateMaterial(meshInFace.material, out MaterialInFace[i]);
                    MaterialInFace[i]?.SetFloat("_Offset", inPanelOffsets[i]);
                }

                meshInFace.materials = MaterialInFace;
            }

            if (meshInBack != null)
            {
                MaterialInBack = new Material[InPanelNum];

                for (int i = 0; i < InPanelNum; i++)
                {
                    CreateMaterial(meshInBack.material, out MaterialInBack[i]);
                    MaterialInBack[i]?.SetFloat("_Offset", inPanelOffsets[i]);
                }

                meshInBack.materials = MaterialInBack;
            }
        }

        protected bool CreateMaterial(Material material, out Material result)
        {
            try
            {
                if (material == null)
                    throw new Exception("target or material null.");

                result = new Material(material);
                result.CopyPropertiesFromMaterial(material);

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }

    [SerializeField] private CustomModel[] models = null;
    protected CustomModel selectModel { get; private set; } = null;

    [SerializeField] protected MeshRenderer floorShadow = default;

    private string stateIdle => "Idle";
    private string stateHide => "Hide";

    private int triggerAppear => Animator.StringToHash("Appear");
    private int triggerDisappear => Animator.StringToHash("Disappear");
    private int triggerDamage => Animator.StringToHash("Damage");
    private int triggerBreak => Animator.StringToHash("Break");
    private int triggerFire => Animator.StringToHash("Fire");
    private int triggerWin => Animator.StringToHash("Win");

    protected Animator animator => selectModel != null ? selectModel.Animator : null;
    protected Material[] materialBase => selectModel != null ? selectModel.MaterialBase : null;
    protected Material materialFace => selectModel != null ? selectModel.MaterialFace : null;
    protected Material materialBack => selectModel != null ? selectModel.MaterialBack : null;
    protected Material[] materialInFace => selectModel != null ? selectModel.MaterialInFace : null;
    protected Material[] materialInBack => selectModel != null ? selectModel.MaterialInBack : null;
    protected int inPanelNum => selectModel != null ? selectModel.InPanelNum : 0;

    [HideInInspector] private Material _materialShadow = null;
    [HideInInspector] private Vector3 _materialFloorPos = Vector3.zero;
    [HideInInspector] private int _index = 0;

    private int CustomParamToCustomNo(string value)
    {
        if (!string.IsNullOrEmpty(value))
            if (value == "Square") return 1;
        return 0;
    }

    protected override void OnSetup(string customParam)
    {
        var customNo = CustomParamToCustomNo(customParam);

        for (int i = 0; i < models.Length; i++)
        {
            models[i]?.CreateMaterials();
            models[i]?.SetVisible(false);

            if (customNo == i)
                selectModel = models[i];
        }

        if (floorShadow != null)
        {
            if (CreateMaterial(floorShadow.material, out _materialShadow))
                floorShadow.material = _materialShadow;
        }

        SetVisible(false);
    }

    protected override void OnSetVisible(bool visible)
    {
        if (selectModel != null)
            selectModel?.SetVisible(visible);

        if (floorShadow != null && floorShadow.enabled != visible)
            floorShadow.enabled = visible;

        if (!visible)
            ResetAnimation();
    }

    protected override void OnUpdateDraw(Vector3 floorPos)
    {
        if (!visible || _materialFloorPos == floorPos)
            return;

        _materialFloorPos = floorPos;

        if (materialBase != null)
        {
            for (_index = 0; _index < materialBase.Length; _index++)
            {
                materialBase[_index]?.SetVector("_anchorPos", _materialFloorPos);
            }
        }

        if (materialFace != null)
            materialFace?.SetVector("_anchorPos", _materialFloorPos);

        if (materialBack != null)
            materialBack?.SetVector("_anchorPos", _materialFloorPos);

        if (_materialShadow != null)
            _materialShadow?.SetVector("_anchorPos", _materialFloorPos);

        for (_index = 0; _index < inPanelNum; _index++)
        {
            if (materialInFace != null && materialInFace.Length > _index)
                materialInFace[_index]?.SetVector("_anchorPos", _materialFloorPos);

            if (materialInBack != null && materialInBack.Length > _index)
                materialInBack[_index]?.SetVector("_anchorPos", _materialFloorPos);
        }
    }

    /// <summary></summary>
    public CPUPanelModel SetTextures(Texture2D idle, Texture2D fire, Texture2D hit, Texture2D die, Texture2D win)
    {
        OnSetTextures(materialFace, idle, fire, hit, die, win);
        OnSetTextures(materialBack, idle, fire, hit, die, win);

        for (int i = 0; i < inPanelNum; i++)
        {
            if (materialInFace != null && materialInFace.Length > i)
                OnSetTextures(materialInFace[i], idle, fire, hit, die, win);

            if (materialInBack != null && materialInBack.Length > i)
                OnSetTextures(materialInBack[i], idle, fire, hit, die, win);
        }

        return this;
    }

    private void OnSetTextures(Material target, Texture2D idle, Texture2D fire, Texture2D hit, Texture2D die, Texture2D win)
    {
        try
        {
            if (target == null)
                throw new Exception("target material null.");

            target.SetTexture("_Texture_Photo", idle);
            target.SetTexture("_Texture_Photo_Fire", fire);
            target.SetTexture("_Texture_Photo_Hit", hit);
            target.SetTexture("_Texture_Photo_Die", die);
            target.SetTexture("_Texture_Photo_Win", win);
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "OnSetTextures", $"{ex.Message}");
        }
    }

    #region　モーション

    private bool CurrentAnimationState(string stateName)
    {
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            return true;
        return false;
    }

    private void ResetAnimation()
    {
        ResetTrigger();
        animator?.Play(stateHide);
    }

    private void ResetTrigger()
    {
        animator?.ResetTrigger(triggerAppear);
        animator?.ResetTrigger(triggerDisappear);
        animator?.ResetTrigger(triggerDamage);
        animator?.ResetTrigger(triggerBreak);
        animator?.ResetTrigger(triggerFire);
        animator?.ResetTrigger(triggerWin);
    }

    protected override void OnAppear()
    {
        ResetTrigger();

        if (!CurrentAnimationState(stateIdle))
            animator?.SetTrigger(triggerAppear);
    }

    protected override void OnDisappear()
    {
        ResetTrigger();

        if (!CurrentAnimationState(stateHide))
            animator?.SetTrigger(triggerDisappear);
    }

    protected override void OnGameBegin()
    {
        // 開始時 とりあえずAppearを読んでおく
        OnAppear();
    }

    protected override void OnGameFinish(bool win)
    {
        ResetTrigger();

        if (win)
        {
            animator?.SetTrigger(triggerWin);
        }
        else
        {
            // 敗けパターン
        }
    }

    protected override void OnAttack()
    {
        ResetTrigger();
        animator?.SetTrigger(triggerFire);
    }

    protected override void OnDefense()
    {
        // シールド
    }

    protected override void OnPointGet()
    {
        // 得点
    }

    protected override void OnDamage()
    {
        ResetTrigger();
        animator?.SetTrigger(triggerDamage);
    }

    protected override void OnDeath()
    {
        ResetTrigger();
        animator?.SetTrigger(triggerBreak);
    }

    protected override void OnRepair()
    {
        ResetTrigger();
        animator?.SetTrigger(triggerAppear);
    }

    #endregion
}
