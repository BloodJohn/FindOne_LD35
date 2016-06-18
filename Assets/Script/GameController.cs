using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SmartLocalization;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;

public class GameController : MonoBehaviour
{
    public Camera mainCamera;
    public Text countText;
    public Text levelText;
    public Text timerText;

    public VictoryController victoryPanel;

    public Sprite[] snailList;
    public Sprite[] mushroomList;
    public AudioClip[] soundList;
    public string[] levelNameList;
    public Color[] colorList;

    #region private variables
    /// <summary>Съедено хороших грибов на экране</summary>
    private int count = 0;
    /// <summary>Всего съедено хороших грибов</summary>
    private int countTotal = 0;
    /// <summary>Съедено ядовитых грибов</summary>
    public int countFail = 0;
    /// <summary>Всего хороших грибов</summary>
    private int countWhite = 0;
    /// <summary>текущее время игры</summary>
    private float timer;
    /// <summary>последнее обновление таймера</summary>
    private float timerPrev;
    private int level = 1;
    /// <summary>Режим перезагрузки уровня</summary>
    private bool restartingLevel = false;
    /// <summary>Грибов съедено одним махом</summary>
    private int comboCount = 0;
    /// <summary>индекс последнего съеденного гриба</summary>
    private int lastClick = 0;

    public static GameController instance;
    private List<CardController> cardList = new List<CardController>(10);
    private const float timerTotal = 90f; //90
    #endregion

    #region Unity
    void Awake()
    {
        instance = this;
        LanguageManager.Instance.ChangeLanguage("ru");
    }

    // Use this for initialization
    void Start()
    {
        var allCard = FindObjectsOfType(typeof(CardController)) as CardController[];
        foreach (var card in allCard)
            cardList.Add(card);

        countText.text = LanguageManager.Instance.GetTextValue("TheSimonSnail");

        StartCoroutine(RestartLevel());
    }

    // Update is called once per frame
    void Update()
    {
        if (restartingLevel) return;

        UpdateTimer();

        if (Input.GetMouseButtonUp(0))
        {
            FinishCombo();
        }
        else if (Input.GetMouseButton(0))
        {
            CheckMousePos();
        }
    }
    #endregion

    private void UpdateTimer()
    {
        if (countFail >= 2) return; //улитка умерла

        if (timerTotal > timer) //время еще есть
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
        else //время закончилось
        {
            timer = timerTotal;
            countFail = 2;

            foreach (var card in cardList)
                card.HideAny(); //HidePoison

            StartCoroutine(GameOver(true));
        }
    }

    private void CheckMousePos()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var hitCollider = Physics2D.OverlapPoint(mousePosition);
        if (hitCollider == null) return;
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
            comboCount++;
            countTotal++;
            countText.text = countTotal.ToString();
            clickCard.sound.PlayOneShot(soundList[0]);
            clickCard.transform.localScale = Vector3.one * (1.05f - 0.05f * comboCount);

            if (count >= countWhite) //все хорошие грибы съедены (след. уровень/экран!)
            {
                countText.text = "Delicious!";
                StartCoroutine(RestartLevel());
            }
        }
        else
        {
            countFail++;
            clickCard.sound.PlayOneShot(soundList[1]);
            FinishCombo();

            if (countFail > 1)
            {
                clickCard.DrawDeath();
                lastClick = clickCard.typeImage;
                foreach (var card in cardList)
                    if (card != clickCard)
                        card.HideAny();

                StartCoroutine(GameOver(false));
            }
        }
    }

    private void FinishCombo()
    {
        comboCount = 0;
    }

    private IEnumerator GameOver(bool isFinishTime)
    {
        countText.text = string.Format("Total {0}", countTotal);

        timerPrev = timer;
        if (countTotal > 0)
        {
            timerText.text = string.Format("{0:#0.#}/sec", countTotal/timerPrev);
            victoryPanel.SetRate(countTotal/timerPrev);
        }
        //else totalRate.text = "Tap only edible mushrooms";

        Analytics.CustomEvent("game over", new Dictionary<string, object>
          {
            { "total", countTotal },
            { "fail", countFail },
            { "level", level },
            { "time", timerPrev },
            { "rate", countTotal / timerPrev }
          });

        restartingLevel = true;
        level = 0;
        countTotal = 0;
        countFail = 0;
        timer = 0f;
        timerPrev = 0f;

        //ждем пока отпустит палец от экрана
        while (Input.GetMouseButton(0)) yield return new WaitForSeconds(0.1f);

        if (isFinishTime)
        {
            ShowVictory();
            yield break;
        }

        //держим паузу для приличия        
        //yield return new WaitForSeconds(2f);
        //ждем пока нажмет еще разок
        while (!Input.GetMouseButton(0)) yield return null;

        countText.text = LanguageManager.Instance.GetTextValue("TheSimonSnail");

        yield return RestartLevel();
    }

    private IEnumerator RestartLevel()
    {
        restartingLevel = true;
        //ждем пока отпустит палец от экрана
        while (Input.GetMouseButton(0)) yield return new WaitForSeconds(0.1f);
        //держим паузу для приличия        
        //yield return new WaitForSeconds(0.5f);

        count = 0;
        countWhite = 0;
        FinishCombo();
        level++;

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

    private void ShowVictory()
    {
        restartingLevel = true;
        foreach (var card in cardList)
            card.HideAny();

        victoryPanel.ShowVictory();
    }

    public void HideVictory()
    {
        StartCoroutine(RestartLevel());
    }
}
