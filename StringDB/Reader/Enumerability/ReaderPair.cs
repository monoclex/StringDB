using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Reader {
	/// <summary>A pair of data - this correlates an index to it's corresponding value.</summary>
	public class ReaderPair {
		internal ReaderPair(IPartDataPair dp, IRawReader rawReader) {
			this._dp = dp;
			this._rawReader = rawReader;
		}
		
		internal IPartDataPair _dp { get; }
		private IRawReader _rawReader { get; }

		internal string _indexCache { get; set; } = null;
		internal string _valueCache { get; set; } = null;

		/// <summary>Get the index as a byte array instead.</summary>
		public byte[] IndexAsByteArray => this._dp.Index;

		/// <summary>Whatever the index is.</summary>
		public string Index => this._indexCache ?? (this._indexCache = this._dp.Index.GetString());

		/// <summary>Retrieves the value of the index. This value isn't actually fetched until you call on it, for performance reasons.</summary>
		public string Value => this._valueCache ?? (this._valueCache = (this._dp.ReadData(this._rawReader) ?? new byte[0] { }).GetString());
		
		public override string ToString() =>
			$"[{this.Index}, {this.Value}]";
	}
}