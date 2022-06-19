using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public string questionText;
    public string leftAnswerText;
    public string rightAnswerText;
    private GameObject leftAnswerObject;
    private GameObject rightAnswerObject;
    public bool rightAnswerIsCorrect;
    private Vector3 initialPosition;
    private float distanceMoved;
    private GameController gameController;
    private bool swipedRight;

    private void Awake()
    {
        leftAnswerObject = transform.Find("Left Answer Text").gameObject;
        rightAnswerObject = transform.Find("Right Answer Text").gameObject;
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (transform.localPosition.x <= 100 && transform.localPosition.x >= -100)
        {
            transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x, transform.localPosition.y);
            transform.rotation = Quaternion.Euler(0, 0, -transform.localPosition.x / 10);
            if (transform.localPosition.x > 0)
            {
                rightAnswerObject.SetActive(true);
                leftAnswerObject.SetActive(false);
            }
            else if (transform.localPosition.x < 0)
            {
                leftAnswerObject.SetActive(true);
                rightAnswerObject.SetActive(false);
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = transform.localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        distanceMoved = Mathf.Abs(transform.localPosition.x - initialPosition.x);
        if (distanceMoved < 0.01 * Screen.width)
        {
            transform.localPosition = initialPosition;
            transform.rotation = Quaternion.identity;
            leftAnswerObject.SetActive(false);
            rightAnswerObject.SetActive(false);
        }
        else
        {
            if (transform.localPosition.x > initialPosition.x)
            {
                if (rightAnswerIsCorrect)
                {
                    gameController.AnswerCorrect();
                }
                swipedRight = true;
            }
            else
            {
                if (!rightAnswerIsCorrect)
                {
                    gameController.AnswerCorrect();
                }
                swipedRight = false;
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
            if (!swipedRight)
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
        gameController.DestroyTopCard();
    }
}
