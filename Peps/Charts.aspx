<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Charts.aspx.cs" Inherits="Peps.Charts" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    
    <!-- Bootstrap Core CSS -->
    <link href="css/bootstrap.min.css" rel="stylesheet"/>

    <!-- Custom CSS -->
    <link href="css/sb-admin.css" rel="stylesheet"/>

    <!-- Morris Charts CSS -->
    <link href="css/plugins/morris.css" rel="stylesheet"/>

    <!-- Custom Fonts -->
    <link href="font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css"/>
</head>
<body>
    <div id="wrapper">

             <!-- Navigation -->
    <nav class="navbar navbar-inverse navbar-fixed-top" role="navigation">
    <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-ex1-collapse">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <a class="navbar-brand" href="Default.aspx">Gestion Moduleis</a>
            </div>

        <ul class="nav navbar-right top-nav">
                <li class="dropdown">
                    <a href="#" class="dropdown-toggle" data-toggle="dropdown"><i class="fa fa-user"></i> User <b class="caret"></b></a>
                    <ul class="dropdown-menu">
                        <li>
                            <a href="#"><i class="fa fa-fw fa-user"></i> Profile</a>
                        </li>
                        <li>
                            <a href="#" data-toggle="modal" data-target="#myModal"><i class="fa fa-fw fa-gear" ></i> Settings</a>
      
                        </li>
                        <li class="divider"></li>
                        <li>
                            <a href="#"><i class="fa fa-fw fa-repeat"></i> Restart</a>
                        </li>
                    </ul>
                </li>
            </ul>
        
            <!-- Sidebar Menu Items - These collapse to the responsive navigation menu on small screens -->
            <div class="collapse navbar-collapse navbar-ex1-collapse">
                <ul class="nav navbar-nav side-nav">
                    <li>
                        <a href="Index.aspx"><i class="fa fa-fw fa-dashboard"></i> Home</a>
                    </li>
                    <li class="active">
                        <a href="Charts.aspx"><i class="fa fa-fw fa-bar-chart-o"></i> Charts</a>
                    </li>
                    <li>
                        <a href="Validation.aspx"><i class="fa fa-fw fa-bar-chart-o"></i> Validation</a>
                    </li>
                </ul>
            </div>
            <!-- /.navbar-collapse -->
        </nav>

        <div id="page-wrapper">

            <div class="container-fluid">

                <!-- Page Heading -->
                <div class="row">
                    <div class="col-lg-12">
                        <div class="col-md-4">
                            <h1 class="page-header" style="border-bottom:0px">
                                Charts
                            </h1>
                        </div>
                        <div class="col-md-8">
                            <div class="page-header" style="border-bottom:0px">
                                <h4 style="color:#e74c3c">
                                    <i class="fa fa-fw fa-bar-chart-o"></i>Portfolio Value
                                </h4>
                                <h4 style="color:#3498db">
                                    <i class="fa fa-fw fa-bar-chart-o"></i>Product Value
                                </h4>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- /.row -->

                <div class="row">
                    <div class="col-lg-12">
                        <div class="panel panel-red">
                            <div class="panel-heading">
                                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Prix du produit calculé avec données réelles, taux d'intérêts constants, 5000 itérations de Monte Carlo, et une maturité de 8,5 an</h3>
                            </div>
                            <div class="panel-body">
                                <div id="price-1"></div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- /.row -->

                <div class="row">
                    <div class="col-lg-12">
                        <div class="panel panel-red">
                            <div class="panel-heading">
                                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Prix du produit calculé avec données simulées, en stressant la volatilité de 30%, taux d'intérêts constants, 1000 itérations de Monte Carlo et une maturité de 5 ans</h3>
                            </div>
                            <div class="panel-body">
                                <div id="price-2"></div>
                            </div>
                        </div>
                    </div>
                </div>
                

                <div class="row">
                    <div class="col-lg-12">
                        <div class="panel panel-red">
                            <div class="panel-heading">
                                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Prix du produit calculé avec données réelles, taux d'intérêts suivant le modèle de Vasicek, 1000 itérations de Monte Carlo et une maturité de 5 ans</h3>
                            </div>
                            <div class="panel-body">
                                <div id="price-3"></div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12">
                        <div class="panel panel-red">
                            <div class="panel-heading">
                                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Couverture du produit et des taux, avec des taux d'intérêts suivant le modèle de Vasicek, 1000 itérations et maturité 1 an</h3>
                            </div>
                            <div class="panel-body">
                                <div id="price-4"></div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12">
                        <div class="panel panel-red">
                            <div class="panel-heading">
                                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Résultat du calcul du prix d'une option Quanto de dimension 2, de strike 50, maturité 1, spot {500, 10} et de volatilité {0.4, 0,2}</h3>
                            </div>
                            <div class="panel-body">
                                <div class="col-md-6">
                                    <div id="quanto1"></div>
                                </div>
                                <div class="col-md-6">
                                    <div id="quanto2"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <!-- /.container-fluid -->

        </div>
        <!-- /#page-wrapper -->

    </div>
    <!-- /#wrapper -->

    <!-- jQuery -->
    <script src="js/jquery.js"></script>

    <!-- Bootstrap Core JavaScript -->
    <script src="js/bootstrap.min.js"></script>

    <!-- Morris Charts JavaScript -->
    <script src="js/plugins/morris/raphael.min.js"></script>
    <script src="js/plugins/morris/morris.min.js"></script>
    <script src="js/plugins/morris/morris-data.js"></script>

    <!-- Flot Charts JavaScript -->
    <!--[if lte IE 8]><script src="js/excanvas.min.js"></script><![endif]-->
    <script src="js/plugins/flot/jquery.flot.js"></script>
    <script src="js/plugins/flot/jquery.flot.tooltip.min.js"></script>
    <script src="js/plugins/flot/jquery.flot.resize.js"></script>
    <script src="js/plugins/flot/jquery.flot.pie.js"></script>
    <script src="js/plugins/flot/flot-data.js"></script>
</body>
</html>

