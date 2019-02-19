namespace StringDB
{
	public interface ITransformer<TPre, TPost>
	{
		TPost Transform(TPre pre);

		TPre Transform(TPost post);
	}
}