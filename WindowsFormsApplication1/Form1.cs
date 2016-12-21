using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    #region Enums
    enum ItemType { ArmorHead, ArmorChest, ArmorFeet, MeleeWeapon, RangedWeapon, MagicWeapon };
    enum MeleeWeaponType { Sword, Axe, Dagger, Knife, Scythe, Spear, Trident, Whip, Claws, Hammer, Mace, Club, Quarterstaff, Flail };
    enum RangedWeaponType { Sling, Boomerang, Flechette, Shuriken, Javelin, Longbow, Crossbow, Pistol, Cannon, Shotgun, Rifle, Launcher };
    enum MagicWeaponType { Blaster, Raygun, Maser, Staff, Wand, Tome, Orb };
    enum ArmorHeadType { Hat, Mask, Visor, Cowl, Helmet };
    enum ArmorChestType { Tunic, Mail, Cloak, Chestplate, Vest };
    enum ArmorFeetType { Leggings, Greaves, Sandals, Shoes, Boots };
    enum BoostType { Strength, Agility, Intellect };
    #endregion
    
    public partial class Form1 : Form
    {
        List<Player> players;

        public Form1()
        {
            InitializeComponent();
            players = new List<Player>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            Item samplep = new Item(random);
            textBox1.Text = samplep.Name;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string playername = textBox4.Text;
            double playerstrength = (double)numericUpDown1.Value;
            double playeragility = (double)numericUpDown2.Value;
            double playerintellect = (double)numericUpDown3.Value;

            Player samplep = new Player(playername, playerstrength, playeragility, playerintellect);
            List<string> playerlist = PrintPlayer(samplep);
            players.Add(samplep);

            playerlist.Add("");
            playerlist.Add("Number of players: " + players.Count);

            String[] playerprint = playerlist.ToArray();

            textBox2.Lines = playerprint;
        }

        private List<string> PrintPlayer(Player samplep)
        {
            string formatString = "\n          Item {0}: {1}, Level {2}, {3:N2} DPS, {4} Protection, X{5} {6} Boost, costs {7:N2}, sells at {8:N2}";

            List<String> playerlist = new List<String>();

            playerlist.Add("Player Name: " + samplep.name);
            playerlist.Add("HP: " + samplep.HP);
            playerlist.Add("Mana: " + samplep.Mana);
            playerlist.Add("==========Stats:==========");
            playerlist.Add("Strength: " + samplep.Strength);
            playerlist.Add("Agility: " + samplep.Agility);
            playerlist.Add("Intellect: " + samplep.Intellect);

            for (int i = 0; i < samplep.items.Count; i++) {
                playerlist.Add(String.Format(formatString,
                    (i + 1), samplep.items[i].Name, samplep.items[i].Level, samplep.items[i].DPS, samplep.items[i].Protection,
                    samplep.items[i].StatMod, samplep.items[i].Boost, samplep.items[i].PurchaseCost, samplep.items[i].SellCost));
            }

            return playerlist;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            Player player1 = null;
            Player player2 = null;

            List<String> turnlist = new List<String>();

            try
            {
                if (players.Count < 2) {
                    Console.Write("\nERROR: Need more than two players.");
                    return;
                } else {
                    int pindex1 = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox("Index of Player 1?", "Select Player 1", "Default Text"));
                    int pindex2 = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox("Index of Player 2?", "Select Player 1", "Default Text"));
                    if ((pindex1 < 0) || (pindex2 < 0)) {
                        MessageBox.Show("Player index must be non-negative.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    } else {
                        player1 = players[pindex1];
                        player2 = players[pindex2];
                    }
                }
            }
            catch (FormatException)
            {
                Console.Write("\nERROR: Integers ONLY.");
                return;
            }

            turnlist.Add("COMMENCING BATTLE");
            turnlist.Add("PLAYER " + player1.name);
            turnlist.Add("VS...");
            turnlist.Add("PLAYER " + player2.name);

            int turnnum = 0;
            double HP1 = player1.HP;
            double HP2 = player2.HP;
            double Mana1 = player1.Mana;
            double Mana2 = player2.Mana;
            double TotalMana1 = Mana1;
            double TotalMana2 = Mana2;

            Tuple<double, double, double, double, List<String>> HPMeter;

            while (true)
            {
                turnnum++;
                turnlist.Add("Turn " + turnnum);

                // Get two items at random, one for each player.
                int p1itemindex = random.Next(player1.items.Count);
                int p2itemindex = random.Next(player2.items.Count);
                double p1hitchance;
                double p2hitchance;
                Item p1item;
                Item p2item;

                if (p1itemindex == 0) {
                    p1item = null;
                    p1hitchance = .5;
                } else {
                    p1item = player1.items[p1itemindex];
                    p1hitchance = p1item.HitChance / p1item.AtkSpd;
                }
                if (p2itemindex == 0) {
                    p2item = null;
                    p2hitchance = .5;
                } else {
                    p2item = player2.items[p2itemindex];
                    p2hitchance = p2item.HitChance / player2.items[p1itemindex].AtkSpd;
                }

                // Determine if each item is a weapon.
                try {
                    if (!p1item.IsWeapon() && !player2.items[p1itemindex].IsWeapon()) {
                        // Player 1 goes first, then Player 2.

                        HPMeter = PlayerHitDefinite(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                        Mana1 = HPMeter.Item3;
                        Mana2 = HPMeter.Item4;
                        turnlist = HPMeter.Item5;
                    }
                    else if (!p1item.IsWeapon() && player2.items[p1itemindex].IsWeapon()) {
                        // Determine player order.

                        if ((p1hitchance / p1item.AtkSpd > p2hitchance / p2item.AtkSpd)) {
                            if (player1.Agility > player2.Agility) {
                                HPMeter = PlayerHitDefinite(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            } else if (player1.Agility < player2.Agility) {
                                HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            } else {
                                HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            }
                        } else if ((p1hitchance / p1item.AtkSpd < p2hitchance / p2item.AtkSpd)) {
                            if (player1.Agility < player2.Agility) {
                                HPMeter = PlayerHitDefinite(player2, player1, p2itemindex, p1itemindex, HP2, HP1, Mana2, Mana1, TotalMana2, TotalMana1, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            } else if (player1.Agility > player2.Agility) {
                                HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            } else {
                                HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            }
                        } else {
                            HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                            HP1 = HPMeter.Item1;
                            HP2 = HPMeter.Item2;
                            Mana1 = HPMeter.Item3;
                            Mana2 = HPMeter.Item4;
                            turnlist = HPMeter.Item5;
                        }
                    }
                    else if (p1item.IsWeapon() && !player2.items[p1itemindex].IsWeapon())
                    {
                        // Determine player order.

                        if ((p1hitchance / p1item.AtkSpd > p2hitchance / p2item.AtkSpd))
                        {
                            if (player1.Agility > player2.Agility)
                            {
                                HPMeter = PlayerHitDefinite(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            }
                            else if (player1.Agility < player2.Agility)
                            {
                                HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            }
                            else
                            {
                                HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            }
                        }
                        else if ((p1hitchance / p1item.AtkSpd < p2hitchance / p2item.AtkSpd))
                        {
                            if (player1.Agility < player2.Agility)
                            {
                                HPMeter = PlayerHitDefinite(player2, player1, p2itemindex, p1itemindex, HP2, HP1, Mana2, Mana1, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            }
                            else if (player1.Agility > player2.Agility)
                            {
                                HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            }
                            else
                            {
                                HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                                HP1 = HPMeter.Item1;
                                HP2 = HPMeter.Item2;
                                Mana1 = HPMeter.Item3;
                                Mana2 = HPMeter.Item4;
                                turnlist = HPMeter.Item5;
                            }
                        }
                        else
                        {
                            HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                            HP1 = HPMeter.Item1;
                            HP2 = HPMeter.Item2;
                            Mana1 = HPMeter.Item3;
                            Mana2 = HPMeter.Item4;
                            turnlist = HPMeter.Item5;
                        }
                    }
                    else
                    {
                        // Determine player order.

                        if ((p1hitchance / p1item.AtkSpd > p2hitchance / p2item.AtkSpd) && player1.Agility > player2.Agility)
                        {
                            HPMeter = PlayerHitDefinite(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                            HP1 = HPMeter.Item1;
                            HP2 = HPMeter.Item2;
                            Mana1 = HPMeter.Item3;
                            Mana2 = HPMeter.Item4;
                            turnlist = HPMeter.Item5;
                        }
                        else if ((p1hitchance / p1item.AtkSpd > p2hitchance / p2item.AtkSpd) && player1.Agility < player2.Agility)
                        {
                            HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                            HP1 = HPMeter.Item1;
                            HP2 = HPMeter.Item2;
                            Mana1 = HPMeter.Item3;
                            Mana2 = HPMeter.Item4;
                            turnlist = HPMeter.Item5;
                        }
                        else if ((p1hitchance / p1item.AtkSpd < p2hitchance / p2item.AtkSpd) && player1.Agility < player2.Agility)
                        {
                            HPMeter = PlayerHitDefinite(player2, player1, p2itemindex, p1itemindex, HP2, HP1, Mana2, Mana1, TotalMana1, TotalMana2, random, turnlist);
                            HP1 = HPMeter.Item1;
                            HP2 = HPMeter.Item2;
                            Mana1 = HPMeter.Item3;
                            Mana2 = HPMeter.Item4;
                            turnlist = HPMeter.Item5;
                        }
                        else if ((p1hitchance / p1item.AtkSpd < p2hitchance / p2item.AtkSpd) && player1.Agility > player2.Agility)
                        {
                            HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                            HP1 = HPMeter.Item1;
                            HP2 = HPMeter.Item2;
                            Mana1 = HPMeter.Item3;
                            Mana2 = HPMeter.Item4;
                            turnlist = HPMeter.Item5;
                        }
                        else
                        {
                            HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                            HP1 = HPMeter.Item1;
                            HP2 = HPMeter.Item2;
                            Mana1 = HPMeter.Item3;
                            Mana2 = HPMeter.Item4;
                            turnlist = HPMeter.Item5;
                        }
                    }
                } catch (ArgumentOutOfRangeException) {
                    HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                    HP1 = HPMeter.Item1;
                    HP2 = HPMeter.Item2;
                    Mana1 = HPMeter.Item3;
                    Mana2 = HPMeter.Item4;
                    turnlist = HPMeter.Item5;
                } catch (NullReferenceException) {
                    if (player1.Agility > player2.Agility) {
                        HPMeter = PlayerHitDefinite(player1, player2, 0, 0, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                        Mana1 = HPMeter.Item3;
                        Mana2 = HPMeter.Item4;
                        turnlist = HPMeter.Item5;
                    } else if (player1.Agility < player2.Agility) {
                        HPMeter = PlayerHitDefinite(player2, player1, 0, 0, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                        Mana1 = HPMeter.Item3;
                        Mana2 = HPMeter.Item4;
                        turnlist = HPMeter.Item5;
                    } else {
                        HPMeter = PlayerHitRandom(player1, player2, 0, 0, HP1, HP2, Mana1, Mana2, TotalMana1, TotalMana2, random, turnlist);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                        Mana1 = HPMeter.Item3;
                        Mana2 = HPMeter.Item4;
                        turnlist = HPMeter.Item5;
                    }
                }

                turnlist.Add("          " + player1.name + " now at " + HP1 + " HP and " + Mana1 + " Mana");
                turnlist.Add("          " + player2.name + " now at " + HP2 + " HP and " + Mana2 + " Mana");
                
                if (HP1 <= 0) {
                    turnlist.Add("PLAYER " + player2.name + " WINS");
                    break;
                } else if (HP2 <= 0) {
                    turnlist.Add("PLAYER " + player1.name + " WINS");
                    break;
                }
                if (turnnum >= 100) {
                    break;
                }
            }

            String[] playerprint = turnlist.ToArray();
            textBox3.Lines = playerprint;
        }

        private static Tuple<double, double, double, double, List<String>> PlayerHitDefinite(Player player1, Player player2, int p1itemindex, int p2itemindex,
            double HP1, double HP2, double Mana1, double Mana2, double TotalMana1, double TotalMana2, Random random, List<String> turnlist)
        {

            double damage;
            double p1protection = player1.Protection;
            double p2protection = player2.Protection;

            try
            {
                Item p1item = player1.items[p1itemindex];
                Item p2item = player2.items[p2itemindex];

                if (p1protection == 0) { p1protection = 1; }
                if (p2protection == 0) { p2protection = 1; }

                if (random.NextDouble() >= (1 - p1item.HitChance)) {
                    if (Mana1 > 0) {
                        Mana1 = Mana1 - TotalMana1 * 0.01 * p1item.DPS / (10 * player1.Intellect);
                    }
                    //turnlist.Add("          Player " + player1.name + " has " + Mana1 + " Mana left.");
                    if (Mana1 <= 0) {
                        damage = player1.Strength / p2protection;
                        HP2 = HP2 - damage;
                    } else {
                        damage = player1.Strength * (1 + p1item.DPS / p2protection);
                        HP2 = HP2 - damage;
                    }
                    turnlist.Add("          Player " + player1.name + " strikes " + player2.name + " for " + damage + " Damage!");
                    //HP2 = HP2 - player1.Strength * (1 + p1item.DPS / player2.Protection);
                    if (HP2 <= 0) {
                        return Tuple.Create(HP1, 0.0, Mana1, Mana2, turnlist);
                    }
                } else {
                    turnlist.Add("          Player " + player1.name + " misses!");
                }

                if (random.NextDouble() >= (1 - p2item.HitChance)) {
                    if (Mana2 > 0) {
                        Mana2 = Mana2 - TotalMana2 * 0.01 * p2item.DPS / (10 * player2.Intellect);
                    }
                    //turnlist.Add("          Player " + player2.name + " has " + Mana2 + " Mana left.");
                    if (Mana2 <= 0) {
                        damage = player2.Strength / p1protection;
                        HP1 = HP1 - damage;
                    } else {
                        damage = player2.Strength * (1 + player2.items[p1itemindex].DPS / p1protection);
                        HP1 = HP1 - damage;
                    }
                    turnlist.Add("          Player " + player2.name + " strikes " + player1.name + " for " + damage + " Damage!");
                    if (HP1 <= 0) {
                        return Tuple.Create(0.0, HP2, Mana1, Mana2, turnlist);
                    }
                } else {
                    turnlist.Add("          Player " + player2.name + " misses!");
                }
                return Tuple.Create(HP1, HP2, Mana1, Mana2, turnlist);
            }
            catch (ArgumentOutOfRangeException)
            {
                if (random.NextDouble() > .5) {
                    damage = player1.Strength;
                    HP2 = HP2 - damage;
                    turnlist.Add("          Player " + player1.name + " strikes " + player2.name + " for " + damage + " Damage!");

                    if (HP2 <= 0) {
                        return Tuple.Create(HP1, 0.0, Mana1, Mana2, turnlist);
                    }
                } else {
                    turnlist.Add("          Player " + player1.name + " misses!");
                }
                if (random.NextDouble() > .5) {
                    damage = player2.Strength;
                    HP1 = HP1 - damage;
                    turnlist.Add("          Player " + player2.name + " strikes " + player1.name + " for " + damage + " Damage!");

                    if (HP1 <= 0) {
                        return Tuple.Create(0.0, HP2, Mana1, Mana2, turnlist);
                    }
                } else {
                    turnlist.Add("          Player " + player2.name + " misses!");
                }
            }

            return Tuple.Create(HP1, HP2, Mana1, Mana2, turnlist);
        }

        private static Tuple<double, double, double, double, List<String>> PlayerHitRandom(Player player1, Player player2, int p1itemindex, int p2itemindex,
            double HP1, double HP2, double Mana1, double Mana2, double TotalMana1, double TotalMana2, Random random, List<String> turnlist) {
            double damage;
            // First, determine hit chance for each player.
            try {
                Item p1item = player1.items[p1itemindex];
                Item p2item = player2.items[p2itemindex];
                double p1hitchance = p1item.HitChance / p1item.AtkSpd;
                double p2hitchance = p2item.HitChance / player2.items[p1itemindex].AtkSpd;
                double p1protection = player1.Protection;
                double p2protection = player2.Protection;

                if (p1protection == 0) { p1protection = 1; }
                if (p2protection == 0) { p2protection = 1; }

                if (p1hitchance >= p2hitchance)
                {
                    //Determine likelihood of each hit.
                    if (random.NextDouble() > (1 - p1item.HitChance)) {
                        if (Mana1 > 0) {
                            Mana1 = Mana1 - TotalMana1 * 0.01 * p1item.DPS / (10 * player1.Intellect);
                        }
                        //turnlist.Add("          Player " + player1.name + " has " + Mana1 + " Mana left.");
                        if (Mana1 <= 0) {
                            damage = player1.Strength / p2protection;
                            HP2 = HP2 - damage;
                        } else {
                            damage = player1.Strength * (1 + p1item.DPS / p2protection);
                            HP2 = HP2 - damage;
                        }
                        turnlist.Add("          Player " + player1.name + " strikes " + player2.name + " for " + damage + " Damage!");

                        if (HP2 <= 0) {
                            return Tuple.Create(HP1, 0.0, Mana1, Mana2, turnlist);
                        }
                    } else {
                        turnlist.Add("          Player " + player1.name + " misses!");
                    }
                    if (random.NextDouble() > (1 - p2item.HitChance)) {
                        if (Mana2 > 0) {
                            Mana2 = Mana2 - TotalMana2 * 0.01 * p2item.DPS / (10 * player2.Intellect);
                        }
                        //turnlist.Add("          Player " + player2.name + " has " + Mana2 + " Mana left.");
                        if (Mana2 <= 0) {
                            damage = player2.Strength / player1.Protection;
                            HP1 = HP1 - damage;
                        } else {
                            damage = player2.Strength * (1 + player2.items[p1itemindex].DPS / p1protection);
                            HP1 = HP1 - damage;
                        }
                        turnlist.Add("          Player " + player2.name + " strikes " + player1.name + " for " + damage + " Damage!");
                        if (HP1 <= 0) {
                            return Tuple.Create(0.0, HP2, Mana1, Mana2, turnlist);
                        }
                    } else {
                        turnlist.Add("          Player " + player2.name + " misses!");
                    }
                } else {
                    //Determine likelihood of each hit.
                    if (random.NextDouble() > (1 - p1item.HitChance)) {
                        if (Mana1 > 0) {
                            Mana1 = Mana1 - TotalMana1 * 0.01 * p1item.DPS / (10 * player1.Intellect);
                        }
                        //turnlist.Add("          Player " + player1.name + " has " + Mana1 + " Mana left.");
                        if (Mana1 <= 0) {
                            damage = player1.Strength / p2protection;
                            HP2 = HP2 - damage;
                        } else {
                            damage = player1.Strength * (1 + p1item.DPS / p2protection);
                            HP2 = HP2 - damage;
                        }
                        turnlist.Add("          Player " + player1.name + " strikes " + player2.name + " for " + damage + " Damage!");
                        if (HP2 <= 0) {
                            return Tuple.Create(HP1, 0.0, Mana1, Mana2, turnlist);
                        }
                    } else {
                        turnlist.Add("          Player " + player1.name + " misses!");
                    }
                    if (random.NextDouble() > (1 - p2item.HitChance)) {
                        if (Mana1 > 0) {
                            Mana2 = Mana2 - TotalMana2 * 0.01 * p2item.DPS / (10 * player2.Intellect);
                        }
                        //turnlist.Add("          Player " + player2.name + " has " + Mana2 + " Mana left.");
                        if (Mana2 <= 0) {
                            damage = player2.Strength / p1protection;
                            HP1 = HP1 - damage;
                        } else {
                            damage = player2.Strength * (1 + player2.items[p1itemindex].DPS / p1protection);
                            HP1 = HP1 - damage;
                        }
                        turnlist.Add("          Player " + player2.name + " strikes " + player1.name + " for " + damage + " Damage!");
                        if (HP1 <= 0) {
                            return Tuple.Create(0.0, HP2, Mana1, Mana2, turnlist);
                        }
                    } else {
                        turnlist.Add("          Player " + player2.name + " misses!");
                    }
                }
            } catch (ArgumentOutOfRangeException) {
                if (random.NextDouble() > .5) {
                    damage = player1.Strength;
                    HP2 = HP2 - damage;
                    turnlist.Add("          Player " + player1.name + " strikes " + player2.name + " for " + damage + " Damage!");

                    if (HP2 <= 0) {
                        return Tuple.Create(HP1, 0.0, Mana1, Mana2, turnlist);
                    }
                } else {
                    turnlist.Add("          Player " + player1.name + " misses!");
                }
                if (random.NextDouble() > .5) {
                    damage = player2.Strength;
                    HP1 = HP1 - damage;
                    turnlist.Add("          Player " + player2.name + " strikes " + player1.name + " for " + damage + " Damage!");

                    if (HP1 <= 0) {
                        return Tuple.Create(0.0, HP2, Mana1, Mana2, turnlist);
                    }
                } else {
                    turnlist.Add("          Player " + player2.name + " misses!");
                }
            }

            return Tuple.Create(HP1, HP2, Mana1, Mana2, turnlist);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try {
                Player samplep = players[(int)numericUpDown4.Value];
                List<string> playerlist = PrintPlayer(samplep);
                playerlist.Add("");
                playerlist.Add("Number of players: " + players.Count);

                String[] playerprint = playerlist.ToArray();

                textBox2.Lines = playerprint;
            } catch (ArgumentOutOfRangeException) {
                String[] playerprint = { "ERROR: Index out of player list range." };
                textBox2.Lines = playerprint;
            }
        }
    }

    public class Item
    {
        // Fields for ID and variables
        public int Level;
        private ItemType Type;
        private BoostType boost_type;
        private string TypeName;
        public string Name;
        public string Boost;
        public double AtkSpd;
        public double Damage;
        public double DPS;
        public double Protection;
        public double HitChance;
        public double PurchaseCost;
        public double SellCost;
        public double StatMod;
        private double StatChance;
        private bool statflag;
        private bool isweapon;

        public Item(Random random)
        {            
            SetItem(random);
        }

        private void SetItem(Random random)
        {

            Level = random.Next(0, 10);

            Array ItemValues = Enum.GetValues(typeof(ItemType));
            Type = (ItemType)ItemValues.GetValue(random.Next(ItemValues.Length));

            if (Type == ItemType.ArmorChest) {
                isweapon = false;
                Array ArmorChestValues = Enum.GetValues(typeof(ArmorChestType));
                TypeName = ArmorChestValues.GetValue(random.Next(ArmorChestValues.Length)).ToString();
            } else if (Type == ItemType.ArmorFeet) {
                isweapon = false;
                Array ArmorChestValues = Enum.GetValues(typeof(ArmorFeetType));
                TypeName = ArmorChestValues.GetValue(random.Next(ArmorChestValues.Length)).ToString();
            } else if (Type == ItemType.ArmorHead) {
                isweapon = false;
                Array ArmorChestValues = Enum.GetValues(typeof(ArmorHeadType));
                TypeName = ArmorChestValues.GetValue(random.Next(ArmorChestValues.Length)).ToString();
            } else if (Type == ItemType.MagicWeapon) {
                isweapon = true;
                Array ArmorChestValues = Enum.GetValues(typeof(MagicWeaponType));
                TypeName = ArmorChestValues.GetValue(random.Next(ArmorChestValues.Length)).ToString();
            } else if (Type == ItemType.MeleeWeapon) {
                isweapon = true;
                Array ArmorChestValues = Enum.GetValues(typeof(MeleeWeaponType));
                TypeName = ArmorChestValues.GetValue(random.Next(ArmorChestValues.Length)).ToString();
            } else if (Type == ItemType.RangedWeapon) {
                isweapon = true;
                Array ArmorChestValues = Enum.GetValues(typeof(RangedWeaponType));
                TypeName = ArmorChestValues.GetValue(random.Next(ArmorChestValues.Length)).ToString();
            }

            AtkSpd = random.NextDouble() * 1.5 + 0.5;
            HitChance = 0.9 + 0.05 * Level;
            Damage = random.Next(5, 9) * Level;

            if ((Type == ItemType.MagicWeapon) || (Type == ItemType.MeleeWeapon) || (Type == ItemType.RangedWeapon))
            {
                DPS = (random.NextDouble() + AtkSpd * HitChance) * Damage;
                Protection = 0;
                PurchaseCost = (Level * DPS + Level * StatMod) * 100;
            }
            else
            {
                Protection = random.Next(1, 4) * Level;
                DPS = 0;
                PurchaseCost = (Level * Protection + Level * StatMod) * 100;
            }
            
            SellCost = PurchaseCost * random.Next(2, 5) / 10;

            StatChance = Level * .08;
            StatMod = Level / 2 * random.Next(1, 4);
            
            double stattest = random.NextDouble();
            if (stattest > StatChance) { statflag = true; } else { StatMod = 0; }

            Array BoostValues = Enum.GetValues(typeof(BoostType));
            boost_type = (BoostType)BoostValues.GetValue(random.Next(BoostValues.Length));

            string newname = TypeName.ToString();
            if (Level == 10) { newname = "Legendary " + newname; }
            else if (Level > 6) {
                if (Type == ItemType.ArmorChest || Type == ItemType.ArmorHead || Type == ItemType.ArmorFeet) {
                    newname = "Rugged " + newname;
                } else {
                    newname = "Epic " + newname;
                }
            }
            else if (Level > 2) {
                if (Type == ItemType.ArmorChest || Type == ItemType.ArmorHead || Type == ItemType.ArmorFeet) {
                    newname = "Mighty " + newname;
                } else {
                    newname = "Reinforced " + newname;
                }
            }

            if (statflag == true && StatMod > 0) {
                if (boost_type == BoostType.Strength) {
                    Boost = "Strength";
                    newname = newname + " of Strength";
                } else if (boost_type == BoostType.Agility) {
                    Boost = "Agility";
                    newname = newname + " of Agility";
                } else if (boost_type == BoostType.Intellect) {
                    Boost = "Intellect";
                    newname = newname + " of Intellect";
                }
            }
            else { Boost = "No"; }

            Name = newname;
        }

        public bool IsWeapon() { return isweapon; }
    }

    public class Player
    {
        // Fields for ID and variables
        public string name;

        //We're assuming all players have HP and Mana bars, like in a typical RPG.
        public int Level;
        public double HP;
        public double Mana;

        //The three stats are Strength (determines attack damage), Agility (determines evasion), and Intellect (determines mana usage).
        public double Strength;
        public double Agility;
        public double Intellect;

        public List<Item> items;

        public bool isCreated;
        public double Protection;

        public Player(string pname, double pstrength, double pagility, double pintellect)
        {
            name = pname;
            Strength = pstrength;
            Agility = pagility;
            Intellect = pintellect;
            isCreated = false;

            HP = 100;
            Mana = 100;

            try {
                int x = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox("How many items?", "Item Number", "Default Text"));
                if (x < 0)
                {
                    MessageBox.Show("Item number must be non-negative.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    Random random = new Random();
                    List<Item> itemslist = new List<Item>();
                    for (int i = 0; i < x; i++)
                    {
                        Item sample2 = new Item(random);
                        itemslist.Add(sample2);
                    }
                    items = itemslist;
                }

                // Every item in the inventory has a chance of providing ONE stat boost. These boosts are divided by 4 before being added to specific stats.
                for (int i = 0; i < x; i++)
                {
                    double boost = items[i].StatMod;
                    if (items[i].Boost == BoostType.Strength.ToString())
                    {
                        Strength += items[i].StatMod / 4;
                    }
                    else if (items[i].Boost == BoostType.Agility.ToString())
                    {
                        Agility += items[i].StatMod / 4;
                    }
                    else if (items[i].Boost == BoostType.Intellect.ToString())
                    {
                        Intellect += items[i].StatMod / 4;
                    }
                }
            } catch (FormatException) {
                MessageBox.Show("Item number must be an integer.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for (int i = 0; i < items.Count; i++) {
                if (!items[i].IsWeapon()) { Protection += items[i].Protection; }
            }

            isCreated = true;
        }
    }
}
