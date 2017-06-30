using System;
using System.Collections.Generic;
using System.Text;

namespace SolutionOfSplitingBills
{
    //Represents each Participant in each group
    class Participant
    {
        private Charge[] charges;
        private int counter = 0;//count how many charges have been added
        private double sum = 0; // total money for this participant


        public Participant(int numOfChags)
        {
            this.charges = new Charge[numOfChags];
            counter = 0;
        }

        //add one charge for current participant
        public void AddCharges(Charge chag)
        {
            charges[counter++] = chag;
            sum += chag.GetMoney();
        }
        
        //get charge array that contains all the charges for this participant
        public Charge[] GetCharges()
        {
            return charges;
        }

        public void SetCharges(Charge[] chags)
        {
            this.charges = chags;
        }

        //calculate total 
       public double GetTotalMoney()
        {
            return this.sum;
        }
    }
}
