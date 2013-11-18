using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Gimela.Presentation.Transitions
{
    /// <summary>
    /// Interaction logic for TransitionDemo.xaml
    /// </summary>
    public partial class TransitionDemo : UserControl
    {
        public TransitionDemo()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, object e)
        {
            string[] pictures = Helpers.GetPicturePaths();
            PropertyInfo[] props = typeof(Brushes).GetProperties();

            List<object> data = new List<object>(17);

            data.Add(new UI());

            for (int i = 0; i < Math.Min(pictures.Length, 8); i++)
            {
                data.Add(new Picture(pictures[i]));
            }

            for (int i = 0; i < Math.Min(props.Length, 8); i++)
            {
                if (props[i].Name != "Transparent")
                {
                    data.Add(new Swatch(props[i].Name));
                }
            }

            this.DataContext = _data.ItemsSource = data;


            // Setup 2 way transitions
            Transition[] transitions = (Transition[])FindResource("ForwardBackTransitions");

            for (int i = 0; i < transitions.Length; i += 2)
            {
                ListTransitionSelector selector = new ListTransitionSelector(transitions[i], transitions[i + 1], data);
                TextSearch.SetText(selector, TextSearch.GetText(transitions[i]));
                _selectors.Items.Add(selector);
            }
        }

        private void OnMouseLeftDown(object s, MouseEventArgs e)
        {
            _data.SelectedIndex = (_data.SelectedIndex + 1) % _data.Items.Count;
            _data.ScrollIntoView(_data.SelectedItem);
        }

        private void OnMouseRightDown(object s, MouseEventArgs e)
        {
            _data.SelectedIndex = (_data.SelectedIndex + _data.Items.Count - 1) % _data.Items.Count;
            _data.ScrollIntoView(_data.SelectedItem);
        }

        private void OnModeChanged(object s, object e)
        {
            if (!_twoWay.IsSelected) _selectors.SelectedIndex = -1;
        }
    }

    internal class ListTransitionSelector : TransitionSelector
    {
        public ListTransitionSelector(Transition forward, Transition backward, IList list)
        {
            _forward = forward;
            _backward = backward;
            _list = list;
        }

        public override Transition SelectTransition(object oldContent, object newContent)
        {
            int oldIndex = _list.IndexOf(oldContent);
            int newIndex = _list.IndexOf(newContent);
            return newIndex > oldIndex ? _forward : _backward;
        }

        private Transition _forward, _backward;
        private IList _list;
    }

    internal class UI
    {
    }

    internal class Picture
    {
        public Picture(string uri)
        {
            _uri = uri;
        }
        private string _uri;
        public string Uri
        {
            get { return _uri; }
        }
    }
    internal class Swatch
    {
        public Swatch(string colorName)
        {
            _colorName = colorName;
        }
        private string _colorName;
        public string ColorName
        {
            get { return _colorName; }
        }
    }

    public class IsStringEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            XmlElement str = value as XmlElement;
            if (str != null)
            {
                XmlAttribute attribute = str.Attributes["Description"];
                if (attribute != null)
                {
                    string foo = attribute.Value;
                    if (foo != null && foo.Length > 0)
                    {
                        if (targetType == typeof(Visibility))
                        {
                            return Visibility.Visible;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            if (targetType == typeof(Visibility))
            {
                return Visibility.Collapsed;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public static class Helpers
    {
        public static string[] GetPicturePaths()
        {
            string[] picturePaths = new string[0];

            string[] commandLineArgs = null;
            try
            {
                commandLineArgs = Environment.GetCommandLineArgs();
            }
            catch (NotSupportedException) { }


            if (commandLineArgs != null)
            {
                foreach (string arg in commandLineArgs)
                {
                    picturePaths = GetPicturePaths(arg);
                    if (picturePaths.Length > 0)
                    {
                        break;
                    }
                }
            }

            if (picturePaths.Length == 0)
            {
                picturePaths = GetPicturePaths(DefaultPicturePath);
            }

            return picturePaths;
        }
        internal static string[] GetPicturePaths(string sourceDirectory)
        {

            if (!string.IsNullOrEmpty(sourceDirectory))
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(sourceDirectory);
                    if (di.Exists)
                    {
                        FileInfo[] fis = di.GetFiles(DefaultImageSearchPattern);
                        string[] strings = new string[fis.Length];
                        for (int i = 0; i < fis.Length; i++)
                        {
                            strings[i] = fis[i].FullName;
                        }
                        return strings;
                    }
                }
                catch (IOException) { }
                catch (ArgumentException) { }
                catch (SecurityException) { }
            }
            return new string[0];
        }
        public static BitmapImage[] GetBitmapImages(int maxCount)
        {
            string[] imagePaths = GetPicturePaths();
            if (maxCount < 0)
            {
                maxCount = imagePaths.Length;
            }

            BitmapImage[] images = new BitmapImage[Math.Min(imagePaths.Length, maxCount)];
            for (int i = 0; i < images.Length; i++)
            {
                images[i] = new BitmapImage(new Uri(imagePaths[i]));
            }
            return images;
        }


        public const string DefaultPicturePath = @"C:\Users\Public\Pictures\Sample Pictures\";
        public const string DefaultImageSearchPattern = @"*.jpg";
    }

    public class ImagePathHolder
    {
        public static string[] ImagePaths
        {
            get
            {
                return Helpers.GetPicturePaths();
            }
        }
        public static BitmapImage[] BitmapImages
        {
            get
            {
                return Helpers.GetBitmapImages(-1);
            }
        }
        public static BitmapImage[] BitmapImages6
        {
            get
            {
                return Helpers.GetBitmapImages(6);
            }
        }

    }
}
