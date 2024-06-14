using UnityEngine;

public class PlayerMovementA : MonoBehaviour
{
    public float speed = 10f;
    private float minX, maxX;
    private float halfPlayerWidth;

    void Start()
    {
        halfPlayerWidth = GetComponent<SpriteRenderer>().bounds.extents.x;
        minX = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x + halfPlayerWidth;
        maxX = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - halfPlayerWidth;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 newPosition = transform.position + Vector3.right * horizontal * speed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        transform.position = newPosition;
    }
}