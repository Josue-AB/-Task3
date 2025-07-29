using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static readonly int[][] dice = {
        new int[] { 3, 3, 3, 3, 3, 6 },  
        new int[] { 2, 2, 2, 5, 5, 5 },  
        new int[] { 1, 4, 4, 4, 4, 4 }   
    };

    static string GenerateHMAC(string key, string value)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    static int RollDice(int[] die, string playerName, string secretKey)
    {
        Random rand = new Random();
        int roll = die[rand.Next(die.Length)];
        string hmac = GenerateHMAC(secretKey, roll.ToString());
        
        Console.WriteLine($"{playerName} tira: {roll} (HMAC: {hmac})");
        return roll;
    }

    static void Main(string[] args)
    {
        Console.WriteLine("=== JUEGO DE DADOS NO TRANSITIVOS ===");
        
        const string secretKeyUser = "clave_usuario";
        const string secretKeyPC = "clave_pc";

        Console.WriteLine("\nElige un dado (A, B, C):");
        char userChoice = Console.ReadLine().ToUpper()[0];
        int userDieIndex = userChoice - 'A';

        Random rand = new Random();
        int pcDieIndex = rand.Next(3);
        Console.WriteLine($"\nPC elige: {(char)('A' + pcDieIndex)}");

        int userRoll = RollDice(dice[userDieIndex], "Usuario", secretKeyUser);
        int pcRoll = RollDice(dice[pcDieIndex], "PC", secretKeyPC);

        Console.WriteLine($"\nResultado: Usuario {userRoll} vs PC {pcRoll}");
        Console.WriteLine(userRoll > pcRoll ? "¡Ganas!" : 
                         userRoll < pcRoll ? "¡PC gana!" : "Empate.");
    }
}