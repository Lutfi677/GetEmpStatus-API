using GetEmpStatus.DataAccessLayer;
using GetEmpStatus.Models;

namespace GetEmpStatus.BusinessLayer;

public class ProcessStatus
{
    private readonly DataAccess _dataAccess;

    public ProcessStatus(DataAccess dataAccess)
    {
        _dataAccess = dataAccess;
        
    }
    //The primary entry point for all employee status requests.
    public object ProcessEmployeeRequest(int nationalNumber)
    {
        try
        {   //1.Validates employee's national number.
            if (!CheckNationalNumberExists(nationalNumber))
            {
                return new
                {
                    success = false,
                    message = "Invalid National Number - Employee not found",
                    data = (object)null 
                };
            }
            //2.Data retrieval from DataAccess Layer
            EmpInfo employee = GetEmployeeInformation(nationalNumber);

            if (employee == null)
            {
                return new
                {
                    success = false,
                    message = "Employee information could not be found!",
                    data = (object)null
                };
            }
            //3.Verify if employee is active in the system.
            bool isActive = CheckEmployeeActiveStatus(employee);
            
            //4.Calculate all salary metrics.
            CalculateEmployeeSalaryStatistics(employee);
            
            //5.Calculate performance status.
            string status = CalculateEmployeeStatus(employee.TotalSalary);
            employee.Status = status;
            
            //Response format.
            return new
            {
                success = true,
                message = "Employee information retrieved successfully!",
                data = new
                {
                     employeeId = employee.EmpId,
                     username = employee.UserName,
                     nationalNumber = employee.NationalNumber,
                     email = employee.Email,
                     phone = employee.PhoneNumber,
                     isActive = employee.IsActive,
                     highestSalary = employee.HighestSalary,
                     averageSalary = Math.Round(employee.AverageSalary, 2),
                     totalSalary = employee.TotalSalary,
                     status = employee.Status,
                     salaryCount = employee.Salaries.Count
                }
            };
          
        }
        
        catch (Exception ex)
        {
            return new
            {
                success = false,
                message = "Error while processing employee request: " + ex.Message,
                data = (object)null
            };
        }
    }
    
    //1
    public bool CheckNationalNumberExists(int nationalNumber)
    {
        try
        {
            return _dataAccess.CheckNationalNumberExists(nationalNumber);
        }
        catch (Exception ex)
        {
           throw new Exception("Error checking National Number existence: " + ex.Message);
        }
    }
    
    //2
    public EmpInfo GetEmployeeInformation(int nationalNumber)
    {
        try
        {
            EmpInfo employee = _dataAccess.GetEmployeeByNationalNumber(nationalNumber);

            if (employee != null)
            {
                employee.Salaries = _dataAccess.GetEmployeeSalaries(employee.EmpId);
            }
            return employee;
        }
        catch (Exception ex)
        {
            throw new Exception("Error getting employee information: " + ex.Message);
        }
    }
    
    //3
    public bool CheckEmployeeActiveStatus(EmpInfo employee)
    {
        try
        {
            if (employee == null)
            {
                throw new ArgumentNullException("Employee object cannot be null");
            }

            return employee.IsActive;
        }
        catch (Exception ex)
        {
           throw new Exception("Error checking employee active status:  " + ex.Message);
        }
    }
    
    //4
    public void CalculateEmployeeSalaryStatistics(EmpInfo employee)
    {
        try
        {
            if (employee == null)
            {
                throw new ArgumentNullException("Employee object cannot be null");
            }

            if (employee.Salaries == null || employee.Salaries.Count == 0)
            {
                employee.HighestSalary = 0;
                employee.AverageSalary = 0;
                employee.TotalSalary = 0;
                return;
            }

            List<decimal> salaryAmounts = new List<decimal>();

            foreach (var salary in employee.Salaries)
            {
                salaryAmounts.Add(salary.Salary);
            }
        
            employee.HighestSalary = salaryAmounts.Max();
        
            employee.AverageSalary = salaryAmounts.Average();
        
            employee.TotalSalary = salaryAmounts.Sum();
        }
        catch (Exception ex)
        {
            
            throw new Exception("Error calculating employee salary statistics: " + ex.Message);
        }
    }
    
    //5
    public string CalculateEmployeeStatus(decimal totalSalary)
    {
        try
        {
            if (totalSalary > 2000)
            {
                return "GREEN";
            }
            else if (totalSalary < 2000)
            {
                return "RED";
            }
            else
            {
                return "ORANGE";
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error calculating employee status: " + ex.Message);
        }
    }


}