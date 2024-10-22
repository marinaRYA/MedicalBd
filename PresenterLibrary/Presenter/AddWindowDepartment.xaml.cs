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
using static DepartmentBd;
using static WardBd;

namespace PresenterLibrary.Presenter
{
    /// <summary>
    /// Логика взаимодействия для AddWindowDepartment.xaml
    /// </summary>
    public partial class AddWindowDepartment : Window
    {
        public Department Department { get; private set; }

        private string _connectionString;

        public AddWindowDepartment(Department dept, string bd)
        {
            Department = dept;
            InitializeComponent();
            _connectionString = $"Data Source={bd};Version=3;";

            LoadBuildings();
            DisplayWardDetails();
        }

        private void LoadBuildings()
        {
            List<string> buildings = new List<string>();

            string query = "SELECT Building_Name FROM Building";
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string buildingName = reader["Building_Name"] as string;
                            if (!string.IsNullOrWhiteSpace(buildingName))
                            {
                                buildings.Add(buildingName);
                            }
                        }
                    }
                }
            }

            BuildingComboBox.ItemsSource = buildings; // Assuming there is a BuildingComboBox in the XAML
        }
        private void DisplayWardDetails()
        {
            if (Department != null)
            {
                BuildingComboBox.SelectedItem = Department.BuildingName;
                NameTextBox.Text = Department.Name;
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    BuildingComboBox.SelectedItem == null)
                 //   string.IsNullOrWhiteSpace(SpecializationTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Assign values to Department object
                Department.Name = NameTextBox.Text;
                Department.BuildingName = BuildingComboBox.SelectedItem as string;
               // Department.Specialization = SpecializationTextBox.Text;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
