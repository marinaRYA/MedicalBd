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
    /// Логика взаимодействия для ChoiceUser.xaml
    /// </summary>
    public partial class ChoiceUser : Window
    {
        public string username;
        private string ConnectionString;
        public ChoiceUser(string bd)
        {
            ConnectionString =bd;
            
            InitializeComponent();
            Load();

        }
        private void Load()
        {
            List<string> userNames = new List<string>();
            string query = "SELECT Username FROM Users;";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string Name = reader["username"] as string;
                            if (!string.IsNullOrWhiteSpace(Name))
                            {
                                userNames.Add(Name);
                            }
                        }
                    }
                }
            }

            ProfileComboBox.ItemsSource = userNames;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                username = ProfileComboBox.SelectedItem as string;
                DialogResult = true;
            }
        }
    }
}
