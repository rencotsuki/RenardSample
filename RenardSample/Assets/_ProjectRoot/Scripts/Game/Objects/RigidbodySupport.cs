using System;
using UnityEngine;
using UniRx;

[Serializable]
public struct ColliderReport
{
    public ColliderSupport HitCollider { get; private set; }
    public Vector3 HitPos { get; private set; }
    public Vector3 HitLocalPos { get; private set; }

    public static ColliderReport Create(ColliderSupport hitCollider, Vector3 hitPos, Vector3 hitLocalPos)
        => new ColliderReport()
        {
            HitCollider = hitCollider,
            HitPos = hitPos,
            HitLocalPos = hitLocalPos
        };
}

[SerializeField]
public class RigidbodySupport : MonoBehaviourCustom
{
    private Rigidbody _rigidbody = null;

    protected Transform anchorField { get; private set; } = null;

    private bool _enabled = false;
    public bool Enabled
    {
        get { return _rigidbody != null ? _enabled : false; }
        set
        {
            if (_rigidbody == null) return;

            _enabled = value;

            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rigidbody.collisionDetectionMode = _enabled ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
            _rigidbody.constraints = _enabled ? (RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation) : RigidbodyConstraints.None;
        }
    }

    public Vector3 Velocity
    {
        get { return _rigidbody != null ? _rigidbody.velocity : Vector3.zero; }
        set { if (_rigidbody != null) _rigidbody.velocity = value; }
    }

    private ColliderSupport _tempGetComponent = null;

    private Subject<ColliderReport> _onCollisionEnterSubject = new Subject<ColliderReport>();
    public IObservable<ColliderReport> OnCollisionEnterObservable => _onCollisionEnterSubject?.AsObservable();

    private Subject<ColliderReport> _onCollisionStaySubject = new Subject<ColliderReport>();
    public IObservable<ColliderReport> OnCollisionStayObservable => _onCollisionStaySubject?.AsObservable();

    private Subject<ColliderReport> _onCollisionExitSubject = new Subject<ColliderReport>();
    public IObservable<ColliderReport> OnCollisionExitObservable => _onCollisionExitSubject?.AsObservable();

    private void Awake()
    {
        IsDebugLog = false;

        anchorField = transform.parent != null ? transform.parent : transform;

        _rigidbody = gameObject.GetComponent<Rigidbody>();

        if (_rigidbody == null)
            _rigidbody = gameObject.AddComponent<Rigidbody>();

        Enabled = false;
    }

    public void SetAnchorField(Transform traget)
    {
        if (traget != null)
            anchorField = traget;
    }

    private Vector3 ToLocalPostion(Vector3 worldPos)
    {
        if (anchorField != null)
            return anchorField.InverseTransformPoint(worldPos);
        return worldPos;
    }

    private ColliderSupport ToColliderSupport(GameObject colliderObject)
    {
        if (colliderObject == null) return null;
        return colliderObject.GetComponent<ColliderSupport>();
    }

    private void OnCollisionEnterSubject(GameObject colliderObject, Vector3 hitPos)
    {
        if (!_enabled || _onCollisionEnterSubject == null) return;

        _tempGetComponent = ToColliderSupport(colliderObject);
        if (_tempGetComponent != null)
        {
            _onCollisionEnterSubject.OnNext(ColliderReport.Create(_tempGetComponent, hitPos, ToLocalPostion(hitPos)));
        }
    }

    private void OnCollisionStaySubject(GameObject colliderObject, Vector3 hitPos)
    {
        if (!_enabled || _onCollisionStaySubject == null) return;

        _tempGetComponent = ToColliderSupport(colliderObject);
        if (_tempGetComponent != null)
        {
            _onCollisionStaySubject.OnNext(ColliderReport.Create(_tempGetComponent, hitPos, ToLocalPostion(hitPos)));
        }
    }

    private void OnCollisionExitSubject(GameObject colliderObject, Vector3 hitPos)
    {
        if (!_enabled || _onCollisionExitSubject == null) return;

        _tempGetComponent = ToColliderSupport(colliderObject);
        if (_tempGetComponent != null)
        {
            _onCollisionExitSubject.OnNext(ColliderReport.Create(_tempGetComponent, hitPos, ToLocalPostion(hitPos)));
        }
    }

    //-- Collision

    private void OnCollisionEnter(Collision collision)
    {
        //Log(DebugerLogType.Info, "OnCollisionEnter", $"{(collision != null ? collision.gameObject.name : "")}");

        if (collision == null || collision.contacts.Length <= 0) return;
        OnCollisionEnterSubject(collision.gameObject, collision.contacts[0].point);
    }
    private void OnCollisionStay(Collision collision)
    {
        //Log(DebugerLogType.Info, "OnCollisionStay", $"{(collision != null ? collision.gameObject.name : "")}");

        if (collision == null || collision.contacts.Length <= 0) return;
        OnCollisionStaySubject(collision.gameObject, collision.contacts[0].point);
    }
    private void OnCollisionExit(Collision collision)
    {
        //Log(DebugerLogType.Info, "OnCollisionExit", $"{(collision != null ? collision.gameObject.name : "")}");

        if (collision == null || collision.contacts.Length <= 0) return;
        OnCollisionExitSubject(collision.gameObject, collision.contacts[0].point);
    }

    //-- Trigger

    private void OnTriggerEnter(Collider other)
    {
        //Log(DebugerLogType.Info, "OnTriggerEnter", $"{(other != null ? other.gameObject.name : "")}");

        if (other == null) return;
        OnCollisionEnterSubject(other.gameObject, _rigidbody.ClosestPointOnBounds(other.transform.position));
    }
    private void OnTriggerStay(Collider other)
    {
        //Log(DebugerLogType.Info, "OnTriggerStay", $"{(other != null ? other.gameObject.name : "")}");

        if (other == null) return;
        OnCollisionStaySubject(other.gameObject, _rigidbody.ClosestPointOnBounds(other.transform.position));
    }
    private void OnTriggerExit(Collider other)
    {
        //Log(DebugerLogType.Info, "OnTriggerExit", $"{(other != null ? other.gameObject.name : "")}");

        if (other == null) return;
        OnCollisionExitSubject(other.gameObject, _rigidbody.ClosestPointOnBounds(other.transform.position));
    }
}
