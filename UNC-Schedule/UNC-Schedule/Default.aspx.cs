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

                String a = testFunction("CSC133", "CSC131", "CSC231");
                MessageBox.Show("The schedules are = " + a);


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
            }
            else

            {
                MessageBox.Show("None");
            }



            String a = testFunction("CSC450", "CSC455", "CSC315");

        }

        private string testFunction(string courseStr1, string courseStr2, string courseStr3)
        {

            BitArray course1 = null; 
            BitArray course2 = null;
            BitArray course3 = null;
            String concatStr = "";

            
            ArrayList test = new ArrayList();
            ArrayList test2 = new ArrayList();
            ArrayList test3 = new ArrayList();
            mappy.TryGetValue(courseStr1, out test);  //CSC450
            mappy.TryGetValue(courseStr2, out test2); //CSC455
            mappy.TryGetValue(courseStr3, out test3); //CSC315

            for (int i = 0; i < test.Count; i++)
            {
                Course tempCourse = (Course)test[i];
                String courseBits1 = tempCourse.getBitArray();
                course1 = new BitArray(courseBits1.Select(c => c == '1').ToArray());

                int j = 0;
                int k = 0;


                for (j = 0; j < test2.Count; j++)
                {
                    Course tempCourse2 = (Course)test2[j];
                    String courseBits2 = tempCourse2.getBitArray();
                    course2 = new BitArray(courseBits2.Select(c => c == '1').ToArray());

                    BitArray compareBits = course1.And(course2);
                    bool testBool = iterateBitArray(compareBits);

                    if (testBool)
                    {
                        //MessageBox.Show("Conflict between: " + tempCourse.getCRN() + "," + tempCourse2.getCRN());
                        continue;
                    }
                    else 
                    {
                        for (k = 0; k < test3.Count; k++)
                        {
                            Course tempCourse3 = (Course)test3[k];
                            String courseBits3 = tempCourse3.getBitArray();
                            course3 = new BitArray(courseBits3.Select(c => c == '1').ToArray());

                            BitArray compareBits2 = course2.And(course3);
                            BitArray compareBits3 = course3.And(course1);
                            
                            testBool = iterateBitArray(compareBits2);
                            bool testBool2 = iterateBitArray(compareBits3);


                            if (testBool || testBool2)
                            {
                                //MessageBox.Show("Conflict between: " + tempCourse.getCRN() + "," + tempCourse2.getCRN() + "," + tempCourse3.getCRN());
                                continue;
                            }
                            else
                            {
                                concatStr += tempCourse.getCRN() + "," + tempCourse2.getCRN() + "," + tempCourse3.getCRN() + ";";
                            }
                        }
                    }
                }
            }
            return concatStr;

        }

        public bool iterateBitArray(BitArray bits)
        {
            for(int i = 0; i < bits.Count; i++)
            {
                if (bits[i]){
                    return true;
                }
            }
            return false;
        }





    }
}