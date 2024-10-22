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
using PresenterLibrary;
namespace PresenterLibrary.Presenter
{
    /// <summary>
    /// Логика взаимодействия для AddWindowStaff.xaml
    /// </summary>
    public partial class AddWindowStaff : Window
    {
        private string connectionString;
        public StaffBd.Staff Staff { get; set; }

        public AddWindowStaff(StaffBd.Staff staff, string dbConnectionString)
        {
            InitializeComponent();
            connectionString = $"Data Source={dbConnectionString};Version=3;"; ;
                     Staff = staff;

            LoadHospitalNames();
            LoadSpecializationNames();

          
            DisplayStaff();
            
        }

        
        private void LoadHospitalNames()
        {
            List<string> hospitals = new List<string>();
            string query = "SELECT Name FROM MedicalFacility";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            hospitals.Add(reader["Name"].ToString());
                        }
                    }
                }
            }

            cmbHospitalName.ItemsSource = hospitals;
        }

        // Метод для загрузки специализаций
        private void LoadSpecializationNames()
        {
            List<string> specializations = new List<string>();
            string query = "SELECT SpecializationName FROM Specialization";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            specializations.Add(reader["SpecializationName"].ToString());
                        }
                    }
                }
            }

            cmbSpecializationName.ItemsSource = specializations;
        }

       
        private void DisplayStaff()
        {
            
            txtName.Text = Staff.Name;
            txtRole.Text = Staff.Role;

         
            cmbHospitalName.SelectedItem = Staff.HospitalName;
            cmbSpecializationName.SelectedItem = Staff.SpecializationName;
        }

        
        private void btnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text;
            string role = txtRole.Text;
            string hospitalName = cmbHospitalName.SelectedItem?.ToString();
            string specializationName = cmbSpecializationName.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(role) || hospitalName == null || specializationName == null)
            {
                MessageBox.Show("Пожалуйста заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                Staff.Name = name;
                Staff.Role = role;
                Staff.HospitalName = hospitalName;
                Staff.SpecializationName = specializationName;
                DialogResult = true;
                Close();
            }

        }

        
       
    }

}

