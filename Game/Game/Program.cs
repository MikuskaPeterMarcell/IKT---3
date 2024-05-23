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
    static int maxSwords = 3;
    static readonly string itemadd;
    static List<Tuple<int, int>> enemies = new List<Tuple<int, int>>();
    static int moveCounter = 0;

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
                else if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    hp = baseHP;
                    score = score / 2;
                    LoadMap("falu.txt");
                }
                else if (keyInfo.Key == ConsoleKey.P)
                {
                    DebugAddItems(itemadd);
                }
                MovePlayer(dx, dy);
                moveCounter++;
                if (moveCounter % 2 == 0)
                {
                    MoveEnemies();
                }
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
            enemies.Clear();

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
                    else if (map[i, j] == 'x')
                    {
                        enemies.Add(new Tuple<int, int>(i, j));
                    }
                    else if (map[i, j] == 'X')
                    {
                        enemies.Add(new Tuple<int, int>(i, j));
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
        Console.WriteLine("Health Points: " + hp);
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
                Console.ForegroundColor = ConsoleColor.DarkYellow; // Houses
                break;
            case 'B':
                Console.ForegroundColor = ConsoleColor.Magenta; // Blacksmith
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
            else if (map[newX, newY] == 'B')
            {
                OpenBlacksmithMenu();
                DisplayMapAndStats();
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
                bool defeated = FightSmall();
                if (defeated)
                {
                    map[playerX, playerY] = ' ';
                    playerX = newX;
                    playerY = newY;
                    map[playerX, playerY] = '!';
                    enemies.Remove(new Tuple<int, int>(newX, newY));
                }
                CheckIfDead();
            }
            else if (map[newX, newY] == 'X')
            {
                bool defeated = FightBig();
                if (defeated)
                {
                    map[playerX, playerY] = ' ';
                    playerX = newX;
                    playerY = newY;
                    map[playerX, playerY] = '!';
                    enemies.Remove(new Tuple<int, int>(newX, newY));
                }
                CheckIfDead();
            }
        }
    }

    static bool FightSmall()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("A small enemy attacked you!");
            Console.WriteLine();
            Console.WriteLine("It has 10 health points");
            Console.WriteLine("You have " + CalculateAttackDamage() + " attack damage.");
            Console.ReadKey();
            if (CalculateAttackDamage() > 10)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Clear();
                Console.WriteLine("You defeated the small enemy!");
                score += 5;
                Console.WriteLine();
                Console.WriteLine("Press any button to continue...");
                Console.ReadKey();
                return true;
            }
            else
            {
                if (hp - 1 > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Clear();
                    Console.WriteLine("The small enemy defeated you!");
                    Console.WriteLine("You lost 1 health point");
                    hp--;
                    Console.WriteLine();
                    Console.WriteLine("Press any button to continue...");
                    Console.ReadKey();
                }
                else
                {
                    hp--;
                    CheckIfDead();
                }
                return false;
            }
        }
    }

    static bool FightBig()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("A big enemy attacked you!");
            Console.WriteLine();
            Console.WriteLine("It has 25 health points");
            Console.WriteLine("You have " + CalculateAttackDamage() + " attack damage.");
            Console.ReadKey();
            if (CalculateAttackDamage() > 25)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Clear();
                Console.WriteLine("You defeated the big enemy!");
                score += 15;
                Console.WriteLine();
                Console.WriteLine("Press any button to continue...");
                Console.ReadKey();
                return true;
            }
            else
            {
                if (hp - 3 > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Clear();
                    Console.WriteLine("The big enemy defeated you!");
                    Console.WriteLine("You lost 3 health points");
                    hp -= 3;
                    Console.WriteLine();
                    Console.WriteLine("Press any button to continue...");
                    Console.ReadKey();
                }
                else
                {
                    CheckIfDead();
                    hp -= 3;
                }
                return false;
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
            Console.WriteLine("1. Buy Sword (5 points) (Max 3)");
            Console.WriteLine("2. Buy Shield (3 points)");
            Console.WriteLine("3. Buy Potion (2 points)");
            Console.WriteLine("4. Exit Shop");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            if (choice == "1" && score >= 5 && inventory.GetValueOrDefault("Sword", 0) < maxSwords)
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
                Console.WriteLine("Invalid choice or not enough points or max swords reached.");
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

    static void RemoveFromInventory(string item, int count)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] -= count;
            if (inventory[item] <= 0)
            {
                inventory.Remove(item);
            }
        }
    }

    static void OpenBlacksmithMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the blacksmith!");
            Console.WriteLine("Here you can upgrade your tools:");
            Console.WriteLine();
            Console.WriteLine("1. Upgrade Sword (Requires 3 Swords)");
            Console.WriteLine("2. Upgrade Shield (Requires 3 Shields)");
            Console.WriteLine("3. Exit Blacksmith");
            string choice = Console.ReadLine();

            if (choice == "1" && inventory.ContainsKey("Sword") && inventory["Sword"] >= 3 || inventory.ContainsKey("Golden Sword") && inventory["Golden Sword"] >= 3 || inventory.ContainsKey("Diamond Sword") && inventory["Diamond Sword"] >= 3 || inventory.ContainsKey("Emerald Sword") && inventory["Emerald Sword"] >= 3)
            {
                Console.Clear();
                Console.WriteLine("What tier do you want to upgrade your Sword?");
                Console.WriteLine();
                Console.WriteLine("1. Gold");
                Console.WriteLine("2. Emerald");
                Console.WriteLine("3. Diamond");
                Console.WriteLine("4. Mythic Stone");
                Console.WriteLine("5. Go back");
                string tierchoice = Console.ReadLine();
                if (tierchoice == "1" && inventory.ContainsKey("Sword") && inventory["Sword"] >= 3)
                {
                    RemoveFromInventory("Sword", 3);
                    AddToInventory("Golden Sword");
                    Console.WriteLine("Your swords have been upgraded to a Golden Sword!");
                }
                else if (tierchoice == "2" && inventory.ContainsKey("Golden Sword") && inventory["Golden Sword"] >= 3)
                {
                    RemoveFromInventory("Golden Sword", 3);
                    AddToInventory("Emerald Sword");
                    Console.WriteLine("Your swords have been upgraded to an Emerald Sword!");
                }
                else if (tierchoice == "3" && inventory.ContainsKey("Emerald Sword") && inventory["Emerald Sword"] >= 3)
                {
                    RemoveFromInventory("Emerald Sword", 3);
                    AddToInventory("Diamond Sword");
                    Console.WriteLine("Your swords have been upgraded to a Diamond Sword!");
                }
                else if (tierchoice == "4" && inventory.ContainsKey("Diamond Sword") && inventory["Diamond Sword"] >= 3)
                {
                    RemoveFromInventory("Diamond Sword", 3);
                    AddToInventory("Mythic Stone Sword");
                    Console.WriteLine("Your swords have been upgraded to a Mythic Stone Sword!");
                }
                else if (tierchoice == "5")
                {
                    break;
                }
            }
            else if (choice == "2" && inventory.ContainsKey("Shield") && inventory["Shield"] >= 3 || inventory.ContainsKey("Golden Shield") && inventory["Golden Shield"] >= 3 || inventory.ContainsKey("Diamond Shield") && inventory["Diamond Shield"] >= 3 || inventory.ContainsKey("Emerald Shield") && inventory["Emerald Sword"] >= 3)
            {
                Console.Clear();
                Console.WriteLine("What tier do you want to upgrade your Shield?");
                Console.WriteLine();
                Console.WriteLine("1. Gold");
                Console.WriteLine("2. Emerald");
                Console.WriteLine("3. Diamond");
                Console.WriteLine("4. Mythic Stone");
                Console.WriteLine("5. Go back");
                string tierchoice = Console.ReadLine();
                if (tierchoice == "1" && inventory.ContainsKey("Shield") && inventory["Shield"] >= 3)
                {
                    RemoveFromInventory("Shield", 3);
                    AddToInventory("Golden Shield");
                    Console.WriteLine("Your Shields have been upgraded to a Golden Shield!");
                }
                else if (tierchoice == "2" && inventory.ContainsKey("Golden Shield") && inventory["Golden Shield"] >= 3)
                {
                    RemoveFromInventory("Golden Shield", 3);
                    AddToInventory("Emerald Shield");
                    Console.WriteLine("Your Shields have been upgraded to an Emerald Shield!");
                }
                else if (tierchoice == "3" && inventory.ContainsKey("Emerald Shield") && inventory["Emerald Shield"] >= 3)
                {
                    RemoveFromInventory("Emerald Shield", 3);
                    AddToInventory("Diamond Shield");
                    Console.WriteLine("Your Shields have been upgraded to a Diamond Shield!");
                }
                else if (tierchoice == "4" && inventory.ContainsKey("Diamond Shield") && inventory["Diamond Shield"] >= 3)
                {
                    RemoveFromInventory("Diamond Shield", 3);
                    AddToInventory("Mythic Stone Shield");
                    Console.WriteLine("Your Shields have been upgraded to a Mythic Stone Shield!");
                }
                else if (tierchoice == "5")
                {
                    break;
                }
            }
            else if (choice == "3")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice or not enough items to upgrade.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }


    static void MoveEnemies()
    {
        List<Tuple<int, int>> newEnemies = new List<Tuple<int, int>>();
        HashSet<Tuple<int, int>> occupiedPositions = new HashSet<Tuple<int, int>>();

        foreach (var enemy in enemies)
        {
            int enemyX = enemy.Item1;
            int enemyY = enemy.Item2;
            char enemyType = map[enemyX, enemyY];
            int dx = 0, dy = 0;

            if (enemyX < playerX) dx = 1;
            else if (enemyX > playerX) dx = -1;

            if (enemyY < playerY) dy = 1;
            else if (enemyY > playerY) dy = -1;

            int newX = enemyX + dx;
            int newY = enemyY + dy;

            if (IsInMap(newX, newY) && map[newX, newY] == ' ' && !occupiedPositions.Contains(new Tuple<int, int>(newX, newY)))
            {
                map[enemyX, enemyY] = ' ';
                enemyX = newX;
                enemyY = newY;
            }

            newEnemies.Add(new Tuple<int, int>(enemyX, enemyY));
            occupiedPositions.Add(new Tuple<int, int>(enemyX, enemyY));
            map[enemyX, enemyY] = enemyType;
        }
        enemies = newEnemies;
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


    static void DebugAddItems(string itemadd)
    {
        while (true)
        {
            Console.Clear();
            itemadd = Console.ReadLine();
            AddToInventory(itemadd);
            break;
        }
    }

    static int CalculateAttackDamage()
    {
        int attackDamage = baseAttackDamage;
        if (inventory.ContainsKey("Sword"))
        {
            int swordCount = inventory["Sword"];
            attackDamage = (int)(baseAttackDamage + (5 * swordCount));
        }
        if (inventory.ContainsKey("Golden Sword"))
        {
            int swordCount = inventory["Golden Sword"];
            attackDamage = (int)(baseAttackDamage + (20 * swordCount));
        }
        if (inventory.ContainsKey("Diamond Sword"))
        {
            int swordCount = inventory["Diamond Sword"];
            attackDamage = (int)(baseAttackDamage + (75 * swordCount));
        }
        if (inventory.ContainsKey("Emerald Sword"))
        {
            int swordCount = inventory["Emerald Sword"];
            attackDamage = (int)(baseAttackDamage + (200 * swordCount));
        }
        if (inventory.ContainsKey("Mythic Stone Sword"))
        {
            int swordCount = inventory["Mythic Stone Sword"];
            attackDamage = (int)(baseAttackDamage + (750 * swordCount));
        }
        return attackDamage;
    }

    static int CalculateArmor()
    {
        int armor = baseArmor;
        if (inventory.ContainsKey("Shield"))
        {
            int armorCount = inventory["Shield"];
            armor = (int)(baseArmor + (1 * armorCount));
        }
        if (inventory.ContainsKey("Golden Shield"))
        {
            int armorCount = inventory["Golden Shield"];
            armor = (int)(baseArmor + (5 * armorCount));
        }
        if (inventory.ContainsKey("Diamond Shield"))
        {
            int armorCount = inventory["Diamond Shield"];
            armor = (int)(baseArmor + (20 * armorCount));
        }
        if (inventory.ContainsKey("Emerald Shield"))
        {
            int armorCount = inventory["Emerald Shield"];
            armor = (int)(baseArmor + (75 * armorCount));
        }
        if (inventory.ContainsKey("Mythic Stone Shield"))
        {
            int armorCount = inventory["Mythic Stone Shield"];
            armor = (int)(baseArmor + (250 * armorCount));
        }
        return armor;
    }
}