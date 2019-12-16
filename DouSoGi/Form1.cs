using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DouSoGi
{
    public partial class Form1 : Form
    {
        Tiles[,] tiles = new Tiles[9,7];
        Tiles[,] edittiles = new Tiles[9, 7];
        Button[,] b = new Button[9,7];
        //temporary x dan y untuk index character
        int xtemp = -1;
        int ytemp = -1;
        //bool player untuk menunjukkan player berada di team mana : blue atau red. 
        //jika false maka player ada di team blue, jika true maka player ada di team red
        bool player=true,Isclicked = false,turn;
        //buat AI nya
        List<Tiles> enemy;
        List<Tiles> user;
        Tiles possibility,emove;
        List<Tiles> listMove = new List<Tiles>();

        public Form1()
        {
            InitializeComponent();
            enemy = new List<Tiles>();
            user = new List<Tiles>();
            set_tiles();
            set_button();
        }

        bool check_win(Tiles a, Tiles b) {
            if (get_value(a.Value) > get_value(b.Value))
            {
                return true;
            }
            else {
                return false;
            }
        }

        void button_click(object sender, EventArgs e) { //fungsi button click 
            if (turnstrip.Text != "")
            {
                Button bt = (Button)sender;
                int X = bt.Location.X / 50;
                int Y = bt.Location.Y / 50;

                MessageBox.Show(tiles[Y, X].Value);

                if (tiles[Y, X].Ismoveable == true)
                {
                    if (tiles[Y, X].Isplayer == player) {
                        if (Isclicked == false)
                        {
                            xtemp = X;
                            ytemp = Y;
                            b[ytemp, xtemp].BackColor = Color.White;
                            Isclicked = true;
                        }
                        else
                        {
                            if (tiles[ytemp, xtemp].Isplayer == false)
                            {
                                b[ytemp, xtemp].BackColor = Color.Blue;
                            }
                            else
                            {
                                b[ytemp, xtemp].BackColor = Color.Red;
                            }
                            Isclicked = false;
                            xtemp = -1;
                            ytemp = -1;
                        }
                    }
                }
                else
                {
                    if (tiles[Y, X].Ismoveable == false) {
                        tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                        tiles[ytemp, xtemp].Animal = Image.FromFile("grass.jpg");
                        if (tiles[Y, X].Isplayer == false)
                        {
                            b[Y, X].BackColor = Color.Blue;
                        }
                        else
                        {
                            b[Y, X].BackColor = Color.Red;
                        }
                        tiles[ytemp, xtemp].Value = "grass";
                        tiles[ytemp, xtemp].Ismoveable = false;
                        tiles[ytemp, xtemp].Isplayer = false;
                        b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                    }
                    else{
                        /*
                        if (winmoves(tiles[ytemp, xtemp], tiles[Y, X]))
                        {
                            tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                            tiles[ytemp, xtemp].Animal = Image.FromFile("grass.jpg");
                            tiles[ytemp, xtemp].Value = "grass";
                            tiles[ytemp, xtemp].Ismoveable = false;
                            tiles[ytemp, xtemp].Isplayer = false;
                            b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                        }
                        else {
                            MessageBox.Show("Tidak bisa kesana!");
                        }*/
                    }

                    //turn musuh
                    edittiles = tiles;
                    enemy_turn();
                    enemy.Clear();
                    user.Clear();
                    us_or_foe();

                    refresh_button();
                    Isclicked = false;
                    xtemp = -1;
                    ytemp = -1;
                }
            }
            else {
                MessageBox.Show("Pilih Turn terlebih dahulu!");
            }
        }

        void enemy_turn() {
            int bestMove = -99999;
            for (int i = 0; i < enemy.Count(); i++)
            {
                int bestVal = minimax(3, false, edittiles);
                MessageBox.Show(bestVal+"");
                if (bestVal > bestMove)
                {
                    bestMove = bestVal;
                    possibility = enemy[i];
                }
            }
            b[possibility.Y/50,possibility.X/50].BackColor = Color.Cyan;
        }

        //algoritma minimax
        int minimax(int depth, bool isMax, Tiles[,] maps) {
            if (isMax) {
                if (depth == 0)
                {
                    return evaluate(maps);
                }
                int max = -99999;
                for (int i = 0; i < enemy.Count(); i++)
                {
                    generate_move(enemy[i], maps);
                    for (int j = 0; j< listMove.Count; j++) {
                        maps[listMove[j].Y / 50, listMove[j].X / 50] = enemy[i];
                        //maps[enemy[i].Y / 50, enemy[i].X / 50] = new Tiles(enemy[i].X, enemy[i].Y, "grass", false, false, Image.FromFile("grass.jpg"));
                        max = Math.Max(max, minimax(depth - 1, false, maps));
                    }
                    listMove.Clear();
                }
                return max;
            }
            else {
                if (depth == 0)
                {
                    return -evaluate(maps);
                }
                int min = 99999;
                for (int i = 0; i < user.Count(); i++)
                {
                    generate_move(user[i], maps);
                    for (int j = 0; j < listMove.Count; j++)
                    {
                        maps[listMove[j].Y / 50, listMove[j].X / 50] = user[i];
                        //maps[user[i].Y / 50, user[i].X / 50] = new Tiles(user[i].X, user[i].Y, "grass", false, false, Image.FromFile("grass.jpg"));
                        min = Math.Min(min, minimax(depth - 1, false, maps));
                    }
                    listMove.Clear();
                }
                return min;
            }
            
        }
        //evaluasi papan di akhir depth
        int evaluate(Tiles[,] maps) {
            int value = 0;  
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    value += get_value(maps[i,j].Value);
                }
            }
            return value;
        }
        
        //generate move yang bisa dilakukan oleh AI (Rasanya ada yang salah disini.)
        void generate_move(Tiles a, Tiles[,] editiles)
        {
            int y = a.X / 50;
            int x = a.Y / 50;
            if (x + 1 < 9)
            {
                if (editiles[x + 1, y].Value == "den" && editiles[x + 1, y].Isplayer != player)
                {
                    listMove.Add(editiles[x + 1, y]);
                }
                else if (editiles[x + 1, y].Value == "trap" && editiles[x + 1, y].Isplayer != player)
                {
                    listMove.Add(editiles[x + 1, y]);
                }
                else if (editiles[x, y].Value == "mouse")
                {
                    if (editiles[x + 1, y].Value == "elephant")
                    {
                        listMove.Add(editiles[x + 1, y]);
                    }
                    else if (editiles[x + 1, y].Value == "water")
                    {
                        listMove.Add(editiles[x + 1, y]);
                    }
                    else
                    {
                        listMove.Add(editiles[x + 1, y]);
                    }
                }
                else if ((editiles[x, y].Value == "tiger" || editiles[x, y].Value == "lion") && editiles[x + 1, y].Value == "water")
                {
                    if (x + 3 < 9)
                    {
                        if (get_value(editiles[x, y].Value) > get_value(editiles[x + 3, y].Value))
                        {
                            listMove.Add(editiles[x + 3, y]);
                        }
                    }
                }
                else
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x + 1, y].Value))
                    {
                        listMove.Add(editiles[x + 1, y]);
                    }
                }
            }
            if (x - 1 >= 0)
            {
                if (editiles[x - 1, y].Value == "den" && editiles[x - 1, y].Isplayer != player)
                {
                    listMove.Add(editiles[x - 1, y]);
                }
                else if (editiles[x - 1, y].Value == "trap" && editiles[x - 1, y].Isplayer != player)
                {
                    listMove.Add(editiles[x - 1, y]);
                }
                else if (editiles[x, y].Value == "mouse")
                {
                    if (editiles[x - 1, y].Value == "elephant")
                    {
                        listMove.Add(editiles[x - 1, y]);
                    }
                    else if (editiles[x - 1, y].Value == "water")
                    {
                        listMove.Add(editiles[x - 1, y]);
                    }
                    else
                    {
                        listMove.Add(editiles[x - 1, y]);
                    }
                }
                else if ((editiles[x, y].Value == "tiger" || editiles[x, y].Value == "lion") && editiles[x - 1, y].Value == "water")
                {
                    if (x - 3 >= 0)
                    {
                        if (get_value(editiles[x, y].Value) > get_value(editiles[x - 3, y].Value))
                        {
                            listMove.Add(editiles[x - 3, y]);
                        }
                    }
                }
                else
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x - 1, y].Value))
                    {
                        listMove.Add(editiles[x - 1, y]);
                    }
                }
            }
            if (y + 1 < 7)
            {
                if (editiles[x, y + 1].Value == "den" && editiles[x, y + 1].Isplayer != player)
                {
                    listMove.Add(editiles[x, y + 1]);
                }
                else if (editiles[x, y + 1].Value == "trap" && editiles[x, y + 1].Isplayer != player)
                {
                    listMove.Add(editiles[x, y + 1]);
                }
                else if (editiles[x, y + 1].Value == "mouse")
                {
                    if (editiles[x, y + 1].Value == "elephant")
                    {
                        listMove.Add(editiles[x, y + 1]);
                    }
                    else if (editiles[x, y + 1].Value == "water")
                    {
                        listMove.Add(editiles[x, y + 1]);
                    }
                    else
                    {
                        listMove.Add(editiles[x, y + 1]);
                    }
                }
                else if ((editiles[x, y].Value == "tiger" || editiles[x, y].Value == "lion") && editiles[x, y + 1].Value == "water")
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x, y + 4].Value))
                    {
                        listMove.Add(editiles[x, y + 4]);
                    }
                }
                else
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x, y + 1].Value))
                    {
                        listMove.Add(editiles[x, y + 1]);
                    }
                }
            }
            if (y - 1 >= 0)
            {
                if (editiles[x, y - 1].Value == "den" && editiles[x, y - 1].Isplayer != player)
                {
                    listMove.Add(editiles[x, y - 1]);
                }
                else if (editiles[x, y - 1].Value == "trap" && editiles[x, y - 1].Isplayer != player)
                {
                    listMove.Add(editiles[x, y - 1]);
                }
                else if (editiles[x, y - 1].Value == "mouse")
                {
                    if (editiles[x, y - 1].Value == "elephant")
                    {
                        listMove.Add(editiles[x, y - 1]);
                    }
                    else if (editiles[x, y - 1].Value == "water")
                    {
                        listMove.Add(editiles[x, y - 1]);
                    }
                    else
                    {
                        listMove.Add(editiles[x, y - 1]);
                    }
                }
                else if ((editiles[x, y].Value == "tiger" || editiles[x, y].Value == "lion") && editiles[x, y - 1].Value == "water")
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x, y - 4].Value))
                    {
                        listMove.Add(editiles[x, y - 4]);
                    }
                }
                else
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x, y - 1].Value))
                    {
                        listMove.Add(editiles[x, y - 1]);
                    }
                }
            }
        }

        bool checkWin() {
            
            return false;
        }

        public int get_value(string tile)
        {
            if (tile == "rat") { return 1; }
            else if (tile == "cat") { return 2; }
            else if (tile == "wolf") { return 3; }
            else if (tile == "dog") { return 4; }
            else if (tile == "leo") { return 5; }
            else if (tile == "tiger") { return 6; }
            else if (tile == "lion") { return 7; }
            else if (tile == "elephant") { return 8; }
            else if (tile == "trap") { return 9; }
            else if (tile == "den") { return 10; }
            else { return 0; }
        }


        void refresh_button() { //refresh gambar pada button, anggap seperti Invalidate();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    b[i, j].BackgroundImage = tiles[i, j].Animal;
                }
            }
        }

        /*
            Coding di bawah ini untuk mengatur susunan awal papan permainan. 
            Tidak akan dipanggil lagi pada proses selanjutnya.
        */

        void set_button() { 
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    b[i,j] = new Button();
                    b[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                    b[i, j].Location = new Point(tiles[i, j].X, tiles[i, j].Y);
                    b[i, j].Width = 50;
                    b[i, j].Height = 50;
                    b[i, j].Click += new EventHandler(button_click);
                    if (tiles[i, j].Isplayer == false){
                        b[i, j].BackColor = Color.Blue;
                    }else {
                        b[i, j].BackColor = Color.Red;
                    }
                    b[i, j].BackgroundImage = tiles[i, j].Animal;
                    Controls.Add(b[i, j]);
                }
            }
        }

        void set_tiles() {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if ((i == 3 && j == 1) || (i == 3 && j == 2) || (i == 4 && j == 1) || (i == 4 && j == 2) || (i == 5 && j == 1) || (i == 5 && j == 2) || (i == 3 && j == 4) || (i == 3 && j == 5) || (i == 4 && j == 4) || (i == 4 && j == 5) || (i == 5 && j == 4) || (i == 5 && j == 5))
                    { //water
                        tiles[i, j] = new Tiles(j * 50, i * 50, "water", false, false, Image.FromFile("water.jpg"));
                    }
                    else if (i == 2 && j == 0)
                    { //rat
                        tiles[i, j] = new Tiles(j * 50, i * 50, "rat", true, false, Image.FromFile("rat.png"));
                    }
                    else if (i == 6 && j == 6)
                    {
                        tiles[i, j] = new Tiles(j * 50, i * 50, "rat", true, true, Image.FromFile("rat.png"));
                    }
                    else if ((i == 1 && j == 5))
                    { //cat
                        tiles[i, j] = new Tiles(j * 50, i * 50, "cat", true, false, Image.FromFile("cat.png"));
                    }
                    else if ((i == 7 && j == 1))
                    {
                        tiles[i, j] = new Tiles(j * 50, i * 50, "cat", true, true, Image.FromFile("cat.png"));
                    }
                    else if ((i == 2 && j == 4))
                    { //wolf
                        tiles[i, j] = new Tiles(j * 50, i * 50, "wolf", true, false, Image.FromFile("wolf.png"));
                    }
                    else if ((i == 6 && j == 2))
                    {
                        tiles[i, j] = new Tiles(j * 50, i * 50, "wolf", true, true, Image.FromFile("wolf.png"));
                    }
                    else if ((i == 1 && j == 1))
                    { //dog
                        tiles[i, j] = new Tiles(j * 50, i * 50, "dog", true, false, Image.FromFile("dog.png"));
                    }
                    else if ((i == 7 && j == 5))
                    {
                        tiles[i, j] = new Tiles(j * 50, i * 50, "dog", true, true, Image.FromFile("dog.png"));
                    }
                    else if ((i == 2 && j == 2))
                    { //leopard
                        tiles[i, j] = new Tiles(j * 50, i * 50, "leo", true, false, Image.FromFile("leopard.png"));
                    }
                    else if ((i == 6 && j == 4))
                    {
                        tiles[i, j] = new Tiles(j * 50, i * 50, "leo", true, true, Image.FromFile("leopard.png"));
                    }
                    else if ((i == 0 && j == 6))
                    { //tiger
                        tiles[i, j] = new Tiles(j * 50, i * 50, "tiger", true, false, Image.FromFile("tiger.png"));
                    }
                    else if ((i == 8 && j == 0))
                    {
                        tiles[i, j] = new Tiles(j * 50, i * 50, "tiger", true, true, Image.FromFile("tiger.png"));
                    }
                    else if ((i == 0 && j == 0))
                    { //lion
                        tiles[i, j] = new Tiles(j * 50, i * 50, "lion", true, false, Image.FromFile("lion.png"));
                    }
                    else if ((i == 8 && j == 6))
                    {
                        tiles[i, j] = new Tiles(j * 50, i * 50, "lion", true, true, Image.FromFile("lion.png"));
                    }
                    else if ((i == 2 && j == 6))
                    { //elephant
                        tiles[i, j] = new Tiles(j * 50, i * 50, "elephant", true, false, Image.FromFile("elephant.png"));
                    }
                    else if ((i == 6 && j == 0))
                    {
                        tiles[i, j] = new Tiles(j * 50, i * 50, "elephant", true, true, Image.FromFile("elephant.png"));
                    }
                    else if (i == 0 && j == 3)
                    { //den
                        tiles[i, j] = new Tiles(j * 50, i * 50, "den", false, false, Image.FromFile("den.png"));
                    }
                    else if ((i == 8 && j == 3))
                    {
                        tiles[i, j] = new Tiles(j * 50, i * 50, "den", false, true, Image.FromFile("den.png"));
                    }
                    else if ((i == 0 && j == 2) || (i == 0 && j == 4) || (i == 1 && j == 3))
                    { //traps
                        tiles[i, j] = new Tiles(j * 50, i * 50, "trap", false, false, Image.FromFile("trap.png"));
                    }
                    else if ((i == 8 && j == 2) || (i == 8 && j == 4) || (i == 7 && j == 3))
                    {
                        tiles[i, j] = new Tiles(j * 50, i * 50, "trap", false, true, Image.FromFile("trap.png"));
                    }
                    else
                    { //grass
                        tiles[i, j] = new Tiles(j * 50, i * 50, "grass", false, false, Image.FromFile("grass.jpg"));
                    }
                }
            }
        }

        private void firstTurnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player = false;
            turnstrip.Text = "Player : Blue";
            us_or_foe();
        }

        private void secondTurnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player = true;
            turnstrip.Text = "Player : Red";
            us_or_foe();
            enemy_turn();
        }

        public void us_or_foe() {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (tiles[i, j].Ismoveable == true && tiles[i, j].Isplayer == player)
                    {
                        user.Add(tiles[i, j]);
                    }
                    else if (tiles[i, j].Ismoveable == true && tiles[i, j].Isplayer != player)
                    {
                        enemy.Add(tiles[i, j]);
                    }
                }
            }
        }
    }
}
