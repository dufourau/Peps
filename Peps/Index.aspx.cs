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
                //if (CurrentPortfolio == null)
                //{
                //    /*
                //     * Redis DEMO: get the portefolio from the database 
                //     * To comment if redis server not running on localhost:6379
                //     */
                //    CurrentPortfolio = new Portfolio(new WrapperClass(), new MarketData(), 1);
                //    CurrentPortfolio.MarketData.getAllStockPrices("30", "7", "2005", "30", "0", "2015");
                //    CurrentPortfolio.save();
                //}

                if (CurrentPortfolio == null)
                {
                    CurrentPortfolio = new Portfolio(new WrapperClass(), new MarketData(), new DateTime(2005, 11, 30));
                    CurrentPortfolio.MarketData.getAllStockPrices("30", "7", "2005", "30", "0", "2015");
                    CurrentPortfolio.save();
                }
                //Init the Display
                //initDisplay();
               
                //Compute delta and price at date 0          
               
                CurrentPortfolio.compute();
                //Display the result of the computation
                //displayData();
               
            }
        }

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

            this.PtfValue.Text = Math.Round(CurrentPortfolio.Wrapper.getPrice(), Properties.Settings.Default.Precision).ToString();
            //this.PtfValue.Text = CurrentPortfolio.wrapper.getPrice().ToString();
            this.IcInterval.Text = Math.Round(CurrentPortfolio.Wrapper.getIC(), Properties.Settings.Default.Precision).ToString();
            FillAssetsTable("27", "02", "2015");
            FillCurrenciesTable("2005-11-29", "2005-11-29");
            //Plot product price and portfolio value
            //number of Step
            int nbStep = CurrentPortfolio.ProductPrice.Length;
            initDisplay();
            for (int j = 0; j < CurrentPortfolio.index; j++)
            {
                //Chart1.Series.FindByName("ProductPrice").Points.Add(CurrentPortfolio.prix[j]);
                //Chart1.Series.FindByName("PortfolioPrice").Points.Add(CurrentPortfolio.pfvalue[j]);
            }
        }

        //Compute the price to the next date
        public void Continue_Computation(Object sender, EventArgs e)
        {
            if (CurrentPortfolio.ProductPrice != null)
            {
                //CurrentPortfolio.Update();
                displayData();
            }
        }
        
     

        private void FillCurrenciesTable(string fxStartDate, string fxEndDate)
        {
            double[] deltaVect = CurrentPortfolio.Wrapper.getDelta();
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
                    price.Text = CurrentPortfolio.MarketData.getLastCurrencyPrice(property.Name.Substring(2), fxStartDate, fxEndDate) + "€";
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
            double[] deltaVect = CurrentPortfolio.Wrapper.getDelta();
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
                    price.Text = CurrentPortfolio.MarketData.getStockPrice(tmpStockTicker, day, month, year) + "€";              
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