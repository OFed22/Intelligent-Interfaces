using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float initialSpawnRate = 2f;
    public float maxSpawnRate = 10f;
    public float spawnRateIncrease = 0.005f;
    public float initialBulletSpeed = 1f;
    public float maxBulletSpeed = 5f;
    public float bulletSpeedIncrease = 0.005f;
    private float nextSpawnTime;
    private float timeSinceLastSpeedIncrease;

    void Start()
    {
        nextSpawnTime = Time.time + initialSpawnRate;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            initialSpawnRate = Mathf.Min(initialSpawnRate, maxSpawnRate);
            initialBulletSpeed = Mathf.Min(initialBulletSpeed, maxBulletSpeed);
            SpawnBulletRadially();
            nextSpawnTime = Time.time + 5f / initialSpawnRate;
            initialSpawnRate += spawnRateIncrease;
        }


        // Update bullet speed
        timeSinceLastSpeedIncrease += Time.deltaTime;
        if (timeSinceLastSpeedIncrease >= 5f)
        {   
            initialBulletSpeed += bulletSpeedIncrease;
            timeSinceLastSpeedIncrease = 0f;
        }
    }

    void SpawnBulletRadially(int numBullets=12)
    {
        float angleStep = 360f / numBullets;
        float startAngle = Random.Range(0f, 360f);

        for (int i = 0; i < numBullets; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 spawnDirection = Quaternion.Euler(0, 0, angle) * Vector3.down;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Rotate the bullet prefab to face its movement direction
            float bulletAngle = -90 + Mathf.Atan2(spawnDirection.y, spawnDirection.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(bulletAngle, Vector3.forward);

            bullet.GetComponent<Rigidbody2D>().velocity = spawnDirection.normalized * initialBulletSpeed;
        }
    }
}