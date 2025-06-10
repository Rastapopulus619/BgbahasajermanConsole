using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using System.Reflection.Metadata.Ecma335;
using bgbahasajermanDB_Libraries;

namespace bgbahasajermanDB_Operations_Test
{
    public class ReplacementManagerStudent
    {
        public string StudentID { get; set; } 
        public string StudentName { get; set; }
        public int MaxPackageNumber { get; set; }

        public ReplacementManagerStudent()
        {

            SetStudentName();
            StudentID = GetStudentIdByName(StudentName);
            MaxPackageNumber = GetLastPackageNumber(StudentID);
        }
        //public Student(string studentName, string studentID)
        //{

        //    StudentName = studentName; 
        //    StudentID = studentID;
            
        //    MaxPackageNumber = GetLastPackageNumber(StudentID);
        //}

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
                    Console.WriteLine("Student's name was not found. Try again.\n");
                    continue;                   
                }

                //Console.WriteLine("Loop Ending break");
                break;

            }
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
