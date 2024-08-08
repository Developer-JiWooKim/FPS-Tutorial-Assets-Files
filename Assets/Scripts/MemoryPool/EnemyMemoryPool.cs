using System.Collections;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform       target;                             // Enemy�� ��ǥ(Player ��...)

    [SerializeField]
    private GameObject      enemySpawnPointPrefab;              // Enemy ���� �ϱ� �� Enemy ���� ��ġ�� �˷��ִ� ������

    [SerializeField]
    private GameObject      enemyPrefab;                        // Enemy ������

    [SerializeField]
    private GameObject      EnemySpawnField;                    // Enemy�� ������ ����(Field)

    [SerializeField]
    private float           enemySpawnTime = 1;                 // Enemy ���� �ֱ�

    [SerializeField]
    private float           enemySpawnLatency = 1;              // Ÿ�� ���� �� Enemy �����ϱ� ���� ��� �ð�

    private MemoryPool      spawnPointMemoryPool;               // Enemy ���� ��ġ�� �˷��ִ� ������Ʈ ����, Ȱ�� / ��Ȱ�� ����
    private MemoryPool      enemyMemoryPool;                    // Enemy ����, Ȱ�� / ��Ȱ�� ����(Ǯ��)

    private Vector2Int      mapSize = new Vector2Int(30, 35);   // �� ũ��

    private int             numberOfEnemiesSpawnedAtOnce = 1;   // ���� �����Ǵ� Enemy ����
    private bool            isSpawn;                            // ���� ���� ����

    /// <summary>
    /// �� �����տ� ���� �޸� Ǯ Ŭ���� ����, ���� ���� ���� �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);
        isSpawn = false;
    }

    /// <summary>
    /// Enemy ������ �����ϱ� �� ������ ��ġ�� ���ϰ� Enemy�� ������Ű�� �ڷ�ƾ ȣ��
    /// </summary>
    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 50;

        while (true)
        {
            // ���ÿ� numberOfEnemiesSpawnedAtOnce ���� ��ŭ ���� �����ǵ��� �ݺ��� ���
            for (int i = 0; i < numberOfEnemiesSpawnedAtOnce; i++)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();

                // �� ���� �������� ��Ÿ�� ��ġ�� item�� ����
                item.transform.position = new Vector3(Random.Range(EnemySpawnField.transform.position.x - (mapSize.x / 2f), EnemySpawnField.transform.position.x + (mapSize.x / 2f)), 1f,
                                                      Random.Range(EnemySpawnField.transform.position.z - (mapSize.y / 2f), EnemySpawnField.transform.position.z + (mapSize.y / 2f)));
                if (isSpawn)
                {
                    StartCoroutine("SpawnEnemy", item);
                }
            }

            currentNumber++;

            if (currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce++;
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    /// <summary>
    /// ������ ��ġ�� �޾� Enemy ������Ʈ�� Ȱ��ȭ(�޸� Ǯ ����)
    /// </summary>
    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(enemySpawnLatency);

        // Enemy ������Ʈ�� �����ϰ�, Enemy ��ġ�� point�� ��ġ�� ����
        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;
        item.GetComponent<EnemyFSM>().Setup(target, this);

        // Ÿ�� ������Ʈ�� ��Ȱ��ȭ
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    /// <summary>
    /// Enemy�� ��Ȱ��ȭ
    /// </summary>
    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
    }

    /// <summary>
    /// ��� Enemy�� ��Ȱ��ȭ
    /// </summary>
    public void DeactivateAllEnemy()
    {
        enemyMemoryPool.DeactivateAllPoolItems();
    }

    /// <summary>
    /// Enemy ������ ����(SpawnTile�ڷ�ƾ�� ȣ��)
    /// </summary>
    public void SpawnStart()
    {
        isSpawn = true;
        StartCoroutine("SpawnTile");
    }

    /// <summary>
    /// Enemy ������ ����(SpawnTile�ڷ�ƾ�� ����)
    /// </summary>
    public void SpawnStop()
    {
        isSpawn = false;
        StopCoroutine("SpawnTile");

        DeactivateAllEnemy();
    }

}
