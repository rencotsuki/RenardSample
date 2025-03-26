using System;
using UnityEngine;
using UniRx;
using SignageHADO.Game;

[Serializable]
public class VirtualCourt : MonoBehaviourCustom
{
    [SerializeField] private MeshRenderer[] planes = null;
    [SerializeField] private RigidbodySupport rigidbodySupport = null;
    [SerializeField] private ColliderSupport[] colliderSupports = null;

    private Material[] _planeMaterials = null;
    private BulletScript _tempBulletScript = null;

    private void Awake()
    {
        CreateMaterials();
    }

    private void CreateMaterials()
    {
        if (planes == null || planes.Length <= 0)
            return;

        _planeMaterials = new Material[planes.Length];

        for (int i = 0; i < planes.Length; i++)
        {
            _planeMaterials[i] = new Material(planes[i].material);
            _planeMaterials[i].CopyPropertiesFromMaterial(planes[i].material);
            planes[i].material = _planeMaterials[i];
        }
    }

    private void Start()
    {
        SetupCollider();
    }

    private void SetupCollider()
    {
        foreach (var collider in colliderSupports)
        {
            if (collider == null)
                continue;

            collider.Setup(this);
            collider.SetupOwner(false);
            collider.Enabled = true;
        }

        if (rigidbodySupport != null)
        {
            rigidbodySupport.OnCollisionEnterObservable
                .Subscribe(report => OnCollisionEnterObserver(report))
                .AddTo(rigidbodySupport);

            rigidbodySupport.OnCollisionStayObservable
                .Subscribe(report => OnCollisionStayObserver(report))
                .AddTo(rigidbodySupport);

            rigidbodySupport.OnCollisionExitObservable
                .Subscribe(report => OnCollisionExitObserver(report))
                .AddTo(rigidbodySupport);

            rigidbodySupport.Enabled = true;
        }
    }

    public bool SetPlaneTextures(Texture[] planes)
    {
        try
        {
            if (planes == null || planes.Length != 5)
                throw new Exception($"not textures. length={(planes != null ? planes.Length : 0)}");

            for (int i = 0; i < _planeMaterials.Length; i++)
            {
                _planeMaterials[i].SetTexture("_MainTex", planes[i]);
            }

            return true;
        }
        catch (Exception ex)
        {
            Log(DebugerLogType.Info, "SetPlaneTextures", $"{ex.Message}");
        }
        return false;
    }

    private void OnCollisionEnterObserver(ColliderReport report)
    {
        CheckHit(report);
    }

    private void OnCollisionStayObserver(ColliderReport report)
    {
        CheckHit(report);
    }

    private void OnCollisionExitObserver(ColliderReport report)
    {
        //CheckHit(report);
    }

    private BulletScript GetBulletColliderReport(ColliderReport report)
    {
        if (report.HitCollider != null && report.HitCollider.AnchorComponent != null)
            return report.HitCollider.AnchorComponent.GetComponent<BulletScript>();
        return null;
    }

    private void CheckHit(ColliderReport report)
    {
        if (report.HitCollider == null)
            return;

        _tempBulletScript = GetBulletColliderReport(report);
        // 壁ヒット
        _tempBulletScript?.Break(false);
    }
}