using StringDB.Databases;

namespace StringDB.Fluency
{
	public static class TransformDatabaseExtensions
	{
		public static IDatabase<TPostKey, TPostValue> WithTransform<TPreKey, TPreValue, TPostKey, TPostValue>
		(
			this IDatabase<TPreKey, TPreValue> database,
			ITransformer<TPreKey, TPostKey> keyTransformer,
			ITransformer<TPreValue, TPostValue> valueTransformer
		)
			=> new TransformDatabase<TPreKey, TPreValue, TPostKey, TPostValue>
			(
				database,
				keyTransformer,
				valueTransformer
			);
	}
}