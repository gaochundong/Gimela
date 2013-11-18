using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Gimela.Crust;
using Gimela.Crust.Tectosphere;
using Gimela.Infrastructure.Messaging;
using Gimela.Infrastructure.Patterns;
using Gimela.Presentation.Transitions;
using Gimela.Rukbat.DomainModels;
using Gimela.Rukbat.DomainModels.MediaSource;
using Gimela.Rukbat.DomainModels.MediaSource.VideoFilters;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Entities;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Models;
using Gimela.Rukbat.GUI.Modules.PublishMedia.ViewModels;
using Gimela.Rukbat.GUI.Modules.PublishMedia.Views;
using Gimela.Rukbat.GUI.Modules.UIMessage;

namespace Gimela.Rukbat.GUI.Modules.PublishMedia
{
	public static class PublishMediaViewRouter
	{
		private static readonly object _binder = new object();
		private static readonly Stack<object> _contentStack = new Stack<object>(10);

		public static TransitionElement Presenter { get; private set; }
		
		static PublishMediaViewRouter()
		{
      Messenger.Default.Register<NotificationMessage>(_binder, message =>
      {
        if (message.Notification == UIMessageType.PublishMedia_CreatePublishedCameraEvent)
        {
          CameraListView view = new CameraListView();
          view.DataContext = new CameraListViewModel(Singleton<CameraModel>.Instance);
          DeployNewView(view);
        }
        else if (message.Notification == UIMessageType.PublishMedia_CancelSelectCameraEvent)
        {
          DeployLastView();
        }
        else if (message.Notification == UIMessageType.PublishMedia_CancelSelectServiceEvent)
        {
          DeployLastView();
        }
        else if (message.Notification == UIMessageType.PublishMedia_CancelConfigCameraEvent)
        {
          DeployLastView();
        }
        else if (message.Notification == UIMessageType.PublishMedia_CameraPublishedEvent)
        {
          DeployLastView();
          DeployLastView();
          DeployLastView();
        }
      });

      Messenger.Default.Register<NotificationMessage<Camera>>(_binder, message =>
      {
        if (message.Notification == UIMessageType.PublishMedia_CameraSelectedEvent)
        {
          PublishServiceSelectionView view = new PublishServiceSelectionView();
          view.DataContext = new PublishServiceSelectionViewModel(
            Singleton<PublishServiceModel>.Instance,
            message.Content as Camera);
          DeployNewView(view);
        }
      });

      Messenger.Default.Register<NotificationMessage<PublishPair>>(_binder, message =>
      {
        if (message.Notification == UIMessageType.PublishMedia_ServiceSelectedEvent)
        {
          PublishedCameraConfigurationView view = new PublishedCameraConfigurationView();
          view.DataContext = new PublishedCameraConfigurationViewModel(
            Singleton<PublishedCameraModel>.Instance,
            (message.Content as PublishPair).Service,
            (message.Content as PublishPair).Camera);
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

      PublishedCameraManagementView view = new PublishedCameraManagementView();
      view.DataContext = new PublishedCameraManagementViewModel(Singleton<PublishedCameraModel>.Instance);
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
