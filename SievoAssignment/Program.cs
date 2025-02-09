﻿using System;
using System.IO;

namespace SievoAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var etl = new Etl(new SievoLogger());
                etl.Execute(args);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Failure in processing input. Details:");
                Console.WriteLine(ex.Message);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Cannot find the file at the specified path. Please check again");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Cannot find the file at the specified path. Please check again");
            }
            catch (FormatException)
            {
                Console.WriteLine("Failure in processing input. Please check if date value and numeric value is in correct formats");
            }
            finally
            {
                Environment.Exit(0);
            }
        }
    }
}
