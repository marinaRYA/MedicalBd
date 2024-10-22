using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;

public class DepartmentBd
{
    public class Department
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string BuildingName { get; set; }
    }

    private readonly string connectionString;
    public List<Department> DepartmentList { get; private set; }

    public DepartmentBd(string bdName)
    {
        connectionString = $"Data Source={bdName};Version=3;";
        DepartmentList = GetDepartmentData();
    }

    private List<Department> GetDepartmentData()
    {
        List<Department> departmentList = new List<Department>();
        string query = @"
        SELECT d.ID, d.Name, b.Building_Name AS BuildingName
        FROM Department d
        LEFT JOIN Building b ON d.Building_ID = b.ID;";

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Department department = new Department
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = Convert.ToString(reader["Name"]),
                            BuildingName = Convert.ToString(reader["BuildingName"])
                        };

                        departmentList.Add(department);
                    }
                }
            }
        }

        return departmentList;
    }

    public void AddDepartment(Department newDepartment)
    {
        string insertQuery = "INSERT INTO Department (Name, Building_ID) VALUES " +
            "(@Name, (SELECT ID FROM Building WHERE Building_Name = @BuildingName))";
        string selectIdQuery = "SELECT last_insert_rowid();";

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", newDepartment.Name);
                    command.Parameters.AddWithValue("@BuildingName", newDepartment.BuildingName);

                    command.ExecuteNonQuery();

                    using (SQLiteCommand selectIdCommand = new SQLiteCommand(selectIdQuery, connection))
                    {
                        newDepartment.ID = Convert.ToInt32(selectIdCommand.ExecuteScalar());
                    }
                }
            }

            DepartmentList.Add(newDepartment);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding department: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void UpdateDepartment(Department updatedDepartment)
    {
        string updateQuery = @"
            UPDATE Department 
            SET Name = @Name, 
                Building_ID = (SELECT ID FROM Building WHERE Building_Name = @BuildingName)
            WHERE ID = @ID;";

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", updatedDepartment.Name);
                    command.Parameters.AddWithValue("@BuildingName", updatedDepartment.BuildingName);
                    command.Parameters.AddWithValue("@ID", updatedDepartment.ID);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0) throw new Exception("Failed to update. Department not found.");

                    int index = DepartmentList.FindIndex(d => d.ID == updatedDepartment.ID);
                    if (index == -1) throw new Exception("Department not found in the list.");

                    DepartmentList[index] = updatedDepartment;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating department: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public void DeleteDepartment(int departmentID)
    {
        string deleteQuery = "DELETE FROM Department WHERE ID = @Id";
        int index = DepartmentList.FindIndex(department => department.ID == departmentID);

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            try
            {
                using (SQLiteCommand command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", departmentID);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0) throw new Exception("Delete operation failed.");

                    if (index == -1) throw new Exception("Department not found in the list.");
                    DepartmentList.RemoveAt(index);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting department: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
