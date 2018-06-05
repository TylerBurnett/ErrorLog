using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ErrorLog
{
    public static class System
    {
        internal static bool CheckFile = true;
        internal static object[] PresetFormat = new object[] { new Time { DateFormat = "DD:HH:ss" }, new TargetSite(), new Source() };
        private static object[] ErrorFormat;
        public static bool AppendFromLastInstance { get; set; }
        public static string ErrorFilePath { get; set; }

        #region Front end code

        public static void LogError(Exception Error)
        {
            StringBuilder ErrorMessage = null;

            foreach (object O in ErrorFormat)
            {
                if (O.GetType().IsAssignableFrom(typeof(ErrorBaseClass)))
                {
                    ErrorMessage.Append(((ErrorBaseClass)O).GetString(Error));
                }
                else if (O.GetType().IsAssignableFrom(typeof(StringBaseClass)))
                {
                    ErrorMessage.Append(((StringBaseClass)O).GetString());
                }
            }

            IOFunctions.WriteLog(ErrorMessage.ToString());
        }

        public static void SetFormat(object[] Format)
        {
            foreach (Type Object in Format)
            {
                if (Object.IsSubclassOf(typeof(System)))
                {
                    // continue
                }
                else
                {
                    throw new InvalidObjectException(Object.FullName);
                }
            }
            ErrorFormat = Format;
        }

        #region Data arrangement Types

        public abstract class ErrorBaseClass
        {
            internal abstract string GetString(Exception Error);
        }

        public abstract class StringBaseClass
        {
            internal abstract string GetString();
        }

        #region Sub classes

        public class Data : ErrorBaseClass
        {
            internal override String GetString(Exception Error)
            {
                return Error.Data.ToString();
            }
        }

        public class HelpLink : ErrorBaseClass
        {
            internal override String GetString(Exception Error)
            {
                return Error.HelpLink.ToString();
            }
        }

        public class Hresult : ErrorBaseClass
        {
            internal override String GetString(Exception Error)
            {
                return Error.HResult.ToString();
            }
        }

        public class InnerException : ErrorBaseClass
        {
            internal override String GetString(Exception Error)
            {
                return Error.InnerException.ToString();
            }
        }

        public class Message : ErrorBaseClass
        {
            internal override String GetString(Exception Error)
            {
                return Error.Message.ToString();
            }
        }

        public class NewLine : ErrorBaseClass
        {
            internal override String GetString(Exception Error)
            {
                return Environment.NewLine;
            }
        }

        public class Source : ErrorBaseClass
        {
            internal override String GetString(Exception Error)
            {
                return Error.Source.ToString();
            }
        }

        public class StackTrace : ErrorBaseClass
        {
            internal override String GetString(Exception Error)
            {
                return Error.StackTrace.ToString();
            }
        }

        public class TargetSite : ErrorBaseClass
        {
            internal override String GetString(Exception Error)
            {
                return Error.TargetSite.ToString();
            }
        }

        public class Time : StringBaseClass
        {
            public string DateFormat { get; set; }

            internal override String GetString()
            {
                if (DateFormat.Length > 0)
                {
                    return DateTime.Now.ToString(DateFormat);
                }
                else return DateTime.Now.ToString("MM:DD:HH:mm");
            }
        }

        #endregion Sub classes

        #endregion Data arrangement Types

        #endregion Front end code

        #region Back end code

        internal class IOFunctions
        {
            private static List<String> WriteQueue = new List<string>();

            public static bool CanAccessFile()
            {
                if (ErrorLog.ErrorFilePath.Length > 0)
                {
                    if (!File.Exists(ErrorFilePath))
                    {
                        File.Create(ErrorFilePath).Dispose();
                    }

                    FileInfo file = new FileInfo(ErrorFilePath);
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

            public static void WriteLog(String Log)
            {
                WriteQueue.Add(Log);

                if (CanAccessFile())
                {
                    // !Appending From last instance Method
                    if (!AppendFromLastInstance && CheckFile)
                    {
                        if (File.ReadAllText(ErrorFilePath).Length == 0)
                        {
                            //Overwrite the file and remove the need for checkfile for this instance
                            File.Create(ErrorFilePath);
                            CheckFile = false;
                        }
                    }

                    try
                    {
                        // Finally Write the Data and clear the current writeQueue
                        File.AppendAllText(ErrorLog.ErrorFilePath, string.Join(Environment.NewLine, WriteQueue.ToArray()));
                        WriteQueue.Clear();
                    }
                    catch (Exception)
                    {
                        // If an error occured in writing the data, add it into the queue to be
                        // written on next call, or when disposed
                    }
                }
            }
        }

        [Serializable]
        internal class InvalidObjectException : Exception
        {
            public InvalidObjectException(string ObjectName)
                : base(String.Format("Internal objects of Errorlog can only be used in error format: {0}", ObjectName))
            {

            }

        }
        #endregion Back end code
    }
}