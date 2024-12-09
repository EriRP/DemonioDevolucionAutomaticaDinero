using Operativa.ComponenteSQL.Conexion.MYSQL;
using System;

namespace DemonioOrdenesRecEquipos
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SE INICIAN LOS PROCESOS A LAS: " + DateTime.Now);
            Metodos metodos = new Metodos();

            //bool resultValidacion = metodos.ValidarDevoluciones();
            //if (resultValidacion)
            //{
            //    Console.WriteLine("PROCESO DE VALIDACIÓN FINALIZADO SATISFACTORIAMENTE. LAS DEVOLUCIONES CON 6 MESES DE ANTIGÜEDAD HAN CAMBIADO DE ESTADO.");
            //    Console.ReadLine();
            //}
            //else
            //{
            //    Console.WriteLine("ERROR AL EJECUTAR EL MÉTODO DE VALIDACIÓN DE DEVOLUCIONES. INTÉNTELO DE NUEVO.");
            //    Console.ReadLine();
            //}

            bool resultEnvioCorreos = metodos.EnviarReporteDevolucionesInactivas();
            if (resultEnvioCorreos)
            {
                Console.WriteLine("PROCESO DE ENVÍO DE REPORTE FINALIZADO SATISFACTORIAMENTE. LOS CORREOS LLEGARÁN DENTRO DE POCO.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("ERROR AL EJECUTAR EL MÉTODO DE ENVÍO DE REPORTES. INTÉNTELO DE NUEVO.");
                Console.ReadLine();
            }
        }
        public class Metodos
        {
            MYSQL MYSQL_C = new MYSQL();
            public bool ValidarDevoluciones()
            {
                return MYSQL_C.ValidarDevoluciones("BILLINGMYSQL");
            }
            public bool EnviarReporteDevolucionesInactivas()
            {
                return MYSQL_C.EnviarReporteDevolucionesInactivas("BILLINGMYSQL");
            }
        }
    }
}
