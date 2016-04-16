using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    private List<CardController> cardList = new List<CardController>(10);

    // Use this for initialization
    void Start()
    {
        CardController[] allCard = FindObjectsOfType(typeof(CardController)) as CardController[];
        foreach (CardController card in allCard)
        {
            cardList.Add(card);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);
            if (hitCollider != null)
            {
                foreach (var card in cardList)
                    if (hitCollider == card.collider) card.OnClick();
            }
        }
    }
}
