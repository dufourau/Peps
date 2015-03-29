<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Peps.Index" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>
<link type="text/css" rel="stylesheet" href="css/bootstrap.css" />
<link type="text/css" rel="stylesheet" href="css/normalize.css" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title></title>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.6/jquery.min.js" type="text/javascript"></script>
    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js" type="text/javascript"></script>
    <link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            var date = new Date(2005, 10, 30);
            $("[id$=DisplayCalendar]").datepicker({
                showOn: 'focus',
                defaultDate: date,
                changeYear: true,
                minDate: date,
                maxDate: new Date(2015, 1, 30)
            });
        });
    </script>


</head>
<body>
    
        <form id="form1" runat="server">
            <p>
                <asp:Button class="btn btn-default"  OnClick="LoadPage1" runat="server" Text = "Gestion" />
                <asp:Button class="btn btn-default"  OnClick="LoadPage2" runat="server" Text = "Vérification" />
                <asp:Button class="btn btn-default"  OnClick="LoadPage3" runat="server" Text = "Données" />
            </p>
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0"  >
                <asp:View ID="View1" runat="server">
                   <div class="container">
                        <!--TODO put this in a file and import it-->
                        <div class="row" style="margin: 5px;">
                            <div class="col-md-6">
                                <h1>Gestion de notre produit: </h1>
                            </div>
                        </div>
                       
                        <div class="row" style="margin: 5px;">
                           <label for="DisplayCalendar" class="col-sm-2 control-label">Changer la date</label>
                            <div class="col-md-2">

                                <asp:TextBox ID="DisplayCalendar" placeholder="11/30/2015" runat="server" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-md-2">
                                <asp:Button class="btn btn-lg btn-primary btn-block" ID="SaveDate" runat="server" Text="Sauvegarder" OnClick="updateDate" />
                            </div>

                        </div>
                        <div class="row" style="margin: 5px;">

                            <div class="col-md-4">
                                <asp:Button class="btn btn-lg btn-primary btn-block" ID="b" Text="Forcez la simulation" OnClick="Compute_Price" runat="server" />
                            </div>
                            <div class="col-md-2">
                                <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button5" Text="GetData" OnClick="Get_Data" runat="server" />
                            </div>

                        </div>
                       <div class="row" style="margin: 5px;">
                            <div class="col-md-4">
                                <h1>Date courante</h1>
                            </div>
                            <div class="col-md-2">
                            </div>
                            
                            <div class="col-md-2">
                                <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button1" Text="Mise à jour" OnClick="Continue_Computation" runat="server" />
                            </div>

                        </div>
                     </div> 
                </asp:View>
                <asp:View ID="View2" runat="server">
                   <div class="container">     
                        <div class="row" style="margin: 5px;">

                            <div class="col-md-6">
                                <h1>Vérification de notre couverture: </h1>
                            </div>
                        </div>
                    
                         <div class="row" style="margin: 5px;">
                            <label for="Rebalancement" class="col-md-3 control-label">Charger une simulation</label>
                            <div class="col-md-2">
                                <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button2" Text="Simulation 1" OnClick="Compute_Simu1" runat="server" />
                            </div>
                            <div class="col-md-2">
                                <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button3" Text="Simulation 2" OnClick="Compute_Simu2" runat="server" />
                            </div>
                            <div class="col-md-2">
                                <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button4" Text="Simulation 3" OnClick="Compute_Simu3" runat="server" />
                            </div>

                        </div>
                       <div class="row" style="margin: 5px;">
                            <div class="col-md-4">
                                <h1>Date courante</h1>
                            </div>
                            <div class="col-md-2">
                            </div>
                            
                            <div class="col-md-2">
                                <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button7" Text="Mise à jour" OnClick="Continue_Simu" runat="server" />
                            </div>

                        </div>
                        <div class="row" style="margin: 5px;">
                           
                            <label for="Rebalancement" class="col-md-3 control-label">Intervalle de rebalancement en jour</label>
                            <div class="col-md-2">
                                <asp:TextBox class="form-control" ID="Rebalancement" runat="server" Text="1">
                                </asp:TextBox>
                            </div>
                        </div>
                       <div class="row" style="margin: 5px;">
                           <label for="Frais" class="col-md-3 control-label">Frais de transaction en %</label>
                            <div class="col-md-2">
                                <asp:TextBox class="form-control" ID="Fees" runat="server" Text="0.5">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>   
                </asp:View> 
                <asp:View ID="View3" runat="server">
                    
                    <div class="container"> 
                        <div class="row" style="margin: 5px;">
                            <div class="col-md-2">
                                <asp:DropDownList ID="DropDownList1" runat="server"></asp:DropDownList>
                            </div>
                            <div class="col-md-2">
                                <asp:Button class="btn btn-lg btn-primary btn-block" ID="Button6" Text="Afficher" OnClick="Display_Chart" runat="server" />
                            </div>
                        </div>
                    </div>
                    
                </asp:View> 
            </asp:MultiView>

            <div class="container">
                    
                        <div class="row" style="margin: 5px;">
                            <div class="col-md-2">
                                Prix
                            </div>

                            <div class="col-md-2">
                                <asp:TextBox class="form-control" ID="prixLabel" runat="server">
                                </asp:TextBox>
                            </div>
                        </div>
                        <div class="row" style="margin: 5px;">
                            <div class="col-md-2">
                                Incertitude
                            </div>
                            <div class="col-md-2">

                                <asp:TextBox class="form-control" ID="icLabel" runat="server">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row" style="margin: 5px;">
                            <div class="col-md-2">
                                Tracking Error
                            </div>
                            <div class="col-md-2">

                                <asp:TextBox class="form-control" ID="teLabel" runat="server">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row" style="margin: 5px;">
                            <div class="col-md-2">
                                Profit & Loss
                            </div>
                            <div class="col-md-2">

                                <asp:TextBox class="form-control" ID="plLabel" runat="server">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row" style="margin: 5px;">
                            <div class="col-md-2">
                                Cash
                            </div>
                            <div class="col-md-2">

                                <asp:TextBox class="form-control" ID="cashLabel" runat="server">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="row" style="margin: 5px;">
                            <div class="col-md-2">
                                Valeur du portefeuille
                            </div>
                            <div class="col-md-2">

                                <asp:TextBox class="form-control" ID="vpLabel" runat="server">
                                </asp:TextBox>
                            </div>
                        </div>
                   
                    <div class="row" style="margin: 5px;">
                        <div class="col-md-1">
                            Portefeuille
                        </div>
                        <div class="col-md-4">
                            <asp:Table ID="assetTable" class="table table-bordered" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell>Quantité d'actif présente dans le portefeuille</asp:TableCell>

                                    <asp:TableCell>Cours de l'actif</asp:TableCell>

                                </asp:TableRow>
                            </asp:Table>
                        </div>
                        <div class="col-md-2">
                            Delta
                        </div>
                        <div class="col-md-4">
                            <asp:Table ID="deltaTable" class="table table-bordered" runat="server">
                                <asp:TableRow>
                                    <asp:TableCell>Quantité à investir dans l'actif à possèder à la date suivante</asp:TableCell>


                                </asp:TableRow>
                            </asp:Table>
                        </div>
                    </div>                       

                      
                <!--Graph-->
                <asp:Chart ID="Chart1" runat="server" Width="1283px">
                    <Series>
                        <asp:Series ChartType="Line" Name="PortfolioPrice">
                        </asp:Series>
                        <asp:Series ChartType="Line" Name="assetPrice">
                        </asp:Series>
                        <asp:Series ChartArea="ChartArea1" ChartType="Line" Name="ProductPrice">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </div>
     </form>
            
</body>
</html>
