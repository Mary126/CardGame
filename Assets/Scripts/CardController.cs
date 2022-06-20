using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public string QuestionText;
    public string LeftAnswerText;
    public string RightAnswerText;
    private GameObject _leftAnswerObject;
    private GameObject _rightAnswerObject;
    public bool RightAnswerIsCorrect;
    private Vector3 _initialPosition;
    public GameController GameController;
    private bool _swipedRight;
    private bool _isMoving;

    private void Awake()
    {
        _leftAnswerObject = transform.Find("Left Answer Text").gameObject;
        _rightAnswerObject = transform.Find("Right Answer Text").gameObject;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if ((transform.localPosition.x < _initialPosition.x + 150 || (Camera.main.ScreenToWorldPoint(Input.mousePosition).x + 150) < _initialPosition.x + 150) &&
            (transform.localPosition.x > _initialPosition.x - 150 || (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - 150) > _initialPosition.x - 150))
        {
            transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x, transform.localPosition.y);
            if (transform.localPosition.x - _initialPosition.x > 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, -30, (_initialPosition.x + transform.localPosition.x)/(Screen.width / 2)));
                _rightAnswerObject.SetActive(true);
                _leftAnswerObject.SetActive(false);
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(0, 30, (_initialPosition.x - transform.localPosition.x) / (Screen.width / 2)));
                _leftAnswerObject.SetActive(true);
                _rightAnswerObject.SetActive(false);
            }
            if (transform.localPosition.x > _initialPosition.x - 90 && transform.localPosition.x < _initialPosition.x + 90)
            {
                _leftAnswerObject.SetActive(false);
                _rightAnswerObject.SetActive(false);
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _initialPosition = transform.localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (transform.localPosition.x < _initialPosition.x + 90 && transform.localPosition.x > _initialPosition.x - 90)
        {

            StartCoroutine(MoveCardToStart());
            transform.rotation = Quaternion.identity;
            _leftAnswerObject.SetActive(false);
            _rightAnswerObject.SetActive(false);
        }
        else
        {
            if (transform.localPosition.x > _initialPosition.x)
            {
                if (RightAnswerIsCorrect)
                {
                    GameController.AnswerCorrect();
                }
                _swipedRight = true;
            }
            else
            {
                if (!RightAnswerIsCorrect)
                {
                    GameController.AnswerCorrect();
                }
                _swipedRight = false;
            }
            StartCoroutine(MovedCard());
        }
    }
    private IEnumerator MovedCard()
    {
        float time = 0;
        while (GetComponent<Image>().color != new Color(1, 1, 1, 0))
        {
            time += Time.deltaTime;
            if (!_swipedRight)
            {
                transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x,
                    transform.localPosition.x - 200, time), Mathf.SmoothStep(transform.localPosition.y, transform.localPosition.y - Screen.height, time), 0);
            }
            else
            {
                transform.localPosition = new Vector3(Mathf.SmoothStep(transform.localPosition.x,
                    transform.localPosition.x + 200, time), Mathf.SmoothStep(transform.localPosition.y, transform.localPosition.y - Screen.height, time), 0);
            }
            GetComponent<Image>().color = new Color(1, 1, 1, Mathf.SmoothStep(1, 0, 4 * time));
            yield return null;
        }
        GameController.DestroyTopCard();
    }
    private IEnumerator MoveCardToStart()
    {
        if (_isMoving)
        {
            yield break; ///exit if this is still running
        }
        _isMoving = true;

        float counter = 0;

        //Get the current position of the object to be moved
        Vector3 startPos = transform.localPosition;

        while (counter < 0.5f)
        {
            counter += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPos, _initialPosition, counter / 0.5f);
            yield return null;
        }

        _isMoving = false;

    }
}
