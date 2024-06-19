using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject spawnerPrefab; // Prefab of the spawner to instantiate
    public float initialSpawnRate = 2f;
    public float maxSpawnRate = 10f;
    public float spawnRateIncrease = 0.005f;
    public float initialBulletSpeed = 1f;
    public float maxBulletSpeed = 5f;
    public float bulletSpeedIncrease = 0.005f;
    public float spawnerCreationInterval = 30f;
    private float nextSpawnTime;
    private float timeSinceLastSpeedIncrease;
    private float timeSinceLastSpawnerCreation;
    private static Vector3 lastSpawnerPosition;
    private bool canCreateSpawner = true;
    private static bool isFirstSpawner = true;
    private static float currentOffset;
    private static bool useExtraOffset;
    private static int spawnerCount = 0;

    void Start()
    {
        nextSpawnTime = Time.time + initialSpawnRate;
        timeSinceLastSpawnerCreation = 0f;

        if (isFirstSpawner)
        {
            lastSpawnerPosition = transform.position;
            currentOffset = Camera.main.orthographicSize * Camera.main.aspect / 2; // 1/4 screen width
            isFirstSpawner = false;
            useExtraOffset = false;
        }
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            initialSpawnRate = Mathf.Min(initialSpawnRate, maxSpawnRate);
            initialBulletSpeed = Mathf.Min(initialBulletSpeed, maxBulletSpeed);
            SpawnRandomBulletPattern();
            nextSpawnTime = Time.time + 5f / initialSpawnRate;
            initialSpawnRate += spawnRateIncrease;
        }

        timeSinceLastSpeedIncrease += Time.deltaTime;
        if (timeSinceLastSpeedIncrease >= 5f)
        {
            initialBulletSpeed += bulletSpeedIncrease;
            timeSinceLastSpeedIncrease = 0f;
        }

        // Create new spawner instances
        if (canCreateSpawner)
        {
            timeSinceLastSpawnerCreation += Time.deltaTime;
            if (timeSinceLastSpawnerCreation >= spawnerCreationInterval)
            {
                CreateNewSpawner();
                canCreateSpawner = false; // Disable further spawner creation for this instance
                timeSinceLastSpawnerCreation = 0f;
            }
        }
    }

    void SpawnRandomBulletPattern()
    {
        int randomPattern = Random.Range(0, 4);

        switch (randomPattern)
        {
            case 0:
                SpawnBulletLine();
                break;
            case 1:
                SpawnBulletSpread();
                break;
            default:
                SpawnBulletRadially();
                break;
        }
    }

    void SpawnBulletRadially(int numBullets = 12)
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

    void SpawnBulletLine()
    {
        int numBullets = 4;
        float spacing = 1f; // Distance between bullets in the line
        float maxAngleOffset = 8f;

        Vector3 spawnPosition = transform.position;
        Vector3 direction = (PlayerPosition() - spawnPosition).normalized;

        // Randomly adjust the direction angle
        float randomAngleOffset = Random.Range(-maxAngleOffset, maxAngleOffset);
        Quaternion rotation = Quaternion.AngleAxis(randomAngleOffset, Vector3.forward);
        Vector3 adjustedDirection = rotation * direction;

        Vector3 startPosition = spawnPosition + adjustedDirection * (-numBullets * spacing);

        for (int i = 0; i < numBullets; i++)
        {
            Vector3 bulletPosition = startPosition + i * spacing * direction;
            GameObject bullet = Instantiate(bulletPrefab, bulletPosition, Quaternion.identity);

            // Rotate the bullet to face towards the player
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

            bullet.GetComponent<Rigidbody2D>().velocity = direction * initialBulletSpeed * 0.8f;
        }
    }

    void SpawnBulletSpread()
    {
        float spreadAngle = 3f; // Angle between bullets in the spread

        Vector3 spawnPosition = transform.position;
        Vector3 direction = (PlayerPosition() - spawnPosition).normalized;

        Vector3 centerDirection = direction;
        Vector3 leftDirection = Quaternion.Euler(0, 0, spreadAngle) * centerDirection;
        Vector3 rightDirection = Quaternion.Euler(0, 0, -spreadAngle) * centerDirection;
        Quaternion bulletDirection = Quaternion.LookRotation(Vector3.forward, centerDirection);

        GameObject bullet;

        // Center bullet
        bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.transform.rotation = bulletDirection;
        bullet.GetComponent<Rigidbody2D>().velocity = centerDirection * initialBulletSpeed;

        // Left and right bullets
        bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.transform.rotation = bulletDirection;
        bullet.GetComponent<Rigidbody2D>().velocity = leftDirection * initialBulletSpeed;

        bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.transform.rotation = bulletDirection;
        bullet.GetComponent<Rigidbody2D>().velocity = rightDirection * initialBulletSpeed;

        // Second layer of bullets
        bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.transform.rotation = bulletDirection;
        bullet.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, -2 * spreadAngle) * centerDirection * initialBulletSpeed;

        bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.transform.rotation = bulletDirection;
        bullet.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 2 * spreadAngle) * centerDirection * initialBulletSpeed;
    }


    void CreateNewSpawner()
    {
        Vector3 newSpawnerPosition;

        if (spawnerCount == 0)
        {
            // First spawner: 1/4 screen width apart from the original spawner
            newSpawnerPosition = new Vector3(transform.position.x + currentOffset, transform.position.y, transform.position.z);
        }
        else
        {
            // Alternate mirroring and mirroring with additional offset
            float variance = useExtraOffset ? currentOffset : 0;
            newSpawnerPosition = new Vector3(-lastSpawnerPosition.x + variance, lastSpawnerPosition.y, lastSpawnerPosition.z);
            useExtraOffset = !useExtraOffset; // Toggle the use of extra offset

            // After every two spawners, halve the currentOffset
            if (spawnerCount % 2 == 1)
            {
                currentOffset /= 2;
            }
        }

        GameObject newSpawner = Instantiate(spawnerPrefab, newSpawnerPosition, Quaternion.identity);

        newSpawner.GetComponent<EnemySpawner>().spawnerPrefab = this.spawnerPrefab;

        lastSpawnerPosition = newSpawner.transform.position;

        spawnerCount++;
    }

    Vector3 PlayerPosition()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.position;
    }
}
