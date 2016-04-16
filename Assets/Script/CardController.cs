using UnityEngine;

public class CardController : MonoBehaviour
{
    public BoxCollider2D collider;
    public GameObject sprite;
    /// <summary>Уже сыгравшая карта (вбрана)</summary>
    public bool isGood;
    /// <summary>Уже сыгравшая карта (вбрана)</summary>
    public bool isSelected;
    private Color color;

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
        SetColor(8);
    }

    public void Restart()
    {
        SetColor(2);
        isGood = (color == Color.white);
        UpdateStats();
    }

    public void UpdateStats()
    {
        Debug.Assert(sprite!=null, "Sprite in null!");
        if (sprite == null) return;

        sprite.transform.localEulerAngles =  new Vector3(0f, 0f, isSelected ? 180f : 0f);
    }

    private void SetColor(int maxColor)
    {
        int rndColor = Random.Range(0, maxColor < 9 ? maxColor : 9);

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

        sprite.GetComponent<Renderer>().material.color = color;
    }
}
