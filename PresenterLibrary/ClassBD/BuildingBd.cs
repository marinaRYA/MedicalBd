using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PresenterLibrary.ClassBD
{

    public class BuildingBd
    {
        public class Building
        {
            public int ID { get; set; }
            public string HospitalName { get; set; }
            public string BuildingName { get; set; }
            public string Address { get; set; }
        }

        private readonly string connectionString;
        public List<Building> BuildingList { get; private set; }

        public BuildingBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            BuildingList = GetBuildingData();
        }

        private List<Building> GetBuildingData()
        {
            List<Building> buildingList = new List<Building>();
            string query = @"
        SELECT b.ID, h.Name AS HospitalName, b.Building_Name, b.Address
        FROM Building b
        JOIN MedicalFacility h ON b.Hospital_ID = h.ID;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Building building = new Building
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                HospitalName = Convert.ToString(reader["HospitalName"]),
                                BuildingName = Convert.ToString(reader["Building_Name"]),
                                Address = reader["Address"] == DBNull.Value ? null : Convert.ToString(reader["Address"]),
                            };

                            buildingList.Add(building);
                        }
                    }
                }
            }

            return buildingList;
        }

        public void AddBuilding(Building newBuilding)
        {
            string insertQuery = "INSERT INTO Building (Hospital_ID, Building_Name, Address) VALUES " +
                "((SELECT ID FROM MedicalFacility WHERE Name = @HospitalName), @BuildingName, @Address)";
            string selectIdQuery = "SELECT last_insert_rowid();";

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@HospitalName", newBuilding.HospitalName);
                        command.Parameters.AddWithValue("@BuildingName", newBuilding.BuildingName);
                        command.Parameters.AddWithValue("@Address", newBuilding.Address);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newBuilding.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                BuildingList.Add(newBuilding);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding building: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateBuilding(Building updatedBuilding)
        {
            string updateQuery = "UPDATE Building " +
                                 "SET Hospital_ID = (SELECT ID FROM MedicalFacility WHERE Name = @HospitalName), " +
                                 "Building_Name = @BuildingName, " +
                                 "Address = @Address " +
                                 "WHERE ID = @ID;";
            try 
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@HospitalName", updatedBuilding.HospitalName);
                        command.Parameters.AddWithValue("@BuildingName", updatedBuilding.BuildingName);
                        command.Parameters.AddWithValue("@Address", updatedBuilding.Address);
                        command.Parameters.AddWithValue("@ID", updatedBuilding.ID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Failed to update. Building not found.");
                    }

                    int index = BuildingList.FindIndex(b => b.ID == updatedBuilding.ID);
                    BuildingList[index] = updatedBuilding;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating building: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteBuilding(int buildingID)
        {
            string deleteQuery = "DELETE FROM Building WHERE ID = @Id";
            int index = BuildingList.FindIndex(building => building.ID == buildingID);

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", buildingID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Delete operation failed.");
                        BuildingList.RemoveAt(index);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting building: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
