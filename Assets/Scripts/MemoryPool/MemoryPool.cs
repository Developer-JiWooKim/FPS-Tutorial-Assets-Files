using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �޸� Ǯ�� ����� Ŭ�������� ������ �⺻ �޸� Ǯ Ŭ����
/// </summary>
public class MemoryPool
{
    /// <summary>
    /// �޸� Ǯ�� ����� ������ Ŭ����
    /// </summary>
    private class PoolItem
    {
        public  bool            isActive;       // "gameObject"�� Ȱ�� / ��Ȱ��ȭ ����
        public  GameObject      gameObject;     // ȭ�鿡 ���̴� ���� ������Ʈ
    }

    private int             increaseCount = 5;          // ������Ʈ�� ������ �� Instantiate()�� �߰� �����Ǵ� ������Ʈ ����
    private int             maxCount;                   // ���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ����
    private int             activeCount;                // ���� ���ӿ� ���ǰ� �ִ� ������Ʈ ����

    private GameObject      poolObject;                  // ������Ʈ Ǯ������ �����ϴ� ���� ������Ʈ ������
    private List<PoolItem>  poolItemList;                // �����Ǵ� ��� ������Ʈ�� �����ϴ� ����Ʈ

    /// <summary>
    /// �ܺο��� ������ ������Ƽ
    /// </summary>
    public int              MaxCount => maxCount;        // ���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ
    public int              ActiveCount => activeCount;  // ���� Ȱ��ȭ �Ǿ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ

    /// <summary>
    /// ������Ʈ�� �ӽ� �����Ǵ� ��ġ
    /// </summary>
    private Vector3 tempPosition = new Vector3(26, 0.01f, -32);
    /// <summary>
    /// ������, �������� �޾� �ʱ� ���� �� �޸� Ǯ ������ ����Ʈ�� ���� �� ������Ʈ�� ����
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
    /// increaseCount ������ ������Ʈ�� ����
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
    /// ���� ������(Ȱ�� / ��Ȱ��)�� ��� ������Ʈ�� ����
    /// </summary>
    public void DestroyObjects()
    {
        if (poolItemList == null)
        {
            return;
        }

        int count = poolItemList.Count; // ����Ʈ�� ����ִ� ������Ʈ �� 

        for (int i = 0; i < count; i++) // ������Ʈ ����
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear(); // poolItemList �ʱ�ȭ
    }

    /// <summary>
    /// poolItemList�� ����Ǿ� �ִ� ������Ʈ�� Ȱ��ȭ �ؼ� ���
    /// </summary>
    public GameObject ActivatePoolItem()
    {
        if (poolItemList == null)
        {
            return null;
        }

        // ���� �����ؼ� �����ϴ� ��� ������Ʈ ������ ���� Ȱ��ȭ ������ ������Ʈ ���� ��
        // ���� ��� ������Ʈ�� ��� ���̸� InstantiateObjects()�� �߰� ����
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
    /// ���� ����� �Ϸ�� ������Ʈ�� ��Ȱ��ȭ ���·� ����
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

                poolItem.gameObject.transform.position = tempPosition; // ������Ʈ�� ��Ȱ��ȭ �� �� ������Ʈ�� ��ġ�� tempPosition���� ����
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    }

    /// <summary>
    /// ���ӿ� ���� ��� ������Ʈ�� ��Ȱ��ȭ ���·� ����
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
                poolItem.gameObject.transform.position = tempPosition; // ������Ʈ ��Ȱ��ȭ �� ������Ʈ ��ġ�� tempPositionfh ����
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }
}
