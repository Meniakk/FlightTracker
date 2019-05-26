using FlightTracker.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace FlightTracker.Controllers
{
    public class displayController : Controller
    {


        [HttpPost]
        public string GetCord()
        {
            Console.Write("Reading data..");
            if (Models.DataWriterClient.Instance.isConnected)
            {
                StringBuilder sb = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                XmlWriter writer = XmlWriter.Create(sb, settings);

                writer.WriteStartDocument();

                writer.WriteStartElement("DATA");
                writer.WriteElementString("lat", Models.DataWriterClient.Instance.GetData("get /position/latitude-deg\r\n"));
                writer.WriteElementString("lng", Models.DataWriterClient.Instance.GetData("get /position/longitude-deg\r\n"));
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();

                return sb.ToString();
            }
           return null;
        }


        // GET: display
        [HttpGet]
        public ActionResult Indexx(string ip, int port, int time)
        {

            #region Connect to Server

            DataReaderServer server = DataReaderServer.Instance;
            bool isServerConnected = server.isRunning;

            try
            {
                string FlightServerIP = ip;
                int flightInfoPort = port;

                // If already connected to this client, don't do anything.
                if (!server.isConnectedToEndPoint(FlightServerIP, flightInfoPort))
                {

                    // If server connected, stop it.
                    if (isServerConnected)
                    {
                        server.stopServer = true;
                        // Wait for server to stop.
                        while (server.isRunning)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }

                    Thread serverThread = new Thread(new ParameterizedThreadStart(server.StartServer));
                    serverThread.IsBackground = true;
                    //serverThread.Start(new Tuple<string, int>(FlightServerIP, flightInfoPort));
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            #endregion

            Console.Write("Reading data..");
            Session["time"] = time;
            ViewBag["lastX"] = null;
            ViewBag["lastY"] = null;
            return View();
        }

        [HttpGet]
        public ActionResult Index(string ip, int port, int time)
        {

            #region Connect to Client

            DataWriterClient client = DataWriterClient.Instance;
            bool isClientConnected = client.isConnected;

            if (!client.isConnectedToEndPoint(ip, port))
            {
                // If client connected, stop it.
                if (isClientConnected)
                {
                    client.CloseConnection();
                }
                // Start client.
                client.StartClient(ip, port);
            }
            // Play Beep when connections are available.
            System.Media.SystemSounds.Beep.Play();
            #endregion
            //GetCord();
            Session["time"] = time;
            return View();
        }
    }
}