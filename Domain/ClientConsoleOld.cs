using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Domain
{
    public class ClientConsoleOld
    {
        private const string error = "\r\nIncorrect input\r\n";
        private const string publishGame = "PUBLISH_GAME";
        private const string separator = "________________________________________________________";
        public void Response(ClientThread ct)
        {
            string id = ct.LocationRequest; //lo que el cliente ingresó
            string request = ct.CurrentConsoleLocation + "." + id; //string completo para analizar. Ej "0.1.1"
            ct.OptionsResponse = GetConsole(ct, request); //respuesta de GetConsole con las opciones a mostrar o un mensaje de error
            if (!ct.OptionsResponse.Equals(error)) //si no hubo error
            {
                //ct.CurrentConsoleLocation = request; //actualizar la ubicacion del cliente en la consola
            }
            else
            {
                ct.OptionsResponse = GetConsole(ct, ct.CurrentConsoleLocation) + ct.OptionsResponse; //si hubo error, mostrar el mensaje y las mismas opciones que tenía antes de ingresar el string
            }
            ct.OptionsResponse = "\r\n" + separator + "\r\n" + ct.OptionsResponse + "\r\n";
            ct.LocationRequest = ""; //borrar el request para que no se envíe a la consola de nuevo

            //Thread.Sleep(5000); //test mutex. Aparentemente funciona bien con varios clientes. No queda clara la necesidad de lock


        }

        private string GetConsole(ClientThread ct, string request)
        {
            string options = "";
            if (request.Equals("0.0")) //menu
            {
                options += "1 Publish game\r\n" +
                    "2 Find game\r\n";
                ct.CurrentConsoleLocation = "0";
            }
            else if (request.Equals("0.1")) //publicar juego
            {
                if (ct.GameToPublish == null)
                {
                    ct.GameToPublish = new Game();
                }
                options += "1 Title: " + ct.GameToPublish.Title + "\r\n" +
                    "2 Genre: " + ct.GameToPublish.Genre + "\r\n" +
                    "3 Age rating: " + ct.GameToPublish.AgeRating + "\r\n" +
                    "4 Description: " + ct.GameToPublish.Description + "\r\n" +
                    "5 Poster: " + ct.GameToPublish.Poster + "\r\n" +
                    "\r\n" +
                    "6 Accept\r\n" +
                    "7 Back\r\n" +
                    "8-------------- TEST FILL\r\n";
                ct.CurrentConsoleLocation = "0.1";
            }
            else if (request.Equals("0.1.1"))
            {
                options += "Input title: \r\n";
                ct.CurrentConsoleLocation = "0.1.1";
            }
            #region Genre
            else if (request.Equals("0.1.2"))
            {
                options += "Ingresar genero: \r\n" +
                    "1 " + EGenre.Action + "\r\n" +
                    "2 " + EGenre.Adventure + "\r\n" +
                    "3 " + EGenre.Horror + "\r\n" +
                    "4 " + EGenre.Survival + "\r\n" +
                    "5 " + EGenre.RPG + "\r\n";
                ct.CurrentConsoleLocation = "0.1.2";
            }
            else if (request.Equals("0.1.2.1"))
            {
                ct.GameToPublish.Genre = EGenre.Action;
                options += GoTo(ct, "0.1");
            }
            else if (request.Equals("0.1.2.2"))
            {
                ct.GameToPublish.Genre = EGenre.Adventure;
                options += GoTo(ct, "0.1");
            }
            else if (request.Equals("0.1.2.3"))
            {
                ct.GameToPublish.Genre = EGenre.Horror;
                options += GoTo(ct, "0.1");
            }
            else if (request.Equals("0.1.2.4"))
            {
                ct.GameToPublish.Genre = EGenre.Survival;
                options += GoTo(ct, "0.1");
            }
            else if (request.Equals("0.1.2.5"))
            {
                ct.GameToPublish.Genre = EGenre.RPG;
                options += GoTo(ct, "0.1");
            }
            #endregion
            else if (request.Equals("0.1.3"))
            {
                options += "Input age rating: \r\n";
                ct.CurrentConsoleLocation = "0.1.3";
            }
            else if (request.Equals("0.1.4"))
            {
                options += "Input description: \r\n";
                ct.CurrentConsoleLocation = "0.1.4";
            }
            else if (request.Equals("0.1.5"))
            {
                options += "Input poster: \r\n";//???
                ct.CurrentConsoleLocation = "0.1.5";
            }
            else if (request.Equals("0.1.6"))//aceptar
            {
                if (ct.GameToPublish.IsFieldsFilled())
                {
                    Sys.AddGame(ct.GameToPublish);
                    ct.GameToPublish = null;
                    options += "\r\nGAME PUBLISHED\r\n";
                }
                else
                {
                    options += "\r\nSome fields are missing info\r\n";
                }
                options = GoTo(ct, "0.1") + options;
            }
            else if (request.Equals("0.1.7"))//atras
            {
                ct.GameToPublish = null;
                options += GoTo(ct, "0.0");
                options += "\r\nPublishing aborted\r\n";
            }
            else if (request.Equals("0.1.8"))//test fill
            {
                ct.GameToPublish.Title = "TEST TITLE";
                ct.GameToPublish.Genre = EGenre.Action;
                ct.GameToPublish.AgeRating = -1;
                ct.GameToPublish.Description = "TEST DESCRIPTION";
                ct.GameToPublish.Poster = "TEST CARATULA";
                options += GoTo(ct, "0.1");
            }

            else if (request.Equals("0.2")) //buscar juego
            {
                options += "1 Back\r\n";
                options += "--------\r\n";
                options += "Games: \r\n";
                options += "\r\n" + Logic.ListGames();
                ct.CurrentConsoleLocation = "0.2";
            }
            else if (request.Equals("0.2.1")) //atras
            {
                options += GoTo(ct, "0.0");
            }
            else if (request.StartsWith("0.2."))
            {
                try
                {
                    int gameIndex = int.Parse(ct.LocationRequest);
                    ct.GameToView = Logic.GetGameByIndex(gameIndex);
                    options += (ct, "0.2." + ct.GameToView.Title);
                }
                catch (FormatException)
                {
                    options += GoTo(ct, "0.2");
                }
            }
            else if (options.Equals("")) //significa que se escribió algo que no era una de las opciones. Si estaba en un campo de respuesta abierta, tomar la respuesta. Si no, dar un error
            {
                if (ct.GameToView != null) //debe ir antes que las respuestas de opción libre porque el título del juego podría ser igual que una de las opciones numéricas
                {
                    if (request.Equals("0.2." + ct.GameToView.Title))
                    {
                        options += "Viewing game: " + ct.GameToView.Title + "\r\n---------\r\n" +
                            "1 Edit game\r\n" +
                            "2 Review game\r\n" +
                            "3 Details\r\n";
                    }
                    else if (request.Equals("0.2." + ct.GameToView.Title + ".1")) //edit game
                    {

                    }
                }
                else
                {
                    if (ct.CurrentConsoleLocation.Equals("0.1.1")) //si estaba en Publicar juego> elegir título
                    {
                        ct.GameToPublish.Title = ct.LocationRequest;
                        options += GoTo(ct, "0.1");
                    }
                    else if (ct.CurrentConsoleLocation.Equals("0.1.3")) //calificacion de edad
                    {
                        try
                        {
                            ct.GameToPublish.AgeRating = int.Parse(ct.LocationRequest);
                            options += GoTo(ct, "0.1");
                        }
                        catch (FormatException)
                        {
                            options += "Input must be a number\r\n";
                            options += GoTo(ct, "0.1.3");
                        }
                    }
                    else if (ct.CurrentConsoleLocation.Equals("0.1.4")) //descripcion
                    {
                        ct.GameToPublish.Description = ct.LocationRequest;
                        options += GoTo(ct, "0.1");
                    }
                    else if (ct.CurrentConsoleLocation.Equals("0.1.5")) //caratula
                    {
                        ct.GameToPublish.Poster = ct.LocationRequest;
                        options += GoTo(ct, "0.1");
                    }
                    else //error
                    {
                        options += error;
                    }
                }
            }
            else
            {
            }
            return options;
        }

        private string GoTo(ClientThread ct, string location)
        {
            ct.CurrentConsoleLocation = location;
            return GetConsole(ct, location);
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
    1.6. Atras
3. Buscar juegos
    3.1. Categoria
    3.2. Titulo
    3.3. Rating
    3.4. Atras

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


    
//El while(true) de client y server hay que cambiarlo por algo que salga si no encuentra conexión. Eso ocurre cuando alguien se desconecta.

    Flujo: ClientProgram -> ServerProgram > ClientConsole < ServerProgram -> ClientProgram
        ClientProgram escribe en consola, se envia a ServerProgram, el servidor llama a ClientConsole, pasándole los datos de posición actual y último input,
        ClientConsole revisa la posicion actual de ESE cliente en la consola, lee el input y decide qué opción corresponde mostrar. 
        Ese string se devuelve al servidor, que lo envía inmediatamente al cliente

    BUGS: Sys no está guardando los juegos de prueba creados en Logic, sólo guarda los creados por el cliente. Problema instance--static class? DOMAIN.LOGIC NO LANZA SU MAIN

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

