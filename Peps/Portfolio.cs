using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Peps
{
    public class Portfolio
    {

        private double InitialCash;
        private double Cash;
        //We can change the current Date
        private int y, m, d;
        public double[][] data;
        public List<String> symbols;
        public Dictionary<String, String> currSymbol;
        public Dictionary<String, double> currCash;
        //Quantity of each asset in the portfolio
        public double[] quantity;
        
        public Portfolio(int size)
        {
            //Benefit from the selling of the product at t=0
            InitialCash = 0;
            //Cash in EUR
            Cash = 0;
            //Currency of each assset
            currSymbol = new Dictionary<string, string>();
            //Cach in each foreign currency
            currCash = new Dictionary<string, double>();
            symbols = new List<String>();
            quantity = new double[size];
            for (int i = 0; i < size;i++ )
            {
                quantity[i] = 0;
            }
            y = 2005;
            m = 11;
            d = 30;
            currCash.Add("GBPEUR", 0);
            currCash.Add("USDEUR", 0);
            currCash.Add("CHFEUR", 0);
            //Livre
            currSymbol.Add("HSEA", "GBPEUR");    
            //Livre
            currSymbol.Add("DEO", "GBPEUR");    
            //Livre
            currSymbol.Add("TSCDF", "GBPEUR");
            //Dollars
            currSymbol.Add("WFC", "USDEUR");
            //Dollars
            currSymbol.Add("PEP", "USDEUR");
            //CHF
            currSymbol.Add("NVS", "CHFEUR");
        }

        public void setDate(int year, int month, int day)
        {
            y = year;
            m = month;
            d = day;
        }
        public int getY()
        {
            return y;
        }
        public int getM(){
            return m;
        }

        public int getD()
        {
            return d;
        }
        public int[] previousDate()
        {
            int[] previousDate= new int[3];
            //The currentDate is a constation date
            if(m==11 && d==30){
                previousDate[0]= y; 
                previousDate[1]= 0;
                previousDate[2] = 0;
                return previousDate;
            }
            //Dans le cas contraire on trouve la date de constatation précédente
            previousDate[2]= 30;
            previousDate[1]= 11;
            //Every date before november are from the previous year
            if(m<=11){
                previousDate[0]=y-1;
            }
            else
            {
                previousDate[0] = y;
            }
            return previousDate;

        }
        public double getCash()
        {
            return Cash;
        }

        public void setCash(double cash)
        {
            Cash = cash;
        }
        public void setInitialCash(double cash)
        {
            InitialCash = cash;
        }

    }
}