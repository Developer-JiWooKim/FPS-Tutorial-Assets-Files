using System.Collections;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform       target;                             // Enemy의 목표(Player 등...)

    [SerializeField]
    private GameObject      enemySpawnPointPrefab;              // Enemy 등장 하기 전 Enemy 등장 위치를 알려주는 프리팹

    [SerializeField]
    private GameObject      enemyPrefab;                        // Enemy 프리팹

    [SerializeField]
    private GameObject      EnemySpawnField;                    // Enemy가 생성될 발판(Field)

    [SerializeField]
    private float           enemySpawnTime = 1;                 // Enemy 생성 주기

    [SerializeField]
    private float           enemySpawnLatency = 1;              // 타일 생성 후 Enemy 등장하기 까지 대기 시간

    private MemoryPool      spawnPointMemoryPool;               // Enemy 등장 위치를 알려주는 오브젝트 생성, 활성 / 비활성 관리
    private MemoryPool      enemyMemoryPool;                    // Enemy 생성, 활성 / 비활성 관리(풀링)

    private Vector2Int      mapSize = new Vector2Int(30, 35);   // 맵 크기

    private int             numberOfEnemiesSpawnedAtOnce = 1;   // 동시 생성되는 Enemy 숫자
    private bool            isSpawn;                            // 스폰 여부 변수

    /// <summary>
    /// 각 프리팹에 대한 메모리 풀 클래스 생성, 스폰 여부 변수 초기화
    /// </summary>
    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);
        isSpawn = false;
    }

    /// <summary>
    /// Enemy 생성을 시작하기 전 생성될 위치를 구하고 Enemy를 생성시키는 코루틴 호출
    /// </summary>
    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 50;

        while (true)
        {
            // 동시에 numberOfEnemiesSpawnedAtOnce 숫자 만큼 적이 생성되도록 반복문 사용
            for (int i = 0; i < numberOfEnemiesSpawnedAtOnce; i++)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();

                // 적 생성 프리팹이 나타날 위치를 item에 저장
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
    /// 생성될 위치를 받아 Enemy 오브젝트를 활성화(메모리 풀 적용)
    /// </summary>
    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(enemySpawnLatency);

        // Enemy 오브젝트를 생성하고, Enemy 위치를 point의 위치로 설정
        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;
        item.GetComponent<EnemyFSM>().Setup(target, this);

        // 타일 오브젝트를 비활성화
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }

    /// <summary>
    /// Enemy를 비활성화
    /// </summary>
    public void DeactivateEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivatePoolItem(enemy);
    }

    /// <summary>
    /// 모든 Enemy를 비활성화
    /// </summary>
    public void DeactivateAllEnemy()
    {
        enemyMemoryPool.DeactivateAllPoolItems();
    }

    /// <summary>
    /// Enemy 스폰을 시작(SpawnTile코루틴을 호출)
    /// </summary>
    public void SpawnStart()
    {
        isSpawn = true;
        StartCoroutine("SpawnTile");
    }

    /// <summary>
    /// Enemy 스폰을 멈춤(SpawnTile코루틴을 멈춤)
    /// </summary>
    public void SpawnStop()
    {
        isSpawn = false;
        StopCoroutine("SpawnTile");

        DeactivateAllEnemy();
    }

}
