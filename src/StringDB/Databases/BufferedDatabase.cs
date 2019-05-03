using JetBrains.Annotations;

using System;
using System.Collections.Generic;

namespace StringDB.Databases
{
	/// <summary>
	/// Buffers writes to a database,
	/// coagulating multiple inserts until the buffer is full.
	/// </summary>
	public class BufferedDatabase<TKey, TValue>
		: BaseDatabase<TKey, TValue>, IDatabaseLayer<TKey, TValue>
	{
		public const int MinimumBufferSize = 16;

		/// <summary>
		/// Creates a new <see cref="BufferedDatabase{TKey, TValue}"/>
		/// with the specified buffer.
		/// </summary>
		/// <param name="bufferSize">The size of the buffer.</param>
		public BufferedDatabase
		(
			[NotNull] IDatabase<TKey, TValue> database,
			int bufferSize = 0x1000
		)
		{
			if (bufferSize < MinimumBufferSize)
			{
				throw new ArgumentException(nameof(bufferSize), $"A buffer smaller than {MinimumBufferSize} is not allowed.");
			}

			InnerDatabase = database;

			_buffer = new KeyValuePair<TKey, TValue>[bufferSize];
			_bufferPos = 0;
		}

		[NotNull] private readonly KeyValuePair<TKey, TValue>[] _buffer;
		private int _bufferPos = 0;

		public IDatabase<TKey, TValue> InnerDatabase { get; }

		/// <summary>
		/// Fills the internal buffer.
		/// </summary>
		/// <param name="fillAmt"></param>
		/// <param name="used">As more of the items in the array are used,</param>
		/// <returns>True if the buffer is filled, false if it is not.</returns>
		private bool FillBuffer([NotNull] KeyValuePair<TKey, TValue>[] fillAmt, ref int used)
		{
			// the amount we can fill,
			// it's either the amount of space remaning in the buffer,
			// or the array length
			var amountCanFill = Math.Min(_buffer.Length - _bufferPos, fillAmt.Length);

			// if we can't fill anything, say that the buffer's full
			if (amountCanFill <= 0)
			{
				return true;
			}

			// an overflow will occur
			var willFill = _bufferPos + fillAmt.Length > _buffer.Length;

			// copy from the src to the buffer
			Array.Copy(fillAmt, used, _buffer, _bufferPos, amountCanFill);

			// increment respective variables
			used += amountCanFill;
			_bufferPos += amountCanFill;

			return willFill;
		}

		/// <summary>
		/// Writes the entire buffer to the db
		/// </summary>
		private void WriteBuffer()
		{
			// if our buffer is full
			if (_bufferPos == _buffer.Length)
			{
				// write the buffer
				InnerDatabase.InsertRange(_buffer);
			}
			else if (_bufferPos == 0)
			{
				return;
			}
			else
			{
				// otherwise, make a temporary array to copy the buffer to
				var array = new KeyValuePair<TKey, TValue>[_bufferPos];

				Array.Copy(_buffer, array, _bufferPos);

				InnerDatabase.InsertRange(array);
			}

			_bufferPos = 0;
		}

		/// <inheritdoc/>
		public override void Dispose()
		{
			WriteBuffer();
			InnerDatabase.Dispose();
		}

		/// <inheritdoc/>
		public override void InsertRange(KeyValuePair<TKey, TValue>[] items)
		{
			int used = 0;

			while (FillBuffer(items, ref used))
			{
				WriteBuffer();
			}
		}

		/// <inheritdoc/>
		protected override IEnumerable<KeyValuePair<TKey, ILazyLoader<TValue>>> Evaluate() => InnerDatabase;
	}
}