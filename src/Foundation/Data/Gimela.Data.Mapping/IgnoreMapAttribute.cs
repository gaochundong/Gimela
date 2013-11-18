using System;

namespace Gimela.Data.Mapping
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class IgnoreMapAttribute : Attribute
	{
	}
}
