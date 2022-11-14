using System;
using System.Globalization;
using System.IO;

namespace WindowsFormsApp1
{
    internal class CsvWriter : IDisposable
    {
        private StreamWriter writer;
        private CultureInfo invariantCulture;

        public CsvWriter(StreamWriter writer, CultureInfo invariantCulture)
        {
            this.writer = writer;
            this.invariantCulture = invariantCulture;

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        internal void WriteRecords(string text)
        {
            throw new NotImplementedException();
        }
    }
}