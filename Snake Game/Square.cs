using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Game
{
    //En klass för alla fyrkanter spelet använder; äpplen, huvudet och svansen
    class Square
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Square()
        {
            X = 0;
            Y = 0;
        }
    }
}
