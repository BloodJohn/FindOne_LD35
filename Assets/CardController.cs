using UnityEngine;

public class CardController : MonoBehaviour
{
    private BoxCollider2D collider;
    public GameObject sprite;
    public bool upOrientation;

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    // Use this for initialization
    void Start()
    {
        UpdateStats();
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
                Debug.Log("I'm hitting " + hitCollider.name);

                if (hitCollider == collider) OnClick();
            }
        }
    }

    public void OnClick()
    {
        Debug.Log("Cick!");

        upOrientation = !upOrientation;
        UpdateStats();
    }

    public void UpdateStats()
    {
        Debug.Assert(sprite!=null, "Sprite in null!");

        sprite.transform.localEulerAngles =  new Vector3(0f, 0f, upOrientation ? 0f : 180f);
    }
}
