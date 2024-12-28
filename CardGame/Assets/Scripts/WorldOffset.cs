using UnityEngine;

public class WorldOffset : MonoBehaviour
{
    [SerializeField] private Transform _target;
    public Vector2 Offset;

    private void LateUpdate()
    {
        if (_target == null) return;

        transform.position = _target.position + (Vector3)Offset;
    }
}
