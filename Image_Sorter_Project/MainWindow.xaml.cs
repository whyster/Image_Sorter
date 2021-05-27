
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using WinForms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Image_Sorter_Project {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private WinForms.FolderBrowserDialog _Folder_Dialog;
        public MainWindow()
        {
            _Folder_Dialog = new WinForms.FolderBrowserDialog();
            InitializeComponent();

            path_box.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            _Folder_Dialog.RootFolder = Environment.SpecialFolder.MyComputer;
        }

        private void fetch_file_thumbnails(string path) {
            foreach (var entry in Directory.EnumerateFiles(path)) {
                if (entry.Substring(entry.Length - 3).ToLower() != "png" ) 
                    continue;
                
                Debug.WriteLine(entry);
                Image new_image = new Image();
                new_image.MaxWidth = 200;
                new_image.MaxHeight = 200;

                new_image.Width = 200;
                new_image.Height = 200;
                new_image.Stretch = Stretch.Uniform;
                new_image.Source = new BitmapImage(new Uri(entry));
                
                // Button button = new Button();
                // button.Content = entry;
                wrap_panel.Children.Add(new_image);
            }

            
        }

        private void browse_button_Click(object sender, RoutedEventArgs e)
        {
            WinForms.DialogResult result = _Folder_Dialog.ShowDialog();
            if (result == WinForms.DialogResult.OK)
            {
                path_box.Text = _Folder_Dialog.SelectedPath;
            }
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            fetch_file_thumbnails(path_box.Text);
        }
    }
}