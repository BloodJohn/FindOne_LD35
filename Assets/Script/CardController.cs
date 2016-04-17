using UnityEngine;

public class CardController : MonoBehaviour
{
    public BoxCollider2D collider;
    public GameObject sprite;
    public AudioSource sound;
    /// <summary>Тип гриба (0,1 - хорошие, 2,3 - плохие)</summary>
    private int typeImage;
    /// <summary>Уже сыгравшая карта (вбрана)</summary>
    public bool isSelected;
    private Color color;

    public bool IsGood { get { return 0 == typeImage % 2; } }

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        sound = GetComponent<AudioSource>();
        isSelected = false;
    }

    // Use this for initialization
    void Start()
    {
        //Restart();
    }

    public void OnClick()
    {
        isSelected = true;
        UpdateStats();
    }

    public void Restart()
    {
        typeImage = Random.Range(0, GameController.instance.MushroomMax);
        UpdateStats();
    }

    private void UpdateStats()
    {
        Debug.Assert(sprite != null, "Sprite in null!");
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
        var color = Color.white;

        if (GameController.instance.colorList.Length > 0)
        {
            var rndIndex = Random.Range(0, GameController.instance.colorList.Length);
            color = GameController.instance.colorList[rndIndex];
        }

        sprite.GetComponent<SpriteRenderer>().material.color = color;
    }

    internal void DrawDeath()
    {
        SetSprite(GameController.instance.snailList[2]);
    }

    private void SetSprite(Sprite newImage)
    {
        sprite.GetComponent<SpriteRenderer>().sprite = newImage;
        
        if (GameController.instance.countFail > 0)
            SetColor();
        else
            sprite.GetComponent<SpriteRenderer>().material.color = Color.white;
    }
}
