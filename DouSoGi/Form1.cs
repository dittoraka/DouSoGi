﻿using System;
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
        Tiles[,] editiles = new Tiles[9, 7];
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
        int maxValue = -0, minValue = 9999;

        public Form1()
        {
            InitializeComponent();
            enemy = new List<Tiles>();
            user = new List<Tiles>();
            set_tiles();
            set_button();
        }
        void button_click(object sender, EventArgs e) { //fungsi button click 
            if (turnstrip.Text != "")
            {
                Button bt = (Button)sender;
                int X = bt.Location.X / 50;
                int Y = bt.Location.Y / 50;

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
                    editiles = tiles;
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
            int bestMove = -9999;
            for (int i = 0; i < enemy.Count(); i++)
            {
                int bestVal = minimax(3, false);

                if (bestVal >= bestMove) {
                    bestMove = bestVal;
                    possibility = enemy[i];
                }
            }
            tiles[emove.Y / 50, emove.X / 50] = tiles[possibility.Y/50,possibility.X/50];
        }

        
        int minimax(int depth, bool isMax) {
            if (depth == 0) {
                return -evaluate();
            }
            if (isMax) {
                int bestMove = -9999;
                for (int i = 0; i < enemy.Count(); i++)
                {
                    generate_move(enemy[i]);
                    editiles[emove.Y/50, emove.X/50] = emove;
                    bestMove = Math.Max(bestMove, minimax(depth - 1, false));
                }
                return bestMove;
            }
            else {
                int bestMove = 9999;

                for (int i = 0; i < user.Count(); i++)
                {
                    generate_move(user[i]);
                    editiles[emove.Y/50, emove.X/50] = emove;
                    bestMove = Math.Min(bestMove, minimax(depth - 1, true));
                }
                return bestMove;
            }
        }

        public int evaluate() {
            int value = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    value += get_value(editiles[i,j].Value);
                }
            }
            return value;
        }

        void generate_move(Tiles a)
        {
            int y = a.X/50;
            int x = a.Y/50;
            if (x + 1 < 9)
            {
                if (get_value(tiles[x, y].Value) > get_value(tiles[x + 1, y].Value))
                {
                    emove = tiles[x + 1, y];
                }
            }
            if (x - 1 >= 0) { 
                if (get_value(tiles[x, y].Value) > get_value(tiles[x - 1, y].Value))
                {
                    emove = tiles[x - 1, y];
                }
            }
            if (y + 1 < 7)
            {
                if (get_value(tiles[x, y].Value) > get_value(tiles[x, y + 1].Value))
                {
                    emove = tiles[x, y + 1];
                }
            }
            if (y - 1 >= 0)
            {
                if (get_value(tiles[x, y].Value) > get_value(tiles[x, y - 1].Value))
                {
                    emove = tiles[x, y - 1];
                }
            }
        }

        public int get_value(string tile)
        {
            if (tile == "rat"){return 1;}
            else if (tile == "cat"){return 2;}
            else if (tile == "wolf"){return 3;}
            else if (tile == "dog"){return 4;}
            else if (tile == "leo"){return 5;}
            else if (tile == "tiger"){return 6;}
            else if (tile == "lion"){return 7;}
            else if (tile == "elephant"){return 8;}
            else{return 0;}
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
