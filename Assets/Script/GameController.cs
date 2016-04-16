using System.Collections;
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
    public AudioClip[] soundList;

    private bool gameOver = false;
    private int count = 0;

    public int countFail = 0;
    private int countWhite = 0;
    private int level = 1;
    private bool restartingLevel = false;

    public static GameController instance;
    private List<CardController> cardList = new List<CardController>(10);

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        var allCard = FindObjectsOfType(typeof(CardController)) as CardController[];
        foreach (var card in allCard)
            cardList.Add(card);

        StartCoroutine(RestartLevel(true));
    }

    // Update is called once per frame
    void Update()
    {
        if (restartingLevel) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (gameOver)
            {
                StartCoroutine(RestartLevel(true));
                return;
            }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hitCollider = Physics2D.OverlapPoint(mousePosition);
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
                    clickCard.sound.PlayOneShot(soundList[0]);

                    if (count >= countWhite)
                    {
                        StartCoroutine(RestartLevel(false));
                        return;
                    }
                }
                else
                {
                    countFail++;
                    clickCard.sound.PlayOneShot(soundList[1]);

                    if (countFail > 1)
                    {
                        clickCard.DrawDeath();
                        GameOver();
                    }
                }
            }
        }
    }

    private void GameOver()
    {
        objectiveText.text = "Game Over!";
        countText.text = string.Format("Total {0}", count);

        foreach (var card in cardList) card.SetColor();

        gameOver = true;
    }

    private IEnumerator RestartLevel(bool firstLevel)
    {
        restartingLevel = true;
        yield return new WaitForSeconds(0.5f);

        gameOver = false;
        objectiveText.text = "Tap only edible\nmushrooms";
        count = 0;
        levelText.text = string.Format("Level {0}", level);
        countWhite = 0;

        if (firstLevel)
        {
            level = 1;
            countFail = 0;
            countText.text = "Let's go!";
        }
        else
        {
            level++;
            countText.text = "Next Level!";
        }

        foreach (var card in cardList)
        {
            card.isSelected = false;
            card.Restart();
            if (card.IsGood) countWhite++;
        }

        restartingLevel = false;
    }

    public int MushroomMax
    {
        get
        {
            if (level < 5) return 2;
            if (level < 10) return 3;
            return mushroomList.Length;
        }
    }
}
