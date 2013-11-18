using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Gimela.Crust;
using Gimela.Infrastructure.Messaging;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.Patterns;
using Gimela.Presentation.Transitions;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.DomainModels.MediaSource.VideoFilters;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Models;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.ViewModels;
using Gimela.Rukbat.GUI.Modules.DeviceConfiguration.Views;
using Gimela.Rukbat.GUI.Modules.UIMessage;

namespace Gimela.Rukbat.GUI.Modules.DeviceConfiguration
{
	public static class DeviceConfigurationViewRouter
	{
		private static readonly object _binder = new object();
		private static readonly Stack<object> _contentStack = new Stack<object>(10);

		public static TransitionElement Presenter { get; private set; }
		
		static DeviceConfigurationViewRouter()
		{
			Messenger.Default.Register<NotificationMessage<MediaService>>(_binder, message =>
			{
				if (message.Notification == UIMessageType.DeviceConfiguration_ServiceSelectedEvent)
				{
					ViewModelLocator.SelectedService = message.Content as MediaService;

					CameraManagementView view = new CameraManagementView();
					view.DataContext = ViewModelLocator.CameraManagement;
					DeployNewView(view);
				}
			});

			Messenger.Default.Register<NotificationMessage<Camera>>(_binder, message =>
			{
				if (message.Notification == UIMessageType.DeviceConfiguration_UpdateCameraEvent)
				{
					CameraCreationView view = new CameraCreationView();
					CameraUpdateViewModel vm = ViewModelLocator.CameraUpdate;
					vm.SetObject(message.Content as Camera);
					view.DataContext = vm;
					DeployNewView(view);
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_CameraCreatedEvent)
				{
					DeployLastView();
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_CameraUpdatedEvent)
				{
					DeployLastView();
				}
			});

			Messenger.Default.Register<NotificationMessage<IList<CameraFilter>>>(_binder, message =>
			{
				if (message.Notification == UIMessageType.DeviceConfiguration_SelectLocalCameraVideoSourceEvent)
				{
					VideoSourceLocalCameraView view = new VideoSourceLocalCameraView();
					LocalCameraVideoSourceViewModel vm = ViewModelLocator.LocalCameraVideoSource;
					vm.VideoFilterCollection.Clear();
					foreach (var item in message.Content)
					{
						vm.VideoFilterCollection.Add(item);
					}
					view.DataContext = vm;
					DeployNewView(view);
				}
			});

			Messenger.Default.Register<NotificationMessage<IList<DesktopFilter>>>(_binder, message =>
			{
				if (message.Notification == UIMessageType.DeviceConfiguration_SelectLocalDesktopVideoSourceEvent)
				{
					VideoSourceLocalDesktopView view = new VideoSourceLocalDesktopView();
					LocalDesktopVideoSourceViewModel vm = ViewModelLocator.LocalDesktopVideoSource;
					vm.VideoFilterCollection.Clear();
					foreach (var item in message.Content)
					{
						vm.VideoFilterCollection.Add(item);
					}
					view.DataContext = vm;
					DeployNewView(view);
				}
			});

			Messenger.Default.Register<NotificationMessage>(_binder, message =>
			{
				if (message.Notification == UIMessageType.DeviceConfiguration_SelectServiceEvent)
				{
					DeployLastView();
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_CreateCameraEvent)
				{
					CameraCreationView view = new CameraCreationView();
					view.DataContext = ViewModelLocator.CameraCreation;
					DeployNewView(view);
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_CancelCameraEvent)
				{
					DeployLastView();
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_CancelNavigateVideoSourceEvent)
				{
					DeployLastView();
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_CancelUpdateVideoSourceEvent)
				{
					DeployLastView();
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_SelectLocalAVIFileVideoSourceEvent)
				{
					VideoSourceLocalAVIFileView view = new VideoSourceLocalAVIFileView();
					view.DataContext = ViewModelLocator.LocalAVIFileVideoSource;
					DeployNewView(view);
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_SelectNetworkJPEGVideoSourceEvent)
				{
					VideoSourceNetworkJPEGView view = new VideoSourceNetworkJPEGView();
					view.DataContext = ViewModelLocator.NetworkJPEGVideoSource;
					DeployNewView(view);
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_SelectNetworkMJPEGVideoSourceEvent)
				{
					VideoSourceNetworkMJPEGView view = new VideoSourceNetworkMJPEGView();
					view.DataContext = ViewModelLocator.NetworkMJPEGVideoSource;
					DeployNewView(view);
				}
			});

			Messenger.Default.Register<MultipleContentNotificationMessage<IList<CameraFilter>, VideoSourceDescription>>(_binder, message =>
			{
				if (message.Notification == UIMessageType.DeviceConfiguration_UpdateLocalCameraVideoSourceEvent)
				{
					VideoSourceLocalCameraView view = new VideoSourceLocalCameraView();
					LocalCameraVideoSourceViewModel vm = ViewModelLocator.LocalCameraVideoSource;
					vm.VideoFilterCollection.Clear();
					foreach (var item in message.FirstContent)
					{
						vm.VideoFilterCollection.Add(item);
					}
					vm.SetObject(message.SecondContent as VideoSourceDescription);
					view.DataContext = vm;
					DeployNewView(view);
				}
			});

			Messenger.Default.Register<MultipleContentNotificationMessage<IList<DesktopFilter>, VideoSourceDescription>>(_binder, message =>
			{
				if (message.Notification == UIMessageType.DeviceConfiguration_UpdateLocalDesktopVideoSourceEvent)
				{
					VideoSourceLocalDesktopView view = new VideoSourceLocalDesktopView();
					LocalDesktopVideoSourceViewModel vm = ViewModelLocator.LocalDesktopVideoSource;
					vm.VideoFilterCollection.Clear();
					foreach (var item in message.FirstContent)
					{
						vm.VideoFilterCollection.Add(item);
					}
					vm.SetObject(message.SecondContent as VideoSourceDescription);
					view.DataContext = vm;
					DeployNewView(view);
				}
			});

			Messenger.Default.Register<NotificationMessage<VideoSourceDescription>>(_binder, message =>
			{
				if (message.Notification == UIMessageType.DeviceConfiguration_NavigateVideoSourceEvent)
				{
					VideoSourceNavigationView view = new VideoSourceNavigationView();
					view.DataContext = ViewModelLocator.VideoSourceNavigation;
					DeployNewView(view);
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_VideoSourceCreateSelectedEvent)
				{
					DeployBeforeLastView();
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_VideoSourceUpdateSelectedEvent)
				{
					DeployLastView();
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_UpdateLocalAVIFileVideoSourceEvent)
				{
					VideoSourceLocalAVIFileView view = new VideoSourceLocalAVIFileView();
					LocalAVIFileVideoSourceViewModel vm = ViewModelLocator.LocalAVIFileVideoSource;
					vm.SetObject(message.Content as VideoSourceDescription);
					view.DataContext = vm;
					DeployNewView(view);
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_UpdateNetworkJPEGVideoSourceEvent)
				{
					VideoSourceNetworkJPEGView view = new VideoSourceNetworkJPEGView();
					NetworkJPEGVideoSourceViewModel vm = ViewModelLocator.NetworkJPEGVideoSource;
					vm.SetObject(message.Content as VideoSourceDescription);
					view.DataContext = vm;
					DeployNewView(view);
				}
				else if (message.Notification == UIMessageType.DeviceConfiguration_UpdateNetworkMJPEGVideoSourceEvent)
				{
					VideoSourceNetworkMJPEGView view = new VideoSourceNetworkMJPEGView();
					NetworkMJPEGVideoSourceViewModel vm = ViewModelLocator.NetworkMJPEGVideoSource;
					vm.SetObject(message.Content as VideoSourceDescription);
					view.DataContext = vm;
					DeployNewView(view);
				}
			});
		}

		public static void SetEntry(Panel container)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			TransitionElement presenter = new TransitionElement();
			container.Children.Add(presenter);
			Presenter = presenter;

			ServiceSelectionView view = new ServiceSelectionView();
			view.DataContext = ViewModelLocator.ServiceSelection;
			DeployNewView(view);
		}

		private static void DeployNewView(object view)
		{
			if (view == null)
				throw new ArgumentNullException("view");

			DispatcherHelper.InvokeOnUI(() =>
			{
				_contentStack.Push(view);

				if (Presenter.Content != null) (Presenter.Content as UIElement).Visibility = Visibility.Hidden;

				Presenter.Transition = GetDeployNewViewTransition();
				Presenter.Content = view;
			});
		}

		private static void DeployLastView()
		{
			DispatcherHelper.InvokeOnUI(() =>
			{
				object currentView = _contentStack.Pop();
				(currentView as UIElement).Visibility = Visibility.Hidden;

				object lastView = _contentStack.Peek();
				(lastView as UIElement).Visibility = Visibility.Visible;

				Presenter.Transition = GetDeployLastViewTransition();
				Presenter.Content = lastView;
				((IViewModelResponsive)((lastView as UserControl).DataContext)).Refresh();
			});
		}

		private static void DeployBeforeLastView()
		{
			DispatcherHelper.InvokeOnUI(() =>
			{
				object currentView = _contentStack.Pop();
				(currentView as UIElement).Visibility = Visibility.Hidden;

				object lastView = _contentStack.Pop();
				(lastView as UIElement).Visibility = Visibility.Hidden;

				object beforeLastView = _contentStack.Peek();
				(beforeLastView as UIElement).Visibility = Visibility.Visible;

				Presenter.Transition = GetDeployLastViewTransition();
				Presenter.Content = beforeLastView;
			});
		}

		private static Transition GetDeployNewViewTransition()
		{
			Transition trans = new TranslateTransition()
			{
				IsNewContentTopmost = true,
				Duration = new Duration(TimeSpan.FromSeconds(0.3)),
				StartPoint = new Point(-1, 0),
				EndPoint = new Point(0, 0)
			};

			return trans;
		}

		private static Transition GetDeployLastViewTransition()
		{
			Transition trans = new TranslateTransition()
			{
				IsNewContentTopmost = true,
				Duration = new Duration(TimeSpan.FromSeconds(0.3)),
				StartPoint = new Point(1, 0),
				EndPoint = new Point(0, 0)
			};

			return trans;
		}
	}
}
