
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
using System.Data.SQLite;
namespace Image_Sorter_Project {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        // private WinForms.FolderBrowserDialog _Folder_Dialog;
        private WinForms.OpenFileDialog _Load_Dialog;
        public MainWindow() {
            
            _Load_Dialog = new WinForms.OpenFileDialog();
            // _Folder_Dialog = new WinForms.FolderBrowserDialog();
            InitializeComponent();

            // path_box.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\data.db";
            // _Folder_Dialog.RootFolder = Environment.SpecialFolder.MyComputer;
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
                
                wrap_panel.Children.Add(new_image);
            }

            
        }

        private void Create_Database_File(string path) {
            string file_path = path + "database.sqlite";
            SQLiteConnection.CreateFile(file_path);
            
            
        }

        private void Load_Click(object sender, RoutedEventArgs e) {

            _Load_Dialog.DefaultExt = "sqlite";
            _Load_Dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            WinForms.DialogResult result = _Load_Dialog.ShowDialog();
            // WinForms.DialogResult result = _Folder_Dialog.ShowDialog();
            if (result == WinForms.DialogResult.OK) {
                // fetch_file_thumbnails(_Load_Dialog.FileName);
            }
        }
    }
}