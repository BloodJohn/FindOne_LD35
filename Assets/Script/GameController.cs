using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Text countText;
    public Text levelText;
    public Text timerText;

    public Sprite[] snailList;
    public Sprite[] mushroomList;
    public AudioClip[] soundList;
    public string[] levelNameList;
    public Color[] colorList;

    private bool gameOver = false;
    private int count = 0;
    private int countTotal = 0;
    public int countFail = 0;
    private int countWhite = 0;
    private float timer;
    private float timerPrev;
    private int level = 1;
    private bool restartingLevel = false;

    public static GameController instance;
    private List<CardController> cardList = new List<CardController>(10);
    private const float timerTotal = 120f;

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

        if (countFail < 2 && timerTotal > timer)
        {
            timer += Time.deltaTime;
            if (timer - timerPrev >= 0.1f)
            {
                if (timerTotal - timer > 99f)
                    timerText.text = string.Format("{0:###}", timerTotal - timer);
                else if (timerTotal - timer > 9f)
                    timerText.text = string.Format("{0:##.#}", timerTotal - timer);
                else
                    timerText.text = string.Format("{0:0.##}", timerTotal - timer);
                timerPrev = timer;
            }
        }
        else if (countFail < 2)
        {
            timer = timerTotal;
            countFail = 2;

            foreach (var card in cardList)
                card.HidePoison();

            GameOver();
        }

        //if (Input.GetMouseButtonDown(0))
        if (Input.GetMouseButton(0))
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
                    countTotal++;
                    countText.text = countTotal.ToString();
                    clickCard.sound.PlayOneShot(soundList[0]);

                    if (count >= countWhite)
                    {
                        countText.text = "Delicious!";
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
                        foreach (var card in cardList)
                            if (card != clickCard)
                                card.HideAny();

                        GameOver();
                    }
                }
            }
        }
    }

    private void GameOver()
    {
        countText.text = string.Format("Total {0}", countTotal);

        timerPrev = timer;
        if (countTotal > 0)
            timerText.text = string.Format("{0:#0.#}/sec", countTotal / timerPrev);

        gameOver = true;
    }

    private IEnumerator RestartLevel(bool firstLevel)
    {
        restartingLevel = true;
        if (gameOver)
            yield return new WaitForSeconds(1f);
        else
            yield return new WaitForSeconds(0.5f);

        gameOver = false;
        count = 0;
        countWhite = 0;


        if (firstLevel)
        {
            level = 1;
            countTotal = 0;
            countFail = 0;
            timer = 0f;
            timerPrev = 0f;

            countText.text = "The Simon Snail";
            //timerText.text = String.Empty;
        }
        else
        {
            level++;
        }

        if (level <= levelNameList.Length)
        {
            levelText.text = levelNameList[level - 1];
        }
        else
        {
            levelText.text = string.Format("Level {0}", level);
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
            if (level < 15) return 4;
            if (level < 20) return 5;
            if (level < 25) return 6;
            if (level < 30) return 7;
            if (level < 35) return 8;
            if (level < 40) return 9;
            return mushroomList.Length;
        }
    }
}
