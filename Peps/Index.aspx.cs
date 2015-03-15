using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Wrapper;

namespace Peps
{
    public partial class Index : System.Web.UI.Page
    {
        static WrapperClass wrapper;
        //use to store data from file
        static double[][] delta;
        static double[] prix;
        static double[][] market;
        static int index = 0;
        static double Cash = 0;
        static double InitialCash;
        protected void Page_Load(object sender, EventArgs e)
        {
            wrapper = new WrapperClass();
            
        }
        protected void Compute_Price(Object sender, EventArgs e)
        {
                deltaTable.Rows.Clear();
                assetTable.Rows.Clear();
                wrapper.computePrice();
                wrapper.computeDelta();
                wrapper.computeHedge();

                prixLabel.Text = wrapper.getPrice().ToString();
                icLabel.Text = wrapper.getIC().ToString();
                teLabel.Text = wrapper.getPL().ToString();
                double[] delta = wrapper.getDelta();
                double[] deltaIC = wrapper.getDeltaIC();
                
                for (int i = 0; i < delta.Length;i++ )
                {
                    TableRow tr = new TableRow();
                    TableCell tc1 = new TableCell();
                    tc1.Text = delta[i].ToString();
                    tr.Cells.Add(tc1);                 
                    deltaTable.Rows.Add(tr);
                }

        
        }

        private double[][] parseFileToMatrix(String file)
        {
             String[] lines = file.Split('\n').Where(x => x != "" && x != null).ToArray();
             double[][] parsed = new double[lines.Length][];
             for (int i = 0; i < lines.Length; i++)
             {
                String[] items = lines[i].Trim().Split(' ');
                parsed[i] = new double[items.Length];
                for (int j = 0; j < items.Length; j++)
                {
                    parsed[i][j] = double.Parse(items[j], System.Globalization.CultureInfo.InvariantCulture);
                }
             }
             return parsed;

        }

       private double[] parseFileToArray(String file)
        {
            String[] lines = file.Split('\n').Where(x => x != "" && x != null).ToArray();
            double[] parsed = new double[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                parsed[i] = double.Parse(lines[i], System.Globalization.CultureInfo.InvariantCulture);
            }
            return parsed;
        }
        public void ComputeSimulation(int idSimulation)
        {
            index = 0;
            deltaTable.Rows.Clear();
            assetTable.Rows.Clear();
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

            delta = parseFileToMatrix(deltaSimu);
            prix = parseFileToArray(prixSimu);
            market = parseFileToMatrix(marketSimu);

            prixLabel.Text = prix[index].ToString();
            //Initial benefit made from the sell of our product
            InitialCash = 100 - prix[index];
            //We use the price of our product for hedging
            Cash = prix[index];
            for (int i = 0; i < delta[index].Length - 1; i++)
            {
                TableRow deltaRow = new TableRow();
                TableCell quantityToBuy = new TableCell();
                quantityToBuy.Text = delta[index+1][i].ToString();
                deltaRow.Cells.Add(quantityToBuy);
                deltaTable.Rows.Add(deltaRow);
                TableRow portfolioRow = new TableRow();
                TableCell quantityAlreadyBought = new TableCell();
                quantityAlreadyBought.Text = delta[index][i].ToString();
                portfolioRow.Cells.Add(quantityAlreadyBought);
                TableCell secondColumnPortfolio = new TableCell();
                secondColumnPortfolio.Text = market[index][i].ToString();
                portfolioRow.Cells.Add(secondColumnPortfolio);

                assetTable.Rows.Add(portfolioRow);
                //We buy a certain quantity of action using delta hedging
                Cash -=delta[index][i] * market[index][i];
            }
            cashLabel.Text = Cash.ToString();
            vpLabel.Text = prix[index].ToString();
            //Compute the tracking error
            teLabel.Text = "0";
            //Compute the profit and Loss
            double pL = InitialCash;
            plLabel.Text = pL.ToString();
            index++;

            //Temporary variable to save the cash value
            double CashTemp = Cash;
            //Plot product price and portfolio value
            //number of Step
            int nbStep= prix.Length;
            for (int j = 1; j < nbStep ; j++)
            {
                
                //Color and scale of the chart
                Chart1.Series.FindByName("ProductPrice").Color = Color.DarkBlue;
                Chart1.Series.FindByName("PortfolioPrice").Color = Color.DarkRed;
                Chart1.ChartAreas[0].AxisY.Maximum = 104;
                Chart1.ChartAreas[0].AxisY.Minimum = 88;
                Chart1.Series.FindByName("ProductPrice").Points.Add(prix[j]);

                double vp = 0;
                //The Cash was hold at a constant rate r
                CashTemp *= Math.Exp(wrapper.getR() * (1 / nbStep));
                for (int i = 0; i < delta[j].Length - 1; i++)
                {
                    //We buy a certain quantity of action required for hedging
                    CashTemp -= (delta[j][i] - delta[j - 1][i]) * market[j][i];
                    vp += delta[j][i] * market[j][i];
                }
                //Vp= asset value + cash
                vp += CashTemp;
                Chart1.Series.FindByName("PortfolioPrice").Points.Add(vp);
            }
        }

        public void Compute_Simu1(Object sender, EventArgs e)
        {
            ComputeSimulation(1);
        }

        public void Compute_Simu2(Object sender, EventArgs e)
        {
            ComputeSimulation(2);
        }
            

        public void Compute_Simu3(Object sender, EventArgs e)
        {
            ComputeSimulation(3);
        }

    
     public void Continue_Simu(Object sender, EventArgs e)
        {
            deltaTable.Rows.Clear();
            assetTable.Rows.Clear();
            prixLabel.Text = prix[index].ToString();
            double vp = 0;
            Cash *= Math.Exp(wrapper.getR()*(1/250));
            InitialCash *= Math.Exp(wrapper.getR() * (1 / 250));
            for (int i = 0; i < delta[index].Length - 1; i++)
            {
                TableRow tr1 = new TableRow();
                TableCell tc1 = new TableCell();
                tc1.Text = delta[index][i].ToString();
                tr1.Cells.Add(tc1);
                deltaTable.Rows.Add(tr1);

                TableRow tr2 = new TableRow();
                TableCell tc3 = new TableCell();
                tc3.Text = delta[index][i].ToString();
                tr2.Cells.Add(tc3);
                TableCell tc4 = new TableCell();
                tc4.Text = market[index][i].ToString();
                tr2.Cells.Add(tc4);
                Cash -= (delta[index][i] - delta[index-1][i]) * market[index][i];
                assetTable.Rows.Add(tr2);
                vp += delta[index][i] * market[index][i];
            }
            vp += Cash;
            cashLabel.Text = Cash.ToString();
            vpLabel.Text = vp.ToString();
            teLabel.Text = (vp - prix[index]).ToString();
            plLabel.Text = (vp - prix[index]+InitialCash).ToString();
            index++;            
        }
    }
}