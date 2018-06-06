using System;
using System.Diagnostics;
using System.IO;
using TylerBurnett;

namespace ConsoleTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Library Variables
            ErrorLog.OutputPath = "ErrorLog.txt";
            ErrorLog.ErrorFormat = (new object[] { new ErrorLog.Time(), new ErrorLog.Message(), new ErrorLog.TargetSite() });
            ErrorLog.AppendFromLastInstance = false;

            // Stopwatch and counter
            Stopwatch S = new Stopwatch();
            long count = 0;


            //Repetitive Log Test
            int[] Array = new int[0];
            for (int i = 0; i != 1000; i++)
            {
                try
                {
                    int b = Array[10];
                }
                catch (Exception e)
                {
                    S.Start();
                    ErrorLog.LogError(e);
                    S.Stop();
                    count += S.ElapsedTicks;
                    Console.WriteLine(S.ElapsedTicks);
                    S.Reset();
                }                
            }
            Console.WriteLine("Average Time = " + count / 1000);

            Console.ReadKey();
        }

       
    }
}