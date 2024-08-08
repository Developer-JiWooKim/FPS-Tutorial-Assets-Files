using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode     keyCodeRun = KeyCode.LeftShift;     // 달리기 키
    [SerializeField]
    private KeyCode     keyCodeJump = KeyCode.Space;        // 점프 키
    [SerializeField]
    private KeyCode     keyCodeReload = KeyCode.R;          // 탄 재장전 키
    [SerializeField]
    private KeyCode     keyCodeEndGame = KeyCode.Escape;    // 게임 종료 키


    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip   audioClipWalk;                      // 걸을 때 재생 할 AudioClip
    [SerializeField]
    private AudioClip   audioClipRun;                       // 뛸 때 재생 할 AudioClip

    [SerializeField]
    private EndUIController             endUIController;    // 게임 종료 시도 시 나타날 UI를 제어하는 컨트롤러


    private RotateToMouse               rotateToMouse;                  // 마우스로 카메라 회전을 제어하는 클래스(Camera Rotate to Mouse Move)
    private MovementCharacterController movementCharacterController;    // 캐릭터 움직임과 관련된 클래스(Player Move and Jump to Keyboard Input)
    private Status                      status;                         // 체력을 가진 클래스가 상속받는 클래스
    private AudioSource                 audioSource;                    // AudioSource 
    private WeaponBase                  weapon;                         // 모든 무기가 상속받는 기반 클래스

    private bool                        isWeaponOnOff;                  // 현재 무기가 OnOff 되어 있는지 판별하는 변수

    private bool                        isEndGame;                      // 게임을 끝내는 UI가 켜져있는지 여부를 판별하는 변수

    /// <summary>
    /// 미션과 관련된 변수들
    /// </summary>
    private const int                   startMissionNumber_move = 0;
    private const int                   startMissionNumber_dash = 1;
    private bool                        moveMission = false;
    private bool                        dashMission = false;

    /// <summary>
    /// 무기가 활성화되어있는지 여부를 판별하는 isWeaponOnOff변수에 대한 프로퍼티
    /// </summary>
    public bool IsWeaponOnOff
    {
        get => isWeaponOnOff;
        set => isWeaponOnOff = value;
    }

    /// <summary>
    /// 플레이어 상태 enum
    /// </summary>
    enum E_PlayerState
    {
        Idle,
        Move,
        Die,
    }

    [SerializeField]
    private E_PlayerState   _state;     // 플레이어 상태를 저장하는 E_PlayerState타입 변수

    /// <summary>
    /// 초기화(변수 초기화, 컴포넌트들을 얻어와 변수에 저장)
    /// </summary>
    private void Awake()
    {
        isWeaponOnOff = false;
        Cursor.visible = false;                     // 마우스 커서 안보이게
        Cursor.lockState = CursorLockMode.Locked;   // 마우스 커서를 현재 위치에 고정
        isEndGame = false;

        rotateToMouse                   = GetComponent<RotateToMouse>();
        movementCharacterController     = GetComponent<MovementCharacterController>();
        status                          = GetComponent<Status>();
        audioSource                     = GetComponent<AudioSource>();
        weapon                          = GetComponentInChildren<WeaponBase>();
    }

    /// <summary>
    /// 플레이어의 초기 상태 Idle로 설정
    /// </summary>
    private void Start()
    {
        _state = E_PlayerState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEndGame();
        UpdateWeaponAction();
        UpdateMove();
        UpdateRotate();
        UpdateJump();
    }

    /// <summary>
    /// 게임 종료 UI를 띄울 시 세팅 값 설정하는 함수
    /// </summary>
    private void EndGameSetting()
    {
        isEndGame = true;
        Cursor.visible = true; // 게임을 종료하기 위해 마우스 커서 보이게
        Cursor.lockState = CursorLockMode.None; // 마우스 커서 고정 해제
    }

    /// <summary>
    /// 플레이어가 마우스를 움직였을 때 카메라를 회전시키는 동작을 수행하는 함수
    /// </summary>
    private void UpdateRotate()
    {
        // ESC키를 눌렀을 때 캐릭터 조작 X
        if (isEndGame)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    /// <summary>
    /// 캐릭터가 움직이거나 멈췄을 때 할 일들을 가진 함수
    /// </summary>
    private void UpdateMove()
    {
        // ESC키를 눌렀을 때 캐릭터 조작 X
        if (isEndGame)
        {
            movementCharacterController.MoveTo(new Vector3(0, 0, 0));
            return;
        }

        float horizontal    = Input.GetAxis("Horizontal");
        float vertical      = Input.GetAxis("Vertical");
        bool isRun          = false;

        MoveStateCheck(horizontal, vertical);

        switch (_state)
        {
            case E_PlayerState.Idle:
                // 정지 상황에서 할 일
                {
                    movementCharacterController.MoveSpeed = 0;
                    weapon.Animator.MoveSpeed = 0;

                    // 멈췄을 때 사운드 재생중이면 정지
                    if (audioSource.isPlaying == true)
                    {
                        audioSource.Stop();
                    }
                }
                break;
            case E_PlayerState.Move:
                // 움직이는 상황에서 할 일
                {
                    // 최초 1회 moveMission이 클리어 되어있지 않으면 미션 클리어
                    if (moveMission == false)
                    {
                        Mission_Move();
                    }

                    // 옆이나 뒤로 이동 시 달리기 불가능(WalkSpeed적용)
                    if (vertical > 0) 
                    {
                        isRun = Input.GetKey(keyCodeRun);
                    }

                    movementCharacterController.MoveSpeed   = isRun == true ? status.RunSpeed : status.WalkSpeed;

                    // 최초 1회 dashMission이 클리어 되어있지 않으면 미션 클리어
                    if (dashMission == false)
                    {
                        if (isRun == true)
                        {
                            Mission_Dash();
                        }
                    }

                    weapon.Animator.MoveSpeed               = isRun == true ? 1 : 0.5f;
                    audioSource.clip                        = isRun == true ? audioClipRun : audioClipWalk;

                    // 방향키 입력 여부는 매 프레임 확인 -> 재생 중일 때 다시 재생하지 않게 isPlaying으로 체크
                    if (audioSource.isPlaying == false)
                    {
                        audioSource.loop = true;
                        audioSource.Play();
                    }
                }
                break;
            case E_PlayerState.Die:
                break;
        }
        movementCharacterController.MoveTo(new Vector3(horizontal, 0, vertical));
    }

    /// <summary>
    /// 점프 키(Space Bar)를 눌렀을 때 캐릭터를 점프 시키는 함수
    /// </summary>
    private void UpdateJump()
    {
        // ESC키를 눌렀을 때 캐릭터 조작 X
        if (isEndGame)
        {
            return;
        }

        if (Input.GetKeyDown(keyCodeJump))
        {
            movementCharacterController.Jump();
        }
    }

    /// <summary>
    /// 무기와 관련된 동작들을 수행하는 함수
    /// </summary>
    private void UpdateWeaponAction()
    {
        // ESC키를 눌렀을 때 캐릭터 조작 X
        if (isEndGame)
        {
            return;
        }

        // 무기가 활성화 되어있지 않으면 무기 액션 불가 
        if (!IsWeaponOnOff)
        {
            return;
        }

        if ( Input.GetMouseButtonDown(0) )
        {
            weapon.StartWeaponAction();
        }
        else if ( Input.GetMouseButtonUp(0) )
        {
            weapon.StopWeaponAction();
        }

        if (Input.GetMouseButtonDown(1))
        {
            weapon.StartWeaponAction(1);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            weapon.StopWeaponAction(1);
        }

        if (Input.GetKeyDown(keyCodeReload))
        {
            weapon.StartReload();
        }
    }

    /// <summary>
    /// 움직이는지 아닌지 판별해 State변수에 판별된 결과 값 대입하는 함수
    /// </summary>
    private void MoveStateCheck(float hor, float ver)
    {
        _state = (hor != 0 || ver != 0) ? E_PlayerState.Move : E_PlayerState.Idle;
    }

    /// <summary>
    /// Move 미션이 클리어될 때 실행될 함수
    /// </summary>
    private void Mission_Move()
    {
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Start);
        // 완료된 미션 처리(완료 처리, UI 변경)
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Start, startMissionNumber_move); 
            moveMission = true;
        }
    }

    /// <summary>
    /// Dash 미션이 클리어될 때 실행될 함수
    /// </summary>
    private void Mission_Dash()
    {
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Start);
        // 완료된 미션 처리(완료 처리, UI 변경)
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Start, startMissionNumber_dash);
            dashMission = true;
        }
    }

    /// <summary>
    /// 데미지를 입었을 때 실행될 함수 
    /// </summary>
    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);
        if (isDie == true)
        {
            Debug.Log("Player is Die!!\nGame Over!!!");
        }
    }

    /// <summary>
    /// 무기를 바꿀 때 호출될 함수
    /// </summary>
    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
    }

    /// <summary>
    /// ESC키를 눌렀을 때 게임을 끝낼지 물어보는 UI를 활성화 하기 위해 검사하는 함수
    /// </summary>
    public void UpdateEndGame()
    {
        if (Input.GetKeyDown(keyCodeEndGame))
        {
            EndGameSetting();
            endUIController.ShowEndUI();
        }
    }

    /// <summary>
    /// 게임 종료 UI를 활성화 했을 때 실행할 함수
    /// </summary>
    public void EndGameSettingReset()
    {
        isEndGame = false;
        Cursor.visible = false; // 게임을 종료하기 위해 마우스 커서 보이게
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 고정 해제
    }
}
