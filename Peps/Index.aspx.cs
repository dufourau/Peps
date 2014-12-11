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
        protected void Page_Load(object sender, EventArgs e)
        {
           // b.Click += new EventHandler(this.Compute_Price);
        }
        protected void Compute_Price(Object sender, EventArgs e)
        {
            // Retrieve the values of the parameters in the TextBoxes
            WrapperClass wc = new WrapperClass();
            wc.getPrice(0, 0, 0, 0, 0, 0);
            prixLabel.Text = wc.getPrice().ToString();
            icLabel.Text = wc.getIC().ToString();
        }
    }
}