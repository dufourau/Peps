using ServiceStack.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using TeamDev.Redis;
using Wrapper;

namespace Peps
{
    public class Portfolio
    {
        private static long CurrentId = 0;

        public long Id { get; set; }
        public long ModelId { get; set; }
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
        public double[] PreviousInterestRates { get; set; }
        double[] StockToFxIndex{ get; set; }
        int RBSindex { get; set; }       
        int CitiGroupIndex { get; set; }

        double[,] hedgingPreviousStocksPrices { get; set; }
        double[] hedgeDPreviousStockPrices { get; set; }

        public void save(){
            /*
            * Redis DEMO: save the portefolio to the database
            */
            var redisManager = new PooledRedisClientManager(Properties.Settings.Default.RedisPassword + "@" + Properties.Settings.Default.RedisDatabaseURL + ":" + Properties.Settings.Default.RedisPort);
            redisManager.ExecAs<Portfolio>(redisPf => {
                using (var redis = new RedisClient(Properties.Settings.Default.RedisDatabaseURL, Properties.Settings.Default.RedisPort, Properties.Settings.Default.RedisPassword))
                {
                    var pf = this;
                    pf.Id = 1;
                    redisPf.Store(pf);
                }
            });
        }

        public Portfolio(WrapperClass wrapper, MarketData marketData)
        {
            this.ModelId = 1;
            this.Wrapper = wrapper;
            this.MarketData = marketData;
            //Cash in EUR
            this.InitialCash = 0;
            this.Cash = 0;
            this.RBSindex = 18;
            this.NumberOfAsset = Properties.Settings.Default.AssetNb + Properties.Settings.Default.FxNb;
            this.QuantityOfAssets = new double[NumberOfAsset];
            this.ProductPriceHistory = new List<double>();
            this.PortfolioValueHistory = new List<double>();
            
        }

        public void InitPortfolio(double InitialCash, double Cash){
                this.InitialCash = InitialCash;
                this.Cash = Cash;
                this.PortfolioValue = Cash;
                this.ProfitAndLoss = InitialCash;
                for (int i = 0; i < NumberOfAsset; i++) { QuantityOfAssets[i] = 0; }
        }

        public static Portfolio find()
        {
            var redisManager = new PooledRedisClientManager(Properties.Settings.Default.RedisPassword + "@" + Properties.Settings.Default.RedisDatabaseURL + ":" + Properties.Settings.Default.RedisPort);

            Portfolio portfolio = null;

            redisManager.ExecAs<Portfolio>(redis =>
            {
                portfolio = redis.GetById(1);
            });

            if (portfolio == null)
            {
                return new Portfolio(new WrapperClass(), new MarketData());
            }
            else
            {
            portfolio.MarketData = new MarketData();
            portfolio.Wrapper = new WrapperClass();
            }
            return portfolio;
        }

        public void goToNextDate()
        {
            this.CurrentDate = MarketData.getNextDate(this.CurrentDate);
        }

        internal void compute()
        {
            DateTime initialDate = new DateTime(2005, 11, 30);
            DateTime finalDate = new DateTime(2015,11,30);
            //t=0
            if (initialDate.CompareTo(CurrentDate) == 0)
            {
                initComputationParameters();
                //We convert RBS and citigroup stocks prices:
                for (int j = 0; j < PreviousStocksPrices.GetLength(0); j++)
                {
                    PreviousStocksPrices[j, RBSindex] = PreviousStocksPrices[j, RBSindex] * PreviousStocksPrices[j, Properties.Settings.Default.AssetNb + 1] / 100.0;
                    PreviousStocksPrices[j, CitiGroupIndex] = PreviousStocksPrices[j, CitiGroupIndex] * PreviousStocksPrices[j, Properties.Settings.Default.AssetNb + 3];
                }
                performInitialComputations();
                InitPortfolio(Properties.Settings.Default.Nominal - this.Wrapper.getPrice(), this.Wrapper.getPrice());
                
            }
            else
            {
                double nbdays = (CurrentDate - initialDate).TotalDays;
                double totalnbdays = (finalDate - initialDate).TotalDays;
                double t = nbdays / (totalnbdays/10);            
                initComputationParameters();
                //We convert RBS and citigroup stocks prices:
                for (int j = 0; j < PreviousStocksPrices.GetLength(0); j++)
                {
                    PreviousStocksPrices[j, RBSindex] = PreviousStocksPrices[j, RBSindex] * PreviousStocksPrices[j, Properties.Settings.Default.AssetNb + 1] / 100.0;
                    PreviousStocksPrices[j, CitiGroupIndex] = PreviousStocksPrices[j, CitiGroupIndex] * PreviousStocksPrices[j, Properties.Settings.Default.AssetNb + 3];
                }
                performComputations(t, Utils.Convert2dArrayto1d(computePast(t)));
                //TO DO: 
                double actualizationFactor = Math.Exp((PreviousInterestRates[0]) * (1 / (Properties.Settings.Default.RebalancingNb / Properties.Settings.Default.Maturity)));
               
            }
            
            this.save();
        }

        double handleRBS_C(ArrayList prices, int index){
            double assetPrice = (double)prices[index];
            if (index == RBSindex)
            {
                assetPrice *= ((double)prices[Properties.Settings.Default.AssetNb + 1]) / 100.0;

            }
            if (index == CitiGroupIndex)
            {
                assetPrice *= ((double)prices[Properties.Settings.Default.AssetNb + 3]) / 100.0;
            }
            return assetPrice;
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
                    date = Utils.GetWorkingWeekday(date);
                    ArrayList prices = MarketData.getAllPricesAtDate(date);
                    for (int j = 0; j < prices.Count; j++)
                    {
                                        
                        past[cpt, j] = handleRBS_C(prices, j);
                    }
                    cpt++;
                }
            }
            else
            {
                 past = new double[(int)Math.Floor(t)+2,this.NumberOfAsset];
                 int cpt = 0;
                 for (int i = 2005; i <= 2005 + (int)Math.Floor(t); i++)
                 {
                     DateTime date = new DateTime(i, 11,30);
                     date = Utils.GetWorkingWeekday(date);
                     ArrayList prices = MarketData.getAllPricesAtDate(date);
                     for (int j = 0; j < prices.Count; j++)
                     {

                         past[cpt, j] = handleRBS_C(prices, j);
                     }
                     cpt++;
                 }
                 ArrayList lastPrices = MarketData.getAllPricesAtDate(CurrentDate);
                 for (int j = 0; j < lastPrices.Count; j++)
                 {
                    past[cpt, j] = handleRBS_C(lastPrices, j);
                 }
            }
            return past;
        }

        private void performInitialComputations()
        {
            double [] oneDpreviousStocksPrices = Utils.Convert2dArrayto1d(PreviousStocksPrices);
            if(ModelId == 1){
                Wrapper.computePrice(oneDpreviousStocksPrices, PreviousInterestRates, StockToFxIndex,
                Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, PreviousStocksPrices.GetLength(0),
                Properties.Settings.Default.StepFiniteDifference);
            }
            else if(ModelId == 2)
            {
                double[] histEur = new double[47];
                double[] histChf = new double[47];
                double[] histGbp = new double[47];
                double[] histJpy = new double[47];
                double[] histUsd = new double[47];

                int offset = this.MarketData.rates.Length;
                int index = this.MarketData.dates.ToList().IndexOf(this.CurrentDate);

                for (int i = 0; i < 47; i++)
                {
                    histEur[i] = this.MarketData.rates[offset - index - i][0] / 100;
                    histChf[i] = this.MarketData.rates[offset - index - i][3] / 100;
                    histGbp[i] = this.MarketData.rates[offset - index - i][1] / 100;
                    histJpy[i] = this.MarketData.rates[offset - index - i][4] / 100;
                    histUsd[i] = this.MarketData.rates[offset - index - i][2] / 100;

                }
                Wrapper.computePriceStoch( oneDpreviousStocksPrices, PreviousInterestRates, StockToFxIndex,
                    Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                    Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, PreviousStocksPrices.GetLength(0),
                    Properties.Settings.Default.StepFiniteDifference, histEur, histChf, histGbp, histJpy, histUsd);
            }
            else if (ModelId == 3)
            {

            }
            else if (ModelId == 3)
            {

            }
            
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
            if (ModelId == 1)
            {
                Wrapper.computePrice(t,oneDPast,oneDpreviousStocksPrices, PreviousInterestRates, StockToFxIndex,
                    Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                    Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, PreviousStocksPrices.GetLength(0),
                    Properties.Settings.Default.StepFiniteDifference);
            }
            else if(ModelId == 2)
            {
                double[] histEur = new double[47];
                double[] histChf = new double[47];
                double[] histGbp = new double[47];
                double[] histJpy = new double[47];
                double[] histUsd = new double[47];
        
                int offset = this.MarketData.rates.Length;
                int index = this.MarketData.dates.ToList().IndexOf(this.CurrentDate);
        
                for(int i = 0 ; i<47; i++){
                    histEur[i] = this.MarketData.rates[offset - index-i][0]/100;
                    histChf[i] = this.MarketData.rates[offset - index-i][3]/100;
                    histGbp[i] = this.MarketData.rates[offset - index-i][1]/100;
                    histJpy[i] = this.MarketData.rates[offset - index-i][4]/100;
                    histUsd[i] = this.MarketData.rates[offset - index-i][2]/100;
                    
                }
                //Wrapper.computePriceStoch(t,oneDpreviousStocksPrices, PreviousInterestRates, StockToFxIndex,
                //    Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                //    Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, PreviousStocksPrices.GetLength(0),
                //    Properties.Settings.Default.StepFiniteDifference,histEur,histChf,histGbp, histJpy, histUsd);

            }
            else if (ModelId == 3)
            {

            }
            else if (ModelId == 3)
            {

            }
            this.ProductPrice = Wrapper.getPrice();

            Wrapper.computeDelta(t,oneDPast,oneDpreviousStocksPrices, PreviousInterestRates, StockToFxIndex,
                Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, PreviousStocksPrices.GetLength(0),
                Properties.Settings.Default.StepFiniteDifference);
            this.Delta = Wrapper.getDelta();
        }

        private void initComputationParameters()
        {
            StockToFxIndex = new double[Properties.Settings.Default.AssetNb];
            PreviousInterestRates = new double[Properties.Settings.Default.FxNb + Properties.Settings.Default.AssetNb + 1];

            
            PreviousStocksPrices = FillPreviousStockPrices( StockToFxIndex);
            FillFxRates(PreviousStocksPrices);
            FillPreviousInterestRates(PreviousInterestRates);
        }

        private void FillHedgingParameters(double[,] previousStocksPrices)
        {
            string tmpStockTicker;
            DateTime stocksStartDate = CurrentDate.AddMonths(-3);
            DateTime stocksEndDate = CurrentDate.AddYears(2);

            int size = 900;
            Dictionary<String, List<double>> symbolToPricesList = new Dictionary<string, List<double>>();
            List<double> tmp;
            foreach (PropertyInfo property in
                typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Length == 12)
                {
                    tmpStockTicker = Properties.Resources.ResourceManager.GetString(property.Name).Split(';')[1];

                    tmp = this.MarketData.getPricesRange(tmpStockTicker, stocksStartDate, stocksEndDate);
                    if (tmp != null)
                    {
                        symbolToPricesList.Add(property.Name, tmp);
                        size = Math.Min((symbolToPricesList[property.Name]).Count, size);
                    }
                }
            }
            int cpt = 0;
            hedgingPreviousStocksPrices = new double[size, Properties.Settings.Default.AssetNb + Properties.Settings.Default.FxNb];
            foreach (KeyValuePair<String, List<double>> entry in symbolToPricesList)
            {
                for (int i = 0; i < size; i++)
                {
                    hedgingPreviousStocksPrices[i, cpt] = (double)entry.Value[i];
                    if (previousStocksPrices[i, cpt] < 0.01) throw new Exception();
                }
                cpt++;
            }
        }

        public void setInterestRate(){
            PreviousInterestRates = new double[Properties.Settings.Default.FxNb + Properties.Settings.Default.AssetNb + 1];
            FillPreviousInterestRates(this.PreviousInterestRates);
        }
        private void FillPreviousInterestRates(double[] previousInterestRates)
        {
            int offset = this.MarketData.rates.Length;
            int index = this.MarketData.dates.ToList().IndexOf(this.CurrentDate);
            previousInterestRates[0] = this.MarketData.rates[offset - index][0] / 100;
            for (int i = 1; i < Properties.Settings.Default.AssetNb; i++) previousInterestRates[i] = 0;
            previousInterestRates[Properties.Settings.Default.AssetNb + 1] = this.MarketData.rates[offset - index][3] / 100;
            previousInterestRates[Properties.Settings.Default.AssetNb + 2] = this.MarketData.rates[offset - index][1] / 100;
            previousInterestRates[Properties.Settings.Default.AssetNb + 3] = this.MarketData.rates[offset - index][4] / 100;
            previousInterestRates[Properties.Settings.Default.AssetNb + 4] = this.MarketData.rates[offset - index][2] / 100;
        
        }

      

        private double[,] FillPreviousStockPrices(double[] stockToFxIndex)
        {
            double[,] previousStocksPrices;
            string tmpStockTicker;
            DateTime calibrationStartDate = CurrentDate.AddDays(-Properties.Settings.Default.VolCalibrationDaysNb);
            calibrationStartDate = Utils.GetWorkingWeekday(calibrationStartDate);
            int size = Properties.Settings.Default.VolCalibrationDaysNb + 1;
            Dictionary<String, List<double>> symbolToPricesList = new Dictionary<string, List<double>>();
            List<double> tmp;
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

                    tmp = MarketData.getPricesRange(tmpStockTicker, calibrationStartDate, CurrentDate);
                    
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
            foreach (KeyValuePair<String, List<double>> entry in symbolToPricesList)
            {
                for (int i = 0; i < size; i++)
                {
                    previousStocksPrices[i, cpt] = entry.Value[i];
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
        private void FillFxRates(double[,] previousStocksPrices)
        {
            DateTime calibrationStartDate = CurrentDate.AddDays(-Properties.Settings.Default.VolCalibrationDaysNb);
            calibrationStartDate = Utils.GetWorkingWeekday(calibrationStartDate);
            List<double> fxPrices;
            int cpt = Properties.Settings.Default.AssetNb;
            foreach (PropertyInfo property in
               typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Substring(0, 2).Equals("Fx"))
                {
                    fxPrices = MarketData.getPricesRange(property.Name.Substring(2), calibrationStartDate, CurrentDate);  
                        
                    for (int j = 0; j < Math.Min(previousStocksPrices.GetLength(0), fxPrices.Count); j++)
                    {
                        previousStocksPrices[j, cpt] = fxPrices[j];
                        //previousStocksPrices[j, cpt] = (double)fxPrices[j];
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
                    double assetPrice = (double) assetPrices[i];
                    //We convert RBS and citigroup stocks prices:
                    if(i == RBSindex)
                    {
                        assetPrice *= ((double) assetPrices[Properties.Settings.Default.AssetNb + 1])/ 100.0;
                       
                    }
                    if(i == CitiGroupIndex){
                        assetPrice *= ((double) assetPrices[Properties.Settings.Default.AssetNb + 3]);
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
                PortfolioValueHistory.Add(PortfolioValue);
                ProductPriceHistory.Add(ProductPrice);
            }
        }
        
        private class MorrisData
        {
            public string date { get; set; }
            public double productPrice { get; set; }
            public double portfolioValue { get; set; }
        }

        public string historyToJSONString() {
            var array = new List<MorrisData>();
            List<DateTime> datesList = MarketData.dates.ToList();
            int firstIndex = datesList.IndexOf(CurrentDate);
            for (int i = 0; i < PortfolioValueHistory.Count; i++)
            {
                array.Add(new MorrisData
                {
                    date = datesList[firstIndex + i].ToString(MarketData.dateFormat),
                    productPrice = ProductPriceHistory[i],
                    portfolioValue = PortfolioValueHistory[i]
                });
            }
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(array);
        }

        internal void resetHistory()
        {
            this.ProductPriceHistory = new List<double>();
            this.PortfolioValueHistory = new List<double>();
        }
    }
}