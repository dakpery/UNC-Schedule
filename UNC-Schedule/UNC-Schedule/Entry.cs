using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UNC_Schedule
{
    public class Entry
    {

        private string key;
        private ArrayList array;


        public Entry(string key, Course newCourse)
        {
            this.key = key;
            array.Add(newCourse);
            
        }
    }
}