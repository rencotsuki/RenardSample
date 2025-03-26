using System;
using UnityEngine;
using UniRx;
using SignageHADO.Game;

[Serializable]
public class FieldHitObjects : MonoBehaviourCustom
{
    [SerializeField] private RigidbodySupport rigidbodySupport = default;
    [SerializeField] private ColliderSupport[] colliderSupport = default;

    private BulletScript _tmpBullet = null;

    private void Start()
    {
        if (rigidbodySupport != null)
        {
            rigidbodySupport.SetAnchorField(transform);

            rigidbodySupport.OnCollisionEnterObservable
                .Subscribe(OnCollision)
                .AddTo(this);

            rigidbodySupport.Enabled = true;
        }

        foreach (var collider in colliderSupport)
        {
            if (collider == null)
                continue;

            collider.Setup(this);
            collider.SetupOwner(false);
            collider.Enabled = true;
        }
    }

    private void OnCollision(ColliderReport report)
    {
        if (report.HitCollider == null)
            return;

        _tmpBullet = report.HitCollider.AnchorComponent?.GetComponent<BulletScript>();

        if (_tmpBullet == null || _tmpBullet.IsBreak)
            return;

        _tmpBullet.Break(false);
    }
}
