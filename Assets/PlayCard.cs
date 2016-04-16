using UnityEngine;

public class PlayCard : MonoBehaviour
{
    private BoxCollider2D collider;

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    // Use this for initialization
    void Start()
    {

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
    }
}
