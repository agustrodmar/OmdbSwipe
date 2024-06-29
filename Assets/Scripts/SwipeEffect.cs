using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Class for managing the swipe effect on a card.
/// </summary>
public class SwipeEffect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _initialPosition;
    private float _distanceMoved;
    private bool _swipeLeft;
    private Image _image;
    public ListController listController;
    public event Action CardMoved;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// Called when the card is being dragged.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        var transformLocal = transform;
        Vector3 localPosition = transformLocal.localPosition;
        localPosition.x += eventData.delta.x;
        transformLocal.localPosition = localPosition;

        var angle = localPosition.x - _initialPosition.x > 0 ? Mathf.LerpAngle(0, -30,
                (_initialPosition.x + localPosition.x) / (Screen.width / 2f)) :
            Mathf.LerpAngle(0, 30, (_initialPosition.x - localPosition.x) / (Screen.width / 2f));
        transformLocal.localEulerAngles = new Vector3(0, 0, angle);
    }
    
    /// <summary>
    /// Called when the user starts dragging the card.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        _initialPosition = transform.localPosition;
    }

    /// <summary>
    /// Called when the user stops dragging the card.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        _distanceMoved = Math.Abs(transform.localPosition.x - _initialPosition.x);
        if (_distanceMoved < 0.4 * Screen.width)
        {
            ResetPositionAndRotation();
        }
        else
        {
            _swipeLeft = transform.localPosition.x <= _initialPosition.x;
            MovieLoader movieLoader = GetComponent<MovieLoader>();
            if (movieLoader != null && movieLoader.movieData != null)
            {
                movieLoader.movieData.Liked = !_swipeLeft;
                if (listController != null)
                {
                    listController.AddToList(movieLoader.movieData.Liked, movieLoader.movieData.Title);
                }
            }

            CardMoved?.Invoke();
            StartCoroutine(MoveCard());
        }
    }
    
    /// <summary>
    /// Resets the position and rotation of the card.
    /// </summary>
    private void ResetPositionAndRotation()
    {
        var transformLocal = transform;
        // Reset the local position of the card to its initial position
        transformLocal.localPosition = _initialPosition;
        // Reset the local rotation of the card to zero
        transformLocal.localEulerAngles = Vector3.zero;
    }

    /// <summary>
    /// Moves the card off the screen after it has been swiped.
    /// </summary>
    private IEnumerator MoveCard()
    {
        float time = 0;
        Vector3 localPosition = transform.localPosition;
        while (_image.color.a > 0) 
        {
            time += Time.deltaTime;
            localPosition.x = Mathf.SmoothStep(localPosition.x, localPosition.x - Screen.width, 4 * time);
            transform.localPosition = localPosition;

            _image.color = new Color(1, 1, 1, Mathf.SmoothStep(1, 0, 4 * time));
            yield return null;
        }
        gameObject.SetActive(false);
    }
}