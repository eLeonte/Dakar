namespace Dakar.Models
{
    public class TestCaseSteps
    {

        public int StepID { get; set; }
        public int ScenarioID { get; set; }
        public int StepOrder { get; set; }
        public string StepDescription { get; set; }
        public string ExpectedResult { get; set; }
        public int TestCaseId { get; set; }
        public string TestCaseName { get; set; }
    }
}
