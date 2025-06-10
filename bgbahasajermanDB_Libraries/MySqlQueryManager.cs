using MySql.Data.MySqlClient;
using System.Collections.Generic;
//using MySqlDatabaseClass;
using System.Data;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using static bgbahasajermanDB_Libraries.DateFormatting;
using bgbahasajermanDB_Libraries.LessonDataModels;
using MySqlX.XDevAPI;

namespace bgbahasajermanDB_Libraries
{
    public static class MySqlQueryManager
    {
        // Private static field to hold the instance of bgbahasajermanDB
        private static readonly bgbahasajermanDB _dbInstance;

        // Static constructor to initialize the bgbahasajermanDB instance
        static MySqlQueryManager()
        {
            _dbInstance = bgbahasajermanDB.GetInstance();
        }

        // Hardcoded queries
        private static readonly Dictionary<string, string> Queries = new Dictionary<string, string>
        {
            // Add more queries as needed
            {"DisplayAllSlots", "CALL Slots()"},

            {"GetStudentIdByName", "SELECT StudentID FROM students WHERE Name = '@Name'"},
            {"GetTitle", "SELECT Title FROM students WHERE StudentID = @StudentID"},
            {"GetLevel", "SELECT Level FROM studydetails WHERE StudentID = @StudentID"},
            {"GetStudentNumberPlusOne","SELECT MAX(StudentNumber)+1 FROM students"},
            {"GetStudyStatus", "SELECT StudyStatus FROM studydetails WHERE StudentID = @StudentID"},
            {"CheckIfCurrentlyBooking","SELECT CASE WHEN EXISTS (SELECT 1 FROM bookedslots WHERE StudentID = @StudentID) THEN 1 ELSE 0 END"},
            {"GetCurrentIntensity","SELECT COUNT(*) FROM bookedslots WHERE StudentID = @StudentID"},
            {"GetPricingDetails","SELECT Currency, DiscountID from studentpricing WHERE StudentID = @StudentID"},
            {"GetDiscountAmount","SELECT Amount FROM discounts WHERE DiscountID = '@DiscountID'"},
            {"GetProductID","SELECT ProductID from pricelist WHERE Currency = '@Currency' AND Level = '@Level'"},
            {"GetOriginalLessonID","SELECT OriginalLessonID FROM rescheduledbookings WHERE BookingID = @BookingID"},
            {"CheckIfLessonBookingsExist","SELECT CASE WHEN EXISTS (SELECT 1 FROM lessonbookings WHERE StudentID = @StudentID) THEN TRUE ELSE FALSE END AS StudentExists"},

            {"GetAllLevels","SELECT Level FROM Levels"},
            {"GetLastPackagesLessonData","SELECT l.Date, lb1.BookingID, l.SlotID, t.StartTime, t.EndTime, t.Time, lessons_original.Date AS OriginalLessonDate, rb.Reason, t.WeekdayNum, l.LessonID FROM lessons l JOIN lessonbookings lb1 ON l.LessonID = lb1.LessonID JOIN ( SELECT p1.PackageID FROM packages p1 WHERE p1.StudentID = @StudentID ORDER BY p1.PackageNumber DESC LIMIT 1 ) latestPackage ON lb1.PackageID = latestPackage.PackageID LEFT JOIN timeslots t ON l.SlotID = t.SlotID LEFT JOIN rescheduledbookings rb ON lb1.BookingID = rb.BookingID LEFT JOIN lessons lessons_original ON rb.OriginalLessonID = lessons_original.LessonID ORDER BY l.Date ASC"},
            {"GetAnyPackagesLessonData","SELECT l.Date, lb1.BookingID, l.SlotID, t.StartTime, t.EndTime, t.Time, lessons_original.Date AS OriginalLessonDate, rb.Reason, t.WeekdayNum, l.LessonID FROM lessons l JOIN lessonbookings lb1 ON l.LessonID = lb1.LessonID JOIN ( SELECT p1.PackageID FROM packages p1 WHERE p1.StudentID = @StudentID AND p1.PackageNumber = @PackageNumber) latestPackage ON lb1.PackageID = latestPackage.PackageID LEFT JOIN timeslots t ON l.SlotID = t.SlotID LEFT JOIN rescheduledbookings rb ON lb1.BookingID = rb.BookingID LEFT JOIN lessons lessons_original ON rb.OriginalLessonID = lessons_original.LessonID ORDER BY l.Date ASC"},
            {"GetAndSetExistingLessonsData","SELECT lb.PackageID, lb.NumberInPackage, lb.BookingID, lb.ProductID, l.LessonID, l.SlotID, l.Date AS CurrLessonDate, t.StartTime, t.EndTime, t.Time, t.WeekdayNum AS DayNum, w.Name AS WeekdayName, rb.OriginalLessonID AS OriLessonID, lessons_original.Date AS OriLessonDate, rb.Reason FROM lessons l JOIN lessonbookings lb ON l.LessonID = lb.LessonID JOIN (SELECT p1.PackageID FROM packages p1 WHERE p1.StudentID = @StudentID AND p1.PackageNumber = @PackageNumber) latestPackage ON lb.PackageID = latestPackage.PackageID LEFT JOIN timeslots t ON l.SlotID = t.SlotID LEFT JOIN weekdays w ON t.WeekdayNum = w.DayNumber LEFT JOIN rescheduledbookings rb ON lb.BookingID = rb.BookingID LEFT JOIN lessons lessons_original ON rb.OriginalLessonID = lessons_original.LessonID ORDER BY l.Date ASC"},
            // ORI********* {"GetNewLessonData","SELECT l.LessonID, l.Date, l.SlotID, t.WeekdayNum, t.Time, t.StartTime, t.EndTime FROM lessons l JOIN timeslots t ON l.SlotID = t.SlotID WHERE l.Date IN (@DateList) AND l.SlotID IN (SELECT SlotID FROM bookedslots WHERE StudentID = @StudentID)"},
            {"GetNewLessonData","SELECT l.LessonID, l.Date, l.SlotID, t.WeekdayNum, t.Time, t.StartTime, t.EndTime FROM lessons l JOIN timeslots t ON l.SlotID = t.SlotID WHERE l.Date IN (@DateList) AND l.SlotID IN (SELECT SlotID FROM bookedslots WHERE StudentID = @StudentID) LIMIT @LessonAmount"},
            {"GetNewReplacementLessonData","SELECT lb.PackageID, lb.NumberInPackage, lb.BookingID, lb.LessonID, l1.Date AS CurrentLessonDate, t.StartTime, rb.OriginalLessonID, l2.Date AS OriginalLessonDate, rb.Reason FROM lessonbookings lb LEFT JOIN rescheduledbookings rb ON lb.BookingID = rb.BookingID LEFT JOIN lessons l1 ON lb.LessonID = l1.LessonID LEFT JOIN lessons l2 ON rb.OriginalLessonID = l2.LessonID JOIN packages p ON lb.PackageID = p.PackageID JOIN timeslots t ON t.SlotID = l1.SlotID WHERE lb.StudentID = @StudentID AND p.PackageNumber = @PackageNumber"},
            {"GetBookingDetails","SELECT s.Name AS StudentName, b.StudentID, b.SlotID, w.Name AS Weekday, t.Time FROM bookedslots b JOIN students s ON b.StudentID = s.StudentID JOIN timeslots t ON b.SlotID = t.SlotID JOIN weekdays w ON t.WeekdayNum = w.DayNumber WHERE b.StudentID = @StudentID"},
            {"GetAllBookedSlotIDs","SELECT SlotID FROM bookedslots"},

            {"GetLastPackageDates","SELECT Date FROM lessons WHERE LessonID IN (SELECT LessonID FROM lessonbookings WHERE PackageID IN (SELECT PackageID FROM lessonbookings WHERE PackageID = (SELECT PackageID FROM packages WHERE StudentID = @StudentID ORDER BY PackageNumber DESC LIMIT 1))) ORDER BY Date ASC"},
            {"GetLastPackageNumber","SELECT MAX(PackageNumber) FROM packages WHERE StudentID = @StudentID" },
            // ORI********* {"GetNextPackageDates","SELECT lessons.Date FROM bookedslots JOIN timeslots ON bookedslots.SlotID = timeslots.SlotID JOIN lessons ON timeslots.SlotID = lessons.SlotID WHERE bookedslots.StudentID = @StudentID AND lessons.Date > (SELECT MAX(Date) FROM lessons WHERE LessonID IN (SELECT LessonID FROM lessonbookings WHERE PackageID = (SELECT PackageID FROM packages WHERE StudentID = @StudentID ORDER BY PackageNumber DESC LIMIT 1))) AND NOT EXISTS (SELECT 1 FROM holidays WHERE holidays.Date = lessons.Date) AND NOT EXISTS (SELECT 1 FROM individualholidays WHERE individualholidays.LessonID = lessons.LessonID AND individualholidays.StudentID = @StudentID) ORDER BY Date ASC LIMIT @LessonAmount"},
            {"GetNextPackageDates","SELECT lessons.Date FROM bookedslots JOIN timeslots ON bookedslots.SlotID = timeslots.SlotID JOIN lessons ON timeslots.SlotID = lessons.SlotID WHERE bookedslots.StudentID = @StudentID AND lessons.LessonID > (SELECT MAX(LessonID) FROM lessons WHERE LessonID IN (SELECT LessonID FROM lessonbookings WHERE PackageID = (SELECT PackageID FROM packages WHERE StudentID = @StudentID ORDER BY PackageNumber DESC LIMIT 1))) AND NOT EXISTS (SELECT 1 FROM holidays WHERE holidays.Date = lessons.Date) AND NOT EXISTS (SELECT 1 FROM individualholidays WHERE individualholidays.LessonID = lessons.LessonID AND individualholidays.StudentID = @StudentID) ORDER BY Date ASC LIMIT @LessonAmount"},
            {"GetNewStudentsPackageDates","SELECT lessons.Date FROM lessons JOIN timeslots ON timeslots.SlotID = lessons.SlotID JOIN bookedslots ON bookedslots.SlotID = timeslots.SlotID WHERE bookedslots.StudentID = @StudentID AND lessons.LessonID >= (SELECT MIN(LessonID) FROM lessons WHERE Date = (SELECT FirstLesson FROM studydetails WHERE StudentID = @StudentID)) ORDER BY Date ASC LIMIT @LessonAmount"},
            {"GetSpecificPackageDates","SELECT lessons.Date FROM lessonbookings JOIN lessons ON lessonbookings.LessonID = lessons.LessonID WHERE StudentID = @StudentID AND PackageID = (SELECT PackageID FROM packages WHERE StudentID = @StudentID AND PackageNumber = @PackageNumber) ORDER BY lessons.Date"},
            {"GetLessonDateLessonIDs","SELECT LessonID FROM lessons WHERE SlotID IN(SELECT SlotID FROM bookedslots WHERE StudentID = 103) AND Date = '2024-01-19'"},

            //{"SetLevel","UPDATE studydetails SET Level = '@Level' WHERE StudentID = @StudentID"},
            {"UpdateLevel","UPDATE studydetails SET Level = '@Level' WHERE StudentID = @StudentID"},


            {"InsertNewStudent","INSERT INTO students (StudentNumber, Name, Title) VALUES (@StudentNumber, '@Name', '@Title')"},
            {"InsertSlotBooking","INSERT INTO bookedslots (SlotID, StudentID) VALUES (@SlotID, @StudentID)" },
            {"DeleteSlotBooking","DELETE FROM bookedslots WHERE SlotID = @SlotID AND StudentID = @StudentID" },
            {"DeleteRescheduledBooking","DELETE FROM rescheduledbookings WHERE BookingID = @BookingID"},
            {"DeletePackageDatesByPackageID","DELETE FROM lessonbookings WHERE PackageID = @PackageID"},
            {"DeletePackageByPackageID","DELETE FROM packages WHERE PackageID = @PackageID"},

            #region Old Queries (delete if unused)

            {"SetFirstLesson","UPDATE studydetails SET FirstLesson = '@DateString' WHERE StudentID = @StudentID"},
            {"SetLastLesson","UPDATE studydetails SET LastLesson = '@DateString' WHERE StudentID = @StudentID"},
            {"SetStudyStatus","UPDATE studydetails SET StudyStatus = '@StudyStatus' WHERE StudentID = @StudentID"},
            //{"SetPackageID","UPDATE lessonbookings SET PackageID = @PackageID WHERE BookingID = @BookingID" },
            {"SetPackageIDAndNumberInPackage","UPDATE lessonbookings SET PackageID = @PackageID, NumberInPackage = @NumberInPackage WHERE BookingID = @BookingID" },
            {"SetLessonID","UPDATE lessonbookings SET LessonID = @NewLessonID WHERE BookingID = @BookingID"},
            {"SetNumberInPackage","UPDATE lessonbookings SET NumberInPackage = @NumberInPackage WHERE BookingID = @BookingID"},
            {"InsertRescheduledBookingWithReason","INSERT INTO rescheduledbookings (BookingID, OriginalLessonID, Reason) VALUES (@BookingID, @OriginalLessonID, '@Reason')"},


            //{"InsertLessonBooking","INSERT INTO lessonbookings (StudentID, LessonID, ProductID, PackageID, NumberInPackage) VALUES (123, 14378, 456, 789, 1)"},
            {"InsertLessonBooking","INSERT INTO lessonbookings (StudentID, LessonID, ProductID) VALUES (@StudentID, @LessonID, @ProductID)"},
            {"InsertIndividualHoliday","INSERT INTO individualholidays (StudentID, LessonID) VALUES (@StudentID, @LessonID)"},
            {"InsertAttendance","INSERT INTO attendance (BookingID, Attended) VALUES (@BookingID, @Attended)"},
            {"InsertRescheduledBooking","INSERT INTO rescheduledbookings (BookingID, OriginalLessonID) VALUES (@BookingID, @OriginalLessonID)"},
            {"InsertPackage", "INSERT INTO packages (StudentID, PackageNumber, PackageCompleted, LessonsAmount, OutstandingLessons, CompletedLessons) VALUES (@StudentID, @PackageNumber, @PackageCompleted, @LessonsAmount, @OutstandingLessons, @CompletedLessons)"},
            {"InsertPayment","INSERT INTO payments (StudentID, PaymentTime, ProductID, PackageID) VALUES (@StudentID, '@PaymentTime', @ProductID, @PackageID)"},

            // HATI HATI lessonbookings is still not chosen as table it is still lessonbookings!!!!************
            {"GenerateLowestLessonID","SELECT MIN(LessonDates.LessonID) AS SmallestLessonID FROM (SELECT LessonID FROM lessons WHERE Date = '@DateString') AS LessonDates LEFT JOIN lessonbookings ON LessonDates.LessonID = lessonbookings.LessonID WHERE lessonbookings.LessonID IS NULL"},
            {"GenerateTwoLowestLessonIDs","SELECT MIN(LessonDates.LessonID) AS SmallestLessonID FROM (SELECT LessonID FROM lessons WHERE Date = '@DateString') AS LessonDates LEFT JOIN lessonbookings ON LessonDates.LessonID = lessonbookings.LessonID WHERE lessonbookings.LessonID IS NULL GROUP BY LessonDates.LessonID HAVING COUNT(lessonbookings.LessonID) = 0 ORDER BY SmallestLessonID LIMIT 2"},
            {"GenerateLowestLessonIDIndividualHolidays","SELECT MIN(LessonDates.LessonID) AS SmallestLessonID FROM (SELECT LessonID FROM lessons WHERE Date = '@DateString') AS LessonDates LEFT JOIN lessonbookings ON LessonDates.LessonID = lessonbookings.LessonID LEFT JOIN individualholidays ON LessonDates.LessonID = individualholidays.LessonID WHERE lessonbookings.LessonID IS NULL AND individualholidays.LessonID IS NULL"},
            {"GetLastBookingID","SELECT MIN(LessonDates.LessonID) AS SmallestLessonID FROM (SELECT LessonID FROM lessons WHERE Date = '@DateString') AS LessonDates LEFT JOIN lessonbookings ON LessonDates.LessonID = lessonbookings.LessonID LEFT JOIN individualholidays ON LessonDates.LessonID = individualholidays.LessonID WHERE lessonbookings.LessonID IS NULL AND individualholidays.LessonID IS NULL"},
            // doesnt work: {"GetLastInsertedBookingID","SELECT LAST_INSERT_ID() AS LastBookingID"},
            {"GetLastInsertedBookingID","SELECT MAX(BookingID) AS LastBookingID FROM lessonbookings"},
            {"GetLastInsertedPackageID","SELECT MAX(PackageID) AS PackageID FROM packages"},
            {"GetSingleLessonID","SELECT LessonID FROM lessonbookings WHERE BookingID = @BookingID"},
            {"GetDoubleLessonID","SELECT LessonID FROM lessonbookings WHERE BookingID IN (@BookingID1, @BookingID2)"},
            {"GetBookingIDsByDate", "SELECT BookingID FROM lessonbookings WHERE LessonDate IN (@DateStrings)"},
            {"GetLessonIDByDateAndSlot","SELECT LessonID FROM lessons WHERE Date = '@Date' AND SlotID = @SlotID"},


            {"ResetInsertedLessonBookingsData","DELETE FROM lessonbookings WHERE StudentID = @StudentID"},
            {"ResetInsertedIndividualHolidaysData","DELETE FROM individualholidays WHERE StudentID = @StudentID"},
            {"ResetInsertedPackagesData","DELETE FROM packages WHERE StudentID = @StudentID"},
            {"ResetInsertedRescheduledBookingsData","DELETE FROM rescheduledbookings WHERE BookingID IN (SELECT BookingID FROM lessonbookings WHERE StudentID = @StudentID)"},
            {"ResetInsertedAttendanceData","DELETE FROM attendance WHERE BookingID IN (SELECT BookingID FROM lessonbookings WHERE StudentID = @StudentID)"},
            {"ResetInsertedPayments","DELETE FROM payments WHERE StudentID = @StudentID"}
            #endregion
        };

        public static List<string> GetList(string query)
        {

            List<int> resultIntList = _dbInstance.ExecuteScalarList<int>(query);
            List<string> resultStringList = resultIntList.Select(i => i.ToString()).ToList();

            return resultStringList;
        }
        public static void PrintAllStudentNames()
        {

        // Example: Execute a query and get the result as a DataTable
        string query = "SELECT * FROM students";
        DataTable resultTable = _dbInstance.GetData(query);

        // Print the DataTable to the console
        Console.WriteLine("DataTable Result:");
        foreach (DataRow row in resultTable.Rows)
        {
            foreach (DataColumn col in resultTable.Columns)
            {
                Console.Write($"{col.ColumnName}: {row[col]} | ");
            }
            Console.WriteLine();
            }
        }
        public static List<string> GetAllStudentNames()
        {
            // Example: Execute a query and get the result as a DataTable
            string query = "SELECT Name FROM students";
            DataTable resultTable = _dbInstance.GetData(query);

            // List to store student names
            List<string> studentNames = new List<string>();

            // Iterate through the DataTable and add student names to the list
            foreach (DataRow row in resultTable.Rows)
            {
                // Assuming that "Name" is the column name in the students table
                string studentName = row.Field<string>("Name");
                studentNames.Add(studentName);
            }

            // Return the list of student names
            return studentNames;
        }
        public static List<int> GetBookingIDsByDate(List<string> dateStrings)
        {
            // Ensure dateStrings is not null
            if (dateStrings == null)
            {
                throw new ArgumentNullException(nameof(dateStrings));
            }

            // Get the query from the dictionary
            if (!Queries.TryGetValue("GetBookingIDsByDate", out string query))
            {
                throw new InvalidOperationException("Query 'GetBookingIDsByDate' not found in the dictionary.");
            }

            // Build the SQL query using a parameterized query for better security
            string formattedDateStrings = string.Join(", ", dateStrings.Select(d => $"'{d}'"));
            query = query.Replace("@DateStrings", formattedDateStrings);

            // Execute the query and retrieve the list of bookingIDs
            List<int> bookingIDs = _dbInstance.ExecuteScalarList<int>(query);

            // Return the result
            return bookingIDs;
        }
        public static string GetStudentIdByName(string studentName)
        {
            //Console.WriteLine("Student Name: " + studentName); ;
            Queries.TryGetValue("GetStudentIdByName", out string query);
            query = query.Replace("@Name", studentName);

            //Console.WriteLine("Query: " + query);
            string studentID = _dbInstance.ExecuteScalar<string>(query);

            //Console.WriteLine(studentID);
            return studentID;
        }
        public static string GetTitle(string studentID)
        {
            Queries.TryGetValue("GetTitle", out string query);

            // Replace placeholders in the query with actual values
            query = query.Replace("@StudentID", studentID);

            // Execute the query and retrieve the result
            string title = _dbInstance.ExecuteScalar<string>(query);

            return title;
        }
        public static string GetLevel(string studentID)
        {
            Queries.TryGetValue("GetLevel", out string query);

            // Replace placeholders in the query with actual values
            query = query.Replace("@StudentID", studentID);

            // Execute the query and retrieve the result
            string level = _dbInstance.ExecuteScalar<string>(query);

            return level;
        }
        public static string GetStudyStatus(string studentID)
        {
            Queries.TryGetValue("GetStudyStatus", out string query);

            // Replace placeholders in the query with actual values
            query = query.Replace("@StudentID", studentID);

            // Execute the query and retrieve the result
            string studyStatus = _dbInstance.ExecuteScalar<string>(query);

            return studyStatus;
        }
        public static int GetStudentNumberPlusOne()
        {
            Queries.TryGetValue("GetStudentNumberPlusOne", out string query);

            var result = _dbInstance.ExecuteScalar<int>(query);

            return result;
        }
        public static void InsertNewStudent(int studentNumber, string name, string title)
        {
            Queries.TryGetValue("InsertNewStudent", out string query);

            query = query.Replace("@StudentNumber", studentNumber.ToString())
                         .Replace("@Name", name)
                         //.Replace("@Title", title != "" ? $"'{title}'" : "NULL");
                         .Replace("@Title", title != "" ? $"{title}" : "NULL");

            _dbInstance.ExecuteNonQuery(query);
        }




        public static List<DateTime> GetLastPackageDates(string studentID)
        {
            Queries.TryGetValue("GetLastPackageDates", out string query);

            query = query.Replace("@StudentID", studentID);

            var dates = _dbInstance.GetDataAsList<DateTime>(query, row => row.Field<DateTime>("Date"));

            // Uncomment the line below if you want to print the dates to the console
            Console.WriteLine("\nLast Package Dates:");
            foreach( var date in dates)
            {
                Console.WriteLine(date);
            }

            return dates;
        }
        public static List<DateTime> GetNextPackageDates(string studentID, int lessonAmount)
        {
            Queries.TryGetValue("GetNextPackageDates", out string query);

            query = query.Replace("@StudentID", studentID);
            query = query.Replace("@LessonAmount", lessonAmount.ToString());

            var dates = _dbInstance.GetDataAsList<DateTime>(query, row => row.Field<DateTime>("Date"));

            // Sort the list by Date
            dates = dates.OrderBy(date => date).ToList();

            // Uncomment the line below if you want to print the dates to the console
            //foreach (var date in dates)
            //{
            //    Console.WriteLine(date);
            //}

            return dates;
        }
        public static List<DateTime> GetNewStudentsPackageDates(string studentID, int lessonAmount)
        {
            Queries.TryGetValue("GetNewStudentsPackageDates", out string query);

            query = query.Replace("@StudentID", studentID);
            query = query.Replace("@LessonAmount", lessonAmount.ToString());

            var dates = _dbInstance.GetDataAsList<DateTime>(query, row => row.Field<DateTime>("Date"));

            // Sort the list by Date
            dates = dates.OrderBy(date => date).ToList();

            // Uncomment the line below if you want to print the dates to the console
            //foreach (var date in dates)
            //{
            //    Console.WriteLine(date);
            //}

            return dates;
        }
        public static bool CheckIfCurrentlyBooking(string studentID)
        {
            Queries.TryGetValue("CheckIfCurrentlyBooking", out string query);
            query = query.Replace("@StudentID", studentID);

            int result = _dbInstance.ExecuteScalar<int>(query);

            // Return true if the result is 1 (booking exists), false otherwise
            return result == 1;
        }
        public static int GetCurrentIntensity(string studentID)
        {
            Queries.TryGetValue("GetCurrentIntensity", out string query);
            query = query.Replace("@StudentID", studentID);

            int intensityCount = _dbInstance.ExecuteScalar<int>(query);

            return intensityCount;
        }
        public static int GetDiscountAmount(string discountID)
        {
            // Get the query from your query dictionary
            string query = Queries["GetDiscountAmount"];

            // Replace placeholders in the query with actual values
            query = query.Replace("@DiscountID", discountID);

            // Execute the query and retrieve the result
            int discountAmount = _dbInstance.ExecuteScalar<int>(query);

            return discountAmount;
        }
        public static List<(string StudentName, int StudentID, int SlotID, string Time)> GetBookingDetails(string studentID)
        {
            Queries.TryGetValue("GetBookingDetails", out string query);

            query = query.Replace("@StudentID", studentID);

            var bookingDetails = _dbInstance.GetDataAsList(query, row => (
                row.Field<string>("StudentName"),
                row.Field<int>("StudentID"),
                row.Field<int>("SlotID"),
                row.Field<string>("Time")
            ));

            return bookingDetails;
        }
        public static void DisplayAllSlots()
        {
            Queries.TryGetValue("DisplayAllSlots", out string query);

            var slotsData = _dbInstance.GetData(query);

            // Display the header
            Console.WriteLine("\n+-------------+--------+----------+----------+------------+---------+---------+---------+");
            Console.WriteLine("| Zeiten      | Montag | Dienstag | Mittwoch | Donnerstag | Freitag | Samstag | Sonntag |");
            Console.WriteLine("+-------------+--------+----------+----------+------------+---------+---------+---------+");

            // Display the data
            foreach (DataRow row in slotsData.Rows)
            {
                string time = row.Field<string>("Zeiten");
                long monday = row.Field<long>("Montag");
                long tuesday = row.Field<long>("Dienstag");
                long wednesday = row.Field<long>("Mittwoch");
                long thursday = row.Field<long>("Donnerstag");
                long friday = row.Field<long>("Freitag");
                long saturday = row.Field<long>("Samstag");
                long sunday = row.Field<long>("Sonntag");

                Console.WriteLine($"| {time,-11} | {monday,-6} | {tuesday,-8} | {wednesday,-8} | {thursday,-10} | {friday,-7} | {saturday,-7} | {sunday,-7} |");
            }

            // Display the footer
            Console.WriteLine("+-------------+--------+----------+----------+------------+---------+---------+---------+\n");
        }
        public static void DisplayBookedSlots(string studentID)
        {
            Queries.TryGetValue("GetBookingDetails", out string query);

            query = query.Replace("@StudentID", studentID);

            var bookingDetails = _dbInstance.GetDataAsList(query, row => (
                row.Field<string>("StudentName"),
                row.Field<int>("StudentID"),
                row.Field<int>("SlotID"),
                row.Field<string>("Weekday"),
                row.Field<string>("Time")
            ));

            Console.WriteLine("\nCurrently booked slots:\n");
            Console.WriteLine("+-------------+-----------+--------+-------------+-------------+");
            Console.WriteLine("| StudentName | StudentID | SlotID | Weekday     | Time        |");
            Console.WriteLine("+-------------+-----------+--------+-------------+-------------+");

            foreach (var (StudentName, StudentID, SlotID, Weekday, Time) in bookingDetails)
            {
                Console.WriteLine($"| {StudentName,-11} | {StudentID,-9} | {SlotID,-6} | {Weekday,-11} | {Time,-11} |");
            }

            Console.WriteLine("+-------------+-----------+--------+-------------+-------------+\n");
        }
        public static void InsertBooking(string studentID)
        {
            Queries.TryGetValue("InsertSlotBooking", out string insertQuery);

            List<string> slotIDs = new List<string>();

            Console.WriteLine("The desired intensity is higher than the current one.");
            Console.WriteLine("Which Slot(s) should be added?\n");
            DisplayAllSlots();

            while (true)
            {
                Console.WriteLine("Enter SlotID:");
                string input = GetValidSlotIDInput().ToString();

                if (CheckSlotAvailable(input)) 
                {
                    slotIDs.Add(input);
                }
                else 
                {
                    Console.WriteLine("Slot Taken. Choose another slot.");
                    continue;
                }

                Console.WriteLine();
                foreach (string slotID in slotIDs)
                    {
                        Console.WriteLine("Added Slot: " + slotID);
                    }
                
                Console.WriteLine();
                bool addMoreChoice = YesNoChoice("Do you want to add more slots?");

                if (addMoreChoice)
                {
                    continue;
                }
                else 
                {
                    break;
                }

            }

            foreach (string slotID in slotIDs)
            {
                Console.WriteLine("printed SlotID: " + slotID);
                string formattedQuery = insertQuery.Replace("@StudentID", studentID)
                                                   .Replace("@SlotID", slotID);

                _dbInstance.ExecuteNonQuery(formattedQuery);
            }
        }
        public static void DeleteBooking(string studentID)
        {
            Queries.TryGetValue("DeleteSlotBooking", out string deleteQuery);

            List<string> slotIDs = new List<string>();

            Console.WriteLine("The desired intensity is lower than the current one.");
            Console.WriteLine("Which Slot(s) should be deleted?\n");
            DisplayAllSlots();

            while (true)
            {
                Console.WriteLine("Enter SlotID to delete: ");
                string input = GetValidSlotIDInput().ToString();


                if (!CheckSlotAvailable(input))
                {
                    slotIDs.Add(input);
                }
                else
                {
                    Console.WriteLine("Slot is vacant. Choose another slot.");
                    continue;
                }

                Console.WriteLine();
                foreach (string slotID in slotIDs)
                {
                    Console.WriteLine("Deleted Slot: " + slotID);
                }

                Console.WriteLine();
                bool deleteMoreChoice = YesNoChoice("Do you want to delete more slots?");

                if (deleteMoreChoice)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }

            foreach (string slotID in slotIDs)
            {
                string formattedQuery = deleteQuery.Replace("@StudentID", studentID)
                                                   .Replace("@SlotID", slotID);

                _dbInstance.ExecuteNonQuery(formattedQuery);
            }
        }
        public static List<string> GetAllBookedSlotIDs()
        {
            Queries.TryGetValue("GetAllBookedSlotIDs", out string query);

            var slotIDs = _dbInstance.GetDataAsList(query, row => row.Field<int>("SlotID").ToString());

            return slotIDs;
        }
        public static int GetLastPackageNumber(string studentID)
        {
            Queries.TryGetValue("GetLastPackageNumber", out string query);

            query = query.Replace("@StudentID", studentID);

            // Execute the query and get the result
            var result = _dbInstance.ExecuteScalar<int>(query);

            return result;
        }
        public static List<string> GetLessonDateLessonIDs(string studentID, string lessonDate)
        {
            Queries.TryGetValue("GetLessonDateLessonIDs", out string query);

            query = query.Replace("@StudentID", studentID)
                         .Replace("@LessonDate", lessonDate);

            var lessonIDs = _dbInstance.GetDataAsList(query, row => row.Field<int>("LessonID").ToString());

            return lessonIDs;
        }
        public static List<LessonData> GetLastPackagesLessonData(string studentID)
        {
            Queries.TryGetValue("GetLastPackagesLessonData", out string query);

            var lessonData = _dbInstance.GetData(query.Replace("@StudentID", studentID));

            // Process the data and convert it to a list of LessonData objects
            List<LessonData> result = new List<LessonData>();

            foreach (DataRow row in lessonData.Rows)
            {
                LessonData lesson = new LessonData
                {
                    Date = row.Field<DateTime>("Date"),
                    BookingID = row.Field<int>("BookingID"),
                    LessonID = row.Field<int>("LessonID"), // Fetch the LessonID from the DataRow
                    SlotID = row.Field<int>("SlotID"),
                    StartTime = row.Field<TimeSpan>("StartTime"),
                    EndTime = row.Field<TimeSpan>("EndTime"),
                    Time = row.Field<string>("Time"),
                    OriginalLessonDate = row.Field<DateTime?>("OriginalLessonDate"), // Adjusted to OriginalLessonDate
                    WeekdayNum = row.Field<int>("WeekdayNum"), // Added WeekdayNum
                    Reason = row.Field<string>("Reason")
                };

                result.Add(lesson);
            }

            // Sort the result list by Date
            result = result.OrderBy(lesson => lesson.Date).ToList();

            return result;
        }
        public static List<LessonData> GetAnyPackagesLessonData(string studentID, string packageNumber)
        {
            Queries.TryGetValue("GetAnyPackagesLessonData", out string query);

            var lessonData = _dbInstance.GetData(query.Replace("@StudentID", studentID)
                                                        .Replace("@PackageNumber", packageNumber));

            // Process the data and convert it to a list of LessonData objects
            List<LessonData> result = new List<LessonData>();

            foreach (DataRow row in lessonData.Rows)
            {
                LessonData lesson = new LessonData
                {
                    Date = row.Field<DateTime>("Date"),
                    BookingID = row.Field<int>("BookingID"),
                    LessonID = row.Field<int>("LessonID"), // Fetch the LessonID from the DataRow
                    SlotID = row.Field<int>("SlotID"),
                    StartTime = row.Field<TimeSpan>("StartTime"),
                    EndTime = row.Field<TimeSpan>("EndTime"),
                    Time = row.Field<string>("Time"),
                    OriginalLessonDate = row.Field<DateTime?>("OriginalLessonDate"), // Adjusted to OriginalLessonDate
                    WeekdayNum = row.Field<int>("WeekdayNum"), // Added WeekdayNum
                    Reason = row.Field<string>("Reason")
                };

                result.Add(lesson);
            }

            // Sort the result list by Date
            result = result.OrderBy(lesson => lesson.Date).ToList();

            return result;
        }
        // ORI ******** public static List<LessonData> GetNewLessonData(List<DateTime> packageDates, string studentID)
        public static List<ExistingLessonDataModel> GetAndSetExistingLessonsData(string studentID, int packageNumber)
        {
            Queries.TryGetValue("GetAndSetExistingLessonsData", out string query);
            
            // Get data from the database using the query
            var lessonData = _dbInstance.GetData(query.Replace("@StudentID", studentID)
                                                        .Replace("@PackageNumber", packageNumber.ToString()));

            List<ExistingLessonDataModel> result = new List<ExistingLessonDataModel>();
            // Process the data and populate the ExistingPackage object
            foreach (DataRow row in lessonData.Rows)
            {
                ExistingLessonDataModel lesson = new ExistingLessonDataModel
                {
                    PackageID = row.Field<int>("PackageID"),
                    NumberInPackage = row.Field<int>("NumberInPackage"),
                    BookingID = row.Field<int>("BookingID"),
                    ProductID = row.Field<int>("ProductID"),
                    LessonID = row.Field<int>("LessonID"),
                    SlotID = row.Field<int>("SlotID"),
                    LessonDateTime = row.Field<DateTime>("CurrLessonDate"),
                    LessonDateString = ConvertToCardDateFormat(row.Field<DateTime>("CurrLessonDate")),
                    StartTime = row.Field<TimeSpan>("StartTime"),
                    EndTime = row.Field<TimeSpan>("EndTime"),
                    LessonSlotTime = row.Field<string>("Time"),
                    WeekdayNum = row.Field<int>("DayNum"),
                    Weekday = row.Field<string>("WeekdayName"),
                    OriginalLessonID = row.Field<int?>("OriLessonID") ?? 0,
                    OriginalLessonDate = row.Field<DateTime?>("OriLessonDate") ?? DateTime.MinValue,
                    //OriginalLessonDateString = (ConvertToCardDateFormat(row.Field<DateTime?>("OriLessonDate") ?? DateTime.MinValue) == "01 Jan 0001") ? (ConvertToCardDateFormat(row.Field<DateTime?>("OriLessonDate") ?? DateTime.MinValue)) : "",
                    Reason = row.Field<string>("Reason") ?? ""
                };
    
                if(lesson.OriginalLessonDate != DateTime.MinValue)
                {
                    lesson.OriginalLessonDateString = ConvertToCardDateFormat(lesson.OriginalLessonDate);
                    lesson.SortingDate = lesson.OriginalLessonDate;
                }
                else
                {
                    lesson.SortingDate = lesson.LessonDateTime;
                }

                lesson.PackageNumber = packageNumber;
                lesson.Paraf = SetParaf(lesson.LessonDateTime, lesson.StartTime);

                result.Add(lesson);
            }

            // Sort the lessons by date
            //result = result.OrderBy(lesson => lesson.LessonDateTime).ToList();
            result = result.OrderBy(lesson => lesson.SortingDate).ToList();
            return result;

        }

        public static List<LessonData> GetNewLessonData(List<DateTime> packageDates, string studentID, string lessonAmount)
        {
            string dateListStrings = "";
            foreach(DateTime date in packageDates)
            {
                string dateString = ConvertDateTimeToDateString(date);
                dateListStrings += "'" + dateString + "',";
            }

            // Remove the last comma
            dateListStrings = dateListStrings.TrimEnd(',');

            Queries.TryGetValue("GetNewLessonData", out string query);

            query = query.Replace("@DateList", dateListStrings)
                         //.Replace("@StudentID", studentID);
                         .Replace("@StudentID", studentID)
                         .Replace("@LessonAmount", lessonAmount);  // TEST****


            var lessonData = _dbInstance.GetData(query);

            // Process the data and convert it to a list of LessonData objects
            List<LessonData> result = new List<LessonData>();

            foreach (DataRow row in lessonData.Rows)
            {
                
                LessonData lesson = new LessonData
                {
                    LessonID = row.Field<int>("LessonID"),
                    Date = row.Field<DateTime>("Date"),
                    SlotID = row.Field<int>("SlotID"),
                    WeekdayNum = row.Field<int>("WeekdayNum"), // Added WeekdayNum
                    Time = row.Field<string>("Time"),
                    StartTime = row.Field<TimeSpan>("StartTime"),
                    EndTime = row.Field<TimeSpan>("EndTime")
                };

                result.Add(lesson);
            }

            // Sort the result list by Date
            result = result.OrderBy(lesson => lesson.Date).ToList();

            return result;
        }
        public static void UpdateLevel(string studentID, string level)
        {

            // Construct the update query
            string query = Queries["UpdateLevel"];

            // Replace placeholders in the query with actual values
            query = query.Replace("@StudentID", studentID)
                         .Replace("@Level", level);

            // Execute the update query
            _dbInstance.ExecuteNonQuery(query);
        }
        // Method to get all levels from the database
        public static List<string> GetAllLevels()
        {
            // Fetch the query from your dictionary
            string query = Queries["GetAllLevels"];

            // Execute the query to get all levels
            DataTable resultTable = _dbInstance.GetData(query);

            // List to store levels
            List<string> levels = new List<string>();

            // Iterate through the DataTable and add levels to the list
            foreach (DataRow row in resultTable.Rows)
            {
                string level = row.Field<string>("Level");
                levels.Add(level);
            }

            return levels;
        }

        public static (string Currency, string DiscountID) GetStudentPricing(string studentID)
        {
            // Get the query from your query dictionary
            string query = Queries["GetPricingDetails"];

            // Replace placeholders in the query with actual values
            query = query.Replace("@StudentID", studentID);

            // Execute the query and retrieve the result
            var result = _dbInstance.GetData(query);

            // Assuming the query will always return exactly one row of data
            // Access the first row directly
            string currency = result.Rows[0]["Currency"].ToString();
            string discountID = result.Rows[0]["DiscountID"]?.ToString() ?? "";

            // Return the values as a tuple
            return (currency, discountID);
        }
        public static string GetProductID(string currency, string level)
        {
            // Get the query from your query dictionary
            string query = Queries["GetProductID"];

            // Replace placeholders in the query with actual values
            query = query.Replace("@Currency", currency)
                         .Replace("@Level", level);

            // Execute the query and retrieve the result
            string productID = _dbInstance.ExecuteScalar<int>(query).ToString();

            return productID;
        }
        public static int GetProductIDInt(string currency, string level)
        {
            // Get the query from your query dictionary
            string query = Queries["GetProductID"];

            // Replace placeholders in the query with actual values
            query = query.Replace("@Currency", currency)
                         .Replace("@Level", level);

            // Execute the query and retrieve the result
            int productID = _dbInstance.ExecuteScalar<int>(query);

            return productID;
        }
        public static List<DateTime> GetSpecificPackageDates(string studentID, int packageNumber)
        {
            // Get the query from your query dictionary
            string query = Queries["GetSpecificPackageDates"];

            // Replace placeholders in the query with actual values
            query = query.Replace("@StudentID", studentID)
                         .Replace("@PackageNumber", packageNumber.ToString());

            // Execute the query and retrieve the dates
            var dates = _dbInstance.GetDataAsList<DateTime>(query, row => row.Field<DateTime>("Date"));

            return dates;
        }

        public static List<ReplacementLessonDataModel> GetNewReplacementLessonData(string studentID, string packageNumber)
        {
            //string dateListStrings = string.Join("','", packageDates.Select(date => date.ToString("yyyy-MM-dd")));

            Queries.TryGetValue("GetNewReplacementLessonData", out string query);

            query = query.Replace("@StudentID", studentID)
                         .Replace("@PackageNumber", packageNumber);

            var lessonData = _dbInstance.GetData(query);

            List<ReplacementLessonDataModel> result = new List<ReplacementLessonDataModel>();

            foreach (DataRow row in lessonData.Rows)
            {
                ReplacementLessonDataModel lesson = new ReplacementLessonDataModel
                {
                    NumberInPackage = row.Field<int>("NumberInPackage"),
                    PackageID = row.Field<int>("PackageID"),
                    BookingID = row.Field<int>("BookingID"),
                    LessonID = row.Field<int>("LessonID"),
                    Date = row.Field<DateTime>("CurrentLessonDate"),
                    StartTime = row.Field<TimeSpan>("StartTime"),
                    FormattedDate = ConvertToCardDateFormat(row.Field<DateTime>("CurrentLessonDate")),
                    OriginalLessonID = row.Field<int?>("OriginalLessonID"),
                    OriginalLessonDate = row.Field<DateTime?>("OriginalLessonDate"),
                    Reason = row.Field<string>("Reason")
                };

                lesson.Paraf = (lesson.Date + lesson.StartTime < DateTime.Now) ? true : false;

                // Check if OriginalLessonDate is not null before formatting
                if (lesson.OriginalLessonDate.HasValue)
                {
                    lesson.FormattedOriginalLessonDate = ConvertToCardDateFormat(lesson.OriginalLessonDate.Value);
                }

                result.Add(lesson);
            }

            result = result.OrderBy(lesson => lesson.Date).ToList();

            return result;
        }
        public static int GetSingleLessonID(string dateString, int slotID)
        {
            Queries.TryGetValue("GetLessonIDByDateAndSlot", out string query);
            query = query.Replace("@Date", dateString) // Format the date
                         .Replace("@SlotID", slotID.ToString());

            int lessonID = _dbInstance.ExecuteScalar<int>(query);

            return lessonID;
        }
        public static void SetLessonID(int newLessonID, int bookingID)
        {
            Queries.TryGetValue("SetLessonID", out string query);
            query = query.Replace("@NewLessonID", newLessonID.ToString())
                         .Replace("@BookingID", bookingID.ToString());

            _dbInstance.ExecuteNonQuery(query);
        }
        public static void SetNumberInPackage(string bookingID, string numberInPackage)
        {
            Queries.TryGetValue("SetNumberInPackage", out string query);
            query = query.Replace("@BookingID", bookingID)
                         .Replace("@NumberInPackage", numberInPackage);

            _dbInstance.ExecuteNonQuery(query);
        }
        public static int GetOriginalLessonID(string bookingID)
        {
            Queries.TryGetValue("GetOriginalLessonID", out string query);
            query = query.Replace("@BookingID", bookingID);

            int originalLessonID = _dbInstance.ExecuteScalar<int>(query);

            return originalLessonID;
        }
        public static void DeleteRescheduledBooking(string bookingID)
        {
            Queries.TryGetValue("DeleteRescheduledBooking", out string query);
            query = query.Replace("@BookingID", bookingID);

            _dbInstance.ExecuteNonQuery(query);
        }
        public static void DeletePackageDatesByPackageID(string packageID)
        {
            Queries.TryGetValue("DeletePackageDatesByPackageID", out string query);
            query = query.Replace("@PackageID", packageID);

            _dbInstance.ExecuteNonQuery(query);
        }
        public static void DeletePackageByPackageID(string packageID)
        {
            Queries.TryGetValue("DeletePackageByPackageID", out string query);
            query = query.Replace("@PackageID", packageID);

            _dbInstance.ExecuteNonQuery(query);
        }
        public static bool CheckIfLessonBookingsExist(string studentID)
        {
            Queries.TryGetValue("CheckIfLessonBookingsExist", out string query);
            query = query.Replace("@StudentID", studentID);

            // Call ExecuteScalarStatic<T> with bool as the generic type argument
            bool studentExists = _dbInstance.ExecuteScalar<bool>(query);


            return studentExists;
        }





















        #region # Old Methods

        public static int GenerateLowestLessonID(string dateString)
        {
            //Console.WriteLine("dateString length: " + dateString.Length);
            dateString = dateString.Trim();

            Queries.TryGetValue("GenerateLowestLessonID", out string query);
            query = query.Replace("@DateString", dateString);

            int lessonID = _dbInstance.ExecuteScalar<int>(query);
            
            //Console.WriteLine($"dateString: {dateString} used to generate lessonID: {lessonID}");

            //Console.WriteLine(lessonID.ToString());
            return lessonID;
        }
        public static List<int> GenerateLowestLessonIDs(string dateString)
        {
            Queries.TryGetValue("GenerateTwoLowestLessonIDs", out string query);
            query = query.Replace("@DateString", dateString);

            var lessonIDs = _dbInstance.ExecuteScalarList<int>(query);

            //Console.WriteLine(string.Join(", ", lessonIDs));
            return lessonIDs;
        }
        public static int GetSingleLessonID(string bookingID)
        {
            Queries.TryGetValue("GetSingleLessonID", out string query);
            query = query.Replace("@BookingID", bookingID);

            int lessonID = _dbInstance.ExecuteScalar<int>(query);

            //Console.WriteLine(lessonID.ToString());
            return lessonID;
        }
        public static List<int> GetDoubleLessonIDs(string bookingID1, string bookingID2)
        {
            Queries.TryGetValue("GetDoubleLessonID", out string query);
            query = query.Replace("@BookingID1", bookingID1)
                         .Replace("@BookingID2", bookingID2);

            var lessonIDs = _dbInstance.ExecuteScalarList<int>(query);

            //Console.WriteLine(string.Join(", ", lessonIDs));
            return lessonIDs;
        }

        public static int GenerateLowestLessonIDIndividualHolidays(string dateString)
        {

            Queries.TryGetValue("GenerateLowestLessonIDIndividualHolidays", out string query);
            query = query.Replace("@DateString", dateString);

            int lessonID = _dbInstance.ExecuteScalar<int>(query);

            //Console.WriteLine(lessonID.ToString());
            return lessonID;
        }
        public static int GetLastInsertedBookingID()
        {
            Queries.TryGetValue("GetLastInsertedBookingID", out string query);
            int lastInsertedBookingID = _dbInstance.ExecuteScalar<int>(query);

            //Console.WriteLine(lastInsertedBookingID.ToString());
            return lastInsertedBookingID;
        }
        public static int GetLastInsertedPackageID()
        {
            Queries.TryGetValue("GetLastInsertedPackageID", out string query);
            int lastInsertedPackageID = _dbInstance.ExecuteScalar<int>(query);

            //Console.WriteLine(lastInsertedBookingID.ToString());
            return lastInsertedPackageID;
        }
        public static int GetLastBookingID()
        {
            Queries.TryGetValue("GetLastBookingID", out string query);
            int lastBookingID = _dbInstance.ExecuteScalar<int>(query);

            //Console.WriteLine(lastBookingID.ToString());
            return lastBookingID;
        }
        public static void SetFirstLesson(string dateString, string studentID)
        {

            Queries.TryGetValue("SetFirstLesson", out string query);
            query = query.Replace("@DateString", dateString)
                         .Replace("@StudentID", studentID);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void SetLastLesson(string dateString, string studentID)
        {

            Queries.TryGetValue("SetLastLesson", out string query);
            query = query.Replace("@DateString", dateString)
                         .Replace("@StudentID", studentID);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void SetStudyStatus(string studyStatus, string studentID)
        {

            Queries.TryGetValue("SetStudyStatus", out string query);
            query = query.Replace("@StudyStatus", studyStatus)
                         .Replace("@StudentID", studentID);

            _dbInstance.ExecuteNonQuery(query);

        }
        //public static void SetPackageID(string packageID, string bookingID)
        //    {
        //        Queries.TryGetValue("SetPackageID", out string query);
        //        query = query.Replace("@PackageID", packageID)
        //                     .Replace("@BookingID", bookingID);

        //        _dbInstance.ExecuteNonQuery(query);
        //    }
        public static void SetPackageIDAndNumberInPackage(string packageID, string numberInPackage, string bookingID)
        {
            Queries.TryGetValue("SetPackageIDAndNumberInPackage", out string query);
            query = query.Replace("@PackageID", packageID)
                         .Replace("@NumberInPackage", numberInPackage)
                         .Replace("@BookingID", bookingID);

            _dbInstance.ExecuteNonQuery(query);
        }


        public static void InsertLessonBooking(string studentID, string lessonID, string productID)
        {
            Console.WriteLine("StudentID: " + studentID + ", LessonID: " + lessonID + ", ProductID: " + productID);
            // LESSON ID NOT DETECTED!!!!!!!!!*********************************************************

            Queries.TryGetValue("InsertLessonBooking", out string query);
            query = query.Replace("@StudentID", studentID)
                         .Replace("@LessonID", lessonID)
                         .Replace("@ProductID", productID);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void InsertIndividualHoliday(string studentID, string lessonID)
        {

            Queries.TryGetValue("InsertIndividualHoliday", out string query);
            query = query.Replace("@StudentID", studentID)
                         .Replace("@LessonID", lessonID);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void InsertPackage(string studentID, string packageNumber, string packageCompleted, string lessonsAmount, string outstandingLessons, string completedLessons)
        {
            Queries.TryGetValue("InsertPackage", out string query);
            query = query.Replace("@StudentID", studentID)
                         .Replace("@PackageNumber", packageNumber)
                         .Replace("@PackageCompleted", packageCompleted)
                         .Replace("@LessonsAmount", lessonsAmount)
                         .Replace("@OutstandingLessons", outstandingLessons)
                         .Replace("@CompletedLessons", completedLessons);

            _dbInstance.ExecuteNonQuery(query);
        }
        public static void InsertAttendance(string bookingID, string attended)
        {

            Queries.TryGetValue("InsertAttendance", out string query);
            query = query.Replace("@BookingID", bookingID)
                         .Replace("@Attended", attended);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void InsertRescheduledBooking(string bookingID, string originalLessonID)
        {
            //Console.WriteLine("BookingID: " +bookingID+ ", OriginalLessonID: " + originalLessonID);

            Queries.TryGetValue("InsertRescheduledBooking", out string query);
            query = query.Replace("@BookingID", bookingID)
                         .Replace("@OriginalLessonID", originalLessonID);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void InsertRescheduledBooking(string bookingID, string originalLessonID, string reason)
        {
            Queries.TryGetValue("InsertRescheduledBookingWithReason", out string query);
            query = query.Replace("@BookingID", bookingID)
                         .Replace("@OriginalLessonID", originalLessonID)
                         .Replace("@Reason", reason);

            _dbInstance.ExecuteNonQuery(query);
        }
        public static void InsertPayment(string studentID, string paymentTime, string productID, string packageID)
        {
            //Console.WriteLine($"studentID: {studentID}, paymentTime: {paymentTime}, productID: {productID}, packageID: {packageID}");

            Queries.TryGetValue("InsertPayment", out string query);
            query = query.Replace("@StudentID", studentID)
                         .Replace("@PaymentTime", paymentTime)
                         .Replace("@ProductID", productID)
                         .Replace("@PackageID", packageID);

            _dbInstance.ExecuteNonQuery(query);
        }
        #endregion
    #region # Reset Tables For Deletion and Testing

        public static void ResetInsertedLessonBookingsData(string studentID)
        {

            Queries.TryGetValue("ResetInsertedLessonBookingsData", out string query);
            query = query.Replace("@StudentID", studentID);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void ResetInsertedIndividualHolidaysData(string studentID)
        {

            Queries.TryGetValue("ResetInsertedIndividualHolidaysData", out string query);
            query = query.Replace("@StudentID", studentID);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void ResetInsertedPackagesData(string studentID)
        {

            Queries.TryGetValue("ResetInsertedPackagesData", out string query);
            query = query.Replace("@StudentID", studentID);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void ResetInsertedRescheduledBookingsData(string studentID)
        {

            Queries.TryGetValue("ResetInsertedRescheduledBookingsData", out string query);
            query = query.Replace("@StudentID", studentID);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void ResetInsertedAttendanceData(string studentID)
        {

            Queries.TryGetValue("ResetInsertedAttendanceData", out string query);
            query = query.Replace("@StudentID", studentID);

            _dbInstance.ExecuteNonQuery(query);

        }
        public static void ResetInsertedPayments(string studentID)
        {

            Queries.TryGetValue("ResetInsertedPayments", out string query);
            query = query.Replace("@StudentID", studentID);

            _dbInstance.ExecuteNonQuery(query);

        }
    #endregion

    }

}