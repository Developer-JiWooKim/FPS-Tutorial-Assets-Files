using UnityEngine;

/// <summary>
/// Item 클래스가 상속받을 Base 추상 클래스
/// </summary>
public abstract class ItemBase : MonoBehaviour
{
    /// <summary>
    /// 아이템이 사용될 때 호출될 추상 메소드
    /// </summary>
    public abstract void Use(GameObject entity);
}
