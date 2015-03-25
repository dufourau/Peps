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
        static WrapperClass wrapper;
        static MarketData data;
        static int index = 0;
        double Cash;  
        Portfolio pf;
        
        public Portfolio CurrentPortfolio
        {
            get
            {
                object temp = Session["Pf"];  
                return temp == null ? null : (Portfolio)temp; ;
               
            }
            set { Session["Pf"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            wrapper = new WrapperClass();
            data = new MarketData();
            if(CurrentPortfolio == null){
                double[][] marketdata = parseFileToMatrix(Properties.Resources.Market3, CurrentPortfolio.dates);
                
                CurrentPortfolio = new Portfolio(wrapper.getOption_size()+wrapper.getCurr_size(),wrapper.getH());
                CurrentPortfolio.data = marketdata;
            }
           
        }

        public void Get_Data(Object sender, EventArgs e)
        {
            //Retrieve data from 30 august 2005 to 30 january 2015  
            data.retrieveDataFromWeb("7","30","2005","0","30","2015");
            data.fixeDataSize();
            data.storeData();
            
            
        }

        public void updateDate(Object sender, EventArgs e)
        {
            String str = Request.Form[DisplayCalendar.UniqueID];
            String[] date = str.Split('/');
            if(date.Length == 3){
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
                deltaTable.Rows.Clear();
                assetTable.Rows.Clear();


                index = 0;
                CurrentPortfolio.profitAndLoss = new double[wrapper.getH()];
                CurrentPortfolio.pfvalue = new double[wrapper.getH()];
                CurrentPortfolio.prix = new double[wrapper.getH()];
                CurrentPortfolio.delta = new double[wrapper.getH()][];
                CurrentPortfolio.market = new double[wrapper.getH()][];
                
                //Copy the market value at the current date
                String str = CurrentPortfolio.getD().ToString() + "-" + CurrentPortfolio.getM().ToString() + "-" + CurrentPortfolio.getY().ToString();
                int indexCurrentDate = CurrentPortfolio.dates.IndexOf(str);
                if (indexCurrentDate == -1)
                {
                    indexCurrentDate = CurrentPortfolio.dates.Count - 1;
                }
                
                

                //Price in 0
                if (CurrentPortfolio.getM() == 11 && CurrentPortfolio.getD() == 30 && CurrentPortfolio.getY() == 2005)
                {

                    //Initialize the spot vector
                    for (int i = 0; i < CurrentPortfolio.market[index].Length; i++ )
                    {
                        wrapper.setSpot(CurrentPortfolio.market[index][i], i);
                    }
                    
                    wrapper.computePrice();
                    wrapper.computeDelta();
                    icLabel.Text = wrapper.getIC().ToString();
                    CurrentPortfolio.market[index] = CurrentPortfolio.data[indexCurrentDate];
                    CurrentPortfolio.delta[index] = wrapper.getDelta();
                    InitSimu(wrapper.getDelta(), wrapper.getPrice());
                }
                else
                {
                    int indexFirstDate = CurrentPortfolio.dates.IndexOf("30-11-2005");
                    //TODO load previous delta from FILE + incrémenter index 

                    //Compute the date t in years
                    int[] previousDate;
                    previousDate = CurrentPortfolio.previousDate();
                    double t = previousDate[0] - 2005;
                    if (previousDate[0] == CurrentPortfolio.getY())
                    {
                        t += ((double)previousDate[2]) / 365.0;
                    }
                    else
                    {
                        t += ((double)(((CurrentPortfolio.getM() - previousDate[1]) + 11) * 30 + previousDate[2])) / 365.0;
                    }
                    //Compute the past matrice
                    computePast(CurrentPortfolio.dates);
                    //Compute delta and price for the date t
                    wrapper.computePrice(t);
                    wrapper.computeDelta(t);
                    //TO DO A appeler avec le premier delta et premier prix
                    //InitSimu(wrapper.getDelta(), wrapper.getPrice());
                    //Copy the current delta and market value in the portfolio
                    CurrentPortfolio.market[indexFirstDate-indexCurrentDate] = CurrentPortfolio.data[indexCurrentDate];
                    CurrentPortfolio.delta[indexFirstDate - indexCurrentDate] = wrapper.getDelta();
                    CurrentPortfolio.prix[indexFirstDate - indexCurrentDate] = wrapper.getPrice();
                    //We move forward in the simulation
                    ComputeSimulation(indexFirstDate - indexCurrentDate);
                    

                }
                initDisplay();
                displayData();   
        }

        protected void Update(Object sender, EventArgs e)
        {
            
            String str = CurrentPortfolio.getD().ToString() + "-" + CurrentPortfolio.getM().ToString() + "-" + CurrentPortfolio.getY().ToString();
            int indexCurrentDate = CurrentPortfolio.dates.IndexOf(str);
            if (indexCurrentDate == -1)
            {
                indexCurrentDate = CurrentPortfolio.dates.Count - 1;
            }
            indexCurrentDate--;
            str= CurrentPortfolio.dates[indexCurrentDate];
            int[] dates = convertToInt(str.Split('-'));
            CurrentPortfolio.setDate(dates[0],dates[1],dates[2]);

            //TO DO factorise
            //Compute the date t in years
            int[] previousDate;
            previousDate = CurrentPortfolio.previousDate();
            double t = previousDate[0] - 2005;
            if (previousDate[0] == CurrentPortfolio.getY())
            {
                t += ((double)previousDate[2]) / 365.0;
            }
            else
            {
                t += ((double)(((CurrentPortfolio.getM() - previousDate[1]) + 11) * 30 + previousDate[2])) / 365.0;
            }
            //Compute the past matrice
            computePast(CurrentPortfolio.dates);
            //Compute delta and price for the date t
            wrapper.computePrice(t);
            wrapper.computeDelta(t);
            //Copy the current delta and market value in the portfolio
            CurrentPortfolio.market[index+1] = CurrentPortfolio.data[indexCurrentDate];
            CurrentPortfolio.delta[index+1] = wrapper.getDelta();
            CurrentPortfolio.prix[index+1] = wrapper.getPrice();
            ComputeSimulation();
            displayData();

        }

        public int[] convertToInt(String [] stringArray)
        {
            int[] intArray = new int[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++ )
            {
                intArray[i] = int.Parse(stringArray[i], System.Globalization.CultureInfo.InvariantCulture);
            }
            return intArray;
        
        }
        //A optimiser
        //Compute the past matrix in wrapper
        public void computePast(List<String> dates)
        {

            int[] previousDate = CurrentPortfolio.previousDate();
            if (CurrentPortfolio.getM() == 11 && CurrentPortfolio.getD() == 30)
            {
                wrapper.initPast(previousDate[0] - 2005 + 1);
            }
            else
            {
                wrapper.initPast(previousDate[0] - 2005 + 2);
            }

            //Include the previous constation date
            String str = "30-11-";
            for (int year = 2005; year <= previousDate[0]; year++)
            {
                String temp = str + year.ToString();
                int index = dates.IndexOf(temp);
                if (index == -1)
                {
                    index = dates.Count - 1;
                }
                for (int i = 0; i < (wrapper.getOption_size() + 3); i++)
                {
                
                    wrapper.set(CurrentPortfolio.data[index][i]);
                }

            }
            //Include one more date if t isn't a constation date

            if (CurrentPortfolio.getM() != 11 && CurrentPortfolio.getD() != 30)
            {
                str = CurrentPortfolio.getD().ToString() + "-" + CurrentPortfolio.getM().ToString() + "-" + CurrentPortfolio.getY().ToString();
                int indexLastDate = dates.IndexOf(str);
                if (indexLastDate == -1)
                {
                    indexLastDate = dates.Count - 1;
                }
                for (int i = 0; i < (wrapper.getOption_size() + 3); i++)
                {
                    wrapper.set(CurrentPortfolio.data[indexLastDate][i]);
                }
            }
        }
        private double[][] parseFileToMatrix(String file,List<String> dates)
        {
             String[] lines = file.Split('\n').Where(x => x != "" && x != null).ToArray();
             double[][] parsed = new double[lines.Length][];
             String[] symbols = lines[0].Trim().Split(' ');
             //Add symbols to the portfolio in the right order    
             for(int i= 0; i<symbols.Length; i++){
                 CurrentPortfolio.symbols.Add(symbols[i]);
             }
             for (int i = 1; i < lines.Length; i++)
             {
                String[] items = lines[i].Trim().Split(' ');
                parsed[i] = new double[items.Length-1];
                dates.Add(items[0]);
                for (int j = 1; j < items.Length; j++)
                {
                    parsed[i][j-1] = double.Parse(items[j], System.Globalization.CultureInfo.InvariantCulture);
                }
             }
             return parsed;

        }

     

        //Use as a validation test of our model
        //Function use to initialize the computation
        public void LoadSimulation(int idSimulation)
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
            //Initialize the historical value stored in our portfolio
            CurrentPortfolio.delta = parseFileToMatrix(deltaSimu);
            CurrentPortfolio.prix = parseFileToArray(prixSimu);
            CurrentPortfolio.market = parseFileToMatrix(marketSimu);       
            CurrentPortfolio.pfvalue= new double[CurrentPortfolio.prix.Length];
            CurrentPortfolio.profitAndLoss = new double[CurrentPortfolio.prix.Length];
            index= 0;
            InitSimu(CurrentPortfolio.delta[index], CurrentPortfolio.prix[index]);

            
        }

        public void Compute_Simu1(Object sender, EventArgs e)
        {
            LoadSimulation(1);
            displayData();
        }

        public void Compute_Simu2(Object sender, EventArgs e)
        {
            LoadSimulation(2);
            displayData();
        }
            

        public void Compute_Simu3(Object sender, EventArgs e)
        {
            LoadSimulation(3);
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

            prixLabel.Text = CurrentPortfolio.prix[index].ToString();
            for (int i = 0; i < CurrentPortfolio.delta[index].Length; i++)
            {
                TableRow deltaRow = new TableRow();
                TableCell quantityToBuy = new TableCell();
                quantityToBuy.Text = CurrentPortfolio.delta[index][i].ToString();
                deltaRow.Cells.Add(quantityToBuy);
                TableCell marketValue = new TableCell();
                marketValue.Text = CurrentPortfolio.market[index][i].ToString();
                deltaRow.Cells.Add(marketValue);
                deltaTable.Rows.Add(deltaRow);
                TableRow portfolioRow = new TableRow();
                TableCell quantityAlreadyBought = new TableCell();
                quantityAlreadyBought.Text = CurrentPortfolio.quantity[i].ToString();
                portfolioRow.Cells.Add(quantityAlreadyBought);
                TableCell secondColumnPortfolio = new TableCell();
                //Add the name of the asset
                secondColumnPortfolio.Text = CurrentPortfolio.symbols[i];
                portfolioRow.Cells.Add(secondColumnPortfolio);

                assetTable.Rows.Add(portfolioRow);
            }

            cashLabel.Text = CurrentPortfolio.getCash().ToString();
            vpLabel.Text = CurrentPortfolio.pfvalue[index].ToString();
            teLabel.Text = CurrentPortfolio.trackingError.ToString();
            plLabel.Text = CurrentPortfolio.profitAndLoss[index].ToString();

        
            //Plot product price and portfolio value
            //number of Step
            int nbStep = CurrentPortfolio.prix.Length;
            initDisplay();
            for (int j = 1; j < index; j++)
            {
                Chart1.Series.FindByName("ProductPrice").Points.Add(CurrentPortfolio.prix[j]);
                Chart1.Series.FindByName("PortfolioPrice").Points.Add(CurrentPortfolio.pfvalue[j]);
            }
        }

        public void ComputeSimulation(int numberOfIteration=1)
        {
            for (int k = 0; k < numberOfIteration; k++)
            {
                index++;
                double vp = 0;
                CurrentPortfolio.setCash(CurrentPortfolio.getCash() * Math.Exp(wrapper.getR() * (1 / 250)));
                CurrentPortfolio.setInitialCash(CurrentPortfolio.getInitialCash() * Math.Exp(wrapper.getR() * (1 / 250)));
                for (int i = 0; i < CurrentPortfolio.delta[index].Length; i++)
                {
                    CurrentPortfolio.setCash(CurrentPortfolio.getCash() - (CurrentPortfolio.delta[index-1][i] - CurrentPortfolio.quantity[i]) * CurrentPortfolio.market[index-1][i]);
                    CurrentPortfolio.quantity[i] = CurrentPortfolio.delta[index - 1][i];
                    vp += CurrentPortfolio.delta[index][i] * CurrentPortfolio.market[index][i];
                }
                vp += CurrentPortfolio.getCash();
                CurrentPortfolio.pfvalue[index] = vp;
                CurrentPortfolio.trackingError = vp - CurrentPortfolio.prix[index];
                CurrentPortfolio.profitAndLoss[index] = CurrentPortfolio.trackingError + CurrentPortfolio.getInitialCash();
            }
        }


        //Init the simulation for t=0
        //Receive the product price, the market prices and the delta as parameters
        public void InitSimu(double[] delta, double prix)
        {
            index= 0;
            
            CurrentPortfolio.prix[index] = prix;
            //Initial benefit made from the sell of our product
            CurrentPortfolio.setInitialCash(100 - prix);
            //We use the price of our product for hedging
            CurrentPortfolio.setCash(prix);
            CurrentPortfolio.pfvalue[index] = prix;
            //Compute the tracking error
            CurrentPortfolio.trackingError = 0;
            //Compute the profit and Loss
            CurrentPortfolio.profitAndLoss[index] = 100 - prix;

        } 


     public void Continue_Simu(Object sender, EventArgs e)
     {
         if (CurrentPortfolio.prix == null) LoadSimulation(1);
         ComputeSimulation(1);
         displayData();
     }

     private double[][] parseFileToMatrix(String file)
     {
         String[] lines = file.Split('\n').Where(x => x != "" && x != null).ToArray();
         double[][] parsed = new double[lines.Length-1][];
         String[] symbols = lines[0].Trim().Split(' ');
         //Add symbols to the portfolio in the right order    
         for (int i = 0; i < symbols.Length; i++)
         {
             CurrentPortfolio.symbols.Add(symbols[i]);
         }

         for (int i = 1; i < lines.Length; i++)
         {
             String[] items = lines[i].Trim().Split(' ');
             parsed[i-1] = new double[items.Length];
             for (int j = 0; j < items.Length; j++)
             {
                 parsed[i-1][j] = double.Parse(items[j], System.Globalization.CultureInfo.InvariantCulture);
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

    }
}