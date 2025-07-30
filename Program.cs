using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // Aqui se hace la validacion de argumentos
        if (args.Length != 2)
        {
            Console.WriteLine("Uso: programa <dado_usuario> <dado_pc>");
            Console.WriteLine("Ejemplo: programa A B");
            return;
        }

        string userDice = args[0].ToUpper();
        string computerDice = args[1].ToUpper();

        // Aqui se genera las claves HMAC para mayor transparencia
        byte[] diceKey = GenerateRandomKey();
        string diceHMAC = GenerateHMAC(diceKey, $"{computerDice}");
        
        Console.WriteLine("\n--- Fase de Transparencia ---");
        Console.WriteLine($"HMAC (clave oculta): {diceHMAC}");

        // Generacion colaborativa de las tiradas
        Console.Write("\nIngresa tu número aleatorio (semilla, 1-1000): ");
        int userSeed;
        while (!int.TryParse(Console.ReadLine(), out userSeed) || userSeed < 1 || userSeed > 1000)
        {
            Console.Write("Entrada inválida. Ingresa un número (1-1000): ");
        }

        var combinedRandom = new Random(userSeed + Environment.TickCount);
        int computerRoll = combinedRandom.Next(1, 7);
        int userRoll = combinedRandom.Next(1, 7);

        // Resultados
        byte[] rollKey = GenerateRandomKey();
        string rollHMAC = GenerateHMAC(rollKey, $"{computerRoll}");

        Console.WriteLine("\n--- Resultados ---");
        Console.WriteLine($"Tu tirada: {userRoll}");
        Console.WriteLine($"Computadora tiró: {computerRoll}");
        Console.WriteLine($"HMAC de la tirada: {rollHMAC}");

        // Reveleacion de las claves
        Console.WriteLine("\n--- Verificación ---");
        Console.WriteLine($"Clave HMAC selección dados: {BitConverter.ToString(diceKey).Replace("-", "")}");
        Console.WriteLine($"Clave HMAC tiradas: {BitConverter.ToString(rollKey).Replace("-", "")}");

        // La logica de quien gano
        Console.WriteLine("\n--- Ganador ---");
        if (userRoll > computerRoll) Console.WriteLine("¡Ganaste!");
        else if (userRoll < computerRoll) Console.WriteLine("¡La computadora gana!");
        else Console.WriteLine("¡Empate!");
    }

    static byte[] GenerateRandomKey()
    {
        byte[] key = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        return key;
    }

    static string GenerateHMAC(byte[] key, string data)
    {
        using (var hmac = new HMACSHA256(key))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = hmac.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
