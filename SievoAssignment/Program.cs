using CommandLine;

namespace SievoAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            var etl = new Etl();
            etl.Execute(args);
        }
    }
}
