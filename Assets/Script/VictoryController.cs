using SmartLocalization;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

/// <summary>Контроллер для панельки с итогами игры</summary>
public class VictoryController : MonoBehaviour
{
    public GameController game;
    public GameObject gamePanel;
    public Text finalRate;
    public Text countDownText;
    public Text winText;
    public Text adsText;
    public Text restartText;
    public Button startGame;
    public Button startAds;

    /// <summary>индекс последнего съеденного гриба</summary>
    public int lastClick = 0;
    /// <summary>Обратный отсчет до игры без рекламы</summary>
    private float countDownTimer;
    private float oldTimer;

    private const float delayStart = 90f;//90f


    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();

        if (countDownTimer > 0)
        {
            countDownTimer -= Time.deltaTime;

            if (Mathf.Round(countDownTimer) != oldTimer)
            {
                oldTimer = Mathf.Round(countDownTimer);
                countDownText.text = string.Format(LanguageManager.Instance.GetTextValue("WaitSec"), oldTimer);
            }
        }
        else
        {
            startGame.gameObject.SetActive(true);
            startAds.gameObject.SetActive(false);
            countDownText.gameObject.SetActive(false);
        }
    }

    public void SetRate(float rate)
    {
        finalRate.text = string.Format(LanguageManager.Instance.GetTextValue("RateSec"), rate);
    }

    public void ShowVictory()
    {
        winText.text = LanguageManager.Instance.GetTextValue("YouWin");
        adsText.text = LanguageManager.Instance.GetTextValue("AdsButton");
        restartText.text = LanguageManager.Instance.GetTextValue("RestartButton");

        gamePanel.SetActive(false);
        gameObject.SetActive(true);
        startGame.gameObject.SetActive(false);
        startAds.gameObject.SetActive(true);

        countDownTimer = delayStart;
        countDownText.gameObject.SetActive(true);
    }

    /// <summary>Вызывается по кнопке startGame</summary>
    public void BackToGame()
    {
        gamePanel.SetActive(true);
        game.HideVictory();
        gameObject.SetActive(false);

    }

    /// <summary>Вызывается по кнопке startAds</summary>
    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            startGame.gameObject.SetActive(false);
            startAds.gameObject.SetActive(false);
            countDownTimer = 0f;
            countDownText.gameObject.SetActive(false);

            var showOptions = new ShowOptions
            {
                resultCallback = HandleShowResult
            };
            Advertisement.Show(null, showOptions);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                startGame.gameObject.SetActive(true);
                break;

            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                startGame.gameObject.SetActive(true);
                break;

            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
}
