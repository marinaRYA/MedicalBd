using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresenterLibrary.ClassBD
{
    public class DoctorBd
    {
        public class Doctor
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Specialization { get; set; } 
            public string Degree { get; set; }
            public string Title { get; set; }
            public string HospitalClinic { get; set; }
            public int OperationCount { get; set; }
            public int OperationDeathCount { get; set; }
            public double HazardousWorkAllowance { get; set; }
            public int VacationDays { get; set; }
            public bool IsConsultant { get; set; }
        }

        private readonly string connectionString;
        public List<Doctor> DoctorList { get; private set; }

        public DoctorBd(string dbName)
        {
            connectionString = $"Data Source={dbName};Version=3;";
            DoctorList = GetDoctorData();
        }

        private List<Doctor> GetDoctorData()
        {
            List<Doctor> doctorList = new List<Doctor>();

            string query = @"
        SELECT d.ID, d.Name, s.SpecializationName, d.Degree, d.Title, 
               mf.Name AS HospitalClinicName, d.Operation_Count, d.Operation_Death_Count,
               d.Hazardous_Work_Allowance, d.Vacation_Days, d.Is_Consultant
        FROM Doctor d
        LEFT JOIN Specialization s ON d.Specialization = s.SpecializationId
        LEFT JOIN MedicalFacility mf ON d.HospitalClinic_ID = mf.ID;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Doctor doctor = new Doctor
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = Convert.ToString(reader["Name"]),
                                Specialization = Convert.ToString(reader["SpecializationName"]),
                                Degree = Convert.ToString(reader["Degree"]),
                                Title = Convert.ToString(reader["Title"]),
                                HospitalClinic = Convert.ToString(reader["HospitalClinicName"]),
                                OperationCount = Convert.ToInt32(reader["Operation_Count"]),
                                OperationDeathCount = Convert.ToInt32(reader["Operation_Death_Count"]),
                                HazardousWorkAllowance = Convert.ToDouble(reader["Hazardous_Work_Allowance"]),
                                VacationDays = Convert.ToInt32(reader["Vacation_Days"]),
                                IsConsultant = Convert.ToBoolean(reader["Is_Consultant"])
                            };

                            doctorList.Add(doctor);
                        }
                    }
                }
            }

            return doctorList;
        }

        public void AddDoctor(Doctor newDoctor)
        {
            string insertQuery = @"
        INSERT INTO Doctor (Name, Specialization, Degree, Title, HospitalClinic_ID, Operation_Count, 
                            Operation_Death_Count, Hazardous_Work_Allowance, Vacation_Days, Is_Consultant) 
        VALUES (@Name, 
                (SELECT SpecializationId FROM Specialization WHERE SpecializationName = @Specialization), 
                @Degree, @Title, 
                (SELECT ID FROM MedicalFacility WHERE Name = @HospitalClinic), 
                @OperationCount, @OperationDeathCount, @HazardousWorkAllowance, 
                @VacationDays, @IsConsultant)";

            string selectIdQuery = "SELECT last_insert_rowid();";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Name", newDoctor.Name);
                            command.Parameters.AddWithValue("@Specialization", newDoctor.Specialization);
                            command.Parameters.AddWithValue("@Degree", newDoctor.Degree);
                            command.Parameters.AddWithValue("@Title", newDoctor.Title);
                            command.Parameters.AddWithValue("@HospitalClinic", newDoctor.HospitalClinic);
                            command.Parameters.AddWithValue("@OperationCount", newDoctor.OperationCount);
                            command.Parameters.AddWithValue("@OperationDeathCount", newDoctor.OperationDeathCount);
                            command.Parameters.AddWithValue("@HazardousWorkAllowance", newDoctor.HazardousWorkAllowance);
                            command.Parameters.AddWithValue("@VacationDays", newDoctor.VacationDays);
                            command.Parameters.AddWithValue("@IsConsultant", newDoctor.IsConsultant);

                            command.ExecuteNonQuery();

                            using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                            {
                                newDoctor.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                            }
                        }

                        transaction.Commit();
                        DoctorList.Add(newDoctor);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error adding doctor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public void UpdateDoctor(Doctor updatedDoctor)
        {
            string updateQuery = @"
        UPDATE Doctor
        SET Name = @Name, 
            Specialization = (SELECT SpecializationId FROM Specialization WHERE SpecializationName = @Specialization),
            Degree = @Degree, 
            Title = @Title, 
            HospitalClinic_ID = (SELECT ID FROM MedicalFacility WHERE Name = @HospitalClinic),
            Operation_Count = @OperationCount, 
            Operation_Death_Count = @OperationDeathCount,
            Hazardous_Work_Allowance = @HazardousWorkAllowance,
            Vacation_Days = @VacationDays, 
            Is_Consultant = @IsConsultant
        WHERE ID = @ID;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", updatedDoctor.Name);
                        command.Parameters.AddWithValue("@Specialization", updatedDoctor.Specialization);
                        command.Parameters.AddWithValue("@Degree", updatedDoctor.Degree);
                        command.Parameters.AddWithValue("@Title", updatedDoctor.Title);
                        command.Parameters.AddWithValue("@HospitalClinic", updatedDoctor.HospitalClinic);
                        command.Parameters.AddWithValue("@OperationCount", updatedDoctor.OperationCount);
                        command.Parameters.AddWithValue("@OperationDeathCount", updatedDoctor.OperationDeathCount);
                        command.Parameters.AddWithValue("@HazardousWorkAllowance", updatedDoctor.HazardousWorkAllowance);
                        command.Parameters.AddWithValue("@VacationDays", updatedDoctor.VacationDays);
                        command.Parameters.AddWithValue("@IsConsultant", updatedDoctor.IsConsultant);
                        command.Parameters.AddWithValue("@ID", updatedDoctor.ID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("Failed to update. Doctor not found.");
                        }

                        int index = DoctorList.FindIndex(d => d.ID == updatedDoctor.ID);
                        DoctorList[index] = updatedDoctor;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating doctor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteDoctor(int doctorId)
        {
            string deleteQuery = "DELETE FROM Doctor WHERE ID = @ID";
            int index = DoctorList.FindIndex(doctor => doctor.ID == doctorId);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ID", doctorId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("Failed to delete. Doctor not found.");
                        }

                        DoctorList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting doctor: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}
