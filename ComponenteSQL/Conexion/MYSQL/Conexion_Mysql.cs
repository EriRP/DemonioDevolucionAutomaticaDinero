using Operativa.ComponenteSQL.Trydicionary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Operativa.ComponenteSQL.Conexion.MYSQL
{
    public class Conexion_Mysql
    {
        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo encargado de traer la informacion de cnn que se encuentra en xbase.sql
        //Fecha de creación: 09/06/2021
        //Fecha Modificación:
        //Cambios Realizados:
        //ESTE METODO SE PUEDE CAMBIAR POR EL APP.CONFIG Y HACER QUE LEA DE AHI LA CONEXION MAS SEGURO
        //public string LeerConexion()
        //{
        //    string url_conexion = "DRIVER={MySQL ODBC 3.51 Driver};SERVER=192.168.55.60;UID=root;PWD=mysqlM94sk04.;Database=billing;OPTIONS='';"; //Variable de la conexion
        //    try
        //    {
        //        //Aqui debe ir lo de base.sql o bien por el app.config o web config
        //    }

        //    catch (Exception ex)
        //    {
        //        url_conexion = "";
        //    }
        //    return url_conexion;
        //}

        Trictionary<string, object, string[]> Arr_Where_ = new Trictionary<string, object, string[]>(); //El trictionary se utiliza para los where
        Dictionary<string, object> Arr_Update = new Dictionary<string, object>();
        String[] Operadores = new String[2];
        static string url_static = "";
        public string LeerConexion()
        {
            //string url_conexion = "DRIVER={MySQL ODBC 3.51 Driver};SERVER=192.168.55.60;UID=root;PWD=mysqlM94sk04.;Database=billing;OPTIONS='';"; //Variable de la conexion
            if (string.IsNullOrEmpty(url_static))
            {
                string url_conexion = File.ReadLines("//192.168.18.3/telecable/xbase_sql.txt").First(); //Variable de la conexion
                //string url_conexion = File.ReadLines("//192.168.18.3/telecable/xbase_sqlprueba.txt").First(); //Variable de la conexion

                url_static = url_conexion;
                //url_static = "DRIVER={MySQL ODBC 8.0 Unicode Driver};SERVER=192.168.18.122;UID=root;PWD=mysqlM94sk04.;Database=billing;OPTIONS='';";
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

        public string MYSQLSTRINGCONNECT(string tipoconexion)
        {
            DataTable Conexion_data = new DataTable();
            string mySqlSelectconex = "";
            try
            {
                string url_conexion = LeerConexion();//TRAEMOS LA CONEXION A MYSQL
                if (!string.IsNullOrEmpty(url_conexion) && !string.IsNullOrEmpty(tipoconexion))
                {
                    //QUERY QUE TRAE LA CONEXION SEGUN EL DATO QUE SE LE MANDE PARA MYSQL Y SQL
                    string select_ = "SELECT TIPO,CONCAT(TRIM(inicadena), ' SERVER=', TRIM(SERVIDOR),';UID=', TRIM(USUARIO) ,';PWD=', TRIM(CLAVE) ,';Database=', TRIM(BASE_DATOS),TRIM(fincadena)) as stringfinal FROM conexiones_sql where tipo = '" + tipoconexion + "'";

                    Conexion_data = select_Mysql(select_, url_conexion);

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
        //Descripción: Metodo encargado de Realizar la conexion a MYSQL y traer los datos solicitados
        //Fecha de creación: 09/06/2021
        //Fecha Modificación:
        //Cambios Realizados:
        public DataTable select_Mysql(string query, string MysqConn)
        {

            DataTable dt_info = new DataTable();
            int intentosConexion = 0;
            //Hace un MAX de 5 intentos para conectar con la BD
            while (intentosConexion < 15)
            {

                using (OdbcConnection dbConnMysql2 = new OdbcConnection(MysqConn))
            {
                try
                {
                    dbConnMysql2.Open();

                    OdbcDataAdapter da = new OdbcDataAdapter(query, dbConnMysql2);

                    /*Nos llena la información de la tabla*/

                    //da.FillSchema(dt_info, SchemaType.Source);

                    /*Fin Nos llena la información de la tabla*/

                    da.Fill(dt_info);
                    dbConnMysql2.Close();
                    dbConnMysql2.Dispose();
                        //break;
                    return dt_info;
                    }
                catch (Exception ex)
                {
                    dbConnMysql2.Close();
                    dbConnMysql2.Dispose();
                    DataTable SetSelect = new DataTable();
                    Thread.Sleep(1000);
                    //bool res = Enviar_Correo_Bitacora(query, ex.ToString().Trim(), "DEMONIO ORDENES UNIFICAR");                    
                    //dt_info.Columns.Add("Exception_mysql_ ");
                    //dt_info.Rows.Add("ERROR " + ex.ToString() + " QUERY " + MysqConn);
                    //return dt_info;
                }
            }
            intentosConexion++;
        }

            return dt_info;

        }

        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo encargado de insertar en MYSQL 
        //Fecha de creación: 14/06/2021
        //Fecha Modificación:
        //Cambios Realizados:

        public bool Executes_Mysql(string query, string MysqConn)
        {

            int intentosConexion = 0;
            bool respuesta = false;
            //Hace un MAX de 5 intentos para conectar con la BD
            while (intentosConexion < 5)
            {
                using (OdbcConnection dbConnMysql2 = new OdbcConnection(MysqConn))
                {
                    OdbcCommand command = new OdbcCommand(query, dbConnMysql2);

                    try
                    {
                        dbConnMysql2.Open();
                        command.ExecuteNonQuery();
                        dbConnMysql2.Close();
                        dbConnMysql2.Dispose();
                        respuesta = true;
                        //return true;
                        //break;//Cierra el ciclo
                        return respuesta;
                    }
                    catch (Exception ex)
                    {
                        dbConnMysql2.Close();
                        dbConnMysql2.Dispose();
                        Thread.Sleep(1000);
                        //Enviar_Correo_Bitacora(query, ex.ToString().Trim(), "DEMONIO ORDENES UNIFICAR");
                        //return false;
                        //return respuesta;
                        //Para que no nos boté el método
                        //throw;

                    }
                }
                intentosConexion++;
            }
            return respuesta;
            
           
        }


        public bool Enviar_Correo_Bitacora(string query, string accion, string conexion)
        {

            try
            {
                MYSQL mysql = new MYSQL();
                string password = string.Empty;
                DataTable parametros = mysql.MYSQLSelect("BILLINGMYSQL", "SELECT * FROM parametros_web WHERE codigo = '24'");
                string correo = string.Empty;
                if (parametros.Rows.Count > 0)
                {
                    correo = parametros.Rows[0]["descripcion"].ToString();
                    password = parametros.Rows[0]["valor"].ToString();
                }
                var basicCredential = new NetworkCredential(correo, password);
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");
                SmtpServer.Credentials = basicCredential;
                mail.From = new MailAddress(correo, "Telecable S.A.");


                MailAddress bcc6 = new MailAddress("joel.alvarez@telecablecr.com");
                MailAddress bcc5 = new MailAddress("fanny.ortega@telecablecr.com");
                MailAddress bcc1 = new MailAddress("axel.reyes@telecablecr.com");
                //MailAddress bcc2 = new MailAddress("sistemas@telecablecr.com");
                //MailAddress bcc3 = new MailAddress("julio.hernandez@telecablecr.com");
                //MailAddress bcc4 = new MailAddress("joel.alvarez@telecablecr.com");          
                //MailAddress bcc7 = new MailAddress("alejandro.vasquez@telecablecr.com");
                //MailAddress bcc8 = new MailAddress("christian.azofeifa@telecablecr.com");
                //MailAddress bcc9 = new MailAddress("orlin.castillo@telecablecr.com");


                mail.Bcc.Add(bcc1);
                //mail.Bcc.Add(bcc2);
                //mail.Bcc.Add(bcc3);
                //mail.Bcc.Add(bcc4);
                mail.Bcc.Add(bcc5);
                mail.Bcc.Add(bcc6);
                //mail.Bcc.Add(bcc7);
                //mail.Bcc.Add(bcc8);
                //mail.Bcc.Add(bcc9);

                string data = string.Empty;

                mail.Subject = "ERROR DEMONIO UNIFICAR ORDENES";

                mail.Body = query + "   ////// exception ////  " + accion + "   ///////////////   " + conexion + " ///////// SE PROCEDE A BAJAR EL PARAMETRO 194, PARA QUE SE VUELVA ACTIVAR PONER EL VALOR EN 1//////";
                mail.IsBodyHtml = true;


                SmtpServer.Port = 587;
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

                Arr_Update.Clear();
                Arr_Where_ = new Trictionary<string, object, string[]>();
                Operadores[0]= "";
                Operadores[1] = "";
                Arr_Update.Add("valor","0");
                Arr_Where_.Add("codigo","194",Operadores);
                mysql.MYSQLUpdate("BILLINGMYSQL", "parametros", Arr_Update, Arr_Where_, "", "");

                return true; // En caso de enviar el correo exitosamente

            }
            catch (Exception ex)
            {
                string error = "Error: " + ex.Message.ToString();
                return false;// En caso de no enviar el correo

            }
        }


    }
}