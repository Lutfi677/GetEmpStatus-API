using System;
using System.Collections.Generic;

namespace GetEmpStatus.Models;

public class EmpInfo
{
    public int EmpId { get; set; }
    public  string UserName  { get; set; } = string.Empty;
    public int NationalNumber { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<SalaryInfo> Salaries { get; set; }
    public decimal HighestSalary { get; set; }
    public decimal AverageSalary { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalSalary { get; set; }
    
}