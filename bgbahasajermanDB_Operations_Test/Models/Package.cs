using static bgbahasajermanDB_Libraries.MySqlQueryManager;

namespace bgbahasajermanDB_Operations_Test.Models
{
    public class Package
    {
        public string PackageID { get; set; }
        public int PackageNumber { get; set; } //oldPackage = already   //newPackage = not yet
        public bool PackageCompleted { get; set; }
        public int Intensity { get; set; } //oldPackage = already   //newPackage = not yet
        public int OutstandingLessons { get; set; }
        public int CompletedLessons { get; set; }
        public DateTime PaymentDateTime { get; set; }
        public string PaymentDateTimeString { get; set; }
        public List<DateTime> PackageLessonDates { get; set; } = new List<DateTime>(); //oldPackage = already   //newPackage = not yet
        //public List<DateTime> SortedPackageLessonDates { get; set; } = new List<DateTime>();
        public List<string> BookingIDs { get; set; } = new List<string>();

        public List<int> WeekdaysInt { get; set; } = new List<int>();
        public List<string> Weekdays { get; set; } = new List<string>();
        public List<string> Times { get; set; } = new List<string>();
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
        
        // this could probably be just a check method whether the list of lessons is still sorted..
        public List<Lesson> SortedPackageLessons { get; set; } = new List<Lesson>(); 

        public Package()
        {

        }
        public Package(Student student)
        {

        }

        // Sorting is handled in the query execution method*****
        //public void CreateSortedDatesList()
        //{
        //    // Sort the PackageLessonDates and populate SortedPackageLessonDates
        //    SortedPackageLessonDates = PackageLessonDates.OrderBy(date => date).ToList();

        //    // Print the sorted dates
        //    Console.WriteLine("Sorted Package Lesson Dates:");
        //    foreach (var date in SortedPackageLessonDates)
        //    {
        //        Console.WriteLine(date.ToString("yyyy-MM-dd HH:mm:ss"));
        //    }
        //}
    }
}
