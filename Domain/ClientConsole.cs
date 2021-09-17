using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    class ClientConsole
    {
        void Response(string id) //del cliente
        {
            string ret = "";
            if (id.Equals("1.")) //publicar juego
            {
                ret += "1 Titulo/r/n" +
                    "2 Genero/r/n" +
                    "3 Calificacion de publico/r/n" +
                    "4 Descripcion/r/n" +
                    "5 Caratula";
            }
            if (id.Equals("1.1."))
            {
                //TODO
            }
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
