using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresenterLibrary.ClassBD
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Windows;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public class SurgeryBd
    {
        public class Surgery
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string PatientName { get; set; }
            public string DoctorName { get; set; }
            public string HospitalName { get; set; }
            public DateTime Date { get; set; }
            public string Outcome { get; set; }
        }

        private readonly string _connectionString;
        public List<Surgery> SurgeryList { get; private set; }

        public SurgeryBd(string dbName)
        {
            _connectionString = $"Data Source={dbName};Version=3;";
            SurgeryList = GetSurgeryData();
        }

        private List<Surgery> GetSurgeryData()
        {
            List<Surgery> surgeryList = new List<Surgery>();
            string query = @"
            SELECT 
                Surgery.ID, 
                Surgery.Name,
                Patient.Name AS PatientName, 
                Doctor.Name AS DoctorName, 
                MedicalFacility.Name AS HospitalName, 
                Surgery.Date, 
                Surgery.Outcome 
            FROM Surgery
            INNER JOIN Patient ON Surgery.Patient_ID = Patient.ID
            INNER JOIN Doctor ON Surgery.Doctor_ID = Doctor.ID
            INNER JOIN MedicalFacility ON Surgery.Hospital_ID = MedicalFacility.ID";

            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Surgery surgery = new Surgery
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = Convert.ToString(reader["Name"]),
                                PatientName = Convert.ToString(reader["PatientName"]),
                                DoctorName = Convert.ToString(reader["DoctorName"]),
                                HospitalName = Convert.ToString(reader["HospitalName"]),
                                Date = Convert.ToDateTime(reader["Date"]),
                                Outcome = Convert.ToString(reader["Outcome"])
                            };

                            surgeryList.Add(surgery);
                        }
                    }
                }
            }

            return surgeryList;
        }

        public void AddSurgery(Surgery newSurgery)
        {
            string insertQuery = @"
            INSERT INTO Surgery (Name, Patient_ID, Doctor_ID, Hospital_ID, Date, Outcome) 
            VALUES (
            @Name,
            (SELECT ID FROM Patient WHERE Name = @PatientName), 
            (SELECT ID FROM Doctor WHERE Name = @DoctorName), 
            (SELECT ID FROM MedicalFacility WHERE Name = @HospitalName LIMIT 1), 
            @Date, 
            @Outcome)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", newSurgery.Name);
                        command.Parameters.AddWithValue("@PatientName", newSurgery.PatientName);
                        command.Parameters.AddWithValue("@DoctorName", newSurgery.DoctorName);
                        command.Parameters.AddWithValue("@HospitalName", newSurgery.HospitalName);
                        command.Parameters.AddWithValue("@Date", newSurgery.Date.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@Outcome", newSurgery.Outcome);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newSurgery.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                SurgeryList.Add(newSurgery);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding surgery: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateSurgery(Surgery updatedSurgery)
        {
            string updateQuery = @"
            UPDATE Surgery 
            SET 
            Name = @Name,
            Patient_ID = (SELECT ID FROM Patient WHERE Name = @PatientName LIMIT 1), 
            Doctor_ID = (SELECT ID FROM Doctor WHERE Name = @DoctorName LIMIT 1), 
            Hospital_ID = (SELECT ID FROM MedicalFacility WHERE Name = @HospitalName LIMIT 1), 
            Date = @Date, 
            Outcome = @Outcome 
            WHERE ID = @ID;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", updatedSurgery.Name);
                        command.Parameters.AddWithValue("@PatientName", updatedSurgery.PatientName);
                        command.Parameters.AddWithValue("@DoctorName", updatedSurgery.DoctorName);
                        command.Parameters.AddWithValue("@HospitalName", updatedSurgery.HospitalName);
                        command.Parameters.AddWithValue("@Date", updatedSurgery.Date.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@Outcome", updatedSurgery.Outcome);
                        command.Parameters.AddWithValue("@ID", updatedSurgery.ID);

                        command.ExecuteNonQuery();
                    }
                }

                var surgery = SurgeryList.Find(s => s.ID == updatedSurgery.ID);
                if (surgery != null)
                {
                    surgery.PatientName = updatedSurgery.PatientName;
                    surgery.DoctorName = updatedSurgery.DoctorName;
                    surgery.HospitalName = updatedSurgery.HospitalName;
                    surgery.Date = updatedSurgery.Date;
                    surgery.Outcome = updatedSurgery.Outcome;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении операции: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteSurgery(int surgeryID)
        {
            string deleteQuery = "DELETE FROM Surgery WHERE ID = @ID;";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ID", surgeryID);
                        command.ExecuteNonQuery();
                    }
                }

                var surgery = SurgeryList.Find(s => s.ID == surgeryID);
                if (surgery != null)
                {
                    SurgeryList.Remove(surgery);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
