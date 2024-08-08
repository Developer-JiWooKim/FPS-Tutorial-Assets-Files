using UnityEngine;

/// <summary>
/// Item Ŭ������ ��ӹ��� Base �߻� Ŭ����
/// </summary>
public abstract class ItemBase : MonoBehaviour
{
    /// <summary>
    /// �������� ���� �� ȣ��� �߻� �޼ҵ�
    /// </summary>
    public abstract void Use(GameObject entity);
}
