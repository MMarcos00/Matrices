using System;

public class Barco
{
    public string Nombre { get; set; }
    public int Longitud { get; set; }
    public Orientacion Orientacion { get; set; }
    public bool Hundido { get; set; }

    public Barco(string nombre, int longitud, Orientacion orientacion)
    {
        Nombre = nombre;
        Longitud = longitud;
        Orientacion = orientacion;
        Hundido = false;
    }
}

public enum Orientacion
{
    Horizontal,
    Vertical
}

public class Tablero
{
    private Barco[,] casillas;
    private int filas;
    private int columnas;

    public int Filas { get { return filas; } }
    public int Columnas { get { return columnas; } }

    public Tablero(int filas, int columnas)
    {
        this.filas = filas;
        this.columnas = columnas;
        casillas = new Barco[filas, columnas];
    }

    public bool PosicionValida(int fila, int columna, Barco barco)
    {
        if (fila < 0 || fila >= filas || columna < 0 || columna >= columnas)
            return false;

        if (Orientacion.Horizontal == barco.Orientacion)
        {
            for (int i = 0; i < barco.Longitud; i++)
            {
                if (columna + i >= columnas || casillas[fila, columna + i] != null)
                    return false;
            }
        }
        else
        {
            for (int i = 0; i < barco.Longitud; i++)
            {
                if (fila + i >= filas || casillas[fila + i, columna] != null)
                    return false;
            }
        }

        return true;
    }

    public void ColocarBarco(int fila, int columna, Barco barco)
    {
        if (!PosicionValida(fila, columna, barco))
            throw new Exception("Posición inválida para el barco");

        if (Orientacion.Horizontal == barco.Orientacion)
        {
            for (int i = 0; i < barco.Longitud; i++)
            {
                casillas[fila, columna + i] = barco;
            }
        }
        else
        {
            for (int i = 0; i < barco.Longitud; i++)
            {
                casillas[fila + i, columna] = barco;
            }
        }
    }

    public bool RecibirDisparo(int fila, int columna)
    {
        if (fila < 0 || fila >= filas || columna < 0 || columna >= columnas)
            return false;

        Barco barco = casillas[fila, columna];
        if (barco != null)
        {
            barco.Hundido = true;
            return true;
        }

        return false;
    }

    public bool TodosHundidos()
    {
        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                if (casillas[i, j] != null && !casillas[i, j].Hundido)
                    return false;
            }
        }

        return true;
    }

    public void MostrarTablero()
    {
        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                if (casillas[i, j] == null)
                    Console.Write("~ ");
                else if (casillas[i, j].Hundido)
                    Console.Write("X ");
                else
                    Console.Write("O ");
            }
            Console.WriteLine();
        }
    }
}

class JuegoBattleship
{
    private Tablero tableroJugador1;
    private Tablero tableroJugador2;
    private bool turnoJugador1;

    public JuegoBattleship(int filas, int columnas)
    {
        tableroJugador1 = new Tablero(filas, columnas);
        tableroJugador2 = new Tablero(filas, columnas);
        turnoJugador1 = true;
    }

    public void IniciarJuego()
    {
        InicializarBarcos(tableroJugador1);
        InicializarBarcos(tableroJugador2);

        MostrarTableros();

        while (!JuegoTerminado())
        {
            int fila, columna;
            Console.WriteLine("Turno del jugador " + (turnoJugador1 ? "1" : "2") + ":");
            fila = GetFilaColumna("Introduzca la fila (1-{0}): ", tableroJugador1.Filas);
            columna = GetColumna("Introduzca la columna (A-J): ");

            bool impacto = turnoJugador1 ? tableroJugador2.RecibirDisparo(fila - 1, columna - 1)
                : tableroJugador1.RecibirDisparo(fila - 1, columna - 1);

            if (impacto)
            {
                Console.WriteLine("¡Impacto!");
                if (turnoJugador1)
                {
                    if (tableroJugador2.TodosHundidos())
                    {
                        Console.WriteLine("¡El jugador 1 ha ganado!");
                        break;
                    }
                }
                else
                {
                    if (tableroJugador1.TodosHundidos())
                    {
                        Console.WriteLine("¡El jugador 2 ha ganado!");
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("¡Agua!");
            }

            turnoJugador1 = !turnoJugador1;

            MostrarTableros();
        }
    }

    private void InicializarBarcos(Tablero tablero)
    {
        tablero.ColocarBarco(0, 0, new Barco("Portaaviones", 5, Orientacion.Horizontal));
        tablero.ColocarBarco(2, 4, new Barco("Acorazado", 4, Orientacion.Vertical));
        tablero.ColocarBarco(4, 2, new Barco("Crucero", 3, Orientacion.Horizontal));
        tablero.ColocarBarco(6, 6, new Barco("Submarino", 3, Orientacion.Vertical));
        tablero.ColocarBarco(8, 0, new Barco("Destructor", 2, Orientacion.Horizontal));
    }

    private int GetFilaColumna(string mensaje, int max)
    {
        int valor;
        do
        {
            Console.Write(mensaje, max);
            string input = Console.ReadLine();
            if (!int.TryParse(input, out valor) || valor < 1 || valor > max)
            {
                Console.WriteLine("Valor inválido. Debe ser un número entre 1 y {0}.", max);
            }
        } while (valor < 1 || valor > max);

        return valor;
    }

    private int GetColumna(string mensaje)
    {
        int valor;
        do
        {
            Console.Write(mensaje);
            string input = Console.ReadLine().ToUpper();
            valor = input[0] - 'A' + 1;
            if (valor < 1 || valor > tableroJugador1.Columnas)
            {
                Console.WriteLine("Valor inválido. Debe ser una letra entre A y J.");
            }
        } while (valor < 1 || valor > tableroJugador1.Columnas);

        return valor;
    }

    private void MostrarTableros()
    {
        Console.WriteLine("Tablero jugador 1:");
        tableroJugador1.MostrarTablero();
        Console.WriteLine();
        Console.WriteLine("Tablero jugador 2:");
        tableroJugador2.MostrarTablero();
        Console.WriteLine();
    }

    private bool JuegoTerminado()
    {
        return tableroJugador1.TodosHundidos() || tableroJugador2.TodosHundidos();
    }

    static void Main(string[] args)
    {
        int filas = 10;
        int columnas = 10;

        JuegoBattleship juego = new JuegoBattleship(filas, columnas);
        juego.IniciarJuego();
    }
}


