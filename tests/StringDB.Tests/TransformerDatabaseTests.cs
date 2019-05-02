using FluentAssertions;

using StringDB.Databases;
using StringDB.Transformers;

using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace StringDB.Tests
{
	/// <summary>
	/// Tests for a <see cref="TransformDatabase{TPreKey, TPreValue, TPostKey, TPostValue}"/>
	/// </summary>
	public class TransformerDatabaseTests
	{
		private readonly MemoryDatabase<string, int> _memoryDatabase;
		private readonly MockTransformer _keyTransformer;
		private readonly MockTransformer _valueTransformer;
		private readonly TransformDatabase<string, int, int, string> _transformDatabase;
		private readonly KeyValuePair<int, string>[] _values;
		private readonly KeyValuePair<string, int>[] _expectValues;

		public TransformerDatabaseTests()
		{
			_memoryDatabase = new MemoryDatabase<string, int>();
			_keyTransformer = new MockTransformer();
			_valueTransformer = new MockTransformer();

			_transformDatabase = new TransformDatabase<string, int, int, string>
			(
				db: _memoryDatabase,
				keyTransformer: _keyTransformer.Reverse(),
				valueTransformer: _valueTransformer
			);

			_values = new[]
			{
				KeyValuePair.Create(1, "a"),
				KeyValuePair.Create(2, "b"),
				KeyValuePair.Create(3, "c"),
			};

			_expectValues = _values
				 .Select
				 (
					 x => KeyValuePair.Create
					 (
						 _keyTransformer.TransformPre(x.Key),
						 _valueTransformer.TransformPost(x.Value)
					 )
				 )
				 .ToArray();
		}

		/// <summary>
		/// Ensures that when we insert a range of entries, it transforms the entries back.
		/// </summary>
		[Fact]
		public void InsertRange()
		{
			_transformDatabase.InsertRange(_values);

			// now the memory db should have the transformed values in them
			_memoryDatabase.EnumerateAggressively(3)
				.Should()
				.BeEquivalentTo(_expectValues, "Inserting values into a transform database should insert the transformed values into the underlying database.");
		}

		/// <summary>
		/// Enumerates over the transform database - tests integration between the memdb & tdb
		/// </summary>
		[Fact]
		public void Enumerate()
		{
			_transformDatabase.InsertRange(_values);

			// make sure enumeration is fine
			_transformDatabase.EnumerateAggressively(3)
				.Should()
				.BeEquivalentTo(_values);
		}
	}
}