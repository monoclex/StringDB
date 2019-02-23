namespace StringDB.Tests
{
	public class LazyItem<T> : ILazyLoading<T>
	{
		public LazyItem(T value) => Value = value;

		public T Value { get; }

		public bool Loaded { get; private set; }

		public T Load()
		{
			Loaded = true;
			return Value;
		}

		public override string ToString() => $"[{Value}: {Loaded}]";
	}
}