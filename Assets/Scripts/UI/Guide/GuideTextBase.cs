using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���̵� �ؽ�Ʈ�� ������ Ŭ�������� ��ӹ޴� �߻� Ŭ����
/// </summary>
public abstract class GuideTextBase : MonoBehaviour
{
    /// <summary>
    /// ���̵� �ؽ�Ʈ�� ������ Ŭ�������� �����ε��� �߻� �޼ҵ��
    /// </summary>
    public abstract void InputText();
    public abstract List<string> GetList();
}
