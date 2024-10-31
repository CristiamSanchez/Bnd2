using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using OfficeOpenXml;
using System.Collections;
using System.Net;
using System.Security.Cryptography;
using Devart.Common;
using System.Data.SqlClient;
using static System.Net.WebRequestMethods;
using System.Threading;
using static Devart.Common.Utils;
using OracleInternal.Secure.Network;
using System.Web.DynamicData;
 
namespace SAhibo
{
    public partial class PaginaPrincipal : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["cnS"].ConnectionString;
        private static bool eventoEjecutado = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //formularioModifica.Visible = !formularioModifica.Visible;               
            }
            // Verificar si la variable de sesión existe
            if (Session["NombUsu"] != null)
            {
                // Recuperar el valor de la variable de sesión
                string nombreUsuario = Session["NombUsu"].ToString();
                evaluaU(nombreUsuario);
                // Utilizar el valor como sea necesario
                lblU.Text = "Hola, " + nombreUsuario + "!";
                //btnDescargar.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                // Redirigimos a la pagina de login
                Response.Redirect("Default.aspx");
            }
            eventoEjecutado = false;
        }

        protected void btn1_Click(object sender, EventArgs e)
        {
            // Crear un hilo adicional para manejar el tiempo
            Thread tiempoThread = new Thread(new ThreadStart(ManejarTiempo));
            tiempoThread.Start();


            if (txtDNI.Text.Length > 0 || txtNombreCli.Text.Length>0) {
                if (txtDNI.Text.Length == 13)  {   LlenarGV(txtDNI.Text,"");   }
                else {LlenarGV(txtNombreCli.Text, txtNombre2Cli.Text);    }
            }else{
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Advertencia', text: 'Debes llenar informaciòn correspondiente!' });", true);
            }

            // Marcar que el evento se ha ejecutado
            eventoEjecutado = true;
            // Detener el hilo de tiempo
            tiempoThread.Abort();
        }

        private void ManejarTiempo()
        {
            // Configurar el tiempo límite en segundos
            int tiempoLimite = 60;
            // Esperar hasta que el tiempo límite se alcance o el evento se haya ejecutado
            for (int i = 0; i < tiempoLimite && !eventoEjecutado; i++)
            {
                Thread.Sleep(1000); // Dormir durante 1 segundo
            }
            // Si el evento no se ha ejecutado después del tiempo límite, puedes realizar acciones adicionales aquí
            if (!eventoEjecutado)
            {
                // Realizar acciones adicionales o lanzar excepciones según sea necesario
                // ...
            }
        }

        private void LlenarGV(string codex, string codex2)
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    string ultimaParte = "";
                    if (double.TryParse(codex, out double resultadoDecimal))
                    { ultimaParte = " BEICLI ='" + codex + "'"; }
                    else
                    { ultimaParte = " BEPNOM LIKE '%" + codex + "%' AND BESNOM LIKE '%" + codex2 + "%'"; }


                    con.Open();
                    using (OracleCommand cmd = new OracleCommand("DPBNDAT.BuscarDatos", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_codex", OracleDbType.Varchar2).Value = ultimaParte;
                        cmd.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)  {    gv1.DataSource = reader;    gv1.DataBind();    }
                            else { gv1.DataSource = null;   gv1.DataBind();   }
                        }
                    }
                    ocultaMuestra("2");
                    gv1.Visible = true;
                }
                catch (Exception ex)  {  LogErrores("LG-1", ex.Message);     }
                finally   {    con.Close();        }
            }
        }

        protected void LogErrores(string codexE, string msgE)
        {
            using (OracleConnection conexion = new OracleConnection(connectionString))
            {
                conexion.Open();
                try
                {
                    using (OracleCommand comando = new OracleCommand("APPSBANADESA.InsertarBELOGE", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Add("p_NombUsu", OracleDbType.Varchar2).Value = Session["NombUsu"];
                        comando.Parameters.Add("p_CodexE", OracleDbType.Varchar2).Value = codexE;
                        comando.Parameters.Add("p_MsgE", OracleDbType.Varchar2).Value = msgE;
                        comando.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "alert('Cantidad de registros afectados:. " + ex.Message.ToString() + "');", true);                }
                } finally  {   conexion.Close();    }
            }
        }

        protected void LogTrx(string codexE, string cli)
        {
            using (OracleConnection conexion = new OracleConnection(connectionString))
            {
                conexion.Open();
                try
                {
                    using (OracleCommand comando = new OracleCommand("APPSBANADESA.InsertarBELOG", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        // Parámetros del procedimiento almacenado
                        comando.Parameters.Add("p_NombUsu", OracleDbType.Varchar2).Value = Session["NombUsu"];
                        comando.Parameters.Add("p_TipoTrx", OracleDbType.Varchar2).Value = codexE;
                        comando.Parameters.Add("p_Clie", OracleDbType.Varchar2).Value = cli;
                        comando.ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { LogErrores("LT-1", ex.Message);       }
                finally     {     conexion.Close();        }
            }
        }

        protected void gv1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtiene el índice de la fila seleccionada
            int rowIndex = gv1.SelectedIndex;
            // Verifica si se seleccionó alguna fila
            if (rowIndex >= 0 && rowIndex < gv1.Rows.Count)
            {
                try
                {
                    txtBEFENV.Text = HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[1].Text);
                    txtBETIBO.Text = HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[2].Text);
                    txtBEICLI.Text = HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[3].Text);
                    txtBECORR.Text = HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[4].Text);
                    
                    txtBEPAPE.Text = HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[6].Text);
                    txtBESAPE.Text = HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[7].Text);
                    txtBEPNOM.Text = HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[8].Text);
                    txtBESNOM.Text = HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[9].Text);
                    txtBEMONT.Text = HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[10].Text);
                    txtDNI.Text = HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[3].Text);


                    //Si esta deshabilitado, aprovecho para habilitarlo
                    if (ddBESTAT.Enabled == false)  {   ddBESTAT.Enabled = true;       }

                    switch (HttpUtility.HtmlDecode(gv1.Rows[rowIndex].Cells[5].Text))
                    {
                        case "ERROR":
                            ddBESTAT.SelectedIndex = 0;
                            break;
                        case "SUSPENDIDO":
                            ddBESTAT.SelectedIndex = 1;
                            break;
                        case "APLICADO":
                            ddBESTAT.SelectedIndex = 2;
                            break;
                        case "HISTORICO":
                            ddBESTAT.SelectedIndex = 3;
                            break;
                        case "PENDIENTE":
                            ddBESTAT.SelectedIndex = 4;
                            break;
                        default:
                            ddBESTAT.SelectedIndex = 4;
                            break;
                    }
                    //Si el registro ya fue cobrado o cambiado de estado, entonces bloqueo
                    if(ddBESTAT.SelectedIndex != 4)  { ddBESTAT.Enabled = false;    }

                    formularioModifica.Visible = true;
                }
                catch(Exception ex) { Console.WriteLine( ex.Message); }               

            }
        }

        protected void btn2_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddBESTAT.SelectedValue =="NULL") {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Advertencia', text: 'NO esta permitido Actualizar en Historico!' });", true);                   
                }
                else {
                    ActualizarRegistro(txtBEFENV.Text, txtBETIBO.Text, txtBEICLI.Text, txtBECORR.Text, txtDNI.Text, ddBESTAT.SelectedValue,
                                       txtBEMONT.Text, txtBEPAPE.Text, txtBESAPE.Text, txtBEPNOM.Text, txtBESNOM.Text);    }               
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }                  
        }

        private void ActualizarRegistro(string id, string id2, string id3, string id4, string id5, string nuevaColumna1, string nuevaColumna2, string nuevaColumna3, string nuevaColumna4, string nuevaColumna5, string nuevaColumna6)
        {
            using (OracleConnection conexion = new OracleConnection(connectionString))
            {
                try
                {
                    txtDNI.ReadOnly = true;
                    string cadenita = verificaCambio(id, id2, id5, id4);
                    conexion.Open();

                    using (OracleCommand command = new OracleCommand("DPBNDAT.ActualizarDatos", conexion))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros de entrada
                        command.Parameters.Add("p_id", OracleDbType.Varchar2).Value = id;
                        command.Parameters.Add("p_id2", OracleDbType.Varchar2).Value = id2;
                        command.Parameters.Add("p_id3", OracleDbType.Varchar2).Value = id3;
                        command.Parameters.Add("p_id4", OracleDbType.Varchar2).Value = id4;
                        command.Parameters.Add("p_id5", OracleDbType.Varchar2).Value = id5;
                        command.Parameters.Add("p_nuevaColumna1", OracleDbType.Varchar2).Value = nuevaColumna1;
                        command.Parameters.Add("p_nuevaColumna2", OracleDbType.Varchar2).Value = nuevaColumna2;
                        command.Parameters.Add("p_nuevaColumna3", OracleDbType.Varchar2).Value = nuevaColumna3;
                        command.Parameters.Add("p_nuevaColumna4", OracleDbType.Varchar2).Value = nuevaColumna4;
                        command.Parameters.Add("p_nuevaColumna5", OracleDbType.Varchar2).Value = nuevaColumna5;
                        command.Parameters.Add("p_nuevaColumna6", OracleDbType.Varchar2).Value = nuevaColumna6;
                        command.ExecuteNonQuery();
                        // Ejecutar el procedimiento almacenado                        
                        int filasActualizadas = 1;
                        //ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "alert('Actualizaciòn èxitosa:. "+ filasActualizadas+"');", true);
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Éxito', 'Actualizaciòn èxitosa:. "+ filasActualizadas+"', 'success');", true);

                        // Resto del código...
                        formularioModifica.Visible = !formularioModifica.Visible;
                        filaBtnGuardar.Visible = !filaBtnGuardar.Visible;
                        /* RECARGO grid*/
                        if (txtDNI.Text.Length > 0 || txtNombreCli.Text.Length > 0)
                        {
                            if (txtDNI.Text.Length == 13) {  LlenarGV(txtDNI.Text, "");  }
                            else {  LlenarGV(txtNombreCli.Text, txtNombre2Cli.Text); }
                        }
                        if (txtDNI.ReadOnly == true) { txtDNI.ReadOnly = false; txtDNI.Text = ""; }
                        LogTrx("AR-1", id5 + cadenita);
                    }
                }
                catch (Exception ex) { LogErrores("AR-1", ex.Message); }                
                finally  { conexion.Close(); txtDNI.ReadOnly = false; txtDNI.Text = ""; }
            
            }
        }

        protected void btnCargarExcel_Click(object sender, EventArgs e)
        {
            if (fileUploadExcel.HasFile)
            {
                string rutaTemporal = Server.MapPath("~/ArchivosTemporales/") + Guid.NewGuid() + ".xlsx";

                try
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage())
                    {
                        // Verifica si el directorio existe, y si no, créalo.
                        if (!Directory.Exists(Path.GetDirectoryName(rutaTemporal)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(rutaTemporal));
                        }
                        // Guardar el archivo subido en el servidor
                        fileUploadExcel.SaveAs(rutaTemporal);

                        DataTable dtExcel = LeerExcel(rutaTemporal);
                        //Limpio primero el grid
                        gv2.DataSource = null;
                        gv2.DataBind();
                        //Lleno despues el grid
                        gv2.DataSource = dtExcel;
                        gv2.DataBind();
                        ocultaMuestra("1");
                        txtDNI.Text = "";
                        txtNombreCli.Text = "";
                        txtNombre2Cli.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Error al leer Excel', text: 'Favor revisar la estructura indicada!' });", true);
                    Console.WriteLine(ex.Message.ToString());
                    LogErrores("CE-1", ex.Message);
                }
                finally
                {
                    // Eliminar el archivo temporal
                    if (System.IO.File.Exists(rutaTemporal))
                    {
                        System.IO.File.Delete(rutaTemporal);
                    }
                }
            }

            #region Codigo Anterior
            //if (fileUploadExcel.HasFile)
            //{
            //    try
            //    {
            //        string rutaTemporal = Server.MapPath("~/ArchivosTemporales/") + Guid.NewGuid() + ".xlsx";
            //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //        using (var package = new ExcelPackage())
            //        {
            //            // Verifica si el directorio existe, y si no, créalo.
            //            if (!Directory.Exists(Path.GetDirectoryName(rutaTemporal)))   {  Directory.CreateDirectory(Path.GetDirectoryName(rutaTemporal));   }

            //            // Guardar el archivo subido en el servidor
            //            fileUploadExcel.SaveAs(rutaTemporal);

            //            DataTable dtExcel = LeerExcel(rutaTemporal);
            //            gv2.DataSource = dtExcel;
            //            gv2.DataBind();
            //            ocultaMuestra("1");
            //            txtDNI.Text = "";
            //            txtNombreCli.Text = "";
            //            txtNombre2Cli.Text = "";

            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Error al leer Excel', text: 'Favor revisar la estructura indicada!' });", true);
            //        Console.WriteLine(ex.Message.ToString());
            //        LogErrores("CE-1", ex.Message);
            //    }
            //}


            #endregion

        }

        private DataTable LeerExcel(string rutaArchivo)
        {
            if (txtDNI.ReadOnly == true) { txtDNI.ReadOnly = false; }
            using (ExcelPackage package = new ExcelPackage(new FileInfo(rutaArchivo)))
            {
                int tibo = 0;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                DataTable dt = new DataTable();

                foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {      dt.Columns.Add(firstRowCell.Text);       }

                for (var rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                {
                    var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                    

                    if (row.Any(cell => !string.IsNullOrEmpty(cell.Text)))
                    {
                        var newRow = dt.Rows.Add();
                        foreach (var cell in row)
                        {
                            if(cell.AddressAbsolute == "$A$2")
                            {
                                tibo = Convert.ToInt32(cell.Text);
                            }
                            newRow[cell.Start.Column - 1] = cell.Text;
                        }
                    }
                }
                if(tibo != 95) { txtNomBase.Visible = true; chk1.Visible = true; lblNomBase.Visible = true; }
                return dt;
            }           
        }

        protected void chk1_CheckedChanged(object sender, EventArgs e)
        {
            if (chk1.Checked)
            {
                txtNomBase.Visible = false; txtNomBase.Text = string.Empty;
                lblNomBase.Visible = false;
            }
            else
            {
                txtNomBase.Visible = true;
                lblNomBase.Visible = true;
            }
        }

        protected void btnGuardarGrid_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            int anio = dt.Year;
            string mes = dt.Month.ToString("D2");
            string dia = dt.Day.ToString("D2");
            string fechaHoy = Convert.ToString(anio + mes + dia);    
            int filasActualizadas = 0;
            int registrosEnBD = 0;
            int numApli = 0;

            if (txtNomBase.Visible== true && txtNomBase.Text.Length == 0)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Advertencia', 'Debe de llenar el nombre de la base!', 'warning');", true);
            }
            else
            {
                try
                {
                    foreach (GridViewRow fila in gv2.Rows)
                    {
                        string valor14 = HttpUtility.HtmlDecode(fila.Cells[13].Text);
                        numApli = Convert.ToInt32(HttpUtility.HtmlDecode(fila.Cells[0].Text));

                        string valorAlca = HttpUtility.HtmlDecode(fila.Cells[4].Text);
                        if (valorAlca.Length > 40) { valorAlca = valorAlca.Substring(0, 40);    }
                        string valorNCCE = HttpUtility.HtmlDecode(fila.Cells[5].Text);
                        if (valorNCCE.Length > 40) { valorNCCE = valorNCCE.Substring(0, 40); }
                        string valorICLI = HttpUtility.HtmlDecode(fila.Cells[7].Text);
                        if (valorICLI.Length > 13) { valorICLI = valorICLI.Substring(0, 13); }
                        string valorPAPE = HttpUtility.HtmlDecode(fila.Cells[8].Text);
                        if (valorPAPE.Length > 20) { valorPAPE = valorPAPE.Substring(0, 20); }
                        string valorSAPE = HttpUtility.HtmlDecode(fila.Cells[9].Text);
                        if (valorSAPE.Length > 20) { valorSAPE = valorSAPE.Substring(0, 20); }
                        string valorPNOM = HttpUtility.HtmlDecode(fila.Cells[10].Text);
                        if (valorPNOM.Length > 20) { valorPNOM = valorPNOM.Substring(0, 20); }
                        string valorSNOM = HttpUtility.HtmlDecode(fila.Cells[11].Text);
                        if (valorSNOM.Length > 20) { valorSNOM = valorSNOM.Substring(0, 20); }

                        if (double.TryParse(valor14, out double numeroConDecimales))
                        {
                            // Redondear el número para eliminar los decimales
                            int numeroSinDecimales = (int)Math.Round(numeroConDecimales);
                            // Convertir el número redondeado a cadena
                            valor14 = numeroSinDecimales.ToString();
                            using (OracleConnection connection = new OracleConnection(connectionString))
                            {
                                connection.Open();
                                using (OracleCommand command = new OracleCommand("DPBNDAT.InsertarActualizarDatos", connection))
                                {
                                    try
                                    {
                                        command.CommandType = CommandType.StoredProcedure;
                                        //// Agregar parámetros de entrada
                                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = Convert.ToString(fechaHoy);
                                        command.Parameters.Add("p_TIBO", OracleDbType.Int32).Value = Convert.ToInt32(HttpUtility.HtmlDecode(fila.Cells[0].Text));
                                        command.Parameters.Add("p_ICLI", OracleDbType.Varchar2).Value = valorICLI;
                                        command.Parameters.Add("p_CORR", OracleDbType.Int32).Value = Convert.ToInt32(HttpUtility.HtmlDecode(fila.Cells[1].Text));
                                        command.Parameters.Add("p_DEDE", OracleDbType.Varchar2).Value = HttpUtility.HtmlDecode(fila.Cells[2].Text);
                                        command.Parameters.Add("p_DEMU", OracleDbType.Varchar2).Value = HttpUtility.HtmlDecode(fila.Cells[3].Text);
                                        command.Parameters.Add("p_ALCA", OracleDbType.Varchar2).Value = valorAlca;
                                        command.Parameters.Add("p_NCCE", OracleDbType.Varchar2).Value = valorNCCE;
                                        command.Parameters.Add("p_PAPE", OracleDbType.Varchar2).Value = valorPAPE;
                                        command.Parameters.Add("p_SAPE", OracleDbType.Varchar2).Value = valorSAPE;
                                        command.Parameters.Add("p_PNOM", OracleDbType.Varchar2).Value = valorPNOM;
                                        command.Parameters.Add("p_SNOM", OracleDbType.Varchar2).Value = valorSNOM;
                                        command.Parameters.Add("p_MONT", OracleDbType.Int32).Value = Convert.ToInt32(valor14);

                                        command.Parameters.Add("p_filasActualizadas", OracleDbType.Int32).Direction = ParameterDirection.Output;

                                        if (verificaK($"{anio}{mes}{dia}", HttpUtility.HtmlDecode(fila.Cells[0].Text), HttpUtility.HtmlDecode(fila.Cells[7].Text), HttpUtility.HtmlDecode(fila.Cells[1].Text))) { registrosEnBD += 1; }
                                        else { command.ExecuteNonQuery(); filasActualizadas += Convert.ToInt32(command.Parameters["p_filasActualizadas"].Value.ToString()); }
                                        ocultaMuestra("3");
                                    }
                                    catch (Exception ex) { LogErrores("IG-1", ex.Message); }
                                    finally { connection.Close(); }
                                }
                            }
                        }
                        else
                        { registrosEnBD += 1; }
                    }
                    LogTrx("GG-1", User + "GuardaGrid-"+ fechaHoy+"-"+ numApli);
                    if (txtNomBase.Visible && txtNomBase.Text.Length > 0)
                    { ActualizaTbls(numApli, txtNomBase.Text); txtNomBase.Text = ""; txtNomBase.Visible = false; lblNomBase.Visible = false; chk1.Checked = false; chk1.Visible = false; }
                    else { chk1.Checked = false; chk1.Visible = false; txtNomBase.Text = ""; txtNomBase.Visible = false; lblNomBase.Visible = false; }
                }
                catch (Exception ex)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ServerControlScript", "alert('Error en lectura de Excel!\\n\\n'" + ex.Message + ");", true);
                    return;
                }

                if (filasActualizadas > 0 && registrosEnBD > 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Insercion exitosa!', 'Cantidad de registros Insertados: " + filasActualizadas + " \\nRegistros(filas) no ingresadas: " + registrosEnBD + "', 'success');", true);
                }
                else if (filasActualizadas == 0 && registrosEnBD == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Advertencia', text: 'Datos ya fueron insertados!' });", true);
                }
                else if (filasActualizadas > 0 && registrosEnBD == 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Insercion exitosa!', 'Cantidad de registros Insertados: " + filasActualizadas + "', 'success');", true);

                }
                else if (filasActualizadas == 0 && registrosEnBD > 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Advertencia', text: 'Datos ya fueron insertados!' });", true);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Inserción exitosa!', 'Cantidad de registros Insertados: " + filasActualizadas + " \\nRegistros(filas) no ingresadas: " + registrosEnBD + "', 'success');", true);
                }

                limpiaCarpeta();
            }

        }

        protected void DescargarArchivo_Click(object sender, EventArgs e)
        {
            // Ruta del archivo Excel en el proyecto
            string rutaArchivo = Server.MapPath("~/ArchivosExcel/Modelo.xlsx");
            // Nombre del archivo que se mostrará al descargar
            string nombreArchivo = "Formato.xlsx";
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + nombreArchivo);
            Response.TransmitFile(rutaArchivo);
            Response.End();
        }

        private void limpiaCarpeta()
        {
            try
            {
                DirectoryInfo tempDir = new DirectoryInfo(Server.MapPath("~/ArchivosTemporales/"));
                // Verifica si el directorio temporal existe
                if (tempDir.Exists)
                {
                    // Obtén la fecha actual
                    DateTime currentDate = DateTime.Now;
                    // Itera a través de los archivos en el directorio temporal
                    foreach (FileInfo file in tempDir.GetFiles())
                    {
                        // Comprueba si el archivo fue creado hace más de un día
                        if (currentDate.Subtract(file.CreationTime).Days > 1)
                        {
                            // Elimina el archivo si ha pasado más de un día desde su creación
                            file.Delete();
                        }
                    }
                }
            }
            catch (Exception ex){  LogErrores("LC-1", ex.Message);      }
        }

        private void ActualizaTbls(int numBase, string nombreBase)
        {
            string numBaseCorregido = "0000000000";
            try
            {
                if (numBase.ToString().Length == 1) { numBaseCorregido += "0" + numBase; }
                else { numBaseCorregido += + numBase; }

                using (OracleConnection conexion = new OracleConnection(connectionString))
                {
                    conexion.Open();
                    try
                    {
                        using (OracleCommand command = new OracleCommand("DPBNDAT.ActualizarDatosTIBOTTLA", conexion))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            // Agregar parámetros de entrada
                            command.Parameters.Add("p_id", OracleDbType.Varchar2).Value = Convert.ToString(numBase);
                            command.Parameters.Add("p_id2", OracleDbType.Varchar2).Value = numBaseCorregido;
                            command.Parameters.Add("p_id3", OracleDbType.Varchar2).Value = nombreBase;
                            command.ExecuteNonQuery();
                        }

                        using (OracleCommand command = new OracleCommand("JTELLDATV2.ActualizarDatosTIBOTTLA", conexion))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            // Agregar parámetros de entrada
                            command.Parameters.Add("p_id", OracleDbType.Varchar2).Value = Convert.ToString(numBase);
                            command.Parameters.Add("p_id2", OracleDbType.Varchar2).Value = numBaseCorregido;
                            command.Parameters.Add("p_id3", OracleDbType.Varchar2).Value = nombreBase;
                            command.ExecuteNonQuery();
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Éxito', 'Actualizaciòn èxitosa 2', 'success');", true);
                        }                        
                    }
                    catch (Exception ex) { LogErrores("AR-1", ex.Message); }
                    finally { conexion.Close(); }
                }
            }
            catch (Exception ex) { LogErrores("LC-1", ex.Message); }
        }

        private void ocultaMuestra(string opcionSeleccionada)
        {           

            switch (opcionSeleccionada)
            {
                case "1":
                    
                    formularioModifica.Visible = false;
                    filaBtnGuardar.Visible = true;
                    gv2.Visible = true;
                    btnGuardarGrid.Visible= true;
                    gv1.Visible = false;
                    break;
                case "2":                    

                    filaBtnGuardar.Visible = false;
                    gv2.Visible = false;
                    btnGuardarGrid.Visible = false;
                    gv1.Visible = true;
                    formularioModifica.Visible = false;

                    break;
                case "3":
                    gv2.Visible = false;
                    btnGuardarGrid.Visible = false;

                    break;
                case "4":
                    btnOpt.Visible = true;
                    llenarGVU();                    
                    break;
                case "5":
                    formularioU.Visible = true;
                    gvU.Visible=true;
                    formularioDeshabilita.Visible = true;
                    formIni.Visible = false;
                    filaBtnGuardar.Visible = false;
                    gv2.Visible = false;
                    gv1.Visible = false;
                    btnGuardarGrid.Visible = false;
                    formularioModifica.Visible = false;                    
                    break;
                case "6":
                    formularioU.Visible = false;
                    gvU.Visible = false;
                    formIni.Visible = true;
                    formularioDeshabilita.Visible = false;
                    break;
                default:
                    formularioModifica.Visible = !formularioModifica.Visible;
                    filaBtnGuardar.Visible = !filaBtnGuardar.Visible;
                    break;
            }
        }

        private bool verificaK(string fecha, string apli, string dni, string corre)
        {
            int count = 0;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("DPBNDAT.VerificarExistencia", connection))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        // Agregar parámetros de entrada
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = fecha;
                        command.Parameters.Add("p_apli", OracleDbType.Varchar2).Value = apli;
                        command.Parameters.Add("p_dni", OracleDbType.Varchar2).Value = dni;
                        command.Parameters.Add("p_corre", OracleDbType.Varchar2).Value = corre;
                        // Agregar parámetro de salida
                        command.Parameters.Add("p_count", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        // Ejecutar el procedimiento almacenado
                        command.ExecuteNonQuery();
                        // Obtener el resultado
                        count = Convert.ToInt32(command.Parameters["p_count"].Value.ToString());
                    }
                    catch (Exception ex)  {   LogErrores("VK-1", ex.Message);     }
                    finally    {      connection.Close();  }
                }
            }

            if (count >= 1) {   return true; }  else    {  return false;     }
        }
        
        private string verificaCambio(string fecha, string apli, string dni, string corre){

            string  cadena = "";
            string encadena2 = "";
            using (OracleConnection conexion = new OracleConnection(connectionString))
            {
                conexion.Open();
                using (OracleCommand comando = new OracleCommand("DPBNDAT.ObtenerDatos", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;

                    comando.Parameters.Add("p_Fecha", OracleDbType.Varchar2).Value = fecha;
                    comando.Parameters.Add("p_Aplicacion", OracleDbType.Varchar2).Value = apli;
                    comando.Parameters.Add("p_DNI", OracleDbType.Varchar2).Value = dni;
                    comando.Parameters.Add("p_Corre", OracleDbType.Varchar2).Value = corre;

                    comando.Parameters.Add("p_Cadena", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
                    comando.Parameters["p_Cadena"].Size = 4000;

                    comando.ExecuteNonQuery();

                    cadena = comando.Parameters["p_Cadena"].Value.ToString();

                    if (!string.IsNullOrEmpty(cadena))
                    {
                        string[] valores = cadena.Split(',');
                        int contador = -1;
                        foreach (string valor in valores)
                        {
                            contador += 1;
                            switch (contador)
                            {
                                case 2:
                                    if (txtDNI.Text != valor) { encadena2 += "- DNI:" + valor; }
                                    break;
                                case 4:
                                    if (ddBESTAT.SelectedValue != valor) { encadena2 += "- ESTADO:" + valor; }
                                    break;
                                case 11:
                                    if (txtBEPAPE.Text != valor) { encadena2 += "- PAPE:" + valor; }
                                    break;
                                case 12:
                                    if (txtBESAPE.Text != valor) { encadena2 += "- SAPE:" + valor; }
                                    break;
                                case 13:
                                    if (txtBEPNOM.Text != valor) { encadena2 += "- PNOM:" + valor; }
                                    break;
                                case 14:
                                    if (txtBESNOM.Text != valor) { encadena2 += "- SNOM:" + valor; }
                                    break;
                                case 15:
                                    if (txtBEMONT.Text != valor) { encadena2 += "- MONT:" + valor; }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("La cadena está vacía.");
                    }
                }
            }

            return encadena2;
        }
             
        protected void btnSalir_Click(object sender, EventArgs e)
        {
            // Limpiar las credenciales de usuario de la sesión
            Session.Clear();
            Session.Abandon();
            // Redirigir a la página de inicio de sesión o a la página principal
            Response.Redirect("~/Default.aspx");
        }

        private void evaluaU(string codex)
        {
            var esUsuarioAutenticado = 0;
            using (OracleConnection conexion = new OracleConnection(connectionString))
            {
                try
                {
                    conexion.Open();
                    using (OracleCommand comando = new OracleCommand("APPSBANADESA.EvaluarUsu", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Add("p_NombreUsuario", OracleDbType.Varchar2).Value = codex.Trim();
                        comando.Parameters.Add("p_EsUsuarioAutenticado", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        comando.ExecuteNonQuery();
                        esUsuarioAutenticado = Convert.ToInt32(comando.Parameters["p_EsUsuarioAutenticado"].Value.ToString());
                    }
                }
                catch (Exception ex)
                {  Console.WriteLine("Error: " + ex.Message);  }
                if (esUsuarioAutenticado > 0)
                { ocultaMuestra("4"); }
            }
        }

        protected void btnOpt_Click(object sender, EventArgs e)
        {
            // Mostrar Controles
            ocultaMuestra("5");
        }

        protected void btnNuevoU_Click(object sender, EventArgs e)
        {
            var nUsu = txtUsuario.Text.Trim();
            var cUsu = txtContraseña.Text.Trim();
            var nCUsu = txtNombre.Text.Trim();
            
            var admon = "N";
            int filasActualizadas = 0;
            if (flexCheckDefault.Checked==true){  admon = "A";   }
            if (nUsu.Length >=4 && cUsu.Length >=4)
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("APPSBANADESA.InsertaU", connection))
                    {
                        try
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.Add("p_nUsu", OracleDbType.Varchar2).Value = Convert.ToString(nUsu.Trim());
                            command.Parameters.Add("p_cUsu", OracleDbType.Varchar2).Value = Convert.ToString(cUsu.Trim());
                            command.Parameters.Add("p_admon", OracleDbType.Varchar2).Value = Convert.ToString(admon.Trim());
                            command.Parameters.Add("p_nCUsu", OracleDbType.Varchar2).Value = Convert.ToString(nCUsu.Trim());
                            command.Parameters.Add("p_filasActualizadas", OracleDbType.Int32).Direction = ParameterDirection.Output;
                            command.ExecuteNonQuery();
                            filasActualizadas += Convert.ToInt32(command.Parameters["p_filasActualizadas"].Value.ToString());
                            //ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "alert('Insercion exitosa!\\n\\nCantidad de registros Insertados: " + filasActualizadas + "');", true);
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Insercion exitosa!', 'Cantidad de registros Insertados: " + filasActualizadas + "', 'success');", true);
                            llenarGVU();
                            txtUsuario.Text = "";
                            txtContraseña.Text = "";
                            txtNombre.Text = "";
                            flexCheckDefault.Checked = false;
                        }
                        catch (Exception ex) { LogErrores("NU-1", ex.Message); }
                        finally { connection.Close(); }
                    }
                }
            }
            else if (nUsu.Length <=0 || cUsu.Length<= 0)
            { Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Advertencia', text: 'Debe de llenar los campos solicitados!' });", true);   }
            else
            { Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Advertencia', text: 'Usuario ò Contraseña no cumplen requisito minimo!' });", true); }
            
        }

        protected void llenarGVU()
        {
            using (OracleConnection con = new OracleConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand("APPSBANADESA.BuscarDatosU", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_resultado", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows) { gvU.DataSource = reader; gvU.DataBind(); }
                            else { gvU.DataSource = null; gvU.DataBind(); }
                        }
                    }
                    gvU.Visible = true;
                }
                catch (Exception ex) { LogErrores("LGU1", ex.Message); }
                finally { con.Close(); }
            }
        }

        protected void gvU_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtiene el índice de la fila seleccionada
            int rowIndex = gvU.SelectedIndex;
            // Verifica si se seleccionó alguna fila
            if (rowIndex >= 0 && rowIndex < gvU.Rows.Count)
            {
                try
                {
                    txtUsuM.Text = HttpUtility.HtmlDecode(gvU.Rows[rowIndex].Cells[1].Text);
                    txtContraM.Text = HttpUtility.HtmlDecode(gvU.Rows[rowIndex].Cells[2].Text);
                    
                    //ESTADO
                    switch (HttpUtility.HtmlDecode(gvU.Rows[rowIndex].Cells[3].Text))
                    {
                        case "INACTIVO":
                            ddEstadoM.SelectedIndex = 0;
                            break;
                        case "ACTIVO":
                            ddEstadoM.SelectedIndex = 1;
                            break;
                        default:
                            ddEstadoM.SelectedIndex = 0;
                            break;
                    }
                    //PERMISOS
                    switch (HttpUtility.HtmlDecode(gvU.Rows[rowIndex].Cells[4].Text))
                    {
                        case "NORMAL":
                            ddAdminPer.SelectedIndex = 0;
                            break;
                        case "ADMINISTRADOR":
                            ddAdminPer.SelectedIndex = 1;
                            break;
                        default:
                            ddAdminPer.SelectedIndex = 0;
                            break;
                    }

                    formularioUM.Visible = true;
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }

            }

        }

        protected void btnUM_Click(object sender, EventArgs e)
        {
            var nUsu = txtUsuM.Text.Trim();
            var cUsu = txtContraM.Text.Trim();
            var estado = "I";
            var admon = "N";
            var restrict = "0";
            int filasActualizadas = 0;
            if (ddEstadoM.SelectedIndex == 0) { estado = "I"; } else { estado = "A"; }
            if (ddAdminPer.SelectedIndex == 0) { admon = "N"; } else { admon = "A"; }
            if(estado=="I") { restrict = "0"; }else{ restrict = "1"; }
            if(nUsu.Length > 0 && cUsu.Length>0) {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand("APPSBANADESA.ActualizaU", connection))
                    {
                        try
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            // Agregar parámetros de entrada
                            command.Parameters.Add("p_UsuM", OracleDbType.Varchar2).Value = Convert.ToString(nUsu.Trim());
                            command.Parameters.Add("p_cUsuM", OracleDbType.Varchar2).Value = Convert.ToString(cUsu.Trim());
                            command.Parameters.Add("p_admon", OracleDbType.Varchar2).Value = Convert.ToString(admon.Trim());
                            command.Parameters.Add("p_esta", OracleDbType.Varchar2).Value = Convert.ToString(estado.Trim());
                            command.Parameters.Add("p_esto", OracleDbType.Varchar2).Value = Convert.ToString(restrict.Trim());
                            command.Parameters.Add("p_filasActualizadas", OracleDbType.Int32).Direction = ParameterDirection.Output;
                            command.ExecuteNonQuery();
                            filasActualizadas += Convert.ToInt32(command.Parameters["p_filasActualizadas"].Value.ToString());
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Actualizaciòn èxitosa!', 'Cantidad de registros Insertados: " + filasActualizadas + "', 'success');", true);
                            llenarGVU();
                        }
                        catch (Exception ex) { LogErrores("UM-1", ex.Message); }
                        finally { connection.Close(); }
                    }
                }
            }
            else
            {  Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Advertencia', text: 'Debe de llenar los campos solicitados!' });", true);
               // ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", "alert('Advertencia: \\n\\nDebe de llenar los campos solicitados');", true);
            }
            
        }

        protected void btnMuestraIni_Click(object sender, EventArgs e)
        {
            ocultaMuestra("6");

        }

        protected void btnDP_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            int anio = dt.Year;
            string mes = dt.Month.ToString("D2");
            string dia = dt.Day.ToString("D2");
            string fechaHoy = Convert.ToString(anio + mes + dia);

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("DPBNDAT.ActualizaHiboAnterior", connection))
                {
                    try
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        //// Agregar parámetros de entrada
                        command.Parameters.Add("p_fecha", OracleDbType.Varchar2).Value = Convert.ToString(fechaHoy);
                        command.Parameters.Add("p_TIBO", OracleDbType.Int32).Value = Convert.ToInt32(HttpUtility.HtmlDecode(txtNumPlanilla.Text.Trim()));
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire('Actualizacion exitosa!', 'Estado de Planillas anteriores modificados!', 'success');", true);
                        command.ExecuteNonQuery();
                        ocultaMuestra("6");
                        txtNumPlanilla.Text = "";
                    }
                    catch (Exception ex) { LogErrores("IG-1", ex.Message); }
                    finally { connection.Close(); }
                }
            }

        }


    }

}