using System;
using Gimela.ServiceModel.ChannelManagement.Transports;
using Gimela.Infrastructure.ResourceLocation;

namespace Gimela.ServiceModel.ChannelManagement.Factories
{
  internal static class TransportManagerFactory
  {
    private static readonly object _transporterLocker = new object();

    static TransportManagerFactory()
    {
      Locator.RegisterType<ITransportManager, TransportManager>();
    }

    public static ITransportManager Create()
    {
      ITransportManager transporter = null;

      lock (_transporterLocker)
      {
        transporter = Locator.Resolve<ITransportManager>();
        if (transporter == null)
        {
          throw new InvalidProgramException("Cannot create transport manager.");
        }
      }

      return transporter;
    }
  }
}
