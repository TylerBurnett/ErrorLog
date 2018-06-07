# ErrorLog
A lightweight library for handling and outputting application errors.

## Abilities
- Full customisability of ouput format 
- Append from last program instance or remove previous errror file
- Lightweight with average Writelog time of only 324 ticks (10,000,000 ticks in a second)
- Disposeability

## Usage
This library has a very quick setup with only the need of only 3 lines of code to initiate it. If not less.
If you choose to not set these variables, the library will automatically use presets.

```
ErrorLog.OutputPath = "ErrorFile.txt";
ErrorLog.SetFormat(new object[] { new ErrorLog.Time(), new ErrorLog.Message(), new ErrorLog.TargetSite()});
ErrorLog.AppendFromLastInstance = false;
```

When an error does happen all that is needed is a simple pass of the exception object and the rest will be handled
```
 try
  {
  //...
  }
 catch (Exception e)
  {
   ErrorLog.LogError(e);
   // Your handling code here...
  }
```

### Formatting the error strings
Errorlog wraps around the pre-exsisting data types found inside the Exception class

- ```ErrorLog.Data()```
- ```ErrorLog.Message()```
- ```ErrorLog.TargetSite()```
- ```ErrorLog.HelpLink()```
- ```ErrorLog.InnerException()```
- ```ErrorLog.Source()```
- ```ErrorLog.StackTrace()```
- ```ErrorLog.TargetSite()```
- ```ErrorLog.Hresult()```
- ```ErrorLog.HashCode()```

Aswell as this there is also the inclusion of two extra types which are:
- ```ErrorLog.NewLine()```
- ```ErrorLog.Time()```
Newline Creates a Newline between the next piece of information
The time object will take a normal string which represents the time format you want to use. Just like ```DateTime.ToStringFormat()```

## Using the string processing pipeline
This library gives full customisation of the final output string if desired through the use of a pipeline setup.
This string customisation is done after the arrangement of the error format and before the writing of the actual error.
Two preset classes are pre defined inside the Errorlog library if you do not wish to customise the output. By default the String processor is used if the variable is null.

### Sample code
```
    public class CustomStringProcessor : ErrorLog.StringFormatPipeline
    {
        public override string ProcessString(String[] UnprocessedString)
        {

            //
            //
            //.....

            return "The processed string" ;
        }
    }
    
    //...
    //
    
    ErrorLog.ProcessorObject = new CustomStringProcessor();
```


