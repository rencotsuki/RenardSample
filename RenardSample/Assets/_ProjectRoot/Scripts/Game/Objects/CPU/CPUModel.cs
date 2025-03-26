using System;
using UnityEngine;

[Serializable]
public abstract class CPUModel : MonoBehaviourCustom
{
    [SerializeField] protected GameObject modelRoot = default;
    [SerializeField] protected Transform headAnchor = default;

    public Transform Head => headAnchor;

    protected bool visible { get; private set; } = false;

    /// <summary></summary>
    public CPUModel Setup(string customParam = "")
    {
        OnSetup(customParam);
        return this;
    }

    protected virtual void OnSetup(string customParam) { }

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

    /// <summary></summary>
    public CPUModel SetVisible(bool value)
    {
        OnSetVisible(value);
        visible = value;
        return this;
    }

    protected virtual void OnSetVisible(bool value) { }

    /// <summary></summary>
    public CPUModel UpdateDraw(Vector3 floorPos)
    {
        OnUpdateDraw(floorPos);
        return this;
    }

    protected virtual void OnUpdateDraw(Vector3 floorPos) { }

    #region モーション

    /// <summary></summary>
    public void Appear()
    {
        OnAppear();
    }

    protected virtual void OnAppear() { }

    /// <summary></summary>
    public void Disappear()
    {
        OnDisappear();
    }

    protected virtual void OnDisappear() { }

    /// <summary></summary>
    public void GameBegin()
    {
        OnGameBegin();
    }

    protected virtual void OnGameBegin() { }

    /// <summary></summary>
    public void GameFinish(bool win)
    {
        OnGameFinish(win);
    }

    protected virtual void OnGameFinish(bool win) { }

    /// <summary></summary>
    public void Attack() => OnAttack();

    protected virtual void OnAttack() { }

    /// <summary></summary>
    public void Defense() => OnDefense();

    protected virtual void OnDefense() { }

    /// <summary></summary>
    public void PointGet() => OnPointGet();

    protected virtual void OnPointGet() { }

    /// <summary></summary>
    public void Damage() => OnDamage();

    protected virtual void OnDamage() { }

    /// <summary></summary>
    public void Death() => OnDeath();

    protected virtual void OnDeath() { }

    /// <summary></summary>
    public void Repair() => OnRepair();

    protected virtual void OnRepair() { }

    #endregion
}
