using System.Collections;
using UnityEngine;

/// <summary>
/// 아이템 스폰을 관리하는 아이템 매니저
/// </summary>
public class ItemRespawnManager : MonoBehaviour
{
    private float spawnTime = 3f;

    /// <summary>
    /// 아이템 리스폰 함수
    /// </summary>
    public void ReSpawnItem(GameObject item)
    {
        StartCoroutine("ReSpawn", item);
    }

    /// <summary>
    /// spawnTime이 지난 후 아이템을 다시 생성하는 코루틴
    /// </summary>
    /// <param name="item"> 아이템 오브젝트 </param>
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
