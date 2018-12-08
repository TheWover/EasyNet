/* Description: Provides a simple API for using the EasyNet packing algorithm.
 * 
 * Author: The Wover
 * 
 * Referemces:
 * GZip .NET API: https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream
 * AES .NET API: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesmanaged
 * Base64 .NET API: https://docs.microsoft.com/en-us/dotnet/api/system.convert.tobase64string
 */

using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace EasyNetLibrary
{
    /// <summary>
    /// Provides a simple API for packing and unpacking arbitrary data using the EasyNet packing algorithm.
    /// </summary>
    /// 
    /// <synopsis>
    /// EasyNet packs (and unpacks) arbitrary data using the GZip -> AES-256 -> Base64 algorithms.
    /// All APIs used are native to the .NET Framework, and have been for a long time.
    /// The AES key and IV are randomly generated at runtime and provided in the result.
    /// </synopsis>
    /// 
    /// <example>
    /// EasyNetResult packed = EasyNet.Pack(bytes); //Pack an array of bytes
    /// 
    /// string result = packed.blob; //Packed data
    /// string key = packed.key; //Randomly generated AES key represented as base64 string
    /// string iv = packed.iv; //Randomly generated AES IV represented as base64 string
    /// 
    /// byte[] unpacked = EasyNet.Unpack(packed); //Unpack previously packed data.
    /// </example>
    public static class EasyNetPacker
    {

        /// <summary>
        /// Packs an arbitrary byte array using the EasyNet Algorithm.
        /// </summary>
        /// <param name="bytes">Input bytes to pack.</param>
        /// <returns></returns>
        public static EasyNetResult Pack(byte[] bytes)
        {

            //Create a disposable stream for holding the input bytes
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (MemoryStream compressedMemoryStream = new MemoryStream())
                {
                    //Create a disposable compression stream
                    using (GZipStream compressionStream = new GZipStream(compressedMemoryStream, CompressionMode.Compress))
                    {
                        //Compress the data
                        stream.CopyTo(compressionStream);
                        //Make sure to close the stream so that the data gets properly flushed into the output buffer
                        compressionStream.Close();

                        //Create a disposable AES class
                        using (AesManaged aesAlg = new AesManaged())
                        {
                            // Create an encryptor to perform the stream transform.
                            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                            // Create the streams used for encryption.
                            using (MemoryStream msEncrypt = new MemoryStream())
                            {
                                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                                {
                                    //Encrypt the daata stream
                                    csEncrypt.Write(compressedMemoryStream.ToArray(), 0, compressedMemoryStream.ToArray().Length);
                                    csEncrypt.FlushFinalBlock();

                                    //Create a results struct and return it
                                    return new EasyNetResult(Convert.ToBase64String(msEncrypt.ToArray()), //Converts the result to a base64 string
                                        Convert.ToBase64String(aesAlg.Key), //The base64 encoded AES key.
                                        Convert.ToBase64String(aesAlg.IV)); //The base64 encoded IV.
                                }
                            }
                        }
                    }
                }
            }
        }//end method

        /// <summary>
        /// Unpacks data previously packed using the EasyNet packing algorithm.
        /// </summary>
        /// <param name="packed">A struct containing previously packed data, as well as the base64-encoded AES key and IV necessary to unpack it.</param>
        /// <returns>The unpacked data as a byte array.</returns>
        public static byte[] Unpack(EasyNetResult packed)
        {
            // Validate the input
            if (packed.blob == null || packed.blob.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (packed.key == null || packed.key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (packed.iv == null || packed.iv.Length <= 0)
                throw new ArgumentNullException("IV");


            // Create an AesManaged object with the specified key and IV.
            using (AesManaged aesAlg = new AesManaged())
            {
                //Decode and use the AES key and IV
                aesAlg.Key = Convert.FromBase64String(packed.key);
                aesAlg.IV = Convert.FromBase64String(packed.iv);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(packed.blob)))
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


                                        // Read the decomoresses bytes from the decompressed stream
                                        return decompressedMemoryStream.ToArray();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }// end method
    }//end class


    /// <summary>
    /// Wraps the results of the EasyNet packing algorithm. Includes the packed blob, as well as everything necessary to unpack it.
    /// </summary>
    public struct EasyNetResult
    {
        public string blob; //EasyNet-packed data blob
        public string key; //Base64-encoded AES key
        public string iv; //Base64-encoded AES IV

        /// <param name="inputBlob">Packed blob produced as a result of the EasyNet packing algorithm.</param>
        /// <param name="inputKey">Base64-encoded AES-256 key.</param>
        /// <param name="inputIV">Base64-encoded AES IV.</param>
        public EasyNetResult(string inputBlob, string inputKey, string inputIV)
        {
            blob = inputBlob;
            key = inputKey;
            iv = inputIV;
        }//end constructor
    }//end struct
}//end namespace
