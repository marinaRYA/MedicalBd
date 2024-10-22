using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

public class LaboratoryBd
{
    public class Laboratory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Profile { get; set; }
        public string HospitalClinicName { get; set; }  
    }

    private readonly string _connectionString;
    public List<Laboratory> LaboratoryList { get; private set; }

    public LaboratoryBd(string dbName)
    {
        _connectionString = $"Data Source={dbName};Version=3;";
        LaboratoryList = GetLaboratoryData();
    }

    private List<Laboratory> GetLaboratoryData()
    {
        List<Laboratory> laboratoryList = new List<Laboratory>();
        string query = @"
            SELECT 
                Laboratory.ID, 
                Laboratory.Name, 
                ProfileLab.ProfileLabName, 
                mf.Name AS HospitalClinicName
            FROM 
                Laboratory 
            INNER JOIN 
                ProfileLab ON ProfileLab.ID = Laboratory.ProfileID
            LEFT JOIN 
                Laboratory_HospitalClinic lh ON lh.Laboratory_ID = Laboratory.ID
            LEFT JOIN 
                MedicalFacility mf ON mf.ID = lh.HospitalClinic_ID";  

        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Laboratory laboratory = new Laboratory
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = Convert.ToString(reader["Name"]),
                            Profile = Convert.ToString(reader["ProfileLabName"]),
                            HospitalClinicName = reader["HospitalClinicName"] != DBNull.Value ? Convert.ToString(reader["HospitalClinicName"]) : null // Get hospital/clinic name
                        };

                        laboratoryList.Add(laboratory);
                    }
                }
            }
        }

        return laboratoryList;
    }

    public void AddLaboratory(Laboratory newLaboratory)
    {
        string insertQuery = "INSERT INTO Laboratory (Name, ProfileID) VALUES (@Name, @ProfileID)";
        string selectIdQuery = "SELECT last_insert_rowid();";
        string insertRelationQuery = "INSERT INTO Laboratory_HospitalClinic (Laboratory_ID, HospitalClinic_ID) VALUES (@LaboratoryID, @HospitalClinicID);";

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", newLaboratory.Name);
                    command.Parameters.AddWithValue("@ProfileID", GetProfileLabIDByName(newLaboratory.Profile));

                    command.ExecuteNonQuery();

                    using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                    {
                        newLaboratory.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                    }
                }

                
                int hospitalClinicID = GetMedicalFacilityIDByName(newLaboratory.HospitalClinicName);
                if (hospitalClinicID != -1) 
                {
                    using (SQLiteCommand insertRelationCommand = new SQLiteCommand(insertRelationQuery, connection))
                    {
                        insertRelationCommand.Parameters.AddWithValue("@LaboratoryID", newLaboratory.ID);
                        insertRelationCommand.Parameters.AddWithValue("@HospitalClinicID", hospitalClinicID);
                        insertRelationCommand.ExecuteNonQuery();
                    }
                }
            }

            LaboratoryList.Add(newLaboratory);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding laboratory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void UpdateLaboratory(Laboratory updatedLaboratory)
    {
        string updateQuery = "UPDATE Laboratory SET Name = @Name, ProfileID = @ProfileID WHERE ID = @ID;";
        string updateRelationQuery = "UPDATE Laboratory_HospitalClinic SET HospitalClinic_ID = @HospitalClinicID WHERE Laboratory_ID = @LaboratoryID;";

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", updatedLaboratory.Name);
                    command.Parameters.AddWithValue("@ProfileID", GetProfileLabIDByName(updatedLaboratory.Profile));
                    command.Parameters.AddWithValue("@ID", updatedLaboratory.ID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0) throw new Exception("Failed to update. Laboratory not found.");
                }

                
                int hospitalClinicID = GetMedicalFacilityIDByName(updatedLaboratory.HospitalClinicName);
                if (hospitalClinicID != -1) 
                {
                    using (SQLiteCommand updateRelationCommand = new SQLiteCommand(updateRelationQuery, connection))
                    {
                        updateRelationCommand.Parameters.AddWithValue("@HospitalClinicID", hospitalClinicID);
                        updateRelationCommand.Parameters.AddWithValue("@LaboratoryID", updatedLaboratory.ID);
                        updateRelationCommand.ExecuteNonQuery();
                    }
                }

                int index = LaboratoryList.FindIndex(lab => lab.ID == updatedLaboratory.ID);
                LaboratoryList[index] = updatedLaboratory;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating laboratory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void DeleteLaboratory(int laboratoryID)
    {
        string deleteRelationQuery = "DELETE FROM Laboratory_HospitalClinic WHERE Laboratory_ID = @LaboratoryID";  // Delete the relation first
        string deleteQuery = "DELETE FROM Laboratory WHERE ID = @ID";
        int index = LaboratoryList.FindIndex(lab => lab.ID == laboratoryID);

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                // Delete the relation
                using (SQLiteCommand deleteRelationCommand = new SQLiteCommand(deleteRelationQuery, connection))
                {
                    deleteRelationCommand.Parameters.AddWithValue("@LaboratoryID", laboratoryID);
                    deleteRelationCommand.ExecuteNonQuery();
                }

                // Now delete the laboratory
                using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", laboratoryID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0) throw new Exception("Failed to delete laboratory.");

                    LaboratoryList.RemoveAt(index);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting laboratory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private int GetProfileLabIDByName(string ProfileLabName)
    {
        string query = "SELECT ID FROM ProfileLab WHERE ProfileLabName = @ProfileLabName";

        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ProfileLabName", ProfileLabName);

                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return Convert.ToInt32(result);
                else
                    return -1; // Return -1 if no matching profile found
            }
        }
    }

    private int GetMedicalFacilityIDByName(string facilityName)
    {
        string query = "SELECT ID FROM MedicalFacility WHERE Name = @Name";

        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", facilityName);

                object result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return Convert.ToInt32(result);
                else
                    return -1; // Return -1 if no matching facility found
            }
        }
    }
}
