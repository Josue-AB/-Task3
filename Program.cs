using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.Write("Elige un dado (A/B/C): ");
        string userDice = Console.ReadLine().ToUpper();

        // Validación de entrada del dado
        while(userDice != "A" && userDice != "B" && userDice != "C")
        {
            Console.Write("Entrada inválida. Elige un dado (A/B/C): ");
            userDice = Console.ReadLine().ToUpper();
        }

        string[] diceOptions = { "A", "B", "C" };
        string computerDice = diceOptions[new Random().Next(0, 3)];

        byte[] diceKey = GenerateRandomKey();
        string diceHMAC = GenerateHMAC(diceKey, $"{userDice}{computerDice}");

        Console.WriteLine("\n--- Selección de dados ---");
        Console.WriteLine($"HMAC (clave oculta): {diceHMAC}");
        Console.WriteLine($"Computadora eligió: {computerDice}");

        Console.Write("\nTu tirada (1-6): ");
        string userRoll = Console.ReadLine();
        int userRollValue;
        
        // Validación de la tirada
        while(!int.TryParse(userRoll, out userRollValue) || userRollValue < 1 || userRollValue > 6)
        {
            Console.Write("Entrada inválida. Tu tirada (1-6): ");
            userRoll = Console.ReadLine();
        }

        string computerRoll = new Random().Next(1, 7).ToString();

        byte[] rollKey = GenerateRandomKey();
        string rollHMAC = GenerateHMAC(rollKey, $"{userRoll}{computerRoll}");

        Console.WriteLine("\n--- Resultados ---");
        Console.WriteLine($"HMAC (clave oculta): {rollHMAC}");
        Console.WriteLine($"Computadora tiró: {computerRoll}");
        Console.WriteLine($"Tu tirada: {userRoll}");

        Console.WriteLine("\n--- Claves para verificación ---");
        Console.WriteLine($"Clave selección dados: {BitConverter.ToString(diceKey).Replace("-", "")}");
        Console.WriteLine($"Clave tirada: {BitConverter.ToString(rollKey).Replace("-", "")}");
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
