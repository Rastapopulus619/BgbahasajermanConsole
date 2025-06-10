using static bgbahasajermanDB_Libraries.MySqlQueryManager;
using static bgbahasajermanDB_Libraries.CommonUserInputHandling;
using static bgbahasajermanDB_Libraries.DateFormatting;
using bgbahasajermanDB_Libraries;
using bgbahasajermanDB_ReplacementsManager_Test.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using bgbahasajermanDB_ReplacementsManager_Test.Enums;

namespace bgbahasajermanDB_Operations_Test
{
    class Program
    {
        public  static ReplacementsEditPackageModel replacementsEditPackage { get; set; }
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();

                ReplacementManagerStudent student = new ReplacementManagerStudent();
                 

                List<DateTime> packageDates = GetSpecificPackageDates(student.StudentID, student.MaxPackageNumber);
                DisplayAPackage(student.StudentName, student.MaxPackageNumber, packageDates);

                bool chooseStudentChoice = YesNoChoice("\nChoose different student?");
                if(!chooseStudentChoice)
                {
                    while (true)
                    {
                        

                        int userInput = GetValidNumberInRange(1, student.MaxPackageNumber);

                        //List<DateTime> packageForEditing = new List<DateTime>();

                        if (userInput != student.MaxPackageNumber)
                        {
                            packageDates = GetSpecificPackageDates(student.StudentID, userInput);
                        }

                        DisplayAPackage(student.StudentName, userInput, packageDates);
                        Console.WriteLine();
                        Console.WriteLine($"Chosen PackageNumber is: {userInput}");

                        replacementsEditPackage = new ReplacementsEditPackageModel(student.StudentID, userInput);


                        bool changePackageChoice = ChooseAction(student);

                        if (changePackageChoice) 
                        { 
                            continue; 
                        }

                        changePackageChoice = YesNoChoice("Do you want to edit a different Package?");
                        if (changePackageChoice)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    bool changeStudentChoice = YesNoChoice("Do you want to edit a different Student's Package?");
                    if(changeStudentChoice)
                    {
                        continue;
                    }
                    else
                    {
                        bool exitChoice = YesNoChoice("Exit application?");
                        if (exitChoice)
                        {
                            break;
                        }
                        else 
                        {
                            continue;
                        }
                    }
                }
                else
                {

                }
            }
        }
       



        public  static void DisplayAPackage(string studentName, int lastPackageNumber, List<DateTime> packageDates)
        {
            Console.Clear();

            Console.WriteLine($"\n{studentName}'s Package {lastPackageNumber} Dates:");
            foreach (var date in packageDates)
            {
                Console.WriteLine(ConvertToCardDateFormat(date));
            }

        }


        public static bool ChooseAction(ReplacementManagerStudent student)
        {
            Console.WriteLine("1. Add Replacement");
            Console.WriteLine("2. Revert Replacement");
            Console.WriteLine("3. Edit Package (not yet)");
            Console.WriteLine("4. Delete Package");
            Console.WriteLine("5. Print Lesson Card (not yet)");
            Console.WriteLine("0. Choose Different Package");

            while (true)
            {
                string actionChoice = Console.ReadLine();

                // Parse the user input into the enum
                if (Enum.TryParse<ActionChoices>(actionChoice, out ActionChoices chosenAction))
                {

                    switch (chosenAction)
                    {
                        case ActionChoices.Choose_Different_Package:
                            Console.WriteLine("Choose different Package was chosen! *****");
                            student.MaxPackageNumber = GetLastPackageNumber(student.StudentID);
                            return false;
                        case ActionChoices.Add_Replacement:
                            Console.WriteLine("Add Replacement was chosen! *****");
                            replacementsEditPackage.AddReplacement();
                            return false;
                        case ActionChoices.Revert_Replacement:
                            Console.WriteLine("Revert Replacement was chosen! *****");
                            replacementsEditPackage.RevertReplacement();
                            return false;
                        case ActionChoices.Edit_Package:
                            // MethodEditPackage();
                            return false;
                        case ActionChoices.Delete_Package:
                            Console.WriteLine("Delete Package was chosen! *****");
                            replacementsEditPackage.DeletePackage();
                            student.MaxPackageNumber = GetLastPackageNumber(student.StudentID);
                            return true;
                        case ActionChoices.Print_Lesson_Card:
                            Console.WriteLine("Print Lesson Card was chosen! *****");
                            replacementsEditPackage.PrintLessonCard(student, replacementsEditPackage);
                            return false;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }
            }
        }
    }
}
