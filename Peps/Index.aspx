﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Peps.Index" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

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
                 
                <div class="col-md-6">
                    <h1>Gestion de notre produit: </h1>
                </div>
            </div>

            <div class="row" style="margin:5px;">
            
                
                <div class="col-md-4">
                    <asp:Button class="btn btn-lg btn-primary btn-block" ID="b" Text="Forcez la simulation" OnClick="Compute_Price" runat="server"/> 
                </div>
                <div class="col-md-2">

                </div>
                
            </div> 
            <div class="row" style="margin:5px;">
            
                
                <div class="col-md-2">
                    <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button2" Text="Simulation 1" OnClick="Compute_Simu1" runat="server"/> 
                </div>
                <div class="col-md-2">
                      <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button3" Text="Simulation 2" OnClick="Compute_Simu2" runat="server"/> 
                </div>
                 <div class="col-md-2">
                       <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button4" Text="Simulation 3" OnClick="Compute_Simu3" runat="server"/> 
                </div>
                
            </div> 

            <div class="row" style="margin:5px;">
                <div class="col-md-4">
                    <h1>Date courante</h1>
                </div>
                <div class="col-md-2">
                </div>
                 <div class="col-md-4">
                    <h1>Date suivante</h1>
                </div>
                <div class="col-md-2">
                    <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button1" Text="Mise à jour" OnClick="Continue_Simu" runat="server"/> 
                </div>

            </div>
            <div class="row" style="margin:5px;">
                 <div class="col-md-2">
                    Prix
                </div>
               
                <div class="col-md-2">
                    <asp:TextBox class="form-control" ID="prixLabel" runat="server">
                    </asp:TextBox>
                </div>
            </div>
            <div class="row" style="margin:5px;">
                 <div class="col-md-2">
                    Incertitude
                </div>
                 <div class="col-md-2">
                  
                    <asp:TextBox class="form-control" ID="icLabel" runat="server">
                    </asp:TextBox>
                </div>
            </div>
            
            <div class="row" style="margin:5px;">
                 <div class="col-md-2">
                    Tracking Error
                </div>
                 <div class="col-md-2">
                  
                    <asp:TextBox class="form-control" ID="teLabel" runat="server">
                    </asp:TextBox>
                </div>
            </div>

             <div class="row" style="margin:5px;">
                 <div class="col-md-2">
                    Profit & Loss
                </div>
                 <div class="col-md-2">
                  
                    <asp:TextBox class="form-control" ID="plLabel" runat="server">
                    </asp:TextBox>
                </div>
            </div>
            <div class="row" style="margin:5px;">
                 <div class="col-md-2">
                    Cash
                </div>
                 <div class="col-md-2">
                  
                    <asp:TextBox class="form-control" ID="cashLabel" runat="server">
                    </asp:TextBox>
                </div>
            </div>
            <div class="row" style="margin:5px;">
                 <div class="col-md-2">
                    Valeur du portefeuille
                </div>
                 <div class="col-md-2">
                  
                    <asp:TextBox class="form-control" ID="vpLabel" runat="server">
                    </asp:TextBox>
                </div>
            </div>
        </form>  
        <div class="row" style="margin:5px;">
            <div class="col-md-1">
                Portefeuille
            </div>     
            <div class="col-md-4">
                <asp:table ID="assetTable" class="table table-bordered" runat="server">
                    <asp:TableRow>
                        <asp:TableCell>Quantité d'actif</asp:TableCell>
                    
                        <asp:TableCell>Cours</asp:TableCell>
                        
                    </asp:TableRow>
                </asp:table>
            </div>
            <div class="col-md-2">
                Delta
            </div>
            <div class="col-md-4">
                <asp:table ID="deltaTable" class="table table-bordered" runat="server">
                    <asp:TableRow>
                        <asp:TableCell>Quantité à investir</asp:TableCell>          
                
                        <asp:TableCell>Cours</asp:TableCell>
                        
                    </asp:TableRow>
                </asp:table>
            </div>
        </div>
    
 

<script src="js/jquery-2.1.1.min.js"></script>
<script src="js/bootstrap.min.js"></script> 
    </div>
    <asp:Chart ID="Chart1" runat="server" Width="1283px">
        <series>
            <asp:Series ChartType="Line" Name="PortfolioPrice">
            </asp:Series>
            <asp:Series ChartArea="ChartArea1" ChartType="Line" Name="ProductPrice">
            </asp:Series>
        </series>
        <chartareas>
            <asp:ChartArea Name="ChartArea1">
            </asp:ChartArea>
        </chartareas>
    </asp:Chart>
</body>
</html>
