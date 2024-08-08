using UnityEngine;

public class EnemySpawnStop : MonoBehaviour
{
    public GameObject   enemyMemoryPoolObject;  // Enemy������ ���ߴ� �Լ��� �����ϱ� ���� EnemyMemoryPool ������Ʈ�� ����
    public GameObject   spawnStartObject;       // Enemy ������ ���۽�Ű�� ������Ʈ

    /// <summary>
    /// Player�� Enemy Room���� ���� �� Enemy������ ���߰� Enemy������ ���۽�Ű�� ������Ʈ�� �ٽ� Ȱ��ȭ
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
