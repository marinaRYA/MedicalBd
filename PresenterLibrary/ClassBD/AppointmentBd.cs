using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresenterLibrary.ClassBD
{
    public class AppointmentBd
    {
        public class Appointment
        {
            public int ID { get; set; }
            public string PatientName { get; set; }
            public string DoctorName { get; set; }
            public DateTime Date { get; set; }
            public string Description { get; set; }
        }

        private readonly string connectionString;
        public List<Appointment> AppointmentList { get; private set; }

        public AppointmentBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            AppointmentList = GetAppointmentData();
        }

        private List<Appointment> GetAppointmentData()
        {
            List<Appointment> appointmentList = new List<Appointment>();
            string query = @"
            SELECT a.ID, p.Name AS PatientName, d.Name AS DoctorName, 
                   a.Date, a.Description
            FROM Appointment a
            LEFT JOIN Patient p ON a.Patient_ID = p.ID
            LEFT JOIN Doctor d ON a.Doctor_ID = d.ID;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Appointment appointment = new Appointment
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                PatientName = Convert.ToString(reader["PatientName"]),
                                DoctorName = Convert.ToString(reader["DoctorName"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Description = reader["Description"] == DBNull.Value ? null : Convert.ToString(reader["Description"])
                            };
                            appointmentList.Add(appointment);
                        }
                    }
                }
            }
            return appointmentList;
        }

        public void AddAppointment(Appointment newAppointment)
        {
            string insertQuery = @"
            INSERT INTO Appointment (Patient_ID, Doctor_ID, Date, Description) 
            VALUES (
                (SELECT ID FROM Patient WHERE Name = @PatientName), 
                (SELECT ID FROM Doctor WHERE Name = @DoctorName), 
                @Date, @Description)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PatientName", newAppointment.PatientName);
                        command.Parameters.AddWithValue("@DoctorName", newAppointment.DoctorName);
                        command.Parameters.AddWithValue("@Date", newAppointment.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@Description", newAppointment.Description ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newAppointment.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                AppointmentList.Add(newAppointment);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding appointment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateAppointment(Appointment updatedAppointment)
        {
            string updateQuery = @"
            UPDATE Appointment 
            SET Patient_ID = (SELECT ID FROM Patient WHERE Name = @PatientName), 
                Doctor_ID = (SELECT ID FROM Doctor WHERE Name = @DoctorName),
                Date = @Date, 
                Description = @Description 
            WHERE ID = @ID;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PatientName", updatedAppointment.PatientName);
                        command.Parameters.AddWithValue("@DoctorName", updatedAppointment.DoctorName);
                        command.Parameters.AddWithValue("@Date", updatedAppointment.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@Description", updatedAppointment.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ID", updatedAppointment.ID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                            throw new Exception("Failed to update. Appointment not found.");

                        int index = AppointmentList.FindIndex(a => a.ID == updatedAppointment.ID);
                        if (index >= 0)
                            AppointmentList[index] = updatedAppointment;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating appointment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteAppointment(int appointmentID)
        {
            string deleteQuery = "DELETE FROM Appointment WHERE ID = @ID";
            int index = AppointmentList.FindIndex(a => a.ID == appointmentID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ID", appointmentID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                            throw new Exception("Delete operation failed.");
                        if (index >= 0)
                            AppointmentList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting appointment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}
