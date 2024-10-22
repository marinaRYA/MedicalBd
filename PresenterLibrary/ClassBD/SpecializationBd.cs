using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

public class SpecializationBd
{
    public class Specialization
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    private readonly string _connectionString;
    public List<Specialization> SpecializationList { get; private set; }

    public SpecializationBd(string dbName)
    {
        _connectionString = $"Data Source={dbName};Version=3;";
        SpecializationList = GetSpecializationData();
    }

    private List<Specialization> GetSpecializationData()
    {
        List<Specialization> specializationList = new List<Specialization>();
        string query = "SELECT * FROM Specialization;";

        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Specialization specialization = new Specialization
                        {
                            ID = Convert.ToInt32(reader["SpecializationId"]),
                            Name = Convert.ToString(reader["SpecializationName"]),
                        };

                        specializationList.Add(specialization);
                    }
                }
            }
        }

        return specializationList;
    }

    public void AddSpecialization(Specialization newSpecialization)
    {
        string insertQuery = "INSERT INTO Specialization (SpecializationName) VALUES (@Name)";
        string selectIdQuery = "SELECT last_insert_rowid();";

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", newSpecialization.Name);

                    command.ExecuteNonQuery();

                    using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                    {
                        newSpecialization.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                    }
                }
            }

            SpecializationList.Add(newSpecialization);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding specialization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void UpdateSpecialization(Specialization updatedSpecialization)
    {
        string updateQuery = "UPDATE Specialization SET SpecializationName = @Name WHERE SpecializationId = @ID;";

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", updatedSpecialization.Name);
                    command.Parameters.AddWithValue("@ID", updatedSpecialization.ID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0) throw new Exception("Failed to update. Specialization not found.");
                }

                int index = SpecializationList.FindIndex(lab => lab.ID == updatedSpecialization.ID);
                SpecializationList[index] = updatedSpecialization;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating specialization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void DeleteSpecialization(int specializationID)
    {
        string deleteQuery = "DELETE FROM Specialization WHERE SpecializationId = @ID";
        int index = SpecializationList.FindIndex(lab => lab.ID == specializationID);

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", specializationID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0) throw new Exception("Failed to delete specialization.");

                    SpecializationList.RemoveAt(index);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting specialization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
