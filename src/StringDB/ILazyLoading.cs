namespace StringDB
{
	public interface ILazyLoading<T>
	{
		T Load();
	}
}