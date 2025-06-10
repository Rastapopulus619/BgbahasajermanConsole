using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Org.BouncyCastle.Crypto.Modes.Gcm;

namespace TimCoreyCourseConsoleApp1
{
    class Program
    {
        public class StudentData
        {
            public int StudentID { get; set; }
            public string StudentName { get; set; }
            public int Intensity { get; set; }
            public string Level { get; set; }
            public int AmountOfWeekdays { get; set; }
            public List<string> Weekdays { get; set; }
            public List<string> SlotTimes { get; set; }
            public List<DateTime> NewLessonDates { get; set; }
            public List<string> NewFormattedDates { get; set; }
            public List<DateTime> OldLessonDates { get; set; }
            public List<string> OldFormattedDates { get; set; }
            public List<string> LesTambahanFormattedDates { get; set; }
            public bool DisplayOldLessonDates { get; set; }
            public bool DisplayLesTambahan { get; set; }
        }


        // Define the connection string
        public const string connectionString = "Server=localhost;Database=bgbahasajerman;User=root;Password=Burungnuri1212;";
        static MySqlConnection conn;

        static void Main()
        {

            conn = new MySqlConnection(connectionString);

            try
            {

                conn.Open();

                Console.WriteLine("display old lessons table? (y/n)");
                bool displayOldLessonDates = getUserYesNo();

                Console.WriteLine("display additional lessons? (y/n)");
                bool displayLesTambahan = getUserYesNo();

                List<string> studentNames = GetStudentlist();

                int studentID;
                string studentName;

                GetStudentNameAndID(studentNames, out studentID, out studentName);
                

                Console.WriteLine(studentID);
                Console.WriteLine(studentName);

                int intensity;
                string level;

                intensity = GetIntensity(studentID);
                level = GetLevel(studentID);

                Console.WriteLine(intensity);
                Console.WriteLine(level);

                int amountOfWeekdays = intensity / 4;

                

                List<int> studentSlots = GetStudentSlots(studentID, amountOfWeekdays);

                List<string> weekdays = new List<string>();
                List<string> slotTimes = new List<string>();


                foreach (int slot in studentSlots)
                {
                    // Access values
                    (string day, string time) = Timetable.GetDayTime(slot);
                    weekdays.Add(day);
                    slotTimes.Add(time);

                    Console.WriteLine($"{day}: {time}");

                }


                var oldDatesResult = GetAndFormatOldLessonDates(studentID);
                List<DateTime> oldLessonDates = oldDatesResult.OldLessonDates;
                List<string> oldFormattedDates = oldDatesResult.OldFormattedDates;

                var newDatesresult = GetAndFormatNewLessonDates(studentID);
                List<DateTime> newLessonDates = newDatesresult.NewLessonDates;
                List<string> newFormattedDates = newDatesresult.NewFormattedDates;

                DatesDisplay(newLessonDates);  // Displays DateTime objects
                DatesDisplay(newFormattedDates);   // Displays formatted date strings


                List<string> lesTambahanFormattedDates = new List<string>
                {
                    "24 Nov 2023",
                    "01 Dez 2023"
                };

                StudentData studentData = new StudentData
                {
                    StudentName = studentName,
                    Intensity = intensity,
                    Level = level,
                    AmountOfWeekdays = amountOfWeekdays,
                    Weekdays = weekdays,
                    SlotTimes = slotTimes,
                    NewLessonDates = newLessonDates,
                    NewFormattedDates = newFormattedDates,
                    DisplayOldLessonDates = displayOldLessonDates,
                    DisplayLesTambahan = displayLesTambahan,
                    LesTambahanFormattedDates = lesTambahanFormattedDates
                };


                GenerateHtmlContent(studentData);


                Console.ReadLine();




                //TODO LIST: *********************
                // FIX HTML BUILDER
                
                // Manual Slot input check that is above 0 and below    *** TEST BELUM
                // Get Level *(similar to getIntensity)   *** TEST BELUM

                // ** // Get Les Tambahan create method and call from html builder Method**
                // Complete Les Tambahan Method with MYSQL Query

                // finish html string constructor

                // add naming for the saved files



                //00********* add safety for situation when new student is created and there is no old package but it is searched for.
                // how can I evade the code to look for old package if there is none to be found?


                // Get the last packages for current students and start all of them at 100 or 1000 as package number
                // then later it will be easy to change all package numbers by calculation agains the actual amount of packages taken
                // that way a sort of accurate package number can be achieved for all 

                // the packages for this year have to be counted by looking at the payments made *the payment sheet will not be accurate, so better look at physical absen book!!
                // start from the date where no payment comments are added in the absen sheet, => these have to be completed first


                // Set up system for replacement lessons so the replacements can then be shown in the card date fields





                // ++ a method might be neccessary to determine paraf or no paraf for todays date, so no user choice is needed *for automation later
                // ++ add logging to database and/or to file, maybe drive, notes or Notion?




            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private static bool getUserYesNo()
        {
            while (true)
            {
                string userChoice = Console.ReadLine().ToLower();

                if (userChoice == "y")
                {
                    return true;
                }
                else if (userChoice == "n")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter 'y' or 'n'.");
                }
            }
        }

        public static StringBuilder SetHeaderData(StudentData studentData, StringBuilder htmlBuilder)
        {

            htmlBuilder.Replace("{name}", $"{studentData.StudentName}");
            htmlBuilder.Replace("{stufe}", $"{studentData.Level}");
            htmlBuilder.Replace("{intensität}", $"{studentData.Intensity}");


            string days = "";
            foreach (var day in studentData.Weekdays)
            {
                days += day;
                days += "<br>";
            }
            days = days.Substring(0, days.Length - 4);
            htmlBuilder.Replace("{tage}", $"{days}");


            string times = "";
            foreach (var time in studentData.SlotTimes)
            {
                times += time;
                times += "<br>";
            }
            times = times.Substring(0, times.Length - 4);
            htmlBuilder.Replace("{zeit}", $"{times}");


            return htmlBuilder;
        }

        public static void GenerateHtmlContent(StudentData studentData)
        {
            //Get Content into stringBuilder
            string htmlInputPath = "C:/Programmieren/bgbahasajermanApp/TimCoreyCourseConsoleApp1/TimCoreyCourseConsoleApp1/ScheduleCard.html";
            string htmlOutputPath = "C:/Programmieren/ProgrammingProjects/PythonProject/src/GoogleAppsAutomation/GoogleDriveNative/Downloads/HtmlSlotsTemp/testing.html";
            string htmlContent = File.ReadAllText(htmlInputPath);
            StringBuilder htmlBuilder = new StringBuilder(htmlContent);

            htmlBuilder = SetHeaderData(studentData, htmlBuilder);

            //Modify Content
            Console.WriteLine("Test successful until here");



            

            if (studentData.DisplayOldLessonDates == true)
            {


    // string oldSlotsTable = tableBuilder(studentData);
    // Get OldLessonsTable and replace the placeholder with it
    // htmlBuilder.Replace("{oldSlotstable}", $"{oldSlotsTable}");

                if (studentData.DisplayLesTambahan == true)
                {
                    // Complete Les Tambahan Method with MYSQ Query
                    string lesTambahanBox = lesTambahanBoxBuilder(studentData);
                    htmlBuilder.Replace("{lesTambahanTable}", $"{lesTambahanBox}");
                }
                else
                {
                    htmlBuilder.Replace("{lesTambahanTable}", "");
                }

            }
            else
            {
                htmlBuilder.Replace("<div class=\"subtitleBox\" id=\"subtitleBox1\">ALTES PAKET</div>", "");
                htmlBuilder.Replace("{oldSlotstable}", "");
                htmlBuilder.Replace("{lesTambahanTable}", "");
            }

            string newSlotsTable = tableBuilder(studentData);

            htmlBuilder.Replace("{newSlotstable}", newSlotsTable);

            //Console.WriteLine(htmlBuilder.ToString());

            // Write Content to File
            File.WriteAllText(htmlOutputPath, htmlBuilder.ToString());
        }

        public static string lesTambahanBoxBuilder(StudentData studentData)
        {

            StringBuilder lesTambahanBoxBuilder = new StringBuilder();

            lesTambahanBoxBuilder.Append(@"
            <div class=""LesTambahanBox"" id=""tambahanBox"">
                  <table>
                    <tr>
                      <td> Les Tambahan:</td>
                        <td style=""padding-left: 8px;"">");

            foreach (var date in studentData.LesTambahanFormattedDates)
            {
                lesTambahanBoxBuilder.Append($"{date} <br>");
            }
            lesTambahanBoxBuilder.Length -= 4;

            lesTambahanBoxBuilder.Append(@"
                        </td>
                    </tr>
                  </table>
              </div>");

            return lesTambahanBoxBuilder.ToString();
        }

        public static string tableBuilder(StudentData studentData)
        {
            int tableColumnsAmount = 2;
            int tableRowsAmount = studentData.NewFormattedDates.Count / tableColumnsAmount;

            StringBuilder newSlotsTableBuilder = new StringBuilder();

            ///////////////

            newSlotsTableBuilder.Append(" < table class=\"slotsTable\">");


            for (int i = 0; i < tableRowsAmount; i++)
            {
                Console.WriteLine("outer loop round: " + i);
                newSlotsTableBuilder.Append("<tr>");

                for (int j = 0; j < tableColumnsAmount; j++)
                {
                    int index = i * tableColumnsAmount + j;

                    string paraf;
                    bool needsParaf = IsAPastDate(studentData.NewLessonDates[i], studentData.NewFormattedDates[i]);

                    if (needsParaf == true)
                    {
                        paraf = "class=\"SlotsTableParaf\"";
                    }
                    else
                    {
                        paraf = "";
                    }

                    newSlotsTableBuilder.Append($"<td {paraf}>{studentData.NewFormattedDates[index]}</td>");
                }

                newSlotsTableBuilder.Append("</tr>");
            }

            newSlotsTableBuilder.Append("</table>");

            ///////////////
            ///


            Console.WriteLine(newSlotsTableBuilder.ToString());
            string newSlotsTable = newSlotsTableBuilder.ToString();

            return newSlotsTable;

        }

        static bool IsAPastDate(DateTime date, string formattedDate)
        {
            DateTime currentDate = DateTime.Now;
            bool isPastDate = false;

            if (date.Date < currentDate.Date)
            {
                Console.WriteLine("(date < currentDate)");
                isPastDate = true;
                return isPastDate;
            }
            else if (date.Date == currentDate.Date)
            {
                Console.WriteLine("(date == currentDate)");

                while (true)
                {
                    Console.WriteLine($"{formattedDate} is Today. Add Paraf to Date? (y/n)");
                    string manualParafChoice = Console.ReadLine().ToLower();

                    if (manualParafChoice == "y")
                    {
                        isPastDate = true;
                        break;
                    }
                    else if (manualParafChoice == "n")
                    {
                        isPastDate = false;
                        break;
                    }
                    else
                    {

                        Console.WriteLine("Invalid choice. Please enter 'y' or 'n'.");
                    }

                }

                return isPastDate;
            }
            else
            {
                Console.WriteLine("(date > currentDate)");

                return false;
            }

        }

        static void DatesDisplay(List<DateTime> dateobjects)
        {
            foreach (DateTime date in dateobjects)
            {
                Console.WriteLine(date);
            }
        }

        static void DatesDisplay(List<string> datestrings)
        {
            foreach (string date in datestrings)
            {
                Console.WriteLine(date);
            }
        }

        public static (List<DateTime> OldLessonDates, List<string> OldFormattedDates) GetAndFormatOldLessonDates(int studentID)
        {
            string oldLessonDatesQuery = $"SELECT Date FROM lessons WHERE LessonID IN(SELECT LessonID FROM lessonbookings WHERE PackageID = (SELECT PackageID FROM packages WHERE PackageNumber = (SELECT MAX(PackageNumber) FROM packages WHERE StudentID = {studentID})));";

            List<DateTime> oldLessonDates = ExecuteQueryForDates(oldLessonDatesQuery);

            List<string> oldFormattedDates = new List<string>();

            CultureInfo germanCulture = new CultureInfo("de-DE"); // German culture

            foreach (DateTime date in oldLessonDates)
            {
                string formattedDate = date.ToString("dd-MMM" +
                    "-yyyy", germanCulture);
                string day = formattedDate.Substring(0, 2);
                string month = formattedDate.Substring(3, 3);
                string year = formattedDate.Substring(formattedDate.Length - 4);
                formattedDate = $"{day} {month} {year}";

                oldFormattedDates.Add(formattedDate);
            }

            return (oldLessonDates, oldFormattedDates);

        }
        public static (List<DateTime> NewLessonDates, List<string> NewFormattedDates) GetAndFormatNewLessonDates(int studentID)
        {
            string newLessonDatesQuery = $"SELECT lessons.Date FROM bookedslots JOIN timeslots ON bookedslots.SlotID = timeslots.SlotID JOIN lessons ON timeslots.SlotID = lessons.SlotID WHERE bookedslots.StudentID = {studentID} AND lessons.Date > (SELECT MAX(Date) FROM lessons WHERE LessonID IN(SELECT LessonID FROM lessonbookings WHERE PackageID = (SELECT PackageID FROM packages WHERE PackageNumber = (SELECT MAX(PackageNumber) FROM packages WHERE StudentID = {studentID})))) LIMIT 4;";

            List<DateTime> newLessonDates = ExecuteQueryForDates(newLessonDatesQuery);

            List<string> newFormattedDates = new List<string>();

            CultureInfo germanCulture = new CultureInfo("de-DE"); // German culture

            foreach (DateTime date in newLessonDates)
            {
                string formattedDate = date.ToString("dd-MMM" +
                    "-yyyy", germanCulture);
                string day = formattedDate.Substring(0, 2);
                string month = formattedDate.Substring(3, 3);
                string year = formattedDate.Substring(formattedDate.Length - 4);
                formattedDate = $"{day} {month} {year}";

                newFormattedDates.Add(formattedDate);
            }

            return (newLessonDates, newFormattedDates);
        }


        public static int GetIntensity(int studentID)
        {
            //Intensity
            string hasBookingQuery = $"SELECT CASE WHEN EXISTS (SELECT 1 FROM bookedslots WHERE StudentID = {studentID}) THEN 1 ELSE 0 END";
            int hasBookingInt = ExecuteScalar<int>(hasBookingQuery);
            bool hasBooking = (hasBookingInt == 1);

            int intensity;

            if (hasBooking)
            {
                Console.WriteLine(); //add empty line
                string intensityQuery = $"SELECT COUNT(*) FROM lessonbookings WHERE StudentID = {studentID}";
                intensity = ExecuteScalar<int>(intensityQuery) * 4;

                while (true)
                {
                    Console.WriteLine($"Do you want to renew the package with an intensity of {intensity}?");
                    Console.WriteLine("Enter 'y' for yes or 'n' for no");
                    string manualIntensityChoice = Console.ReadLine().ToLower();

                    if (manualIntensityChoice == "y")
                    {
                        // unchanged intensity value
                        break; // Exit the loop
                    }
                    else if (manualIntensityChoice == "n")
                    {
                        intensity = CheckAndSetIntensity();
                        break; // Exit the loop
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter 'y' or 'n'.");
                    }
                }

            }
            else
            {
                Console.WriteLine(); //add empty line
                Console.Write("Enter desired intensity: ");
                intensity = CheckAndSetIntensity();
            }
            return intensity;
        }

        public static string GetLevel(int studentID)
        {
            string levelQuery = $"SELECT Level FROM studydetails WHERE StudentID = {studentID}";
            string level = ExecuteScalar<string>(levelQuery);

            return level;
        }

        public static int CheckAndSetIntensity()
        {
            int intensity;
            Console.WriteLine(); //add empty line
            Console.WriteLine("Enter desired intensity (4,8,12,16,..): ");


            while (true)
            {
                string intensityInput = Console.ReadLine();

                if (int.TryParse(intensityInput, out intensity) && intensity > 0 && intensity % 4 == 0)
                {
                    // Successfully parsed the input as an integer
                    break; // Exit the loop
                }
                else
                {
                    Console.WriteLine("Invalid intensity. (4,8,12,16,..)");
                    Console.WriteLine();
                }

            }

            return intensity;
        }

        public static void GetStudentNameAndID(List<string> studentNames, out int studentID, out string studentName)
        {
            //Choose Name and get StudentID
            while (true)
            {
                Console.Write("Enter a student name: ");
                studentName = Console.ReadLine();


                if (studentNames.Contains(studentName))
                {

                    // Define your SQL query
                    string studentIDQuery = $"SELECT StudentID FROM students WHERE Name = '{studentName}'";

                    // Call the method to execute the query and fetch a value
                    studentID = ExecuteScalar<int>(studentIDQuery);

                    break;

                }
                else
                {
                    Console.WriteLine("Invalid name. Please enter a valid name.");
                }
            }
        }

        public static List<string> GetStudentlist()
        {
            //get and display Studentlist
            string currentlyBookingStudentsQuery = "SELECT students.Name FROM students JOIN studydetails ON students.StudentID = studydetails.StudentID WHERE studydetails.Studystatus IN ('booking', 'pending') ORDER BY students.Name ASC";
            List<string> studentNames = ExecuteQueryForStrings(currentlyBookingStudentsQuery);

            Console.WriteLine("Currently Booking Student Names:");
            foreach (string name in studentNames)
            {
                Console.WriteLine(name);
            }
            Console.WriteLine(); //add empty line
            return studentNames;
        }

        public static List<int> GetStudentSlots(int studentID, int amountOfWeekdays)
        {

            //Console.WriteLine("");
            //string intensityInput = Console.ReadLine();

            //get and display Studentlist
            string studentSlotsQuery = $"SELECT SlotID FROM bookedslots WHERE StudentID = {studentID};";
            List<int> slotIDs = ExecuteQueryForInts(studentSlotsQuery);

            Console.WriteLine("Currently booked slots: ");
            foreach (int slotID in slotIDs)
            {
                Console.WriteLine(slotID);
            }
            Console.WriteLine(); //add empty line

            string manualSlotsChoice = "n";

            while (true)
            {

                if (slotIDs.Count == amountOfWeekdays) 
                { 
                    Console.WriteLine("Are these Slots correct? y/n");
                    Console.WriteLine("Enter 'y' for yes or 'n' for no");
                    manualSlotsChoice = Console.ReadLine().ToLower();
                }


                if (manualSlotsChoice == "y")
                {
                    // unchanged intensity value
                    break; // Exit the loop
                }
                else if (manualSlotsChoice == "n")
                {
                    slotIDs.Clear();

                    Console.WriteLine("");

                    for (int i = 0; i < amountOfWeekdays; i++)
                    {
                        while(true)
                        {
                            
                            Console.Write("Enter Slot: ");

                            string slotInput = Console.ReadLine();

                            if (int.TryParse(slotInput, out int result) && result >= 1 && result <= 56)
                            {
                                slotIDs.Add(result);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a valid integer.");
                            }

                        }
                    }
                    break; // Exit the loop
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please enter 'y' or 'n'.");
                }
            }


            return slotIDs;
        }

        public static T ExecuteScalar<T>(string query)
        {
            using (MySqlCommand command = new MySqlCommand(query, conn))
            {
                try
                {
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (T)Convert.ChangeType(result, typeof(T));
                    }

                    return default(T);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                    return default(T);
                }
            }
        }

        public static T ExecuteQuery<T>(string query)
            where T : ICollection<string>, new()
        {
            using (MySqlCommand command = new MySqlCommand(query, conn))
            {
                T result = new T();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dynamic value = reader[0];
                        result.Add(value);
                    }
                }

                return result;
            }
        }

        public static List<string> ExecuteQueryForStrings(string query)
        {
            List<string> result = new List<string>();

            using (MySqlCommand command = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string value = reader[0].ToString();
                        result.Add(value);
                    }
                }
            }

            return result;
        }

        public static List<int> ExecuteQueryForInts(string query)
        {
            List<int> result = new List<int>();

            using (MySqlCommand command = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (int.TryParse(reader[0].ToString(), out int value))
                        {
                            result.Add(value);
                        }
                    }
                }
            }

            return result;
        }
        public static List<DateTime> ExecuteQueryForDates(string query)
        {
            List<DateTime> result = new List<DateTime>();

            using (MySqlCommand command = new MySqlCommand(query, conn))
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add((DateTime)reader["Date"]);
                }
            }

            return result;
        }


    }

}
