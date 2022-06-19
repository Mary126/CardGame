using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        leftAnswerObject = transform.Find("Left Answer Text").gameObject;
        rightAnswerObject = transform.Find("Right Answer Text").gameObject;
        leftAnswerObject.GetComponent<TextMeshProUGUI>().text = leftAnswerText;
        rightAnswerObject.GetComponent<TextMeshProUGUI>().text = rightAnswerText;
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = new Vector2(transform.localPosition.x + eventData.delta.x, transform.localPosition.y);
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
    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = transform.localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        distanceMoved = Mathf.Abs(transform.localPosition.x - initialPosition.x);
        if (distanceMoved < 0.2 * Screen.width)
        {
            transform.localPosition = initialPosition;
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
            }
            else
            {
                if (!rightAnswerIsCorrect)
                {
                    gameController.AnswerCorrect();
                }
            }
            gameController.DestroyTopCard();
        }
    }
}
