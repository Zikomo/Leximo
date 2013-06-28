using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace Debugging
{
    public class DebugConsoleWriter : System.IO.TextWriter
    {
        public StringBuilder currentLine = new StringBuilder();

        public DebugConsoleWriter()
            : base()
        {
        }

        public DebugConsoleWriter(IFormatProvider provider)
            : base(provider)
        {
        }

        public override void Write(bool value)
        {
            currentLine.Append(value);
        }

        public override void Write(char[] buffer)
        {
            currentLine.Append(buffer);
        }

        public override void Write(char value)
        {
            currentLine.Append(value);
        }
        public override void Write(char[] buffer, int index, int count)
        {
            currentLine.Append(buffer, index, count);
        }

        public override void Write(decimal value)
        {
            currentLine.Append(value);
        }

        public override void Write(double value)
        {
            currentLine.Append(value);
        }

        public override void Write(float value)
        {
            currentLine.Append(value);
        }

        public override void Write(int value)
        {
            currentLine.Append(value);
        }

        public override void Write(long value)
        {
            currentLine.Append(value);
        }

        public override void Write(object value)
        {
            Write(value.ToString());
        }

        public override void Write(string format, object arg0)
        {
            string writerString = string.Format(format, arg0);
            Write(writerString);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            string writerString = string.Format(format, arg0, arg1);
            Write(writerString);
        }

        public override void Write(string format, params object[] arg)
        {
            string writerString = string.Format(format, arg);
            Write(writerString);
        }

        public override void Write(string value)
        {
            currentLine.Append(value);
        }

        public override void Write(uint value)
        {
            currentLine.Append(value);
        }

        public override void Write(ulong value)
        {
            currentLine.Append(value);
        }

        protected override void Dispose(bool disposing)
        {
            //add back the console
            base.Dispose(disposing);
        }

        public override Encoding Encoding
        {
            get { return ASCIIEncoding.ASCII; }
        }
    }
}
