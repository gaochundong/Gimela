using System;
using Gimela.Common.ExceptionHandling;
using Gimela.ServiceModel.ManagedDiscovery;
using Gimela.ServiceModel.ManagedService.Attributes;
using Gimela.ServiceModel.ManagedService.Contracts.DataContracts;
using Gimela.ServiceModel.ManagedService.Contracts.MessageContracts;

namespace Gimela.ServiceModel.ManagedService
{
  /// <summary>
  /// 托管的服务基类
  /// </summary>
  [ManagedServiceAttribute]
  public abstract class ManagedServiceBase : DiscoverableServiceBase, IManagedService
  {
    #region Properties

    /// <summary>
    /// 服务状态
    /// </summary>
    public ServiceState State { get; private set; }

    #endregion

    #region IManagedService Members

    /// <summary>
    /// 获取托管服务状态
    /// </summary>
    /// <returns>
    /// 托管服务状态响应
    /// </returns>
    public ServiceStateResponse GetState()
    {
      return new ServiceStateResponse() { State = this.State };
    }

    /// <summary>
    /// 启动托管服务
    /// </summary>
    /// <returns>
    /// 托管服务状态响应
    /// </returns>
    public ServiceStateResponse Start()
    {
      ServiceStateResponse response = new ServiceStateResponse() { State = ServiceState.Unknown };

      try
      {
        State = ServiceState.Starting;
        OnStart();
        State = ServiceState.Started;
        response.State = this.State;
      }
      catch (Exception ex)
      {
        State = ServiceState.StartFailed;
        response.State = this.State;
        response.StateMessage = ex.Message;
        ExceptionHandler.Handle(ex);
      }

      return response;
    }

    /// <summary>
    /// 停止托管服务
    /// </summary>
    /// <returns>
    /// 托管服务状态响应
    /// </returns>
    public ServiceStateResponse Stop()
    {
      ServiceStateResponse response = new ServiceStateResponse() { State = ServiceState.Unknown };

      try
      {
        State = ServiceState.Stopping;
        OnStop();
        State = ServiceState.Stopped;
        response.State = this.State;
      }
      catch (Exception ex)
      {
        State = ServiceState.StopFailed;
        response.State = this.State;
        response.StateMessage = ex.Message;
        ExceptionHandler.Handle(ex);
      }

      return response;
    }

    /// <summary>
    /// 重启托管服务
    /// </summary>
    /// <returns>
    /// 托管服务状态响应
    /// </returns>
    public ServiceStateResponse Restart()
    {
      ServiceStateResponse response = new ServiceStateResponse() { State = ServiceState.Unknown };

      State = ServiceState.Restarting;
      response = Stop();
      response = Start();

      return response;
    }

    #endregion

    #region IHeartBeat Members

    /// <summary>
    /// Ping服务
    /// </summary>
    /// <param name="request">Ping请求消息</param>
    /// <returns>
    /// Ping响应消息
    /// </returns>
    public PingResponse Ping(PingRequest request)
    {
      PingResponse response = new PingResponse() { Status = HeartBeatStatus.Unknown };

      try
      {
        response = OnPing();
      }
      catch (Exception ex)
      {
        response.Status = HeartBeatStatus.Sick;
        response.StatusMessage = ex.Message;
        ExceptionHandler.Handle(ex);
      }

      return response;
    }

    #endregion

    #region Virtual Methods

    /// <summary>
    /// 当服务启动时发生
    /// </summary>
    protected virtual void OnStart()
    {
    }

    /// <summary>
    /// 当服务停止时发生
    /// </summary>
    protected virtual void OnStop()
    {
    }

    /// <summary>
    /// 当接收到Ping时发生
    /// </summary>
    /// <returns></returns>
    protected virtual PingResponse OnPing()
    {
      return new PingResponse() { Status = HeartBeatStatus.Healthy };
    }

    #endregion
  }
}
