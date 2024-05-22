using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static char[,] map;
    static int playerX;
    static int playerY;
    static int score = 10;
    static int baseAttackDamage = 10;
    static int baseArmor = 0;
    static int baseHP = 3;
    static int hp = baseHP;
    static Dictionary<string, int> inventory = new Dictionary<string, int>();

    static void Main()
    {
        LoadMap("falu.txt");
        DisplayMapAndStats();
        while (true)
        {
            
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                int dx = 0, dy = 0;

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    dx = -1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    dx = 1;
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    dy = -1;
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    dy = 1;
                }
                else if(keyInfo.Key == ConsoleKey.Spacebar)
                {
                    hp = baseHP;
                    score = score / 2;
                    LoadMap("falu.txt");
                }
                MovePlayer(dx, dy);
                DisplayMapAndStats();
                CheckIfDead();
            }
        }
    }

    static void LoadMap(string filename)
    {
        try
        {
            string[] lines = File.ReadAllLines(filename);
            int height = lines.Length;
            int width = lines[0].Length;

            map = new char[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    map[i, j] = lines[i][j];
                    if (map[i, j] == '!')
                    {
                        playerX = i;
                        playerY = j;
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Map file not found: " + filename);
        }
    }

    static void DisplayMapAndStats()
    {
        Console.Clear();
        int height = map.GetLength(0);
        int width = map.GetLength(1);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                SetConsoleColor(map[i, j]);
                Console.Write(map[i, j]);
            }
            Console.WriteLine();
        }

        Console.ResetColor();
        Console.WriteLine("Healt Points: " + hp);
        Console.WriteLine("Score: " + score);
        Console.WriteLine("Inventory: " + GetInventoryString());
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Stats:");
        Console.WriteLine("     Attack Damage : " + CalculateAttackDamage());
        Console.WriteLine("     Armor : " + CalculateArmor());
    }

    static void SetConsoleColor(char c)
    {
        switch (c)
        {
            case '#':
                Console.ForegroundColor = ConsoleColor.DarkGray; // Walls
                break;
            case '!':
                Console.ForegroundColor = ConsoleColor.Blue; // Player
                break;
            case 'o':
                Console.ForegroundColor = ConsoleColor.Yellow; // Ore
                break;
            case 'x':
            case 'X':
                Console.ForegroundColor = ConsoleColor.Red; // Enemies
                break;
            case 'H':
                Console.ForegroundColor = ConsoleColor.DarkYellow; //Houses
                break;
            default:
                Console.ResetColor();
                break;
        }
    }

    static string GetInventoryString()
    {
        if (inventory.Count == 0)
        {
            return "Empty";
        }

        List<string> items = new List<string>();
        foreach (var item in inventory)
        {
            items.Add($"{item.Key} x{item.Value}");
        }

        return string.Join(", ", items);
    }

    static void MovePlayer(int dx, int dy)
    {
        int newX = playerX + dx;
        int newY = playerY + dy;

        if (IsInMap(newX, newY))
        {
            if (map[newX, newY] == ' ')
            {
                map[playerX, playerY] = ' ';
                playerX = newX;
                playerY = newY;
                map[playerX, playerY] = '!';

                ObjectCheck();
            }
            else if (map[newX, newY] == ']')
            {
                playerX = newX;
                playerY = newY;
                ObjectCheck();
            }
            else if (map[newX, newY] == '[')
            {
                playerX = newX;
                playerY = newY;
                ObjectCheck();
            }
            else if (map[newX, newY] == 'S')
            {
                OpenShopMenu();
                DisplayMapAndStats();
            }
            else if (map[newX, newY] == 'U')
            {
                playerX = newX;
                playerY = newY;
                ObjectCheck();
            }
            else if (map[newX, newY] == 'o')
            {
                score++;
                map[playerX, playerY] = ' ';
                playerX = newX;
                playerY = newY;
                ObjectCheck();
                map[playerX, playerY] = '!';
                
            }
            else if (map[newX, newY] == 'x')
            { 
                map[playerX, playerY] = ' ';
                playerX = newX;
                playerY = newY;
                map[playerX, playerY] = '!';
                FightSmall();
            }
            else if (map[newX,newY] == 'X')
            {
                map[playerX, playerY] = ' ';
                playerX = newX;
                playerY = newY;
                map[playerX, playerY] = '!';
                FightBig();
            }
        }
    }

    static void ObjectCheck()
    {
        if (map[playerX, playerY] == ']')
        {
            LoadMap("banya3.txt");
            DisplayMapAndStats();
        }
        else if (map[playerX, playerY] == '[')
        {
            LoadMap("faluvissza.txt");
            DisplayMapAndStats();
        }
        else if (map[playerX, playerY] == 'S')
        {
            LoadMap("bolt.txt");
            DisplayMapAndStats();
        }
        else if (map[playerX, playerY] == 'U')
        {
            LoadMap("falubolt.txt");
            DisplayMapAndStats();
        }
    }

    static bool IsInMap(int x, int y)
    {
        return x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1);
    }

    static void OpenShopMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the shop!");
            Console.WriteLine("Score: " + score);
            Console.WriteLine("1. Buy Sword (5 points)");
            Console.WriteLine("2. Buy Shield (3 points)");
            Console.WriteLine("3. Buy Potion (2 points)");
            Console.WriteLine("4. Exit Shop");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            if (choice == "1" && score >= 5)
            {
                score -= 5;
                AddToInventory("Sword");
                Console.WriteLine("You bought a Sword!");
            }
            else if (choice == "2" && score >= 3)
            {
                score -= 3;
                AddToInventory("Shield");
                Console.WriteLine("You bought a Shield!");
            }
            else if (choice == "3" && score >= 2)
            {
                score -= 2;
                AddToInventory("Potion");
                Console.WriteLine("You bought a Potion!");
            }
            else if (choice == "4")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice or not enough points.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    static void AddToInventory(string item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item]++;
        }
        else
        {
            inventory[item] = 1;
        }
    }



    static int CalculateAttackDamage()
    {
        int attackDamage = baseAttackDamage;
        if (inventory.ContainsKey("Sword"))
        {
            int swordCount = inventory["Sword"];
            attackDamage = (int)(baseAttackDamage * Math.Pow(1.5, swordCount));
        }
        return attackDamage;
    }

    static int CalculateArmor()
    {
        int armor = baseArmor;
        if (inventory.ContainsKey("Shield"))
        {
            armor += 10 * inventory["Shield"];
        }
        return armor;
    }

    static void CheckIfDead()
    {
        if (hp < 1)
        {
            inventory.Clear();
            LoadMap("meghalt.txt");
            DisplayMapAndStats();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("██╗   ██╗ ██████╗ ██╗   ██╗    ██████╗ ██╗███████╗██████╗ \r\n╚██╗ ██╔╝██╔═══██╗██║   ██║    ██╔══██╗██║██╔════╝██╔══██╗\r\n ╚████╔╝ ██║   ██║██║   ██║    ██║  ██║██║█████╗  ██║  ██║\r\n  ╚██╔╝  ██║   ██║██║   ██║    ██║  ██║██║██╔══╝  ██║  ██║\r\n   ██║   ╚██████╔╝╚██████╔╝    ██████╔╝██║███████╗██████╔╝\r\n   ╚═╝    ╚═════╝  ╚═════╝     ╚═════╝ ╚═╝╚══════╝╚═════╝ ");
            Console.WriteLine();
            Console.WriteLine("Press 'SPACE' to respawn!");
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfoSpace = Console.ReadKey(true);
                if (keyInfoSpace.Key == ConsoleKey.Spacebar)
                {
                    hp = baseHP;
                    LoadMap("falu.txt");
                    DisplayMapAndStats();
                }
            }
        }
    }
    static void FightSmall()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("A small enemy attacked you!");
            Console.WriteLine("It has 10 health point");
            Console.WriteLine(Environment.NewLine + "You have " + CalculateAttackDamage() + " attack damage.");
            if (CalculateAttackDamage() > 10)
            {
                Console.ForegroundColor= ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine("You defeated the small enemy!");
                score += 5;
                Console.WriteLine("Press any button to continue...");
                Console.ReadKey();
                break;
            }
            else
            {
                if (hp - 1 > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("The small enemy defeated you!");
                    Console.WriteLine("You lost 1 health point");
                    hp--;
                    Console.WriteLine("Press any button to continue...");
                    Console.ReadKey();
                }
                else
                {
                    hp --;
                    CheckIfDead();
                }
                break;
            }
        }
    }
    static void FightBig()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("A big enemy attacked you!");
            Console.WriteLine("It has 25 health point");
            Console.WriteLine(Environment.NewLine + "You have " + CalculateAttackDamage() + " attack damage.");
            if (CalculateAttackDamage() > 25)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine("You defeated the big enemy!");
                score += 15;
                Console.WriteLine("Press any button to continue...");
                Console.ReadKey();
                break;
            }
            else
            {
                if (hp - 3 > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine();
                    Console.WriteLine("The big enemy defeated you!");
                    Console.WriteLine("You lost 3 health point");
                    hp = hp - 3;
                    Console.WriteLine("Press any button to continue...");
                    Console.ReadKey();
                    break;
                }
                else
                {
                    CheckIfDead();
                    hp = hp - 3;
                }
                break;
            }
        }
    }

}