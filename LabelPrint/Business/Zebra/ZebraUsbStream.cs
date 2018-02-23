//-------------------------------
// ZebraUsbStream.cs
//-------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Zebra.Printing;

namespace RfidPrinter
{
    ///


    /// Stream subclass which incorporates low-level USB access to Zebra printers
    ///

    public class ZebraUsbStream : Stream
    {
        UsbPrinterConnector usb;

        public ZebraUsbStream(string port)
        {
            usb = new UsbPrinterConnector(port);
            usb.IsConnected = true;
            base.ReadTimeout = usb.ReadTimeout;
            base.WriteTimeout = usb.WriteTimeout;
        }

        public ZebraUsbStream()
        {
            System.Collections.Specialized.NameValueCollection devs = UsbPrinterConnector.EnumDevices(true, true, false);

            if (devs.Count < 1)
                throw new Exception("No Zebra printers found");

            usb = new UsbPrinterConnector(devs[0].ToString());
            usb.IsConnected = true;
        }


        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override bool CanTimeout
        {
            get { return true; }
        }

        public override void Flush()
        {
            ;
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return usb.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            usb.Send(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            usb.IsConnected = false;
        }

        public override void Close()
        {
            base.Close();
            if (usb.IsConnected)
                usb.IsConnected = false;
        }

        public override int ReadTimeout
        {
            get
            {
                return usb.ReadTimeout;
            }
            set
            {
                usb.ReadTimeout = value;
            }
        }

        public override int WriteTimeout
        {
            get
            {
                return usb.WriteTimeout;
            }
            set
            {
                usb.WriteTimeout = value;
            }
        }
    }
}