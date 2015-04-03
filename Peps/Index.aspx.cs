using ServiceStack.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wrapper;

namespace Peps
{
    public partial class Index : System.Web.UI.Page
    {

        

        double[,] hedgingPreviousStocksPrices;
        double[] hedgeDPreviousStockPrices;

        int RBSindex = 18;
        int citiGroupIndex;

        

        public Portfolio CurrentPortfolio
        {
            get
            {
                object temp = Session["Pf"];
                return temp == null ? null : (Portfolio)temp;
            }
            set { Session["Pf"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {         
                Portfolio temp;
                if (CurrentPortfolio == null)
                {

                    /*
                     * Redis DEMO: get the portefolio from the database 
                     * To comment if redis server not running on localhost:6379
                     */
                    var redisManager = new PooledRedisClientManager("localhost:6379");

                    redisManager.ExecAs<Portfolio>(redisPf =>
                    {
                        temp = redisPf.GetById(1);
                        CurrentPortfolio = new Portfolio(new WrapperClass(), new MarketData());
                        CurrentPortfolio.setDelta(temp.delta);
                        CurrentPortfolio.setMarket(temp.market);
                        CurrentPortfolio.setPrix(temp.prix);
                        CurrentPortfolio.setQuantity(temp.quantity);
                        CurrentPortfolio.setpfvalue(temp.pfvalue);
                        CurrentPortfolio.setPL(temp.profitAndLoss);
                        CurrentPortfolio.setCash(temp.getCash());
                        CurrentPortfolio.setInitialCash(temp.getInitialCash());
                        CurrentPortfolio.index = temp.index;
                        CurrentPortfolio.numberOfStock = temp.numberOfStock;
                    });
                    
                }
                if (CurrentPortfolio == null)
                {
                    CurrentPortfolio = new Portfolio(new WrapperClass(), new MarketData());
                }
                //Init the Display
                //initDisplay();
                CurrentPortfolio.marketData.getAllStockPrices("30","7", "2005","30","0", "2015");
                CurrentPortfolio.save();
                //Compute delta and price at date 0          
                Compute_Price();
                //Display the result of the computation
                displayData();
               
            }
        }


        /* 
         * Pour le hedging 
         * 
         FillHedgingParameters();

                int k = 0;
                hedgeDPreviousStockPrices = new double[previousStocksPrices.GetLength(0) * previousStocksPrices.GetLength(1)];

                for (int i = 0; i < hedgingPreviousStocksPrices.GetLength(0); i++)
                {
                    for (int j = 0; j < hedgingPreviousStocksPrices.GetLength(1); j++)
                    {
                        hedgeDPreviousStockPrices[k++] = hedgingPreviousStocksPrices[i, j];
                    }
                }
                double H = hedgingPreviousStocksPrices.GetLength(0);
                CurrentPortfolio.wrapper.computeHedge(hedgeDPreviousStockPrices, previousInterestRates, stockToFxIndex,
                    Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                    Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, hedgingPreviousStocksPrices.GetLength(0),
                    Properties.Settings.Default.StepFiniteDifference, H);
        //this.PnLDiv.Text = CurrentPortfolio.marketData.getLastCurrencyPrice("USDEUR", );
        */

        public void initDisplay()
        {
            //Color and scale of the chart
            /*
            Chart1.Series.FindByName("ProductPrice").Color = Color.DarkBlue;
            Chart1.Series.FindByName("PortfolioPrice").Color = Color.DarkRed;
            Chart1.ChartAreas[0].AxisY.Maximum = 104;
            Chart1.ChartAreas[0].AxisY.Minimum = 88;
             */
            this.PnLDiv.Text = "0.0983";
            this.PdtValue.Text = Properties.Settings.Default.Nominal.ToString();
            this.PtfValue.Text = "99.31";
        }

        //HERE: 
        //Display logic
        public void displayData()
        {

            this.PtfValue.Text = Math.Round(CurrentPortfolio.wrapper.getPrice(), Properties.Settings.Default.Precision).ToString();
            //this.PtfValue.Text = CurrentPortfolio.wrapper.getPrice().ToString();
            this.IcInterval.Text = Math.Round(CurrentPortfolio.wrapper.getIC(), Properties.Settings.Default.Precision).ToString();
            FillAssetsTable("27", "02", "2015");
            FillCurrenciesTable("2005-11-29", "2005-11-29");
            //Plot product price and portfolio value
            //number of Step
            int nbStep = CurrentPortfolio.prix.Length;
            initDisplay();
            for (int j = 0; j < CurrentPortfolio.index; j++)
            {
                //Chart1.Series.FindByName("ProductPrice").Points.Add(CurrentPortfolio.prix[j]);
                //Chart1.Series.FindByName("PortfolioPrice").Points.Add(CurrentPortfolio.pfvalue[j]);
            }
        }

        //Compute the price and delta of our product at time 0
        //Fill parameters for the wrapper function
        //Store the price and delta IN the portfolio which is persistent
        protected void Compute_Price()
        {
            /*
             * Compute all the parameters
             *  previousStock: historical stock price for calibration
             *  previousInterestRate: previous rate for each currency
             *  stockToFxIndex: stock currency
             */
            double[,] previousStocksPrices;
            double[] oneDpreviousStockPrices;
            double[] previousInterestRates;
            double[] stockToFxIndex;
            stockToFxIndex = new double[Properties.Settings.Default.AssetNb];
            previousStocksPrices = new double[Properties.Settings.Default.VolCalibrationDaysNb + 1, Properties.Settings.Default.AssetNb + Properties.Settings.Default.FxNb];
            previousInterestRates = new double[Properties.Settings.Default.FxNb + Properties.Settings.Default.AssetNb + 1];
            //NO CALL TO YAHOO FINANCE => throw too much request exception
            //GET DATA From Portfolio, it's persistent
            FillPreviousStockPrices(previousStocksPrices, stockToFxIndex);
            FillFxRates(previousStocksPrices);
            FillPreviousInterestRates(previousInterestRates);
            //We convert RBS and citigroup stocks prices:
            for (int j = 0; j < previousStocksPrices.GetLength(0); j++)
            {
                previousStocksPrices[j, RBSindex] = previousStocksPrices[j, RBSindex] * previousStocksPrices[j, Properties.Settings.Default.AssetNb + 1] / 100.0;
                previousStocksPrices[j, citiGroupIndex] = previousStocksPrices[j, citiGroupIndex] * previousStocksPrices[j, Properties.Settings.Default.AssetNb + 3];
            }
            oneDpreviousStockPrices = Utils.Convert2dArrayto1d(previousStocksPrices);


            /*
             * Wrapper Call here
             */ 
            CurrentPortfolio.wrapper.computePrice(oneDpreviousStockPrices, previousInterestRates, stockToFxIndex,
                Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, previousStocksPrices.GetLength(0),
                Properties.Settings.Default.StepFiniteDifference);

            CurrentPortfolio.wrapper.computeDelta(oneDpreviousStockPrices, previousInterestRates, stockToFxIndex,
                Properties.Settings.Default.AssetNb, Properties.Settings.Default.FxNb, Properties.Settings.Default.Maturity,
                Properties.Settings.Default.McSamplesNb, Properties.Settings.Default.TimeSteps, previousStocksPrices.GetLength(0),
                Properties.Settings.Default.StepFiniteDifference);

            /*
             * Store the value in the portfolio
             */ 
                      
        }

        //Compute the price to the next date
        public void Continue_Computation(Object sender, EventArgs e)
        {
            if (CurrentPortfolio.prix != null)
            {
                //CurrentPortfolio.Update();
                displayData();
            }
        }
        
        void FillHedgingParameters(double[,] previousStocksPrices)
        {
            string tmpStockTicker;
            DateTime stocksStartDate = CurrentPortfolio.currentDate.AddMonths(-3);
            DateTime stocksEndDate = CurrentPortfolio.currentDate.AddYears(2);

            int size = 900;
            Dictionary<String, ArrayList> symbolToPricesList = new Dictionary<string, ArrayList>();
            ArrayList tmp;
            foreach (PropertyInfo property in
                typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Length == 12)
                {
                    tmpStockTicker = Properties.Resources.ResourceManager.GetString(property.Name).Split(';')[1];

                    tmp = CurrentPortfolio.marketData.getLastStockPrices(tmpStockTicker, stocksStartDate.Day.ToString(), stocksStartDate.Month.ToString(),
                        stocksStartDate.Year.ToString(), stocksEndDate.Day.ToString(), stocksEndDate.Month.ToString(), stocksEndDate.Year.ToString(),false);
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
            DateTime calibrationStartDate = CurrentPortfolio.currentDate.AddDays(-Properties.Settings.Default.VolCalibrationDaysNb);
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
                        citiGroupIndex = cpt;

                        tmp = CurrentPortfolio.marketData.getLastStockPrices(tmpStockTicker, calibrationStartDate.Day.ToString(), calibrationStartDate.Month.ToString(),
                        calibrationStartDate.Year.ToString(), CurrentPortfolio.currentDate.Day.ToString(), CurrentPortfolio.currentDate.Month.ToString(), CurrentPortfolio.currentDate.Year.ToString(),false);
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

        private void FillFxRates(double[,] previousStocksPrices)
        {
            DateTime calibrationStartDate = CurrentPortfolio.currentDate.AddDays(-Properties.Settings.Default.VolCalibrationDaysNb);
            ArrayList fxPrices;
            int cpt = Properties.Settings.Default.AssetNb;
            foreach (PropertyInfo property in
               typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Substring(0, 2).Equals("Fx"))
                {
                    fxPrices = CurrentPortfolio.marketData.getPreviousCurrencyPrices(property.Name.Substring(2),calibrationStartDate.ToString("u"), CurrentPortfolio.currentDate.ToString("u"));
                    for (int j = 0; j < Math.Min(previousStocksPrices.GetLength(0), fxPrices.Count); j++)
                    {
                        previousStocksPrices[j, cpt] = (double)fxPrices[j];
                    }
                    cpt++;
                }
            }
        }

        private void FillCurrenciesTable(string fxStartDate, string fxEndDate)
        {
            double[] deltaVect = CurrentPortfolio.wrapper.getDelta();
            int cpt = Properties.Settings.Default.AssetNb;
            foreach (PropertyInfo property in
                typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Substring(0, 2).Equals("Fx"))
                {
                    TableRow tr = new TableRow();
                    tr.BackColor = Color.White;
                    TableCell isin = new TableCell();
                    isin.Text = property.Name.Substring(2);
                    tr.Cells.Add(isin);

                    TableCell name = new TableCell();
                    name.Text = Properties.Resources.ResourceManager.GetString(property.Name);
                    tr.Cells.Add(name);

                    TableCell price = new TableCell();
                    price.Text = CurrentPortfolio.marketData.getLastCurrencyPrice(property.Name.Substring(2), fxStartDate, fxEndDate) + "€";
                    // price.Text = "73.73 €";
                    tr.Cells.Add(price);


                    TableCell delta = new TableCell();
                    delta.Text = Math.Round(deltaVect[cpt], Properties.Settings.Default.Precision).ToString();
                    tr.Cells.Add(delta);

                    TableCell totalValue = new TableCell();
                    totalValue.Text = "90.1";
                    tr.Cells.Add(totalValue);

                    stocksTable.Rows.Add(tr);
                    cpt++;
                }
            }
        }
        //month de 00 à 11
        //day et year: normaux
        private void FillAssetsTable(string day, string month, string year)
        {
            double[] deltaVect = CurrentPortfolio.wrapper.getDelta();
            string tmpStockTicker;
            int cpt = 0;
            foreach (PropertyInfo property in
                typeof(Properties.Resources).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (property.Name.Length == 12)
                {
                    //i.e it is an ISIN, TODO: add more robust verification
                    TableRow tr = new TableRow();

                    TableCell isin = new TableCell();
                    isin.Text = property.Name;
                    tr.Cells.Add(isin);

                    TableCell name = new TableCell();
                    name.Text = Properties.Resources.ResourceManager.GetString(property.Name).Split(';')[0];
                    tr.Cells.Add(name);

                    tmpStockTicker = Properties.Resources.ResourceManager.GetString(property.Name).Split(';')[1];
                    TableCell price = new TableCell();
                    //NOT CALL TO YAHOO FINANCE !!!
                    price.Text = CurrentPortfolio.marketData.getStockPrice(tmpStockTicker, day, month, year) + "€";              
                    tr.Cells.Add(price);


                    TableCell delta = new TableCell();

                    delta.Text = Math.Round(deltaVect[cpt], Properties.Settings.Default.Precision).ToString();
                    tr.Cells.Add(delta);

                    TableCell totalValue = new TableCell();
                    totalValue.Text = "90.1";
                    tr.Cells.Add(totalValue);

                    stocksTable.Rows.Add(tr);

                    cpt++;
                }
            }
        }

       

        /*
         *TODO A mettre dans l'onglet de vérification 
         * 
        public void Get_Data(Object sender, EventArgs e)
        {
            CurrentPortfolio.getData();
        }

        public void updateDate(Object sender, EventArgs e)
        {
            String str = Request.Form[DisplayCalendar.UniqueID];
            String[] date = str.Split('/');
            if (date.Length == 3)
            {
                String y = date[2];
                String m = date[0];
                String d = date[1];
                CurrentPortfolio.setDate(Convert.ToInt32(y), Convert.ToInt32(m), Convert.ToInt32(d));
            }
        }

       
        public void Compute_Simu1(Object sender, EventArgs e)
        {
            initDisplay();
            CurrentPortfolio.LoadFromResource(1);
            displayData();
        }
        
        public void Compute_Simu2(Object sender, EventArgs e)
        {
            initDisplay();
            CurrentPortfolio.LoadFromResource(2);
            displayData();
        }


        public void Compute_Simu3(Object sender, EventArgs e)
        {
            initDisplay();
            CurrentPortfolio.LoadFromResource(3);
            displayData();
        }

        

        public void Continue_Simu(Object sender, EventArgs e)
        {
            if (CurrentPortfolio.prix == null) Compute_Simu3(sender, e);
            CurrentPortfolio.ComputeSimulation(10);
            displayData();
        }
        */

    }
}