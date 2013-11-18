using System;

namespace Gimela.Data.Mapping
{
	public interface IValueResolver
	{
		ResolutionResult Resolve(ResolutionResult source);
	}
}