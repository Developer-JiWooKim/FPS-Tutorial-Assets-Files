using UnityEngine;

public class DestructibleBarrel : InteractionObject
{
    [Header("Destructible Barrel")]
    [SerializeField]
    private GameObject      destructibleBarrelPieces;   // destructibleBarrelPieces 프리팹

    private bool            isDestroyed = false;        // 파괴 여부 저장 할 변수

    private Vector3         pos;                        // 리스폰 대비 초기 위치 값 저장
    private Quaternion      rot;                        // 리스폰 대비 초기 회전 값 저장

    /// <summary>
    /// 초기 위치, 초기 회전 값을 변수에 저장
    /// </summary>
    private void Awake()
    {
        pos = transform.position;
        rot = transform.rotation;
    }

    /// <summary>
    /// 데미지를 입었을 때 실행될 함수
    /// </summary>
    public override void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0 && isDestroyed == false)
        {
            isDestroyed = true;

            GameObject pieces = Instantiate(destructibleBarrelPieces, transform.position, transform.rotation);

            // DestructibleBarrel을 리스폰
            gameObject.GetComponentInParent<BarrelManager>().ReSpawnBarrel(gameObject); 

            Destroy(pieces, 4f); // 생성된 destructibleBarrelPieces 삭제
            gameObject.SetActive(false); // DestructibleBarrel 비활성화
        }
    }

    /// <summary>
    /// 비홀성화 될 때 DestructibleBarrel 오브젝트 초기화
    /// </summary>
    private void OnDisable()
    {
        Setup();
    }

    /// <summary>
    /// DestructibleBarrel 오브젝트 초기화 함수
    /// </summary>
    private void Setup()
    {
        isDestroyed = false;
        currentHP = maxHP;
        gameObject.transform.position = pos;
        gameObject.transform.rotation = rot;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
