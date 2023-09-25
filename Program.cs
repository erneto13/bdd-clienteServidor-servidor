using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace PRAC1_ClienteServidor_Sockets
{
    class Programa
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n APLICACION SERVIDOR v0.0.1\n");
            while (true)
            {
                try
                {
                    IPAddress ipAd = IPAddress.Any;
                    // Utilizamos la dirección local, de igual en el cliente
                    // Inicializamos el Listener
                    TcpListener myList = new TcpListener(ipAd, 8001);
                    myList.Start();
                    Console.ResetColor(); // Restablece los colores a los valores predeterminados
                    Console.WriteLine(" Servidor iniciado, puerto 8001 en uso");
                    Console.WriteLine(" Local End Point: " + myList.LocalEndpoint);
                    Console.WriteLine(" Se espera la conexión del cliente");
                    Socket s = myList.AcceptSocket();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n Conexión recibida desde " + s.RemoteEndPoint);
                    // Guardamos en una variable la información recibida del cliente
                    byte[] b = new byte[100];
                    int k = s.Receive(b);
                    Console.WriteLine(" Se esta recibiendo la instrucción");
                    // Convertimos e imprimimos la información
                    string cadena = "";
                    for (int i = 0; i < k; i++)
                    {
                        cadena = cadena + Convert.ToChar(b[i]);
                    }
                    Console.WriteLine(cadena);
                    // Conectamos al SQL Server
                    string connectSQL = "Server=EQUIPODEERNESTO;database=BDD_Practicas;" + " Integrated Security=SSPI;";
                    SqlConnection cm = new SqlConnection();
                    cm.ConnectionString = connectSQL;
                    cm.Open();
                    // Ejecutamos la cadena recibida, como comando de SQL
                    SqlCommand cmd = new SqlCommand(cadena, cm);
                    cmd.ExecuteNonQuery();
                    cm.Close();
                    // Enviar respuesta exitosa al cliente
                    ASCIIEncoding asen = new ASCIIEncoding();
                    Console.ResetColor(); // Restablece los colores a los valores predeterminados
                    s.Send(asen.GetBytes(" Instrucción recibida. Cadena ejecutada con éxito."));
                    Console.WriteLine("\n Confirmación enviada.");
                    s.Close();
                    myList.Stop();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Se perdió la conexión con el cliente...");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Se encontró el error: " + e.StackTrace);
                }
                Console.ResetColor(); // Restablece los colores a los valores predeterminados
            }
        }
    }
}