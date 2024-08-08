using UnityEngine;

public class Impact : MonoBehaviour
{
    private ParticleSystem      particle;       // 파티클 시스템 컴포넌트
    private MemoryPool          memoryPool;     // 메모리 풀 클래스

    /// <summary>
    /// 파티클 시스템 컴포넌트를 얻어와 변수에 저장
    /// </summary>
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// 메모리 풀 클래스를 매개변수로 받아 변수에 저장
    /// </summary>
    public void Setup(MemoryPool pool)
    {
        memoryPool = pool;
    }

    /// <summary>
    /// 파티클(Impact)이 재생 중이 아니면 자동으로 비활성화
    /// </summary>
    private void Update()
    {
        if (particle.isPlaying == false)
        {
            memoryPool.DeactivatePoolItem(gameObject);
        }
    }
}
