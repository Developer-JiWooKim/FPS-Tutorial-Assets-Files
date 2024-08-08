using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    /// <summary>
    /// �÷��̾ �����۰� ��ȣ�ۿ� �� �������� Use�Լ��� �����ϴ� Ʈ����
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<ItemBase>().Use(transform.parent.gameObject);
        }
    }
}
