namespace SharpGDX.Input
{
    public interface ITextInputWrapper
    {

        string GetText();

        int GetSelectionStart();

        int GetSelectionEnd();

        void SetText(string text);

        void SetPosition(int position);

        bool ShouldClose();
    }
}
