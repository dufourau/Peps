using System;
using System.Collections.Generic;
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
        static String[] delta;
        static String[] prix;
        static String[] market;
        static String[] deltaPrec;
        static int indexPrix = 0;
        static int indexDelta = 0;
        static int indexMarket = 0;
        static double Cash = 0;
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
                plLabel.Text = wrapper.getPL().ToString();
                double[] delta = wrapper.getDelta();
                double[] deltaIC = wrapper.getDeltaIC();
                
                for (int i = 0; i < delta.Length;i++ )
                {
                    TableRow tr = new TableRow();
                    TableCell tc1 = new TableCell();
                    tc1.Text = delta[i].ToString();
                    //TableCell tc2 = new TableCell();
                    //tc2.Text = deltaIC[i].ToString();
                    tr.Cells.Add(tc1);
                    //tr.Cells.Add(tc2);
                    deltaTable.Rows.Add(tr);
                    
                }

        
        }

        public void Compute_Simu1(Object sender, EventArgs e)
        {
            indexDelta = 0;
            indexPrix = 0;
            indexMarket = 0;
            deltaTable.Rows.Clear();
            assetTable.Rows.Clear();
            String prix1= Properties.Resources.Prix1.ToString();
            String delta1= Properties.Resources.Deltas1.ToString();
            String market1 = Properties.Resources.Market1.ToString();
            prix= prix1.Split('\n');
            delta = delta1.Split('\n');
            market = market1.Split('\n');
            prixLabel.Text = prix[indexPrix++];
            String[] deltaLine = delta[indexDelta++].Split(' ');
            String[] marketLine = market[indexMarket++].Split(' ');

            Cash= double.Parse(prix[indexPrix-1], System.Globalization.CultureInfo.InvariantCulture);
            for (int i = 0; i < deltaLine.Length-1; i++)
            {
                TableRow tr1 = new TableRow();
                TableCell tc1 = new TableCell();
                tc1.Text = deltaLine[i];
                tr1.Cells.Add(tc1);
                TableCell tc2 = new TableCell();
                tc2.Text = marketLine[i];
                tr1.Cells.Add(tc2);
                deltaTable.Rows.Add(tr1);

                TableRow tr2 = new TableRow();
                TableCell tc3 = new TableCell();
                tc3.Text = "0";
                tr2.Cells.Add(tc3);
                TableCell tc4 = new TableCell();
                tc4.Text = marketLine[i];
                tr2.Cells.Add(tc4);

                assetTable.Rows.Add(tr2);

                Cash -= double.Parse(deltaLine[i], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(marketLine[i], System.Globalization.CultureInfo.InvariantCulture);

            }
            cashLabel.Text = Cash.ToString();
            vpLabel.Text = prix[indexPrix-1];
            plLabel.Text = "0";
     
          
        }

        public void Compute_Simu2(Object sender, EventArgs e)
        {
            indexDelta = 0;
            indexPrix = 0;
            indexMarket = 0;
            deltaTable.Rows.Clear();
            assetTable.Rows.Clear();
            String prix1 = Properties.Resources.Prix2.ToString();
            String delta1 = Properties.Resources.Deltas2.ToString();
            String market1 = Properties.Resources.Market2.ToString();
            prix = prix1.Split('\n');
            delta = delta1.Split('\n');
            market = market1.Split('\n');
            prixLabel.Text = prix[indexPrix++];
            String[] deltaLine = delta[indexDelta++].Split(' ');
            String[] marketLine = market[indexMarket++].Split(' ');

            Cash = double.Parse(prix[indexPrix - 1], System.Globalization.CultureInfo.InvariantCulture);
            for (int i = 0; i < deltaLine.Length - 1; i++)
            {
                TableRow tr1 = new TableRow();
                TableCell tc1 = new TableCell();
                tc1.Text = deltaLine[i];
                tr1.Cells.Add(tc1);
                TableCell tc2 = new TableCell();
                tc2.Text = marketLine[i];
                tr1.Cells.Add(tc2);
                deltaTable.Rows.Add(tr1);

                TableRow tr2 = new TableRow();
                TableCell tc3 = new TableCell();
                tc3.Text = "0";
                tr2.Cells.Add(tc3);
                TableCell tc4 = new TableCell();
                tc4.Text = marketLine[i];
                tr2.Cells.Add(tc4);

                assetTable.Rows.Add(tr2);

                Cash -= double.Parse(deltaLine[i], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(marketLine[i], System.Globalization.CultureInfo.InvariantCulture);

            }
            cashLabel.Text = Cash.ToString();
            vpLabel.Text = prix[indexPrix - 1];
            plLabel.Text = "0";


        }

        public void Compute_Simu3(Object sender, EventArgs e)
        {
            indexDelta = 0;
            indexPrix = 0;
            indexMarket = 0;
            deltaTable.Rows.Clear();
            assetTable.Rows.Clear();
            String prix1 = Properties.Resources.Prix3.ToString();
            String delta1 = Properties.Resources.Deltas3.ToString();
            String market1 = Properties.Resources.Market3.ToString();
            prix = prix1.Split('\n');
            delta = delta1.Split('\n');
            market = market1.Split('\n');
            prixLabel.Text = prix[indexPrix++];
            String[] deltaLine = delta[indexDelta++].Split(' ');
            String[] marketLine = market[indexMarket++].Split(' ');

            Cash = double.Parse(prix[indexPrix - 1], System.Globalization.CultureInfo.InvariantCulture);
            for (int i = 0; i < deltaLine.Length - 1; i++)
            {
                TableRow tr1 = new TableRow();
                TableCell tc1 = new TableCell();
                tc1.Text = deltaLine[i];
                tr1.Cells.Add(tc1);
                TableCell tc2 = new TableCell();
                tc2.Text = marketLine[i];
                tr1.Cells.Add(tc2);
                deltaTable.Rows.Add(tr1);

                TableRow tr2 = new TableRow();
                TableCell tc3 = new TableCell();
                tc3.Text = "0";
                tr2.Cells.Add(tc3);
                TableCell tc4 = new TableCell();
                tc4.Text = marketLine[i];
                tr2.Cells.Add(tc4);

                assetTable.Rows.Add(tr2);

                Cash -= double.Parse(deltaLine[i], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(marketLine[i], System.Globalization.CultureInfo.InvariantCulture);

            }
            cashLabel.Text = Cash.ToString();
            vpLabel.Text = prix[indexPrix - 1];
            plLabel.Text = "0";


        }

        public void Continue_Simu(Object sender, EventArgs e)
        {
            deltaTable.Rows.Clear();
            assetTable.Rows.Clear();
            prixLabel.Text = prix[indexPrix++];
            String[] deltaLine = delta[indexDelta++].Split(' ');
            String[] deltaPrecLine = delta[indexDelta-2].Split(' ');
            String[] marketLine = market[indexMarket++].Split(' ');
            double vp = 0;
            Cash *= Math.Exp(0.05*(1/250));
            for (int i = 0; i < deltaLine.Length - 1; i++)
            {
                TableRow tr1 = new TableRow();
                TableCell tc1 = new TableCell();
                tc1.Text = deltaLine[i];
                tr1.Cells.Add(tc1);
                TableCell tc2 = new TableCell();
                tc2.Text = marketLine[i];
                tr1.Cells.Add(tc2);
                deltaTable.Rows.Add(tr1);

                TableRow tr2 = new TableRow();
                TableCell tc3 = new TableCell();
                tc3.Text = deltaPrecLine[i];
                tr2.Cells.Add(tc3);
                TableCell tc4 = new TableCell();
                tc4.Text = marketLine[i];
                tr2.Cells.Add(tc4);
                Cash -= (double.Parse(deltaLine[i], System.Globalization.CultureInfo.InvariantCulture) - double.Parse(deltaPrecLine[i], System.Globalization.CultureInfo.InvariantCulture)) * double.Parse(marketLine[i], System.Globalization.CultureInfo.InvariantCulture);
                assetTable.Rows.Add(tr2);
                vp += double.Parse(deltaLine[i], System.Globalization.CultureInfo.InvariantCulture) * double.Parse(marketLine[i], System.Globalization.CultureInfo.InvariantCulture);

            }
            vp += Cash;
            cashLabel.Text = Cash.ToString();
            vpLabel.Text = vp.ToString();
            plLabel.Text = (double.Parse(prix[indexPrix - 1], System.Globalization.CultureInfo.InvariantCulture) - vp).ToString(); ;
            
            
        }

    }
}