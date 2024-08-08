using UnityEngine;

public enum WeaponType { Main=0, Sub, Melee, Throw, }

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

/// <summary>
/// 무기들이 상속 받을 추상 클래스
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    // 외부에서 이벤트 함수 등록을 할 수 있도록 public 선언
    [HideInInspector]
    public AmmoEvent                    onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent                onMagazineEvent = new MagazineEvent();

    [Header("WeaponBase")]
    [SerializeField]
    public WeaponType                   weaponType;         // 무기 종류
    [SerializeField]
    protected WeaponSetting             weaponSetting;      // 무기 설정

    [SerializeField]
    protected bool                      isLimited = true;   // 총알 개수 무제한을 판별하는 변수

    protected float                     lastAttackTime = 0; // 마지막 발사시간 체크용
    protected bool                      isReload = false;   // 재장전 중인지 체크
    protected bool                      isAttack = false;   // 공격 여부 체크

    protected AudioSource               audioSource;
    protected PlayerAnimatorController  animator;

    // 메세지 창에 띄워질 메세지들
    protected string[]                  messages;
    protected bool[]                    checkMessages;      // 메세지를 한 번만 보내기 위해 bool 배열 선언

    /// <summary>
    /// 외부에서 필요한 정보를 열람하기 위해 정의한 프로퍼티
    /// </summary>
    public PlayerAnimatorController     Animator => animator;
    public WeaponName                   WeaponName => weaponSetting.weaponName;
    public int                          CurrentMagazine => weaponSetting.currentMagazine;
    public int                          MaxMagazine => weaponSetting.maxMagazine;

    public abstract void StartWeaponAction(int type = 0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void StartReload();

    /// <summary>
    /// 필요한 사운드를 재생하는 함수
    /// </summary>
    protected void PlaySound(AudioClip clip)
    {
        audioSource.Stop(); // 기존에 재생중인 사운드 정지
        audioSource.clip = clip; // 새로운 사운드 clip으로 교체
        audioSource.Play(); // 사운드 재생
    }

    protected void Setup()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<PlayerAnimatorController>();
    }

    /// <summary>
    /// 상속 받은 곳에서 재정의 후 사용될 virtual 함수
    /// </summary>
    public virtual void ResetMagazineMessage()
    {

    }

    /// <summary>
    /// 탄창을 증가 시키는 함수(아이템 사용 시 이 virtual함수에 접근해 활성화된 무기의 탄창을 증가)
    /// </summary>
    public virtual void IncreaseMagazine(int _magazine)
    {
        weaponSetting.currentMagazine = CurrentMagazine + _magazine > MaxMagazine ? 
                                        MaxMagazine : CurrentMagazine + _magazine;

        onMagazineEvent.Invoke(CurrentMagazine);
    }

    /// <summary>
    /// MemoryPoolRoom에 들어갈 때 총알 제한을 해제(나올때 다시 제한)
    /// </summary>
    public void MemoryPoolRoomEvent()
    {
        isLimited = !isLimited;
    }
}
