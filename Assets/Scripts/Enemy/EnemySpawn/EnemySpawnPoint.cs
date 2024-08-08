using System.Collections;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]
    private float           fadeSpeed = 4;
    
    private MeshRenderer    meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();   
    }
    /// <summary>
    /// Ȱ��ȭ �� �� Fadeȿ���� ����
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine("OnFadeEffect");
    }

    /// <summary>
    /// ��Ȱ��ȭ �� �� Fadeȿ���� ����
    /// </summary>
    private void OnDisable()
    {
        StopCoroutine("OnFadeEffect");
    }

    /// <summary>
    /// ������ ������ �� ������ ��ġ�� ǥ�õ� meshRenderer�� ���̵� ȿ�� �����ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator OnFadeEffect()
    {
        while (true)
        {
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSpeed, 1));

            meshRenderer.material.color = color;

            yield return null;
        }
    }
}
