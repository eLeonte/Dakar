using Dakar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Dakar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestCaseStepsController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        public TestCaseStepsController(IConfiguration configuration)
        {

            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                List<TestCaseSteps> testCaseStepsList = new List<TestCaseSteps>();
                string query = @"
SELECT 
    TS.StepID,
    TS.ScenarioID,
    TS.StepOrder,
    TS.StepDescription,
    TS.ExpectedResult,
    TC.TestCaseId,
    TC.TestCaseName
FROM 
    [dbo].[TestSteps] TS
JOIN 
    [dbo].[TestCases] TC ON TS.ScenarioID = TC.TestCaseId
WHERE 
    TC.TestCaseId = @TestCaseId";

                string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@TestCaseId", id);
                        using (SqlDataReader myReader = myCommand.ExecuteReader())
                        {
                            while (myReader.Read())
                            {
                                TestCaseSteps testCaseSteps = new TestCaseSteps()
                                {
                                    StepID = Convert.ToInt32(myReader["StepID"]),
                                    ScenarioID = Convert.ToInt32(myReader["ScenarioID"]),
                                    StepOrder = Convert.ToInt32(myReader["StepOrder"]),
                                    StepDescription = myReader["StepDescription"].ToString(),
                                    ExpectedResult = myReader["ExpectedResult"].ToString(),
                                    TestCaseId = Convert.ToInt32(myReader["TestCaseId"]),
                                    TestCaseName = myReader["TestCaseName"].ToString(),
                                };
                                testCaseStepsList.Add(testCaseSteps);
                            }
                        }
                    }
                    myCon.Close();
                }

                return Ok(testCaseStepsList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public IActionResult Post(TestCaseSteps step)
        {

            List<TestCaseSteps> testCaseStepsList = new List<TestCaseSteps>();
            string query = @"INSERT INTO [dbo].[TestSteps] (ScenarioID, StepOrder, StepDescription, ExpectedResult) VALUES (@ScenarioID, @StepOrder, @StepDescription, @ExpectedResult)";
            string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

            int rowsAffected = 0;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ScenarioID", step.ScenarioID);
                    myCommand.Parameters.AddWithValue("@StepOrder", step.StepOrder);
                    myCommand.Parameters.AddWithValue("@StepDescription", step.StepDescription);
                    myCommand.Parameters.AddWithValue("@ExpectedResult", step.ExpectedResult);

                    rowsAffected = myCommand.ExecuteNonQuery();
                }
            }

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Test Case Step added successfully." });
            }
            else
            {
                return BadRequest(new { message = "Failed to add the Test Case Step." });
            }

        }

        [HttpDelete("{id}/{stepID}")]
        public IActionResult Delete(int id, int stepID)
        {

            string query = @"DELETE FROM [dbo].[TestSteps] WHERE [ScenarioID] = @ScenarioID AND [StepID] = @StepID";
            string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

            int rowsAffected = 0;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ScenarioID", id);
                    myCommand.Parameters.AddWithValue("@StepID", stepID);

                    rowsAffected = myCommand.ExecuteNonQuery();
                }
            }

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Test step removed successfully." });
            }
            else
            {
                return BadRequest(new { message = "Failed to remove the test step." });
            }

        }

        [HttpPut]
        public IActionResult Put(TestCaseSteps step)
        {

            string query = @"Update dbo.[TestSteps] SET StepDescription = @StepDescription, ExpectedResult = @ExpectedResult WHERE StepId = @StepID";
            string sqlDataSource = _configuration.GetConnectionString("DakarAppCon");

            int rowsAffected = 0;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@StepDescription", step.StepDescription);
                    myCommand.Parameters.AddWithValue("@ExpectedResult", step.ExpectedResult);
                    myCommand.Parameters.AddWithValue("@StepID", step.StepID);

                    rowsAffected = myCommand.ExecuteNonQuery();
                }
            }

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Test step updated successfully." });
            }
            else
            {
                return BadRequest(new { message = "Failed to update the Test step." });
            }

        }

    }
}
