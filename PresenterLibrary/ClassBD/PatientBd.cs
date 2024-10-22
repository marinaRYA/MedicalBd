using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresenterLibrary.ClassBD
{
    public class PatientBd
    {
        public class Patient
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public DateTime BirthDate { get; set; }
            public string Address { get; set; }
            public string ClinicName { get; set; }
            public string HospitalName { get; set; }
            public string AttendingDoctorName { get; set; }
            public string Gender { get; set; }
            public string Polis { get; set; }
        }

        private readonly string connectionString;
        public List<Patient> PatientList { get; private set; }

        public PatientBd(string connectionString)
        {
            this.connectionString = $"Data Source={connectionString};Version=3;";
            PatientList = GetPatientData();
        }

        private List<Patient> GetPatientData()
        {
            List<Patient> patientList = new List<Patient>();
            string query = @"
        SELECT p.ID, p.Name, p.Birth_Date AS BirthDate, p.Address, 
               COALESCE(c.Name, '') AS ClinicName, 
               COALESCE(h.Name, '') AS HospitalName, 
               COALESCE(d.Name, '') AS AttendingDoctorName, p.Gender, p.Polis
        FROM Patient p
        LEFT JOIN MedicalFacility c ON p.Clinic_ID = c.ID
        LEFT JOIN MedicalFacility h ON p.Hospital_ID = h.ID
        LEFT JOIN Doctor d ON p.Attending_Doctor_ID = d.ID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Patient patient = new Patient
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = Convert.ToString(reader["Name"]),
                                BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                                Address = reader["Address"] == DBNull.Value ? null : Convert.ToString(reader["Address"]),
                                ClinicName = Convert.ToString(reader["ClinicName"]),
                                HospitalName = Convert.ToString(reader["HospitalName"]),
                                AttendingDoctorName = Convert.ToString(reader["AttendingDoctorName"]),
                                Gender = reader["Gender"] == DBNull.Value ? null : Convert.ToString(reader["Gender"]),
                                Polis = Convert.ToString(reader["Polis"])
                            };

                            patientList.Add(patient);
                        }
                    }
                }
            }

            return patientList;
        }

        public void AddPatient(Patient newPatient)
        {
            string insertQuery = @"
        INSERT INTO Patient (Name, Birth_Date, Address, Clinic_ID, Hospital_ID, Attending_Doctor_ID, Gender, Polis) 
        VALUES (@Name, @BirthDate, @Address, 
                (SELECT ID FROM MedicalFacility WHERE Name = @ClinicName ),
                (SELECT ID FROM MedicalFacility WHERE Name = @HospitalName),
                (SELECT ID FROM Doctor WHERE Name = @AttendingDoctorName),
                @Gender, @Polis)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", newPatient.Name);
                        command.Parameters.AddWithValue("@BirthDate", newPatient.BirthDate.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@Address", newPatient.Address ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ClinicName", string.IsNullOrWhiteSpace(newPatient.ClinicName) ? (object)DBNull.Value : newPatient.ClinicName);
                        command.Parameters.AddWithValue("@HospitalName", string.IsNullOrWhiteSpace(newPatient.HospitalName) ? (object)DBNull.Value : newPatient.HospitalName);
                        command.Parameters.AddWithValue("@AttendingDoctorName", string.IsNullOrWhiteSpace(newPatient.AttendingDoctorName) ? (object)DBNull.Value : newPatient.AttendingDoctorName);
                        command.Parameters.AddWithValue("@Gender", newPatient.Gender ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Polis", newPatient.Polis ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newPatient.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                PatientList.Add(newPatient);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding patient: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdatePatient(Patient updatedPatient)
        {
            string updateQuery = @"
        UPDATE Patient 
        SET Name = @Name, 
            Birth_Date = @BirthDate, 
            Address = @Address, 
            Clinic_ID = (SELECT ID FROM MedicalFacility WHERE Name = @ClinicName LIMIT 1),
            Hospital_ID = (SELECT ID FROM MedicalFacility WHERE Name = @HospitalName LIMIT 1),
            Attending_Doctor_ID = (SELECT ID FROM Doctor WHERE Name = @AttendingDoctorName LIMIT 1),
            Gender = @Gender, 
            Polis = @Polis 
        WHERE ID = @ID";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", updatedPatient.Name);
                        command.Parameters.AddWithValue("@BirthDate", updatedPatient.BirthDate.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@Address", updatedPatient.Address ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ClinicName", string.IsNullOrWhiteSpace(updatedPatient.ClinicName) ? (object)DBNull.Value : updatedPatient.ClinicName);
                        command.Parameters.AddWithValue("@HospitalName", string.IsNullOrWhiteSpace(updatedPatient.HospitalName) ? (object)DBNull.Value : updatedPatient.HospitalName);
                        command.Parameters.AddWithValue("@AttendingDoctorName", string.IsNullOrWhiteSpace(updatedPatient.AttendingDoctorName) ? (object)DBNull.Value : updatedPatient.AttendingDoctorName);
                        command.Parameters.AddWithValue("@Gender", updatedPatient.Gender ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Polis", updatedPatient.Polis ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ID", updatedPatient.ID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Failed to update. Patient not found.");
                    }

                    int index = PatientList.FindIndex(p => p.ID == updatedPatient.ID);
                    PatientList[index] = updatedPatient;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating patient: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeletePatient(int patientID)
        {
            string deleteQuery = "DELETE FROM Patient WHERE ID = @Id";
            int index = PatientList.FindIndex(patient => patient.ID == patientID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", patientID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Delete operation failed.");
                        PatientList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting patient: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}
