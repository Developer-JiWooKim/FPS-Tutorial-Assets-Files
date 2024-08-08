using UnityEngine;

public class ParticleAutoDestroyerByTime : MonoBehaviour
{
    private ParticleSystem      particle;       // ��ƼŬ �ý��� ������Ʈ

    /// <summary>
    /// ��ƼŬ �ý��� ������Ʈ�� ���� ������ ����
    /// </summary>
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();    
    }

    /// <summary>
    /// ��ƼŬ(����Ʈ)�� ��� ���� �ƴϸ� �ڵ����� ����
    /// </summary>
    void Update()
    {
        if (particle.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}
