using System;

namespace SievoAssignment
{
    public class SievoLogger : ISievoLogger
    {
        public void Info(string msg)
        {
            Console.WriteLine(msg);
        }
    }

    public interface ISievoLogger
    {
        public void Info(string msg);
    }
}
