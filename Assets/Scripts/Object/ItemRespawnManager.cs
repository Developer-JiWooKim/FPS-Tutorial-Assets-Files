using System.Collections;
using UnityEngine;

/// <summary>
/// ������ ������ �����ϴ� ������ �Ŵ���
/// </summary>
public class ItemRespawnManager : MonoBehaviour
{
    private float spawnTime = 3f;

    /// <summary>
    /// ������ ������ �Լ�
    /// </summary>
    public void ReSpawnItem(GameObject item)
    {
        StartCoroutine("ReSpawn", item);
    }

    /// <summary>
    /// spawnTime�� ���� �� �������� �ٽ� �����ϴ� �ڷ�ƾ
    /// </summary>
    /// <param name="item"> ������ ������Ʈ </param>
    private IEnumerator ReSpawn(GameObject item)
    {
        float currentTime = 0;
        
        while (spawnTime > currentTime)
        {
            currentTime += Time.deltaTime;
            if (currentTime > spawnTime)
            {
                item.SetActive(true);
            }
            yield return null;
        }
    }
}
