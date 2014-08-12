using System;
using System.Collections.Generic;
using System.Text;

namespace PDTUtils.Models
{
    public class ScreenshotModel
    {
        public uint CurrentIndex { get; set; }
        public uint MaxIndex { get; set; }

        public ScreenshotModel()
        {
            CurrentIndex = 0;
            MaxIndex = 0;
        }
    }
}
