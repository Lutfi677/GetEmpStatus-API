using System;
using System.Collections.Generic;
using System.Data;
using GetEmpStatus.Models;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace GetEmpStatus.DataAccessLayer;

public class DataAccess
{
    private readonly string connectionString;

    public DataAccess(IConfiguration configuration)
    {
        // Updated to use configuration injection for better practices
        connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string not found");
    }

    // Updated for PostgreSQL - same logic, different syntax
    public bool CheckNationalNumberExists(int nationalNumber)
    {
        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // PostgreSQL uses lowercase table/column names
                string query = "SELECT COUNT(*) FROM users WHERE national_number = @nationalNumber";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nationalNumber", nationalNumber);
                    
                    // PostgreSQL returns long, cast to int for compatibility
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch (NpgsqlException pgEx)
        {
            throw new Exception($"PostgreSQL error checking national number: {pgEx.Message}", pgEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error checking National Number existence: {ex.Message}", ex);
        }
    }

    // Updated for PostgreSQL schema and data types
    public EmpInfo GetEmployeeByNationalNumber(int nationalNumber)
    {
        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Updated column names to match PostgreSQL schema
                string query = @"SELECT id, username, national_number, email, phone, is_active
                                FROM users 
                                WHERE national_number = @nationalNumber";
                
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nationalNumber", nationalNumber);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            EmpInfo employee = new EmpInfo
                            {
                                // PostgreSQL returns bigint, cast to int for existing model compatibility
                                EmpId = (int)reader.GetInt64("id"),
                                UserName = reader.GetString("username"),
                                NationalNumber = (int)reader.GetInt64("national_number"),
                                Email = reader.GetString("email"),
                                PhoneNumber = reader.GetString("phone"),
                                IsActive = reader.GetBoolean("is_active"),
                                Salaries = new List<SalaryInfo>()
                            };
                            return employee;
                        }
                    }
                }
            }
            return null;
        }
        catch (NpgsqlException pgEx)
        {
            throw new Exception($"PostgreSQL error retrieving employee: {pgEx.Message}", pgEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving employee information: {ex.Message}", ex);
        }
    }

    // Updated for PostgreSQL schema with proper month ordering
    public List<SalaryInfo> GetEmployeeSalaries(int employeeId)
    {
        List<SalaryInfo> salaries = new List<SalaryInfo>();
        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Updated column names and proper month ordering for PostgreSQL
                string query = @"SELECT id, year, month, salary, user_id
                                FROM salaries
                                WHERE user_id = @UserId
                                ORDER BY year DESC, 
                                        CASE month 
                                            WHEN 'January' THEN 1 WHEN 'February' THEN 2 WHEN 'March' THEN 3
                                            WHEN 'April' THEN 4 WHEN 'May' THEN 5 WHEN 'June' THEN 6
                                            WHEN 'July' THEN 7 WHEN 'August' THEN 8 WHEN 'September' THEN 9
                                            WHEN 'October' THEN 10 WHEN 'November' THEN 11 WHEN 'December' THEN 12
                                        END DESC";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", employeeId);

                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SalaryInfo salary = new SalaryInfo
                            {
                                SalaryId = (int)reader.GetInt64("id"),
                                Year = reader.GetInt32("year"),
                                Month = reader.GetString("month"),
                                Salary = reader.GetDecimal("salary"),
                                EmployeeId = (int)reader.GetInt64("user_id"),
                            };
                            salaries.Add(salary);
                        }
                    }
                }
            }
        }
        catch (NpgsqlException pgEx)
        {
            throw new Exception($"PostgreSQL error retrieving salaries: {pgEx.Message}", pgEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving employee salaries: {ex.Message}", ex);
        }
        return salaries;
    }
}