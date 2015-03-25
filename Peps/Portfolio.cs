using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapper;

namespace Peps
{
    public class Portfolio
    {
        public int index { get; set; }
        public int numberOfStock { get; set; }
        private double InitialCash;
        private double Cash;
        //We can change the current Date
        private int y, m, d;
        //Historical data
        public double[][] delta { get; set; }
        public double[][] market { get; set; }
        public double[] prix { get; set; }
        public double[] pfvalue { get; set; }
        public double[] profitAndLoss { get; set; }
        public double trackingError { get; set; }
        public double[] quantity { get; set; }
        public List<String> dates;
        
        //All market values
        public double[][] data;
        public List<String> symbols;
        public Dictionary<String, String> currSymbol;
        public Dictionary<String, double> currCash;
        //Quantity of each asset in the portfolio
        public WrapperClass wrapper;
        public MarketData marketData;

        public Portfolio(WrapperClass wrapper, MarketData marketData)
        {
            index = 0;
            this.wrapper = wrapper;
            this.marketData = marketData;
            int size = wrapper.getOption_size() + wrapper.getCurr_size();
            int nbDate = wrapper.getH();
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

        public int getM()
        {
            return m;
        }

        public int getD()
        {
            return d;
        }

        public double[] getCurrentDelta()
        {
            return this.delta[index];
        }

        public double[] getCurrentMarketValues()
        {
            return this.market[index];
        }

        public double[] getCurrentQuantities()
        {
            return this.quantity;
        }

        public double getCurrentProfitAndLoss()
        {
            return profitAndLoss[index];
        }

        public double getCurrentPortfolioValue()
        {
            return pfvalue[index];
        }

        public double getCurrentPrice()
        {
            return prix[index];
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
            if ( m <= 11 )
            {
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

        public double getInitialCash()
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

        public int incrementIndex()
        {
            return index++;
        }

        public void loadSymbols(String[] symbols)
        {
            foreach (String symbol in symbols)
            {
                this.symbols.Add(symbol);
            }
        }

        public void getData()
        {
            //Retrieve data from 30 august 2005 to 30 january 2015  
            marketData.retrieveDataFromWeb("7", "30", "2005", "0", "30", "2015");
            marketData.fixeDataSize();
            marketData.storeData();
        }

        //Use as a validation test of our model
        //Function use to initialize the computation
        public void LoadFromResource(int idSimulation)
        {
            index = 0;
            String prixSimu, deltaSimu, marketSimu;
            switch (idSimulation)
            {
                case 1:
                    prixSimu = Properties.Resources.Prix1;
                    deltaSimu = Properties.Resources.Deltas1;
                    marketSimu = Properties.Resources.Market1;
                    break;
                case 2:
                    prixSimu = Properties.Resources.Prix2;
                    deltaSimu = Properties.Resources.Deltas2;
                    marketSimu = Properties.Resources.Market2;
                    break;
                case 3:
                    prixSimu = Properties.Resources.Prix3;
                    deltaSimu = Properties.Resources.Deltas3;
                    marketSimu = Properties.Resources.Market3;
                    break;
                default:
                    return;
            }
            // Load the symbols from first line of market resource
            this.loadSymbols(marketSimu.Substring(0, marketSimu.IndexOf('\n')).Split(' '));
            //Initialize the historical value stored in our portfolio
            this.delta = Utils.parseFileToMatrix(deltaSimu);
            this.numberOfStock = this.delta[0].Length;
            this.prix = Utils.parseFileToArray(prixSimu);
            this.market = Utils.parseFileToMatrix(marketSimu);

            InitSimu();
        }

        //Init the simulation for t=0
        //Receive the product price, the market prices and the delta as parameters
        public void InitSimu()
        {
            this.index = 0;

            // Allocate memory
            this.pfvalue = new double[this.prix.Length];
            this.profitAndLoss = new double[this.prix.Length];

            double prixInit = this.prix[0];

            //Initial benefit made from the sell of our product
            this.setInitialCash(100 - prixInit);
            //We use the price of our product for hedging
            this.setCash(prixInit);
            this.pfvalue[0] = prixInit;
            //Compute the tracking error
            this.trackingError = 0;
            //Compute the profit and Loss
            this.profitAndLoss[0] = 100 - prixInit;
        }

        public void loadFromComputations()
        {
            index = 0;
            this.prix = new double[wrapper.getH()];
            this.delta = new double[wrapper.getH()][];
            this.market = new double[wrapper.getH()][];
            this.dates = new List<String>(); 
            this.data = Utils.parseFileToMatrix(Properties.Resources.Market3, dates);
            //Compute the current index in the data 
            String currentDateString = this.getD().ToString() + "-" + this.getM().ToString() + "-" + this.getY().ToString();
            int indexCurrentDate = this.dates.IndexOf(currentDateString);
            if (indexCurrentDate == -1)
            {
                indexCurrentDate = this.dates.Count - 1;
            }
            //TO DO: load all previous delta + market value + prix 
          
            //TO DO 
            //Calibration de la volatilité
            wrapper.initHistPrice(64);
            for (int i = indexCurrentDate; i < indexCurrentDate + 64; i++ )
            {
                for (int j = 0; j < wrapper.getOption_size() + wrapper.getCurr_size(); j++ )
                {
                    wrapper.setHistPrice(this.data[i][j]);
                }
            }
            //wrapper.computeVol();

            //Price in 0
            if (this.getM() == 11 && this.getD() == 30 && this.getY() == 2005)
            {
                this.market[index] = this.data[indexCurrentDate];
                //Initialize the spot vector
                for (int i = 0; i < this.market[index].Length; i++)
                {
                    wrapper.setSpot(this.market[index][i], i);
                }
                wrapper.computePrice();
                wrapper.computeDelta();         
                this.delta[index] = wrapper.getDelta();
                this.prix[index] = wrapper.getPrice();
                this.InitSimu();
            }
            else
            {
                int indexFirstDate = this.dates.IndexOf("30-11-2005");
                //TODO load previous delta from FILE + incrémenter index 

                //Compute the date t in years
                int[] previousDate;
                previousDate = this.previousDate();
                double t = previousDate[0] - 2005;
                if (previousDate[0] == this.getY())
                {
                    t += ((double)previousDate[2]) / 365.0;
                }
                else
                {
                    t += ((double)(((this.getM() - previousDate[1]) + 11) * 30 + previousDate[2])) / 365.0;
                }
                //Compute the past matrice
                computePast(this.dates);
                //Compute delta and price for the date t
                wrapper.computePrice(t);
                wrapper.computeDelta(t);
                //TO DO A appeler avec le premier delta et premier prix
                //InitSimu(wrapper.getDelta(), wrapper.getPrice());
                //Copy the current delta and market value in the portfolio
                this.market[indexFirstDate - indexCurrentDate] = this.data[indexCurrentDate];
                this.delta[indexFirstDate - indexCurrentDate] = wrapper.getDelta();
                this.prix[indexFirstDate - indexCurrentDate] = wrapper.getPrice();
                //We move forward in the simulation
                ComputeSimulation(indexFirstDate - indexCurrentDate);
            }
        }

        //A optimiser
        //Compute the past matrix in wrapper
        public void computePast(List<String> dates)
        {

            int[] previousDate = this.previousDate();
            if (this.getM() == 11 && this.getD() == 30)
            {
                wrapper.initPast(previousDate[0] - 2005 + 1);
            }
            else
            {
                wrapper.initPast(previousDate[0] - 2005 + 2);
            }

            //Include the previous constation date
            String str = "30-11-";
            for (int year = 2005; year <= previousDate[0]; year++)
            {
                String temp = str + year.ToString();
                int index = dates.IndexOf(temp);
                if (index == -1)
                {
                    index = dates.Count - 1;
                }
                for (int i = 0; i < (wrapper.getOption_size() + 3); i++)
                {

                    wrapper.set(this.data[index][i]);
                }

            }
            //Include one more date if t isn't a constation date

            if (this.getM() != 11 && this.getD() != 30)
            {
                str = this.getD().ToString() + "-" + this.getM().ToString() + "-" + this.getY().ToString();
                int indexLastDate = dates.IndexOf(str);
                if (indexLastDate == -1)
                {
                    indexLastDate = dates.Count - 1;
                }
                for (int i = 0; i < (wrapper.getOption_size() + 3); i++)
                {
                    wrapper.set(this.data[indexLastDate][i]);
                }
            }
        }

        public void ComputeSimulation(int numberOfIteration = 1)
        {
            for (int k = 0; k < numberOfIteration; k++)
            {
                this.incrementIndex();
                double vp = 0;
                this.setCash(this.getCash() * Math.Exp(wrapper.getR() * (1 / 250)));
                this.setInitialCash(this.getInitialCash() * Math.Exp(wrapper.getR() * (1 / 250)));
                for (int i = 0; i < this.delta[index].Length; i++)
                {
                    this.setCash(this.getCash() - (this.delta[index - 1][i] - this.quantity[i]) * this.market[index - 1][i]);
                    this.quantity[i] = this.delta[index - 1][i];
                    vp += this.delta[index][i] * this.market[index][i];
                }
                vp += this.getCash();
                this.pfvalue[index] = vp;
                this.trackingError = vp - this.prix[index];
                this.profitAndLoss[index] = this.trackingError + this.getInitialCash();
            }
        }

        public void Update()
        {
            String str = this.getD().ToString() + "-" + this.getM().ToString() + "-" + this.getY().ToString();
            int indexCurrentDate = this.dates.IndexOf(str);
            if (indexCurrentDate == -1)
            {
                indexCurrentDate = this.dates.Count - 1;
            }
            indexCurrentDate--;
            str = this.dates[indexCurrentDate];
            int[] dates = Utils.convertToInt(str.Split('-'));
            this.setDate(dates[0], dates[1], dates[2]);

            //TO DO factorise
            //Compute the date t in years
            int[] previousDate;
            previousDate = this.previousDate();
            double t = previousDate[0] - 2005;
            if (previousDate[0] == this.getY())
            {
                t += ((double)previousDate[2]) / 365.0;
            }
            else
            {
                t += ((double)(((this.getM() - previousDate[1]) + 11) * 30 + previousDate[2])) / 365.0;
            }
            //Compute the past matrice
            computePast(this.dates);
            //Compute delta and price for the date t
            wrapper.computePrice(t);
            wrapper.computeDelta(t);
            //Copy the current delta and market value in the portfolio
            this.market[index + 1] = this.data[indexCurrentDate];
            this.delta[index + 1] = wrapper.getDelta();
            this.prix[index + 1] = wrapper.getPrice();
            ComputeSimulation();
        }
    }
}