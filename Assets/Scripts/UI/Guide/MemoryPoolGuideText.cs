using System.Collections.Generic;

public class MemoryPoolGuideText : GuideTextBase
{
    private     List<string>    guideTexts; // 가이드 텍스트를 담을 string List

    /// <summary>
    /// guideTexts 리스트를 생성 후 리스트에 가이드 텍스트들을 추가
    /// </summary>
    private void Awake()
    {
        guideTexts = new List<string>();
        InputText();
    }

    /// <summary>
    /// 리스트에 가이드 텍스트들을 저장하는 InputText함수를 오버로드
    /// </summary>
    public override void InputText()
    {
        guideTexts.Add("There is no limit to bullets here!");
        guideTexts.Add("Memory pooling is applied,\nso attack to your heart’s content!");
        guideTexts.Add("Be careful about noise!");
    }

    /// <summary>
    /// 가이드 텍스트가 저장된 리스트를 반환
    /// </summary>
    public override List<string> GetList()
    {
        return guideTexts;
    }
}
