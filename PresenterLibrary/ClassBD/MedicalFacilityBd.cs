using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SQLite;
namespace PresenterLibrary.ClassBD
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Linq;
    using System.Windows;

    public class MedicalFacilityBd
    {
        public class MedicalFacility
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Address { get; set; }
            public string ParentHospitalName { get; set; } 
        }

        private readonly string connectionString;
        public List<MedicalFacility> FacilityList { get; private set; }

        public MedicalFacilityBd(string bdName)
        {
            connectionString = $"Data Source={bdName};Version=3;";
            FacilityList = GetFacilityData();
        }

        private List<MedicalFacility> GetFacilityData()
        {
            List<MedicalFacility> facilityList = new List<MedicalFacility>();
            string query = @"
            SELECT f.ID, f.Name, f.Type, f.Address, h.Name AS ParentHospitalName
            FROM MedicalFacility f
            LEFT JOIN MedicalFacility h ON f.Parent_Hospital_ID = h.ID;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MedicalFacility facility = new MedicalFacility
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = Convert.ToString(reader["Name"]),
                                Type = Convert.ToString(reader["Type"]),
                                Address = reader["Address"] == DBNull.Value ? null : Convert.ToString(reader["Address"]),
                                ParentHospitalName = reader["ParentHospitalName"] == DBNull.Value ? null : Convert.ToString(reader["ParentHospitalName"])
                            };

                            facilityList.Add(facility);
                        }
                    }
                }
            }

            return facilityList;
        }

        public void AddFacility(MedicalFacility newFacility)
        {
            string insertQuery = "INSERT INTO MedicalFacility (Name, Type, Address, Parent_Hospital_ID) VALUES " +
                "(@Name, @Type, @Address, (SELECT ID FROM MedicalFacility WHERE Name = @ParentHospitalName))";
            string selectIdQuery = "SELECT last_insert_rowid();";

           
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", newFacility.Name);
                        command.Parameters.AddWithValue("@Type", newFacility.Type);
                        command.Parameters.AddWithValue("@Address", newFacility.Address);
                        command.Parameters.AddWithValue("@ParentHospitalName", newFacility.ParentHospitalName ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();

                        using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                        {
                            newFacility.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                        }
                    }
                }

                FacilityList.Add(newFacility);
           
        }

        public void UpdateFacility(MedicalFacility updatedFacility)
        {
            string updateQuery = "UPDATE MedicalFacility " +
                     "SET Name = @Name, " +
                     "Type = @Type, " +
                     "Address = @Address, " +
                     "Parent_Hospital_ID = (SELECT ID FROM MedicalFacility WHERE Name = @ParentHospitalName) " +
                     "WHERE ID = @ID;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", updatedFacility.Name);
                        command.Parameters.AddWithValue("@Type", updatedFacility.Type);
                        command.Parameters.AddWithValue("@Address", updatedFacility.Address);
                        command.Parameters.AddWithValue("@ParentHospitalName", updatedFacility.ParentHospitalName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ID", updatedFacility.ID);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Failed to update. Medical facility not found.");
                    }
                    int index = FacilityList.FindIndex(c => c.ID == updatedFacility.ID);
                    FacilityList[index] = updatedFacility;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating medical facility: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void DeleteFacility(int facilityID)
        {
            string deleteQuery = "DELETE FROM MedicalFacility WHERE ID = @Id";
            int index = FacilityList.FindIndex(facility => facility.ID == facilityID);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", facilityID);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0) throw new Exception("Delete operation failed.");
                        FacilityList.RemoveAt(index);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting medical facility: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}
