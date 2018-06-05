using System;
using System.Collections.Generic;
using System.Text;

namespace StringDB.Reader {
	public interface IReaderChain {
		ulong IndexChain { get; }
		ulong IndexChainWrite { get; }
	}

	public struct ReaderChain : IReaderChain {
		public ReaderChain(ulong indexChain, ulong indexChainWrite) {
			this.IndexChain = indexChain;
			this.IndexChainWrite = indexChainWrite;
		}

		public ulong IndexChain { get; }
		public ulong IndexChainWrite { get; }
	}
}