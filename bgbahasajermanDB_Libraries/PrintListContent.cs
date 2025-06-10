
namespace bgbahasajermanDB_Libraries
{

    public static class PrintListContent
    {
        public static void PrintListOfStrings(List<string[]> absenCsvData)
        {
            Console.WriteLine("Absen CSV Data:");
            foreach (var rowData in absenCsvData)
            {
                Console.WriteLine(string.Join(", ", rowData));
            }
            Console.WriteLine();
        }

        public static void PrintListOfStringLists(List<List<string>> allReplacements)
        {
            Console.WriteLine("All Replacements:");
            foreach (var values in allReplacements)
            {
                Console.WriteLine(string.Join(", ", values));
            }
            Console.WriteLine();
        }
    }

}