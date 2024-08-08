using UnityEngine;

[System.Serializable]
public class HPEvent :  UnityEngine.Events.UnityEvent<int, int> { }

/// <summary>
/// HP를 갖는 오브젝트들이 갖게될 Status 클래스
/// </summary>
public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent  onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float   walkSpeed;
    [SerializeField]
    private float   runSpeed;

    [Header("HP")]
    [SerializeField]
    private int     maxHP = 100;
    private int     currentHP;

    /// <summary>
    /// 읽는 용도의 property
    /// </summary>
    public float    WalkSpeed => walkSpeed;
    public float    RunSpeed => runSpeed;
    public float    MaxHP => maxHP;
    public int      CurrentHP => currentHP;

    /// <summary>
    /// 체력을 가지는 오브젝트들은 시작과 동시에 최대 체력을 가지고 시작
    /// </summary>
    private void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 체력이 감소할 때 호출할 함수
    /// </summary>
    public bool DecreaseHP(int damage)
    {
        int previousHP = currentHP;

        //(현재 체력 - 받은 데미지) 가 0보다 크면 현재 체력에서 받은 데미지를 감소 시킴, 0보다 작으면 현재 체력을 0으로 설정
        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

        // onHPEvent에 등록된 이벤트 실행, UI를 업데이트
        onHPEvent.Invoke(previousHP, currentHP);

        // 현재 체력이 0이면 true리턴
        if (currentHP == 0) 
        {
            return true;
        }

        return false; // 아니면 false리턴
    }

    /// <summary>
    /// 체력 회복 아이템을 먹었을 때 호출할 함수
    /// </summary>
    public void IncreaseHP(int hp)
    {
        int previousHP = currentHP;

        //(현재 체력 + 회복 체력) 가 최대 체력 보다 크면 체력을 최대 체력으로 설정
        currentHP = currentHP + hp > maxHP ? maxHP : currentHP + hp;

        // onHPEvent에 등록된 이벤트 실행, UI를 업데이트
        onHPEvent.Invoke(previousHP, currentHP);
    }

    /// <summary>
    /// 체력을 초기화
    /// </summary>
    public void SetUpHP()
    {
        currentHP = maxHP;
    }
}
