using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotoItemGenerator
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

    #region Classes

    /// <summary>
    /// Items are procedurally generated. Their type determines their usage, whether or not they provide damage/protection
    /// and how much, purchase and selling cost, and stat modifiers (if any). The name of each item is determined from a combination
    /// of the item's type, level, and stat boost.
    /// </summary>
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

        //Generate an item.
        public Item() {
            statflag = false;
            Name = "";
            Random random = new Random();
            SetItem(random);
        }

        public Item(Random random)
        {
            statflag = false;
            Name = "";
            SetItem(random);
        }

        public void SetItem(Random random) {
            isweapon = false;
            //Determine item type.
            Array ItemValues = Enum.GetValues(typeof(ItemType));
            Type = (ItemType)ItemValues.GetValue(random.Next(ItemValues.Length));

            Array BoostValues = Enum.GetValues(typeof(BoostType));
            boost_type = (BoostType)BoostValues.GetValue(random.Next(BoostValues.Length));

            //Determine item name, depending on the category.
            if (Type == ItemType.ArmorChest) {
                Array ArmorChestValues = Enum.GetValues(typeof(ArmorChestType));
                TypeName = ArmorChestValues.GetValue(random.Next(ArmorChestValues.Length)).ToString();
            } else if (Type == ItemType.ArmorHead) {
                Array ArmorHeadValues = Enum.GetValues(typeof(ArmorHeadType));
                TypeName = ArmorHeadValues.GetValue(random.Next(ArmorHeadValues.Length)).ToString();
            } else if (Type == ItemType.ArmorFeet) {
                Array ArmorFeetValues = Enum.GetValues(typeof(ArmorFeetType));
                TypeName = ArmorFeetValues.GetValue(random.Next(ArmorFeetValues.Length)).ToString();
            } else if (Type == ItemType.MagicWeapon) {
                isweapon = true;
                Array MagicWeaponValues = Enum.GetValues(typeof(MagicWeaponType));
                TypeName = MagicWeaponValues.GetValue(random.Next(MagicWeaponValues.Length)).ToString();
            } else if (Type == ItemType.MeleeWeapon) {
                isweapon = true;
                Array MeleeWeaponValues = Enum.GetValues(typeof(MeleeWeaponType));
                TypeName = MeleeWeaponValues.GetValue(random.Next(MeleeWeaponValues.Length)).ToString();
            } else if (Type == ItemType.RangedWeapon) {
                isweapon = true;
                Array RangedWeaponValues = Enum.GetValues(typeof(RangedWeaponType));
                TypeName = RangedWeaponValues.GetValue(random.Next(RangedWeaponValues.Length)).ToString();
            }

            //Generate/calculate item stats, including attack speed, damage, hit chance, purchase/sell cost, and modifiers.
            Level = random.Next(0, 10);
            AtkSpd = random.NextDouble() * 1.5 + .5;
            Damage = random.Next(5, 9) * Level;
            HitChance = .9 + Level * .05;
            StatChance = Level * .08;
            StatMod = Level / 2 * random.Next(1, 4);
            if ((Type == ItemType.MagicWeapon) || (Type == ItemType.MeleeWeapon) || (Type == ItemType.RangedWeapon)) {
                DPS = (random.NextDouble() + AtkSpd * HitChance) * Damage;
                Protection = 0;
            } else {
                Protection = random.Next(1, 4) * Level;
                DPS = 0;
            }
            PurchaseCost = (Level * DPS + Level * StatMod) * 100;
            SellCost = PurchaseCost * random.Next(2, 5) / 10;

            double stattest = random.NextDouble();
            if (stattest > StatChance) { statflag = true; } else { StatMod = 0; }

            //Automatically generate the item name.
            string newname = TypeName.ToString();
            if (Level == 10) { newname = "Legendary " + newname; }
            else if (Level > 6) {
                if (Type == ItemType.ArmorChest || Type == ItemType.ArmorHead || Type == ItemType.ArmorFeet) {
                    newname = "Rugged " + newname;
                } else {
                    newname = "Epic " + newname;
                }
            } else if (Level > 2) {
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
            } else { Boost = "No"; }

            Name = newname;
        }

        public bool IsWeapon () { return isweapon; }
    }

    /// <summary>
    /// The Player class includes the following fields:
    ///    - Name
    ///    - Level
    ///    - HP and Mana
    ///    - Stats (Strength, Agility, Intellect)
    ///    - Item list
    /// This class defines a player character, their starting inventory, and their starting stats based on said inventory.
    /// Assuming this is within the context of a single game, the player always starts at Level 1, with fixed HP and Mana.
    /// </summary>
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

        public Player() {
            Console.Write("\nSelect a name for your player: ");
            isCreated = false;
            string nm = Console.ReadLine();
            name = nm;
            items = new List<Item>();

            Level = 1;
            HP = 100;
            Mana = 100;
            Protection = 0;

            try {
                /*
                Console.Write("HP? ");
                int newhp = Convert.ToInt32(Console.ReadLine());
                HP = newhp;
                Console.Write("Mana? ");
                int newmana = Convert.ToInt32(Console.ReadLine());
                Mana = newmana;
                */
                Console.Write("Strength? ");
                int newstrength = Convert.ToInt32(Console.ReadLine());
                Strength = newstrength;
                Console.Write("Agility? ");
                int newagility = Convert.ToInt32(Console.ReadLine());
                Agility = newagility;
                Console.Write("Intellect? ");
                int newintellect = Convert.ToInt32(Console.ReadLine());
                Intellect = newintellect;

                Console.Write("\nHow many items will you give your player? ");
                int x = Convert.ToInt32(Console.ReadLine());
                if (x < 0) {
                    Console.Write("\nERROR: Item number must be non-negative.");
                    return;
                } else {
                    Random random = new Random();
                    List<Item> itemslist = new List<Item>();
                    for (int i = 0; i < x; i++) {
                        Item sample2 = new Item(random);
                        itemslist.Add(sample2);
                    }
                    items = itemslist;
                }

                // Every item in the inventory has a chance of providing ONE stat boost. These boosts are divided by 4 before being added to specific stats.
                for (int i = 0; i < x; i++) {
                    double boost = items[i].StatMod;
                    if (items[i].Boost == BoostType.Strength.ToString()) {
                        Strength += items[i].StatMod/4;
                    } else if (items[i].Boost == BoostType.Agility.ToString()) {
                        Agility += items[i].StatMod / 4;
                    } else if (items[i].Boost == BoostType.Intellect.ToString()) {
                        Intellect += items[i].StatMod / 4;
                    }
                }

                for (int i = 0; i < items.Count; i++)
                {
                    if (!items[i].IsWeapon()) { Protection += items[i].Protection; }
                }

                isCreated = true;
            } catch (FormatException) {
                Console.Write("\nERROR: Integers ONLY.");
                return;
            }
        }

        public void SetName(string newName) { name = newName; }
        public void SetHP(double newHP) { HP = newHP; }
        public void SetMana(double newMana) { Mana = newMana; }
        public void SetStrength(double newStrength) { Strength = newStrength; }
        public void SetAgility(double newAgility) { Agility = newAgility; }
        public void SetIntellect(double newIntellect) { Intellect = newIntellect; }
        public void SetItemList(List<Item> newItemList) { items = newItemList; }

        public void PrintPlayer()
        {
            string formatString = "\n          Item {0}: {1}, Level {2}, {3:N2} DPS, {4} Protection, X{5} {6} Boost, costs {7:N2}, sells at {8:N2}";

            Console.Write("\nPlayer Name: " + name);
            Console.Write("\nHP: " + HP);
            Console.Write("\nMana: " + Mana);
            Console.Write("\n\n==========Stats:==========");
            Console.Write("\nStrength: " + Strength);
            Console.Write("\nAgility: " + Agility);
            Console.Write("\nIntellect: " + Intellect);
            for (int i = 0; i < items.Count; i++) {
                Console.Write(String.Format(formatString,
                    (i+1), items[i].Name, items[i].Level, items[i].DPS, items[i].Protection,
                    items[i].StatMod, items[i].Boost, items[i].PurchaseCost, items[i].SellCost));
            }
        }
    }
    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            List<Player> players = new List<Player>();

            Console.WriteLine("WELCOME. Press any key to start.");
            Console.ReadKey();

            while (true) {
                Console.WriteLine("\nPress W to generate a weapon.");
                Console.WriteLine("Press P to generate a sample player.");
                Console.WriteLine("Press B to generate a player battle.");
                Console.WriteLine("Press E to exit the program.");
                char c = Console.ReadKey().KeyChar;
                if (c == 'w' || c == 'W')
                {
                    string formatString = "\nResult: {0}, Level {1}, {2:N2} DPS, {3} Protection, X{4} {5} Boost, costs {6:N2}, sells at {7:N2}";
                    Item sample = new Item();
                    Console.Write(String.Format(formatString, sample.Name, sample.Level, sample.DPS, sample.Protection,
                        sample.StatMod, sample.Boost, sample.PurchaseCost, sample.SellCost));
                    Console.WriteLine("\nCONTINUE? Y/N ");
                    c = Console.ReadKey().KeyChar;
                    if (c == 'y' || c == 'Y') { continue; }
                    else if (c == 'n' || c == 'N') { break; }
                }
                else if (c == 'p' || c == 'P')
                {
                    Player samplep = new Player();
                    if (samplep.isCreated) {
                        players.Add(samplep);
                        samplep.PrintPlayer();
                    }
                    Console.WriteLine("\nNumber of players: " + players.Count);
                    Console.WriteLine("\nCONTINUE? Y/N ");
                    c = Console.ReadKey().KeyChar;
                    if (c == 'y' || c == 'Y') { continue; }
                    else if (c == 'n' || c == 'N') { break; }
                }
                else if (c == 'b' || c == 'B')
                {
                    try
                    {
                        if (players.Count < 2) { Console.Write("\nERROR: Need more than two players."); }
                        else {
                            Console.Write("ID for Player 1? ");
                            int p1 = Convert.ToInt32(Console.ReadLine());
                            Console.Write("ID for Player 2? ");
                            int p2 = Convert.ToInt32(Console.ReadLine());

                            if ((p1 < 0) || (p2 < 0)) { Console.Write("\nERROR: ID must be non-negative."); }
                            else {
                                BattleSim(players[p1], players[p2]);
                            }
                        }
                    }
                    catch (FormatException)
                    {
                        Console.Write("\nERROR: Integers ONLY.");
                    }
                    Console.WriteLine("\nCONTINUE? Y/N ");
                    c = Console.ReadKey().KeyChar;
                    if (c == 'y' || c == 'Y') { continue; }
                    else if (c == 'n' || c == 'N') { break; }
                }
                else if (c == 'e' || c == 'E')
                {
                    break;
                }
                else { continue; }
            }

            Console.WriteLine("\nGAME OVER! Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// We will assume that this is a turn-based RPG.
        /// For each turn, a random item is selected from the inventory for the player to use during that turn.
        /// - If neither player uses a weapon, Player 1 attacks, then Player 2.
        /// - If only one player is armed, the armed player has their attack chance calculated by Hit Chance/Attack Speed.
        ///      Unarmed players have an attack speed of 1 second. If the armed player's attack chance is < 1, AND if the unarmed player's Agility
        ///      is HIGHER than that of the armed player, the unarmed player goes first.
        ///      If the unarmed player's Agility is LOWER than that of the armed player, the turn order is randomly decided.
        /// - If both players are armed, the order in which the players try to hit each other depends on attack speed and chance to hit.
        ///      If Player 1's Hit Chance/Attack Speed is greater than that of Player 2 AND Player 1's Agility is HIGHER than that of Player 2, Player 1 goes first.
        ///      If Player 1's Hit Chance/Attack Speed is greater than that of Player 2 AND Player 1's Agility is LOWER than that of Player 2, the turn order is random.
        ///      Vice versa also applies.
        /// The hit chance for each player is randomly selected. If the number is greater than 1-Hit Chance, the player will hit the opponent.
        /// Damage is calculated based on the ratio of the item's DPS and the Protection on the foe, with an added Strength modifier.

        /// Each attack using a weapon costs mana, costing 1% of the player's total mana multiplied by a factor of DPS/(100*Intellect).
        /// Magical weapons use twice as much mana as non-magical weapons. The higher the Intellect, the less mana is used for each attack.

        /// The battle ends if one player's health is reduced to 0. If this occurs in the middle of a turn (e.g. if Player 2 went first
        /// and killed Player 1), the battle ends with the surviving player automatically being declared the winner.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        private static void BattleSim(Player player1, Player player2)
        {
            Random random = new Random();
            int turnnum = 0;
            double HP1 = player1.HP;
            double HP2 = player2.HP;
            Tuple<double, double> HPMeter;

            while (true) {
                turnnum++;
                Console.WriteLine("Turn " + turnnum);

                // Get two items at random, one for each player.
                int p1itemindex = random.Next(player1.items.Count);
                int p2itemindex = random.Next(player2.items.Count);
                double p1hitchance = player1.items[p1itemindex].HitChance / player1.items[p1itemindex].AtkSpd;
                double p2hitchance = player2.items[p2itemindex].HitChance / player2.items[p1itemindex].AtkSpd;

                // Determine if each item is a weapon.
                if (!player1.items[p1itemindex].IsWeapon() && !player2.items[p1itemindex].IsWeapon()) {
                    // Player 1 goes first, then Player 2.

                    PlayerHitDefinite(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                } else if (!player1.items[p1itemindex].IsWeapon() && player2.items[p1itemindex].IsWeapon()){
                    // Determine player order.

                    if ((p1hitchance / player1.items[p1itemindex].AtkSpd > p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility > player2.Agility) {
                        HPMeter = PlayerHitDefinite(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    } else if ((p1hitchance / player1.items[p1itemindex].AtkSpd > p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility < player2.Agility) {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    } else {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    }

                    if ((p1hitchance / player1.items[p1itemindex].AtkSpd < p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility < player2.Agility) {
                        HPMeter = PlayerHitDefinite(player2, player1, p2itemindex, p1itemindex, HP2, HP1, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    } else if ((p1hitchance / player1.items[p1itemindex].AtkSpd < p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility > player2.Agility) {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    } else {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    }
                } else if (player1.items[p1itemindex].IsWeapon() && !player2.items[p1itemindex].IsWeapon()) {
                    // Determine player order.

                    if ((p1hitchance / player1.items[p1itemindex].AtkSpd > p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility > player2.Agility) {
                        HPMeter = PlayerHitDefinite(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    } else if ((p1hitchance / player1.items[p1itemindex].AtkSpd > p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility < player2.Agility) {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    } else {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    }

                    if ((p1hitchance / player1.items[p1itemindex].AtkSpd < p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility < player2.Agility) {
                        HPMeter = PlayerHitDefinite(player2, player1, p2itemindex, p1itemindex, HP2, HP1, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    } else if ((p1hitchance / player1.items[p1itemindex].AtkSpd < p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility > player2.Agility) {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    } else {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    }
                }
                else {
                    // Determine player order.

                    if ((p1hitchance / player1.items[p1itemindex].AtkSpd > p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility > player2.Agility) {
                        HPMeter = PlayerHitDefinite(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    }
                    else if ((p1hitchance / player1.items[p1itemindex].AtkSpd > p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility < player2.Agility) {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    }
                    else if ((p1hitchance / player1.items[p1itemindex].AtkSpd < p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility < player2.Agility) {
                        HPMeter = PlayerHitDefinite(player2, player1, p2itemindex, p1itemindex, HP2, HP1, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    }
                    else if ((p1hitchance / player1.items[p1itemindex].AtkSpd < p2hitchance / player2.items[p2itemindex].AtkSpd) && player1.Agility > player2.Agility) {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    }
                    else {
                        HPMeter = PlayerHitRandom(player1, player2, p1itemindex, p2itemindex, HP1, HP2, random);
                        HP1 = HPMeter.Item1;
                        HP2 = HPMeter.Item2;
                    }
                    
                }

                Console.WriteLine("          HP 1: " + HP1);
                Console.WriteLine("          HP 2: " + HP2);

                //TO DO: Determine whether HP for each player is down to zero.
                if (HP1 <= 0) {
                    Console.WriteLine(player2.name + " WINS");
                    break;
                } else if (HP2 <= 0) {
                    Console.WriteLine(player1.name + " WINS");
                    break;
                }
                if (turnnum >= 100) {
                    break;
                }
            }

        }

        private static Tuple<double, double> PlayerHitDefinite(Player player1, Player player2, int p1itemindex, int p2itemindex, double HP1, double HP2, Random random)
        {
            if (random.NextDouble() >= (1 - player1.items[p1itemindex].HitChance)) { HP2 = HP2 - player1.Strength * (1 + player1.items[p1itemindex].DPS / player2.Protection); }
            if (random.NextDouble() >= (1 - player2.items[p2itemindex].HitChance)) { HP1 = HP1 - player2.Strength * (1 + player2.items[p1itemindex].DPS / player1.Protection); }

            //Console.WriteLine("          HP 1: " + HP1);
            //Console.WriteLine("          HP 2: " + HP2);

            return Tuple.Create(HP1, HP2);
        }

        private static Tuple<double, double> PlayerHitRandom(Player player1, Player player2, int p1itemindex, int p2itemindex, double HP1, double HP2, Random random)
        {
            // First, determine hit chance for each player.
            double p1hitchance = player1.items[p1itemindex].HitChance / player1.items[p1itemindex].AtkSpd;
            double p2hitchance = player2.items[p2itemindex].HitChance / player2.items[p1itemindex].AtkSpd;

            if (p1hitchance >= p2hitchance)
            {
                //Determine likelihood of each hit.
                if (random.NextDouble() >= (1 - player1.items[p1itemindex].HitChance)) { HP2 = HP2 - player1.Strength * (1 + player1.items[p1itemindex].DPS / player2.Protection); }
                if (random.NextDouble() >= (1 - player2.items[p2itemindex].HitChance)) { HP1 = HP1 - player2.Strength * (1 + player2.items[p1itemindex].DPS / player1.Protection); }
            }
            else
            {
                //Determine likelihood of each hit.
                if (random.NextDouble() >= (1 - player2.items[p2itemindex].HitChance)) { HP1 = HP1 - player2.Strength * (1 + player2.items[p1itemindex].DPS / player1.Protection); }
                if (random.NextDouble() >= (1 - player1.items[p1itemindex].HitChance)) { HP2 = HP2 - player1.Strength * (1 + player1.items[p1itemindex].DPS / player2.Protection); }
            }
            
            //Console.WriteLine("          HP 1: " + HP1);
            //Console.WriteLine("          HP 2: " + HP2);

            return Tuple.Create(HP1, HP2);
        }
    }
}
