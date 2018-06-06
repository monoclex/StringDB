namespace StringDB.Reader {
	/// <summary>Shouldn't really be used unless you're implementing an IReader ( of which I ask you why????? )</summary>
	public interface IReaderChain {
		//TODO: make it not bad

		/// <summary>IndexChain</summary>
		ulong IndexChain { get; } /// <summary>IndexChainWrite</summary>
		ulong IndexChainWrite { get; }
	}

	/// <inheritdoc/>
	public struct ReaderChain : IReaderChain {
		/// <summary>Constructor.</summary>
		public ReaderChain(ulong indexChain, ulong indexChainWrite) {
			this.IndexChain = indexChain;
			this.IndexChainWrite = indexChainWrite;
		} /// <inheritdoc/>

		public ulong IndexChain { get; } /// <inheritdoc/>
		public ulong IndexChainWrite { get; }
	}
}