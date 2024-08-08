using System.Collections;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField]
    private float           deactivateTime = 5.0f;      // ź�� ���� �� ��Ȱ��ȭ �Ǵ� �ð� 

    [SerializeField]
    private float           casingSpin = 1.0f;          // ź�ǰ� ȸ���ϴ� �ӷ� ���

    [SerializeField]
    private AudioClip[]     audioClips;                 // ź�ǰ� �ε����� �� ����Ǵ� ����

    private Rigidbody       rigidbody3D;
    private AudioSource     audioSource;
    private MemoryPool      memoryPool;

    /// <summary>
    /// ź�� �ʱ�ȭ �Լ�
    /// </summary>
    public void SetUp(MemoryPool pool, Vector3 direction)
    {
        rigidbody3D = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        memoryPool = pool;

        /* ź���� �̵� �ӷ°� ȸ�� �ӷ� ����*/

        // �̵�
        rigidbody3D.velocity = new Vector3(direction.x, 1.0f, direction.z);
        // ȸ��
        rigidbody3D.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                                Random.Range(-casingSpin, casingSpin),
                                                Random.Range(-casingSpin, casingSpin));

        // ź�� �ڵ� ��Ȱ��ȭ�� ���� �ڷ�ƾ ����
        StartCoroutine("DeactiveAfterTime");

    }

    /// <summary>
    /// deactivateTime �ð� �� ź�Ǹ� ��Ȱ��ȭ�ϴ� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator DeactiveAfterTime()
    {
        yield return new WaitForSeconds(deactivateTime);

        memoryPool.DeactivatePoolItem(this.gameObject);
    }

    /// <summary>
    /// ź�ǰ� �ε����� �� ���带 �����Ű�� ���� �浹 ó��
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

}
