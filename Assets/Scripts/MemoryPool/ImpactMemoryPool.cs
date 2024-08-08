using UnityEngine;

public enum ImpactType {  Normal = 0, Obstacle, Enemy, InteractionObject, }

/// <summary>
/// �ǰ� ����Ʈ �޸� Ǯ Ŭ����
/// </summary>
public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[]        impactPrefab;       // �ǰ� ����Ʈ
    private MemoryPool[]        memoryPool;         // �ǰ� ����Ʈ �޸�Ǯ

    /// <summary>
    /// �ǰ� ����Ʈ�� ���� �޸� Ǯ Ŭ������ ����
    /// </summary>
    private void Awake()
    {
        // �ǰ� ����Ʈ�� ���� �����̸� �������� memoryPool ����
        memoryPool = new MemoryPool[impactPrefab.Length];
        for (int i = 0; i < impactPrefab.Length; i++)
        {
            memoryPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    /// <summary>
    /// �ε��� ������Ʈ�� ���� �ǰ� ����Ʈ�� ����(�޸� Ǯ ����)
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
        // ��ȣ�ۿ� ������Ʈ�� ������ ���� ������ ������Ʈ ���� Ÿ�� ȿ���� ���� X
        // ������Ʈ ���� ���� ���ϵ��� ����
        else if (hit.transform.CompareTag("InteractionObject"))
        {
            Color color = hit.transform.GetComponent<MeshRenderer>().material.color;
            OnSpawnImpact(ImpactType.InteractionObject, hit.point, Quaternion.LookRotation(hit.normal), color);
        }
    }

    /// <summary>
    /// ���� ������ Knife�� �ε����� �� ����Ʈ�� ����ϱ� ���� Collider�� �޼ҵ� ���� 
    /// </summary>
    public void SpawnImpact(Collider other, Transform knifeTransform)
    {
        // �ε��� ������Ʈ�� �±׿� ���� �ٸ��� ó��
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
    /// ImpactTypeŸ�Կ� ���� �޸� Ǯ Ŭ������ ������ �ǰ� ����Ʈ�� ���
    /// </summary>
    /// <param name="type"> ImpactTypeŸ�� </param>
    /// <param name="position"> ����(����Ʈ ���)�� ��ġ </param>
    /// <param name="rotation"> ����Ʈ ȸ�� �� </param>
    /// <param name="color"> InteractionObject�� �� ����� ���� </param>
    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation, Color color = new Color())
    {
        GameObject item = memoryPool[(int)type].ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);

        if (type == ImpactType.InteractionObject)
        {
            // ParticleSystem�� main������Ƽ�� �ٷ� ������ �Ұ� -> ParticleSystem.MainModule������ ������ ���� �� ����
            ParticleSystem.MainModule main = item.GetComponent<ParticleSystem>().main;
            main.startColor = color;
        }
    }
}
