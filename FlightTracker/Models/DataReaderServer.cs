using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace FlightTracker.Models
{
    class DataReaderServer
    {
        private static DataReaderServer instance;
        public bool stopServer { get; set; }
        public bool isRunning { get; private set; }
        public TcpListener current_TcpLisner { get; private set; }
        private IPEndPoint clientEndPoint;
        private static readonly object padlock = new object();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private DataReaderServer()
        {
            stopServer = false;
            isRunning = false;
        }

        /// <summary>
        /// Returns an instance of the class ListenerServer.
        /// </summary>
        public static DataReaderServer Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DataReaderServer();
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// Starting the server.
        /// Check if server isRunning before calling, if it is - stop him before via stopServer.
        /// </summary>
        public void StartServer(object parameters)
        {
            Tuple<string, int> args = (Tuple<string, int>)parameters;
            string ip = args.Item1;
            int port = args.Item2;
            string data = null;
            byte[] buffer = new byte[2048];
            // Try to stablish the remote endpoint for the socket.
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            TcpListener tcpListener = new TcpListener(iPEndPoint);
            // If server is already running 
            if (isRunning)
            {
                // If connected to a diffrent EndPoint, failure.
                throw new InvalidOperationException("Called StartServer when a diffrent server is already running.\n");
            }
            // Create a TCP/IP  socket.
            tcpListener.Start();
            Socket listener = tcpListener.AcceptSocket();
            listener.ReceiveTimeout = 1000;
            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                isRunning = true;
                data = null;
                current_TcpLisner = tcpListener;
                clientEndPoint = iPEndPoint;
                int bytesRec = 0;
                // Start listening for connections.  
                while (!stopServer)
                {
                    // Try for one second to read data from client, if no data received - skip iteration.
                    try
                    {
                        /* Save our read data in a string. */
                        bytesRec = listener.Receive(buffer);
                    }
                    catch (SocketException e)
                    {
                        continue;
                    }
                    data += Encoding.ASCII.GetString(buffer, 0, bytesRec);
                    /* Check if this is a full set of data. */
                    if (data.Contains('\n'))
                    {
                        /* Get the set of data. */
                        string till_new_line = data.Substring(0, data.IndexOf('\n'));
                        data = data.Substring(data.IndexOf('\n'));
                        //Console.WriteLine("DATA: {0}", till_new_line);
                        /* Making sure there are no "leftovers" from data. */
                        while (!string.IsNullOrEmpty(data) && data[0] == '\n')
                        {
                            data = data.Substring(1);
                        }
                        /* Update map If we got new data. */
                        if (!string.IsNullOrEmpty(till_new_line))
                        {
                            UpdateMap(till_new_line);
                        }
                        till_new_line = string.Empty;
                    }
                }
                closeServer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return;
        }

        /// <summary>
        /// Update the map with the client's data.
        /// </summary>
        /// <param name="str">A string sent by the client.</param>
        private void UpdateMap(string str)
        {
            // Get symbol table instance.
            SymbolTable table = SymbolTable.Instance;
            // Split the string to values
            string[] split = str.Split(',');
            double[] values = new double[25];
            for (int i = 0; i < split.Length; i++)
            {
                values[i] = Convert.ToDouble(split[i]);
            }
            /* Update the map. */
            table.setValueOf("/position/longitude-deg", values[0]);
            table.setValueOf("/position/latitude-deg", values[1]);
            table.setValueOf("/instrumentation/airspeed-indicator/indicated-speed-kt", values[2]);
            table.setValueOf("/instrumentation/altimeter/indicated-altitude-ft", values[3]);
            table.setValueOf("/instrumentation/altimeter/pressure-alt-ft", values[4]);
            table.setValueOf("/instrumentation/attitude-indicator/indicated-pitch-deg", values[5]);
            table.setValueOf("/instrumentation/attitude-indicator/indicated-roll-deg", values[6]);
            table.setValueOf("/instrumentation/attitude-indicator/internal-pitch-deg", values[7]);
            table.setValueOf("/instrumentation/attitude-indicator/internal-roll-deg", values[8]);
            table.setValueOf("/instrumentation/encoder/indicated-altitude-ft", values[9]);
            table.setValueOf("/instrumentation/encoder/pressure-alt-ft", values[10]);
            table.setValueOf("/instrumentation/gps/indicated-altitude-ft", values[11]);
            table.setValueOf("/instrumentation/gps/indicated-ground-speed-kt", values[12]);
            table.setValueOf("/instrumentation/gps/indicated-vertical-speed", values[13]);
            table.setValueOf("/instrumentation/heading-indicator/indicated-heading-deg", values[14]);
            table.setValueOf("/instrumentation/magnetic-compass/indicated-heading-deg", values[15]);
            table.setValueOf("/instrumentation/slip-skid-ball/indicated-slip-skid", values[16]);
            table.setValueOf("/instrumentation/turn-indicator/indicated-turn-rate", values[17]);
            table.setValueOf("/instrumentation/vertical-speed-indicator/indicated-speed-fpm", values[18]);
            table.setValueOf("/controls/flight/aileron", values[19]);
            table.setValueOf("/controls/flight/elevator", values[20]);
            table.setValueOf("/controls/flight/rudder", values[21]);
            table.setValueOf("/controls/flight/flaps", values[22]);
            table.setValueOf("/controls/engines/current-engine/throttle", values[23]);
            table.setValueOf("/engines/engine/rpm", values[24]);
        }

        /// <summary>
        /// Closes the connection to the client.
        /// </summary>
        public void closeServer()
        {
            if (isRunning && current_TcpLisner != null)
            {
                current_TcpLisner.Stop();
                current_TcpLisner = null;
                clientEndPoint = null;
                isRunning = false;
                stopServer = false;
            }
        }

        public bool isConnectedToEndPoint(string ip, int port)
        {
            if (clientEndPoint == null)
            {
                return false;
            }
            IPEndPoint tempEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            return clientEndPoint.Equals(tempEndPoint);
        }
    }
}