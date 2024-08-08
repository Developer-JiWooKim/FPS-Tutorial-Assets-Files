using System.Collections;
using UnityEngine;

/// <summary>
/// Barrel ������Ʈ�� Ȱ��ȭ�� �����ϴ� �Ŵ���
/// </summary>
public class BarrelManager : MonoBehaviour
{
    private float       delayTime = 5f;

    /// <summary>
    /// Barrel ������Ʈ�� ���������ִ� �Լ�
    /// </summary>
    public void ReSpawnBarrel(GameObject barrel)
    {
        StartCoroutine("ReSpawn", barrel);
    }

    /// <summary>
    /// delayTime �� ������Ʈ�� Ȱ��ȭ�ϴ� �Լ�
    /// </summary>
    private IEnumerator ReSpawn(GameObject item)
    {
        yield return new WaitForSeconds(delayTime);
        item.SetActive(true);
    }
}
