using ErrorLog;
using System;
using System.Diagnostics;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            ErrorLog.System.OutputPath = "ERRORS.txt";
            ErrorLog.System.SetFormat(new object[] { new ErrorLog.System.Time(), new ErrorLog.System.Message(), new ErrorLog.System.TargetSite()});
            ErrorLog.System.AppendFromLastInstance = false;


            Console.WriteLine("Starting Test");
            string[] Array = { "Test", "Test", "Test" };
            Stopwatch t = new Stopwatch();
            long Average = 0;

            for (int i = 0; i != 20; i++)
            {
                try
                {
                    string S = Array[300];
                }
                catch (Exception E)
                {
                    t.Start();
                    ErrorLog.System.LogError(E);
                    t.Stop();
                }
                Average += t.ElapsedMilliseconds;
                Console.WriteLine(t.ElapsedMilliseconds);
                t.Reset();
            }

            Console.WriteLine("Average Write Time in MS: " + Average / 20);


            Console.ReadKey();
        }
    }
}
