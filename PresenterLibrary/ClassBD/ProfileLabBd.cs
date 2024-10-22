using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

public class ProfileLabBd
{
    public class ProfileLab
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    private readonly string _connectionString;
    public List<ProfileLab> ProfileLabList { get; private set; }

    public ProfileLabBd(string dbName)
    {
        _connectionString = $"Data Source={dbName};Version=3;";
        ProfileLabList = GetProfileLabData();
    }

    private List<ProfileLab> GetProfileLabData()
    {
        List<ProfileLab> profileLabList = new List<ProfileLab>();
        string query = "SELECT * FROM ProfileLab;";

        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProfileLab profileLab = new ProfileLab
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = Convert.ToString(reader["ProfileLabName"]),
                        };

                        profileLabList.Add(profileLab);
                    }
                }
            }
        }

        return profileLabList;
    }

    public void AddProfileLab(ProfileLab newProfileLab)
    {
        string insertQuery = "INSERT INTO ProfileLab (ProfileLabName) VALUES (@Name)";
        string selectIdQuery = "SELECT last_insert_rowid();";

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", newProfileLab.Name);

                    command.ExecuteNonQuery();

                    using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                    {
                        newProfileLab.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                    }
                }
            }

            ProfileLabList.Add(newProfileLab);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void UpdateProfileLab(ProfileLab updatedProfileLab)
    {
        string updateQuery = "UPDATE ProfileLab SET ProfileLabName = @Name WHERE ID = @ID;";

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", updatedProfileLab.Name);
                    command.Parameters.AddWithValue("@ID", updatedProfileLab.ID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0) throw new Exception("Failed to update. ProfileLab not found.");
                }

                int index = ProfileLabList.FindIndex(lab => lab.ID == updatedProfileLab.ID);
                ProfileLabList[index] = updatedProfileLab;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void DeleteProfileLab(int profileLabID)
    {
        string deleteQuery = "DELETE FROM ProfileLab WHERE ID = @ID";
        int index = ProfileLabList.FindIndex(lab => lab.ID == profileLabID);

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", profileLabID);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0) throw new Exception("Failed to delete profileLab.");

                    ProfileLabList.RemoveAt(index);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
