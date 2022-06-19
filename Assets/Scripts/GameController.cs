using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private List<GameObject> cards;
    public GameObject cardBlank;
    private Stack<GameObject> cardStack;
    private GameObject firstCard;
    private GameObject secondCard;
    public GameObject cardsObjects;
    public string fileResourceName;
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
    private void parseCSV()
    {
        var dataset = Resources.Load<TextAsset>(fileResourceName);
        var dataLines = dataset.text.Split('\n');
        for (int i = 1; i < dataLines.Length - 1; i++)
        {
            var data = dataLines[i].Split(';');
            GameObject newCard = Instantiate(cardBlank, Vector3.zero, Quaternion.identity);
            newCard.GetComponent<CardController>().questionText = data[0];
            string correctAnswer = data[1];
            string incorrectAnswer = data[2];
            int number = Random.Range(1, 3);
            if (number == 1)
            {
                newCard.GetComponent<CardController>().rightAnswerText = correctAnswer;
                newCard.GetComponent<CardController>().leftAnswerText = incorrectAnswer;
                newCard.GetComponent<CardController>().rightAnswerIsCorrect = true;
            }
            else if (number == 2)
            {
                newCard.GetComponent<CardController>().rightAnswerText = incorrectAnswer;
                newCard.GetComponent<CardController>().leftAnswerText = correctAnswer;
                newCard.GetComponent<CardController>().rightAnswerIsCorrect = false;
            }
            cards.Add(newCard);
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
        cards = new List<GameObject>();
        parseCSV();
        ShuffleList(cards);
        cardStack = new Stack<GameObject>(cards);
        secondCard = GenerateNewCard(1);
        firstCard = GenerateNewCard(0);
        questionText.text = firstCard.GetComponent<CardController>().questionText;
        enemyHealthText.text = "Enemy Health: " + enemyHealth.ToString();
    }
}
