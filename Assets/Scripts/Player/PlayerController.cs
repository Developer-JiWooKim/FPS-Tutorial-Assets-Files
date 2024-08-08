using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode     keyCodeRun = KeyCode.LeftShift;     // �޸��� Ű
    [SerializeField]
    private KeyCode     keyCodeJump = KeyCode.Space;        // ���� Ű
    [SerializeField]
    private KeyCode     keyCodeReload = KeyCode.R;          // ź ������ Ű
    [SerializeField]
    private KeyCode     keyCodeEndGame = KeyCode.Escape;    // ���� ���� Ű


    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip   audioClipWalk;                      // ���� �� ��� �� AudioClip
    [SerializeField]
    private AudioClip   audioClipRun;                       // �� �� ��� �� AudioClip

    [SerializeField]
    private EndUIController             endUIController;    // ���� ���� �õ� �� ��Ÿ�� UI�� �����ϴ� ��Ʈ�ѷ�


    private RotateToMouse               rotateToMouse;                  // ���콺�� ī�޶� ȸ���� �����ϴ� Ŭ����(Camera Rotate to Mouse Move)
    private MovementCharacterController movementCharacterController;    // ĳ���� �����Ӱ� ���õ� Ŭ����(Player Move and Jump to Keyboard Input)
    private Status                      status;                         // ü���� ���� Ŭ������ ��ӹ޴� Ŭ����
    private AudioSource                 audioSource;                    // AudioSource 
    private WeaponBase                  weapon;                         // ��� ���Ⱑ ��ӹ޴� ��� Ŭ����

    private bool                        isWeaponOnOff;                  // ���� ���Ⱑ OnOff �Ǿ� �ִ��� �Ǻ��ϴ� ����

    private bool                        isEndGame;                      // ������ ������ UI�� �����ִ��� ���θ� �Ǻ��ϴ� ����

    /// <summary>
    /// �̼ǰ� ���õ� ������
    /// </summary>
    private const int                   startMissionNumber_move = 0;
    private const int                   startMissionNumber_dash = 1;
    private bool                        moveMission = false;
    private bool                        dashMission = false;

    /// <summary>
    /// ���Ⱑ Ȱ��ȭ�Ǿ��ִ��� ���θ� �Ǻ��ϴ� isWeaponOnOff������ ���� ������Ƽ
    /// </summary>
    public bool IsWeaponOnOff
    {
        get => isWeaponOnOff;
        set => isWeaponOnOff = value;
    }

    /// <summary>
    /// �÷��̾� ���� enum
    /// </summary>
    enum E_PlayerState
    {
        Idle,
        Move,
        Die,
    }

    [SerializeField]
    private E_PlayerState   _state;     // �÷��̾� ���¸� �����ϴ� E_PlayerStateŸ�� ����

    /// <summary>
    /// �ʱ�ȭ(���� �ʱ�ȭ, ������Ʈ���� ���� ������ ����)
    /// </summary>
    private void Awake()
    {
        isWeaponOnOff = false;
        Cursor.visible = false;                     // ���콺 Ŀ�� �Ⱥ��̰�
        Cursor.lockState = CursorLockMode.Locked;   // ���콺 Ŀ���� ���� ��ġ�� ����
        isEndGame = false;

        rotateToMouse                   = GetComponent<RotateToMouse>();
        movementCharacterController     = GetComponent<MovementCharacterController>();
        status                          = GetComponent<Status>();
        audioSource                     = GetComponent<AudioSource>();
        weapon                          = GetComponentInChildren<WeaponBase>();
    }

    /// <summary>
    /// �÷��̾��� �ʱ� ���� Idle�� ����
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
    /// ���� ���� UI�� ��� �� ���� �� �����ϴ� �Լ�
    /// </summary>
    private void EndGameSetting()
    {
        isEndGame = true;
        Cursor.visible = true; // ������ �����ϱ� ���� ���콺 Ŀ�� ���̰�
        Cursor.lockState = CursorLockMode.None; // ���콺 Ŀ�� ���� ����
    }

    /// <summary>
    /// �÷��̾ ���콺�� �������� �� ī�޶� ȸ����Ű�� ������ �����ϴ� �Լ�
    /// </summary>
    private void UpdateRotate()
    {
        // ESCŰ�� ������ �� ĳ���� ���� X
        if (isEndGame)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    /// <summary>
    /// ĳ���Ͱ� �����̰ų� ������ �� �� �ϵ��� ���� �Լ�
    /// </summary>
    private void UpdateMove()
    {
        // ESCŰ�� ������ �� ĳ���� ���� X
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
                // ���� ��Ȳ���� �� ��
                {
                    movementCharacterController.MoveSpeed = 0;
                    weapon.Animator.MoveSpeed = 0;

                    // ������ �� ���� ������̸� ����
                    if (audioSource.isPlaying == true)
                    {
                        audioSource.Stop();
                    }
                }
                break;
            case E_PlayerState.Move:
                // �����̴� ��Ȳ���� �� ��
                {
                    // ���� 1ȸ moveMission�� Ŭ���� �Ǿ����� ������ �̼� Ŭ����
                    if (moveMission == false)
                    {
                        Mission_Move();
                    }

                    // ���̳� �ڷ� �̵� �� �޸��� �Ұ���(WalkSpeed����)
                    if (vertical > 0) 
                    {
                        isRun = Input.GetKey(keyCodeRun);
                    }

                    movementCharacterController.MoveSpeed   = isRun == true ? status.RunSpeed : status.WalkSpeed;

                    // ���� 1ȸ dashMission�� Ŭ���� �Ǿ����� ������ �̼� Ŭ����
                    if (dashMission == false)
                    {
                        if (isRun == true)
                        {
                            Mission_Dash();
                        }
                    }

                    weapon.Animator.MoveSpeed               = isRun == true ? 1 : 0.5f;
                    audioSource.clip                        = isRun == true ? audioClipRun : audioClipWalk;

                    // ����Ű �Է� ���δ� �� ������ Ȯ�� -> ��� ���� �� �ٽ� ������� �ʰ� isPlaying���� üũ
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
    /// ���� Ű(Space Bar)�� ������ �� ĳ���͸� ���� ��Ű�� �Լ�
    /// </summary>
    private void UpdateJump()
    {
        // ESCŰ�� ������ �� ĳ���� ���� X
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
    /// ����� ���õ� ���۵��� �����ϴ� �Լ�
    /// </summary>
    private void UpdateWeaponAction()
    {
        // ESCŰ�� ������ �� ĳ���� ���� X
        if (isEndGame)
        {
            return;
        }

        // ���Ⱑ Ȱ��ȭ �Ǿ����� ������ ���� �׼� �Ұ� 
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
    /// �����̴��� �ƴ��� �Ǻ��� State������ �Ǻ��� ��� �� �����ϴ� �Լ�
    /// </summary>
    private void MoveStateCheck(float hor, float ver)
    {
        _state = (hor != 0 || ver != 0) ? E_PlayerState.Move : E_PlayerState.Idle;
    }

    /// <summary>
    /// Move �̼��� Ŭ����� �� ����� �Լ�
    /// </summary>
    private void Mission_Move()
    {
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Start);
        // �Ϸ�� �̼� ó��(�Ϸ� ó��, UI ����)
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Start, startMissionNumber_move); 
            moveMission = true;
        }
    }

    /// <summary>
    /// Dash �̼��� Ŭ����� �� ����� �Լ�
    /// </summary>
    private void Mission_Dash()
    {
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.Start);
        // �Ϸ�� �̼� ó��(�Ϸ� ó��, UI ����)
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.Start, startMissionNumber_dash);
            dashMission = true;
        }
    }

    /// <summary>
    /// �������� �Ծ��� �� ����� �Լ� 
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
    /// ���⸦ �ٲ� �� ȣ��� �Լ�
    /// </summary>
    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
    }

    /// <summary>
    /// ESCŰ�� ������ �� ������ ������ ����� UI�� Ȱ��ȭ �ϱ� ���� �˻��ϴ� �Լ�
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
    /// ���� ���� UI�� Ȱ��ȭ ���� �� ������ �Լ�
    /// </summary>
    public void EndGameSettingReset()
    {
        isEndGame = false;
        Cursor.visible = false; // ������ �����ϱ� ���� ���콺 Ŀ�� ���̰�
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� ���� ����
    }
}
