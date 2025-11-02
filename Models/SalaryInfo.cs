using System;

namespace GetEmpStatus.Models;

public class SalaryInfo
{
    public int SalaryId { get; set; }
    public int Year { get; set; }
    public string Month { get; set; } =  string.Empty;
    public decimal Salary { get; set; }
    public int EmployeeId { get; set; } 
}