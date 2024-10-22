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
using static PresenterLibrary.ClassBD.DoctorBd;

namespace PresenterLibrary.Presenter
{
    /// <summary>
    /// Логика взаимодействия для AddDoctorWindow.xaml
    /// </summary>
    public partial class AddDoctorWindow : Window
    {
        public Doctor Doctor { get; private set; }
        private string ConnectionString;


        public AddDoctorWindow(Doctor doctor, string connectionString)
        {
            InitializeComponent();
            Doctor = doctor;

            ConnectionString = $"Data Source={connectionString};Version=3;";
            LoadSpecialization();
            LoadParentHospitals();
       
            if (Doctor != null)
            {
                NameTextBox.Text = Doctor.Name;
                SpecializationComboBox.SelectedItem = Doctor.Specialization;
                DegreeTextBox.Text = Doctor.Degree;
                TitleTextBox.Text = Doctor.Title;
                HospitalClinicComboBox.SelectedItem = Doctor.HospitalClinic;
                OperationCountTextBox.Text = Doctor.OperationCount.ToString();
                OperationDeathCountTextBox.Text = Doctor.OperationDeathCount.ToString();
                HazardousWorkAllowanceTextBox.Text = Doctor.HazardousWorkAllowance.ToString();
                VacationDaysTextBox.Text = Doctor.VacationDays.ToString();
                IsConsultantCheckBox.IsChecked = Doctor.IsConsultant;
            }

        }
        private void LoadParentHospitals()
        {
            List<string> hospitalNames = new List<string>();
            string query = "SELECT DISTINCT Name FROM MedicalFacility;";

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

            HospitalClinicComboBox.ItemsSource = hospitalNames;
        }
        private void LoadSpecialization()
        {
            List<string> SpecNames = new List<string>();
            string query = "SELECT DISTINCT SpecializationName FROM Specialization;";

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string specName = reader["SpecializationName"] as string;
                            if (!string.IsNullOrWhiteSpace(specName))
                            {
                                SpecNames.Add(specName);
                            }
                        }
                    }
                }
            }

            SpecializationComboBox.ItemsSource = SpecNames;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка ввода
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || SpecializationComboBox.SelectedItem == null || HospitalClinicComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please enter required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Doctor.Name = NameTextBox.Text;
            Doctor.Specialization = SpecializationComboBox.SelectedItem.ToString();
            Doctor.Degree = DegreeTextBox.Text;
            Doctor.Title = TitleTextBox.Text;
            Doctor.HospitalClinic = HospitalClinicComboBox.SelectedItem.ToString();

          
            if (int.TryParse(OperationCountTextBox.Text, out int operationCount) || operationCount <0)
            {
                Doctor.OperationCount = operationCount;
            }
            else
            {
                MessageBox.Show("Неверное количество операций.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (int.TryParse(OperationDeathCountTextBox.Text, out int operationDeathCount) || operationDeathCount < 0 || operationDeathCount> operationCount)
            {
                Doctor.OperationDeathCount = operationDeathCount;
            }
            else
            {
                MessageBox.Show("Неверное количество операций c летальным исходом.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (double.TryParse(HazardousWorkAllowanceTextBox.Text, out double hazardousWorkAllowance))
            {
                Doctor.HazardousWorkAllowance = hazardousWorkAllowance;
            }
            else
            {
                MessageBox.Show("Неверный коэффицент.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (int.TryParse(VacationDaysTextBox.Text, out int vacationDays))
            {
                Doctor.VacationDays = vacationDays;
            }
            else
            {
                MessageBox.Show("Неверное количество дней отпуска.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Doctor.IsConsultant = IsConsultantCheckBox.IsChecked ?? false;

           
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
