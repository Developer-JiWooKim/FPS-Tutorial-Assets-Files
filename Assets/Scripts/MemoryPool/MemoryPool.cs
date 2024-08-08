using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 메모리 풀이 적용될 클래스에서 생성될 기본 메모리 풀 클래스
/// </summary>
public class MemoryPool
{
    /// <summary>
    /// 메모리 풀이 적용될 아이템 클래스
    /// </summary>
    private class PoolItem
    {
        public  bool            isActive;       // "gameObject"의 활성 / 비활성화 정보
        public  GameObject      gameObject;     // 화면에 보이는 실제 오브젝트
    }

    private int             increaseCount = 5;          // 오브젝트가 부족할 때 Instantiate()로 추가 생성되는 오브젝트 개수
    private int             maxCount;                   // 현재 리스트에 등록되어 있는 오브젝트 개수
    private int             activeCount;                // 현재 게임에 사용되고 있는 오브젝트 개수

    private GameObject      poolObject;                  // 오브젝트 풀링에서 관리하는 게임 오브젝트 프리팹
    private List<PoolItem>  poolItemList;                // 관리되는 모든 오브젝트를 저장하는 리스트

    /// <summary>
    /// 외부에서 접근할 프로퍼티
    /// </summary>
    public int              MaxCount => maxCount;        // 현재 리스트에 등록되어 있는 오브젝트 개수 확인을 위한 프로퍼티
    public int              ActiveCount => activeCount;  // 현재 활성화 되어 있는 오브젝트 개수 확인을 위한 프로퍼티

    /// <summary>
    /// 오브젝트가 임시 보관되는 위치
    /// </summary>
    private Vector3 tempPosition = new Vector3(26, 0.01f, -32);
    /// <summary>
    /// 생성자, 프리팹을 받아 초기 설정 및 메모리 풀 아이템 리스트에 저장 후 오브젝트를 생성
    /// </summary>
    public MemoryPool(GameObject poolObject)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;

        poolItemList = new List<PoolItem>();

        InstantiateObjects();
    }

    /// <summary>
    /// increaseCount 단위로 오브젝트를 생성
    /// </summary>
    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for (int i = 0; i < increaseCount; i++)
        {
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(poolObject);
            poolItem.gameObject.transform.position = tempPosition;
            poolItem.gameObject.SetActive(false);

            poolItemList.Add(poolItem);
        }
    }

    /// <summary>
    /// 현재 관리중(활성 / 비활성)인 모든 오브젝트를 삭제
    /// </summary>
    public void DestroyObjects()
    {
        if (poolItemList == null)
        {
            return;
        }

        int count = poolItemList.Count; // 리스트에 담겨있는 오브젝트 수 

        for (int i = 0; i < count; i++) // 오브젝트 삭제
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear(); // poolItemList 초기화
    }

    /// <summary>
    /// poolItemList에 저장되어 있는 오브젝트를 활성화 해서 사용
    /// </summary>
    public GameObject ActivatePoolItem()
    {
        if (poolItemList == null)
        {
            return null;
        }

        // 현재 생성해서 관리하는 모든 오브젝트 개수와 현재 활성화 상태인 오브젝트 개수 비교
        // 현재 모든 오브젝트가 사용 중이면 InstantiateObjects()로 추가 생성
        if (maxCount == activeCount)
        {
            InstantiateObjects();
        }

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++)
        {
            PoolItem poolItem = poolItemList[i]; ;
            if (poolItem.isActive == false)
            {
                activeCount++;

                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject;
            }
        }

        return null;
    }


    /// <summary>
    /// 현재 사용이 완료된 오브젝트를 비활성화 상태로 설정
    /// </summary>
    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null)
        {
            return;
        }

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject == removeObject)
            {
                activeCount--;

                poolItem.gameObject.transform.position = tempPosition; // 오브젝트를 비활성화 할 때 오브젝트의 위치를 tempPosition으로 설정
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    }

    /// <summary>
    /// 게임에 사용된 모든 오브젝트를 비활성화 상태로 설정
    /// </summary>
    public void DeactivateAllPoolItems()
    {
        if (poolItemList == null) 
        {
            return;
        }

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject != null && poolItem.isActive == true)
            {
                poolItem.gameObject.transform.position = tempPosition; // 오브젝트 비활성화 시 오브젝트 위치를 tempPositionfh 설정
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }
}
