using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class BulletShadow : MonoBehaviourCustom
{
    [SerializeField] private Transform _model = default;
    [SerializeField] private MeshRenderer _meshRenderer = default;
    [SerializeField] private Material _defaultMat = default;

    private Vector3 localPosition
    {
        get
        {
            if (transform != null)
            {
                if (transform.parent != null)
                    return transform.parent.localPosition;
                return transform.localPosition;
            }
            return Vector3.zero;
        }
    }
    private Vector3 localScale
    {
        get
        {
            if (transform != null)
            {
                if (transform.parent != null)
                    return transform.parent.localScale;
                return transform.localScale;
            }
            return Vector3.one;
        }
    }

    private bool _visible = false;
    private Material _material = null;
    [HideInInspector] private Vector3 _shadowSize = Vector3.zero;

    private void Start()
    {
        CreateMaterial(_defaultMat, out _material);
        SetMaterial(_material, _meshRenderer);
        Return();
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

    public void SetDrawParam(float scale, float speed)
    {
        _shadowSize.x = scale;
        _shadowSize.y = 0.01f;
        _shadowSize.z = (speed - 0.7f) < 0 ? scale : scale * ((speed - 0.7f) * 0.7f + 1f);

        if (_model != null)
            _model.localScale = _shadowSize;
    }

    public void Appear()
    {
        _visible = true;
    }

    public void Hide()
    {
        _visible = false;
    }

    public void Return()
    {
        Hide();

        if (_meshRenderer != null && _meshRenderer.enabled)
            _meshRenderer.enabled = false;
    }

    public void UpdateDraw()
    {
        try
        {
            if (!_visible || localScale.y <= 0)
            {
                if (_meshRenderer.enabled)
                    _meshRenderer.enabled = false;
            }
            else
            {
                if (!_meshRenderer.enabled)
                    _meshRenderer.enabled = true;

                if (_model != null)
                    _model.localPosition = new Vector3(0f, -localPosition.y / localScale.y + 0.01f, 0f);
            }

            _material?.SetFloat("_Value_Height", localPosition.y);
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "UpdateDraw", $"{ex.Message}");
        }
    }
}
