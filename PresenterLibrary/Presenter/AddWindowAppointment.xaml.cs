using PresenterLibrary.ClassBD;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Numerics;
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
    /// Логика взаимодействия для AddWindowAppointment.xaml
    /// </summary>
    public partial class AddWindowAppointment : Window
    {
        public Appointment Appointment { get; private set; }

        private string _connectionString;

        public AddWindowAppointment(Appointment app, string connectionString)
        {
            Appointment = app;
            InitializeComponent();
            _connectionString =  $"Data Source={connectionString};Version=3;";

            LoadDoctors();
            LoadPatients();
            DisplayAppointmentDetails();
        }
        private void DisplayAppointmentDetails()
        {
            if (Appointment != null)
            {

                PationComboBox.Text = Appointment.PatientName;
                DoctorComboBox.SelectedItem = Appointment.DoctorName;              
                DateDatePicker.SelectedDate = Appointment.Date;
                DescriptionTextBox.Text = Appointment.Description;
            }
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
                            string doctorName = reader["Name"] as string;
                            if (!string.IsNullOrWhiteSpace(doctorName))
                            {
                                doctors.Add(doctorName);
                            }
                        }
                    }
                }
            }

            DoctorComboBox.ItemsSource = doctors;
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
                    DoctorComboBox.SelectedItem == null ||
                    DateDatePicker.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                Appointment.PatientName = PationComboBox.SelectedItem as string;
                Appointment.DoctorName = DoctorComboBox.SelectedItem as string;
                Appointment.Date = DateDatePicker.SelectedDate.Value;
                Appointment.Description = DescriptionTextBox.Text;

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
            DialogResult = false;
            Close();
        }
    }
}
