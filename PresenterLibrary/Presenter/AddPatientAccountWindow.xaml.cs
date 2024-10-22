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
using static PresenterLibrary.ClassBD.AppointmentBd;

namespace PresenterLibrary.Presenter
{
    /// <summary>
    /// Логика взаимодействия для AddPatientAccountWindow.xaml
    /// </summary>
    public partial class AddPatientAccountWindow : Window
    {
        private string _connectionString;
        public PatientAccountBd.PatientAccount patientAccount;
        public AddPatientAccountWindow(PatientAccountBd.PatientAccount pac_acc, string connectionString)
        {
            patientAccount = pac_acc;
            InitializeComponent();
            _connectionString = $"Data Source={connectionString};Version=3;";

           
            LoadPatients();
            DisplayPatientAccount();
            _connectionString = connectionString;
        }
        private void DisplayPatientAccount()
        { 
            if (patientAccount != null)
            {
                PationComboBox.Text = patientAccount.PatientName;               
                DescriptionTextBox.Text = patientAccount.MedicalHistory;
            }
        }
        private void LoadPatients()
        {
            List<string> patients = new List<string>();

            string query = "SELECT Name FROM Patient";
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
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
                                patients.Add(patientName);
                            }
                        }
                    }
                }
            }

            PationComboBox.ItemsSource = patients;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PationComboBox.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                patientAccount.PatientName = PationComboBox.SelectedItem as string;
                patientAccount.MedicalHistory = DescriptionTextBox.Text;

                // Close the window with a successful result
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
