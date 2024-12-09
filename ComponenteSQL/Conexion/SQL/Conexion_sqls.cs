using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
namespace Operativa.ComponenteSQL.Conexion.SQL
{
    public class Conexion_sqls
    {
        MYSQL.Conexion_Mysql MYSQL_D = new MYSQL.Conexion_Mysql();
        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo encargado de traer la informacion de cnn que se encuentra en xbase.sql
        //Fecha de creación: 09/06/2021
        //Fecha Modificación:
        //Cambios Realizados:
        //ESTE METODO SE PUEDE CAMBIAR POR EL APP.CONFIG Y HACER QUE LEA DE AHI LA CONEXION MAS SEGURO

        static string url_static = "";
        public string LeerConexion()
        {
            //string url_conexion = "DRIVER={MySQL ODBC 3.51 Driver};SERVER=192.168.55.60;UID=root;PWD=mysqlM94sk04.;Database=billing;OPTIONS='';"; //Variable de la conexion
            if (string.IsNullOrEmpty(url_static))
            {
                string url_conexion = File.ReadLines("//192.168.18.3/telecable/xbase_sql.txt").First();//Variable de la conexion
                url_static = url_conexion;
            }
            try
            {
                //Aqui debe ir lo de base.sql o bien por el app.config o web config
                return url_static;
            }



            catch (Exception ex)
            {
                url_static = "";
            }
            return url_static;
        }


        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo encargado de Realizar la conexion a MYSQL
        //Fecha de creación: 09/06/2021
        //Fecha Modificación:
        //Cambios Realizados:

        public string SQLSSTRINGCONNECT(string tipoconexion)
        {
            DataTable Conexion_data = new DataTable();
            string mySqlSelectconex = "";
            try
            {
                string url_conexion = LeerConexion();//TRAEMOS LA CONEXION A MYSQL
                if (!string.IsNullOrEmpty(url_conexion) && !string.IsNullOrEmpty(tipoconexion))
                {
                    //QUERY QUE TRAE LA CONEXION SEGUN EL DATO QUE SE LE MANDE PARA MYSQL Y SQL
                    string select_ = "SELECT TIPO,CONCAT('Data Source=',TRIM(SERVIDOR),';Initial Catalog=', TRIM(BASE_DATOS),';User ID=',TRIM(USUARIO),';Password=',TRIM(CLAVE)) as stringfinal FROM conexiones_sql where tipo = '" + tipoconexion + "'";//String de conexion para SQL SERVER
                    //string select_ = "SELECT TIPO,CONCAT(TRIM(inicadena), ' SERVER=', TRIM(SERVIDOR),';UID=', TRIM(USUARIO) ,';PWD=', TRIM(CLAVE) ,';Database=', TRIM(BASE_DATOS),TRIM(fincadena)) as stringfinal FROM conexiones_sql where tipo = '" + tipoconexion + "'";

                    Conexion_data = MYSQL_D.select_Mysql(select_, url_conexion);

                    //Si hay datos entonces vamos a tomar el string final de la conexion
                    if (Conexion_data != null)
                    {

                        if (Conexion_data.Rows.Count > 0)
                        {
                            if (Conexion_data.Columns.Count > 0)
                            {
                                //Este for nos indica si la columna que recibio es la una excepcion(Error)
                                for (int i = 0; i < Conexion_data.Columns.Count; i++) //Recorro las columnas, 
                                {
                                    if (Conexion_data.Columns[i].ColumnName.Contains("Exception_mysql_")) //Si la columna tiene este encabezado entonces
                                    {
                                        if (Conexion_data.Rows.Count > 0)
                                        {
                                            return Conexion_data.Rows[0]["Exception_mysql_"].ToString().Trim(); //Retornamos el error
                                        }
                                        else
                                        {
                                            return "error al obtener el string de conexion";
                                        }

                                    }
                                }
                            }

                            mySqlSelectconex = Conexion_data.Rows[0]["stringfinal"].ToString().Trim();//obtenemos el string de la conexion 

                            if (!string.IsNullOrEmpty(mySqlSelectconex))
                            {
                                return mySqlSelectconex;
                            }
                            else
                            {
                                return "error";
                            }

                        }
                        else
                        {
                            return "error";
                        }

                    }
                    else
                    {
                        return "error";
                    }

                }
                else
                {
                    return "error";//Retorna error
                }

            }
            catch (Exception e)
            {
                return "error" + e.ToString();//Retorna error

            }

        }

        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo encargado de Realizar la conexion a SQL y traer los datos solicitados
        //Fecha de creación: 09/06/2021
        //Fecha Modificación:
        //Cambios Realizados:
        public DataTable select_sqls(string query, string sqsConn)
        {

            DataTable dt_info = new DataTable();
            int intentosConexion = 0;
            //Hace un MAX de 5 intentos para conectar con la BD
            //while (intentosConexion < 5)
            //{

            using (SqlConnection sqlConn = new SqlConnection(sqsConn))
            {
                try
                {
                    sqlConn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(query, sqlConn);

                    /*Nos llena la información de la tabla*/

                    //da.FillSchema(dt_info, SchemaType.Source);

                    /*Fin Nos llena la información de la tabla*/

                    da.Fill(dt_info);
                    sqlConn.Close();
                    sqlConn.Dispose();
                    //break;

                }
                catch (Exception ex)
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                    DataTable SetSelect = new DataTable();
                    Thread.Sleep(1000);
                    //dt_info.Columns.Add("Exception_mysql_ ");
                    //dt_info.Rows.Add("ERROR " + ex.ToString() + " QUERY " + MysqConn);
                    //return dt_info;


                }
            }

            //intentosConexion++;
            //}

            return dt_info;

        }

        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo encargado de insertar en SQL
        //Fecha de creación: 14/06/2021
        //Fecha Modificación:
        //Cambios Realizados:

        public bool Executes_sqls(string query, string sqsConn)
        {
            //int intentosConexion = 0;
            bool respuesta = false;
            //Hace un MAX de 5 intentos para conectar con la BD
            /*while (intentosConexion < 5)
            {*/
            using (SqlConnection sqlConn = new SqlConnection(sqsConn))
            {
                SqlCommand command = new SqlCommand(query, sqlConn);

                try
                {
                    sqlConn.Open();
                    command.ExecuteNonQuery();
                    sqlConn.Close();
                    sqlConn.Dispose();
                    respuesta = true;
                    //return true;
                    //break;//Cierra el ciclo

                }
                catch (Exception ex)
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                    Thread.Sleep(1000);
                    //return respuesta;
                    //Para que no nos boté el método
                    //throw;

                }
            }

            //intentosConexion++;
            //}
            return respuesta;
        }


    }
}