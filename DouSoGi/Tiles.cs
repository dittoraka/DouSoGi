using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DouSoGi
{
    class Tiles
    {
        private int x, y;
        private string value;
        private bool ismoveable;
        private bool isplayer;
        private Image animal;
        public bool diair;

        public Tiles(int x, int y, string value, bool ismoveable, bool isplayer, Image animal)
        {
            this.x = x;
            this.y = y;
            this.value = value;
            this.ismoveable = ismoveable;
            this.isplayer = isplayer;
            this.animal = animal;
            this.diair = false;
        }
        

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public string Value { get => value; set => this.value = value; }
        public bool Ismoveable { get => ismoveable; set => ismoveable = value; }
        public bool Isplayer { get => isplayer; set => isplayer = value; }
        public Image Animal { get => animal; set => animal = value; }
    }
}
