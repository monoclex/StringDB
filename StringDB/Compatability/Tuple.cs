#if NET20 || NET35
namespace System {
	/// <summary>Custom tuple implementation since it doesn't exist in net 2.0/3.5</summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	public class Tuple<T1, T2> {
		/// <summary>Constructor</summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public Tuple(T1 a, T2 b) {
			this.Item1 = a;
			this.Item2 = b;
		}
		
		/// <summary>Item1</summary>
		public T1 Item1 { get; set; }

		/// <summary>Item2</summary>
		public T2 Item2 { get; set; }
	}
}
#endif