using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
namespace ImageViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FolderBrowserDialog fd = new FolderBrowserDialog();
        private ObservableCollection<MyImageClass> _myImages;
        public ObservableCollection<MyImageClass> MyImages
        {
            get { return _myImages; }
            set
            {
                _myImages = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            MyImages = new ObservableCollection<MyImageClass>();
            FillImages();
            lstview.ItemsSource = MyImages;
            txtPath.Text = fd.SelectedPath;
            txtsize.Text = lstview.Items.Count.ToString() + " items";
        }

        private void txtsearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                SearchText();
            }
        }

        public void FillImages()
        {
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string file in Directory.GetFiles(fd.SelectedPath))
                {
                    if (file.EndsWith(".png") || file.EndsWith(".jpg"))
                    {
                        _myImages.Add(new MyImageClass(System.IO.Path.GetFileNameWithoutExtension(file), GetImageFromResourceString(file)));
                    }
                }
            }
        }

        public BitmapImage GetImageFromResourceString(string imageName)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(imageName);
            image.EndInit();
            return image;
        }
        private void lstview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;
            while (obj != null && obj != lstview)
            {
                if (obj.GetType() == typeof(ListBoxItem))
                {
                    foreach (var img in MyImages)
                    {
                        if (lstview.SelectedItem.Equals(img))
                        {
                            Process.Start(img.Image.ToString());
                        }
                    }
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            if (obj.GetType() != typeof(ListBoxItem))
            {
                SearchText();
            }
        }

        private void lstview_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewer1.ScrollToVerticalOffset(scrollViewer1.VerticalOffset - e.Delta);
        }

        public void SearchText()
        {
            if (string.IsNullOrEmpty(txtsearch.Text) == false)
            {
                ObservableCollection<MyImageClass> tmpimages = new ObservableCollection<MyImageClass>();
                foreach (var str in MyImages)
                {
                    if (str.Title.StartsWith(txtsearch.Text, StringComparison.InvariantCultureIgnoreCase))
                    {
                            tmpimages.Add(str);
                    }
                }
                lstview.ItemsSource = tmpimages;
                txtsize.Text = lstview.Items.Count.ToString() + " items";
            }
            else if (txtsearch.Text == "")
            {
                lstview.ItemsSource = MyImages;
                txtsize.Text = lstview.Items.Count.ToString() + " items";
            }
        }
    }

    public class MyImageClass
    {
        public string Title { get; set; }

        public ImageSource Image { get; set; }

        public MyImageClass(string title, ImageSource image)
        {
            this.Title = title;
            this.Image = image;
        }
    }

    public class TruncateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            if (parameter == null)
                return value;
            int _MaxLength;
            if (!int.TryParse(parameter.ToString(), out _MaxLength))
                return value;
            var _String = value.ToString();
            if (_String.Length > _MaxLength)
                _String = _String.Substring(0, _MaxLength) + "...";
            return _String;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
