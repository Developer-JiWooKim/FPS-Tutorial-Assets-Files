using UnityEngine;

public enum ImpactType {  Normal = 0, Obstacle, Enemy, InteractionObject, }

/// <summary>
/// 피격 이펙트 메모리 풀 클래스
/// </summary>
public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[]        impactPrefab;       // 피격 이펙트
    private MemoryPool[]        memoryPool;         // 피격 이펙트 메모리풀

    /// <summary>
    /// 피격 이펙트에 대한 메모리 풀 클래스를 생성
    /// </summary>
    private void Awake()
    {
        // 피격 이펙트가 여러 종류이면 종류별로 memoryPool 생성
        memoryPool = new MemoryPool[impactPrefab.Length];
        for (int i = 0; i < impactPrefab.Length; i++)
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    /// <summary>
    /// 부딪힌 오브젝트에 따라 피격 이펙트를 생성(메모리 풀 적용)
    /// </summary>
    public void SpawnImpact(RaycastHit hit)
    {
        if (hit.transform.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if (hit.transform.CompareTag("ImpactEnemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal));
        }
        // 상호작용 오브젝트의 종류가 많기 때문에 오브젝트 별로 타격 효과를 생성 X
        // 오브젝트 색상에 따라 변하도록 설정
        else if (hit.transform.CompareTag("InteractionObject"))
        {
            Color color = hit.transform.GetComponent<MeshRenderer>().material.color;
            OnSpawnImpact(ImpactType.InteractionObject, hit.point, Quaternion.LookRotation(hit.normal), color);
        }
    }

    /// <summary>
    /// 근접 공격인 Knife는 부딪혔을 때 이펙트를 재생하기 위해 Collider용 메소드 정의 
    /// </summary>
    public void SpawnImpact(Collider other, Transform knifeTransform)
    {
        // 부딪힌 오브젝트의 태그에 따라 다르게 처리
        if (other.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
        }
        else if (other.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
        }
        else if (other.CompareTag("ImpactEnemy"))
        {
            OnSpawnImpact(ImpactType.Enemy, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
        }
        else if (other.CompareTag("InteractionObject"))
        {
            Color color = other.transform.GetComponentInChildren<MeshRenderer>().material.color;
            OnSpawnImpact(ImpactType.InteractionObject, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation),color);
        }
    }

    /// <summary>
    /// ImpactType타입에 따라 메모리 풀 클래스에 생성된 피격 이펙트를 재생
    /// </summary>
    /// <param name="type"> ImpactType타입 </param>
    /// <param name="position"> 생성(이펙트 재생)될 위치 </param>
    /// <param name="rotation"> 이펙트 회전 값 </param>
    /// <param name="color"> InteractionObject일 때 적용될 색깔 </param>
    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation, Color color = new Color())
    {
        GameObject item = memoryPool[(int)type].ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);

        if (type == ImpactType.InteractionObject)
        {
            // ParticleSystem의 main프로퍼티는 바로 접근이 불가 -> ParticleSystem.MainModule형으로 변수를 생성 후 접근
            ParticleSystem.MainModule main = item.GetComponent<ParticleSystem>().main;
            main.startColor = color;
        }
    }
}
