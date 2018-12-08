# EasyNet Library

Provides a simple API for using the EasyNet packing algorithm in C#.

Namespace: EasyNetLibrary

![Alt text](https://github.com/TheWover/EasyNet/blob/master/img/API.JPG?raw=true "Class Diagram")

Basic Usage:

```csharp

EasyNetResult packed = EasyNet.Pack(bytes); //Pack an array of bytes

string result = packed.blob; //Packed data
string key = packed.key; //Randomly generated AES key represented as base64 string
string iv = packed.iv; //Randomly generated AES IV represented as base64 string

byte[] unpacked = EasyNet.Unpack(packed); //Unpack previously packed data.

```

Unpacking data packed by another program using EasyNet:

```csharp

//Get the AES-256 Key and IV.
string key = "zFNCf6AvQI7NCQrx43x6mIVmYYdZ+O1n+8NL29+IhEo=";
string iv = "AxnDDpUzglq9HkkqbjNEnA==";

//Construct a struct to wrap the packed payload.
EasyNetResult packed;

//Save the AES variables.
packed.key = key;
packed.iv = iv;

//Get the packed blob from a file, resource, etc. Provide it as a string.
packed.blob = "4mU...w==";

byte[] unpacked = EasyNet.Unpack(packed); //Unpack the data.

```

Including the library:

To use EasyNet as a library, include it as a reference in your project.

If you want to use EasyNet without requiring the DLL to exist on-disk, you can use [ILMerge](https://github.com/dotnet/ILMerge) or [Costura](https://github.com/Fody/Costura) to merge the DLL into your executable.