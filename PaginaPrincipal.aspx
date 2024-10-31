<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaginaPrincipal.aspx.cs" Inherits="SAhibo.PaginaPrincipal" Culture="es-ES" UICulture="es-ES" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="../Content/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="../Content/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="../Resources/StyleSheet.css" />
    <link rel="shortcut icon" href="../img/Banadesa.png" />

    
    <title>SAC</title>

    <style>
        body {
             background-color: #FDFEFE;
             /*background-image: linear-gradient(350deg, #b2e742 0, #9bdf47 16.67%, #83d64c 33.33%, #6acc50 50%, #50c154 66.67%, #33b757 83.33%, #00ae5b 100%);*/
            font-family: Arial, sans-serif;
        }
        
 /*Estilos para pantallas medianas y pequeñas (tablet y móvil) */
@media (width>= 912px) {
    
        .btnOpt {
                width: 230px;
                height: 50px;
            }
        #imgSalir{
                width: 80px;
                height: 50px;
        }
}

 /*Estilos para pantallas medianas y pequeñas (tablet y móvil) */
@media (width<912px) {
             .btnOpt {
                 width: 230px;
                 height: 50px;
             }
         #imgSalir{
                 width: 80px;
                 height: 50px;
         }
}

@media (width<=600px) {
             .btnOpt {
                 width: 280px;
                 height: 50px;
             }
         #imgSalir{
                 width: 80px;
                 height: 50px;
         }
}


    </style>

    <script src="../Scripts/sweetalert2.all.min.js"></script>
</head>
<body >

    <form id="form1" runat="server">

        <div class="container-fluid">

            <div class="col-md-12 col-lg-12 text-center bg-success">

                <div class="row">

                    <div class="col-md-2 col-lg-2 ">
                        <asp:Image ID="Image1" runat="server" Width="140px" ImageUrl="../img/Banadesa.png" TabIndex="99" />
                    </div>

                    <div class="col-md-8 col-lg-8">
                        <asp:Label ID="Label2" runat="server" class="text-white " Text="Sistema de Mantenimiento Servicio al Cliente BANADESA" Font-Bold="True" Font-Size="XX-Large"></asp:Label><br />
                        <asp:Label ID="lblU" runat="server" class="text-white " Text="." Font-Bold="True" Font-Size="X-Large"></asp:Label>
                        <br />
                        <asp:Button ID="btnDescargar" class="btn btn-outline-success bg-success-subtle btn-circle m-2 fw-bold rounded text-success" runat="server" Text="Descargar Plantilla" OnClick="DescargarArchivo_Click" />
                    </div>

                    <div class="col-md-2 col-lg-2">
                        <div class="col-auto bg-success text-center d-flex flex-column align-items-center mt-2 btn-container">
                            <asp:Button ID="btnOpt" runat="server" Visible="false" class="btn btn-outline-warning btn-sm m-2 fw-bold" Text="Admin Usuarios" OnClick="btnOpt_Click" />
        
                            <asp:LinkButton ID="btnSalir" runat="server" OnClick="btnSalir_Click" CssClass="btn btn-success btn-sm m-1 fw-bold">
                                <img id="imgSalir" src="../img/signout2.png" alt="Salir" class="icono m-1 fw-bold" />                                
                            </asp:LinkButton>
                        </div>
                    </div>

                </div>

            </div>

            <div class="row">

                <aside class="col-md-1 col-lg-1 d-md-block  sidebar">
                </aside>

                <main class="col-md-11 col-lg-11 px-md-1">

                    <div class="col-md-11 col-lg-11 px-md-1" runat="server" id="formIni">

                        <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left m-2 ">

                            <asp:Label ID="Label1" runat="server" class="text-white" Text="Buscar Por DNI:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtDNI" placeholder="Eje. 0101200010101" class="form-control" runat="server" onkeypress="return evitarCaracteresNoNumericos(event);" autocomplete="off"></asp:TextBox>
                            <br />
                            <asp:Label ID="LblNombreCli" runat="server" class="text-white" Text="Buscar Por Nombre:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtNombreCli" placeholder="Primer nombre" MaxLength="20" class="form-control" runat="server" autocomplete="off" oninput="convertirMayusculas(this);"></asp:TextBox>
                            <br />
                            <asp:TextBox ID="txtNombre2Cli" placeholder="Segundo nombre" MaxLength="20" class="form-control m-2" runat="server" autocomplete="off" oninput="convertirMayusculas(this);"></asp:TextBox>
                            <br />
                            <asp:Button ID="btn1" class="btn btn-primary btn-circle mt-2 fw-bold" runat="server" Text="Buscar" OnClick="btn1_Click" />
                            <br />
                        </div>

                        <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left m-2 ">

                            <asp:FileUpload ID="fileUploadExcel" runat="server" class="btn bg-success-subtle  btn-circle m-2 form-control" /><br />
                            <asp:Button ID="btnCargarExcel" class="btn btn-warning  btn-circle m-2 fw-bold" runat="server" Text="Cargar Excel" OnClick="btnCargarExcel_Click" />
                        </div>

                    </div>

                    <div class="col-md-11 col-lg-11 px-md-1" id="filaBtnGuardar" runat="server">
                        <div class="row align-items-center bg-success justify-content-left m-2">
                            <div class="col-md-3 col-lg-3">
                                <asp:CheckBox ID="chk1" runat="server" class="form-check-input text-dark" Visible="false" AutoPostBack="true" OnCheckedChanged="chk1_CheckedChanged" Text="No llenar Nombre:" />
                            </div>        
                            <div class="col-md-3 col-lg-4 d-flex align-items-center">
                                <asp:Label ID="lblNomBase" Visible="false" runat="server" class="form-label me-2 text-white" Text="Nombre Base:" style="margin-bottom: 0;" />
                                <asp:TextBox ID="txtNomBase" Visible="false" placeholder="Nombre Base" MaxLength="50" class="form-control" runat="server" autocomplete="off" />
                            </div>                            
                            <div class="col-md-2 col-lg-2">
                                <asp:Button ID="btnGuardarGrid" class="btn btn-warning btn-circle fw-bold m-2" Visible="false" runat="server" Text="Guardar Datos" OnClick="btnGuardarGrid_Click" />
                            </div>
                        </div>
                        <br />
                        <div class="table-responsive">
                            <asp:GridView ID="gv2" runat="server" Visible="False" class="col-md-12 justify-content-center table table-bordered table-condensed table-striped table-hover" style="font-size: 12px;">
                            </asp:GridView>
                        </div> 
                    </div>

                    <br />

                    <div class="col-md-11 col-lg-11 border border-3 bg-white" id="formularioModifica" runat="server" visible="false">

                        <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left m-1 ">

                            <asp:Label ID="Label3" runat="server" class="text-white" Text="Fecha:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtBEFENV" placeholder="20010101" class="form-control" runat="server" autocomplete="off" ReadOnly="True" onkeypress="return soloNumeros(event)"></asp:TextBox>
                            <br />
                            <asp:Label ID="Label4" runat="server" class="text-white" Text="Aplicaciòn:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtBETIBO" placeholder="0" class="form-control" runat="server" autocomplete="off" ReadOnly="True" onkeypress="return soloNumeros(event)"></asp:TextBox>
                            <br />
                            <asp:Label ID="Label6" runat="server" class="text-white" Text="Correlativo:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtBECORR" placeholder="0" class="form-control" runat="server" autocomplete="off" ReadOnly="True" onkeypress="return soloNumeros(event)"></asp:TextBox>
                            <br />
                        </div>

                        <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left m-1 ">

                            <asp:Label ID="Label5" runat="server" class="text-white" Text="DNI:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtBEICLI" placeholder="0000000000000" MaxLength="18" class="form-control" runat="server" autocomplete="off" onkeypress="return soloNumeros(event)"></asp:TextBox>
                            <br />
                            <asp:Label ID="Label7" runat="server" class="text-white" Text="Estado:"></asp:Label>
                            <br />

                            <asp:DropDownList ID="ddBESTAT" runat="server" class="form-control">
                                <asp:ListItem Value="E">ERROR</asp:ListItem>
                                <asp:ListItem Value="S">SUSPENDIDO</asp:ListItem>
                                <asp:ListItem Value="A">APLICADO</asp:ListItem>
                                <asp:ListItem Value="NULL">HISTORICO</asp:ListItem>
                                <asp:ListItem Value=" ">PENDIENTE</asp:ListItem>
                            </asp:DropDownList>

                            <%-- <asp:TextBox ID="txtBECODE" placeholder="0" class="form-control" runat="server" autocomplete="off"></asp:TextBox>--%>
                            <br />
                            <asp:Label ID="Label9" runat="server" class="text-white" Text="Monto:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtBEMONT" placeholder="0" class="form-control" runat="server" autocomplete="off" ReadOnly="true"></asp:TextBox>
                            <%--<asp:TextBox ID="txtBECOMU" placeholder="0" class="form-control" runat="server" autocomplete="off" onkeypress="return soloNumeros(event)"></asp:TextBox>--%>
                            <br />

                        </div>

                        <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left m-1 ">

                            <asp:Label ID="Label14" runat="server" class="text-white" Text="Primer Apellido:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtBEPAPE" placeholder="P. Apellido" MaxLength="20" class="form-control" runat="server" autocomplete="off" oninput="convertirMayusculas(this);"></asp:TextBox>
                            <br />
                            <asp:Label ID="Label15" runat="server" class="text-white" Text="Segundo Apellido:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtBESAPE" placeholder="S. Apellido" MaxLength="20" class="form-control" runat="server" autocomplete="off" oninput="convertirMayusculas(this);"></asp:TextBox>
                            <br />
                        </div>

                        <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left m-1 ">

                            <asp:Label ID="Label16" runat="server" class="text-white" Text="Primer Nombre:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtBEPNOM" placeholder="P. Nombre" MaxLength="20" class="form-control" runat="server" autocomplete="off" oninput="convertirMayusculas(this);"></asp:TextBox>
                            <br />
                            <asp:Label ID="Label17" runat="server" class="text-white" Text="Segundo Nombre:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtBESNOM" placeholder="S. Nombre" MaxLength="20" class="form-control" runat="server" autocomplete="off" oninput="convertirMayusculas(this);"></asp:TextBox>
                            <br />
                        </div>

                        <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left m-1 ">
                            <asp:Button ID="btn2" class="btn btn-danger fw-bold btn-circle m-2 form-control" runat="server" Text="Modificar" OnClick="btn2_Click" />
                            <br />
                        </div>

                    </div>

                    <div class="col-md-11 col-lg-11">
                        <div>
                            <br />
                            <div class="table-responsive">
                                <asp:GridView ID="gv1" runat="server" class=" col-md-12 justify-content-center table table-bordered table-condensed table table-striped table-hover " AutoGenerateSelectButton="True" OnSelectedIndexChanged="gv1_SelectedIndexChanged" Visible="False">
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-11 col-lg-11" id="formularioU" runat="server" visible="false">


                        <!-- INICIO -->

                         <div id="formularioDeshabilita" runat="server" visible="false">

                             <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left ">
                                 
                                 <asp:Label ID="lblNumPlanilla" runat="server" class="text-white" Text="Ingrese Numero Planilla:"></asp:Label>
                                 <br />
                                 <asp:TextBox ID="txtNumPlanilla" placeholder="" MaxLength="2" class="form-control" runat="server" onkeypress="return soloNumeros(event)" autocomplete="off"></asp:TextBox><br />

                             </div>

                             <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left m-1 ">
                                 <asp:Button ID="btnDP" class="btn btn-outline-warning fw-bold btn-circle m-1 form-control" runat="server" Text="Deshabilita Planilla Anterior" OnClick="btnDP_Click" />
                                 <br />
                                 <br />
                             </div>
                         </div>

                        <!-- FIN -->
                        <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left  ">
                            <asp:Label ID="lblUser1" runat="server" class="text-white" Text="Usuario:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtUsuario" placeholder="" MaxLength="18" class="form-control" runat="server" onkeypress="return evitarEspacios(event)" autocomplete="off"></asp:TextBox>
                            <br />
                            <asp:Label ID="lblContra" runat="server" class="text-white" Text="Contraseña:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtContraseña" placeholder="" MaxLength="50" class="form-control" runat="server" onkeypress="return evitarEspacios(event)" autocomplete="off"></asp:TextBox>
                            <br />

                        </div>

                        <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left  ">
                            <asp:Label ID="Label18" runat="server" class="text-white" Text="Nombre completo:"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtNombre" placeholder="" MaxLength="100" class="form-control" runat="server" autocomplete="off"></asp:TextBox>
                            <br />
                            <label class="text-white">Es Administrador:</label>
                            <div class="form-check">
                                <input class="form-check-input " type="checkbox" runat="server" value="" id="flexCheckDefault" />
                            </div>
                            <br />
                        </div>

                        <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left m-1 ">
                            <asp:Button ID="btnNuevoU" class="btn btn-outline-warning  btn-circle m-2 form-control fw-bold" runat="server" Text="Crear Nuevo" OnClick="btnNuevoU_Click" />
                            <br />
                            <asp:Button ID="btnMuestraIni" class="btn btn-outline-warning fw-bold btn-circle m-1 form-control" runat="server" Text="Mostrar Inicio" OnClick="btnMuestraIni_Click" />
                        </div>

                        <div>
                            <div class="table-responsive">
                                <asp:GridView ID="gvU" runat="server" class=" col-md-12 justify-content-center table table-bordered table-condensed table table-striped table-hover " AutoGenerateSelectButton="True" OnSelectedIndexChanged="gvU_SelectedIndexChanged" Visible="False">
                                </asp:GridView>
                            </div>
                        </div>

                        <div id="formularioUM" runat="server" visible="false">

                            <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left ">
                                <asp:Label ID="Label8" runat="server" class="text-white" Text="Usuario:"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtUsuM" placeholder="" ReadOnly="true" MaxLength="18" class="form-control" runat="server" onkeypress="return evitarEspacios(event)" autocomplete="off"></asp:TextBox>
                                <br />
                                <asp:Label ID="Label10" runat="server" class="text-white" Text="Contraseña:"></asp:Label>
                                <br />
                                <asp:TextBox ID="txtContraM" placeholder="" MaxLength="50" class="form-control" runat="server" onkeypress="return evitarEspacios(event)" autocomplete="off"></asp:TextBox><br />

                                <asp:Label ID="Label11" runat="server" class="text-white" Text="Estado:"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddEstadoM" runat="server" class="form-control">
                                    <asp:ListItem Value="I">INACTIVO</asp:ListItem>
                                    <asp:ListItem Value="A">ACTIVO</asp:ListItem>
                                </asp:DropDownList><br />

                                <asp:Label ID="Label12" runat="server" class="text-white" Text="Permisos:"></asp:Label>
                                <br />
                                <asp:DropDownList ID="ddAdminPer" runat="server" class="form-control">
                                    <asp:ListItem Value="N">NORMAL</asp:ListItem>
                                    <asp:ListItem Value="A">ADMINISTRADOR</asp:ListItem>
                                </asp:DropDownList><br />

                            </div>

                            <div class="col-auto bg-success p-3 text-center d-flex align-items-center justify-content-left m-1 ">
                                <asp:Button ID="btnUM" class="btn btn-outline-warning fw-bold btn-circle m-1 form-control" runat="server" Text="Actualizar Usuario" OnClick="btnUM_Click" />
                                <br />
                                <br />
                            </div>

                        </div>

                    </div>

                    <br />

                </main>

            </div>

        </div>

    </form>


    <script type="text/javascript">

        function resaltaBordes(textBoxId) {
            var textBox = document.getElementById(textBoxId);

            // Agregar la clase CSS para resaltar los bordes
            textBox.classList.add("bordeRojo");

            // Esperar 3 segundos (3000 milisegundos) y luego quitar la clase CSS
            setTimeout(function () {
                textBox.classList.remove("bordeRojo");
            }, 3000);
        }
        function convertirMayusculas(input) {
            input.value = input.value.toUpperCase();
        }

        const toggleMenu = () => {
            document.querySelector('#menu').classList.toggle('open');
        }
        function soloNumeros(e) {
            var keyCode = e.keyCode || e.which;
            if ((keyCode < 48 || keyCode > 57) && keyCode !== 8 && keyCode !== 9 && keyCode !== 13) {
                //alert('Solo se permiten números.');
                return false;
            }

            // Permitir otras teclas
            return true;
        }
        function evitarEspacios(e) {
            if (e.which === 32) {

                // Si la tecla presionada es un espacio, prevenimos su inserción
                e.preventDefault();

                return false;
            } else {
                if (soloNumeros(e) == true) {

                } else {
                    //eliminarLetra(e);
                    //alert('Solo valores numericos:' + e);
                    //var str = document.getElementById('txtDNI').value;
                    //str = str.slice(0, str.length - 1);
                    //console.log(str);
                    //document.getElementById('txtDNI').innerHTML = str;
                }
            }
        }
        function evitarCaracteresNoNumericos(e) {
            var charCode = e.which || e.keyCode;

            // Verificar si la tecla presionada es un número (0-9)
            if (charCode >= 48 && charCode <= 57) {
                return true; // Permitir números
            } else {
                // Si la tecla no es un número, prevenimos su inserción
                e.preventDefault();
                return false;
            }
        }
    </script>

</body>
</html>
