using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class Card : MonoBehaviour
{
    [SerializeField] private Sprite[] _cardSprites;
    [SerializeField] private SpriteRenderer _frontSpriteRenderer;

    [SerializeField] private SortingGroup _sortingGroup;
    public SortingGroup SortingGroup => _sortingGroup;

    [Header("Flip Animation Config")]
    [SerializeField] private float _minTilt = 10f;
    [SerializeField] private float _maxTilt = 30f;
    //[SerializeField] private float _height = 1f;
    [SerializeField] private float _flipDuration = 1f;
    public float FlipDuration => _flipDuration;
    [SerializeField] private WorldOffset _shadowOffset;

    private void Start()
    {
        _frontSpriteRenderer.sprite = _cardSprites[Random.Range(0, _cardSprites.Length)];
    }

    public Sequence Flip(Vector3 targetPosition)
    {
        DOTween.Kill(this, complete: true);

        var isFaceDown = Vector3.Dot(Camera.main.transform.forward, transform.forward) < 0;
        var tiltDirection = isFaceDown ? 1 : -1f;
        if (targetPosition.y > transform.position.y) tiltDirection *= -1;
        var rotateDirection = targetPosition.x >= transform.position.x ? -1 : 1;
        var moveDirection = (targetPosition - transform.position).normalized;
        var tiltAmount = Mathf.Lerp(_minTilt, _maxTilt, Mathf.Abs(Vector3.Dot(moveDirection, Vector3.up)));

        var sequence = DOTween.Sequence();

        var xRot = 0f;
        var yRot = transform.localEulerAngles.y;
        var height = 0f;

        sequence.Append(DOTween.To(() => xRot, x => xRot = x, tiltAmount * tiltDirection, .5f * _flipDuration)
            .SetEase(Ease.Linear));
        sequence.Append(DOTween.To(() => xRot, x => xRot = x, 0, .5f * _flipDuration)
            .SetEase(Ease.Linear));
        sequence.Insert(0, DOTween.To(() => yRot, y => yRot = y, 180 * rotateDirection, 1f * _flipDuration)
            .SetRelative()
            .SetEase(Ease.InOutSine));
        sequence.Insert(0, transform.DOMove(targetPosition, 1f * _flipDuration)
            .SetEase(Ease.InOutQuad));
        //sequence.Insert(0, transform.DOJump(targetPosition, _height, 1, 1f));
        sequence.Insert(0, DOTween.To(() => height, h => height = h, 1, .5f * _flipDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(2, LoopType.Yoyo));

        sequence.OnUpdate(() =>
        {
            transform.eulerAngles = new Vector3(xRot, yRot, 0);
            var offsetStrength = Mathf.Lerp(0, .5f, height);
            _shadowOffset.Offset = new Vector2(offsetStrength, -offsetStrength);
            /*transform.localPosition = new Vector3(
                transform.localPosition.x, 
                height * _height, 
                transform.localPosition.z);*/
        });

        sequence.SetId(this);

        return sequence;
     
    }
}
