using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

public class StaffBd
{
    public class Staff
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string HospitalName { get; set; }
        public string SpecializationName { get; set; }
    }

    private readonly string _connectionString;
    public List<Staff> StaffList { get; private set; }

    public StaffBd(string dbName)
    {
        _connectionString = $"Data Source={dbName};Version=3;";
        StaffList = GetStaffData();
    }

    // Метод для получения списка сотрудников
    private List<Staff> GetStaffData()
    {
        List<Staff> staffList = new List<Staff>();
        string query = "SELECT Staff.ID, Staff.Name, Staff.Role, Specialization.SpecializationName, MedicalFacility.Name as HospitalName " +
                       "FROM Staff " +
                       "INNER JOIN Specialization ON Specialization.SpecializationId = Staff.SpecializationId " +
                       "INNER JOIN MedicalFacility ON MedicalFacility.ID = Staff.HospitalClinic_ID";

        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Staff staff = new Staff
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = Convert.ToString(reader["Name"]),
                            Role = Convert.ToString(reader["Role"]),
                            SpecializationName = Convert.ToString(reader["SpecializationName"]),
                            HospitalName = Convert.ToString(reader["HospitalName"])
                        };

                        staffList.Add(staff);
                    }
                }
            }
        }

        return staffList;
    }

    // Метод для добавления сотрудника
    public void AddStaff(Staff newStaff)
    {
        string insertQuery = "INSERT INTO Staff (Name, Role, HospitalClinic_ID, SpecializationId) VALUES (@Name, @Role, @HospitalClinic_ID, @SpecializationId)";
        string selectIdQuery = "SELECT last_insert_rowid();";

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", newStaff.Name);
                    command.Parameters.AddWithValue("@Role", newStaff.Role);
                    command.Parameters.AddWithValue("@HospitalClinic_ID", GetMedicalFacilityIDByName(newStaff.HospitalName));
                    command.Parameters.AddWithValue("@SpecializationId", GetSpecializationIDByName(newStaff.SpecializationName));

                    command.ExecuteNonQuery();

                    using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                    {
                        newStaff.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                    }
                }
            }

            StaffList.Add(newStaff);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Метод для обновления данных сотрудника
    public void UpdateStaff(Staff updatedStaff)
    {
        string updateQuery = "UPDATE Staff SET Name = @Name, Role = @Role, HospitalClinic_ID = @HospitalClinic_ID, " +
                             "SpecializationId = @SpecializationId WHERE ID = @ID;";

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", updatedStaff.Name);
                    command.Parameters.AddWithValue("@Role", updatedStaff.Role);
                    command.Parameters.AddWithValue("@HospitalClinic_ID", GetMedicalFacilityIDByName(updatedStaff.HospitalName));
                    command.Parameters.AddWithValue("@SpecializationId", GetSpecializationIDByName(updatedStaff.SpecializationName));
                    command.Parameters.AddWithValue("@ID", updatedStaff.ID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0) throw new Exception("Failed to update. Staff not found.");
                }

                int index = StaffList.FindIndex(staff => staff.ID == updatedStaff.ID);
                if (index >= 0)
                {
                    StaffList[index] = updatedStaff;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Метод для удаления сотрудника
    public void DeleteStaff(int staffID)
    {
        string deleteQuery = "DELETE FROM Staff WHERE ID = @ID";
        int index = StaffList.FindIndex(staff => staff.ID == staffID);

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", staffID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0) throw new Exception("Failed to delete staff.");

                    if (index >= 0)
                    {
                        StaffList.RemoveAt(index);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

  private int GetMedicalFacilityIDByName(string MedicalFacilityName)
    {
        string query = "SELECT ID FROM MedicalFacility WHERE Name = @MedicalFacilityName";

        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@MedicalFacilityName", MedicalFacilityName);

                object result = command.ExecuteScalar();
                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : -1;
            }
        }
    }

  
    private int GetSpecializationIDByName(string SpecializationName)
    {
        string query = "SELECT SpecializationId FROM Specialization WHERE SpecializationName = @SpecializationName";

        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@SpecializationName", SpecializationName);

                object result = command.ExecuteScalar();
                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : -1;
            }
        }
    }
}

