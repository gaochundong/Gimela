using System;
using System.ComponentModel;
using System.ServiceModel;
using Gimela.Crust.Tectosphere;
using Gimela.Common.ExceptionHandling;
using Gimela.Common.Logging;
using Gimela.Infrastructure.AsyncModel;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.ServiceModel.ChannelManagement;

namespace Gimela.Rukbat.GUI.Modules.UserManagement.Models
{
  public class LoginModel : ModelBase
  {
    public void Login(string serverIPAddress, string userName, string password, EventHandler<AsyncWorkerCallbackEventArgs<bool>> callback)
    {
      try
      {
        AsyncWorkerHandle<bool> handle = AsyncWorkerHelper.DoWork<bool>(
          (sender, e) => 
          {
            //LoginRequest request = new LoginRequest()
            //{
            //  UserName = userName,
            //  Password = password
            //};

            //LoginResponse response = MessageSender.Send<ILoginService, LoginResponse, LoginRequest>(
            //  (contract, argument) => 
            //  {
            //    return contract.Login(request);
            //  }, 
            //  request);

            //e.Result = response.LoginResult;
            e.Result = true; // always true for testing
          },
          callback);
      }
      catch (Exception ex)
      {
        ExceptionHandler.Handle(ex);
      }
    }
  }
}
