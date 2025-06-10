
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using static bgbahasajermanDB_Libraries.DateFormatting;
using static bgbahasajermanDB_Libraries.MySqlQueryManager;


namespace bgbahasajermanDB_ReplacementsManager_Test.Models
{
    public class LessonPackageModel
    {
        //public List<DateTime> LessonDates { get; set; } = new List<DateTime>();
        public string StudentID { get; set; }
        public int PackageNumber { get; set; }
        public int PackageID { get; set; }

        
        public LessonPackageModel(string studentID, int packageNumber) 
        {


        }
    }
}
