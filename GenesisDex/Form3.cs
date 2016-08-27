﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using GenesisDexEngine;

namespace GenesisDex
{
    public partial class FormScan : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        PokemonList pokeXML = new PokemonList();
        MoveList moveXML = new MoveList();
        AbilityList abiXML = new AbilityList();
        SkillList skillXML = new SkillList();
        CapabilityList capXML = new CapabilityList();
        ItemList typeXML = new ItemList();
        NatureList natureXML = new NatureList();
        ItemList habitatXML = new ItemList();
        List<Pokemon> pokeList = new List<Pokemon>();
        List<Move> moveList = new List<Move>();
        List<Ability> abiList = new List<Ability>();
        List<Skill> skillList = new List<Skill>();
        List<Capability> capList = new List<Capability>();
        Random rng = new Random();
        int pbPokeLocX { get; set; }
        int pbPokeLocY { get; set; }
        List<Items> habitatList = new List<Items>();
        List<Items> typeList = new List<Items>();
        List<Nature> natureList = new List<Nature>();
        List<string> habitats = new List<string>();
        List<string> types = new List<string>();
        int nature { get; set; }
        Pokemon IChooseYou = new Pokemon();
        int TrueLevel = new int();
        int Page = 1;

        public FormScan()
        {
            InitializeComponent();
            pbPokeLocX = pbPokemon.Location.X;
            pbPokeLocY = pbPokemon.Location.Y;
            pokeList = pokeXML.createList("Pokemon");
            typeList = typeXML.createList("Types", "Type");
            habitatList = habitatXML.createList("Habitats", "Habitat");
            natureList = natureXML.createList("Natures", "Nature");
            habitats.Clear();
            types.Clear();
            for (var h = 0; h < habitatList.Count; h++)
            {
                habitats.Add(habitatList[h].id);
            }
            for (var t = 0; t < typeList.Count; t++)
            {
                types.Add(typeList[t].id);
            }
            pkHabitat.DataSource = habitats;
            pkType.DataSource = types;
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pbScan_MouseHover(object sender, EventArgs e)
        {
            pbScan.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Data\\GUI\\PokedexHover.png");
        }

        private void pbScan_MouseLeave(object sender, EventArgs e)
        {
            pbScan.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Data\\GUI\\PokedexHover.png");
        }

        private void pbScan_Click_1(object sender, EventArgs e)
        {
            FormMain fm = new FormMain();
            this.Hide();
            fm.Show();
        }
        private void FormScan_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void FormScan_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void FormScan_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void pbScanPokemon_MouseHover(object sender, EventArgs e)
        {
            pbScanPokemon.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Data\\GUI\\ScanPokemonHover.png");
        }

        private void pbScanPokemon_MouseLeave(object sender, EventArgs e)
        {
            pbScanPokemon.Image = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Data\\GUI\\ScanPokemon.png");
        }

        private void pbScanPokemon_Click(object sender, EventArgs e)
        {
            pokeList.Clear();
            pokeList = pokeXML.createList("Pokemon");
            CheckHabitat();
            CheckType();
            Pokemon HeyYou = GetPokemon();
            if (HeyYou == null){ return; }
            Pokemon Pikachu = GetNature(HeyYou);
            int Level = GetLevel();
            Pokemon throwspokeball = GetGender(Pikachu);
            Pokemon PokeBall = LevelPokemon(throwspokeball, Level);
            if (pkCanBeShiny.Checked == true)
            {
                int i = rng.Next(1, 101);
                if (i == 1 || i == 100)
                {
                    pbPokemon.Image = getImage(AppDomain.CurrentDomain.BaseDirectory + "Data\\Images\\Shiny\\" + PokeBall.number + ".gif");
                }
                else
                {
                    pbPokemon.Image = getImage(AppDomain.CurrentDomain.BaseDirectory + "Data\\Images\\Pokemon\\" + PokeBall.number + ".gif");
                }

            }
            else
            {
                pbPokemon.Image = getImage(AppDomain.CurrentDomain.BaseDirectory + "Data\\Images\\Pokemon\\" + PokeBall.number + ".gif");
            }
            SetImage();
            IChooseYou = PokeBall;
            TrueLevel = Level;
            UpdatePage();

                
        }

        private Image getImage(string x)
        {
            string path = (x);
            if (File.Exists(x) == true)
            {
                return Image.FromFile(path);
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data\\Images\\MissingNo.gif"))
            {
                return Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "Data\\Images\\MissingNo.gif");
            }
            else
            {
                return Image.FromFile(null);
            }
        }

        private void CheckHabitat()
        {
            string Habitat = pkHabitat.Text;
            if (Habitat == "Any") { return; }
            for(var e = 0; e < pokeList.Count; e++)
            {
                if (pokeList[e].habitat.Contains(Habitat) == false)
                {
                    pokeList.RemoveAt(e);
                    e -= 1;
                }
            }

        }

        private void CheckType()
        {
            string Type = pkType.Text;
            if ( Type == "Any") { return; }
            for (var e = 0; e < pokeList.Count; e++)
            {
                if (pokeList[e].type.Contains(Type) == false)
                {
                    pokeList.RemoveAt(e);
                    e -= 1;
                }
                      
            }
        }

        private Pokemon GetNature(Pokemon poke)
        {
            natureList = natureXML.createList("Natures", "Nature");
            int i = rng.Next(0, natureList.Count - 1);
            int hp = Convert.ToInt32(poke.hp);
            int atk = Convert.ToInt32(poke.atk);
            int def = Convert.ToInt32(poke.def);
            int spatk = Convert.ToInt32(poke.spatk);
            int spdef = Convert.ToInt32(poke.spdef);
            int spd = Convert.ToInt32(poke.spd);
            if (natureList[i].up == "hp") { hp++; }
            else if (natureList[i].up == "atk") { atk += 2; }
            else if (natureList[i].up == "def") { def += 2; }
            else if (natureList[i].up == "spatk") { spatk += 2; }
            else if (natureList[i].up == "spdef") { spdef += 2; }
            else if (natureList[i].up == "spd") { spd += 2; }
            if (natureList[i].down == "hp") { hp--; }
            else if (natureList[i].down == "atk") { atk -= 2; }
            else if (natureList[i].down == "def") { def -= 2; }
            else if (natureList[i].down == "spatk") { spatk -= 2; }
            else if (natureList[i].down == "spdef") { spdef -= 2; }
            else if (natureList[i].down == "spd") { spd -= 2; }
            poke.hp = hp.ToString();
            poke.atk = atk.ToString();
            poke.def = def.ToString();
            poke.spatk = spatk.ToString();
            poke.spdef = spdef.ToString();
            poke.spd = spd.ToString();
            nature = i;
            return poke;
        }

        private Pokemon GetPokemon()
        {
            int i = rng.Next(0, pokeList.Count);
            try { return pokeList[i]; } catch { MessageBox.Show("There are no registered Pokemon that fit this criteria..."); return null; }
            
        }

        private int GetLevel()
        {
            int min = Convert.ToInt32(pkLevelMin.Value);
            int max = Convert.ToInt32(pkLevelMax.Value);
            int i = rng.Next(min, max);
            return i;
        }

        private void SetImage()
        {
            var pokePic = pbPokemon.Image;
            int pokeH = (pokePic.Height);
            int pbH = pbPokemon.Height;
            pbPokemon.Location = new Point(pbPokeLocX, (pbPokeLocY + ((pbH / 2) - (pokeH / 2))));
        }

        private Pokemon LevelPokemon(Pokemon poke, int level)
        {
            List<Stat> stats = new List<Stat>();
            StatList getstats = new StatList();
            int hp = Convert.ToInt32(poke.hp);
            int atk = Convert.ToInt32(poke.atk);
            int def = Convert.ToInt32(poke.def);
            int spatk = Convert.ToInt32(poke.spatk);
            int spdef = Convert.ToInt32(poke.spdef);
            int spd = Convert.ToInt32(poke.spd);
            stats = getstats.createList(hp, atk, def, spatk, spdef, spd);
            SortStats(stats);

            for (var l = level; l > 0; l--)
            {
                int i = rng.Next(1, 7);
                if ( i == 1) { if (stats[5].stat <= stats[4].stat) { stats[5].stat++; } else { l++; } }
                else if (i == 2) { if (stats[4].stat <= stats[3].stat) { stats[4].stat++; } else { l++; } }
                else if (i == 3) { if (stats[3].stat <= stats[2].stat) { stats[3].stat++; } else { l++; } }
                else if (i == 4) { if (stats[2].stat <= stats[1].stat) { stats[2].stat++; } else { l++; } }
                else if (i == 5) { if (stats[1].stat <= stats[0].stat) { stats[1].stat++; } else { l++; } }
                else if (i == 6) { stats[0].stat++; }
            }
            for (var z = 0; z < stats.Count; z++)
            {
                if (stats[z].id == "hp") { poke.hp = stats[z].stat.ToString(); }
            }
            for (var z = 0; z < stats.Count; z++)
            {
                if (stats[z].id == "atk") { poke.atk = stats[z].stat.ToString(); }
            }
            for (var z = 0; z < stats.Count; z++)
            {
                if (stats[z].id == "def") { poke.def = stats[z].stat.ToString(); }
            }
            for (var z = 0; z < stats.Count; z++)
            {
                if (stats[z].id == "spatk") { poke.spatk = stats[z].stat.ToString(); }
            }
            for (var z = 0; z < stats.Count; z++)
            {
                if (stats[z].id == "spdef") { poke.spdef = stats[z].stat.ToString(); }
            }
            for (var z = 0; z < stats.Count; z++)
            {
                if (stats[z].id == "spd") { poke.spd = stats[z].stat.ToString(); }
            }
            return poke;

        }

        private void SortStats(List<Stat> stats)
        {
            stats.Sort(delegate (Stat x, Stat y)
            {
                return y.stat.CompareTo(x.stat);
            });
        }

        private Pokemon GetGender(Pokemon poke)
        {
            string[] gender = poke.gender.Split(' ');
            string m = gender[0];
            m = m.Replace("%"," ");
            m = m.Trim();
            if (m.Contains('.'))
            {
                string[] gendermale = m.Split('.');
                m = gendermale[0];
                m = m.Trim();
            }
            int male = Convert.ToInt32(m);
            int i = rng.Next(0, 101);
            if (i <= male)
            {
                poke.gender = "Male";
                return poke;
            }
            else
            {
                poke.gender = "Female";
                return poke;
            }
        }

        private void UpdatePage()
        {
            if (Page == 0) { Page = 2; }
            if (Page == 1) { WriteInfo(); }
            if (Page == 2) { WriteMoves(); }
            if (Page == 3) { Page = 1; }

        }

        private void WriteInfo()
        {
            rtbInfo1.Text = "Number: " + IChooseYou.number + Environment.NewLine +
                "Name: " + IChooseYou.id + Environment.NewLine +
                "Type: " + IChooseYou.type + Environment.NewLine +
                Environment.NewLine +
                "Level: " + TrueLevel + Environment.NewLine +
                "Nature: " + natureList[nature].id + Environment.NewLine +
                Environment.NewLine +
                "Stats:" + Environment.NewLine +
                "HP:\t\t" + IChooseYou.hp + Environment.NewLine +
                "ATK:\t\t" + IChooseYou.atk + Environment.NewLine +
                "DEF:\t\t" + IChooseYou.def + Environment.NewLine +
                "SPATK:\t\t" + IChooseYou.spatk + Environment.NewLine +
                "SPDEF:\t\t" + IChooseYou.spdef + Environment.NewLine +
                "SPD:\t\t" + IChooseYou.spd + Environment.NewLine +
                Environment.NewLine +
                "Gender: " + IChooseYou.gender + Environment.NewLine +
                "Size: " + IChooseYou.size + Environment.NewLine +
                "Weight: " + IChooseYou.weight;
        }

        private void WriteMoves()
        {
            int i = TrueLevel;
            MoveList moveXML = new MoveList();
            List<string> moves = new List<string>();
            moveList.Clear();
            moveList = moveXML.createList(IChooseYou.number);
            rtbInfo1.Text = ("Moves:" + Environment.NewLine);
            for (var e = 0; e < moveList.Count; e++)
            {
                string[] moveLevel = moveList[e].move.Split(' ');
                int lvl = Convert.ToInt32(moveLevel[0]);
                if (lvl <= i) { moves.Add(moveList[e].move); }
                if (moves.Count > 6) { moves.RemoveAt(0); }
            }
            for (var w = 0; w < moves.Count; w++)
            {
                rtbInfo1.Text += "-" + moves[w] + Environment.NewLine;
            }
        }

        private void infoBack_Click(object sender, EventArgs e)
        {
            Page--;
            UpdatePage();
        }

        private void infoForward_Click(object sender, EventArgs e)
        {
            Page++;
            UpdatePage();
        }
    }
}
