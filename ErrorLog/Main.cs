using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        public static object ProcessorObject { get; set; }

        #endregion Public Properties

        #region Internal Fields

        // Used to check if we have a IO stream setup yet
        internal static bool Initialized = false;

        // The Internal filestream (Gets initialized in LogError())
        internal static FileStream OutputStream = null;

        // Backup format incase they dont specify one
        internal static object[] PresetFormat = new object[] { new Time { DateFormat = "DD:HH:mm" }, new Message(), new TargetSite(), new Source() };

        #endregion Internal Fields

        #region Public Methods

        /// <summary>
        /// Main Function, Takes error -&gt; Prepares it for the format processor -&gt; String gets
        /// processed by class -&gt; Writes string.
        /// </summary>
        /// <param name="Error">The exception object</param>
        public static void LogError(Exception Error)
        {
            // Why would you even?
            if (Error == null) throw new InvalidObjectException("Exception object equals null");

            // Initialise the Output File beforehand
            if (Initialized == false)
            {
                OpenFile();
                Initialized = true;
            }

            // Check class isnt empty before running
            object[] ChosenFormat = ErrorFormat ?? PresetFormat;

            // The raw unprocessed string array
            string[] RawString = new string[ChosenFormat.Length];

            //Prepare string for processing
            for (int i = 0; i != ChosenFormat.Length; i++)
            {
                if (ChosenFormat[i] is ExceptionBaseClass)
                {
                    RawString[i] = ((ExceptionBaseClass)ChosenFormat[i]).GetString(Error);
                }
                else if (ChosenFormat[i] is StringBaseClass)
                {
                    RawString[i] = ((StringBaseClass)ChosenFormat[i]).GetString();
                }
                else
                {
                    throw new InvalidObjectException(ChosenFormat[i].GetType().Name);
                }
            }

            // Check if the String Process pipeline is null before using presets
            object ChosenProcessorObject = ProcessorObject ?? new StringProcessor();

            // Check its a subtype before messin with it
            if (ChosenProcessorObject is StringFormatPipeline)
            {
                // Revert to presets if function is null, else use their code
                string ProcessedString = ((StringFormatPipeline)ChosenProcessorObject).ProcessString(RawString);

                // Write the Processed string
                byte[] Data = new UTF8Encoding(EncoderShouldEmitUTF8Identifier).GetBytes(ProcessedString);
                OutputStream.Write(Data, 0, Data.Length);
                OutputStream.Flush();
            }
            else
            {
                throw new InvalidObjectException(String.Format("{0} is not a child type of StringFormatPipeline(), Only child classes can be used in string processing.", ChosenProcessorObject.GetType().Name));
            }
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

            public abstract string ProcessString(string[] UnprocessedString);

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
                    ErrorMessage.Append(RawError[I]);

                    if (I != RawError.Length)
                    {
                        ErrorMessage.Append(", ");
                    }
                }
                ErrorMessage.Append(Environment.NewLine);

                return ErrorMessage.ToString();
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
                return Error.Data.ToString() ?? "Data was not found in exception object";
            }

            #endregion Internal Methods
        }

        public class HashCode : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.GetHashCode().ToString() ?? "HashCode was not found in exception object";
            }

            #endregion Internal Methods
        }

        public class HelpLink : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.HelpLink.ToString() ?? "HelpLink was not found in exception object";
            }

            #endregion Internal Methods
        }

        public class HResult : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.HResult.ToString() ?? "HResult was not found in exception object";
            }

            #endregion Internal Methods
        }

        public class InnerException : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.InnerException.ToString() ?? "InnerException was not found in exception object";
            }

            #endregion Internal Methods
        }

        public class Message : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.Message.ToString() ?? "Message was not found in exception object";
            }

            #endregion Internal Methods
        }

        public class Source : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.Source.ToString() ?? "Source was not found in exception object";
            }

            #endregion Internal Methods
        }

        public class StackTrace : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.StackTrace.ToString() ?? "StackTrace was not found in exception object";
            }

            #endregion Internal Methods
        }

        public class TargetSite : ExceptionBaseClass
        {
            #region Internal Methods

            internal override String GetString(Exception Error)
            {
                return Error.TargetSite.ToString() ?? "TargetSite was not found in exception object";
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
                return DateTime.Now.ToString(DateFormat) ?? DateTime.Now.ToString("dd:HH:mm:ss");
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

        #endregion Public Classes

        #region Internal Classes

        /// <summary>
        /// Error stuff cause you know somones gonna try to break this
        /// </summary>
        [Serializable]
        internal class InvalidObjectException : Exception
        {
            #region Public Constructors

            public InvalidObjectException(string Message)
            : base(Message) { }

            #endregion Public Constructors
        }

        #endregion Internal Classes
    }
}