using TylerBurnett;
using System;
using System.Diagnostics;

namespace ConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            ErrorLog.OutputPath = "ERRORS.txt";
            ErrorLog.SetFormat(new object[] { new ErrorLog.Time(), new ErrorLog.Message(), new ErrorLog.TargetSite()});
            ErrorLog.AppendFromLastInstance = false;

            Exception E = new Exception();

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
                    ErrorLog.LogError(E);
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
