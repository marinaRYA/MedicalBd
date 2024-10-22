using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

public class WardBd
{
    public class Ward
    {
        public int ID { get; set; }
        public int Number { get; set; }
        public string DepartmentName { get; set; }
        public int BedCount { get; set; }
    }

    private readonly string connectionString;
    public List<Ward> WardList { get; private set; }

    public WardBd(string bdName)
    {
        connectionString = $"Data Source={bdName};Version=3;";
        WardList = GetWardData();
    }

    private List<Ward> GetWardData()
    {
        List<Ward> wardList = new List<Ward>();
        string query = @"
        SELECT w.ID, w.Number, d.Name AS DepartmentName, w.Bed_Count
        FROM Ward w
        LEFT JOIN Department d ON w.Department_ID = d.ID;";

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ward ward = new Ward
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Number = Convert.ToInt32(reader["Number"]),
                            DepartmentName = Convert.ToString(reader["DepartmentName"]),
                            BedCount = Convert.ToInt32(reader["Bed_Count"])
                        };

                        wardList.Add(ward);
                    }
                }
            }
        }

        return wardList;
    }

    public void AddWard(Ward newWard)
    {
        string insertQuery = "INSERT INTO Ward (Number, Department_ID, Bed_Count) VALUES " +
            "(@Number, (SELECT ID FROM Department WHERE Name = @DepartmentName), @BedCount)";
        string selectIdQuery = "SELECT last_insert_rowid();";

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Number", newWard.Number);
                    command.Parameters.AddWithValue("@DepartmentName", newWard.DepartmentName);
                    command.Parameters.AddWithValue("@BedCount", newWard.BedCount);

                    command.ExecuteNonQuery();

                    using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                    {
                        newWard.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                    }
                }
            }

            WardList.Add(newWard);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding ward: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void UpdateWard(Ward updatedWard)
    {
        string updateQuery = @"
            UPDATE Ward 
            SET Number = @Number, 
                Department_ID = (SELECT ID FROM Department WHERE Name = @DepartmentName),
                Bed_Count = @BedCount
            WHERE ID = @ID;";

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Number", updatedWard.Number);
                    command.Parameters.AddWithValue("@DepartmentName", updatedWard.DepartmentName);
                    command.Parameters.AddWithValue("@BedCount", updatedWard.BedCount);
                    command.Parameters.AddWithValue("@ID", updatedWard.ID);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0) throw new Exception("Failed to update. Ward not found.");

                    int index = WardList.FindIndex(w => w.ID == updatedWard.ID);
                    if (index == -1) throw new Exception("Ward not found in the list.");

                    WardList[index] = updatedWard;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating ward: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public void DeleteWard(int wardID)
    {
        string deleteQuery = "DELETE FROM Ward WHERE ID = @Id";
        int index = WardList.FindIndex(ward => ward.ID == wardID);

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", wardID);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0) throw new Exception("Delete operation failed.");

                    if (index == -1) throw new Exception("Ward not found in the list.");
                    WardList.RemoveAt(index);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting ward: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

