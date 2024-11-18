namespace SharpGDX;

public partial interface IInput
{
    public interface IKeyboardHeightObserver
    {
        public void OnKeyboardHeightChanged(int height);
    }
}