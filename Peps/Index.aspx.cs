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
        protected void Page_Load(object sender, EventArgs e)
        {
            wrapper = new WrapperClass();
            //b.Click += new EventHandler(this.Compute_Price);
        }
        protected void Compute_Price(Object sender, EventArgs e)
        {
            //try
            //{
                // Retrieve the values of the parameters in the TextBoxes
                //WrapperClass wc = new ();

                wrapper.computePrice();
                wrapper.computeDelta(); 
                prixLabel.Text = wrapper.getPrice().ToString();
                icLabel.Text = wrapper.getIC().ToString();
                double[] delta = wrapper.getDelta();
                double[] deltaIC = wrapper.getDeltaIC();
                
                for (int i = 0; i < delta.Length;i++ )
                {
                    TableRow tr = new TableRow();
                    TableCell tc1 = new TableCell();
                    tc1.Text = delta[i].ToString();
                    TableCell tc2 = new TableCell();
                    tc2.Text = deltaIC[i].ToString();
                    tr.Cells.Add(tc1);
                    tr.Cells.Add(tc2);
                    deltaTable.Rows.Add(tr);
                    
                }

            /*    
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Message: "+ ex.Message);
            } 
             */
        }
    }
}