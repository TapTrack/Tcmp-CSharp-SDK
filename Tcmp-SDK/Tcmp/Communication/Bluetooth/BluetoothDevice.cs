using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TapTrack.Tcmp.Communication.Bluetooth
{
    internal class BluetoothDevice
    {
        public BluetoothDevice()
        {

        }

        public BluetoothDevice(byte[] bluetoothAddress, byte bond, sbyte rssi, string name)
        {
            BluetoothAddress = bluetoothAddress;
            Bond = bond;
            Rssi = rssi;
            Name = name;
            ConnectionHandle = null;
        }

        public BluetoothDevice(byte[] bluetoothAddress, byte bond, sbyte rssi, string name, byte connectionHandle)
        {
            BluetoothAddress = bluetoothAddress;
            Bond = bond;
            Rssi = rssi;
            Name = name;
            ConnectionHandle = connectionHandle;
        }

        public byte[] BluetoothAddress { get; set; }

        public byte Bond { get; set; }

        public sbyte Rssi { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Used by the Bluegiga bluetooth dongle to sent data to a certain bluetooth device
        /// </summary>
        public byte? ConnectionHandle { get; set; }
    }
}
