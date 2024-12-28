using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDeck : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Card _cardPrefab;
    [SerializeField] private Transform[] _drawDestinations;

    private List<Card> _spawnedCards = new();
    private int _drawCount = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private Camera _mainCamera;
    private void Update()
    {

       if (!_mainCamera) _mainCamera = Camera.main;
        _mainCamera.transform.localPosition = new Vector3(
            0, 
            0, 
            Screen.orientation == ScreenOrientation.Portrait 
            || Screen.orientation == ScreenOrientation.PortraitUpsideDown ? -18 : -10);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DrawCard();
    }

    private void DrawCard()
    {
        //Spawn Card at deck Position
        var card = Instantiate(_cardPrefab, transform.position, Quaternion.Euler(0, -180, 0));
        _spawnedCards.Add(card);
        var sortingOrder = _spawnedCards.Count;

        //Animate Card to target position
        var destination = _drawDestinations[_drawCount % _drawDestinations.Length];
        var targetPosition = destination.position;
        //targetPosition.y += .1f * (_spawnedCards.Count - 1);
        //targetPosition.z -= .1f * (_spawnedCards.Count - 1);
        var sequence = card.Flip(targetPosition);
        DOVirtual.DelayedCall(card.FlipDuration/2, () =>
        {
            card.transform.SetParent(destination.transform, true);
            //card.SortingGroup.sortingOrder = sortingOrder;
        });
        sequence.OnComplete(() =>
        {
            foreach(Transform child in destination)
            {
                if (child == card.transform) break;
                Destroy(child.gameObject);
                _spawnedCards.Remove(_spawnedCards.Find(c => c.transform == child));
            }
        });

        _drawCount++;
    }
}
