using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FlightTracker.Models
{
    public class FileHandler
    {
        private static readonly object padlock = new object();

        /// <summary>
        /// Saves data to file.
        /// </summary>
        /// <param name="filename"> the name of the file. </param>
        /// <param name="data"> the data to be saved. Comes in format: Lon, Lat, Throttle, Rudder </param>
        public void saveToFile(string filename, List<int> data)
        {
            lock(padlock)
            {
                //Write data to file. Will append to the file if it exists, and create a new one if it doesn't
                for (int i = 0; i < data.Count; ++i)
                {
                    using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filename + ".txt", true))
                    {
                        file.WriteLine(data.ElementAt(i) + "\n");

                        //If all data was written, write a token.
                        file.WriteLine("$\n");
                    }
                }
            }
        }

        /// <summary>
        /// Read all file contents and return a list containing all seperate lines.
        /// </summary>
        /// <param name="filename"> the file to read from </param>
        /// <returns> a list containing all seperate lines. </returns>
        public List<string> readFromFile(string filename)
        {
            List<string> fileContent = null;
            lock (padlock)
            {
                try
                {
                    fileContent = new List<string>();
                    //Open the text file using a stream reader.
                    using (StreamReader sr = new StreamReader(filename + ".txt"))
                    {
                        //Read the stream to a string, and write the string to the list.
                        String line = sr.ReadLine();
                        line.Trim(); //To remove \n
                        fileContent.Add(line);
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }

            return fileContent;
        }
    }
}