using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace SUAMVC.Models
{
    public class SUAHelper
    {
        private OleDbConnection connection = null;

        public SUAHelper() {
            conectar();
        }
        public SUAHelper(String path) {
            conectar(path);
        }

        /**
         * Verificamos si es valido actualizar los campos calculados
         * por registro patronal y numero de afiliacion
         * @param: registroPatronal
         * @param: numeroAfiliacion
         * @return: Boolean
         * 
         */
        public Boolean esValidoActualizarPorMovimiento(String registroPatronal, String numeroAfiliacion){
            Boolean esValido = true;
            String sSQL = "SELECT FEC_INIC, TIP_MOVS FROM Movtos " +
                          " WHERE REG_PATR = '" + registroPatronal.Trim() + "'" +
                          "   AND NUM_AFIL = '" + numeroAfiliacion.Trim() + "'" +
                          " ORDER BY FEC_INIC ";

            DataTable dt = this.ejecutarSQL(sSQL);
            
            foreach (DataRow rows in dt.Rows) {

                String tipoMovimiento = rows["TIP_MOVS"].ToString();
                if (tipoMovimiento.Trim().Equals("06") || tipoMovimiento.Trim().Equals("12")) {
                    esValido = false;
                }//El ultimo movimiento realizado es baja o suspención ?
                break;
            }

            return esValido;
        }
        /**
         * Establecemos la conexión a nuestra base de datos SUA.mdb 
         * @param path: dirección fisica donde se encuentra nuestra BD.
         * @return OleDbConnection: la conexión establecida
         */
        public OleDbConnection conectar(String path) {

            this.cerrarConexion();

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=" + path.Trim() + "\\sua.mdb;Jet OLEDB:Database Password=S5@N52V49;";
            this.connection = new OleDbConnection(connectionString);
            this.connection.Open();

            return this.connection;
        }

        /**
         * Establecemos la conexión a nuestra base de datos SUA.mdb 
         * 
         */
        public OleDbConnection conectar()
        {

            this.cerrarConexion();

            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                                          "Data Source=C:\\SUA\\sua.mdb;Jet OLEDB:Database Password=S5@N52V49;";
            connection = new OleDbConnection(connectionString);
            connection.Open();

            return connection;
        }

        /**
         * Ejecutamos un SQL a partir de un string
         * @return DataTable
         * 
         */
        public DataTable ejecutarSQL(String sSQL) {

            DataTable dt = new DataTable();
            if (this.connection.State.Equals(ConnectionState.Open))
            {
                DataSet ds = new DataSet();

                OleDbDataAdapter da = new OleDbDataAdapter(sSQL, this.connection);
                da.Fill(ds);
                dt = ds.Tables[0];
            }

            return dt;
        }

        /**
         * Cerramos la conexión 
         * 
         */
        public void cerrarConexion() {
            try
            {
                if (this.connection != null)
                {
                    if (this.connection.State.Equals(ConnectionState.Open))
                    {
                        this.connection.Close();
                    }
                }
                
            }catch(OleDbException ex){
                Console.WriteLine("error en al cerrar la conexión SUA:" + ex.Message);
            }
        }
    }
}