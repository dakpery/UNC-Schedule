namespace UNC_Schedule
{
    using MySql.Data.MySqlClient;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web.UI;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="_Default" />
    /// </summary>
    public partial class _Default : Page


    {


 

        /// <summary>
        /// Defines the mappy
        /// </summary>
        public static Dictionary<string, ArrayList> mappy = new Dictionary<string, ArrayList>();

        /// <summary>
        /// Defines the debug
        /// Determines if schedules will be written to a text file
        /// </summary>
        public static Boolean debug = true;


        public static string strGlobal = "before";

        /// <summary>
        /// sw writes to a textfile on the current machines desktop
        /// Directory WILL need to change
        /// </summary>
        //public static StreamWriter sw = new StreamWriter(@"C:\Users\ctr20\Desktop\schedules.txt");


        [System.Web.Services.WebMethod]
        public static string backEndFunction()
        {
            return strGlobal;
        }



        /// <summary>
        /// The Page_Load
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Course tempCourse;
                // Create HashMap
                ArrayList tempList = new ArrayList();



                string queryStr = "Select * FROM course_table;";
                string ConnectionStr = "server=localhost; uid=root; pwd=12345; database=Courses";
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

        /// <summary>
        /// The Button1_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        public void Button1_Click(object sender, EventArgs e)
        {




            //"MAT161", "ENG101", "CSC331"
            //"CSC133", "CSC331", "CSC231", "ENG101" 
            //"CSC231", "CSC133", "CSC331", "CSC231", "ENG101" 
            //"CSC385", "CSC231", "CSC133", "CSC331", "CSC231", "ENG101" 
            ArrayList desiredCourses = new ArrayList() { "ACG201", "ENG101"};
            String a = testFunction(desiredCourses);
            //sw.Close();
        }

        /// <summary>
        /// The testFunction
        /// </summary>
        /// <param name="desiredCourses">The desiredCourses<see cref="ArrayList"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string testFunction(ArrayList desiredCourses)
        {

            ArrayList sectionsDesiredCourses = new ArrayList();

            foreach (string courseStr in desiredCourses)
            {
                ArrayList courseList = new ArrayList();
                mappy.TryGetValue(courseStr, out courseList);
                sectionsDesiredCourses.Add(courseList);
            }

            strGlobal = "Number of courses: " + sectionsDesiredCourses.Count;

            BitArray course1BitArray = null;
            BitArray course2BitArray = null;
            BitArray course3BitArray = null;
            BitArray course4BitArray = null;
            BitArray course5BitArray = null;
            BitArray course6BitArray = null;

            int counter = 0;
            string concatStr = "";
            int numberOfDesiredCourses = desiredCourses.Count;

            if (numberOfDesiredCourses >= 1)
            {
                ArrayList course1List = (ArrayList)sectionsDesiredCourses[0];
                for (int i = 0; i < course1List.Count; i++) //Grab first course
                {
                    Course tempCourse = (Course)course1List[i];
                    String courseBits1 = tempCourse.getBitArray();
                    course1BitArray = new BitArray(courseBits1.Select(c => c == '1').ToArray());

                    int j = 0;
                    int k = 0;
                    int m = 0;
                    int n = 0;
                    int h = 0;

                    if (desiredCourses.Count >= 2)
                    {
                        ArrayList course2List = (ArrayList)sectionsDesiredCourses[1];
                        for (j = 0; j < course2List.Count; j++) //Grab second course
                        {
                            Course tempCourse2 = (Course)course2List[j];
                            String courseBits2 = tempCourse2.getBitArray();
                            course2BitArray = new BitArray(courseBits2.Select(c => c == '1').ToArray());
                            bool testBool;

                            try
                            {
                                BitArray compareBits = course1BitArray.And(course2BitArray);
                                testBool = iterateBitArray(compareBits);
                            }
                            catch (Exception e)
                            {
                                testBool = false;
                            }


                            if (testBool)
                            {
                                //MessageBox.Show("Conflict between: " + tempCourse.getCRN() + "," + tempCourse2.getCRN());
                                continue;
                            }

                            if (desiredCourses.Count == 2 && debug) //for debugging purposes
                            {
                                counter++;
                                concatStr = tempCourse.getCRN() + "," + tempCourse2.getCRN() + ",";
                                //sw.WriteLine(counter + ": " + concatStr);
                            }

                            if (desiredCourses.Count >= 3)
                            {
                                ArrayList course3List = (ArrayList)sectionsDesiredCourses[2];
                                for (k = 0; k < course3List.Count; k++) //grab third course
                                {
                                    Course tempCourse3 = (Course)course3List[k];
                                    String courseBits3 = tempCourse3.getBitArray();
                                    course3BitArray = new BitArray(courseBits3.Select(c => c == '1').ToArray());
                                    Boolean testBool2;

                                    try
                                    {
                                        BitArray compareBits2 = course2BitArray.And(course3BitArray);
                                        testBool = iterateBitArray(compareBits2);


                                    }
                                    catch (Exception e)
                                    {
                                        testBool = false;
                                    }

                                    try
                                    {
                                        BitArray compareBits3 = course3BitArray.And(course1BitArray);
                                        testBool2 = iterateBitArray(compareBits3);
                                    }
                                    catch (Exception e)
                                    {
                                        testBool2 = false;
                                    }

                                    if (testBool || testBool2)
                                    {
                                        //MessageBox.Show("Conflict between: " + tempCourse.getCRN() + "," + tempCourse2.getCRN() + "," + tempCourse3.getCRN());
                                        continue;
                                    }

                                    if (desiredCourses.Count == 3 && debug) //for debugging purposes
                                    {
                                        counter++;
                                        concatStr = tempCourse.getCRN() + "," + tempCourse2.getCRN() +
                                            "," + tempCourse3.getCRN() + "," + ";";
                                        //sw.WriteLine(counter + ": " + concatStr);
                                    }

                                    if (desiredCourses.Count >= 4)
                                    {
                                        ArrayList course4List = (ArrayList)sectionsDesiredCourses[3];

                                        for (m = 0; m < course4List.Count; m++) //grab third course
                                        {
                                            Course tempCourse4 = (Course)course4List[m];
                                            String courseBits4 = tempCourse4.getBitArray();
                                            course4BitArray = new BitArray(courseBits4.Select(c => c == '1').ToArray());
                                            Boolean testBool3;

                                            try
                                            {
                                                BitArray compareBits2 = course1BitArray.And(course4BitArray);
                                                testBool = iterateBitArray(compareBits2);


                                            }
                                            catch (Exception e)
                                            {
                                                testBool = false;
                                            }

                                            try
                                            {
                                                BitArray compareBits3 = course2BitArray.And(course4BitArray);
                                                testBool2 = iterateBitArray(compareBits3);
                                            }
                                            catch (Exception e)
                                            {
                                                testBool2 = false;
                                            }

                                            try
                                            {
                                                BitArray compareBits3 = course3BitArray.And(course4BitArray);
                                                testBool3 = iterateBitArray(compareBits3);
                                            }
                                            catch (Exception e)
                                            {
                                                testBool3 = false;
                                            }

                                            if (testBool || testBool2 || testBool3)
                                            {
                                                //MessageBox.Show("Conflict between: " + tempCourse.getCRN() + "," + tempCourse2.getCRN() + "," + tempCourse3.getCRN());
                                                continue;
                                            }



                                            if (desiredCourses.Count == 4 && debug) //for debugging purposes
                                            {
                                                counter++;
                                                concatStr = tempCourse.getCRN() + "," + tempCourse2.getCRN() +
                                                    "," + tempCourse3.getCRN() + "," + tempCourse4.getCRN() + ";";
                                                //sw.WriteLine(counter + ": " + concatStr);
                                            }
                                            if (desiredCourses.Count >= 5)
                                            {
                                                ArrayList course5List = (ArrayList)sectionsDesiredCourses[4];
                                                for (n = 0; n < course5List.Count; n++) //grab third course
                                                {
                                                    Course tempCourse5 = (Course)course5List[n];
                                                    String courseBits5 = tempCourse5.getBitArray();
                                                    course5BitArray = new BitArray(courseBits5.Select(c => c == '1').ToArray());
                                                    Boolean testBool4;

                                                    try
                                                    {
                                                        BitArray compareBits2 = course1BitArray.And(course5BitArray);
                                                        testBool = iterateBitArray(compareBits2);


                                                    }
                                                    catch (Exception e)
                                                    {
                                                        testBool = false;
                                                    }

                                                    try
                                                    {
                                                        BitArray compareBits3 = course2BitArray.And(course5BitArray);
                                                        testBool2 = iterateBitArray(compareBits3);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        testBool2 = false;
                                                    }

                                                    try
                                                    {
                                                        BitArray compareBits3 = course3BitArray.And(course5BitArray);
                                                        testBool3 = iterateBitArray(compareBits3);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        testBool3 = false;
                                                    }

                                                    try
                                                    {
                                                        BitArray compareBits4 = course4BitArray.And(course5BitArray);
                                                        testBool4 = iterateBitArray(compareBits4);
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        testBool4 = false;
                                                    }

                                                    if (testBool || testBool2 || testBool3 || testBool4)
                                                    {
                                                        //MessageBox.Show("Conflict between: " + tempCourse.getCRN() + "," + tempCourse2.getCRN() + "," + tempCourse3.getCRN());
                                                        continue;
                                                    }

                                                    if (desiredCourses.Count == 5 && debug) //for debugging purposes
                                                    {
                                                        counter++;
                                                        concatStr = tempCourse.getCRN() + "," + tempCourse2.getCRN() +
                                                            "," + tempCourse3.getCRN() + "," + tempCourse4.getCRN() +
                                                            "," + tempCourse5.getCRN() + ";";
                                                        //sw.WriteLine(counter + ": " + concatStr);
                                                    }




                                                    if (desiredCourses.Count >= 6)
                                                    {
                                                        ArrayList course6List = (ArrayList)sectionsDesiredCourses[5];

                                                        for (h = 0; h < course6List.Count; h++) //grab third course
                                                        {
                                                            Course tempCourse6 = (Course)course6List[h];
                                                            String courseBits6 = tempCourse6.getBitArray();
                                                            course6BitArray = new BitArray(courseBits6.Select(c => c == '1').ToArray());
                                                            Boolean testBool5;

                                                            try
                                                            {
                                                                BitArray compareBits2 = course1BitArray.And(course5BitArray);
                                                                testBool = iterateBitArray(compareBits2);


                                                            }
                                                            catch (Exception e)
                                                            {
                                                                testBool = false;
                                                            }

                                                            try
                                                            {
                                                                BitArray compareBits3 = course2BitArray.And(course5BitArray);
                                                                testBool2 = iterateBitArray(compareBits3);
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                testBool2 = false;
                                                            }

                                                            try
                                                            {
                                                                BitArray compareBits3 = course3BitArray.And(course5BitArray);
                                                                testBool3 = iterateBitArray(compareBits3);
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                testBool3 = false;
                                                            }

                                                            try
                                                            {
                                                                BitArray compareBits4 = course4BitArray.And(course5BitArray);
                                                                testBool4 = iterateBitArray(compareBits4);
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                testBool4 = false;
                                                            }

                                                            try
                                                            {
                                                                BitArray compareBits5 = course5BitArray.And(course6BitArray);
                                                                testBool5 = iterateBitArray(compareBits5);
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                testBool5 = false;
                                                            }

                                                            if (testBool || testBool2 || testBool3 || testBool4 || testBool5)
                                                            {
                                                                //MessageBox.Show("Conflict between: " + tempCourse.getCRN() + "," + tempCourse2.getCRN() + "," + tempCourse3.getCRN());
                                                                continue;
                                                            }

                                                            if (desiredCourses.Count == 6 && debug) //for debugging purposes
                                                            {
                                                                counter++;
                                                                concatStr = tempCourse.getCRN() + "," + tempCourse2.getCRN() +
                                                                    "," + tempCourse3.getCRN() + "," + tempCourse4.getCRN() +
                                                                    "," + tempCourse5.getCRN() + "," + tempCourse6.getCRN() + ";";
                                                                //sw.WriteLine(counter + ": " + concatStr);
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return concatStr;
        }

        /// <summary>
        /// The iterateBitArray
        /// </summary>
        /// <param name="bits">The bits<see cref="BitArray"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public bool iterateBitArray(BitArray bits)
        {
            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
