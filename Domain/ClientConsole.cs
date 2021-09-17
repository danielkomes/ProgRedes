using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    class ClientConsole
    {
        public void GetConsole(ClientThread ct, string id)
        {
            string[] response = Response(id);
            ct.currentConsoleLocation = response[0];
            ct.locationRequest = "";
            ct.optionsResponse = response[1];
        }

        private string[] Response(string id) //del cliente
        {
            string options = "";
            string currentLocation = "";
            if (id.Equals("1.")) //publicar juego
            {
                options += "1 Titulo/r/n" +
                    "2 Genero/r/n" +
                    "3 Calificacion de publico/r/n" +
                    "4 Descripcion/r/n" +
                    "5 Caratula";
            }
            if (id.Equals("1.1."))
            {
                //TODO
            }
            string[] ret = { currentLocation, options };
            return ret;
        }
    }
}

/*
menu principal
1. Publicar juego
    1.1. Titulo
    1.2. Genero
    1.3. Calificacion de publico
    1.4. Descripcion
    1.5. Caratula
3. Buscar juegos
    3.1. Categoria
    3.2. Titulo
    3.3. Rating

Juego:
2. Modificar o borrar juego
    2.1. Titulo
    2.2. Descripcion
    2.3. Calificacion de publico
4. Calificar juego
    4.1. Comentario
    4.2. Rating
5. Detalle
    5.1. Titulo
    5.2. Genero
    5.3. Calificacion de publico
    5.4. Descripcion
    5.5. Caratula
    5.6. Ver reviews

 */

//Opcion 1: una consola en el servidor, que atiende clientes de a uno. La consola tendrá que ser mutex.
//Pros: no afectaría la memoria (RAM) del lado del servidor
//Contras: Si hubiera muchos clientes cada uno tendría que esperar a los anteriores a que terminen de usar la consola.

//Opcion 2: una consola en cada thread de clientes.
//Pros: Los clientes no tendrían que esperar su turno en la consola. 
//Contras: se podría acabar la memoria con muchos clientes.

//Opcion 3: cada cliente se "descarga" una consola que habla con el servidor (significaría poner la consola en el lado del cliente, y que use la memoria de su máquina)
//Pros: sin espera de turno y el servidor no se sobrecarga con n consolas
//Contras: el cliente debe descargar la consola, tener la RAM necesaria. Pierde la gracia el obligatorio, en el que suponemos que hay que demostrar lo que entendimos de comunicación de redes 

//Si suponemos que los clientes pasarán más tiempo leyendo el texto que devolvió la consola que el tiempo que gasta el servidor en procesar los requests de páginas, entonces la mejor opción es la 1. 
//No vale la pena llenar la RAM del servidor con n hilos con consolas si los clientes hacen pocos requests por unidad de tiempo (en 1 unidad de tiempo, si menos del 50% se usa en requests (o responses?), conviene opcion 1 (calcular??))


//El while(true) de client hay que cambiarlo por algo que salga si no encuentra conexión al servidor. Eso ocurre cuando el servidor se desconecta.