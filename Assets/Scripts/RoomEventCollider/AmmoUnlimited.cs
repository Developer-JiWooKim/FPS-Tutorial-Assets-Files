using UnityEngine;

/// <summary>
/// 총알 개수를 제한 or 제한 해제하는 클래스
/// </summary>
public class AmmoUnlimited : MonoBehaviour
{
    /// <summary>
    /// 해당 콜라이더에 접촉 시 총알 개수를 제한 or 제한 해제
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        UnlimitedAmmo(other.gameObject);
    }

    /// <summary>
    /// 플레이어의 무기들에 접근해 총알을 제한/제한해제 하는 함수
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
