using System;
using System.IO;
using System.Text;

namespace TylerBurnett
{
    public class ErrorLog : IDisposable
    {
        #region Public Properties

        public static bool AppendFromLastInstance { get; set; }

        public static object[] ErrorFormat { get; set; }

        public static string OutputPath { get; set; }

        #endregion Public Properties

        #region Internal Fields

        internal static bool Initialized = false;

        internal static FileStream OutputStream = null;

        internal static object[] PresetFormat = new object[] { new Time { DateFormat = "MM:DD:HH:mm" }, new Message(), new TargetSite(), new Source() };

        #endregion Internal Fields

        #region Public Methods

        /// <summary>
        /// Front end function for recieving and formatting and writing the error (Will be changed with the addition of xml and json)
        /// </summary>
        /// <param name="Error">The exception object</param>
        public static void LogError(Exception Error)
        {
            StringBuilder ErrorMessage = new StringBuilder();
            object[] _ErrorFormat;

            // Initialise the Output File beforehand
            if (Initialized == false)
            {
                OpenFile();
                Initialized = true;
            }

            // Check class isnt empty before running
            if (ErrorFormat == null || ErrorFormat.Length == 0)
            {
                _ErrorFormat = PresetFormat;
            }
            else
            {
                _ErrorFormat = ErrorFormat;
            }

            foreach (object FormatObject in _ErrorFormat)
            {
                Type FormateType = FormatObject.GetType();

                if (FormateType.IsSubclassOf(typeof(ExceptionBaseClass)))
                {
                    ErrorMessage.Append(((ExceptionBaseClass)FormatObject).GetString(Error));
                }
                else if (FormateType.IsSubclassOf(typeof(StringBaseClass)))
                {
                    ErrorMessage.Append(((StringBaseClass)FormatObject).GetString());
                }
                else
                {
                    throw new InvalidObjectException(FormateType.Name);
                }

                ErrorMessage.Append(", ");
            }
            ErrorMessage.Append(Environment.NewLine);

            byte[] Info = new UTF8Encoding(true).GetBytes(ErrorMessage.ToString());
            OutputStream.Write(Info, 0, Info.Length);
            OutputStream.Flush();
        }

        /// <summary>
        /// Disposal of the library instance
        /// </summary>
        public void Dispose()
        {
            OutputStream.Flush();
            OutputStream.Dispose();
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Checks the current accessability of the desired file
        /// </summary>
        /// <returns>Boolean</returns>
        internal static void OpenFile()
        {
            FileInfo OutputFileInfo;

            // Check if file already exsists
            if (File.Exists(OutputPath) == false)
            {
                OutputFileInfo = new FileInfo(OutputPath);
                OutputStream = OutputFileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
            }
            else
            {
                OutputFileInfo = new FileInfo(OutputPath);

                if (Initialized == false && AppendFromLastInstance == false)
                {
                    OutputStream = OutputFileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
                }
                else
                {
                    OutputStream = OutputFileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
                }
            }
        }

        #endregion Internal Methods

        #region Public Classes

        /// <summary>
        /// Abstract Exception parent and sub classes
        /// </summary>
        public abstract class ExceptionBaseClass
        {
            #region Internal Methods

            internal abstract string GetString(Exception Error);

            #endregion Internal Methods
        }

        public class HashCode : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                try
                {
                    return Error.GetHashCode().ToString();
                }
                catch
                {
                    return "HashCode was not found in exception object";
                }
            }

            #endregion Internal Methods
        }

        public class Data : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                try
                {
                    return Error.Data.ToString();
                }
                catch
                {
                    return "Data was not found in exception object";
                }
            }

            #endregion Internal Methods
        }

        public class HelpLink : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                try
                {
                    return Error.HelpLink.ToString();
                }
                catch
                {
                    return "HelpLink was not found in exception object";
                }
            }

            #endregion Internal Methods
        }

        public class HResult : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                try
                {
                    return Error.HResult.ToString();
                }
                catch
                {
                    return "HResult was not found in exception object";
                }
            }

            #endregion Internal Methods
        }

        public class InnerException : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                try
                {
                    return Error.InnerException.ToString();
                }
                catch
                {
                    return "InnerException was not found in exception object";
                }
            }

            #endregion Internal Methods
        }

        public class Message : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                try
                {
                    return Error.Message.ToString();
                }
                catch
                {
                    return "Message was not found in exception object";
                }
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
                try
                {
                    return Error.Source.ToString();
                }
                catch
                {
                    return "Source was not found in exception object";
                }
            }

            #endregion Internal Methods
        }

        public class StackTrace : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                try
                {
                    return Error.StackTrace.ToString();
                }
                catch
                {
                    return "StackTrace was not found in exception object";
                }
            }

            #endregion Internal Methods
        }

        /// <summary>
        /// Abstract string parent and sub-classes
        /// </summary>
        public abstract class StringBaseClass
        {
            #region Internal Methods

            internal abstract string GetString();

            #endregion Internal Methods
        }

        public class TargetSite : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                try
                {
                    return Error.TargetSite.ToString();
                }
                catch
                {
                    return "TargetSite was not found in exception object";
                }
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
                try
                {
                    return DateTime.Now.ToString(DateFormat);
                }
                catch
                {
                    return DateTime.Now.ToString("MM:DD:HH:mm");
                }
            }

            #endregion Internal Methods
        }

        #endregion Public Classes

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

        #endregion Internal Classes
    }
}