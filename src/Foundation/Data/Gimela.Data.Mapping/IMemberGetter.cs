using System;
using System.Reflection;

namespace Gimela.Data.Mapping
{
	public interface IMemberResolver : IValueResolver
	{
		Type MemberType { get; }
	}

	public interface IMemberGetter : IMemberResolver, ICustomAttributeProvider
	{
		MemberInfo MemberInfo { get; }
		string Name { get; }
		object GetValue(object source);
	}

	public interface IMemberAccessor : IMemberGetter
	{
		void SetValue(object destination, object value);
	}
}