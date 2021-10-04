using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string inputFilePath = @"D:\04-10-2021.pu"; // txt_inPath.Text;
            string OutputFilePath = @"D:\";   // txt_OutPath.Text;  //Please type the path only without the file name

            try
            {
                ParseData(inputFilePath, OutputFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// validate if file input is exist and have valid name;
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <returns></returns>
        public static bool ValidateInputFile(string inputFilePath)
        {
            //get file info
            FileInfo f = new FileInfo(inputFilePath);
            //check if file exist
            if (!f.Exists)
                return false;
            //check if file name is valid
            if (f.Name != string.Format("{0}.pu", DateTime.Now.ToString("dd-MM-yyyy")))
                return false;
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="OutputFilePath"></param>
        ///
        public static void ParseData(string inputFilePath, string OutputFilePath)
        {
            if (!ValidateInputFile(inputFilePath))      //validate input file
                throw new Exception("Invalid file");
            var one_row = File.ReadAllLines(inputFilePath);
            var first_line = one_row[0].Split(',').ToList();
            if (first_line[0] != ("date:" + inputFilePath.Substring(inputFilePath.Length - 13, 10)))
                throw new Exception("Invalid file");
            if (first_line[1] != "count:" + (one_row.Count() - 1).ToString())
                throw new Exception("Invalid file");
            using (StreamWriter sw = new StreamWriter(OutputFilePath + inputFilePath.Substring(inputFilePath.Length - 13, 10) + ".ur"))
            {
                sw.WriteLine(one_row[0] + "\n" + "{");
                for (int r = 1; r < one_row.Length; r++)
                {
                    string[] All = one_row[r].ToString().Split(new string[] { "[", "]" }, StringSplitOptions.None);
                    string[] haf = All[0].Split(new char[] { ',' }); //putFilePath{Number ,
                    if (haf.Count() != 4)  //Check the number and columns in all lines;
                        throw new Exception("Invalid file");
                    sw.WriteLine("[");
                    //Writing the first half {Number , string};
                    sw.WriteLine("age:" + haf[0].ToString() + "," + "\n" + "        country:" + haf[1].ToString() + "," + "\n" + "        Name:" + haf[2].ToString());
                    if (All[1].Contains(","))   //Find out if there is between [] more than one element {array of string};
                    {   //Write more than one element;
                        string[] last_half = All[1].Split(new char[] { ',' }); //putFilePath array
                        sw.WriteLine("Date:[");
                        for (int x = 0; x < last_half.Length; x++)
                        {
                            sw.WriteLine("" + last_half[x] + ",");
                        }
                        sw.WriteLine("]");
                        sw.WriteLine("]");
                    }
                    else
                    {   //If it is one element;
                        sw.WriteLine("Date:[");
                        sw.WriteLine("" + All[1]);
                        sw.WriteLine("]");
                        sw.WriteLine("],");
                    }
                }
            }
            MessageBox.Show("The data from the file has been successfully processed");
        }
    }
}