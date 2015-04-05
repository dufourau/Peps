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

        }

        public Portfolio CurrentPortfolio
        {
            get
            {
                object temp = Session["Pf"];
                return temp == null ? null : (Portfolio)temp;
            }
            set { Session["Pf"] = value; }
        }


        public void load_computation(Object sender, EventArgs e)
        {
            String str = Request.Form[DisplayCalendar.UniqueID];
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
            //Compute delta and price at date 0                     
            CurrentPortfolio.compute(true);          
            //Display the result of the computation
            displayData();

        }

        public void computeHedge(Object sender, EventArgs e)
        {
            CurrentPortfolio.computeHedge();
        }


        //TODO: refactor with the same function in index Index
        public void initDisplay()
        {
            InitialCash.Text = "0";
            CashEuro.Text = "0";
            CashDollar.Text = "0";
            CashCHF.Text = "0";
            CashGBP.Text = "0";
            CashYen.Text = "0";
        }

        //HERE: 
        //Display logic
        public void displayData()
        {
            InitialCash.Text = Math.Round(CurrentPortfolio.InitialCash, Properties.Settings.Default.Precision).ToString();
            this.PdtValue.Text = Math.Round(CurrentPortfolio.Wrapper.getPrice(), Properties.Settings.Default.Precision).ToString();
            this.PtfValue.Text = Math.Round(CurrentPortfolio.Wrapper.getPrice(), Properties.Settings.Default.Precision).ToString();
            this.IcInterval.Text = Math.Round(CurrentPortfolio.Wrapper.getIC(), Properties.Settings.Default.Precision).ToString();
            this.PnLDiv.Text = (100 - Math.Round(CurrentPortfolio.Wrapper.getPrice(), Properties.Settings.Default.Precision)).ToString();
            FillAssetsTable("27", "02", "2015");
            FillCurrenciesTable("2005-11-29", "2005-11-29");

            //for (int j = 0; j < CurrentPortfolio.index; j++)
            //{
            //    Chart1.Series.FindByName("ProductPrice").Points.Add(CurrentPortfolio.prix[j]);
            //    Chart1.Series.FindByName("PortfolioPrice").Points.Add(CurrentPortfolio.pfvalue[j]);
            //}
        }



        private void FillCurrenciesTable(string fxStartDate, string fxEndDate)
        {
            double[] deltaVect = CurrentPortfolio.Wrapper.getDelta();
            int cpt = Properties.Settings.Default.AssetNb;
            string tmp;
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
                    tmp = CurrentPortfolio.MarketData.getLastCurrencyPrice(property.Name.Substring(2), fxStartDate, fxEndDate) + "€";
                    price.Text = tmp + " €";
                    tr.Cells.Add(price);

                    TableCell delta = new TableCell();
                    delta.Text = Math.Round(deltaVect[cpt], Properties.Settings.Default.Precision).ToString();
                    tr.Cells.Add(delta);

                    TableCell totalValue = new TableCell();
                    totalValue.Text = (Math.Round(Double.Parse(tmp, CultureInfo.InvariantCulture) * deltaVect[cpt], Properties.Settings.Default.Precision)).ToString();
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
            string tmp;
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
                    tmp = CurrentPortfolio.MarketData.getStockPrice(tmpStockTicker, day, month, year) + "€";
                    price.Text = tmp + " €";
                    tr.Cells.Add(price);

                    TableCell delta = new TableCell();

                    delta.Text = Math.Round(deltaVect[cpt], Properties.Settings.Default.Precision).ToString();
                    tr.Cells.Add(delta);

                    TableCell totalValue = new TableCell();
                    totalValue.Text = (Math.Round(Double.Parse(tmp, CultureInfo.InvariantCulture) * deltaVect[cpt], Properties.Settings.Default.Precision)).ToString();
                    tr.Cells.Add(totalValue);

                    stocksTable.Rows.Add(tr);

                    cpt++;
                }
            }
        }
    }
}