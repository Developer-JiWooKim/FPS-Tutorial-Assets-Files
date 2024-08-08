using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAssultRifle : WeaponBase
{
    private enum E_MessageIndex
    {
        AmmoReload = 0,
        MagazineReload,
    }

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip           audioClipTakeOutWeapon;     // 무기 장착 사운드
    [SerializeField]
    private AudioClip           audioClipFire;              // 공격 사운드
    [SerializeField]
    private AudioClip           audioClipReload;            // 재장전 사운드

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject          muzzleFlashEffect;

    [Header("Spawn Points")]
    [SerializeField]
    private Transform           casingSpawnPoint;           // 탄피 생성 위치
    [SerializeField]
    private Transform           bulletSpawnPoint;           // 총알 생성 위치

    [Header("UI")]
    [SerializeField]
    private Image               imageAim;
    [SerializeField]
    private MessageController   MessageController;

    private bool                isModeChange = false;       // 모드 전환 여부 체크용
    private float               defaultModeFOV = 60;        // 기본 모드 카메라 FOV
    private float               AimModeFOV = 30;            // 에임 모드 카메라 FOV
    
    private CasingMemoryPool    casingMemoryPool;           // 탄피 생성 후 활성 / 비활성 관리(풀링)
    private ImpactMemoryPool    impactMemoryPool;           // 공격 효과 생성 후 활성 / 비활성 관리(풀링)
    private Camera              mainCamera;                 

    /// <summary>
    /// 미션과 관련된 변수
    /// </summary>
    private const int           weaponChangeMissionNumber = 1; // 에임모드 미션 번호
    private bool                aimModeMission = false;

    /// <summary>
    /// 탄피, 무기 초기화
    /// </summary>
    private void Awake()
    {
        base.Setup();

        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        // 처음 탄창, 탄 수는 최대로 설정
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;

        // 메세지들을 배열에 저장
        InputMessage();
    }
    private void OnEnable()
    {
        PlaySound(audioClipTakeOutWeapon); // 무기 장착 사운드 재생
        muzzleFlashEffect.SetActive(false); // 총구 이펙트 비활성화
        ResetVariables();

        // 총알 제한이 안되어있으면
        if (isLimited == false) 
        {
            onMagazineEvent.Invoke(0);
            onAmmoEvent.Invoke(999, 999);
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
            weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        }
        // 총알이 제한되어있으면
        else
        {
            // 무기가 활성화될 때 해당 무기의 탄창 정보를 갱신
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);

            // 무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        }
    }

    /// <summary>
    /// AimMode 미션 클리어 시 호출될 함수
    /// </summary>
    private void Mission_AimMode()
    {
        // 활성화된 미션이 있고 그 미션이 WeaponChangeRoom의 미션일 때
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.ChangeWeapon);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.ChangeWeapon, weaponChangeMissionNumber);
            aimModeMission = true;
        }
    }

    /// <summary>
    /// 메세지 배열에 메세지 입력
    /// </summary>
    private void InputMessage()
    {
        messages = new string[2];
        checkMessages = new bool[2];

        messages[(int)E_MessageIndex.AmmoReload] = "Please reload!\nPress Key : R";
        checkMessages[(int)E_MessageIndex.AmmoReload] = false;

        messages[(int)E_MessageIndex.MagazineReload] = "Run out of magazines!";
        checkMessages[(int)E_MessageIndex.MagazineReload] = false;
    }

    /// <summary>
    /// 공격 시 Ray를 쏴서 부딪힌 대상에 따라 데미지 처리하는 함수
    /// </summary>
    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;
        bool isBump = false;

        // 화면의 중앙 좌표(Aim기준으로 Raycast연산)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        isBump = Physics.Raycast(ray, out hit, weaponSetting.attackDistance);

        // 공격 사거리(attackDistance) 안에 부딪히는 오브젝트가 있으면 targetPoint는 광선에 부딪힌 위치
        if (isBump)
        {
            targetPoint = hit.point;
        }
        // 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetPoint는 최대 사거리 위치
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }
        // TODO#:광선이 제대로 발사되고 있는지 확인하는 테스트용 / 테스트 이후에는 삭제, 귀찮으면 전처리로 디버그, 릴리즈 구분
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

        // 첫 번째 Raycast 연산으로 얻어진 targetPoint를 목표지점으로 설정, 총구를 시작지점으로 하여 Raycast연산 
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if (hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);

    }

    /// <summary>
    /// 공격 버튼 클릭 시 호출될 함수
    /// </summary>
    public override void StartWeaponAction(int type = 0)
    {
        // 재장전 중이면 무기 액션 불가
        if (isReload == true)
        {
            return;
        }
        // 모드 전환 중이면 무기 액션 불가
        if (isModeChange == true)
        {
            return;
        }
        // 마우스 왼쪽 클릭 -> 공격 시작
        if (type == 0)
        {
            // 연속 공격
            if (weaponSetting.isAutomaticAttack == true)
            {
                StartCoroutine("OnAttackLoop");
            }
            // 단발 공격
            else
            {
                OnAttack();
            }
        }
        // 마우스 오른쪽 클릭 -> 모드 전환
        else
        {
            // 공격 중일 때는 모드 전환 불가
            if (isAttack == true)
            {
                return;
            }
            // 미션이 클리어 되어있지 않으면 미션 클리어
            if (aimModeMission == false)
            {
                Mission_AimMode();
            }
            
            StartCoroutine("OnModeChange");
        }
    }

    /// <summary>
    /// 재장전 시 호출될 함수
    /// </summary>
    public override void StartReload()
    {
        if (isLimited == false)
        {
            return; // 총알 제한 X -> 재장전 X
        }

        // 현재 재장전 중이면 재장전 불가 / 현재 잔여 탄창이 0이하이면 재장전 불가
        if (isReload == true)
        {
            return;
        }

        if (weaponSetting.currentMagazine <= 0)
        {
            // 탄창이 다 떨어졌음을 알리는 메세지를 한 번 만 보냄
            if (checkMessages[(int)E_MessageIndex.MagazineReload] == false)
            {
                MessageController.MessageSave(messages[(int)E_MessageIndex.MagazineReload]);
                checkMessages[(int)E_MessageIndex.MagazineReload] = true;
            }
            return;
        }

        // 무기 액션 도중에 'R'키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
        StopWeaponAction();

        // 현재 에임모드에서 재장전을 시도 중이면 에임모드를 중지 후 재장전 후 다시 에임모드로
        if (animator.AimModeIs == true)
        {
            StartCoroutine("AimModeReload");

        }
        else // 에임모드가 아닌 상태에서 재장전 시도 시 리로드 코루틴 시작
        {
            StartCoroutine("OnReload");
        }
    }

    /// <summary>
    /// 공격을 멈출 때 호출될 함수
    /// </summary>
    public override void StopWeaponAction(int type = 0)
    {
        // 마우스 왼쪽 클릭(공격 종료)
        if (type == 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }
    }

    /// <summary>
    /// 메세지 초기화 함수(ResetMagazine)
    /// </summary>
    public override void ResetMagazineMessage()
    {
        this.checkMessages[(int)E_MessageIndex.MagazineReload] = false;
    }

    /// <summary>
    /// Aim 모드일 때 재장전 시 (에임 모드 해제 - 재장전 - 다시 에임모드)를 적용하는 코루틴 함수
    /// </summary>
    private IEnumerator AimModeReload()
    {
        yield return StartCoroutine("OnModeChange");
        yield return StartCoroutine("OnReload");
        yield return StartCoroutine("OnModeChange");
    }
    
    /// <summary>
    /// 연속공격(공격 버튼을 계속 누르고 있으면) 호출되는 코루틴 함수
    /// </summary>
    private IEnumerator OnAttackLoop()
    {
        while(true)
        {
            OnAttack();
            yield return null;
        }
    }

    /// <summary>
    /// 공격 시 호출되는 함수
    /// </summary>
    private void OnAttack()
    {
        bool isAttack = Time.time - lastAttackTime > weaponSetting.attackRate;
        if (isAttack)
        {
            // 뛰고 있을 때 공격 불가
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            lastAttackTime = Time.time; // 공격 주기가 되어야 공격 할 수 있게 현재 시간을 저장

            // 현재 탄수가 0이하 이면 공격 불가
            if (weaponSetting.currentAmmo <= 0)
            {
                if (this.checkMessages[(int)E_MessageIndex.AmmoReload] == false)
                {
                    // 탄이 다 떨어졌음을 알리는 메세지 출력
                    MessageController.MessageSave(messages[(int)E_MessageIndex.AmmoReload]);
                    checkMessages[(int)E_MessageIndex.AmmoReload] = true;
                }
                return;
            }

            if (isLimited == true) // 총알이 제한되어있으면 기존대로
            {
                weaponSetting.currentAmmo--; // 공격 성공 시 currentAmmo 하나 감소
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo); // 총알을 소비할 때 마다 UI를 갱신할 이벤트(onAmmoEvent) 발생 - 해당 함수 실행(등록된 함수)  
            }// 제한되어 있지 않으면 UI변경 X
            

            // 무기 애니메이션 재생(모드에 따라 AimFire or Fire 애니메이션 재생)
            //animator.Play("Fire", -1, 0); 
            string animation = animator.AimModeIs == true ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);

            //총구 이펙트 재생(default mode일 때만 재생)
            if (animator.AimModeIs == false)
            {
                StartCoroutine("OnMuzzleFlashEffect"); // 이펙트 재생 코루틴
            }
            
            PlaySound(audioClipFire); // 공격 사운드
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right); // 탄피 생성

            // 광선을 발사해 원하는 위치 공격
            TwoStepRaycast();
        }
    }

    /// <summary>
    /// MuzzleFlashEffect를 재생하는 코루틴 함수
    /// </summary>
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

    /// <summary>
    /// 재장전 코루틴 함수
    /// </summary>
    private IEnumerator OnReload()
    {
        isReload = true;
        animator.OnReload();

        PlaySound(audioClipReload);

        while(true)
        {
            // 사운드 재생중이 아니고, 현재 애니메이션이 Movement이면 재장전 애니메이션, 사운드 재생이 종료되었다는 뜻
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                // 현재 탄창 수를 1감소, 바뀐 탄창 정보를 Text UI에 업데이트
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                // 현재 탄수를 최대로 설정, 바뀐 탄 수 정보를 Text UI에 업데이트
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                // 총알이 다 충전 되었으므로 다시 메세지를 보낼 수 있게
                checkMessages[(int)E_MessageIndex.AmmoReload] = false;

                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 현재 상태(재장전여부, 공격여부) 초기화
    /// </summary>
    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
        isModeChange = false;
    }

    /// <summary>
    /// 모드 변경 코루틴 함수(Aim < - > Normal)
    /// </summary>
    private IEnumerator OnModeChange()
    {
        float current = 0f;
        float percent = 0f;
        float time = 0.35f;

        animator.AimModeIs = !animator.AimModeIs;
        imageAim.enabled = !imageAim.enabled;

        float start = mainCamera.fieldOfView;
        float end = animator.AimModeIs == true ? AimModeFOV : defaultModeFOV;

        isModeChange = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        isModeChange = false;
    }
}
