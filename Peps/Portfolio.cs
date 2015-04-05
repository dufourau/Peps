using ServiceStack.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using TeamDev.Redis;
using Wrapper;

namespace Peps
{
    public class Portfolio
    {
        private static long CurrentId = 0;

        public long Id { get; set; }

        public int NumberOfAsset { get; set; }
        public DateTime CurrentDate { get; set; }
        //Quantity of each asset in the portfolio
        public WrapperClass Wrapper;
        public MarketData MarketData;
        public double InitialCash { get; set; }
        public double Cash { get; set; }
        public double ProductPrice { get; set; }
        public double PortfolioValue { get; set; }
        public List<double> PortfolioValueHistory { get; set; }
        public List<double> ProductPriceHistory { get; set; }        
        public double ProfitAndLoss { get; set; }
        public double TrackingError { get; set; }
        public double[] QuantityOfAssets { get; set; }
        public double[] Delta { get; set; }
        public double TransactionFees { get; set; }
        double[,] PreviousStocksPrices { get; set; }
        double[] PreviousInterestRates { get; set; }
        double[] StockToFxIndex{ get; set; }
        int RBSindex { get; set; }       
        int CitiGroupIndex { get; set; }

        double[,] hedgingPreviousStocksPrices { get; set; }
        double[] hedgeDPreviousStockPrices { get; set; }

        public void save(){
            /*
            * Redis DEMO: save the portefolio to the database
            */
            var redisManager = new PooledRedisClientManager(Properties.Settings.Default.RedisDatabaseURL);
            redisManager.ExecAs<Portfolio>(redisPf => {
                using (var redis = new RedisClient(Properties.Settings.Default.RedisDatabaseURL))
                {
                    this.Id = redis.As<Portfolio>().GetNextSequence();
                var pf = this;
                redisPf.Store(pf);
                }
            });
        }

        public Portfolio(WrapperClass wrapper, MarketData marketData, DateTime initialDate)
        {
            this.CurrentDate = initialDate;
            this.Wrapper = wrapper;
            this.MarketData = marketData;
            //Cash in EUR
            this.InitialCash = 0;
            this.Cash = 0;
            this.RBSindex = 18;
            this.NumberOfAsset = Properties.Settings.Default.AssetNb + Properties.Settings.Default.FxNb;
            this.QuantityOfAssets = new double[NumberOfAsset];
        }

        public Portfolio find(long Id)
        {
            var redisManager = new PooledRedisClientManager(Properties.Settings.Default.RedisDatabaseURL);

            Portfolio portfolio = null;

            redisManager.ExecAs<Portfolio>(redis =>
        {
                portfolio = redis.GetById(Id);
            });

            portfolio.MarketData = new MarketData();
            portfolio.Wrapper = new WrapperClass();

            return portfolio;
        }

        public void goToNextDate()
        {
            this.CurrentDate = this.CurrentDate.AddDays(1);
        }

        internal void compute(Boolean live)
        {
            DateTime initialDate = new DateTime(2005, 11, 30);
            DateTime finalDate = new DateTime(2015,11,30);
            //t=0
            if (initialDate.CompareTo(CurrentDate) == 0)
            {
                initComputationParameters(live);
                //We convert RBS and citigroup stocks prices:
                for (int j = 0; j < PreviousStocksPrices.GetLength(0); j++)
                {
                    PreviousStocksPrices[j, RBSindex] = PreviousStocksPrices[j, RBSindex] * PreviousStocksPrices[j, Properties.Settings.Default.AssetNb + 1] / 100.0;
                    PreviousStocksPrices[j, CitiGroupIndex] = PreviousStocksPrices[j, CitiGroupIndex] * PreviousStocksPrices[j, Properties.Settings.Default.AssetNb + 3];
                }
                performInitialComputations();           
                this.InitialCash = Properties.Settings.Default.Nominal - this.Wrapper.getPrice();
                this.Cash = this.Wrapper.getPrice();
                this.PortfolioValue = Cash;
                this.ProfitAndLoss = InitialCash;

            }else{
                double nbdays = (CurrentDate - initialDate).TotalDays;
                double totalnbdays = (finalDate - initialDate).TotalDays;
                double t = nbdays / (totalnbdays/10);            
                initComputationParameters(live);
                //We convert RBS and citigroup stocks prices:
                for (int j = 0; j < PreviousStocksPrices.GetLength(0); j++)
                {
                    PreviousStocksPrices[j, RBSindex] = PreviousStocksPrices[j, RBSindex] * PreviousStocksPrices[j, Properties.Settings.Default.AssetNb + 1] / 100.0;
                    PreviousStocksPrices[j, CitiGroupIndex] = PreviousStocksPrices[j, CitiGroupIndex] * PreviousStocksPrices[j, Properties.Settings.Default.AssetNb + 3];
                }
                performComputations(t, Utils.Convert2dArrayto1d(computePast(t)));
                //TO DO: 
                double actualizationFactor = Math.Exp((PreviousInterestRates[0] / 100) * (1 / (Properties.Settings.Default.RebalancingNb / Properties.Settings.Default.Maturity)));
                this.InitialCash = Properties.Settings.Default.Nominal - this.Wrapper.getPrice();

            }
            
        }

        //TODO to factorize
        public double[,] computePast(double t)
        {
            double[,] past;
            //Constatation date
            if(Math.Floor(t) == t){
                past = new double[(int)Math.Floor(t),Properties.Settings.Default.AssetNb];
                int cpt = 0;
                for(int i = 2005; i < 2005 +(int)Math.Floor(t) ;i++){
                    DateTime date = new DateTime(i, 30, 11);
                    ArrayList prices = MarketData.getAllPricesAtDate(date);
                    for (int j = 0; j < prices.Count; j++)
                    {
                        past[cpt, j] = (double)prices[0];
                    }
                    cpt++;
                }
            }
            else
            {
                 past = new double[(int)Math.Floor(t)+1,Properties.Settings.Default.AssetNb];
                 int cpt = 0;
                 for (int i = 2005; i < 2005 + (int)Math.Floor(t); i++)
                 {
                     DateTime date = new DateTime(i, 30, 11);
                     ArrayList prices = MarketData.getAllPricesAtDate(date);
                     for (int j = 0; j < prices.Count; j++)
                     {
                         past[cpt, j] = (double)prices[0];
                     }
                     cpt++;
                 }
                 ArrayList lastPrices = MarketData.getAllPricesAtDate(CurrentDate);
                 for (int j = 0; j < lastPrices.Count; j++)
            {
                     past[cpt, j] = (double)lastPrices[0];
                 }

            }

            return past;


            
        }

        private void performInitialComputations()
        {
            double [] oneDpreviousStocksPrices = Utils.Convert2dArrayto1d(PreviousStocksPrices);
            Wrapper.computePrice(oneDpreviousStocksPrices, PreviousInterestRates, StockToFxIndex,
                Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, PreviousStocksPrices.GetLength(0),
                Properties.Settings.Default.StepFiniteDifference);
            this.ProductPrice = Wrapper.getPrice();

            Wrapper.computeDelta(oneDpreviousStocksPrices, PreviousInterestRates, StockToFxIndex,
                Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, PreviousStocksPrices.GetLength(0),
                Properties.Settings.Default.StepFiniteDifference);
          this.Delta = Wrapper.getDelta();
        }

        private void performComputations(double t, double[] oneDPast)
        {
            double[] oneDpreviousStocksPrices = Utils.Convert2dArrayto1d(PreviousStocksPrices);
            Wrapper.computePrice(oneDpreviousStocksPrices, PreviousInterestRates, StockToFxIndex,
                Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, PreviousStocksPrices.GetLength(0),
                Properties.Settings.Default.StepFiniteDifference);
            this.ProductPrice = Wrapper.getPrice();

            Wrapper.computeDelta(oneDpreviousStocksPrices, PreviousInterestRates, StockToFxIndex,
                Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, PreviousStocksPrices.GetLength(0),
                Properties.Settings.Default.StepFiniteDifference);
            this.Delta = Wrapper.getDelta();
        }

        //live = true => request to yahoo finance
        //live = false => search in our database
        private void initComputationParameters(Boolean live)
        {
            StockToFxIndex = new double[Properties.Settings.Default.AssetNb];
            PreviousInterestRates = new double[Properties.Settings.Default.FxNb + Properties.Settings.Default.AssetNb + 1];

            
            PreviousStocksPrices = FillPreviousStockPrices( StockToFxIndex, live);
            FillFxRates(PreviousStocksPrices,live);
            FillPreviousInterestRates(PreviousInterestRates);
        }

        private void FillHedgingParameters(double[,] previousStocksPrices)
        {
            string tmpStockTicker;
            DateTime stocksStartDate = CurrentDate.AddMonths(-3);
            DateTime stocksEndDate = CurrentDate.AddYears(2);

            int size = 900;
            Dictionary<String, ArrayList> symbolToPricesList = new Dictionary<string, ArrayList>();
            ArrayList tmp;
            foreach (PropertyInfo property in
                typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Length == 12)
                {
                    tmpStockTicker = Properties.Resources.ResourceManager.GetString(property.Name).Split(';')[1];

                    tmp = this.MarketData.getLastStockPrices(tmpStockTicker, stocksStartDate.Day.ToString(), stocksStartDate.Month.ToString(),
                        stocksStartDate.Year.ToString(), stocksEndDate.Day.ToString(), stocksEndDate.Month.ToString(), stocksEndDate.Year.ToString());
                    if (tmp != null)
                    {
                        symbolToPricesList.Add(property.Name, tmp);
                        size = Math.Min((symbolToPricesList[property.Name]).Count, size);
                    }
                }
            }
            int cpt = 0;
            hedgingPreviousStocksPrices = new double[size, Properties.Settings.Default.AssetNb + Properties.Settings.Default.FxNb];
            foreach (KeyValuePair<String, ArrayList> entry in symbolToPricesList)
            {
                for (int i = 0; i < size; i++)
                {
                    hedgingPreviousStocksPrices[i, cpt] = (double)entry.Value[i];
                    if (previousStocksPrices[i, cpt] < 0.01) throw new Exception();
                }
                cpt++;
            }
        }

        private void FillPreviousInterestRates(double[] previousInterestRates)
        {
            int index = this.MarketData.rates.Length - this.MarketData.dates.IndexOf(this.CurrentDate);

            previousInterestRates[0] = this.MarketData.rates[index][0] / 100;
            for (int i = 1; i < Properties.Settings.Default.AssetNb; i++) previousInterestRates[i] = 0;
            previousInterestRates[Properties.Settings.Default.AssetNb + 1] = this.MarketData.rates[index][1] / 100;
            previousInterestRates[Properties.Settings.Default.AssetNb + 2] = this.MarketData.rates[index][2] / 100;
            previousInterestRates[Properties.Settings.Default.AssetNb + 3] = this.MarketData.rates[index][3] / 100;
            previousInterestRates[Properties.Settings.Default.AssetNb + 4] = this.MarketData.rates[index][4] / 100;
        }

      

        private double[,] FillPreviousStockPrices(double[] stockToFxIndex, Boolean live)
        {
            double[,] previousStocksPrices;
            string tmpStockTicker;
            DateTime calibrationStartDate = CurrentDate.AddDays(-Properties.Settings.Default.VolCalibrationDaysNb);
            int size = Properties.Settings.Default.VolCalibrationDaysNb + 1;
            Dictionary<String, ArrayList> symbolToPricesList = new Dictionary<string, ArrayList>();
            ArrayList tmp;
            int cpt = 0;
            foreach (PropertyInfo property in
                typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Length == 12)
                {
                    tmpStockTicker = Properties.Resources.ResourceManager.GetString(property.Name).Split(';')[1];
                    if (tmpStockTicker.Contains("RBS"))
                        RBSindex = cpt;
                    else if (tmpStockTicker.Equals("C"))
                        CitiGroupIndex = cpt;

                    if (live)
                    {
                        tmp = MarketData.getLastStockPricesFromWeb(tmpStockTicker, calibrationStartDate.Day.ToString(), calibrationStartDate.Month.ToString(),
                        calibrationStartDate.Year.ToString(), CurrentDate.Day.ToString(), CurrentDate.Month.ToString(), CurrentDate.Year.ToString());
                    }
                    else
                    {
                        tmp = MarketData.getLastStockPrices(tmpStockTicker, calibrationStartDate.Day.ToString(), calibrationStartDate.Month.ToString(),
                        calibrationStartDate.Year.ToString(), CurrentDate.Day.ToString(), CurrentDate.Month.ToString(), CurrentDate.Year.ToString());
                    }
                    
                    if (tmp != null)
                    {
                        symbolToPricesList.Add(property.Name, tmp);
                        size = Math.Min((symbolToPricesList[property.Name]).Count, size);
                    }


                    cpt++;
                }
            }
            previousStocksPrices = new double[size, Properties.Settings.Default.AssetNb + Properties.Settings.Default.FxNb];
            cpt = 0;
            foreach (KeyValuePair<String, ArrayList> entry in symbolToPricesList)
            {
                for (int i = 0; i < size; i++)
                {
                    if (live)
                    {
                        previousStocksPrices[i, cpt] = (double)entry.Value[i];
                    }
                    else
                    {
                        previousStocksPrices[i, cpt] = Double.Parse(((String)entry.Value[i]).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    SetStockToFxList(stockToFxIndex, entry.Key, cpt);
                    if (previousStocksPrices[i, cpt] < 0.01) throw new Exception();
                }
                cpt++;
            }
            return previousStocksPrices;
        }

        void SetStockToFxList(double[] stockToFxIndex, string stockSymbol, int stockIndex)
        {
            switch (stockSymbol.Substring(0, 2))
            {
                case "CH": stockToFxIndex[stockIndex] = Properties.Settings.Default.AssetNb;
                    break;
                case "GB": stockToFxIndex[stockIndex] = Properties.Settings.Default.AssetNb + 1;
                    break;
                case "JP": stockToFxIndex[stockIndex] = Properties.Settings.Default.AssetNb + 2;
                    break;
                case "US": stockToFxIndex[stockIndex] = Properties.Settings.Default.AssetNb + 3;
                    break;
                default: stockToFxIndex[stockIndex] = -1;
                    break;
            }
        }

        //Fill currency prices
        private void FillFxRates(double[,] previousStocksPrices,Boolean live)
        {
            DateTime calibrationStartDate = CurrentDate.AddDays(-Properties.Settings.Default.VolCalibrationDaysNb);
            ArrayList fxPrices;
            int cpt = Properties.Settings.Default.AssetNb;
            foreach (PropertyInfo property in
               typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Substring(0, 2).Equals("Fx"))
                {
                    if (live)
                    {
                        fxPrices = MarketData.getPreviousCurrencyPricesFromWeb(property.Name.Substring(2), calibrationStartDate.ToString("u"), CurrentDate.ToString("u"));
                    }
                    else
                    {
                        fxPrices = MarketData.getPreviousCurrencyPrices(property.Name.Substring(2), calibrationStartDate, CurrentDate);
                    }    
                        
                    for (int j = 0; j < Math.Min(previousStocksPrices.GetLength(0), fxPrices.Count); j++)
                    {
                        if (live)
                        {
                            previousStocksPrices[j, cpt] = (double)fxPrices[j];
                        }
                        else
                        {
                            previousStocksPrices[j, cpt] = Double.Parse(((String)fxPrices[j]).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                        }
                                            
                    }
                    cpt++;
                }
            }
        }

        public void computeHedge(int numberOfIteration = 1)
        {
            for (int k = 0; k < numberOfIteration; k++)
            {
                this.goToNextDate();
                double portfolioValue = 0;
                // Actualize Cash and Initial Cash
                double actualizationFactor = Math.Exp((PreviousInterestRates[0]) * (1 / (Properties.Settings.Default.RebalancingNb / Properties.Settings.Default.Maturity)));
                this.Cash *= actualizationFactor;
                this.InitialCash *= actualizationFactor;
                //To put in currency cash? or in cash?
                // Handle the currency interest
                for (int i = 0; i < Properties.Settings.Default.FxNb; i++)
                {
                    this.Cash += this.QuantityOfAssets[Properties.Settings.Default.AssetNb + i] * (PreviousInterestRates[Properties.Settings.Default.AssetNb + i +1]) * (1 / (Properties.Settings.Default.RebalancingNb / Properties.Settings.Default.Maturity));
                }
       
                ArrayList assetPrices = MarketData.getAllPricesAtDate(CurrentDate);

                for (int i = 0; i < this.Delta.Length; i++)
                {
                    //Apply transaction fee when asset is bought
                    double assetPrice = Double.Parse(((String)assetPrices[i]).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    //We convert RBS and citigroup stocks prices:
                    if(i == RBSindex)
                    {
                        assetPrice *= Double.Parse(((String)assetPrices[Properties.Settings.Default.AssetNb + 1]).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture)/ 100.0;
                       
                    }
                    if(i == CitiGroupIndex){
                        assetPrice *= Double.Parse(((String)assetPrices[Properties.Settings.Default.AssetNb + 3]).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture) / 100.0;
                    }

                    
                    double quantityToBuy = (this.Delta[i] - this.QuantityOfAssets[i]) * assetPrice;
                    this.Cash -= quantityToBuy ;
                    this.QuantityOfAssets[i] = this.Delta[i];
                    portfolioValue += this.Delta[i] * assetPrice;
                }
                portfolioValue += this.Cash;
                this.PortfolioValue = portfolioValue;
                this.TrackingError = this.PortfolioValue - this.ProductPrice;
                this.ProfitAndLoss = this.TrackingError + this.InitialCash;
            }
        }
    }
}