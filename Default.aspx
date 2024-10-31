<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SAhibo.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<link href ="Content/bootstrap.min.css" rel="stylesheet" type="text/css" />
<link rel="shortcut icon" href="../img/Banadesa.png"/> 
    <title>Servicio al Cliente Bnd</title>
    <style>
        body {
           /* background-color: #1E8449;*/
            /*background-image: linear-gradient(350deg, #b2e742 0, #9bdf47 16.67%, #83d64c 33.33%, #6acc50 50%, #50c154 66.67%, #33b757 83.33%, #00ae5b 100%);*/
            background-image: radial-gradient(circle at 56.99% 59.99%, #7db000 0, #65ae00 16.67%, #48ab08 33.33%, #15a815 50%, #00a420 66.67%, #00a12c 83.33%, #009d38 100%);
            

            font-family: Arial, sans-serif;
        }         
        h1 {
            color: green;
        }
        .contenedor {
            background-color: #1E8449;
            padding: 20px;
            }
        .imagen-transparente {
            width: 100px;
            height: 100px;
            align-items:center;
           /* 
               margin-top: 1rem;
           opacity: 0.9;   Ajusta la opacidad según tus necesidades (0 a 1) */
            }
        .centro {
            text-align: center;
        }
       
        #cuerpo{
        background-image: linear-gradient(
        to bottom,
        rgba(0, 255, 0, 0.5),
        rgba(0, 0, 255, 0.5)
        ),url('../img/bnd1.png');
        background-size: cover; /* La imagen cubrirá toda la pantalla */
        background-position: center; /* La imagen se centrará */
        background-attachment: fixed; /* La imagen se mantendrá fija */
           
        }

    </style>
    <%--<link rel="stylesheet" type="text/css" href="../Content/sweetalert2.min.css"/>--%>
    <script src="../Scripts/sweetalert2.all.min.js"></script>

</head>
<body id="cuerpo">
   
    <div class="contenedor">
       
         <div class="col-md-12 col-lg-12 text-center bg-success">
              <div class="row bg-success-subtle">      
                 <div class="col-md-2 col-lg-2 ">
                        <asp:Label ID="Label3" runat="server" class="text-success" Text="Banadesa" Font-Bold="True" Font-Size="xx-large"></asp:Label><br />
                 </div>

                 <div class="col-md-8 col-lg-8">           
                 </div> 
          
                 <div class="col-md-2 col-lg-2">
                        <img class="imagen-transparente" src="../img/Logo_Banadesa.png" alt="Imagen Transparente" />
                 </div> 
              </div>             
        </div>            
    </div>

    <form id="form2" runat="server" class="d-flex align-items-center justify-content-center mt-3">
        <div class="row">
            <div class="centro">
                <asp:Label ID="Label2" runat="server" class="text-white mt-1 text-center" Text="Bienvenido Servicio al Cliente:" Font-Bold="True" Font-Size="x-large"></asp:Label><br />
                <div class="col-auto bg-white p-5 text-center">

                    <asp:Label ID="Label1" runat="server" class="text-dark" Font-Bold="True" Text="Usuario:"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtUsuario" class="form-control" MaxLength="18" runat="server" onkeypress="return evitarEspacios(event)" ></asp:TextBox>
                    <br />
                    <asp:Label ID="LblContraseña" runat="server" class="text-dark" Font-Bold="True" Text="Contraseña:"></asp:Label>
                    <br />
                    <asp:TextBox ID="txtContraseña" class="form-control" MaxLength="50" runat="server" TextMode="Password" onkeypress="return evitarEspacios(event)"></asp:TextBox>
                    <asp:Label ID="lblError" runat="server" Font-Bold="True" ForeColor="#FF3300" Text="Error en contraseña o Usuario." Visible="False"></asp:Label>
                    <br />
                    <asp:Button ID="btnIngresar" class="btn btn-primary  btn-circle mt-2" runat="server" Text="Ingresar" OnClick="btnIngresar_Click" />
                </div>
            </div>   
            
        </div>
        
    </form>
    <br />
    <script>
        function evitarEspacios(e) {
            if (e.which === 32) {
                e.preventDefault();
                return false;
            }
        }
    </script>
</body>
</html>
