# ExampleWrapper
Unpacks an EasyNet-packed .NET Assembly and runs it from memory using the Reflection API, passing in command-line arguments to the payload. Since the wrapper is, itself, a .NET Assembly, it can be run from existing execution vectors such as execute-assembly or DotNetToJScript.

ExampleWrapper may use a payload that is:
* Embedded as a literal string
* Embedded as a file resource in its .NET Assembly.
* Hosted at a URL.
* Provided at runtime as a command-line argument.

This wrapper is provided as a TEMPLATE ONLY. By default, it has a HelloWorld program embedded as a file resource. It loads that, using a hardcoded AES-256 key and IV. The intention is for you to customize it for your own use. Please feel free to do so. When you do, please do not provide attribution to me. ;-)

# Usage

First, use EasyNet.exe to pack some arbitrary data. Keep the key and IV that it provides for later use.

Now, we must decide how we will provide the packed data to the unpacker. There are three methods provided with this example:

1) Embed as strings
    * Simply copy the blob that EasyNet spit out and paste it into the embedded_blob section in the source code.
    * Copy the key provided by EasyNet and paste it as the value for the aes_key section in the source code.
    * Build the wrapper using the instructions below.
    * As a result, the payload may be run without interaction.
2) Command-line arguments
    * Both the key and the payload may be passed in as command-line argument using the syntax below.
3) Resource
    * In Visual Studios, ...
    * With csc.exe, the command-line arguments ... will embed the payload as a file resource
    * With msc.exe (Mono), the command-line arguments ... will embed the payload as a file resource
    * As a result, the payload may be run without interaction
4) Hosted on a web-server
    * Copy the packed .NET Assembly payload to the location you wish to host it from.
    * Start the webserver.
    * Modify the wrapper code so that the URL is passed in either as a command-line or is hardcoded as a string. Some sample code is commented-out in the source. It uses WebClient to retrieve the payload from a URL.
    * As a result, you can update your payload on the webserver and clients will always load the latest version.

Next, decide how you will provide the AES key. Just like the payload, it may be provided as an embedded string, a command-line argument, or a resource. Use the same methods described above to provide the AES key.

Finally, ExampleWrapper must be built before it may be used. Refer to the Building section for details.

Notes about using with Cobalt Strike's execute-assembly:
* execute-assembly has an undocumented maximum file size of 1 MB. If you embed the payload as a large resource or blob, then this can cause your command to fail.
* execute-assembly has an indefinitely-sized buffer for command-line arguments. If you run into issues with a file-size, then pass in the payload as a command-line argument. The GUI will look weird, but the command will still run.

# Building
ExampleWrapper may be built as either an EXE or DLL using Visual Studios, csc.exe, or msc.exe (the Mono compiler). It includes the standard Program.Main entry point, as well as an exported function (Wrapper.Exec). They both use the same syntax.

If you want to add your own logic to ExampleWrapper, you may embed your DLLs (in a way that resolves references) using ILMerge (https://www.microsoft.com/en-us/download/details.aspx?id=17630) or Costura (https://github.com/Fody/Costura).


# Recommendations

    * If you are only running this once or do not intend to embed your payloads, then remove the code for the other payload options. This reduces the size of the payload and complexity of the unpacker.
    * If you are going to run a payload through ExampleWrapper from disk on the target machine, then run ExampleWrapper through an obfuscator or such as Dotfuscator (https://www.preemptive.com/products/dotfuscator/overview) or CrpyStr (https://archive.codeplex.com/?p=cryptstr). This ensures that every single instance of ExampleWrapper will possess a unique signature.
    * If you wish ExampleWrapper to load its key and payload from files, that should be trivial to add. It is not included in this example because the goal is to demonstrate the possibilities of a generic packer like EasyNet, rather than provide a generic stager.
    * Some defenders are starting to recognize that the Reflection API may be used for evil. If you get caught because of this, then you should be able to integrate another loading mechanism. Again, the point of this wrapper is to show how to use EasyNet rather than how to load Assemblies.
    * Nothing about EasyNet or this wrapper prevents a resourceful malware analyst from reversing this wrapper or its payload. An analyst could easily reverse the EasyNet packing algorithm by inspecting the sequence of .NET API calls in ExampleWrapper through a tool like dnSpy. But, the process of recognizing the payload as unknown evil, obtaining it AND its key, and then sending it to an analyst for study takes time and resources that the defenders may not have.