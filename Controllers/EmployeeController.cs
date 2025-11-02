using GetEmpStatus.BusinessLayer;
using Microsoft.AspNetCore.Mvc;

namespace GetEmpStatus.Controllers;


[ApiController ]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeeController : ControllerBase
{
    private readonly ProcessStatus _processStatus;

    public EmployeeController(ProcessStatus processStatus)
    {
        _processStatus = processStatus;
    }
    
    //Retrieves complete employee status information.
    [HttpGet("status/{nationalNumber}")]
    public async Task<IActionResult> GetEmployeeStatus(string nationalNumber)
    {
        try
        {   //Check if parameter is null or empty.
            if (string.IsNullOrEmpty(nationalNumber))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "National number is required",
                    timestamp = DateTime.Now.ToString("yy-MM-dd HH:mm:ss")
                });
            }
            
            //Ensure national number is numeric of type integer.
            if (!int.TryParse(nationalNumber, out int nat))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid National Number format. Please enter a valid numeric National Number.",
                    timestamp = DateTime.Now.ToString("yy-MM-dd HH:mm:ss")
                });

            }
            //Delegate the processing task to the business layer.
            var result = await Task.Run(() => _processStatus.ProcessEmployeeRequest(nat));
            
            //Map business layer results to appropriate HTTP response.
            var resultType = result.GetType();
            var successProperty = resultType.GetProperty("success");
            
            //if HTTP 200
            if (successProperty != null && (bool)successProperty.GetValue(result))
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
                
            }
        }
        
        //Server-side error
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "An error occured while processing the request",
                error =  ex.Message,
                timestamp = DateTime.Now.ToString("yy-MM-dd HH:mm:ss")
            });
        }
    }
    
    //Verify API service availability and status.
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            success = true,
            message = "GetEmpStatus Web API is running successfully",
            timestamp = DateTime.Now.ToString("yy-MM-dd HH:mm:ss"),
            version = "1.0.0",
            framework = ".NET 9.0",
            sqlClient = "Microsoft.Data.SqlClient 6.1.2",
        });
    }
    
    
    //Provides API documentation and testing guidance.
    [HttpGet("info")]
    public IActionResult GetServiceInfo()
    {
        return Ok(new
            {
                success = true,
                message = "Service information retrieved successfully",
                version = "1.0.0",
                framework = ".NET 9.0",
                description = "Employee Status Management Web API",
                endpoints = new[]
                {
                    new
                    {
                        method = "GET",
                        path = "/api/employee/status/{nationalNumber}",
                        description ="Get employee status by National Number",
                    },
                    new
                    {
                        method = "GET",
                        path = "/api/employee/health", 
                        description = "Health check endpoint" ,
                    },
                    new
                    {
                        method = "GET", 
                        path = "/api/employee/info", 
                        description = "Service information" 
                    }
                },
                testCases = new[]
                {
                    new
                    {
                        nationalNumber = "123456789",
                        expectedStatus = "GREEN",
                        description = "John Doe - High Performer",
                    },
                    new
                    {
                        nationalNumber = "987654321", 
                        expectedStatus = "RED", 
                        description = "Jane Smith - Needs Improvement" 
                    },
                    new { 
                        nationalNumber = "555666777", 
                        expectedStatus = "ORANGE", 
                        description = "Ahmed Hassan - Meets Minimum" 
                    },
                    new { 
                        nationalNumber = "444555666", 
                        expectedStatus = "RED", 
                        description = "Mike Johnson - Inactive Employee" 
                    },
                    new { 
                        nationalNumber = "999999999", 
                        expectedStatus = "ERROR", 
                        description = "Invalid National Number" 
                    }
                    
                },
                statusLegend = new
                {
                    GREEN = "Total salary > $2000 (High Performer)",
                    RED = "Total salary < $2000 (Needs Improvement)",
                    ORANGE = "Total salary = $2000 (Meets Minimum)"
                },
                timestamp =  DateTime.Now.ToString("yy-MM-dd HH:mm:ss")
                
                
            });
    }

}