using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Peps
{
    public partial class Validation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (CurrentPortfolio == null)
                {
                    CurrentPortfolio = Portfolio.find(false);
                    CurrentPortfolio.MarketData.loadAllStockPrices();
                    displayData();
                }
                else
                {
                    CurrentPortfolio.MarketData.loadAllStockPrices();
                    displayData();
                }
            }
        }

        public Portfolio CurrentPortfolio
        {
            get
            {
                object temp = Session["Pf2"];
                return temp == null ? null : (Portfolio)temp;
            }
            set { Session["Pf2"] = value; }
        }


        public void loadComputation(Object sender, EventArgs e)
        {
            String str = Request.Form[DisplayCalendar.UniqueID];
            if (str.Equals(""))
            {
                Error_message.Text = "Please choose a valid date";
                Error_panel.Visible = true;
                return;
            }
            //Init the Display
            String[] date = str.Split('/');
            if (date.Length == 3)
            {
                String y = date[2];
                String m = date[0];
                String d = date[1];
                CurrentPortfolio.CurrentDate = Utils.createDateTime(y,m,d);
            }
            initDisplay();
            CurrentPortfolio.resetHistory();
            //Compute delta and price at date t                
            CurrentPortfolio.compute();
            DateTime initialDate = new DateTime(2005,11,30);
            double nbdays = (CurrentPortfolio.CurrentDate - initialDate).TotalDays;
            double actualizationFactor = Math.Exp((0.0239) * (nbdays / (Properties.Settings.Default.RebalancingNb / Properties.Settings.Default.Maturity)));
            double initialCash = 8.5 * actualizationFactor;
            CurrentPortfolio.InitPortfolio(initialCash, CurrentPortfolio.ProductPrice);
            //Display the result of the computation
            displayData();

        }

        public void computeHedge(Object sender, EventArgs e)
        {
            if (!CurrentPortfolio.Initialized)
            {
                Error_message.Text = "Please load a date first";
                Error_panel.Visible = true;
                return;
            }
            if (CurrentPortfolio.CurrentDate.CompareTo(DateTime.Today.AddDays(-1)) >= 0)
            {
                displayData();
                Error_message.Text = "You cannot go in the future";
                Error_panel.Visible = true;
                return;
            }
            CurrentPortfolio.setInterestRate();
            CurrentPortfolio.computeHedge();
            CurrentPortfolio.compute();
            CurrentPortfolio.save(false);
            displayData();
            
        }

        //TODO: refactor with the same function in index Index
        public void initDisplay()
        {
            InitialCash.Text = "0";
            CashEuro.Text = "0";
        }

        //Load the current model
        public void loadModel1(Object sender, EventArgs e)
        {
            CurrentPortfolio.ModelId = 1;
        }
        public void loadModel2(Object sender, EventArgs e)
        {
            CurrentPortfolio.ModelId = 2;
        }
        public void loadModel3(Object sender, EventArgs e)
        {
            CurrentPortfolio.ModelId = 3;
        }
        public void loadModel4(Object sender, EventArgs e)
        {
            CurrentPortfolio.ModelId = 4;
        }



        //Display logic
        public void displayData()
        {
            if (CurrentPortfolio == null || !CurrentPortfolio.Initialized) return;
            Error_panel.Visible = false;
            InitialCash.Text = Math.Round(CurrentPortfolio.InitialCash, Properties.Settings.Default.Precision).ToString();
            this.PdtValue.Text = Math.Round(CurrentPortfolio.ProductPrice, Properties.Settings.Default.Precision).ToString();
            this.PtfValue.Text = Math.Round(CurrentPortfolio.PortfolioValue, Properties.Settings.Default.Precision).ToString();
            this.IcInterval.Text = Math.Round(CurrentPortfolio.Wrapper.getIC(), Properties.Settings.Default.Precision).ToString();
            this.PnLDiv.Text = Math.Round(CurrentPortfolio.ProfitAndLoss, Properties.Settings.Default.Precision).ToString();
            CashEuro.Text = Math.Round(CurrentPortfolio.Cash, Properties.Settings.Default.Precision).ToString();
            FillAssetsTable(CurrentPortfolio.CurrentDate);
            FillCurrenciesTable(CurrentPortfolio.CurrentDate);
            date.Text = "Current Date: " + CurrentPortfolio.CurrentDate.ToShortDateString();
        }

        public string chartData {
            get
            {
                return CurrentPortfolio.historyToJSONString();
            }
        }



        private void FillCurrenciesTable(DateTime fxStartDate)
        {
            double[] deltaVect = CurrentPortfolio.Delta;
            int cpt = Properties.Settings.Default.AssetNb;
            double tmp;
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
                    tmp = CurrentPortfolio.MarketData.getPrice(property.Name.Substring(2), fxStartDate);
                    price.Text = Math.Round(tmp, Properties.Settings.Default.Precision) + " €";
                    tr.Cells.Add(price);

                    TableCell delta = new TableCell();
                    delta.Text = Math.Round(deltaVect[cpt], Properties.Settings.Default.Precision).ToString();
                    tr.Cells.Add(delta);

                    TableCell totalValue = new TableCell();
                    totalValue.Text = (Math.Round(tmp * deltaVect[cpt], Properties.Settings.Default.Precision)).ToString();
                    tr.Cells.Add(totalValue);

                    TableCell quantity = new TableCell();
                    quantity.Text = (Math.Round(CurrentPortfolio.QuantityOfAssets[cpt], Properties.Settings.Default.Precision)).ToString();
                    tr.Cells.Add(quantity);


                    stocksTable.Rows.Add(tr);
                    cpt++;
                }
            }
        }
        //month de 00 à 11
        //day et year: normaux
        private void FillAssetsTable(DateTime date)
        {
            double[] deltaVect = CurrentPortfolio.Delta;
            string tmpStockTicker;
            int cpt = 0;
            double tmp;
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
                    tmp = CurrentPortfolio.MarketData.getPrice(tmpStockTicker, date);
                    price.Text = tmp + " €";
                    tr.Cells.Add(price);

                    TableCell delta = new TableCell();

                    delta.Text = Math.Round(deltaVect[cpt], Properties.Settings.Default.Precision).ToString();
                    tr.Cells.Add(delta);

                    TableCell totalValue = new TableCell();
                    totalValue.Text = (Math.Round(tmp * deltaVect[cpt], Properties.Settings.Default.Precision)).ToString();
                    tr.Cells.Add(totalValue);

                    TableCell quantity = new TableCell();
                    quantity.Text = (Math.Round(CurrentPortfolio.QuantityOfAssets[cpt], Properties.Settings.Default.Precision)).ToString();
                    tr.Cells.Add(quantity);


                    stocksTable.Rows.Add(tr);

                    cpt++;
                }
            }
        }
    }
}