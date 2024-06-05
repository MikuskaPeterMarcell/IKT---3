using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Lib
{
    public class Load
    {
        public Load() { }

        public Save LoadGame()
        {
            Console.WriteLine("\tSoot Adventure");
            if (File.Exists("save.txt")) 
            {
                string[] saveLines = File.ReadAllLines("save.txt");
                Save save = new(saveLines[0], int.Parse(saveLines[1]), int.Parse(saveLines[2]));

                Console.WriteLine($"\n\t Mentés sikeresen betöltve!");
                return save;
            }
            else
            {
                Console.WriteLine("\n\tMég nincs mentés ezen a gépen");
                Save save = NewSave();
                return save;
            }
        }
        private Save NewSave()
        {
            return new Save ( "Empty x1", 10, 3 );
        }
    }
}
