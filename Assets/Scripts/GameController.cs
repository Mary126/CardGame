using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private List<GameObject> _cards;
    public GameObject cardBlank;
    private Stack<GameObject> _cardStack;
    private GameObject _firstCard;
    private GameObject _secondCard;
    public GameObject CardsObjects;
    public string FileResourceName;
    public int EnemyHealth;
    public TMPro.TextMeshProUGUI EnemyHealthText;
    public int PointsToDeduct;
    public TMPro.TextMeshProUGUI QuestionText;

    
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
        var dataset = Resources.Load<TextAsset>(FileResourceName);
        var dataLines = dataset.text.Split('\n');
        for (int i = 1; i < dataLines.Length - 1; i++)
        {
            var data = dataLines[i].Split(';');
            GameObject newCard = Instantiate(cardBlank);
            newCard.transform.SetParent(gameObject.transform);
            newCard.GetComponent<CardController>().QuestionText = data[0];
            newCard.GetComponent<CardController>().GameController = this;
            string correctAnswer = data[1];
            string incorrectAnswer = data[2];
            int number = Random.Range(1, 3);
            if (number == 1)
            {
                newCard.GetComponent<CardController>().RightAnswerObject.GetComponent<TMPro.TextMeshProUGUI>().text = correctAnswer;
                newCard.GetComponent<CardController>().LeftAnswerObject.GetComponent<TMPro.TextMeshProUGUI>().text = incorrectAnswer;
                newCard.GetComponent<CardController>().RightAnswerIsCorrect = true;
            }
            else if (number == 2)
            {
                newCard.GetComponent<CardController>().RightAnswerObject.GetComponent<TMPro.TextMeshProUGUI>().text = incorrectAnswer;
                newCard.GetComponent<CardController>().LeftAnswerObject.GetComponent<TMPro.TextMeshProUGUI>().text = correctAnswer;
                newCard.GetComponent<CardController>().RightAnswerIsCorrect = false;
            }
            newCard.SetActive(false);
            _cards.Add(newCard);
        }
    }
    private GameObject GenerateNewCard(float positionZ)
    {
        GameObject newCard = _cardStack.Pop();
        newCard.SetActive(true);
        newCard.transform.SetParent(CardsObjects.transform);
        newCard.transform.position = new Vector3(0, 0, positionZ);
        newCard.name = "Card " + _cardStack.Count;
        newCard.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        return newCard;
    }
    public void AnswerCorrect()
    {
        EnemyHealth -= PointsToDeduct;
        EnemyHealthText.text = "EnemyHealth: " + EnemyHealth.ToString();
    }
    public void DestroyTopCard()
    {
        Destroy(_firstCard);
        if (_secondCard == null)
        {
            QuestionText.text = "Game end";
        }
        else
        {
            _firstCard = _secondCard;
            _firstCard.transform.position = new Vector3(0, 0, 0);
            QuestionText.text = _firstCard.GetComponent<CardController>().QuestionText;
            if (_cardStack.Count > 0)
            {
                _secondCard = GenerateNewCard(1);
                _firstCard.transform.SetAsLastSibling();
            }
            else
            {
                Debug.Log("End of stack");
                _secondCard = null;
            }
        }
    }
    public void Awake()
    {
        _cards = new List<GameObject>();
        parseCSV();
        ShuffleList(_cards);
        _cardStack = new Stack<GameObject>(_cards);
        _secondCard = GenerateNewCard(1);
        _firstCard = GenerateNewCard(0);
        QuestionText.text = _firstCard.GetComponent<CardController>().QuestionText;
        EnemyHealthText.text = "Enemy Health: " + EnemyHealth.ToString();
    }
}
