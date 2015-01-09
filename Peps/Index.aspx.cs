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
        static String delta;
        static String prix;
        static String spot;
        protected void Page_Load(object sender, EventArgs e)
        {
            wrapper = new WrapperClass();
            
        }
        protected void Compute_Price(Object sender, EventArgs e)
        {
           
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
            prix= Properties.Resources.Prix1.ToString();

            prix = System.IO.File.ReadAllLines("~/Prix1.txt");
            prixLabel.Text = prix[0]; 
        }
    }
}