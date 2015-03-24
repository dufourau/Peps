using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
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

        protected void Page_Load(object sender, EventArgs e)
        { 
            if (CurrentPortfolio == null)
            {
                CurrentPortfolio = new Portfolio(new WrapperClass(), new MarketData());
            }
        }

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

        //Cette fonction calcule le prix à la date spécifié par l'utilisateur
        //Après ça on calcule le portefeuille en rebalançant tous les jours 
        protected void Compute_Price(Object sender, EventArgs e)
        {
            CurrentPortfolio.loadFromComputations();
            displayData();
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

        public void initDisplay()
        {
            //Color and scale of the chart
            Chart1.Series.FindByName("ProductPrice").Color = Color.DarkBlue;
            Chart1.Series.FindByName("PortfolioPrice").Color = Color.DarkRed;
            Chart1.ChartAreas[0].AxisY.Maximum = 104;
            Chart1.ChartAreas[0].AxisY.Minimum = 88;
        }


        public void displayData()
        {
            deltaTable.Rows.Clear();
            assetTable.Rows.Clear();

            prixLabel.Text = CurrentPortfolio.getCurrentPrice().ToString();
            for (int i = 0; i < CurrentPortfolio.numberOfStock; i++)
            {
                TableRow deltaRow = new TableRow();
                TableCell quantityToBuy = new TableCell();
                quantityToBuy.Text = CurrentPortfolio.getCurrentDelta()[i].ToString();
                deltaRow.Cells.Add(quantityToBuy);
                TableCell marketValue = new TableCell();
                marketValue.Text = CurrentPortfolio.getCurrentMarketValues()[i].ToString();
                deltaRow.Cells.Add(marketValue);
                deltaTable.Rows.Add(deltaRow);
                TableRow portfolioRow = new TableRow();
                TableCell quantityAlreadyBought = new TableCell();
                quantityAlreadyBought.Text = CurrentPortfolio.getCurrentQuantities()[i].ToString();
                portfolioRow.Cells.Add(quantityAlreadyBought);
                TableCell secondColumnPortfolio = new TableCell();
                //Add the name of the asset
                secondColumnPortfolio.Text = CurrentPortfolio.symbols[i];
                portfolioRow.Cells.Add(secondColumnPortfolio);

                assetTable.Rows.Add(portfolioRow);
            }

            cashLabel.Text = CurrentPortfolio.getCash().ToString();
            vpLabel.Text = CurrentPortfolio.getCurrentPortfolioValue().ToString();
            teLabel.Text = CurrentPortfolio.trackingError.ToString();
            plLabel.Text = CurrentPortfolio.getCurrentProfitAndLoss().ToString();


            //Plot product price and portfolio value
            //number of Step
            int nbStep = CurrentPortfolio.prix.Length;
            initDisplay();
            for (int j = 1; j < CurrentPortfolio.index; j++)
            {
                Chart1.Series.FindByName("ProductPrice").Points.Add(CurrentPortfolio.prix[j]);
                Chart1.Series.FindByName("PortfolioPrice").Points.Add(CurrentPortfolio.pfvalue[j]);
            }
        }

        public void Continue_Simu(Object sender, EventArgs e)
        {
            if (CurrentPortfolio.prix == null) Compute_Simu3(sender, e);
            CurrentPortfolio.ComputeSimulation(1);
            displayData();
        }
    }
}