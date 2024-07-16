using EditorExtensions;
using UnityEngine.UIElements;

public class MessageOverlay : VisualElement
{
    private readonly Label messageTitle;
    private readonly Label messageDetail;

    public MessageOverlay()
    {
        this.AddUXMLByName();

        messageTitle = this.Q<Label>("message-title");
        messageDetail = this.Q<Label>("message-detail");

        this.StretchToParentSize();

        Hide();
    }

    public void ShowAndSetText(string title, string detail = "")
    {
        Show();
        SetText(title, detail);
    }

    public void SetText(string title, string detail = "")
    {
        messageTitle.text = title;
        messageDetail.text = detail;
    }

    public void Show()
    {
        RemoveFromClassList("hide");
    }

    public void Hide()
    {
        AddToClassList("hide");

        messageTitle.text = "";
        messageDetail.text = "";
    }
}
