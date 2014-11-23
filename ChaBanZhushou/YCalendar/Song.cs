using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCalendar
{
    public class Song
    {
        public string SongName { get; set; }

        public Uri Uri { get; set; }

        public override string ToString()
        {
            return SongName;
        }
    }
}
