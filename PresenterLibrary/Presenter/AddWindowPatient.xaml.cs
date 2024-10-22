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
using static PresenterLibrary.ClassBD.PatientBd;

namespace PresenterLibrary.Presenter
{
    /// <summary>
    /// Логика взаимодействия для AddWindowPatient.xaml
    /// </summary>
    public partial class AddWindowPatient : Window
    {
        public Patient Patient { get; private set; }
        private string _connectionString;

        public AddWindowPatient(Patient patient, string connectionString)
        {
            InitializeComponent();
            Patient = patient;
            _connectionString = $"Data Source={connectionString};Version=3;";

            LoadClinics();
            LoadHospitals();
            LoadDoctors();
            LoadGender();

            DisplayPatientDetails();
        }

        private void LoadClinics()
        {
            List<string> clinics = new List<string>();
            string query = "SELECT DISTINCT Name FROM MedicalFacility WHERE Type = 'Поликлиника';";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clinics.Add(reader["Name"].ToString());
                        }
                    }
                }
            }

            ClinicComboBox.ItemsSource = clinics;
        }

        private void LoadHospitals()
        {
            List<string> hospitals = new List<string>();
            string query = "SELECT DISTINCT Name FROM MedicalFacility WHERE Type = 'Больница';";


            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
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

            HospitalComboBox.ItemsSource = hospitals;
        }
        private void LoadGender()
        {
            List<string> genders = new List<string> { "Женщина", "Мужчина"};


            GenderComboBox.ItemsSource = genders;
        }
        private void LoadDoctors()
        {
            List<string> doctors = new List<string>();
            string query = "SELECT Name FROM Doctor";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            doctors.Add(reader["Name"].ToString());
                        }
                    }
                }
            }

            DoctorComboBox.ItemsSource = doctors;
        }

        

        private void DisplayPatientDetails()
        {
            if (Patient != null)
            {
                NameTextBox.Text = Patient.Name;
                BirthDatePicker.SelectedDate = Patient.BirthDate;
                AddressTextBox.Text = Patient.Address;
                ClinicComboBox.SelectedItem = Patient.ClinicName;
                HospitalComboBox.SelectedItem = Patient.HospitalName;
                DoctorComboBox.SelectedItem = Patient.AttendingDoctorName;
                GenderComboBox.SelectedItem = Patient.Gender;
                PolisTextBox.Text = Patient.Polis;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                    BirthDatePicker.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(AddressTextBox.Text) ||
                    DoctorComboBox.SelectedItem == null ||
                    GenderComboBox.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(PolisTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Patient.Name = NameTextBox.Text;
                Patient.BirthDate = BirthDatePicker.SelectedDate.Value;
                Patient.Address = AddressTextBox.Text;
                Patient.ClinicName = ClinicComboBox.SelectedItem as string;
                Patient.HospitalName = HospitalComboBox.SelectedItem as string;
                Patient.AttendingDoctorName = DoctorComboBox.SelectedItem as string;
                Patient.Gender = GenderComboBox.SelectedItem as string;
                Patient.Polis = PolisTextBox.Text;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
