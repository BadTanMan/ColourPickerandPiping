using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;


namespace ColourPickerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "colourpipe", PipeDirection.In))
            {
                // attempt pipe connection
                Console.Write("Waiting for pipe connection...");
                pipeClient.Connect();
                Console.WriteLine("Connected to pipe!");
                Console.WriteLine("{0} pipe server instances open.", pipeClient.NumberOfServerInstances);
                Console.WriteLine("Waiting for color picker data...");

                using (StreamReader sr = new StreamReader(pipeClient))
                {
                    // Display the text to the console
                    string temp;
                    while ((temp = sr.ReadLine()) != null)
                    {
                        Console.WriteLine("Received from server: {0}", temp);
                    }
                }
            }
            Console.Write("Press any key to exit...");
            Console.ReadKey();        
        }
    }
}
