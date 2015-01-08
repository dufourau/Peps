<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Peps.Index" %>

<!DOCTYPE html>
<link type="text/css" rel="stylesheet" href="css/bootstrap.css" /> 
<link type="text/css" rel="stylesheet" href="css/normalize.css" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    
    <div class="container">
        <form id="form1" runat="server">
            <div class="row" style="margin:5px;">
                 <div class="col-md-1">
                    
                </div>
                <div class="col-md-6">
                    <h1>Gestion de notre produit: </h1>
                </div>
            </div>
            <div class="row" style="margin:5px;">
                <div class="col-md-1">
                    
                </div>
                <div class="col-md-2">
                    <asp:Button class="btn btn-lg btn-primary btn-block" ID="b" Text="Calculez" OnClick="Compute_Price" runat="server"/> 
                </div>
                <div class="col-md-1">
                    H
                </div>
                <div class="col-md-1">
                    <asp:TextBox class="form-control" ID="HLabel" runat="server">
                    </asp:TextBox>
                </div>
            </div> 
            <div class="row" style="margin:5px;">
                 <div class="col-md-1">
                    Prix
                </div>
               
                <div class="col-md-4">
                    <asp:TextBox class="form-control" ID="prixLabel" runat="server">
                    </asp:TextBox>
                </div>
            </div>
            <div class="row" style="margin:5px;">
                 <div class="col-md-1">
                    Incertitude
                </div>
                 <div class="col-md-4">
                  
                    <asp:TextBox class="form-control" ID="icLabel" runat="server">
                    </asp:TextBox>
                </div>
            </div>

             <div class="row" style="margin:5px;">
                 <div class="col-md-1">
                    Profit & Loss
                </div>
                 <div class="col-md-4">
                  
                    <asp:TextBox class="form-control" ID="plLabel" runat="server">
                    </asp:TextBox>
                </div>
            </div>
        </form>  
        <div class="row" style="margin:5px;">
            <div class="col-md-1">
                Delta
            </div>     
            <div class="col-md-4">
                <asp:table ID="deltaTable" class="table table-bordered" runat="server">
                    <asp:TableRow>
                        <asp:TableCell>Quantité à investir</asp:TableCell>
                        <asp:TableCell>Incertitude</asp:TableCell>
                        <asp:TableCell>Actifs</asp:TableCell>
                        <asp:TableCell>Cours</asp:TableCell>
                        
                    </asp:TableRow>
                </asp:table>
            </div>
    
 

<script src="js/jquery-2.1.1.min.js"></script>
<script src="js/bootstrap.min.js"></script> 
</body>
</html>
