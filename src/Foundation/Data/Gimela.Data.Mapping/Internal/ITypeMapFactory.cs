using System;

namespace Gimela.Data.Mapping
{
    public interface ITypeMapFactory
    {
        TypeMap CreateTypeMap(Type sourceType, Type destinationType, IMappingOptions mappingOptions, MemberList memberList);
    }
}