using System.Collections;
using UnityEngine;

/// <summary>
/// Barrel 오브젝트의 활성화만 관리하는 매니저
/// </summary>
public class BarrelManager : MonoBehaviour
{
    private float       delayTime = 5f;

    /// <summary>
    /// Barrel 오브젝트를 리스폰해주는 함수
    /// </summary>
    public void ReSpawnBarrel(GameObject barrel)
    {
        StartCoroutine("ReSpawn", barrel);
    }

    /// <summary>
    /// delayTime 후 오브젝트를 활성화하는 함수
    /// </summary>
    private IEnumerator ReSpawn(GameObject item)
    {
        yield return new WaitForSeconds(delayTime);
        item.SetActive(true);
    }
}
