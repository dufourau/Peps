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

        //Historical data     
        public double[][] market { get; set; }
        public double[][] data;
        public double[][] rates;
        public double[] currentRates;
        public List<String> dates;
        
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
            this.NumberOfAsset = Properties.Settings.Default.AssetNb;
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
            this.CurrentDate.AddDays(1);
        }

        public void copyCurrentRate(int index)
        {
            for(int i = 0 ; i < rates[rates.Length - 2 - index].Length ; i++){
                currentRates[i] = rates[rates.Length - 2 - index][i];
            }
        }

        internal void compute()
        {
            initComputationParameters();
            //We convert RBS and citigroup stocks prices:
            for (int j = 0; j < PreviousStocksPrices.GetLength(0); j++)
            {
                PreviousStocksPrices[j, RBSindex] = PreviousStocksPrices[j, RBSindex] * PreviousStocksPrices[j,Properties.Settings.Default.AssetNb + 1] / 100.0;
                PreviousStocksPrices[j, CitiGroupIndex] = PreviousStocksPrices[j, CitiGroupIndex] * PreviousStocksPrices[j,Properties.Settings.Default.AssetNb + 3];
            }

            performComputations();
        }

        private void performComputations()
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

        private void initComputationParameters()
        {
            StockToFxIndex = new double[Properties.Settings.Default.AssetNb];
            PreviousStocksPrices = new double[Properties.Settings.Default.VolCalibrationDaysNb + 1, Properties.Settings.Default.AssetNb + Properties.Settings.Default.FxNb];
            PreviousInterestRates = new double[Properties.Settings.Default.FxNb + Properties.Settings.Default.AssetNb + 1];

            //NO CALL TO YAHOO FINANCE => throw too much request exception
            //GET DATA From Portfolio, it's persistent
            FillPreviousStockPrices(PreviousStocksPrices, StockToFxIndex);
            FillFxRates(PreviousStocksPrices);
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

        //TO DO get real rate for all date
        private void FillPreviousInterestRates(double[] previousInterestRates)
        {
            previousInterestRates[0] = 0.02;
            for (int i = 1; i < Properties.Settings.Default.AssetNb; i++) previousInterestRates[i] = 0;
            previousInterestRates[Properties.Settings.Default.AssetNb + 1] = 0.0075;
            previousInterestRates[Properties.Settings.Default.AssetNb + 2] = 0.0475;
            previousInterestRates[Properties.Settings.Default.AssetNb + 3] = 0.0004;
            previousInterestRates[Properties.Settings.Default.AssetNb + 4] = 0.0407;
        }

        private void FillPreviousStockPrices(double[,] previousStocksPrices, double[] stockToFxIndex)
        {
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

                    tmp = MarketData.getLastStockPricesFromWeb(tmpStockTicker, calibrationStartDate.Day.ToString(), calibrationStartDate.Month.ToString(),
                    calibrationStartDate.Year.ToString(), CurrentDate.Day.ToString(), CurrentDate.Month.ToString(), CurrentDate.Year.ToString());
                    if (tmp != null)
                    {
                        symbolToPricesList.Add(property.Name, tmp);
                        size = Math.Min((symbolToPricesList[property.Name]).Count, size);
                    }


                    cpt++;
                }
            }

            cpt = 0;
            foreach (KeyValuePair<String, ArrayList> entry in symbolToPricesList)
            {
                for (int i = 0; i < size; i++)
                {
                    previousStocksPrices[i, cpt] = (double)entry.Value[i];
                    SetStockToFxList(stockToFxIndex, entry.Key, cpt);
                    if (previousStocksPrices[i, cpt] < 0.01) throw new Exception();
                }
                cpt++;
            }
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
        private void FillFxRates(double[,] previousStocksPrices)
        {
            DateTime calibrationStartDate = CurrentDate.AddDays(-Properties.Settings.Default.VolCalibrationDaysNb);
            ArrayList fxPrices;
            int cpt = Properties.Settings.Default.AssetNb;
            foreach (PropertyInfo property in
               typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Substring(0, 2).Equals("Fx"))
                {
                    fxPrices = MarketData.getPreviousCurrencyPricesFromWeb(property.Name.Substring(2), calibrationStartDate.ToString("u"), CurrentDate.ToString("u"));
                    for (int j = 0; j < Math.Min(previousStocksPrices.GetLength(0), fxPrices.Count); j++)
                    {
                        previousStocksPrices[j, cpt] = (double)fxPrices[j];
                    }
                    cpt++;
                }
            }
        }

        public void ComputeSimulation(int numberOfIteration = 1)
        {
            for (int k = 0; k < numberOfIteration; k++)
            {
                this.goToNextDate();
                double portfolioValue = 0;
                // Actualize Cash and Initial Cash
                double actualizationFactor = Math.Exp((currentRates[0] / 100) * (1 / (Properties.Settings.Default.RebalancingNb / Properties.Settings.Default.Maturity)));
                this.Cash *= actualizationFactor;
                this.InitialCash *= actualizationFactor;
                // Handle the currency interest
                for (int i = 0; i < Properties.Settings.Default.FxNb; i++)
                {
                    portfolioValue += this.QuantityOfAssets[Properties.Settings.Default.AssetNb - 1 + i] * (currentRates[i + 1] / 100) * (1 / (Properties.Settings.Default.AssetNb / Properties.Settings.Default.Maturity));
                }
       
                ArrayList assetPrices = MarketData.getAllPricesAtDate(CurrentDate);

                for (int i = 0; i < this.Delta.Length; i++)
                {
                    //Apply transaction fee when asset is bought
                    double assetPrice = ((double)assetPrices[i]);
                    double quantityToBuy = (this.Delta[i] - this.QuantityOfAssets[i]) * assetPrice;
                    this.Cash -= quantityToBuy + Math.Abs(quantityToBuy) * this.TransactionFees;
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