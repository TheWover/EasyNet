# EasyNet
Packs/unpacks arbitrary data using a simple Data -> Gzip -> AES -> Base64 algorithm. Generates a random AES-256 key and and IV and provides them to the user. Can be used to pack or unpack arbitrary data. Provided both as a program and a library.

There are several advantages of EasyNet:

1) It is simple and easy to use.
2) Every algorithm involved is native to the .NET API and has been for a long time, ensuring that it may be unpacked on the other side using libraries natively available on the target system.
3) Every time EasyNet is used, its output possesses a unique signature. This is because the AES key and IV are randomly generated at runtime.
4) Due to the compression of GZip, payloads packed with EasyNet will tend to have their size reduced.

Because the key must (probably) be sent along with the packed file to be unpacked on the other end, EasyNet does not provide a high degree of confidentiality. However, it is not intended to. Rather, it is intended to obfuscate arbitrary data to prevent it from being analyzed by automated defenses. Every time the same data is packed with EasyNet, the result possesses a unique signature. A human (or educated machine) could pick out the key from the stream and use it to unpack the accompanying data. However, that would require pre-existing knowledge of the implementation of EasyNet and the tool that uses it. That is unlikely, and is the chance that this tool bets on. In case EasyNet becomes understood by defenders, the source is provided so that the implementation may be easily modified.

One way to use this is with a non-packed wrapper. The wrapper is provided with a key and some packed data. At runtime, the wrapper unpacks the data using EasyNet and the provided key.

An example wrapper is provided by the ExampleWrapper folder. That example can unpack data passed in as a command-line argument, an embedded blob, or a file resource in the Assembly. Whichever way it is passed in, the example will unpack it, assume that it is a .NET Assembly, and then load it using Reflection. Command-line arguments may be passed in. If used with something like Cobalt Strike's execute-assembly, this results in a heavily obfuscated blob that may only be reversed if it is pulled out of memory in a short time-window at runtime along with the key, and then unpacked using knowledge of EasyNet for inspection. It is incredibly unlikely that this would ever occur, or that anyone will (soon) develop the capability to do such. At this point, the signaturable element would be the wrapper, which would be very easy to modify for signature avoidance.

EasyNet could also be used to protect exfiltrated data. Or, to bypass DLP (Data-Loss Prevention). Or, many other things. Really, it could be used for many purposes that I have not predicted or intended. That, to me, is the mark of a good tool. ;-)

Language: C# (.NET 3.5 compatible)

# Usage

Usage to pack: EasyNet.exe --pack input_file [output_file]

Usage to unpack: EasyNet.exe --unpack AES_Key AES_IV input_file [output_file]

![Alt text](https://github.com/TheWover/EasyNet/blob/master/img/usage.JPG?raw=true "General Usage")

The result of the packing and unpacking algorithms are written to stdout. All other output is written to stderr. As a result, you may safely use pipe redirection for output as demonstrated below.

![Alt text](https://github.com/TheWover/EasyNet/blob/master/img/packing.JPG?raw=true "Packing a File")

As you can see, the AES key and IV are randomly generated every time EasyNet is used. This produces a unique signature for every instance of the packed data.

![Alt text](https://github.com/TheWover/EasyNet/blob/master/img/unpacking.JPG?raw=true "Unpacking Data")

To unpack the data, simply provide the AES key and IV that were provided when the data was packed. Pipe redirection works in the same way as packing.

Examples:

* EasyNet.exe --pack testinput.txt testoutput.txt

Packs the contents of testinput.txt and writes the result to testoutput.txt.

* EasyNet.exe --pack testinput.txt

Packs the contents of testinput.txt and prints the result to the screen.

* EasyNet.exe --pack testinput.txt | clip

Packs the contents of testinput.txt and pipes the result into your clipboard so that it may be pasted elsewhere.

* EasyNet.exe --pack testinput.txt > testoutput.txt

Packs the contents of testinput.txt and pipes the result to a file.

* EasyNet.exe --unpack w4EoA1NWtzdT5zi4kjMCB7s5qA7Sf+qbQYSWQgOliho= c+pL/ttbs5AyRQOA0gyrFA== testoutput.txt

Unpacks the contents of testoutput.txt using the specified AES key and IV and prints the result.

* EasyNet.exe --unpack w4EoA1NWtzdT5zi4kjMCB7s5qA7Sf+qbQYSWQgOliho= c+pL/ttbs5AyRQOA0gyrFA== testoutput.txt testunpackedoutput.txt

Unpacks the contents of testoutput.txt using the specified AES key and IV and writes the result to testunpackedoutput.txt.

WARNING: When using pipe redirectiong for packing (as demonstrated above), avoid using Powershell. It automatically writes all output to the console as Unicode. However, C# reads the input file (for unpacking) as ASCII. As there is no clean way to detect the encoding of the packed input and I cannot (from within C#) force Powershell to output ASCII, I suggest that you avoid using Powershell for packing and instead use CMD.exe. It is friendlier.

A usage guide for the EasyNet Library and ExampleWrapper are provided in their subdirectories.

# ATT&CK Mapping

ATT&CK ID: T1045

Technique Name: Software Packing

ATT&CK Framework Link: https://attack.mitre.org/techniques/T1045/ 

# To-Do
* Overload the pack function to optionally take a pre-defined key.
* Add an environmentally keyed version that uses an enum that you OR togethor to get your options of what to check.
