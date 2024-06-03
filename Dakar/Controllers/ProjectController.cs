using Dakar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Dakar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase {

        private readonly IConfiguration _configuration;
        public ProjectController(IConfiguration configuration) {

            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Project> projectList = new List<Project>();
                string query = @"Select ProjectId, ProjectDescription, Active FROM dbo.Projects;";
                string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        using (SqlDataReader myReader = myCommand.ExecuteReader())
                        {
                            while (myReader.Read())
                            {
                                Project project = new Project()
                                {
                                    ProjectID = Convert.ToInt32(myReader["ProjectId"]),
                                    ProjectDescription = myReader["ProjectDescription"].ToString(),
                                    Active = Convert.ToBoolean(myReader["Active"])
                                };
                                projectList.Add(project);
                            }
                        }
                    }
                }

                return Ok(projectList);
            }
            catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
        
        [HttpPost]
        public IActionResult Post(Project proj)
        {
            string query = @"INSERT INTO dbo.Projects (ProjectDescription, Active) VALUES (@ProjectDescription, @Active);";
            string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

            int rowsAffected = 0;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProjectDescription", proj.ProjectDescription);
                    // Assuming Active is a boolean column, you might want to set it to true for new projects
                    myCommand.Parameters.AddWithValue("@Active", true);

                    rowsAffected = myCommand.ExecuteNonQuery();
                }
            }

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Project added successfully." });
            }
            else
            {
                return BadRequest(new { message = "Failed to add the project." });
            }
        }

        [HttpPut]
        public IActionResult Put(Project proj)
        {
            string query = @"UPDATE dbo.Projects SET ProjectDescription = @ProjectDescription WHERE ProjectID = @ProjectID";
            string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

            int rowsAffected = 0;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProjectID", proj.ProjectID);
                    myCommand.Parameters.AddWithValue("@ProjectDescription", proj.ProjectDescription);
                    // Assuming Active is a boolean column, you might want to set it to true for new projects
                    myCommand.Parameters.AddWithValue("@Active", true);

                    rowsAffected = myCommand.ExecuteNonQuery();
                }
            }

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Project updated successfully." });
            }
            else
            {
                return BadRequest(new { message = "Failed to update the project." });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            string query1 = @"DELETE FROM dbo.Projects WHERE ProjectID = @ProjectID";
            string query2 = @"DELETE FROM dbo.TestCases WHERE ProjectID = @ProjectID";
            string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

            int rowsAffected = 0;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlTransaction transaction = myCon.BeginTransaction())
                {
                    try
                    {

                        using (SqlCommand myCommand = new SqlCommand(query1, myCon, transaction))
                        {
                            myCommand.Parameters.AddWithValue("@ProjectID", id);

                            rowsAffected = myCommand.ExecuteNonQuery();
                        }

                        using (SqlCommand myCommand = new SqlCommand(query2, myCon, transaction))
                        {
                            myCommand.Parameters.AddWithValue("@ProjectID", id);

                            rowsAffected = myCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if command fails
                        transaction.Rollback();
                        return BadRequest(new { message = "Failed to delete the Project.", error = ex.Message });
                    }
                }
            }

            if (rowsAffected >= 0)
            {
                return Ok(new { message = "Project and related test cases are deleted successfully." });
            }
            else
            {
                return BadRequest(new { message = "Failed to remove Project and related test." });
            }

        }

    }
}
