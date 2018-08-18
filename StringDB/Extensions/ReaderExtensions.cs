using StringDB.Reader;
using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB {
	public static partial class Extensions {
		/// <summary>Gets the ReaderPair responsible for a given index</summary>
		public static IReaderPair Get<T>(this IReader reader, T index)
			=> reader.Get<T>(TypeManager.GetHandlerFor<T>(), index);

		/// <summary>Attempts to get the ReaderPair</summary>
		public static bool TryGet<T>(this IReader reader, T index, out IReaderPair value)
			=> reader.TryGet<T>(TypeManager.GetHandlerFor<T>(), index, out value);

		/// <summary>Gets the multiple ReaderPairs responsible for a given index</summary>
		public static IEnumerable<IReaderPair> GetAll<T>(this IReader reader, T index)
			=> reader.GetAll<T>(TypeManager.GetHandlerFor<T>(), index);
	}
}