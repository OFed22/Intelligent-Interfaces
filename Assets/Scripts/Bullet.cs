using UnityEngine;

public class Bullet : MonoBehaviour
{
    void Update()
    {
        if (!IsVisibleFrom(Camera.main, GetComponent<Renderer>(), 1.5f))
        {
            Destroy(gameObject);
        }
    }

    // Check if a renderer is visible from a camera with a certain leeway
    bool IsVisibleFrom(Camera camera, Renderer renderer, float leeway)
    {
        // Get the viewport position of the renderer bounds
        Vector3 viewportPos = camera.WorldToViewportPoint(renderer.bounds.center);

        // Check if the renderer is outside the camera viewport with leeway
        bool isVisible = viewportPos.x >= -leeway && viewportPos.x <= 1 + leeway &&
                          viewportPos.y >= -leeway && viewportPos.y <= 1 + leeway;

        return isVisible;
    }
}
