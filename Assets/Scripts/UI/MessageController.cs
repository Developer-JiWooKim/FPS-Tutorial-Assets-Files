using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private GameObject          message;                // �޼��� ������Ʈ
    [SerializeField]
    private TextMeshProUGUI     messageText;            // �޼��� �ؽ�Ʈ

    private bool                isMessageUse = false;   // �޼����� ������� �ִ��� �˻�

    private float               outMessagePos = 450f;   // �޼����� �ٱ��� ���� �� ��ġ
    private float               inMessagePos = 0f;      // �޼����� ���ʿ� ���� �� ��ġ


    private Queue<string>       messages;               // �޼������� ���� ť

    /// <summary>
    /// �޼��� ť�� ����
    /// </summary>
    private void Start()
    {
        messages = new Queue<string>();
    }

    private void Update()
    {
        UpdateMessage();
    }

    /// <summary>
    /// �޼����� ����� ���� ��� �˻��ϰ�, �޼����� ����� �Ǹ� OnMessageEvent�ڷ�ƾ �Լ� ����
    /// </summary>
    private void UpdateMessage()
    {
        // �޼����� �����(�ٸ� �޼����� ���� ������) ����
        if (isMessageUse == true)
        {
            return;
        }

        // ť�� �޼����� ������ ����
        if (messages.Count == 0)
        {
            return;
        }

        if (messages.Count > 0)
        {
            messageText.text = messages.Dequeue();
        }
        StartCoroutine("OnMessageEvent");
    }

    /// <summary>
    /// �޼����� �������� �ϴ� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator OnMessageEvent()
    {
        yield return StartCoroutine("MessageMoveLeft");
        yield return new WaitForSeconds(3.5f);
        yield return StartCoroutine("MessageMoveRight");
    }

    /// <summary>
    /// ȭ�� �ۿ� �ִ� �޼��� ������Ʈ�� ȭ�鿡 ������(�������� �̵�)�ϴ� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator MessageMoveLeft()
    {
        float current = 0f;
        float percent = 0f;
        float time = 0.5f;

        isMessageUse = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            message.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(outMessagePos, inMessagePos, percent), 0);
            yield return null;
        }
    }

    /// <summary>
    /// �޼��� ������Ʈ�� �ٽ� ȭ�� ������(���������� �̵�) �̵���Ű�� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator MessageMoveRight() // ���������� �̵�
    {
        float current = 0f;
        float percent = 0f;
        float time = 0.5f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            message.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(inMessagePos, outMessagePos, percent), 0);
            yield return null;
        }
        isMessageUse = false;
    }

    /// <summary>
    /// ť�� �޼����� �����ϴ� �Լ�
    /// </summary>
    public void MessageSave(string _message)
    {
        messages.Enqueue(_message);
    }
}