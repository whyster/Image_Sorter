
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
        private string database_location;
        private List<string> visible_image_paths;

        public MainWindow() {
            visible_image_paths = new List<string>();
            _Load_Dialog = new WinForms.OpenFileDialog();

            // _Folder_Dialog = new WinForms.FolderBrowserDialog();
            InitializeComponent();

            // path_box.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\data.db";
            // _Folder_Dialog.RootFolder = Environment.SpecialFolder.MyComputer;
        }

        private void Create_New_Database_Click(object sender, RoutedEventArgs e) {
            Initialize_Database();
            wrap_panel.Children.Clear();
            visible_image_paths = new List<string>();
        }
        
        private void Initialize_Database() {
            WinForms.SaveFileDialog _Create_File_Dialog = new WinForms.SaveFileDialog();
            _Create_File_Dialog.Title = "Database file location";
            _Create_File_Dialog.Filter = "SQLite Database|*.sqlite";
            _Create_File_Dialog.DefaultExt = "sqlite";
            _Create_File_Dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            WinForms.DialogResult result = _Create_File_Dialog.ShowDialog();
            
            if (result == WinForms.DialogResult.OK) {
                // Debug.WriteLine(_Create_File_Dialog.FileName);
                // database_location = _Create_File_Dialog.FileName;
                Create_Database_File(_Create_File_Dialog.FileName);
            }
            
        }
        private void Create_Database_File(string path) {
            string file_path = path;
            Debug.WriteLine($"Creating file at {path}");
            
            SQLiteConnection.CreateFile(file_path);
            SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={file_path};Version=3;");
            Debug.WriteLine($"File created, Connection achieved");
            
            string create_image = @"
                                      CREATE TABLE image
                                      (
                                          id INTEGER PRIMARY KEY, 
                                          path TEXT UNIQUE, 
                                          name TEXT,
                                          extension TEXT
                                      );
                                  ";
            string create_tag = @"
                                    CREATE TABLE tag
                                    (
                                        id INTEGER PRIMARY KEY, 
                                        name TEXT UNIQUE
                                    );
                                ";
            string create_tag_map = @"
                                        CREATE TABLE tag_map(
                                            id INTEGER PRIMARY KEY, 
                                            image_id INTEGER, 
                                            tag_id INTEGER, 
                                            FOREIGN KEY(image_id) REFERENCES image(id), 
                                            FOREIGN KEY(tag_id) REFERENCES tag(id)
                                        );
                                    ";
            
            using (dbConnection) {
                dbConnection.Open();
                using (SQLiteCommand image_command = new(create_image, dbConnection)) { image_command.ExecuteNonQuery(); }
                using (SQLiteCommand tag_command = new(create_tag, dbConnection)) { tag_command.ExecuteNonQuery(); }
                using (SQLiteCommand tag_map_command = new(create_tag_map, dbConnection)) { tag_map_command.ExecuteNonQuery(); }
            }

            database_location = file_path;
            Status_Item.Content = $"Connected to {database_location}";
        }
        
        
        private void Load_Database_Click(object sender, RoutedEventArgs e) {
            
            _Load_Dialog.DefaultExt = "sqlite";
            _Load_Dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            WinForms.DialogResult result = _Load_Dialog.ShowDialog();
            // WinForms.DialogResult result = _Folder_Dialog.ShowDialog();
            if (result == WinForms.DialogResult.OK) {
                // fetch_file_thumbnails(_Load_Dialog.FileName);
                database_location = _Load_Dialog.FileName;
                Status_Item.Content = $"Connected to {database_location}";
                Add_Valid_Images();
            }
            // database_location = path
        }
        private void Import_Images_Click(object sender, RoutedEventArgs e) {

            Prompt_User_Search_Images();
            Add_Valid_Images();
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
                Add_Image_To_Database(file, temp[temp.Length-1]);
            }
        }
        private void Add_Image_To_Database(string path, string title) {
            var temp = path.Split('.');
            string extension = temp[temp.Length - 1];
            string add_sql = $@"
                                    INSERT INTO image(path, name, extension)
                                    VALUES ('{path}', '{title}', '{extension}')
                              ";
            using SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={database_location}");
            dbConnection.Open();
            try {
                using SQLiteCommand add_image = new(add_sql, dbConnection);
                add_image.ExecuteNonQuery();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                // throw;
            }
            dbConnection.Close();

        }
        
        
        private void Create_New_Tag_Click(object sender, RoutedEventArgs e)
        {
            
        } 
        
        private void Add_Tag_To_Database(string name) {
            string add_sql = $@"
                                    INSERT INTO tag(name)
                                    VALUES ('{name}')
                              "; 
            using SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={database_location}");
            dbConnection.Open();
            try {
                using SQLiteCommand add_tag = new(add_sql, dbConnection);
                add_tag.ExecuteNonQuery();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                // throw;
            }
            dbConnection.Close();
        }
        
        
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Add_Valid_Images();
        }
        
        private void Add_Valid_Images() {
            string get_paths = @"
                                   SELECT path FROM image
                                   WHERE LOWER(image.extension) in ('png', 'jpg', 'bmp')
                               ";
            
            using SQLiteConnection dbConnection = new SQLiteConnection($"Data Source={database_location}");
            dbConnection.Open();
            using SQLiteCommand get_images = new(get_paths, dbConnection);
            var query_reader = get_images.ExecuteReader();
            
            Add_Image_Thumbnails(query_reader);
            
            dbConnection.Close();
        }
        private void Add_Image_Thumbnails(SQLiteDataReader query_reader) {
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