using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Game_Lib
{
    public class Save
    {
        public string Inventory { get; init; }
        public int Score { get; init; }
        public int Hp { get; init; }
        public Save(string inv, int score, int hp) 
        {
            Inventory = inv;
            Score = score;
            Hp = hp;
        }
        public void SaveGame()
        {
            StreamWriter fileOut = new("save.txt", false);
            fileOut.WriteLine(Inventory);
            fileOut.WriteLine(Score);
            fileOut.WriteLine(Hp);
            fileOut.Close();
        }
    }
}
