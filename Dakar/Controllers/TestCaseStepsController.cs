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


    }
}
