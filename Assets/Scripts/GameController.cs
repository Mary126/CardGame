using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<GameObject> cards;
    private Stack<GameObject> cardStack;
    private GameObject firstCard;
    private GameObject secondCard;
    public GameObject cardsObjects;
    public int enemyHealth;
    public TMPro.TextMeshProUGUI enemyHealthText;
    public int pointsToDeduct;
    public TMPro.TextMeshProUGUI questionText;

    private void ShuffleList(List<GameObject> inputList)
    {
        System.Random random = new System.Random();
        for (int i = inputList.Count - 1; i >= 1; i--)
        {
            int j = random.Next(i + 1);
            GameObject temp = inputList[j];
            inputList[j] = inputList[i];
            inputList[i] = temp;
        }
    }
    private GameObject GenerateNewCard(float positionZ)
    {
        GameObject newCard = Instantiate(cardStack.Pop());
        newCard.transform.SetParent(cardsObjects.transform);
        newCard.transform.position = new Vector3(0, 0, positionZ);
        newCard.name = "Card " + cardStack.Count;
        newCard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        return newCard;
    }
    public void AnswerCorrect()
    {
        enemyHealth -= pointsToDeduct;
        enemyHealthText.text = "EnemyHealth: " + enemyHealth.ToString();
    }
    public void DestroyTopCard()
    {
        Destroy(firstCard);
        if (secondCard == null)
        {
            questionText.text = "Game end";
        }
        else
        {
            firstCard = secondCard;
            firstCard.transform.position = new Vector3(0, 0, 0);
            questionText.text = firstCard.GetComponent<CardController>().questionText;
            if (cardStack.Count > 0)
            {
                secondCard = GenerateNewCard(1);
                firstCard.transform.SetAsLastSibling();
            }
            else
            {
                Debug.Log("End of stack");
                secondCard = null;
            }
        }
    }
    public void Awake()
    {
        ShuffleList(cards);
        cardStack = new Stack<GameObject>(cards);
        secondCard = GenerateNewCard(1);
        firstCard = GenerateNewCard(0);
        questionText.text = firstCard.GetComponent<CardController>().questionText;
        enemyHealthText.text = "Enemy Health: " + enemyHealth.ToString();
    }
}
