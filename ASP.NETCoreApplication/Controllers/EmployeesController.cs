using ASP.NETCoreApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace ASP.NETCoreApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public EmployeesController(IConfiguration configuration)
        {

            _configuration = configuration; 

        }

        [HttpGet]
        [Route("GetAllEmployees")]
        public string GetEmployees()
        {
            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT EmpId, EmpName, Password, DateTime FROM Employees", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Employee> employeeList = new List<Employee>();
            Response response = new Response();
            if (dt.Rows.Count > 0)
            {
                for(int i=0; i<dt.Rows.Count; i++)
                {
                    Employee employee = new Employee();
                    employee.EmpId = Convert.ToInt32(dt.Rows[i]["EmpId"]);
                    employee.EmpName = Convert.ToString(dt.Rows[i]["EmpName"]);
                    employee.Password = Convert.ToString(dt.Rows[i]["Password"]);
                    employee.DateTime = Convert.ToDateTime(dt.Rows[i]["DateTime"]);
                    employeeList.Add(employee); 
                }
            }
            if (employeeList.Count > 0)
                return JsonConvert.SerializeObject(employeeList);
            else
            {
                response.StatusCode = 100;
                response.ErrorMessage = "No data found";
                return JsonConvert.SerializeObject(response);
            }
        }

        [HttpPost]
        [Route("AddEmployee")]
        public IActionResult AddEmployee([FromBody] Employee employee)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon")))
                {
                    con.Open();
                    string query = "INSERT INTO Employees (EmpId, EmpName, Password, DateTime)";
                        query += " VALUES (@EmpId, @EmpName, @Password, @DateTime)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmpId", employee.EmpId);
                        cmd.Parameters.AddWithValue("@EmpName", employee.EmpName);
                        cmd.Parameters.AddWithValue("@Password", employee.Password);
                        cmd.Parameters.AddWithValue("@DateTime", employee.DateTime);

                        cmd.ExecuteNonQuery();
                        return Ok(new Response { StatusCode = 200, Message = "Employee added successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { StatusCode = 500, ErrorMessage = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateEmployee/{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employee updatedEmployee)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon")))
                {
                    con.Open();
                    string query = "UPDATE Employees SET EmpName = @EmpName, Password = @Password, DateTime = @DateTime WHERE EmpId = @EmpId";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmpId", id);
                        cmd.Parameters.AddWithValue("@EmpName", updatedEmployee.EmpName);
                        cmd.Parameters.AddWithValue("@Password", updatedEmployee.Password);
                        cmd.Parameters.AddWithValue("@DateTime", updatedEmployee.DateTime);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return Ok(new Response { StatusCode = 200, Message = "Employee updated successfully" });
                        else
                            return NotFound(new Response { StatusCode = 100, ErrorMessage = "Employee not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { StatusCode = 500, ErrorMessage = ex.Message });
            }
        }

        [HttpDelete]
        [Route("DeleteEmployee/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("EmployeeAppCon")))
                {
                    con.Open();
                    string query = "DELETE FROM Employees WHERE EmpId = @EmpId";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@EmpId", id);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return Ok(new Response { StatusCode = 200, Message = "Employee deleted successfully" });
                        else
                            return NotFound(new Response { StatusCode = 100, ErrorMessage = "Employee not found" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response { StatusCode = 500, ErrorMessage = ex.Message });
            }
        }
    }
}
