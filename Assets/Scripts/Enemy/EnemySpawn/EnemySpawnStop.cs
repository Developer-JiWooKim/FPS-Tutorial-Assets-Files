using UnityEngine;

public class EnemySpawnStop : MonoBehaviour
{
    public GameObject   enemyMemoryPoolObject;  // Enemy스폰을 멈추는 함수를 실행하기 위해 EnemyMemoryPool 오브젝트를 저장
    public GameObject   spawnStartObject;       // Enemy 스폰을 시작시키는 오브젝트

    /// <summary>
    /// Player가 Enemy Room에서 나갈 때 Enemy스폰을 멈추고 Enemy스폰을 시작시키는 오브젝트를 다시 활성화
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyMemoryPoolObject.GetComponent<EnemyMemoryPool>().SpawnStop();
            spawnStartObject.SetActive(true);
        }
    }
}
