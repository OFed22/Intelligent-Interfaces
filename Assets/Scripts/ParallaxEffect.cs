using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    public float scrollSpeed = 1.0f;
    private Transform[] backgrounds;
    private float backgroundHeight; 

    void Start()
    {
        backgrounds = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            backgrounds[i] = transform.GetChild(i);
        }

        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.size.y;

        backgrounds[0].localPosition = Vector3.zero;
        backgrounds[1].localPosition = new Vector3(0f, backgroundHeight, 0f);
    }

    void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].Translate(Vector2.down * scrollSpeed * Time.deltaTime);
        }

        if (backgrounds[0].position.y < -backgroundHeight)
        {
            backgrounds[0].localPosition = new Vector3(0f, backgrounds[1].localPosition.y + backgroundHeight, 0f);

            Transform temp = backgrounds[0];
            backgrounds[0] = backgrounds[1];
            backgrounds[1] = temp;
        }
    }
}
