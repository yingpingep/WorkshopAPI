using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkshopAPI.Models
{
    public class MyDataType
    {
        public string imageuri { get; set; }
        public List<Rect> rects { get; set; }
        public List<string> emoes { get; set; }
        public List<Age> ages { get; set; }
    }

    public class Rect
    {
        public int x { get; set; }
        public int y { get; set; }
        public int len { get; set; }
    }

    public class Age
    {
        public Age(double age)
        {
            this.age = age;
        }
        public double age { get; set; }
    }
}