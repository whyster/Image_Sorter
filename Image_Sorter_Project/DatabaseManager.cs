using System;
using System.Data;
using System.Data.SQLite;


namespace Image_Sorter_Project {
    public class SorterDatabaseManager : IDisposable {
        private SQLiteConnection _database_connection;
        private string _database_file_location;

        // ReSharper disable once ConvertToAutoProperty
        public string DatabaseLocation => _database_file_location;
        // public readonly string database_schema;

        public SorterDatabaseManager() {
            // database_connection = new SQLiteConnection();

        }

        public SorterDatabaseManager(string database_location) {
            _database_file_location = database_location;
            _database_connection = new SQLiteConnection($"Data Source={database_location};Version=3;");
            _database_connection.Open();
        }

        public void Initialize_Database_With_Schema() {
            string database_schema = @"CREATE TABLE image(
                                    id INTEGER PRIMARY KEY, 
                                    path TEXT UNIQUE, 
                                    name TEXT,
                                    extension TEXT
                                );

                                CREATE TABLE tag(
                                    id INTEGER PRIMARY KEY, 
                                    name TEXT UNIQUE
                                );
                                CREATE TABLE tag_map(
                                    id INTEGER PRIMARY KEY, 
                                    image_id INTEGER, 
                                    tag_id INTEGER, 
                                    FOREIGN KEY(image_id) REFERENCES image(id), 
                                    FOREIGN KEY(tag_id) REFERENCES tag(id)
                                );";
            using SQLiteCommand create_command = new SQLiteCommand(database_schema, _database_connection);
            create_command.ExecuteNonQuery();
        }

        public void Create_Database(string save_location) {
            SQLiteConnection.CreateFile(save_location);
            _database_connection = new SQLiteConnection($"Data Source={save_location};Version=3;");
            _database_connection.Open();
            _database_file_location = save_location;
            Initialize_Database_With_Schema();

        }

        public void Load_Database(string database_location) {
            _database_connection?.Close();
            _database_connection?.Dispose();


            _database_file_location = database_location;
            _database_connection = new SQLiteConnection($"Data Source={database_location};Version=3;");
            _database_connection.Open();
        }

        public void Insert_Image(string path, string title, string extension) {
            string insert_img_statement = $@"
                                    INSERT INTO image(path, name, extension)
                                    VALUES (@image_path, @title, @extension)
                              ";
            try {
                // using SQLiteCommand add_image = new(add_sql, dbConnection);
                using SQLiteCommand add_image = new SQLiteCommand(insert_img_statement, _database_connection);
                add_image.Parameters.Add("@image_path", DbType.String).Value = path;
                add_image.Parameters.Add("@title", DbType.String).Value  = title;
                add_image.Parameters.Add("@extension", DbType.String).Value = extension;
                add_image.ExecuteNonQuery();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }


        private static string Generate_Placeholder_Parameter_List(int count_of_parameters) {
            string ret = "(?";
            for (int i = 1; i < count_of_parameters; i++) {
                ret += ", ?";
            }
            ret += ")";
            
            return ret;
        }
        public SQLiteDataReader Get_Images_By_Extension(String[] extensions) {
            string get_paths = @$"
                                    SELECT path FROM image
                                    WHERE LOWER(image.extension) in {Generate_Placeholder_Parameter_List(extensions.Length)}
                                ";
            
            using SQLiteCommand get_images = new(get_paths, _database_connection);
            // get_images.Parameters.AddRange(extensions);
            foreach (var extension in extensions) {
                get_images.Parameters.AddWithValue(null, extension.ToLower());

            }
            var query_reader = get_images.ExecuteReader();
            return query_reader;
        }

        public bool Get_Connection_Status() {
            if (_database_connection is not null) {
                return true;
            }

            return false;
        }
        public void Dispose() {
            _database_connection?.Close();
            _database_connection?.Dispose();
        }
    }
}