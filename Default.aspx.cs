using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;


namespace SAhibo
{
    public partial class Default : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["cnS"].ConnectionString;
        private static bool eventoEjecutado = false;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            // Crear un hilo adicional para manejar el tiempo
            Thread tiempoThread = new Thread(new ThreadStart(ManejarTiempo));
            tiempoThread.Start();

            var esUsuarioAutenticado = 1;
            using (OracleConnection conexion = new OracleConnection(connectionString))
            {
                try
                {
                    conexion.Open();
                    // Crea un comando Oracle
                    using (OracleCommand comando = new OracleCommand("APPSBANADESA.AutenticarUsuario", conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.Parameters.Add("p_NombreUsuario", OracleDbType.Varchar2).Value = txtUsuario.Text.Trim();
                        comando.Parameters.Add("p_Contraseña", OracleDbType.Varchar2).Value = txtContraseña.Text.Trim();
                        comando.Parameters.Add("p_EsUsuarioAutenticado", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        comando.ExecuteNonQuery();
                        if (comando.Parameters["p_EsUsuarioAutenticado"].Value == null)
                        {   Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Error', text: 'Error en respuesta!' });", true);    }
                        else { esUsuarioAutenticado = Convert.ToInt32(comando.Parameters["p_EsUsuarioAutenticado"].Value.ToString()); }
                    }
                }
                catch (Exception ex)
                {     Console.WriteLine("Error: " + ex.Message);    }
                if (esUsuarioAutenticado > 0)
                {  IsValidUser();     Response.Redirect("PaginaPrincipal.aspx");   }
                else{ lblError.Visible = true; lblError.Text = "Nombre de usuario o contraseña incorrectos.";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "Swal.fire({ icon: 'error', title: 'Error', text: 'Nombre de usuario o contraseña incorrectos!' });", true);
                }
            }

            // Marcar que el evento se ha ejecutado
            eventoEjecutado = true;
            // Detener el hilo de tiempo
            tiempoThread.Abort();

        }
        
        private void IsValidUser()
        {
            Session["NombUsu"] = txtUsuario.Text;
            Session.Timeout = 30;
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
            
        }



    }

}