using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
        // private WinForms.FolderBrowserDialog _Folder_Dialog;
        

        // private string database_location;
        private List<string> visible_image_paths;
        private SorterDatabaseManager dbManager;

        public MainWindow() {
            visible_image_paths = new List<string>();
            // _Load_Dialog = new WinForms.OpenFileDialog();


            InitializeComponent();
            dbManager = new SorterDatabaseManager();
        }


        private void Update_Connection_Status() {
            if (dbManager.Get_Connection_Status()) {
                Status_Item.Content = $"Connected to {dbManager.DatabaseLocation}";
            }
            else {
                Status_Item.Content = "Not Connected";
            }
        }

        private void Create_New_Database_Click(object sender, RoutedEventArgs e) {
            Prompt_Path_Create_Database();
            wrap_panel.Children.Clear();
            visible_image_paths = new List<string>();

            Update_Connection_Status();
        }

        private void Prompt_Path_Create_Database() {
            WinForms.SaveFileDialog _Create_File_Dialog = new WinForms.SaveFileDialog();
            _Create_File_Dialog.Title = "Database file location";
            _Create_File_Dialog.Filter = "SQLite Database|*.sqlite";
            _Create_File_Dialog.DefaultExt = "sqlite";
            _Create_File_Dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            WinForms.DialogResult result = _Create_File_Dialog.ShowDialog();

            if (result == WinForms.DialogResult.OK) {
                // Debug.WriteLine(_Create_File_Dialog.FileName);
                // database_location = _Create_File_Dialog.FileName;
                dbManager.Create_Database(_Create_File_Dialog.FileName);
            }
        }


        private void Load_Database_Click(object sender, RoutedEventArgs e) {
            WinForms.OpenFileDialog Load_Dialog = new WinForms.OpenFileDialog();
            Load_Dialog.Title = "Database file location";
            Load_Dialog.Filter = "SQLite Database|*.sqlite";
            Load_Dialog.DefaultExt = "sqlite";
            Load_Dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            WinForms.DialogResult result = Load_Dialog.ShowDialog();
            if (result == WinForms.DialogResult.OK) {
                dbManager.Load_Database(Load_Dialog.FileName);
                Update_Connection_Status();
                return;
            }
            // Todo: Error handling
        }

        private void Import_Images_Click(object sender, RoutedEventArgs e) {
            // TODO: Handle User Interrupts
            Prompt_User_Search_Images();
            Update_UI_With_Images();
        }

        private void Prompt_User_Search_Images() {
            WinForms.FolderBrowserDialog _import_images_dialog = new WinForms.FolderBrowserDialog();
            _import_images_dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            _import_images_dialog.Description = "Image location";

            WinForms.DialogResult result = _import_images_dialog.ShowDialog();

            if (result == WinForms.DialogResult.OK) {
                // Debug.WriteLine(_Create_File_Dialog.FileName);
                // database_location = _Create_File_Dialog.FileName;
                Recursive_Image_Add(_import_images_dialog.SelectedPath);
            }
        }

        private void Recursive_Image_Add(string path) {
            foreach (string directory in Directory.EnumerateDirectories(path)) {
                Recursive_Image_Add(directory);
            }

            foreach (string file in Directory.EnumerateFiles(path)) {
                var temp = file.Split('\\');
                Add_Image_To_Database(file, temp[temp.Length - 1]);
            }
        }

        private void Add_Image_To_Database(string path, string title) {
            dbManager.Insert_Image(path, title, path.Substring(path.LastIndexOf('.')+1));
        }


        private void Create_New_Tag_Click(object sender, RoutedEventArgs e) {
            // var test = new Tag_Dialog
            Tag_Dialog test = new Tag_Dialog();
        }

        private void Add_Tag_To_Database(string name) {
            // TODO: Refactor using Database Manger

            throw new NotImplementedException("Add_Tag_To_Database not implemented");

            // string add_sql = $@"
            //                         INSERT INTO tag(name)
            //                         VALUES ('?')
            //                   ";
            // using SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={database_location}");
            // dbConnection.Open();
            // try {
            //     using SQLiteCommand add_tag = new(add_sql, dbConnection);
            //     add_tag.Parameters.Add(name);
            //     add_tag.ExecuteNonQuery();
            // }
            // catch (Exception e) {
            //     Console.WriteLine(e);
            //     // throw;
            // }
            //
            // dbConnection.Close();
        }


        private void Refresh_Click(object sender, RoutedEventArgs e) {
            Update_UI_With_Images();
        }

        private void Update_UI_With_Images() {
            String[] valid_extensions = new[] {"png", "jpg", "bmp"};
            var reader = dbManager.Get_Images_By_Extension(valid_extensions);
            Add_Image_Thumbnails(reader);
            // throw new NotImplementedException("Update_UI_With_Images not implemented");

            // string get_paths = @"
            //                        SELECT path FROM image
            //                        WHERE LOWER(image.extension) in ('png', 'jpg', 'bmp')
            //                    ";
            //
            // using SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={database_location}");
            // dbConnection.Open();
            // using SQLiteCommand get_images = new(get_paths, dbConnection);
            // var query_reader = get_images.ExecuteReader();
            //
            // Add_Image_Thumbnails(query_reader);
            //
            // dbConnection.Close();
        }

        private void Add_Image_Thumbnails(SQLiteDataReader query_reader) {
            // TODO: Refactor Adding Image Controls with DatabaseManager
            
            // throw new NotImplementedException("Add_Image_Thumbnails not implemented");


            while (query_reader.Read()) {
                string path = query_reader.GetString(0);
                if (visible_image_paths.Contains(path))
                    continue;
            
                Button new_button = new Button();
                new_button.Width = 200;
                new_button.Height = 200;
            
            
                Image new_image = new Image();
                new_image.Width = 200;
                new_image.Height = 200;
                new_image.Stretch = Stretch.Uniform;
                new_image.Source = new BitmapImage(new Uri(path));
                new_button.Content = new_image;
            
                wrap_panel.Children.Add(new_button);
                visible_image_paths.Add(path);
            }
        }
    }
}