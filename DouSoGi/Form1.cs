using System;
using System.Collections;
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
        private Random rnd = new Random();
        //temporary x dan y untuk index character
        int xtemp = -1;
        int ytemp = -1;
        //bool player untuk menunjukkan player berada di team mana : blue atau red. 
        //jika false maka player ada di team blue, jika true maka player ada di team red
        bool player=true,Isclicked = false,turn;

        //bool mauJalan untuk tau apakah  player lagi klik binatang atau tidak
        bool mauJalan = false;
        //buat AI nya
        List<Tiles> enemy;
        List<Tiles> user;
        Tiles possibility,emove;
        List<Tiles> listMove = new List<Tiles>();
        List<Tiles> listE = new List<Tiles>();

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
                        mauJalan = true;
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
                    else
                    {
                        //musuh
                        if (get_value(tiles[Y, X].Value) <= get_value(tiles[ytemp, xtemp].Value))
                        {
                            //makan
                            tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                            tiles[ytemp, xtemp].Animal = Image.FromFile("grass.jpg");
                            tiles[ytemp, xtemp].Value = "grass";
                            tiles[ytemp, xtemp].Ismoveable = false;
                            tiles[ytemp, xtemp].Isplayer = false;
                            b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                            if (tiles[Y, X].Isplayer == false)
                            {
                                b[Y, X].BackColor = Color.Blue;
                            }
                            else
                            {
                                b[Y, X].BackColor = Color.Red;
                            }
                        }
                        else if (tiles[Y, X].Value == "elephant" && tiles[ytemp, xtemp].Value == "rat")
                        {
                            if (tiles[ytemp, xtemp].diair)
                            {
                                //tidak bisa makan kalo tikus di air
                                MessageBox.Show("tidak bisa makan kalau dari air");
                                if (tiles[ytemp, xtemp].Isplayer == false)
                                {
                                    b[ytemp, xtemp].BackColor = Color.Blue;
                                }
                                else
                                {
                                    b[ytemp, xtemp].BackColor = Color.Red;
                                }
                            }
                            else
                            {
                                //tikus makan gajah
                                tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                                tiles[ytemp, xtemp].Animal = Image.FromFile("grass.jpg");
                                tiles[ytemp, xtemp].Value = "grass";
                                tiles[ytemp, xtemp].Ismoveable = false;
                                tiles[ytemp, xtemp].Isplayer = false;
                                b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                                if (tiles[Y, X].Isplayer == false)
                                {
                                    b[Y, X].BackColor = Color.Blue;
                                }
                                else
                                {
                                    b[Y, X].BackColor = Color.Red;
                                }
                            }
                        }
                        refresh_button();
                    }
                }
                else
                {
                    if (tiles[Y, X].Ismoveable == false) {
                        if (mauJalan)
                        {
                            if (tiles[Y, X].Value == "water")
                            {
                                if (tiles[ytemp, xtemp].Value == "rat")
                                {
                                    if (tiles[ytemp, xtemp].diair == true)
                                    {
                                        tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                                        tiles[ytemp, xtemp].Animal = Image.FromFile("water.jpg");
                                        tiles[ytemp, xtemp].Value = "water";
                                        tiles[ytemp, xtemp].Ismoveable = false;
                                        tiles[ytemp, xtemp].Isplayer = false;
                                        b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                                        if (tiles[Y, X].Isplayer == false)
                                        {
                                            b[Y, X].BackColor = Color.Blue;
                                        }
                                        else
                                        {
                                            b[Y, X].BackColor = Color.Red;
                                        }
                                        mauJalan = false;
                                    }
                                    else
                                    {
                                        tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                                        tiles[ytemp, xtemp].Animal = Image.FromFile("grass.jpg");
                                        tiles[ytemp, xtemp].Value = "grass";
                                        tiles[ytemp, xtemp].Ismoveable = false;
                                        tiles[ytemp, xtemp].Isplayer = false;
                                        b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                                        if (tiles[Y, X].Isplayer == false)
                                        {
                                            b[Y, X].BackColor = Color.Blue;
                                        }
                                        else
                                        {
                                            b[Y, X].BackColor = Color.Red;
                                        }
                                        mauJalan = false;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Binatang ini tidak boleh masuk ke air");
                                    if (tiles[ytemp, xtemp].Isplayer == false)
                                    {
                                        b[ytemp, xtemp].BackColor = Color.Blue;
                                    }
                                    else
                                    {
                                        b[ytemp, xtemp].BackColor = Color.Red;
                                    }
                                }
                                tiles[Y, X].diair = true;
                            }
                            else
                            {
                                if (tiles[ytemp, xtemp].diair == true)
                                {
                                    tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                                    tiles[ytemp, xtemp].Animal = Image.FromFile("water.jpg");
                                    tiles[ytemp, xtemp].Value = "water";
                                    tiles[ytemp, xtemp].Ismoveable = false;
                                    tiles[ytemp, xtemp].Isplayer = false;
                                    b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                                    if (tiles[Y, X].Isplayer == false)
                                    {
                                        b[Y, X].BackColor = Color.Blue;
                                    }
                                    else
                                    {
                                        b[Y, X].BackColor = Color.Red;
                                    }
                                    mauJalan = false;
                                }
                                else
                                {
                                    tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                                    tiles[ytemp, xtemp].Animal = Image.FromFile("grass.jpg");
                                    tiles[ytemp, xtemp].Value = "grass";
                                    tiles[ytemp, xtemp].Ismoveable = false;
                                    tiles[ytemp, xtemp].Isplayer = false;
                                    b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                                    if (tiles[Y, X].Isplayer == false)
                                    {
                                        b[Y, X].BackColor = Color.Blue;
                                    }
                                    else
                                    {
                                        b[Y, X].BackColor = Color.Red;
                                    }
                                    mauJalan = false;
                                }
                                tiles[Y, X].diair = false;
                            }
                        }
                    }
                    else{
                        if (tiles[Y, X].Value == "den" && tiles[Y, X].Isplayer != player) {
                            MessageBox.Show("Player wins!");
                        }
                        else if (tiles[Y, X].Value == "trap" && tiles[Y, X].Isplayer != player) {
                            tiles[Y, X] = new Tiles(X * 50, Y * 50, "grass", false, false, Image.FromFile("grass.jpg"));
                            tiles[ytemp, xtemp].Animal = Image.FromFile("grass.jpg");
                            tiles[ytemp, xtemp].Value = "grass";
                            tiles[ytemp, xtemp].Ismoveable = false;
                            tiles[ytemp, xtemp].Isplayer = false;
                            b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                            if (tiles[Y, X].Isplayer == false)
                            {
                                b[Y, X].BackColor = Color.Blue;
                            }
                            else
                            {
                                b[Y, X].BackColor = Color.Red;
                            }
                        }
                        else if (tiles[ytemp, xtemp].Value == "rat" && tiles[Y, X].Value == "water")
                        { //water check
                            if (tiles[ytemp - 1, xtemp].Value != "water")
                            {
                                tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                                tiles[ytemp, xtemp].Animal = Image.FromFile("grass.jpg");
                                tiles[ytemp, xtemp].Value = "grass";
                                tiles[ytemp, xtemp].Ismoveable = false;
                                tiles[ytemp, xtemp].Isplayer = false;
                                b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                                if (tiles[Y, X].Isplayer == false)
                                {
                                    b[Y, X].BackColor = Color.Blue;
                                }
                                else
                                {
                                    b[Y, X].BackColor = Color.Red;
                                }
                            }
                            else {
                                tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                                tiles[ytemp, xtemp].Animal = Image.FromFile("water.jpg");
                                tiles[ytemp, xtemp].Value = "water";
                                tiles[ytemp, xtemp].Ismoveable = false;
                                tiles[ytemp, xtemp].Isplayer = false;
                                b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                                if (tiles[Y, X].Isplayer == false)
                                {
                                    b[Y, X].BackColor = Color.Blue;
                                }
                                else
                                {
                                    b[Y, X].BackColor = Color.Red;
                                }
                            }
                        }
                        else if (tiles[ytemp, xtemp].Value == "rat" && tiles[Y, X].Value == "elephant") {
                            tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                            tiles[ytemp, xtemp].Animal = Image.FromFile("grass.jpg");
                            tiles[ytemp, xtemp].Value = "grass";
                            tiles[ytemp, xtemp].Ismoveable = false;
                            tiles[ytemp, xtemp].Isplayer = false;
                            b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                            if (tiles[Y, X].Isplayer == false)
                            {
                                b[Y, X].BackColor = Color.Blue;
                            }
                            else
                            {
                                b[Y, X].BackColor = Color.Red;
                            }
                        }/*
                        else if ((tiles[ytemp, xtemp].Value == "tiger" || tiles[ytemp, xtemp].Value == "lion") && tiles[Y, X].Value == "water")
                        {
                            
                        }*/
                        else if (get_value(tiles[ytemp, xtemp].Value) > get_value(tiles[Y, X].Value)) {
                            tiles[Y, X] = new Tiles(X * 50, Y * 50, tiles[ytemp, xtemp].Value, true, player, tiles[ytemp, xtemp].Animal);
                            tiles[ytemp, xtemp].Animal = Image.FromFile("grass.jpg");
                            tiles[ytemp, xtemp].Value = "grass";
                            tiles[ytemp, xtemp].Ismoveable = false;
                            tiles[ytemp, xtemp].Isplayer = false;
                            b[Y, X].BackgroundImage = tiles[Y, X].Animal;
                            if (tiles[Y, X].Isplayer == false)
                            {
                                b[Y, X].BackColor = Color.Blue;
                            }
                            else
                            {
                                b[Y, X].BackColor = Color.Red;
                            }
                        }
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

                    set_edit();
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

        void set_edit()
        {

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    edittiles[i, j] = tiles[i, j];
                }
            }
        }

        void enemy_turn() {
            int bestMove = -9999;
            for (int i = 0; i < enemy.Count(); i++)
            {
                generate_move(enemy[i], tiles);
            }
            int bestVal = minimax(3, true, edittiles);
            MessageBox.Show("Done : "+possibility.Value);
            tiles[emove.Y / 50, emove.X / 50] = possibility;
            tiles[possibility.Y / 50, possibility.X / 50] = new Tiles(possibility.X, possibility.Y, "grass", false, false, Image.FromFile("grass.jpg"));
            listMove.Clear();
            listE.Clear();
        }

        

        //algoritma minimax
        int minimax(int depth, bool isMax, Tiles[,] maps) {
            int bestMoveValue;
            if (depth == 0)
            {
                //listMove.Clear();
                return evaluate(maps);
            }

            //if (isMax)
            //{
            //    bestMoveValue = -9999;
            //}
            //else
            //{
            //    bestMoveValue = 9999;
            //}

            //for (int i = 0; i < enemy.Count(); i++)
            //{
            //    generate_move(enemy[i], maps);
            //}
            //for (int i = 0; i < listMove.Count(); i++)
            //{
            //    maps[listMove[i].Y / 50, listMove[i].X / 50] = listMove[i];
            //    int bestval = minimax(depth - 1, !isMax, maps);
            //    if (isMax)
            //    {
            //        if (bestval > bestMoveValue)
            //        {
            //            bestMoveValue = bestval;
            //            possibility = listE[i];
            //            emove = listMove[i];
            //        }
            //    }
            //    else
            //    {
            //        if (bestval < bestMoveValue)
            //        {
            //            bestMoveValue = bestval;
            //            possibility = listE[i];
            //            emove = listMove[i];
            //        }
            //    }
            //}
            //return bestMoveValue;
            
            if (isMax)
            {
                int max = -9999;
                for (int i = 0; i < enemy.Count(); i++)
                {
                    for (int j = 0; j < listMove.Count; j++)
                    {
                        maps[listMove[j].Y / 50, listMove[j].X / 50] = enemy[i];
                        //maps[enemy[i].Y / 50, enemy[i].X / 50] = new Tiles(enemy[i].X, enemy[i].Y, "grass", false, false, Image.FromFile("grass.jpg"));
                        int bestVal =  minimax(depth - 1, !isMax, maps);
                        if (bestVal > max)
                        {
                            max = bestVal;
                            emove = listMove[j];
                            possibility = listE[i];
                        }
                        maps[listMove[j].Y / 50, listMove[j].X / 50] = tiles[listMove[j].Y / 50, listMove[j].X / 50];
                        maps[enemy[i].Y / 50, enemy[i].X / 50] = tiles[enemy[i].Y / 50, enemy[i].X / 50];
                    }
                }
                return max;
            }
            else
            {
                int min = 9999;
                for (int i = 0; i < user.Count(); i++)
                {
                    for (int j = 0; j < listMove.Count; j++)
                    {
                        maps[listMove[j].Y / 50, listMove[j].X / 50] = user[i];
                        //maps[user[i].Y / 50, user[i].X / 50] = new Tiles(user[i].X, user[i].Y, "grass", false, false, Image.FromFile("grass.jpg"));
                        int bestVal = minimax(depth - 1, !isMax, maps);
                        if (bestVal < min)
                        {
                            min = bestVal;
                            emove = listMove[j];
                            possibility = listE[i];
                        }
                        maps[listMove[j].Y / 50, listMove[j].X / 50] = tiles[listMove[j].Y / 50, listMove[j].X / 50];
                        maps[enemy[i].Y / 50, enemy[i].X / 50] = tiles[enemy[i].Y / 50, enemy[i].X / 50];
                    }
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
                    listE.Add(editiles[x,y]);
                }
                else if (editiles[x + 1, y].Value == "trap" && editiles[x + 1, y].Isplayer != player)
                {
                    listMove.Add(editiles[x + 1, y]); listE.Add(editiles[x, y]);
                }
                else if (editiles[x, y].Value == "mouse")
                {
                    if (editiles[x + 1, y].Value == "elephant" && editiles[x + 1, y].Isplayer != player)
                    {
                        listMove.Add(editiles[x + 1, y]); listE.Add(editiles[x, y]);
                    }
                    else if (editiles[x + 1, y].Value == "water")
                    {
                        listMove.Add(editiles[x + 1, y]); listE.Add(editiles[x, y]);
                    }
                    else
                    {
                        listMove.Add(editiles[x + 1, y]); listE.Add(editiles[x, y]);
                    }
                }
                else if ((editiles[x, y].Value == "tiger" || editiles[x, y].Value == "lion") && editiles[x + 1, y].Value == "water")
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x + 3, y].Value) && editiles[x + 3, y].Isplayer != player)
                    {
                        listMove.Add(editiles[x + 3, y]); listE.Add(editiles[x, y]);
                    }
                }
                else
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x + 1, y].Value) && editiles[x + 1, y].Isplayer != player)
                    {
                        listMove.Add(editiles[x + 1, y]); listE.Add(editiles[x, y]);
                    }
                }
            }
            if (x - 1 >= 0)
            {
                if (editiles[x - 1, y].Value == "den" && editiles[x - 1, y].Isplayer != player)
                {
                    listMove.Add(editiles[x - 1, y]); listE.Add(editiles[x, y]);
                }
                else if (editiles[x - 1, y].Value == "trap" && editiles[x - 1, y].Isplayer != player)
                {
                    listMove.Add(editiles[x - 1, y]); listE.Add(editiles[x, y]);
                }
                else if (editiles[x, y].Value == "mouse")
                {
                    if (editiles[x - 1, y].Value == "elephant" && editiles[x - 1, y].Isplayer != player)
                    {
                        listMove.Add(editiles[x - 1, y]); listE.Add(editiles[x, y]);
                    }
                    else if (editiles[x - 1, y].Value == "water")
                    {
                        listMove.Add(editiles[x - 1, y]); listE.Add(editiles[x, y]);
                    }
                    else
                    {
                        listMove.Add(editiles[x - 1, y]); listE.Add(editiles[x, y]);
                    }
                }
                else if ((editiles[x, y].Value == "tiger" || editiles[x, y].Value == "lion") && editiles[x - 1, y].Value == "water")
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x - 3, y].Value) && editiles[x - 3, y].Isplayer != player)
                    {
                        listMove.Add(editiles[x - 3, y]); listE.Add(editiles[x, y]);
                    }
                }
                else
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x - 1, y].Value) && editiles[x - 1, y].Isplayer != player)
                    {
                        listMove.Add(editiles[x - 1, y]); listE.Add(editiles[x, y]);
                    }
                }
            }
            if (y + 1 < 7)
            {
                if (editiles[x, y + 1].Value == "den" && editiles[x, y + 1].Isplayer != player)
                {
                    listMove.Add(editiles[x, y + 1]); listE.Add(editiles[x, y]);
                }
                else if (editiles[x, y + 1].Value == "trap" && editiles[x, y + 1].Isplayer != player)
                {
                    listMove.Add(editiles[x, y + 1]); listE.Add(editiles[x, y]);
                }
                else if (editiles[x, y + 1].Value == "mouse")
                {
                    if (editiles[x, y + 1].Value == "elephant" && editiles[x, y + 1].Isplayer != player)
                    {
                        listMove.Add(editiles[x, y + 1]); listE.Add(editiles[x, y]);
                    }
                    else if (editiles[x, y + 1].Value == "water")
                    {
                        listMove.Add(editiles[x, y + 1]); listE.Add(editiles[x, y]);
                    }
                    else
                    {
                        listMove.Add(editiles[x, y + 1]); listE.Add(editiles[x, y]);
                    }
                }
                else if ((editiles[x, y].Value == "tiger" || editiles[x, y].Value == "lion") && editiles[x, y + 1].Value == "water")
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x, y + 4].Value) && editiles[x, y + 4].Isplayer != player)
                    {
                        listMove.Add(editiles[x, y + 4]); listE.Add(editiles[x, y]);
                    }
                }
                else
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x, y + 1].Value) && editiles[x, y + 1].Isplayer != player)
                    {
                        listMove.Add(editiles[x, y + 1]); listE.Add(editiles[x, y]);
                    }
                }
            }
            if (y - 1 >= 0)
            {
                if (editiles[x, y - 1].Value == "den" && editiles[x, y - 1].Isplayer != player)
                {
                    listMove.Add(editiles[x, y - 1]); listE.Add(editiles[x, y]);
                }
                else if (editiles[x, y - 1].Value == "trap" && editiles[x, y - 1].Isplayer != player)
                {
                    listMove.Add(editiles[x, y - 1]); listE.Add(editiles[x, y]);
                }
                else if (editiles[x, y - 1].Value == "mouse")
                {
                    if (editiles[x, y - 1].Value == "elephant" && editiles[x, y - 1].Isplayer != player)
                    {
                        listMove.Add(editiles[x, y - 1]); listE.Add(editiles[x, y]);
                    }
                    else if (editiles[x, y - 1].Value == "water")
                    {
                        listMove.Add(editiles[x, y - 1]); listE.Add(editiles[x, y]);
                    }
                    else
                    {
                        listMove.Add(editiles[x, y - 1]); listE.Add(editiles[x, y]);
                    }
                }
                else if ((editiles[x, y].Value == "tiger" || editiles[x, y].Value == "lion") && editiles[x, y - 1].Value == "water")
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x, y - 4].Value) && editiles[x, y - 4].Isplayer != player)
                    {
                        listMove.Add(editiles[x, y - 4]); listE.Add(editiles[x, y]);
                    }
                }
                else
                {
                    if (get_value(editiles[x, y].Value) > get_value(editiles[x, y - 1].Value) && editiles[x, y - 1].Isplayer != player)
                    {
                        listMove.Add(editiles[x, y - 1]); listE.Add(editiles[x, y]);
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

        private void Form1_Load(object sender, EventArgs e)
        {

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
            set_edit();
            enemy_turn();
            enemy.Clear();
            user.Clear();
            us_or_foe();

            refresh_button();
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
