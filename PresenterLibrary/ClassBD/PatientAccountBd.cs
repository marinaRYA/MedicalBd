using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresenterLibrary.ClassBD
{
    public class PatientAccountBd
    {
        
        public class PatientAccount
        {
            public int ID { get; set; }
            public string PatientName { get; set; }
            public string MedicalHistory { get; set; }
            public string DoctorName { get; set; }
            public string HospitalName { get; set; }
            //public List<string> Surgeries { get; set; }
           // public List<string> Recommendations { get; set; }
            public string SurgeriesDisplay { get; set; }
            public string RecommendationsDisplay { get; set; }
        }

        private readonly string _connectionString;
        public List<PatientAccount> PatientAccountList { get; private set; }

        public PatientAccountBd(string dbName)
        {
            _connectionString = $"Data Source={dbName};Version=3;";
            PatientAccountList = GetPatientAccountData();
        }

        
        public List<PatientAccount> GetPatientAccountData()
        {
            List<PatientAccount> patientAccountList = new List<PatientAccount>();
            string query = @"
            SELECT pa.ID, p.ID AS patient_ID, p.Name AS PatientName, pa.MedicalHistory, 
                   d.Name AS DoctorName, 
                   mf.Name AS HospitalName 
            FROM PatientAccount pa
            INNER JOIN Patient p ON pa.Patient_ID = p.ID
            INNER JOIN Doctor d ON p.Attending_Doctor_ID = d.ID
            INNER JOIN MedicalFacility mf ON p.Hospital_ID = mf.ID";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var patientAccount = new PatientAccount
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                PatientName = Convert.ToString(reader["PatientName"]),
                                MedicalHistory = Convert.ToString(reader["MedicalHistory"]),
                                DoctorName = reader["DoctorName"] != DBNull.Value ? Convert.ToString(reader["DoctorName"]) : null,
                                HospitalName = reader["HospitalName"] != DBNull.Value ? Convert.ToString(reader["HospitalName"]) : null,
                                SurgeriesDisplay = GetPatientSurgeries(Convert.ToInt32(reader["patient_ID"])),
                                RecommendationsDisplay = GetPatientRecommendations(Convert.ToInt32(reader["patient_ID"]))
                            };

                            patientAccountList.Add(patientAccount);
                        }
                    }
                }
            }

            return patientAccountList;
        }

       
        public void AddPatientAccount(string patientName, string medicalHistory)
        {
            string query = @"
            INSERT INTO PatientAccount (Patient_ID, MedicalHistory) 
            VALUES (
                (SELECT ID FROM Patient WHERE Name = @PatientName),
                @MedicalHistory)";
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PatientName", patientName);
                        command.Parameters.AddWithValue("@MedicalHistory", medicalHistory);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding patient account: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void UpdatePatientAccount(PatientAccount patientAccount)
        {
       string query = @"
        UPDATE PatientAccount 
        SET MedicalHistory = @MedicalHistory,
        Patient_ID = (SELECT ID FROM Patient WHERE Name = @PatientName)
        WHERE ID = @ID;";


            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        using (SQLiteCommand command = new SQLiteCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@MedicalHistory", patientAccount.MedicalHistory);
                            command.Parameters.AddWithValue("@PatientName", patientAccount.PatientName);
                            command.Parameters.AddWithValue("@ID", patientAccount.ID);
                           
                            int rowsAffected = command.ExecuteNonQuery();
                            transaction.Commit();
                            //if (rowsAffected == 0) MessageBox.Show("Failed to update. Patient not found.");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DeletePatientAccount(int patientId)
        {
            string query = @"
            DELETE FROM PatientAccount WHERE Id = @Id;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        using (SQLiteCommand command = new SQLiteCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@Id", patientId);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting patient account: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private string GetPatientSurgeries(int patientId)
        {
            List<string> surgeriesList = new List<string>();
            string query = @"
            SELECT Name 
            FROM Surgery 
            WHERE Patient_ID = @PatientId";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PatientId", patientId);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            surgeriesList.Add(Convert.ToString(reader["Name"]));
                        }
                    }
                }
            }

            return string.Join(", ", surgeriesList);
        }

      
        private string GetPatientRecommendations(int patientId)
        {
            List<string> recommendationsList = new List<string>();
            string query = @"
            SELECT Description
            FROM Appointment
            WHERE Patient_ID = @PatientId";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PatientId", patientId);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recommendationsList.Add(Convert.ToString(reader["Description"]));
                        }
                    }
                }
            }
            return string.Join(", ", recommendationsList);
        }
    }

}
