using System;
using System.Diagnostics;
using TylerBurnett;

namespace ConsoleTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //ErrorLog.ProcessorObject = new ErrorLog.JsonProcessor();
            object[] CustomFormat = new object[] { new ErrorLog.Time(), new ErrorLog.Message(), new ErrorLog.TargetSite() };
            string CustomPath =  "ErrorLog.txt";

            // Library Variables
            ErrorLog.OutputPath = CustomPath;
            ErrorLog.ErrorFormat = CustomFormat;
            ErrorLog.AppendFromLastInstance = false;

            Stopwatch S = new Stopwatch();
            long I = 0;

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
                }
                I += S.ElapsedTicks;
                Console.WriteLine(S.ElapsedTicks);
                S.Reset();
            }
            Console.WriteLine("Average time :" + I / 1000);

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        public static void RepetitionTest()
        {

            int[] Array = new int[0];
            for (int i = 0; i != 1000; i++)
            {
                try
                {
                    int b = Array[10];
                }
                catch (Exception e)
                {
                    ErrorLog.LogError(e);
                }
            }
        }

        public static void EmptyErrorTest()
        {
            Exception E = new Exception();

            try
            {
                ErrorLog.LogError(E);
            }
            catch
            {
                Console.WriteLine("Error Caught by library");
            }
        }

        public static void EmptyErrorFormatTest(object[] OldFormat)
        {
            ErrorLog.ErrorFormat = null;
            // Used to check that it recognises the changes
            ErrorLog.LogError(new FormatException());
            ErrorLog.ErrorFormat = (new object[] { new ErrorLog.Time(), new ErrorLog.Message(), new ErrorLog.TargetSite() });
        }

        public static void InvalidOutputPathTest(string OldPath)
        {
            ErrorLog.OutputPath = "";
            ErrorLog.LogError(new FormatException());
            ErrorLog.OutputPath = OldPath;
        }
    }

    public class CustomStringProcessor : ErrorLog.StringFormatPipeline
    {
        public override string ProcessString(String[] UnprocessedString)
        {

            //
            //
            //.....

            return "The processed string";
        }
    }
}