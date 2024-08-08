using System.Collections.Generic;

public class MemoryPoolGuideText : GuideTextBase
{
    private     List<string>    guideTexts; // ���̵� �ؽ�Ʈ�� ���� string List

    /// <summary>
    /// guideTexts ����Ʈ�� ���� �� ����Ʈ�� ���̵� �ؽ�Ʈ���� �߰�
    /// </summary>
    private void Awake()
    {
        guideTexts = new List<string>();
        InputText();
    }

    /// <summary>
    /// ����Ʈ�� ���̵� �ؽ�Ʈ���� �����ϴ� InputText�Լ��� �����ε�
    /// </summary>
    public override void InputText()
    {
        guideTexts.Add("There is no limit to bullets here!");
        guideTexts.Add("Memory pooling is applied,\nso attack to your heart��s content!");
        guideTexts.Add("Be careful about noise!");
    }

    /// <summary>
    /// ���̵� �ؽ�Ʈ�� ����� ����Ʈ�� ��ȯ
    /// </summary>
    public override List<string> GetList()
    {
        return guideTexts;
    }
}
