using StringDB.Reader;
using StringDB.Writer;
using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB {
	public static partial class Extensions {
		/// <summary>Fills up the DB with the index and value specified, 'amt' times. Useful for benchmarking purposes, and faster then a for loop with Insert.</summary>
		public static void Fill<T1, T2>(this IWriter writer, T1 index, T2 value, int times)
			=> writer.Fill(TypeManager.GetHandlerFor<T1>(), TypeManager.GetHandlerFor<T2>(), index, value, times);

		/// <summary>Insert an item into the database</summary>
		public static void Insert<T1, T2>(this IWriter writer, T1 index, T2 value)
			=> writer.Insert(TypeManager.GetHandlerFor<T1>(), TypeManager.GetHandlerFor<T2>(), index, value);

		/// <summary>Insert an item into the database</summary>
		public static void Insert<T1, T2>(this IWriter writer, KeyValuePair<T1, T2> kvp)
			=> writer.Insert(TypeManager.GetHandlerFor<T1>(), TypeManager.GetHandlerFor<T2>(), kvp);

		/// <summary>Insert multiple items into the database.</summary>
		public static void InsertRange<T1, T2>(this IWriter writer, IEnumerable<KeyValuePair<T1, T2>> items)
			=> writer.InsertRange(TypeManager.GetHandlerFor<T1>(), TypeManager.GetHandlerFor<T2>(), items);

		/// <summary>Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.</summary>
		public static void OverwriteValue<T>(this IWriter writer, IReaderPair replacePair, T newValue)
			=> writer.OverwriteValue(TypeManager.GetHandlerFor<T>(), replacePair, newValue);
	}
}