using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ImageManipulation
{
    class Program
    {
        static void Main(string[] args)
        {
            // Ask the user for file paths and number of old package lessons
            Console.Write("Enter the file path for the JPEG image: ");
            string jpgFilePath3 = @"C:\Programmieren\ProgrammingProjects\PythonProject\src\GoogleAppsAutomation\GoogleDriveNative\Downloads\SlotsPictures\28022024_201625_Mario__zahlungsbestätigung_Test.jpg";
            string jpgFilePath2 = @"C:\Programmieren\ProgrammingProjects\PythonProject\src\GoogleAppsAutomation\GoogleDriveNative\Downloads\SlotsPictures\Test2Modify.jpg";
            string jpgFilePath = @"C:\Programmieren\ProgrammingProjects\PythonProject\src\GoogleAppsAutomation\GoogleDriveNative\Downloads\SlotsPictures\test\23012024_232804_Carissa__zahlungsbestätigung.jpg";

            Console.Write("Enter the file path for the PNG image: ");
            string pngFilePath = @"C:\Programmieren\ProgrammingProjects\PythonProject\src\GoogleAppsAutomation\GoogleDriveNative\Downloads\projectFiles\Paraf_withoutEdges_Test.png";

            Console.Write("How many OldPackage lessons? 4,8,12,16,20,24");
            int oldPackageLessons = int.Parse(Console.ReadLine());

            // Load the JPEG and PNG images
            Bitmap jpgImage = new Bitmap(jpgFilePath);
            Bitmap pngImage = new Bitmap(pngFilePath);

            // Define the position where the PNG image will be inserted into the JPEG image

            int x = 1690; // Column 1
            int x2 = 3674; // Column 2
            int yR1 = 1660; // Intensität 4 Y position
            int yR2 = 2290; // Intensität 4 Y position
            int yR3 = 2920; // Intensität 4 Y position
            int yR4 = 3550; // Intensität 4 Y position
            int Y_Differenz = 630; // Differenz zwischen den Reihen
            int Y_Verschiebung = 158; // Verschiebung nach unten zwischen Intensität 4 und Intensität 8

            if (oldPackageLessons == 4)
            {
                // Insert the PNG image into the JPEG image
                using (Graphics graphics = Graphics.FromImage(jpgImage))
                {
                    graphics.DrawImage(pngImage, x, yR1, pngImage.Width, pngImage.Height); // 1
                    graphics.DrawImage(pngImage, x2, yR1, pngImage.Width, pngImage.Height); // 2
                    graphics.DrawImage(pngImage, x, yR2, pngImage.Width, pngImage.Height); // 3
                    graphics.DrawImage(pngImage, x2, yR2, pngImage.Width, pngImage.Height); // 4
                }
            }
            else if (oldPackageLessons == 8)
            {
                // Insert the PNG image into the JPEG image
                using (Graphics graphics = Graphics.FromImage(jpgImage))
                {
                    yR1 = yR1 + Y_Verschiebung;
                    yR2 = yR2 + Y_Verschiebung;
                    yR3 = yR3 + Y_Verschiebung;
                    yR4 = yR4 + Y_Verschiebung;

                    graphics.DrawImage(pngImage, x, yR1, pngImage.Width, pngImage.Height); // 1
                    graphics.DrawImage(pngImage, x2, yR1, pngImage.Width, pngImage.Height); // 2
                    graphics.DrawImage(pngImage, x, yR2, pngImage.Width, pngImage.Height); // 3
                    graphics.DrawImage(pngImage, x2, yR2, pngImage.Width, pngImage.Height); // 4
                    graphics.DrawImage(pngImage, x, yR3, pngImage.Width, pngImage.Height); // 5
                    graphics.DrawImage(pngImage, x2, yR3, pngImage.Width, pngImage.Height); // 6
                    graphics.DrawImage(pngImage, x, yR4, pngImage.Width, pngImage.Height); // 7
                    graphics.DrawImage(pngImage, x2, yR4, pngImage.Width, pngImage.Height); // 8
                }
            }

            //*****************add more possible locations


            // Save the modified JPEG image
            //string outputFilePath = $"Modified_{oldPackageLessons}_lessons.jpg";
            string outputDirectory = @"C:\Programmieren\ProgrammingProjects\PythonProject\src\GoogleAppsAutomation\GoogleDriveNative\Downloads\SlotsPictures\test\";
            string outputFileName = $"Modified_LessonsCard.jpg";
            string outputFilePath = System.IO.Path.Combine(outputDirectory, outputFileName);
            jpgImage.Save(outputFilePath);

            // Dispose the images to release resources
            jpgImage.Dispose();
            pngImage.Dispose();

            Console.WriteLine($"Image with PNG inserted saved as {outputFilePath}");
        }
    }
}
