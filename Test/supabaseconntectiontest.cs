using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace GetEmpStatus.Test
{
    public static class SupabaseConnectionTest
    {
        private static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            return configuration.GetConnectionString("DefaultConnection")!;
        }

        // Quick test to verify Supabase connectivity
        public static bool QuickConnectionTest()
        {
            Console.WriteLine("Supabase PostgreSQL Connection Test");
            Console.WriteLine("===================================");
            
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    Console.WriteLine("✅ Supabase connection successful!");
                    Console.WriteLine($"   Host: {connection.Host}");
                    Console.WriteLine($"   Database: {connection.Database}");
                    Console.WriteLine($"   PostgreSQL Version: {connection.PostgreSqlVersion}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection failed: {ex.Message}");
                return false;
            }
        }

        // Comprehensive testing of your GetEmpStatus data and queries
        public static void ComprehensiveTest()
        {
            Console.WriteLine("\nComprehensive Supabase Test");
            Console.WriteLine("===========================");
            
            // Test 1: Basic Connection
            if (!TestBasicConnection())
            {
                Console.WriteLine("Basic connection failed. Stopping tests.");
                return;
            }

            // Test 2: Table Structure
            TestTableStructure();

            // Test 3: Data Verification
            TestDataIntegrity();

            // Test 4: GetEmpStatus API Query Simulation
            TestGetEmpStatusQueries();

            Console.WriteLine("\n🎉 Comprehensive test completed successfully!");
        }

        // Test Basic Supabase Connection
        private static bool TestBasicConnection()
        {
            Console.WriteLine("\nTest 1: Basic Connection");
            Console.WriteLine("------------------------");
            
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    Console.WriteLine("✅ Connection established successfully");
                    Console.WriteLine($"   Host: {connection.Host}");
                    Console.WriteLine($"   Database: {connection.Database}");
                    Console.WriteLine($"   PostgreSQL Version: {connection.PostgreSqlVersion}");
                    return true;
                }
            }
            catch (NpgsqlException pgEx)
            {
                Console.WriteLine($"❌ PostgreSQL Error: {pgEx.Message}");
                Console.WriteLine($"   Error Code: {pgEx.SqlState}");
                PrintSupabaseErrorSolution(pgEx.SqlState);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ General Error: {ex.Message}");
                return false;
            }
        }

        // Test Supabase Table Structure
        private static void TestTableStructure()
        {
            Console.WriteLine("\nTest 2: Table Structure");
            Console.WriteLine("-----------------------");
            
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    
                    // Check if required tables exist in public schema
                    string query = @"
                        SELECT table_name 
                        FROM information_schema.tables 
                        WHERE table_schema = 'public' 
                        AND table_type = 'BASE TABLE'
                        ORDER BY table_name";
                    
                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            var tables = new List<string>();
                            while (reader.Read())
                            {
                                tables.Add(reader.GetString("table_name"));
                            }
                            
                            Console.WriteLine($"Found {tables.Count} tables:");
                            foreach (string table in tables)
                            {
                                Console.WriteLine($"   • {table}");
                            }
                            
                            // Check for required tables
                            bool hasUsers = tables.Contains("users");
                            bool hasSalaries = tables.Contains("salaries");
                            
                            if (hasUsers && hasSalaries)
                            {
                                Console.WriteLine("✅ All required tables present");
                            }
                            else
                            {
                                if (!hasUsers) Console.WriteLine("❌ Missing: users table");
                                if (!hasSalaries) Console.WriteLine("❌ Missing: salaries table");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Table structure test failed: {ex.Message}");
            }
        }

        // Test Data Integrity
        private static void TestDataIntegrity()
        {
            Console.WriteLine("\nTest 3: Data Integrity");
            Console.WriteLine("----------------------");
            
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    
                    // Test Users table data
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT COUNT(*) FROM users", connection))
                    {
                        long userCount = (long)command.ExecuteScalar();
                        Console.WriteLine($"Users table: {userCount} records");
                        
                        if (userCount == 0)
                        {
                            Console.WriteLine("⚠️  Warning: No users found in database");
                        }
                        else if (userCount == 10)
                        {
                            Console.WriteLine("✅ Expected 10 users found");
                        }
                    }
                    
                    // Test Salaries table data
                    using (NpgsqlCommand command = new NpgsqlCommand("SELECT COUNT(*) FROM salaries", connection))
                    {
                        long salaryCount = (long)command.ExecuteScalar();
                        Console.WriteLine($"Salaries table: {salaryCount} records");
                    }
                    
                    // Test specific employee data
                    using (NpgsqlCommand command = new NpgsqlCommand(
                        "SELECT id, username FROM users WHERE national_number = 123456789", connection))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                long id = reader.GetInt64("id");
                                string username = reader.GetString("username");
                                Console.WriteLine($"✅ Test employee found: ID={id}, Username={username}");
                            }
                            else
                            {
                                Console.WriteLine("❌ Test employee (NAT: 123456789) not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Data integrity test failed: {ex.Message}");
            }
        }

        // Test GetEmpStatus API Query Simulation
        private static void TestGetEmpStatusQueries()
        {
            Console.WriteLine("\nTest 4: GetEmpStatus API Query Simulation");
            Console.WriteLine("-----------------------------------------");
            
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    
                    // Test 1: CheckNationalNumberExists simulation
                    Console.WriteLine("Testing: CheckNationalNumberExists query");
                    using (NpgsqlCommand command = new NpgsqlCommand(
                        "SELECT COUNT(*) FROM users WHERE national_number = @nationalNumber", connection))
                    {
                        command.Parameters.AddWithValue("@nationalNumber", 123456789);
                        long count = (long)command.ExecuteScalar();
                        Console.WriteLine($"✅ NAT existence check: {count > 0} (Count: {count})");
                    }
                    
                    // Test 2: GetEmployeeByNationalNumber simulation
                    Console.WriteLine("Testing: GetEmployeeByNationalNumber query");
                    using (NpgsqlCommand command = new NpgsqlCommand(
                        "SELECT id, username, national_number, email, phone, is_active FROM users WHERE national_number = @nationalNumber", connection))
                    {
                        command.Parameters.AddWithValue("@nationalNumber", 123456789);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("✅ Employee retrieval: Success");
                                Console.WriteLine($"   ID: {reader.GetInt64("id")}");
                                Console.WriteLine($"   Username: {reader.GetString("username")}");
                                Console.WriteLine($"   Email: {reader.GetString("email")}");
                                Console.WriteLine($"   Active: {reader.GetBoolean("is_active")}");
                            }
                            else
                            {
                                Console.WriteLine("❌ Employee retrieval: Failed");
                            }
                        }
                    }
                    
                    // Test 3: Salary calculation simulation
                    Console.WriteLine("Testing: Salary calculation query");
                    using (NpgsqlCommand command = new NpgsqlCommand(@"
                        SELECT 
                            COUNT(*) as salary_count,
                            SUM(salary) as total_salary,
                            MAX(salary) as highest_salary,
                            AVG(salary) as average_salary
                        FROM salaries s
                        INNER JOIN users u ON s.user_id = u.id
                        WHERE u.national_number = @nationalNumber", connection))
                    {
                        command.Parameters.AddWithValue("@nationalNumber", 123456789);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("✅ Salary calculation: Success");
                                Console.WriteLine($"   Salary Records: {reader.GetInt64("salary_count")}");
                                Console.WriteLine($"   Total Salary: ${reader.GetDecimal("total_salary")}");
                                Console.WriteLine($"   Highest Salary: ${reader.GetDecimal("highest_salary")}");
                                Console.WriteLine($"   Average Salary: ${Math.Round(reader.GetDecimal("average_salary"), 2)}");
                                
                                decimal totalSalary = reader.GetDecimal("total_salary");
                                string status = totalSalary > 2000 ? "GREEN" : (totalSalary < 2000 ? "RED" : "ORANGE");
                                Console.WriteLine($"   Expected Status: {status}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ API query simulation failed: {ex.Message}");
            }
        }

        // Test Multiple Employees
        public static void TestMultipleEmployees()
        {
            Console.WriteLine("\nTesting Multiple Employee Scenarios");
            Console.WriteLine("===================================");
            
            var testCases = new[]
            {
                new { NAT = 123456789, Name = "John Doe", ExpectedStatus = "GREEN" },
                new { NAT = 987654321, Name = "Jane Smith", ExpectedStatus = "RED" },
                new { NAT = 555666777, Name = "Ahmed Hassan", ExpectedStatus = "ORANGE" },
                new { NAT = 444555666, Name = "Mike Johnson", ExpectedStatus = "RED" },
                new { NAT = 999999999, Name = "Non-existent", ExpectedStatus = "NOT_FOUND" }
            };

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    
                    foreach (var testCase in testCases)
                    {
                        Console.WriteLine($"\nTesting: {testCase.Name} (NAT: {testCase.NAT})");
                        
                        // Check if employee exists
                        using (NpgsqlCommand command = new NpgsqlCommand(
                            "SELECT COUNT(*) FROM users WHERE national_number = @nat", connection))
                        {
                            command.Parameters.AddWithValue("@nat", testCase.NAT);
                            long exists = (long)command.ExecuteScalar();
                            
                            if (exists == 0)
                            {
                                Console.WriteLine($"   Result: NOT_FOUND (Expected: {testCase.ExpectedStatus})");
                                Console.WriteLine($"   Status: {(testCase.ExpectedStatus == "NOT_FOUND" ? "✅ PASS" : "❌ FAIL")}");
                                continue;
                            }
                        }
                        
                        // Calculate total salary
                        using (NpgsqlCommand command = new NpgsqlCommand(@"
                            SELECT COALESCE(SUM(salary), 0) as total_salary
                            FROM salaries s
                            INNER JOIN users u ON s.user_id = u.id
                            WHERE u.national_number = @nat", connection))
                        {
                            command.Parameters.AddWithValue("@nat", testCase.NAT);
                            decimal totalSalary = (decimal)command.ExecuteScalar();
                            
                            string actualStatus = totalSalary > 2000 ? "GREEN" : (totalSalary < 2000 ? "RED" : "ORANGE");
                            Console.WriteLine($"   Total Salary: ${totalSalary}");
                            Console.WriteLine($"   Actual Status: {actualStatus}");
                            Console.WriteLine($"   Expected Status: {testCase.ExpectedStatus}");
                            Console.WriteLine($"   Result: {(actualStatus == testCase.ExpectedStatus ? "✅ PASS" : "❌ FAIL")}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Multiple employee test failed: {ex.Message}");
            }
        }

        // Print Supabase Error Solutions
        private static void PrintSupabaseErrorSolution(string? sqlState)
        {
            Console.WriteLine("\nSuggested Solutions:");
            switch (sqlState)
            {
                case "08006":
                    Console.WriteLine("   • Check your internet connection");
                    Console.WriteLine("   • Verify Supabase project is active");
                    Console.WriteLine("   • Check connection string format");
                    break;
                case "28P01":
                    Console.WriteLine("   • Check your Supabase password");
                    Console.WriteLine("   • Verify project reference in connection string");
                    Console.WriteLine("   • Ensure database user has proper permissions");
                    break;
                case "3D000":
                    Console.WriteLine("   • Database 'postgres' should exist by default");
                    Console.WriteLine("   • Check your project reference");
                    Console.WriteLine("   • Verify Supabase project is properly set up");
                    break;
                default:
                    Console.WriteLine("   • Check your Supabase connection string");
                    Console.WriteLine("   • Verify Supabase project is running");
                    Console.WriteLine("   • Check firewall/proxy settings");
                    Console.WriteLine("   • Ensure SSL Mode=Require is set");
                    break;
            }
        }
    }
}