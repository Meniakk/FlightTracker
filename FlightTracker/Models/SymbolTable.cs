using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
namespace FlightTracker.Models
{
    /// <summary>
    /// A singlton class that holds our dictionary, updated by DataWriterServer.
    /// </summary>
    public class SymbolTable
    {
        public delegate void mapUpdated(string key, double value);
        public event mapUpdated mapUpdatedEvent;
        private static SymbolTable instance;
        private static readonly object padlock = new object();
        private IDictionary<String, double> symbol_table;

        /// <summary>
        /// Private contructor.
        /// </summary>
        private SymbolTable()
        {
            symbol_table = new Dictionary<string, double>();
            symbol_table.Add("/position/longitude-deg", 0.0);
            symbol_table.Add("/position/latitude-deg", 0.0);
            symbol_table.Add("/instrumentation/airspeed-indicator/indicated-speed-kt", 0.0);
            symbol_table.Add("/instrumentation/altimeter/indicated-altitude-ft", 0.0);
            symbol_table.Add("/instrumentation/altimeter/pressure-alt-ft", 0.0);
            symbol_table.Add("/instrumentation/attitude-indicator/indicated-pitch-deg", 0.0);
            symbol_table.Add("/instrumentation/attitude-indicator/indicated-roll-deg", 0.0);
            symbol_table.Add("/instrumentation/attitude-indicator/internal-pitch-deg", 0.0);
            symbol_table.Add("/instrumentation/attitude-indicator/internal-roll-deg", 0.0);
            symbol_table.Add("/instrumentation/encoder/indicated-altitude-ft", 0.0);
            symbol_table.Add("/instrumentation/encoder/pressure-alt-ft", 0.0);
            symbol_table.Add("/instrumentation/gps/indicated-altitude-ft", 0.0);
            symbol_table.Add("/instrumentation/gps/indicated-ground-speed-kt", 0.0);
            symbol_table.Add("/instrumentation/gps/indicated-vertical-speed", 0.0);
            symbol_table.Add("/instrumentation/heading-indicator/indicated-heading-deg", 0.0);
            symbol_table.Add("/instrumentation/magnetic-compass/indicated-heading-deg", 0.0);
            symbol_table.Add("/instrumentation/slip-skid-ball/indicated-slip-skid", 0.0);
            symbol_table.Add("/instrumentation/turn-indicator/indicated-turn-rate", 0.0);
            symbol_table.Add("/instrumentation/vertical-speed-indicator/indicated-speed-fpm", 0.0);
            symbol_table.Add("/controls/flight/aileron", 0.0);
            symbol_table.Add("/controls/flight/elevator", 0.0);
            symbol_table.Add("/controls/flight/rudder", 0.0);
            symbol_table.Add("/controls/flight/flaps", 0.0);
            symbol_table.Add("/controls/engines/current-engine/throttle", 0.0);
            symbol_table.Add("/engines/engine/rpm", 0.0);
        }

        /// <summary>
        /// Returns a value for a specidied key, if not found throw ArgumentException.
        /// </summary>
        /// <param name="key">The key to sample.</param>
        /// <returns></returns>
        public double getValueOf(string key)
        {
            lock (padlock)
            {
                double value;
                if (symbol_table.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    throw new ArgumentException("Could not find key \"{0}\" in dictionary.\n", key);
                }
            }
        }

        /// <summary>
        /// Sets a value for a specidied key, if not found throw ArgumentException.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The new value.</param>
        public void setValueOf(string key, double value)
        {
            lock (padlock)
            {
                double temp;
                if (symbol_table.TryGetValue(key, out temp))
                {
                    symbol_table[key] = value;
                    mapUpdatedEvent?.Invoke(key, value);
                }
                else
                {
                    throw new ArgumentException("Could not find key \"{0}\" in dictionary.\n", key);
                }
            }
        }

        /// <summary>
        /// Returns an instance of the class SymbolTable.
        /// </summary>
        public static SymbolTable Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SymbolTable();
                }
                return instance;
            }
        }
    }
}