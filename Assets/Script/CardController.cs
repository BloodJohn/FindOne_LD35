using UnityEditor;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public BoxCollider2D collider;
    public GameObject sprite;
    /// <summary>Тип гриба (0,1 - хорошие, 2,3 - плохие)</summary>
    private int typeImage;
    /// <summary>Уже сыгравшая карта (вбрана)</summary>
    public bool isSelected;
    private Color color;

    public bool IsGood { get { return typeImage < 2; } }

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        isSelected = false;
    }

    // Use this for initialization
    void Start()
    {
        Restart();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnClick()
    {
        isSelected = true;
        UpdateStats();
    }

    public void Restart()
    {
        typeImage = Random.Range(0, GameController.instance.mushroomList.Length);
        UpdateStats();
    }

    public void UpdateStats()
    {
        Debug.Assert(sprite!=null, "Sprite in null!");
        if (sprite == null) return;

        //sprite.transform.localEulerAngles =  new Vector3(0f, 0f, isSelected ? 180f : 0f);

        if (isSelected)
        {
            if (IsGood)
            {
                SetSprite(GameController.instance.snailList[0]);
            }
            else
            {
                SetSprite(GameController.instance.snailList[1]);
            }
        }
        else
        {
            SetSprite(GameController.instance.mushroomList[typeImage]);
        }
    }

    public void SetColor()
    {
        const int maxColor = 9;
        int rndColor = Random.Range(0, maxColor);

        color = Color.white;

        switch (rndColor)
        {
            case 1: color = Color.red; break;
            case 2: color = Color.green; break;
            case 3: color = Color.blue; break;
            case 4: color = Color.yellow; break;
            case 5: color = Color.cyan; break;
            case 6: color = Color.magenta; break;
            case 7: color = Color.grey; break;
            case 8: color = Color.gray; break;
            default: color = Color.white; break;
        }
        
        sprite.GetComponent<SpriteRenderer>().material.color = color;
    }

    private void SetSprite(Sprite newImage)
    {
        sprite.GetComponent<SpriteRenderer>().sprite = newImage;
        sprite.GetComponent<SpriteRenderer>().material.color = Color.white;
    }
}
