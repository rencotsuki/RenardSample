using System;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class BulletModel : MonoBehaviourCustom
{
    [SerializeField] private Animator _animator = null;
    [SerializeField] private Animator _animatorDotBall = null;
    [SerializeField] private Transform _scaleAnm = null;
    [SerializeField] private MeshRenderer _meshRendererDotBall = null;
    [SerializeField] private MeshRenderer _meshRendererCenterCore = null;
    [SerializeField] private MeshRenderer _meshRendererGrea = null;
    [SerializeField] private MeshRenderer _meshRendererThunder = null;
    [SerializeField] private TrailRenderer _trailRendererTrail = null;
    [SerializeField] private ParticleSystem _particleDotAppear = null;
    [SerializeField] private ParticleSystem _particleBackFire = null;
    [SerializeField] private ParticleSystem _particleCircleLine = null;
    [SerializeField] private ParticleSystem _particleDotL = null;
    [SerializeField] private ParticleSystem _particleDotS = null;
    [SerializeField] private float breakAnime = 0.5f;
    [SerializeField] private Vector3 dotRollValue = new Vector3(-900f, 0f, 0f);

    public float BreakWaitTime => breakAnime;

    [Serializable]
    private struct MaterialData
    {
        public Material Dot;
        public Material Core;
        public Material Grea;
        public Material Thunder;
        public Material Trail;
        public Material HexFlake;
        public Material CircleLine;
    }

    [SerializeField] private MaterialData _materialData = new MaterialData();

    protected Material dot = null;
    protected Material core = null;
    protected Material grea = null;
    protected Material thunder = null;
    protected Material trail = null;
    protected Material hexFlake = null;
    protected Material circleLine = null;

    [HideInInspector] private Vector3 _scaleMeshSize = Vector3.zero;
    [HideInInspector] private Vector3 _scaleMeshPosition = Vector3.zero;
    [HideInInspector] private Vector3 _centerCoreSize = Vector3.zero;
    [HideInInspector] private float _greaXY = 0f;
    [HideInInspector] private float _greaZ = 0f;
    [HideInInspector] private float _greaAlpha = 0f;
    [HideInInspector] private float _greaBate = 0f;
    [HideInInspector] private Vector3 _greaSize = Vector3.zero;
    [HideInInspector] private Vector3 _greaPosition = Vector3.zero;
    [HideInInspector] private Vector3 _thunderSize = Vector3.zero;
    [HideInInspector] private Vector3 _trailPosition = Vector3.zero;
    [HideInInspector] private Vector4 _anchorPosVector4 = Vector4.zero;

    private Sequence _animationSequence = null;
    private Tween _tweenDotRoll = null;

    private void Awake()
    {
        CreateMaterial(_materialData.Dot, out dot);
        CreateMaterial(_materialData.Core, out core);
        CreateMaterial(_materialData.Grea, out grea);
        CreateMaterial(_materialData.Thunder, out thunder);
        CreateMaterial(_materialData.Trail, out trail);
        CreateMaterial(_materialData.HexFlake, out hexFlake);
        CreateMaterial(_materialData.CircleLine, out circleLine);

        SetMaterial(dot, _meshRendererDotBall);
        SetMaterial(core, _meshRendererCenterCore);
        SetMaterial(grea, _meshRendererGrea);
        SetMaterial(thunder, _meshRendererThunder);
        SetMaterial(trail, _trailRendererTrail);
        SetMaterial(hexFlake, _particleDotAppear, _particleDotL, _particleDotS);
        SetMaterial(circleLine, _particleBackFire, _particleCircleLine);

        // TODO: 現行もAnimatorほぼ使ってないなら素材から外した方がいいのでは？

        // 制御しきれないのでアニメーションは使わない
        if (_animator != null)
        {
            _animator?.ResetTrigger("Disappear");
            _animator.enabled = false;
        }

        // アニメーションの軸が展開方向によっては合わないので使わない
        if (_animatorDotBall != null)
            _animatorDotBall.enabled = false;
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
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "CreateMaterial", $"{ex.Message}");

            result = null;
            return false;
        }
    }

    public void Return()
    {
        KillAnimation();
    }

    protected void SetMaterial<T>(Material material, params T[] renderers) where T : Renderer
    {
        try
        {
            if (material == null)
                throw new Exception("set material null.");

            if (renderers == null || renderers.Length <= 0)
                throw new Exception("target or renderers null or empty.");

            foreach (var renderer in renderers)
            {
                try
                {
                    if (renderer.material != material)
                        renderer.material = material;
                }
                catch (Exception ex)
                {
                    Log(DebugerLogType.Info, "SetMaterial", $"{ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "SetMaterial", $"{ex.Message}");
        }
    }

    protected void SetMaterials<T>(Material[] materials, params T[] renderers) where T : Renderer
    {
        try
        {
            if (materials == null && materials.Length <= 0)
                throw new Exception("set materials null.");

            if (renderers == null || renderers.Length <= 0)
                throw new Exception("target or renderers null or empty.");

            foreach (var renderer in renderers)
            {
                try
                {
                    renderer.materials = materials;
                }
                catch (Exception ex)
                {
                    Log(DebugerLogType.Info, "SetMaterials", $"{ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "SetMaterials", $"{ex.Message}");
        }
    }

    protected void SetMaterial(Material material, params ParticleSystem[] particles)
    {
        try
        {
            if (material == null)
                throw new Exception("set material null.");

            if (particles == null || particles.Length <= 0)
                throw new Exception("target or particles null or empty.");

            ParticleSystemRenderer renderer = null;

            foreach (var particle in particles)
            {
                try
                {
                    renderer = particle?.GetComponent<ParticleSystemRenderer>();

                    if (renderer != null && renderer.material != material)
                        renderer.material = material;
                }
                catch (Exception ex)
                {
                    Log(DebugerLogType.Info, "SetMaterial", $"{ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "SetMaterial", $"{ex.Message}");
        }
    }

    public void SetDrawParam(float scale, float speed)
    {
        if (_meshRendererDotBall != null)
        {
            _scaleMeshSize = new Vector3(scale,
                                         scale,
                                         (speed - 0.7f) < 0 ? scale : scale * ((speed - 0.7f) * 0.7f + 1f));

            _scaleMeshPosition = new Vector3(0f,
                                             0f,
                                             (speed - 0.7f) < 0 ? 0 : (scale + 0.5f) * ((speed - 0.7f) * -0.07f));

            _scaleAnm.parent.transform.localScale = _scaleMeshSize;
            _scaleAnm.parent.transform.localPosition = _scaleMeshPosition;

            _meshRendererDotBall.transform.localPosition = Vector3.zero;
            _meshRendererDotBall.transform.localRotation = Quaternion.identity;
            _meshRendererDotBall.material.SetFloat("_Outline_Width", scale);
        }

        if (_meshRendererCenterCore != null)
        {
            _centerCoreSize = new Vector3(scale + 0.5f,
                                          scale + 0.5f,
                                          (speed - 0.5f) < 0 ? scale + 0.5f : (scale + 0.5f) * ((speed - 0.5f) * 0.3f + 1f));

            _meshRendererCenterCore.transform.localScale = _centerCoreSize;
        }

        if (_meshRendererGrea != null)
        {
            _greaXY = (0.5f - scale) < 0 ? scale : (0.5f - scale) * 0.3f + scale;
            _greaAlpha = (0.5f - scale) < 0 ? 0 : (0.5f - scale);
            _greaBate = (speed - 0.7f) < 0 ? 0 : (speed - 0.7f);
            _greaZ = (_greaAlpha * 0.5f + scale) * (_greaBate * 0.4f + 1f);
            _greaSize = new Vector3(_greaXY, _greaXY, _greaZ);

            _greaPosition = new Vector3(0f,
                                        0f,
                                        (speed - 0.7f) < 0 ? 0 : (scale + 0.3f) * ((speed - 0.7f) * -0.13f));

            _meshRendererGrea.transform.localScale = _greaSize;
            _meshRendererGrea.transform.localPosition = _greaPosition;
        }

        if (_meshRendererThunder != null)
        {
            _thunderSize = Vector3.one * (((1f - scale) * 0.5f) + scale);

            _meshRendererThunder.transform.localScale = _thunderSize;
            _meshRendererThunder.material.SetFloat("_Value_Speed", speed);
        }

        if (_trailRendererTrail != null)
        {
            _trailPosition = new Vector3(0f,
                                         0f,
                                         (scale - 0.2f) * -0.4f);

            _trailRendererTrail.transform.localPosition = _trailPosition;
            _trailRendererTrail.startWidth = scale;
            _trailRendererTrail.endWidth = scale;
        }
    }

    public void Appear()
    {
        KillAnimation();

        AppearAnime();

        if (_meshRendererDotBall != null && dotRollValue != Vector3.zero)
        {
            _tweenDotRoll = _meshRendererDotBall.transform
                                .DOLocalRotate(dotRollValue, 1f, RotateMode.FastBeyond360)
                                .SetEase(Ease.Linear)
                                .SetLoops(-1, LoopType.Incremental);
            _tweenDotRoll?.Play();
        }

        SetParticleEffects(true, false);
    }

    public void Disappear()
    {
        KillAnimation();
        DisappearAnime();
    }

    public void HitBreak()
    {
        KillAnimation();
        DisappearAnime();
    }

    private void KillAnimation()
    {
        _animationSequence?.Kill();
        _animationSequence = null;

        _tweenDotRoll?.Kill();

        SetParticleEffects(false);
    }

    private void AppearAnime()
    {
        if (_scaleAnm != null)
            _scaleAnm.localScale = Vector3.one;

        dot?.SetFloat("_Value_Alpha", 1f);
        core?.SetFloat("_Value_alpha", 1f); // 小文字が混ざってる。。。
        grea?.SetFloat("_Value_Alpha", 1f);
        grea?.SetFloat("_Value_Dark", 1f);
        thunder?.SetFloat("_Value_Alpha", 1f);
    }

    private void DisappearAnime()
    {
        if (_scaleAnm != null)
            _scaleAnm.localScale = Vector3.one;

        _animationSequence = DOTween.Sequence();

        _animationSequence
            .Prepend(DOTween.To(() => 1, (n) => { }, 0f, 0.3f))
            .Join(DisappearScaleAnm())
            .Join(DisappearDotBall())
            .Join(DisappearCore())
            .Join(DisappearGrea())
            .Join(DisappearThunder());

        _animationSequence?.Play();
    }

    private Sequence DisappearScaleAnm()
    {
        return DOTween.Sequence()
            .OnStart(() => _scaleAnm.localScale = new Vector3(1.2f, 1.2f, 0.8f))
            .Prepend(_scaleAnm.DOScale(new Vector3(3f, 3f, 0.4f), 0.04f))
            .Append(_scaleAnm.DOScale(new Vector3(4f, 4f, 0.05f), 0.03f))
            .Append(_scaleAnm.DOScale(new Vector3(0.05f, 0.05f, 0.05f), 0.01f));
    }

    private Sequence DisappearDotBall()
    {
        if (dot == null)
            return DOTween.Sequence();

        return DOTween.Sequence()
            .OnStart(() => dot.SetFloat("_Value_Alpha", 1f))
            .Prepend(DOTween.To(() => dot.GetFloat("_Value_Alpha"), (n) => dot.SetFloat("_Value_Alpha", n), 0.3f, 0.04f))
            .Append(DOTween.To(() => dot.GetFloat("_Value_Alpha"), (n) => dot.SetFloat("_Value_Alpha", n), 0f, 0.03f));
    }

    private Tween DisappearCore()
    {
        if (core == null)
            return DOTween.Sequence();

        // 小文字が混ざってる。。。
        return DOTween.Sequence()
            .OnStart(() => core.SetFloat("_Value_alpha", 1f))
            .Prepend(DOTween.To(() => core.GetFloat("_Value_alpha"), (n) => core.SetFloat("_Value_alpha", n), 0f, 0.05f));
    }

    private Sequence DisappearGrea()
    {
        if (grea == null)
            return DOTween.Sequence();

        return DOTween.Sequence()
            .OnStart(() =>
            {
                grea.SetFloat("_Value_Alpha", 1f);
                grea.SetFloat("_Value_Dark", 1f);
            })
            .Prepend(DOTween.To(() => grea.GetFloat("_Value_Alpha"), (n) => grea.SetFloat("_Value_Alpha", n), 0f, 0.05f))
            .Join(DOTween.To(() => grea.GetFloat("_Value_Dark"), (n) => grea.SetFloat("_Value_Dark", n), 0f, 0.02f));
    }

    private Sequence DisappearThunder()
    {
        //if (thunder == null)
        //    return DOTween.Sequence();

        //return DOTween.Sequence()
        //    .OnStart(() => thunder.SetFloat("_Value_Alpha", 1f))
        //    .Prepend(DOTween.To(() => thunder.GetFloat("_Value_Alpha"), (n) => thunder.SetFloat("_Value_Alpha", n), 0f, 0.05f));

        // このプロジェクトでも参照できない。なぜ？
        if (_meshRendererThunder == null || _meshRendererThunder.material == null)
            return DOTween.Sequence();

        return DOTween.Sequence()
            .OnStart(() => _meshRendererThunder.material.SetFloat("_Value_Alpha", 1f))
            .Prepend(DOTween.To(() => _meshRendererThunder.material.GetFloat("_Value_Alpha"), (n) => _meshRendererThunder.material.SetFloat("_Value_Alpha", n), 0f, 0.05f));
    }

    private void SetParticleEffects(bool enabled, bool trailEnabled = false)
    {
        if (_trailRendererTrail != null)
        {
            _trailRendererTrail.enabled = trailEnabled;
            _trailRendererTrail.Clear();
        }

        if (enabled)
        {
            _particleDotAppear?.Play();
            _particleDotL?.Play();
            _particleDotS?.Play();
            _particleBackFire?.Play();
            _particleCircleLine?.Play();
        }
        else
        {
            _particleDotAppear?.Stop();
            _particleBackFire?.Stop();
            _particleCircleLine?.Stop();
            _particleDotL?.Stop();
            _particleDotS?.Stop();
        }
    }

    public void UpdateDraw(Vector3 anchorPos)
    {
        try
        {
            _anchorPosVector4 = new Vector4(anchorPos.x, anchorPos.y, anchorPos.z, 1f);

            dot?.SetVector("_anchorPos", _anchorPosVector4);
            core?.SetVector("_anchorPos", _anchorPosVector4);
            grea?.SetVector("_anchorPos", _anchorPosVector4);
            thunder?.SetVector("_anchorPos", _anchorPosVector4);
            trail?.SetVector("_anchorPos", _anchorPosVector4);
            hexFlake?.SetVector("_anchorPos", _anchorPosVector4);
            circleLine?.SetVector("_anchorPos", _anchorPosVector4);
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "UpdateDraw", $"{ex.Message}");
        }
    }
}
