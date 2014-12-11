<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Peps.Index" %>

<!DOCTYPE html>
<link type="text/css" rel="stylesheet" href="css/bootstrap.css" /> 
<link type="text/css" rel="stylesheet" href="css/normalize.css" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>Gestion de notre produit: </div>
        <asp:Button ID="b" Text="test" OnClick="Compute_Price" runat="server"/> 
        <asp:TextBox ID="prixLabel" runat="server">
        </asp:TextBox>
        <asp:TextBox ID="icLabel" runat="server">
        </asp:TextBox>
     </form>    
    <div class="container">   
    <table class="table table-bordered">
        <tr>
            <td>Actifs</td>
            <td>Cours</td>
            <td>Quantité à investir</td>
        </tr>
        <tr>
            
        </tr>
        <tr>
            
        </tr>
        
        
     

    </table>
    </div>
    
 

<script src="js/jquery-2.1.1.min.js"></script>
<script src="js/bootstrap.min.js"></script> 
</body>
</html>
