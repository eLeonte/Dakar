using Dakar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Dakar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestCaseController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public TestCaseController(IConfiguration configuration)
        {

            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                List<TestCase> testCaseList = new List<TestCase>();
                string query = @"
SELECT
TC.TestCaseId,
TC.TestCaseName,
PR.ProjectID
FROM 
    [dbo].[TestCases] TC
JOIN
    [dbo].[Projects] PR ON TC.ProjectID = PR.ProjectID
WHERE 
    TC.ProjectID = @ProjectID
ORDER BY TC.TestCaseId
";

                string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {

                        myCommand.Parameters.AddWithValue("@ProjectID", id);
                        using (SqlDataReader myReader = myCommand.ExecuteReader())
                        {
                            while (myReader.Read())
                            {
                                TestCase testCase = new TestCase()
                                {
                                    TestCaseId = Convert.ToInt32(myReader["TestCaseId"]),
                                    TestCaseName = myReader["TestCaseName"].ToString(),
                                    ProjectId = Convert.ToInt32(myReader["ProjectId"])
                                };
                                testCaseList.Add(testCase);
                            }
                        }
                    }
                    myCon.Close();
                }

                return Ok(testCaseList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public IActionResult Post(TestCase test){

            List<TestCase> testCaseList = new List<TestCase>();
            string query = @"INSERT INTO dbo.TestCases (TestCaseName, ProjectID) Values (@TestCaseName, @ProjectID)";
            string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

            int rowsAffected = 0;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@TestCaseName", test.TestCaseName);
                    myCommand.Parameters.AddWithValue("@ProjectID", test.ProjectId);

                    rowsAffected = myCommand.ExecuteNonQuery();
                }
            }

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Test Case added successfully." });
            }
            else
            {
                return BadRequest(new { message = "Failed to add the Test Case." });
            }

        }

           // functioneaza asa, valideaza mai tarziu daca merge la fel
        [HttpPut]
        public IActionResult Put(TestCase test)
        {

            string query = @"Update dbo.TestCases SET TestCaseName = @TestCaseName WHERE TestCaseId = @TestCaseId";
            string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

            int rowsAffected = 0;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@TestCaseId", test.TestCaseId);
                    myCommand.Parameters.AddWithValue("@TestCaseName", test.TestCaseName);

                    rowsAffected = myCommand.ExecuteNonQuery();
                }
            }

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Test Case updated successfully." });
            }
            else
            {
                return BadRequest(new { message = "Failed to update the Test Case." });
            }

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            string query1 = @"Delete FROM dbo.TestCases WHERE TestCaseId = @TestCaseId";
            string query2 = @"Delete FROM dbo.TestSteps WHERE ScenarioID = @ScenarioID";
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
                            myCommand.Parameters.AddWithValue("@TestCaseId", id);

                            rowsAffected = myCommand.ExecuteNonQuery();
                        }

                        using (SqlCommand myCommand = new SqlCommand(query2, myCon, transaction))
                        {
                            myCommand.Parameters.AddWithValue("@ScenarioID", id);

                            rowsAffected = myCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if command fails
                        transaction.Rollback();
                        return BadRequest(new { message = "Failed to add the Test Case Step or update related data.", error = ex.Message });
                    }
                }
            }

            if (rowsAffected >= 0)
            {
                return Ok(new { message = "Test Case removed successfully." });
            }
            else
            {
                return BadRequest(new { message = "Failed to remove the Test Case." });
            }

        }
    }
}
