using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SolutionOfSplitingBills
{
    class SplitingBills
    {
        private string[] strOfFile; //store string representation of input file
        private int countParticipant = 0;//count how many participants have actually been added into participant array
        private int countCharge = 0;//count how many charges have actually been added into charge array
        private string fileName = ""; //hold the file name for the selected file
        private const string INTPATTERN = @"^-?\d+$";
        private const string MSGAFTERERR = ", and press any key to continue";
        private const string ZERO = "0";
        private const string LSTLINNOZERO = "The last line of input file is not zero";
        private const string DOT = ".";
        private const string INVALIDNUMFORMAT = "Invalid number format for first two lines, they should be integers";
        private const string INSUFNUMOFPARTS = "There are not enough participants information for current trip according to the number has provided";
        private const string INSUFNUMOFCHAGS = "There are not enough charges for current participant according to the number has privided";
        private const string NEGVALUE = "Value is negative";
        private const string INVALIDINTFORMAT = "The format for Integer type is invalid";

        //parse file to an array of string, one line at a time
        public void StrRepOfFile()
        {
            Console.Write("Press Enter to open the input file ");
            Console.ReadLine();
            OpenFileDialog ofd = new OpenFileDialog();
            //Debug.Print($"current path is {Directory.GetCurrentDirectory()}");
            ofd.InitialDirectory = $"{Directory.GetCurrentDirectory()}";
            ofd.Title = "Select Input File";
            ofd.DefaultExt = "txt";
            ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                fileName = ofd.SafeFileName;
                //clear the content from previous output in case users want to try the same input file
                this.ClearOutputOrCreateOutputFile();
                //Debug.Print($"the file name is {fileName}");
                //read file one line at a time and store them into a string array
                string[] strTemp = System.IO.File.ReadAllLines($"{fileName}");
                //Console.WriteLine("Contents of input1 is ");
                List<string> tempList = new List<string>();
                //add each nonempty line to a temp list
                // int count = 0;
                foreach (string line in strTemp)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        tempList.Add(line);
                    }

                }
                //convert list to an array
                this.strOfFile = tempList.ToArray();
                //suppend console
                //Console.ReadLine(); 
            }
        }

        //convert string array to array of Participants
        public void ConvertToParti()
        {
            //declare an int to traverse strOfFile, and traverse through the strOfFile till second last element.
            int i = 0;
            try
            {
                //check if the last line of input file is 0
                if (!strOfFile[strOfFile.Length - 1].Equals(ZERO))
                {
                    throw new Exception($"{LSTLINNOZERO} (Input File Line:{strOfFile.Length - 1})");
                }
                if (!Regex.Match(strOfFile[0], INTPATTERN).Success || !Regex.Match(strOfFile[1], INTPATTERN).Success)
                {
                    throw new Exception(INVALIDINTFORMAT);
                }
                //declare the array of participant for first camping trip

                int numOfPartis = this.ConvertToInt(strOfFile[0]);
                Participant[] parts = new Participant[numOfPartis]; 
                for (i = 1; i < strOfFile.Length; i++)
                {
                    //check if it is next trip
                    if (this.IsNextTrip(i))
                    {
                        CheckNumOfParticipant(parts);
                        CheckNumOfCharges(parts[parts.Length - 1]);
                        double sum = CalculateToTFunds(parts);
                        double avg = CalculateAvgMoney(sum, parts);
                        WriteToFile(avg, parts);
                        //get number of participants and initialize the arrya of participants
                        numOfPartis = this.ConvertToInt(strOfFile[i]);
                        parts = new Participant[numOfPartis];
                        //this.countParticipant = 0;
                    }
                    else if (this.IsNextParticipant(i))
                    {
                        if(this.countParticipant > 0)
                        {
                            CheckNumOfCharges(parts[countParticipant - 1]);
                        }
                        int numOfChags = this.ConvertToInt(strOfFile[i]);
                        Participant part = new Participant(numOfChags);
                        parts[countParticipant++] = part;
                        Debug.WriteLine("current number of participants is" + this.countParticipant);
                        //this.countCharge = 0;
                    }
                    else
                    {
                        Charge nextCharge = new Charge(strOfFile[i]);
                        parts[countParticipant - 1].AddCharges(nextCharge);
                        this.countCharge++;
                    }
                }
            }
            catch (Exception e)
            {
                this.PrintErrMes($"{e.Message} (Error From Input File line:${i})");
            }

        }

        //calculate total funds for current trip
        public double CalculateToTFunds(Participant[] parts)
        {
            double sum = 0;
            //go through the participants list and add its totoal money
            for (int i = 0; i < parts.Length; i++)
            {
                sum += parts[i].GetTotalMoney();
            }
            return sum;
        }

        //calculate average money for this trip
        public double CalculateAvgMoney(double sum, Participant[] parts)
        {
            return Math.Round(sum / parts.Length, 2);
        }

        //write to a text file with associated money for each participant
        public void WriteToFile(double avg, Participant[] parts)
        {
            StreamWriter sw = new StreamWriter($"{fileName}.out", true);
            //start writting to the output file
            for (int i = 0; i < parts.Length; i++)
            {    
                double reminder = Math.Round(avg - parts[i].GetTotalMoney(), 2);
                //check if reminder is negative or positive, if positive, just write the value, otherwise, add backets around the value
                if(reminder < 0)
                {
                    double absRem = Math.Abs(reminder);
                    sw.WriteLine($"(${absRem})");
                }
                else
                {
                    sw.WriteLine($"${reminder}");
                }
                
            }
            sw.WriteLine();
            sw.WriteLine();
            sw.Close();
        }

        //print output file to console
        public void PrintToConsole()
        {
            Console.WriteLine();
            Console.WriteLine();
            StreamReader sr = File.OpenText($"{fileName}.out");
            string s = "";
            while ((s = sr.ReadLine()) != null)
            {
                Console.WriteLine(s);
            }
            sr.Close();
        }

        //print error message and press any key to continue
        public void PrintErrMes(String msg)
        {
            //print error message on console
            Console.Write(msg + MSGAFTERERR);
            //suspent console
            Console.ReadLine();
            //exit the application
            Environment.Exit(-1);
        }

        //check if it is about to start next trip
        public Boolean IsNextTrip(int i)
        {
            Boolean result = false;
            if (this.strOfFile[i].Equals(ZERO))
            {
                result = true;
            }
            else if (Regex.Match(strOfFile[i], INTPATTERN).Success && Regex.Match(strOfFile[i + 1], INTPATTERN).Success)
            {
                result = true;
            }
            else
            {

            }
            return result;
        }


        //check if the input file provides sufficient participants that input file has specified, or reset countparticipant
        public Boolean CheckNumOfParticipant(Participant[] parts)
        {
            Boolean result = true;
            if (parts.Length != this.countParticipant)
            {
                result = false;
                throw new Exception(INSUFNUMOFPARTS);
            }
            else
            {
                countParticipant = 0;
            }
            return result;
        }

        //for each participant check if there are sufficient charges according to the input file has specified, or reset countcharge
        public Boolean CheckNumOfCharges(Participant p)
        {
            Boolean result = true;
            int s = p.GetCharges().Length;
            if (s != countCharge)
            {
                result = false;
                throw new Exception(INSUFNUMOFCHAGS);
            }
            else
            {
                countCharge = 0;
            }
            return result;
        }

        //convert string to int
        public int ConvertToInt(string line)
        {
            if (!Regex.Match(line, INTPATTERN).Success)
            {
                throw new Exception(INVALIDINTFORMAT);
            }
            int result = int.Parse(line);
            if (result < 0)
            {
                throw new Exception(NEGVALUE);
            }
            return result;
        }

        public Boolean IsNextParticipant(int i)
        {
            Boolean result = false;
            if (Regex.Match(strOfFile[i], INTPATTERN).Success)
            {
                result = true;
            }
            return result;
        }
        public void ClearOutputOrCreateOutputFile()
        {
            //reset the output file, clear the content
            System.IO.File.WriteAllText($"{fileName}.out", string.Empty);
        }
        //you must add the [STAHread on the top in order to use the fileopendialog function, otherwise you would get an exception]
        [STAThread]
        static void Main(string[] args)
        {
            SplitingBills spltb = new SplitingBills();
            spltb.StrRepOfFile();
            spltb.ConvertToParti();
            spltb.PrintToConsole();
            Console.Write("Do you want to continue testing? Press y to continue or n to exit the application(y/n)");
            string result = Console.ReadLine();
            while (result.Equals("y"))
            {
                spltb.StrRepOfFile();
                spltb.ConvertToParti();
                spltb.PrintToConsole();
                Console.Write("Do you want to continue testing? Press y to continue or n to exit the application(y/n)");
                result = Console.ReadLine();
            }
        }

    }
}