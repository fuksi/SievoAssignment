using CommandLine;
using System;

namespace SievoAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            try { 
                var etl = new Etl(new SievoLogger());
                etl.Execute(args);
                return;
            } 
            catch (ArgumentException ex)
            {
                Console.WriteLine("Failure in processing input. Details:");
                Console.WriteLine(ex.Message);
                Environment.Exit(0);
            }
            catch (FormatException)
            {
                Console.WriteLine("Failure in processing input. Please check if date value and numeric value is in correct formats");
                Environment.Exit(0);
            }
        }
    }
}
