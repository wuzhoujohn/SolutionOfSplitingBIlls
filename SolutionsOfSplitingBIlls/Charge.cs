using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SolutionOfSplitingBills
{
    //represents each charge of a participant
    class Charge
    {
        private double money;
        private const string DOUBLEPATTERN = @"^-?\d+.\d\d$";
        private const string NEGVALUE = "Value is negative";
        private const string INVALIDMONEYFORMAT = "Money format is invalid";

        public Charge(string funds)
        {
            try
            {
                money = this.ConvertDouble(funds);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        //convert funds to double
        public double ConvertDouble(string funds)
        {
            double result;
            try
            {
                Match match = Regex.Match(funds, DOUBLEPATTERN);
                if (!match.Success)
                {
                    throw new Exception(INVALIDMONEYFORMAT);
                }
                else
                {
                    result = Double.Parse(funds);
                    if (result < 0)
                    {
                        throw new Exception(NEGVALUE);
                    }
                    return result;
                }               
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //get money value
        public double GetMoney()
        {
            return this.money;
        }
    }
}
