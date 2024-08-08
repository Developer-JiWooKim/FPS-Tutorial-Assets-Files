using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject  casingPrefab;    // 탄피 오브젝트

    private MemoryPool  memoryPool;      // 탄피 메모리풀

    /// <summary>
    /// 탄피 오브젝트에 대한 메모리 풀 클래스를 생성
    /// </summary>
    private void Awake()
    {
        memoryPool = new MemoryPool(casingPrefab);
    }

    /// <summary>
    /// 탄피를 정해진 위치에서 생성(메모리 풀 적용)
    /// </summary>
    public void SpawnCasing(Vector3 position, Vector3 direction)
    {
        GameObject item = memoryPool.ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().SetUp(memoryPool, direction);
    }

}
