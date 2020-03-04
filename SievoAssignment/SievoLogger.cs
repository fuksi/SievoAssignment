using System;

namespace SievoAssignment
{
    public class SievoLogger : ISievoLogger
    {
        public void Info(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Info(string[] msgParts)
        {
            Console.WriteLine(string.Join('\t', msgParts));
        }
    }

    public interface ISievoLogger
    {
        public void Info(string msg);
        public void Info(string[] msgParts);
    }
}
