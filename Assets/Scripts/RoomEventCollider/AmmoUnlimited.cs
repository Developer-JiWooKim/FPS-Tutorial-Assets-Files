using UnityEngine;

/// <summary>
/// �Ѿ� ������ ���� or ���� �����ϴ� Ŭ����
/// </summary>
public class AmmoUnlimited : MonoBehaviour
{
    /// <summary>
    /// �ش� �ݶ��̴��� ���� �� �Ѿ� ������ ���� or ���� ����
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        UnlimitedAmmo(other.gameObject);
    }

    /// <summary>
    /// �÷��̾��� ����鿡 ������ �Ѿ��� ����/�������� �ϴ� �Լ�
    /// </summary>
    private void UnlimitedAmmo(GameObject _player) 
    {
        WeaponBase[] _weapons;
        _weapons = _player.GetComponentsInChildren<WeaponBase>(true);
        for (int i = 0; i < _weapons.Length; i++)
        {
            Debug.Log(_weapons[i].name);
            _weapons[i].MemoryPoolRoomEvent();
        }
    }
}
