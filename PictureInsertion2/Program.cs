using System;
using System.Drawing;

namespace ImageManipulation
{
    class Program
    {
        static void Main(string[] args)
        {
            // Ask the user for file paths and number of old package lessons
            Console.Write("Enter the file path for the JPEG image: ");
            string jpgFilePath = @"C:\Programmieren\ProgrammingProjects\PythonProject\src\GoogleAppsAutomation\GoogleDriveNative\Downloads\SlotsPictures\28022024_201625_Mario__zahlungsbestätigung_Test.jpg";

            Console.Write("Enter the file path for the PNG image: ");
            string pngFilePath = @"C:\Programmieren\ProgrammingProjects\PythonProject\src\GoogleAppsAutomation\GoogleDriveNative\Downloads\projectFiles\Paraf_withoutEdges_Test.png";

            //Console.Write("How many OldPackage lessons? ");
            //int oldPackageLessons = int.Parse(Console.ReadLine());

            // Load the JPEG and PNG images
            Bitmap jpgImage = new Bitmap(jpgFilePath);
            Bitmap pngImage = new Bitmap(pngFilePath);

            // Define the position where the PNG image will be inserted into the JPEG image
            int x = 100; // Example X position
            int y = 100; // Example Y position

            // Insert the PNG image into the JPEG image
            using (Graphics graphics = Graphics.FromImage(jpgImage))
            {
                graphics.DrawImage(pngImage, x, y, pngImage.Width, pngImage.Height);
            }

            // Save the modified JPEG image
            //string outputFilePath = $"Modified_{oldPackageLessons}_lessons.jpg";
            string outputDirectory = @"C:\Programmieren\ProgrammingProjects\PythonProject\src\GoogleAppsAutomation\GoogleDriveNative\Downloads\SlotsPictures";
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
