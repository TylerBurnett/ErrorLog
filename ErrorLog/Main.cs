using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TylerBurnett
{
    public class ErrorLog : IDisposable
    {

        #region Public Properties

        //Front end
        public static bool AppendFromLastInstance { get; set; }

        public static string OutputPath { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Front end function for recieving and formatting the error for write
        /// </summary>
        /// <param name="Error">The exception object</param>
        public static void LogError(Exception Error)
        {
            StringBuilder ErrorMessage = new StringBuilder();


            foreach (object O in ErrorFormat)
            {
                if (O.GetType().IsSubclassOf(typeof(ExceptionBaseClass)))
                {
                    ErrorMessage.Append(((ExceptionBaseClass)O).GetString(Error));
                }
                else if (O.GetType().IsSubclassOf(typeof(StringBaseClass)))
                {
                    ErrorMessage.Append(((StringBaseClass)O).GetString());
                }

                ErrorMessage.Append(", ");
            }
            ErrorMessage.Append(Environment.NewLine);
            IOFunctions.WriteLog(ErrorMessage.ToString());
        }

        /// <summary>
        /// Sets and the format of the outputted error after checking the array is only internal objects
        /// </summary>
        /// <param name="Format">The object array containing the desired output format of the error</param>
        public static void SetFormat(object[] Format)
        {
            foreach (Object O in Format)
            {
                Type ObjectType = O.GetType();

                if (ObjectType.IsSubclassOf(typeof(ExceptionBaseClass)) || ObjectType.IsSubclassOf(typeof(StringBaseClass)))
                {
                    // continue
                }
                else
                {
                    throw new InvalidObjectException(O.GetType().Name);
                }
            }
            ErrorFormat = Format;
        }

        /// <summary>
        /// Disposal of the library instance
        /// </summary>
        public void Dispose()
        {
            IOFunctions.FlushQueue();
        }

        #endregion Public Methods

        #region Public Classes

        public class Data : ExceptionBaseClass
        {

            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.Data.ToString();
            }

            #endregion Internal Methods

        }


        /// <summary>
        /// These are the two big daddy parent classes #polymorphism
        /// </summary>
        public abstract class StringBaseClass
        {

            #region Internal Methods

            internal abstract string GetString();

            #endregion Internal Methods

        }

        public abstract class ExceptionBaseClass
        {

            #region Internal Methods

            internal abstract string GetString(Exception Error);

            #endregion Internal Methods

        }

        /// <summary>
        /// All these classes are pretty self explanatory, They wrap around the current types in the exception object
        /// access on a normal exception
        /// </summary>
        public class HelpLink : ExceptionBaseClass
        {

            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.HelpLink.ToString();
            }

            #endregion Internal Methods

        }

        public class Hresult : ExceptionBaseClass
        {

            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.HResult.ToString();
            }

            #endregion Internal Methods

        }

        public class InnerException : ExceptionBaseClass
        {

            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.InnerException.ToString();
            }

            #endregion Internal Methods

        }

        public class Message : ExceptionBaseClass
        {

            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.Message.ToString();
            }

            #endregion Internal Methods

        }

        public class NewLine : ExceptionBaseClass
        {

            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Environment.NewLine;
            }

            #endregion Internal Methods

        }

        public class Source : ExceptionBaseClass
        {

            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.Source.ToString();
            }

            #endregion Internal Methods

        }

        public class StackTrace : ExceptionBaseClass
        {

            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.StackTrace.ToString();
            }

            #endregion Internal Methods

        }

        public class TargetSite : ExceptionBaseClass
        {

            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.TargetSite.ToString();
            }

            #endregion Internal Methods

        }

        public class HashCode : ExceptionBaseClass
        {

            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.GetHashCode().ToString();
            }

            #endregion Internal Methods

        }

        public class Time : StringBaseClass
        {

            #region Public Properties

            public string DateFormat { get; set; }

            #endregion Public Properties

            #region Internal Methods

            internal override String GetString()
            {
                if (DateFormat == null || DateFormat.Length > 0)
                {
                    return DateTime.Now.ToString(DateFormat);
                }
                else return DateTime.Now.ToString("MM:DD:HH:mm");
            }

            #endregion Internal Methods

        }

        #endregion Public Classes

        #region Internal Fields

        // Back end
        internal static bool CheckFile = true;

        internal static object[] ErrorFormat = PresetFormat;

        internal static object[] PresetFormat = new object[] { new Time { DateFormat = "DD:HH:ss" }, new Message(), new TargetSite(), new Source() };

        #endregion Internal Fields

        #region Internal Classes

        /// <summary>
        /// Error stuff cause you know somones gonna try to break this
        /// </summary>
        [Serializable]
        internal class InvalidObjectException : Exception
        {

            #region Public Constructors

            public InvalidObjectException(string ObjectName)
            : base(String.Format("Object type: {0} invalid. Internal objects of Errorlog can only be used in error format", ObjectName))
            {
            }

            #endregion Public Constructors

        }

        internal class IOFunctions
        {

            #region Public Methods

            /// <summary>
            /// Checks the current accessability of the desired file
            /// </summary>
            /// <returns>Boolean</returns>
            public static bool CanAccessFile()
            {
                if (ErrorLog.OutputPath.Length > 0)
                {
                    if (!File.Exists(OutputPath))
                    {
                        File.Create(OutputPath).Dispose();
                    }

                    FileInfo file = new FileInfo(OutputPath);
                    FileStream stream = null;

                    try
                    {
                        stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                    }
                    catch (IOException)
                    {
                        return false;
                    }
                    finally
                    {
                        if (stream != null) stream.Close();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Backup function to ensure all error data is written, whether that be in another file
            /// or not
            /// </summary>
            public static void FlushQueue()
            {
                if (WriteQueue.Count > 0)
                {
                    bool FlushComplete = false;

                    if (CanAccessFile())
                    {
                        try
                        {
                            // Write remaining data to file
                            File.AppendAllText(OutputPath, string.Join(Environment.NewLine, WriteQueue.ToArray()));
                            WriteQueue.Clear();
                            FlushComplete = true;
                        }
                        catch
                        {
                        }
                    }
                    else if (FlushComplete == false)
                    {
                        try
                        {
                            // Create a file in the same directory as the output and then write the data
                            FileInfo Output = new FileInfo(OutputPath);

                            // Ha, cant write to this file cause its all mine
                            using (FileStream fs = File.Create(Output.DirectoryName + @"\ErrorLogBackup.txt"))
                            {
                                Byte[] info = new UTF8Encoding(true).GetBytes(string.Join(Environment.NewLine, WriteQueue.ToArray()));
                                fs.Write(info, 0, info.Length);
                            }
                            WriteQueue.Clear();
                            FlushComplete = true;
                        }
                        catch
                        {
                            // God save our souls if it manages to get this far
                        }
                    }
                }
            }

            /// <summary>
            /// Writes the desired string to the specified output file
            /// </summary>
            /// <param name="FormattedError">The string formatted error created by LogError()</param>
            public static void WriteLog(String FormattedError)
            {
                WriteQueue.Add(FormattedError);

                if (CanAccessFile())
                {
                    // Append from last instance is a safer write method for the first bit atleast
                    if (AppendFromLastInstance == false && CheckFile)
                    {
                        using (FileStream fs = File.Create(OutputPath))
                        {
                            Byte[] info = new UTF8Encoding(true).GetBytes(string.Join(Environment.NewLine, WriteQueue.ToArray()));
                            fs.Write(info, 0, info.Length);
                            CheckFile = false;
                            WriteQueue.Clear();
                        }
                    }
                    else
                    {
                        try
                        {
                            // Finally Write the Data and clear the current writeQueue
                            File.AppendAllText(ErrorLog.OutputPath, string.Join(Environment.NewLine, WriteQueue.ToArray()));
                            WriteQueue.Clear();
                        }
                        catch (Exception E)
                        {
                            // If an error occured in writing the data, add it into the queue to be
                            // written on next call, or when disposed
                            Console.WriteLine(E.Message);
                        }
                    }
                }
            }

            #endregion Public Methods

            #region Private Fields

            /// <summary>
            /// The backup queue that stores all unwritten exceptions
            /// </summary>
            private static List<String> WriteQueue = new List<string>();

            #endregion Private Fields

        }
        #endregion Internal Classes

    }
}