using UnityEngine;

public class Impact : MonoBehaviour
{
    private ParticleSystem      particle;       // ��ƼŬ �ý��� ������Ʈ
    private MemoryPool          memoryPool;     // �޸� Ǯ Ŭ����

    /// <summary>
    /// ��ƼŬ �ý��� ������Ʈ�� ���� ������ ����
    /// </summary>
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// �޸� Ǯ Ŭ������ �Ű������� �޾� ������ ����
    /// </summary>
    public void Setup(MemoryPool pool)
    {
        memoryPool = pool;
    }

    /// <summary>
    /// ��ƼŬ(Impact)�� ��� ���� �ƴϸ� �ڵ����� ��Ȱ��ȭ
    /// </summary>
    private void Update()
    {
        if (particle.isPlaying == false)
        {
            memoryPool.DeactivatePoolItem(gameObject);
        }
    }
}
