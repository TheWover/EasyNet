/* Description: A minimal packer that uses the EasyNet packing algorithm.
 * 
 * Author: The Wover
 * 
 * Referemces:
 * GZip .NET API: https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream
 * AES .NET API: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesmanaged
 * Base64 .NET API: https://docs.microsoft.com/en-us/dotnet/api/system.convert.tobase64string
 */

using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using EasyNetLibrary;

namespace EasyNet
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">Command-line arguments</param>
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "--pack")
                {
                    
                    //Check if we have the correct number of command-line arguments
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Error: Invalid number of command-line arguments.");

                        //Exit with error
                        Environment.Exit(1);
                    }
                    else
                    {
                        try
                        {
                            //Pack the file and get the results wrapped with the data necessary to unpack it.
                            EasyNetResult packed = EasyNetPacker.Pack(File.ReadAllBytes(args[1]));


                            //No output file specified
                            if (args.Length == 2)
                            {

                                //Write the packed data to stdout
                                Console.Write(packed.blob);
                            }
                            else
                            {

                                //Write the packed data to the output file.
                                File.WriteAllBytes(args[2], Encoding.ASCII.GetBytes(packed.blob));
                            }

                            //Formatting
                            Console.Error.WriteLine("");

                            //Provide the user with the AES variables
                            Console.Error.WriteLine("AES Key: {0}", packed.key);
                            Console.Error.WriteLine("AES IV: {0}", packed.iv);

                            
                        }
                        catch (CryptographicException except)
                        {
                            Console.WriteLine("Cryptographic Error: {0}", except.Message + "\n");

                            Environment.Exit(1);
                        }
                        catch (FormatException except)
                        {
                            Console.WriteLine("Format Error: {0}", except.Message + "\n");

                            Environment.Exit(1);
                        }
                        catch (FileNotFoundException except)
                        {
                            Console.WriteLine("File Error: {0}", except.Message + "\n");

                            Environment.Exit(1);
                        }
                    }
                }
                else if (args[0] == "--unpack")
                {
                    //Check if we have the correct number of command-line arguments
                    if (args.Length < 4)
                    {
                        Console.WriteLine("Error: Invalid number of command-line arguments.");

                        //Exit with error
                        Environment.Exit(1);
                    }
                    //Correct number of command-line arguments
                    else
                    {
                        //Wrap in a try block in case base64 conversion or unpacking fails.
                        try
                        {
                            //Create the struct that wraps the packed data with the data necessary to unpack it.
                            EasyNetResult packed = new EasyNetResult(Encoding.ASCII.GetString(File.ReadAllBytes(args[3])), args[1], args[2]);
                            
                            //No output file specified
                            if (args.Length == 4)
                            {
                                //Write the packed data to stdout
                                Console.Write(Encoding.ASCII.GetString(EasyNetPacker.Unpack(packed)));
                            }
                            else
                            {

                                //Unpack the data and write the result to the output file.
                                File.WriteAllBytes(args[4], EasyNetPacker.Unpack(packed));

                                Console.Error.WriteLine("\nUnpacked to " + args[4] + ".");
                            }
                        }
                        catch (CryptographicException except)
                        {
                            Console.WriteLine("Cryptographic Error: {0}", except.Message + "\n");

                            Environment.Exit(1);
                        }
                        catch (FormatException except)
                        {
                            Console.WriteLine("Format Error: {0}", except.Message + "\n");

                            Environment.Exit(1);
                        }
                        catch (FileNotFoundException except)
                        {
                            Console.WriteLine("File Error: {0}", except.Message + "\n");

                            Environment.Exit(1);
                        }
                    }
                }
                //The user asked for the usage
                else if (args[0] == "--help" || args[0] == "-h" || args[0] == "/?")
                {
                    //Print the usage
                    PrintUsage();

                    //Exit
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Error: Invalid number of command-line arguments.");

                    //Print the usage
                    PrintUsage();

                    //Exit
                    Environment.Exit(0);
                }
            }
            else
            {
                //Print the usage
                PrintUsage();

                //Exit
                Environment.Exit(0);
            }
        }//end method

        /// <summary>
        /// Prints the usage instructions for the program.
        /// </summary>
        private static void PrintUsage()
        {
            Console.WriteLine("Usage:\n");

            Console.WriteLine("Packing: EasyNet.exe --pack input_file output_file");
            Console.WriteLine("Unpacking: EasyNet.exe --unpack AES_Key AES_IV input_file output_file");
        }
    }//end method
}//end namespace
