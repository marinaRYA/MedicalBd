using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using System.Windows.Shapes;

namespace PresenterLibrary
{
    /// <summary>
    /// Логика взаимодействия для SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public struct RootUser
        {
            public int R;
            public int W;
            public int E;
            public int D;
            public RootUser(int r, int w, int e, int d)
            {
                R = r;
                W = w;
                E = e;
                D = d;
            }
        }
        private string _connectionString;
        private int UserId;

        public SettingWindow(string bd, string username)
        {
            _connectionString = $"Data Source={bd};Version=3;";
            UserId = GetUserIdByUsername(username);
            InitializeComponent();
            LoadUserPermissions();
        }

        private List<string> GetMenuNames()
        {
            return new List<string>
        {
            "Лаборатории",
            "Пациенты",
            "Медицинские учреждения",
            "Отделения",
            "Палаты",
            "Персонал",
            "Корпуса",
            "Доктора",
            "Операции",
            "Учет",
            "Назначения",
            "Специализации",
            "Профили лабораторий",
            "Настройки", 
            "Больницы", 
            "Диагнозы"
        };
        }
        private int GetUserIdByUsername(string username)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT User_ID FROM Users WHERE Username = @Username";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["User_ID"]);
                        }
                        else return -1;
                    }
                }
            }
        }

        private void LoadUserPermissions()
        {
            List<string> menuNames = GetMenuNames();
            for (int i = 0; i < menuNames.Count; i++)
            {
                int menuItemId = GetMenuItemIdByName(menuNames[i]);
                if (menuItemId != -1)
                {
                    RootUser permissions = GetUserPermissions(UserId, menuItemId);
                    
                    SetCheckboxes(menuNames[i], permissions);
                }
            }
        }
       
        private int GetMenuItemIdByName(string menuName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id FROM Menu WHERE MenuText = @MenuName";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MenuName", menuName);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["Id"]);
                        }
                        else return -1;
                    }
                }
            }
        }

        private RootUser GetUserPermissions(int userId, int menuItemId)
        {
            RootUser permissions = new RootUser();
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT R, W, E, D FROM User_Rights WHERE User_ID = @UserId AND Menu_Item_ID = @MenuItemId";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            permissions.R = Convert.ToInt32(reader["R"]);
                            permissions.W = Convert.ToInt32(reader["W"]);
                            permissions.E = Convert.ToInt32(reader["E"]);
                            permissions.D = Convert.ToInt32(reader["D"]);
                        }
                    }
                }
            }
            return permissions;
        }

        private void SetCheckboxes(string menuName, RootUser permissions)
        {
            menuName = menuName.Replace(" ", "");
            CheckBox readCheckbox = FindName($"ReadCheckbox_{menuName}") as CheckBox;
            CheckBox writeCheckbox = FindName($"WriteCheckbox_{menuName}") as CheckBox;
            CheckBox editCheckbox = FindName($"EditCheckbox_{menuName}") as CheckBox;
            CheckBox deleteCheckbox = FindName($"DeleteCheckbox_{menuName}") as CheckBox;
            

            if (readCheckbox != null) readCheckbox.IsChecked = permissions.R == 1;
            if (writeCheckbox != null) writeCheckbox.IsChecked = permissions.W == 1;
            if (editCheckbox != null) editCheckbox.IsChecked = permissions.E == 1;
            if (deleteCheckbox != null) deleteCheckbox.IsChecked = permissions.D == 1;
           
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> menuNames = GetMenuNames();
            for (int i = 0; i < menuNames.Count; i++)
            {
                int menuItemId = GetMenuItemIdByName(menuNames[i]);
                if (menuItemId != -1)
                {
                    RootUser permissions = GetPermissionsFromCheckboxes(menuNames[i]);
                    SaveUserPermissions(UserId, menuItemId, permissions);
                   
                    
                }
            }
            MessageBox.Show("Ваши изменения успешно сохранены!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private int GetPermissionsFromExport(string menuName)
        {
            
            menuName = menuName.Replace(" ", "");
            CheckBox exportCheckbox = FindName($"ExportCheckbox_{menuName}") as CheckBox;
            int expermissions = exportCheckbox?.IsChecked == true ? 1 : 0;
            return expermissions;
        }
        private RootUser GetPermissionsFromCheckboxes(string menuName)
        {
            menuName = menuName.Replace(" ", "");
            CheckBox readCheckbox = FindName($"ReadCheckbox_{menuName}") as CheckBox;
            CheckBox writeCheckbox = FindName($"WriteCheckbox_{menuName}") as CheckBox;
            CheckBox editCheckbox = FindName($"EditCheckbox_{menuName}") as CheckBox;
            CheckBox deleteCheckbox = FindName($"DeleteCheckbox_{menuName}") as CheckBox;
            
            int r = readCheckbox?.IsChecked == true ? 1 : 0;
            int w = writeCheckbox?.IsChecked == true ? 1 : 0;
            int e = editCheckbox?.IsChecked == true ? 1 : 0;
            int d = deleteCheckbox?.IsChecked == true ? 1 : 0;
            
            return new RootUser(r, w, e, d);
        }
        
        private void SaveUserPermissions(int userId, int menuItemId, RootUser permissions)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                UPDATE User_Rights
                SET R = @R, W = @W, E = @E, D = @D
                WHERE User_ID = @UserId AND Menu_Item_ID = @MenuItemId";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@MenuItemId", menuItemId);
                    command.Parameters.AddWithValue("@R", permissions.R);
                    command.Parameters.AddWithValue("@W", permissions.W);
                    command.Parameters.AddWithValue("@E", permissions.E);
                    command.Parameters.AddWithValue("@D", permissions.D);
                    command.ExecuteNonQuery();
                }
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
