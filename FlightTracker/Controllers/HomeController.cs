﻿using FlightTracker.Models;
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
    public class HomeController : Controller
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
                writer.WriteElementString("throttle", Models.DataWriterClient.Instance.GetData("get /controls/engines/current-engine/throttle\r\n"));
                writer.WriteElementString("rudder", Models.DataWriterClient.Instance.GetData("get /controls/flight/rudder\r\n"));
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();

                return sb.ToString();
            }
            return null;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Display(string ip, int port, int time)
        {
            //Check if the first parameter is a valid ip or a filename
            if (!DataWriterClient.Instance.ValidateIPv4(ip))
            {
                FileHandler fileHandler = new FileHandler();
                List<string> fileContent = fileHandler.readFromFile(ip);
                ViewBag.data = fileContent;
                Session["fileContent"] = 1;
                Session["time"] = port;
                Session["connected"] = 0;
                return View();
            }

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

            #endregion

            if (!DataWriterClient.Instance.isConnected)
            {
                Session["connected"] = 0;
            } else
            {
                Session["connected"] = 1;
            }

            Session["time"] = time;
            return View();
        }
        
        [HttpGet]
        public ActionResult Save(string ip, int port, int time, int period, string filename)
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

            #endregion
            if (!DataWriterClient.Instance.isConnected)
            {
                Session["connected"] = 0;
            }
            else
            {
                Session["connected"] = 1;
            }

            Session["time"] = time;
            Session["period"] = period;
            Session["filename"] = filename;
            return View();
        }

        [HttpPost]
        public void WriteToFile(string ip, int port, int time, int period, string filename)
        {
            //???
            int lat = port;
            int lon = time;
            int throttle = period;
            int rudder = int.Parse(filename);
            filename = ip;

            List<int> parameters = new List<int>();
            parameters.Add(lat);
            parameters.Add(lon);
            parameters.Add(throttle);
            parameters.Add(rudder);

            FileHandler fileHandler = new FileHandler();
            fileHandler.saveToFile(filename, parameters);
        }

        /*
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        */
    }
}