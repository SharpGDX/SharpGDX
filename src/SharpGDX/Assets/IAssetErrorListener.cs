namespace SharpGDX.Assets;

public interface IAssetErrorListener
{
	public void Error(IAssetDescriptor asset, Exception throwable);
}