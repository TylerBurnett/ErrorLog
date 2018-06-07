//#define DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace TylerBurnett
{
    public class ErrorLog : IDisposable
    {
        #region Public Properties

        // Append to the errorlog from the last program instance?
        public static bool AppendFromLastInstance { get; set; }

        // The Arrangement of the error format
        public static object[] ErrorFormat { get; set; }

        // The errors output text file location
        public static string OutputPath { get; set; }

        // Incase you want to customise the format of the string written
        public static object StringFormat { get; set; }

        #endregion Public Properties

        #region Internal Fields

        // Used to check if we have a IO stream setup yet
        internal static bool Initialized = false;

        // The Internal filestream (Gets initialized in LogError())
        internal static FileStream OutputStream = null;

        // Backup format incase they dont specify one
        internal static object[] PresetFormat = new object[] { new Time { DateFormat = "MM:DD:HH:mm" }, new Message(), new TargetSite(), new Source() };

        #endregion Internal Fields

        #region Public Methods

        /// <summary>
        /// Main Function, Takes error -> Prepares it for the format processor -> String gets
        /// processed by class -> Writes string.
        /// </summary>
        /// <param name="Error">The exception object</param>
        public static void LogError(Exception Error)
        {
            object[] StringSequence;

            // Initialise the Output File beforehand
            if (Initialized == false)
            {
                OpenFile();
                Initialized = true;
            }

            // Check class isnt empty before running
            if (ErrorFormat == null || ErrorFormat.Length == 0)
            {
                StringSequence = PresetFormat;
            }
            else
            {
                StringSequence = ErrorFormat;
            }

            // The raw unprocessed string array
            string[] RawString = new string[StringSequence.Length];

            //Prepare string for processing
            for (int I = 0; I != StringSequence.Length; I++)
            {
                Type FormateType = StringSequence[I].GetType();

                if (FormateType.IsSubclassOf(typeof(ExceptionBaseClass)))
                {
                    RawString[I] = ((ExceptionBaseClass)StringSequence[I]).GetString(Error);
                }
                else if (FormateType.IsSubclassOf(typeof(StringBaseClass)))
                {
                    RawString[I] = ((StringBaseClass)StringSequence[I]).GetString();
                }
                else
                {
                    throw new InvalidObjectException(FormateType.Name);
                }
            }

            // The processed string
            String ProcessedString = null;

            // If the format is null, they obviously want the string format
            if (StringFormat == null)
            {
                ProcessedString = ((StringFormatPipeline)new StringProcessor()).ProcessString(RawString);
            }

            // Else if the class is a subclass of the ErrorFormatPipeline, They want a processed string
            else if (StringFormat.GetType().IsSubclassOf(typeof(StringFormatPipeline)))
            {
                ProcessedString = ((StringFormatPipeline)StringFormat).ProcessString(RawString);
            }

            // Else if the type isnt a subclass of ErrorFormatPipeline, They obviously doing the
            // wrong thing
            else if (!StringFormat.GetType().IsSubclassOf(typeof(StringFormatPipeline)))
            {
                throw new InvalidObjectException(StringFormat.GetType().Name);
            }

            // Finally write the data
            byte[] Data = new UTF8Encoding(true).GetBytes(ProcessedString);
            OutputStream.Write(Data, 0, Data.Length);
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
        /// Opens the IO stream safely for logging those errors, Also Creates file if it doesnt exsist.
        /// </summary>
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
        /// Abstract StringFormatPipeline parent and subclasses
        /// </summary>
        public abstract class StringFormatPipeline
        {
            #region Public Methods

            public abstract string ProcessString(string[] RawError);

            #endregion Public Methods
        }

        //Processes in the generice string format (Part of string process pipeline)
        public class StringProcessor : StringFormatPipeline
        {
            #region Public Methods

            public override string ProcessString(string[] RawError)
            {
                StringBuilder ErrorMessage = new StringBuilder();

                for (int I = 0; I != RawError.Length; I++)
                {
                    ErrorMessage.Append(RawError[I] + ", ");

                    if (I != RawError.Length)
                    {
                        ErrorMessage.Append(", ");
                    }
                }

                return ErrorMessage.ToString();
            }

            #endregion Public Methods
        }

        // Formats as a json (Part of string process pipeline)
        public class JsonProcessor : StringFormatPipeline
        {
            #region Public Methods

            public override string ProcessString(string[] RawError)
            {
                JsonClass JsonObject = new JsonClass();

                foreach (string S in RawError)
                {
                    JsonObject.ErrorData.Add(S);
                }

                JavaScriptSerializer Json = new JavaScriptSerializer();

                return Json.Serialize(JsonObject);
            }

            #endregion Public Methods
        }

        /// <summary>
        /// Abstract Exception parent and sub classes
        /// </summary>
        public abstract class ExceptionBaseClass
        {
            #region Internal Methods

            internal abstract string GetString(Exception Error);

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

        /// <summary>
        /// Abstract string parent and sub-classes
        /// </summary>
        public abstract class StringBaseClass
        {
            #region Internal Methods

            internal abstract string GetString();

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

        // Small class thats used for json serialization
        internal class JsonClass
        {
            #region Internal Properties

            internal List<string> ErrorData { get; set; }

            #endregion Internal Properties
        }

        #endregion Internal Classes
    }
}