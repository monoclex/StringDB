#if NET20 || NET35
namespace System {
	public class Tuple<T1, T2> {
		public Tuple(T1 a, T2 b) {
			this.Item1 = a;
			this.Item2 = b;
		}
		
		public T1 Item1 { get; set; }
		public T2 Item2 { get; set; }
	}
}
#endif