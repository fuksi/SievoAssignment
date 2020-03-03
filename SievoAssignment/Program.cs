using CommandLine;

namespace SievoAssignment
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<EtlOptions>(args).WithParsed(opt =>
            {
                var etl = new Etl();
                etl.Execute(opt);
            });
        }
    }
}
