using UnityEngine;

[SerializeField]
[RequireComponent(typeof(Collider))]
public class ColliderSupport : MonoBehaviourCustom
{
    public bool IsEnemy { get; private set; } = false;

    public Component AnchorComponent { get; private set; } = null;

    public ColliderSupport Setup(Component anchorComponent = null)
    {
        AnchorComponent = anchorComponent;
        return this;
    }

    public ColliderSupport SetLayer(int layer)
    {
        gameObject.layer = layer;
        return this;
    }

    public ColliderSupport SetupOwner(bool enemy)
    {
        IsEnemy = enemy;
        return this;
    }

    protected Collider _collider = null;

    public bool Enabled
    {
        get { return _collider != null ? _collider.enabled : false; }
        set { if (_collider != null) _collider.enabled = value; }
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
}
