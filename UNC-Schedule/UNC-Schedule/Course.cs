using System;
using System.Numerics;

namespace UNC_Schedule
{
    public class Course
    {
        //public string CRN { get { return CRN; } set { CRN = value; } }
        //public string Subject { get { return Subject; } set { Subject = value; } }
        //public string Crse { get { return Crse; } set { Crse = value; } }
        //public string Sec { get { return Sec; } set { Sec = value; } }
        //public string Cmp { get { return Cmp; } set { Cmp = value; } }
        //public string Cred { get { return Cred; } set { Cred = value; } }
        //public string Title { get { return Title; } set { Title = value; } }
        //public string Days { get { return Days; } set { Days = value; } }
        //public string Time { get { return Time; } set { Time = value; } }
        //public string SeatsRem { get { return SeatsRem; } set { SeatsRem = value; } }
        //public string WLCAP { get { return WLCAP; } set { WLCAP = value; } }
        //public string WLACT { get { return WLACT; } set { WLACT = value; } }
        //public string WLREM { get { return WLREM; } set { WLREM = value; } }
        //public string RsrvdRem { get { return RsrvdRem; } set { RsrvdRem = value; } }
        //public string Instructor { get { return Instructor; } set { Instructor = value; } }
        //public string Dates { get { return Dates; } set { Dates = value; } }
        //public string Session { get { return Session; } set { Session = value; } }
        //public string Location { get { return Location; } set { Location = value; } }
        //public string BitArray { get { return BitArray; } set { BitArray = value; } }

        private string CRN;
        private string Subject;
        private string Crse;
        private string Sec;
        private string Cmp;
        private string Cred;
        private string Title;
        private string Days;
        private string Time;
        private string SeatsRem;
        private string WLCAP;
        private string WLACT;
        private string WLREM;
        private string RsrvdRem;
        private string Instructor;
        private string Dates;
        private string Session;
        private string Location;
        private string BitArray;


            public Course(object CRN, object Subject, object Crse, object Sec, object Cmp, object Cred, object Title, object Days, object Time,
                            object SeatsRem, object WLCAP, object WLACT, object WLREM, object RsrvdRem, object Instructor, object Dates, object Session, object Location, object BitArray)
            {
                this.CRN = Convert.ToString(CRN);
                this.Subject = Convert.ToString(Subject);
                this.Crse = Convert.ToString(Crse);
                this.Sec = Convert.ToString(Sec);
                this.Cmp = Convert.ToString(Cmp);
                this.Cred = Convert.ToString(Cred);
                this.Title = Convert.ToString(Title);
                this.Days = Convert.ToString(Days);
                this.Time = Convert.ToString(Time);
                this.SeatsRem = Convert.ToString(SeatsRem);
                this.WLCAP = Convert.ToString(WLCAP);
                this.WLACT = Convert.ToString(WLACT);
                this.WLREM = Convert.ToString(WLREM);
                this.RsrvdRem = Convert.ToString(RsrvdRem);
                this.Instructor = Convert.ToString(Instructor);
                this.Dates = Convert.ToString(Dates);
                this.Session = Convert.ToString(Session);
                this.Location = Convert.ToString(Location);
                this.BitArray = Convert.ToString(BitArray);
                string test = "";
            }

        public string getSubject()
        {
            return Subject;
        }

        public string getCourseNum()
        {
            return Crse;
        }

        public String getBitArray()
        {
            return BitArray;
        }

        public String getCRN()
        {
            return CRN;
        }





        }
    }
