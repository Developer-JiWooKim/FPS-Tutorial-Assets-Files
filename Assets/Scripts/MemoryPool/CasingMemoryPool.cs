using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject  casingPrefab;    // ź�� ������Ʈ

    private MemoryPool  memoryPool;      // ź�� �޸�Ǯ

    /// <summary>
    /// ź�� ������Ʈ�� ���� �޸� Ǯ Ŭ������ ����
    /// </summary>
    private void Awake()
    {
        memoryPool = new MemoryPool(casingPrefab);
    }

    /// <summary>
    /// ź�Ǹ� ������ ��ġ���� ����(�޸� Ǯ ����)
    /// </summary>
    public void SpawnCasing(Vector3 position, Vector3 direction)
    {
        GameObject item = memoryPool.ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().SetUp(memoryPool, direction);
    }

}
