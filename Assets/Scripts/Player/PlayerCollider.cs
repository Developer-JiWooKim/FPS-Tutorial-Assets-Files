using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    /// <summary>
    /// 플레이어가 아이템과 상호작용 시 아이템의 Use함수를 실행하는 트리거
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<ItemBase>().Use(transform.parent.gameObject);
        }
    }
}
