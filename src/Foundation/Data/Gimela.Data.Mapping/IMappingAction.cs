namespace Gimela.Data.Mapping
{
	public interface IMappingAction<TSource, TDestination>
	{
		void Process(TSource source, TDestination destination);
	}
}