using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FlightTracker.Models
{
    public class FileHandler
    {
        private static FileHandler instance;
        private static readonly object padlock = new object();

        private string filename;

        private FileHandler(string name) { this.filename = name; }

        public static FileHandler Instance(string name)
        {
            if (instance == null)
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new FileHandler(name);
                    }
                }
            }
            else
            {
                if (instance.filename != name)
                {
                    lock (padlock)
                    {
                        if (instance.filename != name)
                        {
                            instance = new FileHandler(name);
                        }
                    }
                }
            }
            return instance;
        }

        /// <summary>
        /// Saves data to file.
        /// </summary>
        /// <param name="filename"> the name of the file. </param>
        /// <param name="data"> the data to be saved. Comes in format: Lon, Lat, Throttle, Rudder </param>
        public void saveToFile(List<float> data)
        {
            lock (padlock)
            {
                //Write data to file. Will append to the file if it exists, and create a new one if it doesn't
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(System.AppContext.BaseDirectory + filename, true))
                {
                    for (int i = 0; i < data.Count; ++i)
                    {
                        file.WriteLine(data.ElementAt(i) + "\n");
                    }

                    //All data was written, write a token.
                    file.WriteLine("$\n");
                }
            }
        }

        /// <summary>
        /// Read all file contents and return a list containing all seperate lines.
        /// </summary>
        /// <param name="filename"> the file to read from </param>
        /// <returns> a list containing all seperate lines. </returns>
        public List<string> readFromFile()
        {
            List<string> fileContent = null;
            lock (padlock)
            {
                try
                {
                    fileContent = new List<string>();
                    //Open the text file using a stream reader.
                    using (StreamReader sr = new StreamReader(System.AppContext.BaseDirectory + filename))
                    {
                        String line = null;
                        while ((line = sr.ReadLine()) != null)
                        {
                            line.Trim(); //To remove \n
                            if (line != "")
                            {
                                fileContent.Add(line);
                            }
                            
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                    return null;
                }
            }

            return fileContent;
        }
    }
}