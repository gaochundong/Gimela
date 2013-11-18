using System;
using System.Collections.Generic;

namespace Gimela.Data.Mapping.Configuration
{
    public class MapperConfiguration
    {
        public MapperConfiguration(Action<MapperRegistry> initializationExpression)
        {
            var registry = new MapperRegistry();

            initializationExpression(registry);
        }

        public IEnumerable<TypeMapConfiguration> TypeMaps
        {
            get { return new TypeMapConfiguration[0]; }
        }
    }
}