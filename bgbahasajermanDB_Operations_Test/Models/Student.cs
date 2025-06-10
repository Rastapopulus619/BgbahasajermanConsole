using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using System.Reflection.Metadata.Ecma335;
using bgbahasajermanDB_Libraries;

namespace bgbahasajermanDB_Operations_Test.Models
{
    public class Student
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string Title { get; set; }
        public string StudyStatus { get; set; }
        public string Level { get; set; }   //***************************NEW ! write into Flowchart
        public string Currency { get; set; }   //***************************NEW ! write into Flowchart
        public string DiscountID { get; set; }  //***************************NEW ! write into Flowchart
        public int DiscountInt { get; set; }  //***************************NEW ! write into Flowchart
        public bool HasBooking { get; set; }
        public bool NewStudent { get; set; }
        public bool DoublePayment { get; set; }
        public int OldIntensity { get; set; }
        public int NewIntensity { get; set; }
        public Package OldPackage { get; set; }
        public Package NewPackage { get; set; }
        public Package NewPackage2 { get; set; } // could this be just a repetition of the procedure making/converting a NewPackage into an OldPackage class object?






        public List<Package> Packages { get; set; } = new List<Package>();

        public Student()
        {

            Console.WriteLine("\nStudent Created");
            SetStudentName();
            StudentID = GetStudentIdByName(StudentName);
            Title = GetTitle(StudentID);
            StudyStatus = GetStudyStatus(StudentID);
            Level = GetLevel(StudentID);//***********************Write into FLowchart!
            HasBooking = CheckIfBookingExists(StudentID);

            UpdateStudentPricing();//***********************Write into FLowchart!


            Console.Clear();

            PrintStudentDetails();

            OldIntensity = GetCurrentIntensity(StudentID) * 4;

            //Where to print student details
            bool studentExists = CheckIfNewStudent(StudentID);
            if (studentExists)
            {
                OldPackage = new OldPackage(StudentID);
            }
            else
            {
                NewStudent = true;
                Console.WriteLine("************");
                Console.WriteLine("No OldPackage was created because the student is a new Student!");
                Console.WriteLine("************");
            }

            /*
             * DoublePayment
             * OldPackage (in process)****
             * NewPackage
             * NewPackage2
             */

        }

        public void UpdateStudentPricing()
        {
            // Call the GetStudentPricing method to retrieve the values
            (string currency, string discountID) = GetStudentPricing(StudentID);

            // Assign the retrieved values to the properties
            Currency = currency;
            DiscountID = discountID;

            if(DiscountID != "")
            {
                DiscountInt = GetDiscountAmount(DiscountID);
            }
        }

        public void CreateNewPackage()
        {
            if (!NewStudent)
            {
                NewPackage = new NewPackage(this);
            }
            else
            {
                NewPackage = new FirstPackage(this);
            }

        }

        private void PrintStudentDetails()
        {
            Console.WriteLine($"Student: {StudentName}\nStatus: {StudyStatus}");
            if (HasBooking) 
            {
                Console.WriteLine("Booked Slot found.");
            }
            else 
            { 
                Console.WriteLine("No Booked Slot found."); 
            }
        }

        public void SetStudentName()
        {
            List<string> students = GetAllStudentNames();

            while (true)
            {
                string studentName = ValidateStudentName("Enter student who's Package should be updated: ");

                if (students.Contains(studentName))
                {
                    StudentName = studentName;
                }
                else
                {
                    Console.WriteLine("Student's name not in students\n");
                    bool addNewStudentChoice = YesNoChoice("Add New Student?");
                    if (addNewStudentChoice)
                    {
                        NewStudent = true;
                        string newStudentName = AddNewStudentToDatabase();

                        students = GetAllStudentNames();

                        if (students.Contains(newStudentName))
                        {
                            StudentName = newStudentName;
                            //Console.WriteLine("Loop Ending break");
                            //break;
                        }
                        else
                        {
                            throw new Exception($"Creating New Student {newStudentName} Failed.");
                        }
                    }
                    else
                    {
                        continue;
                    }
            
                }

                //Console.WriteLine("Loop Ending break");
                break;

            }
        }





        
        public string AddNewStudentToDatabase()
        {
            string newStudentName = ValidateStudentName("Re-enter new student name:");
            int newStudentNumber = GetStudentNumberPlusOne();
            string title = "";

            bool addTitleChoice = YesNoChoice("Add Title (Herr/Frau)?");
            if(addTitleChoice)
            {
                
                Console.WriteLine("Pick a Title: '1' for 'Herr', '2' for 'Frau'.");
                int numberChoice = NumericChoice();
                if(numberChoice == 1)
                { 
                    title = "Herr"; 
                }
                else
                {
                    title = "Frau";
                }

                InsertNewStudent(newStudentNumber, newStudentName, title);
            }
            else
            {
                InsertNewStudent(newStudentNumber, newStudentName, title);
            }


            return newStudentName;
        }

        public static string ValidateStudentName(string EnterStudentMessage)
        {
            string newStudentName;

            while (true)
            {
                Console.WriteLine("Enter Student Name:");

                // Get user input
                newStudentName = Console.ReadLine();

                // Check if the input is not empty
                if (string.IsNullOrWhiteSpace(newStudentName))
                {
                    Console.WriteLine("Error: Name cannot be empty. Please try again.");
                    continue;
                }

                // Check if the input is not just one character
                if (newStudentName.Length == 1)
                {
                    Console.WriteLine("Error: Name must be longer than one character. Please try again.");
                    continue;
                }

                // Check if the input contains only alphabetical characters, spaces, and dots
                if (!newStudentName.All(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '.'))
                {
                    Console.WriteLine("Error: Name can only contain alphabetical characters, spaces, and dots. Please try again.");
                    continue;
                }

                // Check if the first letter of each word is capital
                if (!newStudentName.Split(' ').All(word => char.IsUpper(word[0])))
                {
                    Console.WriteLine("Error: The first letter of each word must be capitalized. Please try again.");
                    continue;
                }

                // All conditions met, return the processed name
                return newStudentName;
            }
        }

    }
}
