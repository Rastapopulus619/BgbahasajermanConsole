using static bgbahasajermanDB_Libraries.MySqlQueryManager;

namespace bgbahasajermanDB_Operations.Models
{
    public class Package
    {
        public int PackageID { get; set; }
        public int PackageNumber { get; set; } //oldPackage = already   //newPackage = not yet
        public bool PackageCompleted { get; set; }
        public int Intensity { get; set; } //oldPackage = already   //newPackage = not yet
        public int OutstandingLessons { get; set; }
        public int CompletedLessons { get; set; }
        public int PayedPrice { get; set; } // ONLY FOR OLDPACKAGE***
        public int RealTotalPrice { get; set; } // ONLY FOR OLDPACKAGE***
        public int OverpaymentAmount { get; set; } // ONLY FOR OLDPACKAGE***
        public DateTime PaymentDateTime { get; set; } // only for new Package Insertion ***
        public string PaymentDateTimeString { get; set; } // only for new Package Insertion ***
        public List<DateTime> PackageLessonDates { get; set; } = new List<DateTime>(); //oldPackage = already   //newPackage = not yet
        //public List<DateTime> SortedPackageLessonDates { get; set; } = new List<DateTime>();
        public List<int> BookingIDs { get; set; } = new List<int>();

        public List<int> WeekdaysInt { get; set; } = new List<int>();
        public List<string> Weekdays { get; set; } = new List<string>();
        public List<string> Times { get; set; } = new List<string>();
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
        
        // this could probably be just a check method whether the list of lessons is still sorted..
        public List<Lesson> SortedPackageLessons { get; set; } = new List<Lesson>();
        public Lesson SelectedLesson { get; set; } // Selected Lesson is user-defined
        public SortedDictionary<int, string[]> WeekdaysAndTimes { get; set; } = new SortedDictionary<int, string[]>();

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
