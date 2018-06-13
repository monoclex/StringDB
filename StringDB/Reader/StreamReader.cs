using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Reader {
	/// <inheritdoc/>
	public class StreamReader : IReader {

		/// <summary>Create a new StreamReader.</summary>
		/// <param name="stream">The stream to read.</param>
		/// <param name="dbv">The database version to read from.</param>
		/// <param name="leaveOpen">Whether or not the stream is to be disposed after using it.</param>
		public StreamReader(System.IO.Stream stream, DatabaseVersion dbv, bool leaveOpen) {
			this._stream = stream ?? throw new ArgumentNullException(nameof(stream));
			this._dbv = dbv;
			this._leaveOpen = leaveOpen;

#if NET20 || NET35 || NET40
			this._br = new System.IO.BinaryReader(this._stream, Encoding.UTF8);
#else
			this._br = new System.IO.BinaryReader(this._stream, Encoding.UTF8, leaveOpen);
#endif
		}

		private System.IO.Stream _stream;
		private System.IO.BinaryReader _br;
		private DatabaseVersion _dbv;
		private bool _leaveOpen;

		/// <inheritdoc/>
		public IReaderInteraction FirstIndex() => Empty() ? null : ReadOn(null, false); /// <inheritdoc/>
		public IReaderInteraction LastIndex() => Empty() ? null : ReadOnWithRequirements(null, new ReaderRequirements(false, null, false, 0, false, 0, false, 0, true)); /// <inheritdoc/>

		public byte[] GetDirectValueOf(ulong dataPos) => ReadValueAsByteArray(new ReaderInteraction(null, dataPos, 0, 0)); /// <inheritdoc/>

		public byte[][] GetIndexes() => GetIndexesOfDupeValues(GetDupeValues(new ReaderRequirements(false)))?.ToArray(); /// <inheritdoc/>

		public ulong GetOverhead() => StringDBOverheadCount(); /// <inheritdoc/>

		public bool Empty() => this._stream.Length == 0; /// <inheritdoc/>

		public IReaderChain GetReaderChain() => GetIndexChain(); /// <inheritdoc/>

		public byte[] GetValueOf(IReaderInteraction r, bool doSeek = false) => ReadValueAsByteArray(ReadOnWithRequirements((doSeek ? r ?? throw new ArgumentNullException(nameof(r)) : null), new ReaderRequirements(true, (r ?? throw new ArgumentNullException(nameof(r))).Index))); /// <inheritdoc/>
		public byte[] GetValueOf(string index, bool doSeek = false, ulong quickSeek = 0) => GetValueOf(new ReaderInteraction(index, 0, quickSeek, 0), doSeek); /// <inheritdoc/>

		public byte[][] GetValuesOf(IReaderInteraction r, bool doSeek = false) => GetValuesOfDupeValues(GetDupeValues(new ReaderRequirements(true, (r == null ? throw new ArgumentNullException(nameof(r)) : r).Index ?? throw new ArgumentNullException(nameof(r.Index)), doSeek, (r ?? throw new ArgumentNullException(nameof(r))).QuickSeek)))?.ToArray(); /// <inheritdoc/>
		public byte[][] GetValuesOf(string index, bool doSeek = false, ulong quickSeek = 0) => GetValuesOf(new ReaderInteraction(index ?? throw new ArgumentNullException(nameof(index)), 0, (doSeek ? quickSeek : 0))); /// <inheritdoc/>

		public IReaderInteraction IndexAfter(IReaderInteraction r, bool doSeek = false) => ReadOnWithRequirements(r ?? throw new ArgumentNullException(nameof(r)), new ReaderRequirements(!doSeek, r.Index ?? throw new ArgumentNullException(nameof(r.Index)), false, r.QuickSeek, false, 0, true, 1)); /// <inheritdoc/>
		public IReaderInteraction IndexAfter(string index, bool doSeek = false, ulong quickSeek = 0) => IndexAfter(new ReaderInteraction(index, 0, (doSeek ? quickSeek : 0)), doSeek); /// <inheritdoc/>

		public bool IsIndexAfter(IReaderInteraction r, bool doSeek = false) => null != IndexAfter(r, doSeek); /// <inheritdoc/>
		public bool IsIndexAfter(string index, bool doSeek = false, ulong quickSeek = 0) => IsIndexAfter(new ReaderInteraction(index, 0, quickSeek, 0), doSeek); /// <inheritdoc/>

		public IEnumerator<ReaderPair> GetEnumerator() => new ReaderEnumerator(this, this.FirstIndex()); /// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => new ReaderEnumerator(this, this.FirstIndex());

		//TODO: improve order and such with all of these functions

		private IReaderChain GetIndexChain() {
			var li = this.LastIndex();

			if (li == null)
				return null;

			return ReadIndexChain(null);
		}

		private IReaderInteraction ReadOnWithRequirements(IReaderInteraction lastIndex, IReaderRequirements requirements) {
			if (this.Empty()) return null;
			if (lastIndex == null) lastIndex = ReaderInteraction.Null;
			if (requirements == null) throw new ArgumentNullException(nameof(requirements));
			if(requirements.RequireIndexMatch) if (requirements.Index == null) throw new ArgumentNullException(nameof(requirements.Index));

			var quit = false;
			var lastInteraction = lastIndex;
			var interaction = lastIndex;
			var adv = 0uL;
			var metIndex = false;
			var first = false;

			while (!quit) { //as long as reading the next one isn't null and we're not quitting
				lastInteraction = interaction;

				if (!first)
					interaction = ReadOn(interaction, (first = true) == false);
				else interaction = ReadOn(interaction, true);

				var canQuit = true;

				//for auto seeking to a specific position since we know quickseek isn't set properly
				if (!metIndex)
					if (requirements.RequireIndexMatch)
						if (interaction.Index == requirements.Index) { //match requirements
							metIndex = true;
							quit = true;
							adv = 0; //reset counter
						} else canQuit = false;

				if (requirements.RequireDataPositionMatch)
					if (interaction.DataPosition == requirements.DataPosition)
						quit = true;
					else canQuit = false;

				if (requirements.RequireQuickSeekMatch)
					if (interaction.QuickSeek == requirements.QuickSeek)
						quit = true;
					else canQuit = false;

				if (requirements.RequireAdvances)
					if (adv >= ((ReaderRequirements)requirements).Advances)
						quit = true;
					else canQuit = false;

				if (requirements.RequireLast)
					if (interaction == null)
						return lastInteraction;

				if (!canQuit) //make sure every requirement was met
					quit = false;

				if (quit)
					break;

				if (interaction == null)
					return null;

				adv++;
			}

			return interaction; //if it's null we'll return null
		}

		private List<byte[]> GetValuesOfDupeValues(List<IReaderInteraction> items) {
			if (items == null)
				return null;

			if (items.Count == 0)
				return null;


			var res = new List<byte[]>();

			foreach (var i in items)
				res.Add(ReadValueAsByteArray(i));

			if (res.Count == 0)
				return null;

			return res;
		}

		private List<byte[]> GetIndexesOfDupeValues(List<IReaderInteraction> items) {
			if (items == null) return null;
			if (items.Count == 0) return null;

			var res = new List<byte[]>();

			//TODO: make i.Index return a byte[] natively

			foreach (var i in items)
				res.Add(Encoding.UTF8.GetBytes(i.Index));

			if (res.Count == 0)
				return null;

			return res;
		}

		private ulong StringDBOverheadCount() {
			if (this.Empty())
				return 0uL;

			ulong counter = 0;
			var lastIndexChain = 0u;

			foreach (var i in this) {
				if(i._indexchainPassTimes > lastIndexChain) {
					lastIndexChain = i._indexchainPassTimes;
					counter += 9uL;
				}

				counter += 9uL;

				this._stream.Seek((long)i._dataPos.DataPosition, System.IO.SeekOrigin.Begin);

				var b = this._br.ReadByte();

				switch (b) {
					case Consts.IsByteValue:
					counter += 2uL;
					break;

					case Consts.IsUShortValue:
					counter += 3uL;
					break;

					case Consts.IsUIntValue:
					counter += 5uL;
					break;

					case Consts.IsULongValue:
					counter += 9uL;
					break;
				}
			}
			
			counter += 9uL;

			return counter;
		}

		private List<IReaderInteraction> GetDupeValues(IReaderRequirements requirements) {
			if (this.Empty()) return null;

			var res = new List<IReaderInteraction>();
			
			var interaction = ReaderInteraction.Null;
			var first = false;

			while (interaction != null) { //as long as reading the next one isn't null and we're not quitting
				if (!first)
					interaction = ReadOn(interaction, (first = true) == false);
				else interaction = ReadOn(interaction, true);

				if (interaction == null)
					break;

				//for auto seeking to a specific position since we know quickseek isn't set properly
				if (requirements.RequireIndexMatch) {
					if (interaction.Index == requirements.Index) //match requirements
						res.Add(interaction);
				} else res.Add(interaction);
			}

			if (res.Count == 0)
				return null;

			return res; //if it's null we'll return null
		}

		/// <summary>Read the next IReaderInteraction after this part</summary>
		/// <param name="lastIndex">The index to read after</param>
		/// <param name="leapForward">If we should be leaping forward</param>
		/// <returns>The next index after this one</returns>
		private IReaderInteraction ReadOn(IReaderInteraction lastIndex, bool leapForward = true) {
			if (this.Empty()) return null; //prevent stream length errors

			var iri = lastIndex;
			if (iri == null)
				iri = ReaderInteraction.Null; //defaults

			var leap = (leapForward ? 1L + 8L + (long)iri.Index.Length : 0);

			//seek to the next one ( hance Read*On* )
			this._stream.Seek((long)iri.QuickSeek + leap, System.IO.SeekOrigin.Begin); //seek to position

			var currIndex = (ReaderInteraction)iri;

			var quickSeek = (ulong)this._stream.Position;
			var b = this._br.ReadByte();
			var passedIndexChain = ((ReaderInteraction)iri).IndexChainPassedAmount;

			while (b == Consts.IndexSeperator) { //if we come across an index seperator
				passedIndexChain++;

				var seekTo = this._br.ReadUInt64(); //jump to the next index chain

				if (seekTo == 0)
					return null; //approached the end of the file

				this._stream.Seek((long)seekTo, System.IO.SeekOrigin.Begin);

				quickSeek = seekTo;
				b = this._br.ReadByte();
			}

			var dataPos = this._br.ReadUInt64();
			var name = this._br.ReadBytes((int)b);
			return new ReaderInteraction(Database.GetString(name), dataPos, quickSeek, passedIndexChain);
		}

		/// <summary>Read the next IReaderInteraction after this part</summary>
		/// <param name="lastIndex">The index to read after</param>
		/// <param name="leapForward">If we should be leaping forward</param>
		/// <returns>The next index after this one</returns>
		private IReaderChain ReadIndexChain(IReaderInteraction lastIndex, bool leapForward = false) {
			if (this.Empty()) return null; //prevent stream length errors

			var iri = lastIndex;
			if (iri == null)
				iri = ReaderInteraction.Null; //defaults

			var leap = (leapForward ? 1L + 8L + (long)iri.Index.Length : 0);

			//seek to the next one ( hance Read*On* )
			this._stream.Seek((long)iri.QuickSeek + leap, System.IO.SeekOrigin.Begin); //seek to position

			var currIndex = (ReaderInteraction)iri;

			var quickSeek = (ulong)this._stream.Position;
			byte b;
			var passedIndexChain = ((ReaderInteraction)iri).IndexChainPassedAmount;

			var indexChain = this._stream.Length;

			while (true) {
				b = this._br.ReadByte();

				while (b == Consts.IndexSeperator) { //if we come across an index seperator
					passedIndexChain++;

					var seekTo = this._br.ReadUInt64(); //jump to the next index chain

					if (seekTo == 0)
						return new ReaderChain((ulong)indexChain, (ulong)this._stream.Position - 8uL); //approached the end of the file

					this._stream.Seek((long)seekTo, System.IO.SeekOrigin.Begin);

					quickSeek = seekTo;
					//indexChain = seekTo;
					b = this._br.ReadByte();
				}

				var dataPos = this._br.ReadUInt64();
				var name = this._br.ReadBytes((int)b);
			}
		}

		private byte[] ReadValueAsByteArray(IReaderInteraction index) =>
			(index == null ?
				throw new ArgumentNullException(nameof(index))
				: this._br.ReadBytes((int)ReadNumber(index)));

		private string ReadValueAsString(IReaderInteraction index) =>
			Database.GetString(ReadValueAsByteArray(index));

		private void ReadValueAsStream(IReaderInteraction index, System.IO.Stream outputDataTo) {
			if (index == null) return;

			var len = ReadNumber(index);
			var bufferLen = 4096;
			
			if (len < (ulong)bufferLen)
				bufferLen = (int)len;

			var buffer = new byte[bufferLen];
			while(len > (ulong)bufferLen) {
				buffer = this._br.ReadBytes(bufferLen);
				outputDataTo.Write(buffer, 0, buffer.Length);
				len -= (ulong)bufferLen;
			}

			buffer = this._br.ReadBytes(bufferLen);
			outputDataTo.Write(buffer, 0, bufferLen);
		}

		private ulong ReadNumber(IReaderInteraction index) {
			if (this._stream.Length == 0) return 0; //prevent stream length errors

			this._stream.Seek((long)index.DataPosition, System.IO.SeekOrigin.Begin);

			var b = this._br.ReadByte();

			switch(b) {
				case Consts.IsByteValue:
				return this._br.ReadByte();
				case Consts.IsUShortValue:
				return this._br.ReadUInt16();
				case Consts.IsUIntValue:
				return this._br.ReadUInt32();
				case Consts.IsULongValue:
				return this._br.ReadUInt64();
			}
			
			throw new Exception("Unable to read number.");
		}


		
		private bool disposedValue = false; /// <inheritdoc/>

		protected virtual void Dispose(bool disposing) {
			if (!this.disposedValue) {
				if (disposing && !this._leaveOpen) {
					this._stream.Dispose();

#if NET20 || NET35 || NET40
					((IDisposable)this._br).Dispose();
#else
					this._br.Dispose();
#endif
				}
				
				this.disposedValue = true;
			}
		} /// <inheritdoc/>

		public void Dispose() =>
			this.Dispose(true);
	}

	/// <summary>Requirements for a seeking</summary>
	public interface IReaderRequirements : IReaderInteraction {
		/// <summary>If the indexes need to match</summary>
		bool RequireIndexMatch { get; }

		/// <summary>If the QuickSeeks match</summary>
		bool RequireQuickSeekMatch { get; }

		/// <summary>If the DataPosition matches</summary>
		bool RequireDataPositionMatch { get; }

		/// <summary>If we should require it to advance a specific number of times</summary>
		bool RequireAdvances { get; }

		/// <summary>If we need to go to the last index</summary>
		bool RequireLast { get; }
	}

	/// <inheritdoc/>
	public struct ReaderRequirements : IReaderRequirements {
		/// <summary>Create some requirements</summary>
		/// <param name="IndexRequired">If an index is required</param>
		/// <param name="Index">The index to use</param>
		/// <param name="QuickSeekRequired">If a quickseek is required</param>
		/// <param name="QuickSeek">The quickseek</param>
		/// <param name="DataPositionRequired">If the data position is required</param>
		/// <param name="DataPosition">The data position</param>
		/// <param name="LastIndex">The last index is needed</param>
		public ReaderRequirements(bool IndexRequired = false, string Index = null, bool QuickSeekRequired = false, ulong QuickSeek = 0, bool DataPositionRequired = false, ulong DataPosition = 0, bool Advances = false, ulong AdvanceAmount = 0, bool LastIndex = false) {
			this.RequireIndexMatch = IndexRequired;
			this.Index = Index;

			this.RequireQuickSeekMatch = QuickSeekRequired;
			this.QuickSeek = QuickSeek;

			this.RequireDataPositionMatch = DataPositionRequired;
			this.DataPosition = DataPosition;

			this.RequireAdvances = Advances;
			this.Advances = AdvanceAmount;

			this.RequireLast = LastIndex;
		}

		/// <inheritdoc/>
		public bool RequireIndexMatch { get; } /// <inheritdoc/>
		public bool RequireQuickSeekMatch { get; } /// <inheritdoc/>
		public bool RequireDataPositionMatch { get; } /// <inheritdoc/>
		public bool RequireAdvances { get; } /// <inheritdoc/>
		public bool RequireLast { get; } /// <inheritdoc/>
		public string Index { get; } /// <inheritdoc/>
		public ulong QuickSeek { get; } /// <inheritdoc/>
		public ulong DataPosition { get; } /// <inheritdoc/>
		public ulong Advances { get; }
	}
}