# Example Payload

This example demonstrates how to use EasyNet for crafting payloads. The example in question is an MSBuild XML file containing C# code. The code unpacks and loads a .NET Assembly that was previously packed by EasyNet. The AES key, IV, and the packed Assembly are all hardcoded into the XML file. By using EasyNet, we can build an on-disk loader that does not need to stage from a server, while ensuring that we bypass any signature detection for our embedded payload. I originally created this for staging SILENTTRINITY, and have tested it.

I have provided two files:

* msbuild_example.xml - This is the MSBuild XML file that I refer to above.
* TestCode.cs - This contains the code from the XML file as a usable C# program so that you can test your code on its own before embedded it into the XML. It is also worth noting that this example demonstrates how to unpack EasyNet code WITHOUT using the EasyNet DLL or source code in your program.

Inside of the XML file, examples are provided for loading both DLLs and EXEs. once you have added in your own payload, you can run it using the command below: 

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe msbuild_example.xml
```

# ATT&CK Mapping

ATT&CK ID: T1127

Technique Name: Trusted Developer Utitlities

ATT&CK Framework Link: https://attack.mitre.org/techniques/T1127/


## OPSEC note:

Using C# for MSBuild payloads results in more anomalous behaviour than using JSCript. When the payload is run, a csc.exe process will start in the background that compiles the code to a temp executable and runs it. All techniques that leverage C# inside of XML work this way. The tradeoff is that, while using C# is noisier, it is more flexible than JScript and is not as thoroughly detected as popular tools such as DotNetToJScript.