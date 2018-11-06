using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace UNC_Schedule
{
    public partial class _Default : Page
    {
        public static Dictionary<string, ArrayList> mappy = new Dictionary<string, ArrayList>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Course tempCourse;
                // Create HashMap
                ArrayList tempList = new ArrayList();



                string queryStr = "Select * FROM course_table;";
                string ConnectionStr = "server=localhost; uid=root; pwd=Deepw00d; database=Courses";
                using (MySqlConnection connection = new MySqlConnection(ConnectionStr))
                {
                    MySqlCommand command = new MySqlCommand(queryStr, connection);
                    connection.Open();
                    try
                    {
                        MySqlDataReader reader = command.ExecuteReader();


                        while (reader.Read())
                        {


                            tempCourse = new Course(reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3), reader.GetValue(4),
                                               reader.GetValue(5), reader.GetValue(6), reader.GetValue(7), reader.GetValue(8), reader.GetValue(9),
                                               reader.GetValue(10), reader.GetValue(11), reader.GetValue(12), reader.GetValue(13), reader.GetValue(14),
                                               reader.GetValue(15), reader.GetValue(16), reader.GetValue(17), reader.GetValue(18));


                            string sub = tempCourse.getSubject();
                            string courseNum = tempCourse.getCourseNum();

                            // Determine if Hashmap has entry for key(Subject+CourseNum)
                            if (mappy.ContainsKey(sub + courseNum))
                            {
                                // If it does: get the value of arraylist. Append new course. put back into map. 
                                mappy.TryGetValue(sub + courseNum, out tempList);

                                tempList.Add(tempCourse);

                                mappy.Remove(sub + courseNum);

                                mappy.Add(sub + courseNum, tempList);



                            }
                            else
                            {
                                ArrayList listy = new ArrayList();
                                listy.Add(tempCourse);
                                mappy.Add(sub + courseNum, listy);
                            }
                        }

                    }
                    finally
                    {
                        // Always call Close when done reading.
                        connection.Close();
                    }

                }








            
            }

        }

        public void Button1_Click(object sender, EventArgs e)
        {
            ArrayList test = new ArrayList();
            mappy.TryGetValue("BIO246", out test);
            BitArray course1 = null;
            BitArray course2 = null;


            for (int i = 0; i < 1; i++)
            {
                Course tempCourse = (Course)test[i];
                String courseBits = tempCourse.getBitArray();
                String CRN1 = tempCourse.getCRN();
                course1 = new BitArray(courseBits.Select(c => c == '1').ToArray());

            }

            mappy.TryGetValue("GGY140", out test);

            for (int i = 0; i < 1; i++)
            {
                Course tempCourse = (Course)test[i];
                String courseBits = tempCourse.getBitArray();
                String CRN2 = tempCourse.getCRN();
                course2 = new BitArray(courseBits.Select(c => c == '1').ToArray());

            }

            BitArray testBits = course1.And(course2);
            if (testBits[0])
            {
                MessageBox.Show("Conflict");
            } else

            {
                MessageBox.Show("None");
            }
            

            


        }

    }
}