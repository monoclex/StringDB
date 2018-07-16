using StringDB.Reader;

using System.Collections.Generic;
using System.IO;

namespace StringDB.Writer {

	/// <summary>Some kind of wwriter that writes stuff.</summary>
	public interface IWriter {

		/// <summary>Flushes the prepending data to write to the stream</summary>
		void Flush();

		//  Method |       Mean |    Error |   StdDev |
		//-------- |-----------:|---------:|---------:|
		// ForLoop | 1,190.5 ms | 30.36 ms | 28.40 ms |
		//    Fill |   556.7 ms | 10.68 ms | 10.49 ms |

		/// <summary>Fills up the DB with the index and value specified, 'amt' times. Useful for benchmarking purposes, and faster then a for loop with Insert.</summary>
		void Fill<T1, T2>(T1 index, T2 value, int times);

		/// <summary>Fills up the DB with the index and value specified, 'amt' times. Useful for benchmarking purposes, and faster then a for loop with Insert.</summary>
		void Fill<T1, T2>(TypeHandler<T1> wt1, TypeHandler<T2> wt2, T1 index, T2 value, int times);

		/// <summary>Insert an item into the database</summary>
		void Insert<T1, T2>(T1 index, T2 value);

		/// <summary>Insert an item into the database</summary>
		void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, T1 index, T2 value);

		/// <summary>Insert an item into the database</summary>
		void Insert<T1, T2>(KeyValuePair<T1, T2> kvp);

		/// <summary>Insert an item into the database</summary>
		void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, KeyValuePair<T1, T2> kvp);

		/// <summary>Insert multiple items into the database.</summary>
		void InsertRange<T1, T2>(IEnumerable<KeyValuePair<T1, T2>> items);

		/// <summary>Insert multiple items into the database.</summary>
		void InsertRange<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, IEnumerable<KeyValuePair<T1, T2>> items);

		/// <summary>Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.</summary>
		void OverwriteValue<T>(IReaderPair replacePair, T newValue);

		/// <summary>Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.</summary>
		void OverwriteValue<T>(TypeHandler<T> typeHandler, IReaderPair replacePair, T newValue);
	}

	/// <inheritdoc/>
	public class Writer : IWriter {
		
		internal Writer(IRawWriter rawWriter) => this._rawWriter = rawWriter;

		private readonly IRawWriter _rawWriter;

		/// <inheritdoc/>
		public void Flush()
			=> this._rawWriter.Flush();

		/// <inheritdoc/>
		public void Fill<T1, T2>(T1 index, T2 value, int times)
			=> this.Fill(TypeManager.GetHandlerFor<T1>(), TypeManager.GetHandlerFor<T2>(), index, value, times);

		/// <inheritdoc/>
		public void Fill<T1, T2>(TypeHandler<T1> wt1, TypeHandler<T2> wt2, T1 index, T2 value, int times)
			=> this._rawWriter.InsertRange<T1, T2>(wt1, wt2, FillIEnumerable(index, value, times));

		/// <inheritdoc/>
		public void Insert<T1, T2>(T1 index, T2 value)
			=> this.Insert<T1, T2>(new KeyValuePair<T1, T2>(index, value));

		/// <inheritdoc/>
		public void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, T1 index, T2 value)
			=> this.Insert<T1, T2>(typeHandlerT1, typeHandlerT2, new KeyValuePair<T1, T2>(index, value));

		/// <inheritdoc/>
		public void Insert<T1, T2>(KeyValuePair<T1, T2> kvp)
			=> this.InsertRange<T1, T2>(kvp.AsEnumerable());

		/// <inheritdoc/>
		public void Insert<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, KeyValuePair<T1, T2> kvp)
			=> this.InsertRange<T1, T2>(typeHandlerT1, typeHandlerT2, kvp.AsEnumerable());

		/// <inheritdoc/>
		public void InsertRange<T1, T2>(IEnumerable<KeyValuePair<T1, T2>> items)
			=> this._rawWriter.InsertRange<T1, T2>(TypeManager.GetHandlerFor<T1>(), TypeManager.GetHandlerFor<T2>(), items);

		/// <inheritdoc/>
		public void InsertRange<T1, T2>(TypeHandler<T1> typeHandlerT1, TypeHandler<T2> typeHandlerT2, IEnumerable<KeyValuePair<T1, T2>> items)
			=> this._rawWriter.InsertRange<T1, T2>(typeHandlerT1, typeHandlerT2, items);

		/// <inheritdoc/>
		public void OverwriteValue<T>(IReaderPair replacePair, T newValue)
			=> this.OverwriteValue<T>(TypeManager.GetHandlerFor<T>(), replacePair, newValue);

		/// <inheritdoc/>
		public void OverwriteValue<T>(TypeHandler<T> typeHandler, IReaderPair replacePair, T newValue) {
			var newPos = this._rawWriter.OverwriteValue<T>(typeHandler, newValue, replacePair.Value.GetLength(), replacePair.DataPosition, replacePair.Position + sizeof(byte));

			//TODO: update the readerpair?

			/*
			var repl = replacePair;
			while (replacePair is ThreadSafeReaderPair newRepl)
				repl = newRepl._readerPair;

			if (repl is ReaderPair rp)
				rp._dataPos = newPos;
			*/
		}

		private static IEnumerable<KeyValuePair<T1, T2>> FillIEnumerable<T1, T2>(T1 index, T2 value, int times) {
			var kvp = new KeyValuePair<T1, T2>(index, value);

			for (int i = 0; i < times; i++)
				yield return kvp;
		}
	}
}