# ErrorLog
A lightweight library for handling and outputting application errors.

## Abilities
- Full customisability of ouput format 
- Append from last program instance or remove previous errror file
- Lightweight with average Writelog time of only 6ms (Including IO fluctuations, test was a thousand itterations)
- Disposeability
- IO safe, various protections have been put in place to prevent IO ouput issues as they can be a large issue for such a simple task.

## Usage
This library has a very quick setup with only the need of only 3 lines of code to initiate it. If not less.
If you choose to not set these variables, the library will automatically revert to presets.

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

Aswell as this there is also the inclusion of two extra types which are:
- ```ErrorLog.NewLine()```
- ```ErrorLog.Time()```

The NewLine type is pretty self explanatory but the Time type not as much
The time object will take a normal string which represents the time format you want to use. Just like ```DateTime.ToStringFormat()```

