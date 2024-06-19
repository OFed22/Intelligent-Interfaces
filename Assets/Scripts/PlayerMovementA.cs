using UnityEngine;

public class PlayerMovementA : MonoBehaviour
{
    public float speed = 10f;
    public bool fullMovement = true;

    private float minX, maxX, minY, maxY;
    private float halfPlayerWidth, halfPlayerHeight;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        halfPlayerWidth = sr.bounds.extents.x;
        halfPlayerHeight = sr.bounds.extents.y;

        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        minX = bottomLeft.x + halfPlayerWidth;
        maxX = topRight.x - halfPlayerWidth;

        minY = bottomLeft.y + halfPlayerHeight;
        maxY = topRight.y - halfPlayerHeight;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 newPosition = transform.position;

        if (fullMovement)
        {
            newPosition += new Vector3(horizontal, vertical, 0) * speed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        }
        else
        {
            newPosition += Vector3.right * horizontal * speed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        }

        transform.position = newPosition;
    }
}
