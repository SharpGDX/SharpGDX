namespace SharpGDX.Graphics.G3D.Environments;

public abstract class BaseLight//<T>
// TODO: Why?? where T: BaseLight<T>
{
	public readonly Color color = new Color(0, 0, 0, 1);

    public void setColor(float r, float g, float b, float a)
    {
//public T setColor (float r, float g, float b, float a) {
        this.color.Set(r, g, b, a);
		//return (T)this;
	}

    public void setColor(Color color)
    {
//public T setColor (Color color) {
        this.color.Set(color);
		//return (T)this;
	}
}
