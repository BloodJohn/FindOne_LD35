using UnityEngine;
using System.Collections;
using SmartLocalization;
using UnityEngine.UI;

/// <summary>Контроллер для панельки с описанием грибочков</summary>
public class HelpController : MonoBehaviour
{
    public GameController game;
    public GameObject gamePanel;
    public Text[] mushTextList;

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            BackToGame();
        }
    }

    public void ShowHelp()
    {
        for (var i = 0; i < mushTextList.Length; i++)
        {
            mushTextList[i].text = LanguageManager.Instance.GetTextValue(string.Format("mushroom_{0}", i));
            //Debug.Log(mushTextList[i].text);
        }

        gamePanel.SetActive(false);
        gameObject.SetActive(true);
    }

    private void BackToGame()
    {
        gamePanel.SetActive(true);
        game.HideVictory();
        gameObject.SetActive(false);

    }
}
