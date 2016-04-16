using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Text objectiveText;
    public Text countText;
    public Text levelText;

    public Sprite[] snailList;
    public Sprite[] mushroomList;


    private bool gameOver = false;
    private int count = 0;
    private int countWhite = 0;
    private int level = 1;

    public static GameController instance;
    private List<CardController> cardList = new List<CardController>(10);

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        CardController[] allCard = FindObjectsOfType(typeof(CardController)) as CardController[];
        foreach (CardController card in allCard)
            cardList.Add(card);

        RestartLevel(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (gameOver)
            {
                RestartLevel(true);
                return;
            }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
            if (hitCollider != null)
            {
                CardController clickCard = null;

                foreach (var card in cardList)
                    if (hitCollider == card.collider)
                    {
                        clickCard = card;
                        break;
                    }

                if (clickCard == null) return;
                if (clickCard.isSelected) return;

                clickCard.OnClick();

                if (clickCard.IsGood)
                {
                    count++;
                    countText.text = count.ToString();

                    if (count >= countWhite)
                    {
                        RestartLevel(false);
                    }
                }
                else
                {
                    GameOver();
                }
            }
        }
    }

    private void GameOver()
    {
        objectiveText.text = "Game Over!";
        countText.text = string.Format("Total {0}", count);

        foreach (var card in cardList)
            card.SetColor();

        gameOver = true;
    }

    private void RestartLevel(bool firstLevel)
    {
        gameOver = false;
        objectiveText.text = "Tap only edible\nmushrooms";
        count = 0;
        level = firstLevel ? 1 : (level + 1);
        levelText.text = string.Format("Level {0}", level);
        countWhite = 0;

        foreach (var card in cardList)
        {
            card.isSelected = false;
            card.Restart();
            if (card.IsGood) countWhite++;
        }


        if (firstLevel)
            countText.text = "Let's go!";
        else
            countText.text = "Next Level!";
    }
}
