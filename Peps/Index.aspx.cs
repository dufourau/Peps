using ServiceStack.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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

        public Portfolio CurrentPortfolio
        {
            get
            {
                object temp = Session["Pf"];
                return temp == null ? null : (Portfolio)temp;
            }
            set { Session["Pf"] = value; }
        }

        public string chartData
        {
            get
            {
                return CurrentPortfolio.historyToJSONString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (CurrentPortfolio == null)
                {
                    CurrentPortfolio = Portfolio.find();
                    
                    CurrentPortfolio.MarketData.loadAllStockPrices();
                    CurrentPortfolio.save();
                }
            }
        }

        public void loadComputation(Object sender, EventArgs e)
        {
            CurrentPortfolio.CurrentDate = new DateTime(2005, 11, 30);
            //Init the Display
            initDisplay();
            //Compute delta and price at date 0          
            CurrentPortfolio.compute();
            //Display the result of the computation
            displayData();
            Update.Enabled = true;
        }

        public void computeHedge(Object sender, EventArgs e){
            CurrentPortfolio.computeHedge();
            displayData();
            Update.Enabled = false;
        }

        public void dumpDatabase(Object sender, EventArgs e)
        {
            CurrentPortfolio.MarketData.loadAllStockPrices();
        }


        public void initDisplay()
        {
            InitialCash.Text = "0";
            CashEuro.Text = "0";
            CashDollar.Text = "0";
            CashCHF.Text = "0";
            CashGBP.Text = "0";
            CashYen.Text = "0";     
        }


        //Display logic
        public void displayData()
        {
            InitialCash.Text = Math.Round(CurrentPortfolio.InitialCash, Properties.Settings.Default.Precision).ToString();
            this.PdtValue.Text = Math.Round(CurrentPortfolio.Wrapper.getPrice(), Properties.Settings.Default.Precision).ToString();
            this.PtfValue.Text = Math.Round(CurrentPortfolio.PortfolioValue, Properties.Settings.Default.Precision).ToString();
            this.IcInterval.Text = Math.Round(CurrentPortfolio.Wrapper.getIC(), Properties.Settings.Default.Precision).ToString();
            this.PnLDiv.Text = Math.Round(CurrentPortfolio.ProfitAndLoss, Properties.Settings.Default.Precision).ToString();
            CashEuro.Text = Math.Round(CurrentPortfolio.Cash, Properties.Settings.Default.Precision).ToString();
            FillAssetsTable(CurrentPortfolio.CurrentDate);
            FillCurrenciesTable(CurrentPortfolio.CurrentDate);
            date.Text = "Next Date: " + CurrentPortfolio.CurrentDate.ToShortDateString();
        }

        private void FillCurrenciesTable(DateTime fxStartDate)
        {
            double[] deltaVect = CurrentPortfolio.Wrapper.getDelta();
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
                    price.Text = Math.Round(tmp, Properties.Settings.Default.Precision).ToString() + " €";
                    tr.Cells.Add(price);

                    double deltaTmp = 0;
                    if (deltaVect != null) deltaTmp = deltaVect[cpt];

                    TableCell delta = new TableCell();
                    delta.Text = Math.Round(deltaTmp, Properties.Settings.Default.Precision).ToString();
                    tr.Cells.Add(delta);

                    TableCell totalValue = new TableCell();
                    totalValue.Text = (Math.Round(tmp * deltaTmp, Properties.Settings.Default.Precision)).ToString();
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
                    price.Text = Math.Round(tmp, Properties.Settings.Default.Precision) + " €";
                    tr.Cells.Add(price);

                    TableCell delta = new TableCell();

                    double deltaTmp = 0;
                    if (deltaVect != null) deltaTmp = deltaVect[cpt];

                    delta.Text = Math.Round(deltaTmp, Properties.Settings.Default.Precision).ToString();
                    tr.Cells.Add(delta);

                    TableCell totalValue = new TableCell();



                    totalValue.Text = (Math.Round(tmp * deltaTmp, Properties.Settings.Default.Precision)).ToString();
                    tr.Cells.Add(totalValue);

                    TableCell quantity = new TableCell();
                    quantity.Text = (Math.Round(CurrentPortfolio.QuantityOfAssets[cpt], Properties.Settings.Default.Precision)).ToString();
                    tr.Cells.Add(quantity);

                    stocksTable.Rows.Add(tr);

                    cpt++;
                }
            }
        }

       
        /*
        public void Continue_Simu(Object sender, EventArgs e)
        {
            if (CurrentPortfolio.prix == null) Compute_Simu3(sender, e);
            CurrentPortfolio.ComputeSimulation(10);
            displayData();
        }
        */

    }

}