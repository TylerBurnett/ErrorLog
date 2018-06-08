# ErrorLog
Errorlog is a super flexible, optimized, customiseable library for: Arranging, Processing, and Writing Exception objects directly to IO. 

## Simplicity and customiseability combined
Errorlog uses a polymorphic class structure to enable devs to further extend and customise the arrangement and final output of the exception objects before they are written to IO.

At the same time Errorlog also offers little setup if that is also desired for the user.

## Quick start (Minimal setup)
This library has a very quick setup with only the need of only 3 lines of code to initiate it. If not less.
If you choose to not set these variables, the library will automatically use presets.

```
ErrorLog.OutputPath = "ErrorFile.txt";
ErrorLog.ErrorFormat = new object[] { new ErrorLog.Time(), new ErrorLog.Message(), new ErrorLog.TargetSite()};
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

### Exception arrangement
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

## Extending the library for custom needs
Since this library aim is to be lightweight and independant, certain aspects like json serialization and xml serialization have been
left for the user to implement. For example use with json.net for json serialization. Changing the final format of the data can be done easily as shown below.

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
    
    // set new class for string processing
    ErrorLog.ProcessorObject = new CustomStringProcessor();
```

This same class system can be used to add custom arrangement parts for exception arrangement too.

```
    public class CustomStringFormat : ErrorLog.StringBaseClass
    {
        public override string GetString()
        {

            return "Your stuff" ;
        }
    }
    
    public class CustomExceptionFormat : ErrorLog.ExceptionBaseClass
    {
        public override string GetString(Exception Error)
        {

            return "Your stuff" ;
        }
    }


    // set new class for string processing
    ErrorLog.ErrorFormat = new object[] { new ErrorLog.Time(), new ErrorLog.Message(), new ErrorLog.TargetSite(),
    new CustomStringFormat(), new CustomExceptionFormat()};
```


