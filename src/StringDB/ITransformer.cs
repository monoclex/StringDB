namespace StringDB
{
	public interface ITransformer<TPre, TPost>
	{
		TPost TransformPre(TPre pre);

		TPre TransformPost(TPost post);
	}
}