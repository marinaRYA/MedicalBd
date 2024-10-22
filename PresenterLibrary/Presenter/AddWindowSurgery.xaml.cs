using PresenterLibrary.ClassBD;
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

namespace PresenterLibrary.Presenter
{
    /// <summary>
    /// Логика взаимодействия для AddWindowSurgery.xaml
    /// </summary>
    public partial class AddWindowSurgery : Window
    {
        public SurgeryBd.Surgery Surgery;
        private MedicalFacilityBd medicalBd;
        private string ConnectionString;

        public AddWindowSurgery(SurgeryBd.Surgery surgery, string bd)
        {
            InitializeComponent();
            medicalBd = new MedicalFacilityBd(bd);
            Surgery = surgery ?? new SurgeryBd.Surgery();
            ConnectionString = $"Data Source={bd};Version=3;";
            LoadParentHospitals();
            LoadPatients();
            LoadDoctors();
            DisplaySurgeryDetails();
        }

        private void DisplaySurgeryDetails()
        {
            if (Surgery != null)
            {
                NameTextBox.Text = Surgery.Name;
                PatientNameComboBox.SelectedItem = Surgery.PatientName;
                DoctorNameComboBox.SelectedItem = Surgery.DoctorName;
                HospitalNameComboBox.SelectedItem = Surgery.HospitalName;
                DateTextBox.SelectedDate = Surgery.Date;
                OutcomeTextBox.Text = Surgery.Outcome;
            }
        }

        private void LoadParentHospitals()
        {
            List<string> hospitalNames = new List<string>();
            string query = "SELECT DISTINCT Name FROM MedicalFacility WHERE Type = 'Больница';";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string hospitalName = reader["Name"] as string;
                            if (!string.IsNullOrWhiteSpace(hospitalName))
                            {
                                hospitalNames.Add(hospitalName);
                            }
                        }
                    }
                }
            }

            HospitalNameComboBox.ItemsSource = hospitalNames;
        }

        private void LoadPatients()
        {
            List<string> patientNames = new List<string>();
            string query = "SELECT DISTINCT Name FROM Patient;";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string patientName = reader["Name"] as string;
                            if (!string.IsNullOrWhiteSpace(patientName))
                            {
                                patientNames.Add(patientName);
                            }
                        }
                    }
                }
            }

            PatientNameComboBox.ItemsSource = patientNames;
        }

        private void LoadDoctors()
        {
            List<string> doctorNames = new List<string>();
            string query = "SELECT DISTINCT Name FROM Doctor;";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string doctorName = reader["Name"] as string;
                            if (!string.IsNullOrWhiteSpace(doctorName))
                            {
                                doctorNames.Add(doctorName);
                            }
                        }
                    }
                }
            }

            DoctorNameComboBox.ItemsSource = doctorNames;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PatientNameComboBox.SelectedItem as string) ||
                    string.IsNullOrWhiteSpace(DoctorNameComboBox.SelectedItem as string) ||
                    string.IsNullOrWhiteSpace(HospitalNameComboBox.SelectedItem as string) ||
                    string.IsNullOrWhiteSpace(DateTextBox.Text) ||
                    string.IsNullOrWhiteSpace(OutcomeTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Surgery.Name = NameTextBox.Text;
                    Surgery.PatientName = PatientNameComboBox.SelectedItem as string;
                    Surgery.DoctorName = DoctorNameComboBox.SelectedItem as string;
                    Surgery.HospitalName = HospitalNameComboBox.SelectedItem as string;
                    Surgery.Date = DateTextBox.SelectedDate.Value;
                    Surgery.Outcome = OutcomeTextBox.Text;

                    DialogResult = true; 
                    Close();
                }
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
