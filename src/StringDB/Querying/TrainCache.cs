namespace StringDB.Querying
{
	public class TrainCache<T>
	{
		public object Lock;
		public int Accessors;
		public T Item;
	}
}