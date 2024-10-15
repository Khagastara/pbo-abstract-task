using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace abstractClassAndInterface
{
    interface IKemampuan
    {
        string Nama { get; }
        int Cooldown { get; }
        bool SelfTargeting { get; }
        bool AoETarget { get; }
        void Gunakan(Robot wielder, Robot target);
        void Gunakan(Robot wielder, List<Robot> enemies);
        int getCooldownCounter();
        void UpdateCooldown();
    }

    abstract class Robot
    {
        public string Nama;
        public int Energi;
        public int Armor;
        public int Serangan;
        public bool Stunned;
        public BuffStatus Buffs;
        private List<IKemampuan> _kemampuanList;

        public Robot(string nama, int energi, int armor, int serangan)
        {
            Nama = nama;
            Energi = energi;
            Armor = armor;
            Serangan = serangan;
            Stunned = false;
            Buffs = new BuffStatus();
        }

        public virtual void Serang(Robot target)
        {
            if (Stunned)
            {
                Console.WriteLine($"{Nama} is stunned.");
                Stunned = false;
                return;
            }

            int damage = Serangan - target.Armor;
            if (damage < 0) damage = 0;

            target.UpdateEnergi(-damage);
            Console.WriteLine($"{Nama} attacks {target.Nama} with {damage} damage.");
        }

        public void UpdateEnergi(int damage)
        {
            Energi += damage;

            if (Energi < 0)
            {
                Energi = 0;
            }
        }

        public abstract void GunakanKemampuan(IKemampuan kemampuan, Robot wielder, Robot target);

        public void GunakanKemampuan(IKemampuan kemampuan, Robot wielder, List<Robot> enemies)
        {
            if (kemampuan.AoETarget)
            {
                kemampuan.Gunakan(wielder, enemies);
            }
            else
            {
                Console.WriteLine($"{kemampuan.Nama} is not an AoE ability.");
            }
        }

        public void CetakInformasi()
        {
            Console.WriteLine($"Nama: {Nama}");
            Console.WriteLine($"Energi: {Energi}");
            Console.WriteLine($"Armor: {Armor}");
            Console.WriteLine($"Serangan: {Serangan}");
            Console.WriteLine($"Stunned: {Stunned}");
            Console.WriteLine($"SuperShield Turns: {Buffs.SuperShieldTurns}");
        }
        public virtual List<IKemampuan> GetKemampuanList()
        {
            return _kemampuanList;
        }
    }

    class BossRobot : Robot
    {
        public int Pertahanan;
        private List<IKemampuan> _kemampuanList;
        public BossRobot(string nama, int energi, int armor, int serangan, int pertahanan)
            : base(nama, energi, armor, serangan)
        {
            Pertahanan = pertahanan;

            _kemampuanList = new List<IKemampuan>
            {
                new Perbaikan(),
                new SeranganListrik(),
                new SeranganPlasma(),
                new PertahananSuper()
            };
        }

        public override void Serang(Robot target)
        {
            if (Stunned)
            {
                Console.WriteLine($"{Nama} is stunned");
                Stunned = false;
                return;
            }

            int damage = Serangan - (target.Armor + Pertahanan);
            if (damage < 0) damage = 0;

            target.UpdateEnergi(-damage);
            Console.WriteLine($"{Nama} attacks {target.Nama} with {damage} damage");
        }

        public override void GunakanKemampuan(IKemampuan kemampuan, Robot wielder, Robot target)
        {
            kemampuan.Gunakan(wielder, target);
        }

        public override List<IKemampuan> GetKemampuanList()
        {
            return _kemampuanList;
        }
    }

    class BuffStatus
    {
        public int SuperShieldTurns;
        public int Burn;

        public BuffStatus()
        {
            SuperShieldTurns = 0;
            Burn = 0;
        }
    }

    class BuffManagement
    {
        public static void UpdateBuffs(Robot robot)
        {
            if (robot.Buffs.SuperShieldTurns > 0)
            {
                robot.Buffs.SuperShieldTurns--;
                if (robot.Buffs.SuperShieldTurns == 0)
                {
                    robot.Armor -= 20;
                    Console.WriteLine($"{robot.Nama}'s Super Shield Has Worn off. Armor reduced.");
                }
            }
            if (robot.Buffs.Burn > 0)
            {
                robot.Buffs.Burn--;

                int burnDamage = 20;
                robot.UpdateEnergi(-burnDamage);
                Console.WriteLine($"{robot.Nama} takes {burnDamage} damage from burn");
            }
        }
        public static void DisplayBuffs(Robot robot)
        {
            if (robot.Buffs.SuperShieldTurns > 0)
            {
                Console.WriteLine($"   Super Shield    {robot.Buffs.SuperShieldTurns} Turns.");
            }
            if (robot.Buffs.Burn > 0)
            {
                Console.WriteLine($"   Burned   {robot.Buffs.Burn} Turns");
            }
        }
    }

    class Perbaikan : IKemampuan
    {
        public string Nama { get; set; } = "Repair";
        public int Cooldown { get; set; } = 3;
        private int cooldownCounter = 0;
        public bool SelfTargeting { get; set; } = true;
        public bool AoETarget { get; set; } = false;

        public void Gunakan(Robot wielder, Robot target)
        {
            if (cooldownCounter > 0)
            {
                Console.WriteLine($"{Nama} is on Cooldown");
                return;
            }

            int healEnergy = 50;
            wielder.UpdateEnergi(healEnergy);
            Console.WriteLine($"{wielder.Nama} uses Repair and heals {healEnergy} Energy.");
            cooldownCounter = Cooldown;
        }

        public int getCooldownCounter()
        {
            return cooldownCounter;
        }
        public void UpdateCooldown()
        {
            if (cooldownCounter > 0)
            {
                cooldownCounter--;
            }
        }
        public void Gunakan(Robot wielder, List<Robot> enemies) { }
    }

    class SeranganListrik : IKemampuan
    {
        public string Nama { get; set; } = "Electric Shock";
        public int Cooldown { get; set; } = 3;
        private int cooldownCounter = 0;
        public bool SelfTargeting { get; set; } = false;
        public bool AoETarget { get; set; } = false;

        public void Gunakan(Robot wielder, Robot target)
        {
            if (cooldownCounter > 0)
            {
                Console.WriteLine($"{Nama} is on Cooldown");
                return;
            }

            int damage = 35;
            int totalDamage = damage - target.Armor;

            if (totalDamage < 0)
            {
                totalDamage = 0;
            }

            target.UpdateEnergi(-totalDamage);
            target.Stunned = true;
            Console.WriteLine($"{wielder.Nama} uses Electric Shock on {target.Nama} for {totalDamage} damage!");
            Console.WriteLine($"{target.Nama} is stunned");

            cooldownCounter += Cooldown;
        }
        public int getCooldownCounter()
        {
            return cooldownCounter;
        }

        public void UpdateCooldown()
        {
            if (cooldownCounter > 0)
            {
                cooldownCounter--;
            }
        }
        public void Gunakan(Robot wielder, List<Robot> enemies) { }
    }

    class SeranganPlasma : IKemampuan
    {
        public string Nama { get; set; } = "Plasma Cannon";
        public int Cooldown { get; set; } = 5;
        private int cooldownCounter = 0;
        public bool SelfTargeting { get; set; } = false;
        public bool AoETarget { get; set; } = false;

        public void Gunakan(Robot wielder, Robot target)
        {
            if (cooldownCounter > 0)
            {
                Console.WriteLine($"{Nama} is on Cooldown");
                return;
            }

            int damage = 50;
            int totalDamage = damage;
            if (totalDamage < 0)
            {
                totalDamage = 0;
            }
            target.UpdateEnergi(-totalDamage);
            Console.WriteLine($"{wielder.Nama} uses Plasma Cannon on {target.Nama} for {damage} damage, ignoring armor.");
            cooldownCounter += Cooldown;
        }
        public int getCooldownCounter()
        {
            return cooldownCounter;
        }

        public void UpdateCooldown()
        {
            if (cooldownCounter > 0)
            {
                cooldownCounter--;
            }
        }
        public void Gunakan(Robot wielder, List<Robot> enemies) { }
    }

    class PertahananSuper : IKemampuan
    {
        public string Nama { get; set; } = "Super Shield";
        public int Cooldown { get; set; } = 5;
        private int cooldownCounter = 0;
        public bool SelfTargeting { get; set; } = true;
        public bool AoETarget { get; set; } = false;

        public void Gunakan(Robot wielder, Robot target)
        {
            if (cooldownCounter > 0)
            {
                Console.WriteLine($"{Nama} is on Cooldown");
                return;
            }

            wielder.Armor += 20;
            wielder.Buffs.SuperShieldTurns = 3;
            Console.WriteLine($"{wielder.Nama} uses Super Shield! Armor increased temporarily.");
            cooldownCounter += Cooldown;
        }
        public int getCooldownCounter()
        {
            return cooldownCounter;
        }

        public void UpdateCooldown()
        {
            if (cooldownCounter > 0)
            {
                cooldownCounter--;
            }
        }
        public void Gunakan(Robot wielder, List<Robot> enemies) { }
    }

    class NormalRobot : Robot
    {
        private List<IKemampuan> _kemampuanList;

        public NormalRobot(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
        {
            _kemampuanList = new List<IKemampuan>
            {
                new Perbaikan(),
                new SeranganListrik(),
                new SeranganPlasma(),
                new PertahananSuper()
            };
        }

        public override void Serang(Robot target)
        {
            base.Serang(target);
        }

        public override void GunakanKemampuan(IKemampuan kemampuan, Robot wielder, Robot target)
        {
            if (_kemampuanList.Contains(kemampuan))
            {
                kemampuan.Gunakan(this, target);
            }
            else
            {
                Console.WriteLine($"{wielder.Nama} cannot use {kemampuan.Nama}.");
            }
        }
        public bool Mati()
        {
            return Energi <= 0;
        }

        public override List<IKemampuan> GetKemampuanList()
        {
            return _kemampuanList;
        }
    }

    class SchlachtschiffKanone : IKemampuan
    {
        public string Nama { get; set; } = "Schachtschiff-Kanone";
        public int Cooldown { get; set; } = 3;
        private int cooldownCounter = 0;
        public bool SelfTargeting { get; set; } = false;
        public bool AoETarget { get; set; } = false;

        public void Gunakan(Robot wielder, Robot target)
        {
            if (cooldownCounter > 0)
            {
                Console.WriteLine($"{Nama} is on Cooldown");
                return;
            }

            int damage = 35;
            int totalDamage = damage;
            if (totalDamage < 0)
            {
                totalDamage = 0;
            }

            target.UpdateEnergi(-totalDamage);
            Console.WriteLine($"{wielder.Nama} uses Battleship Cannon on {target.Nama} with {damage} Damage It's Piercing Its Armor.");
            cooldownCounter += Cooldown;
        }
        public int getCooldownCounter()
        {
            return cooldownCounter;
        }

        public void UpdateCooldown()
        {
            if (cooldownCounter > 0)
            {
                cooldownCounter--;
            }
        }
        public void Gunakan(Robot wielder, List<Robot> enemies) { }
    }

    class RobotPanzer : Robot
    {
        private List<IKemampuan> _kemampuanList;

        public RobotPanzer(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
        {
            _kemampuanList = new List<IKemampuan>
            {
                new Perbaikan(),
                new SchlachtschiffKanone(),
                new PertahananSuper()
            };
        }

        public override void Serang(Robot target)
        {
            base.Serang(target);
        }

        public override void GunakanKemampuan(IKemampuan kemampuan, Robot wielder, Robot target)
        {
            if (_kemampuanList.Contains(kemampuan))
            {
                kemampuan.Gunakan(this, target);
            }
            else
            {
                Console.WriteLine($"{wielder.Nama} cannot use {kemampuan.Nama}.");
            }
        }

        public bool Mati()
        {
            return Energi <= 0;
        }

        public override List<IKemampuan> GetKemampuanList()
        {
            return _kemampuanList;
        }
    }

    class FlammenWerfer : IKemampuan
    {
        public string Nama { get; set; } = "Flamethrower";
        public int Cooldown { get; set; } = 3;
        private int cooldownCounter = 0;
        public bool SelfTargeting { get; set; } = false;
        public bool AoETarget { get; set; } = true;
        public void Gunakan(Robot wielder, Robot target)
        {

        }

        public void Gunakan(Robot wielder, List<Robot> enemies)
        {
            if (cooldownCounter > 0)
            {
                Console.WriteLine($"{Nama} is on Cooldown");
                return;
            }

            int damage = 35;
            int totalDamage = damage;

            foreach (var enemy in enemies)
            {
                if (totalDamage < 0)
                {
                    totalDamage = 0;
                }

                enemy.UpdateEnergi(-totalDamage);
                enemy.Buffs.Burn = 3;

                Console.WriteLine($"{wielder.Nama} uses Flamethrower on {enemy.Nama} for {totalDamage} damage.");
            }

            cooldownCounter += Cooldown;
        }
        public int getCooldownCounter()
        {
            return cooldownCounter;
        }

        public void UpdateCooldown()
        {
            if (cooldownCounter > 0)
            {
                cooldownCounter--;
            }
        }
    }

    class SelfDestructed : IKemampuan
    {
        public string Nama { get; set; } = "Self Destructed";
        public int Cooldown { get; set; } = 99;
        private int cooldownCounter = 0;
        public bool SelfTargeting { get; set; } = false;
        public bool AoETarget { get; set; } = true;
        public void Gunakan(Robot wielder, Robot target) { }

        public void Gunakan(Robot wielder, List<Robot> enemies)
        {
            if (cooldownCounter > 0)
            {
                Console.WriteLine($"{Nama} is on Cooldown");
                return;
            }

            int damage = 80;
            foreach (var enemy in enemies)
            {
                int totalDamage = damage - enemy.Armor;
                if (totalDamage < 0)
                {
                    totalDamage = 0;
                }
                enemy.UpdateEnergi(-totalDamage);
                Console.WriteLine($"{wielder.Nama} uses Self Destructed, dealing {damage} damage to all enemies.");
            }

            wielder.UpdateEnergi(-99999);
            Console.WriteLine($"{wielder.Nama} died.");
            cooldownCounter += Cooldown;
        }
        public int getCooldownCounter()
        {
            return cooldownCounter;
        }

        public void UpdateCooldown()
        {
            if (cooldownCounter > 0)
            {
                cooldownCounter--;
            }
        }

    }

    class OilDrumRobot : Robot
    {
        private List<IKemampuan> _kemampuanList;

        public OilDrumRobot(string nama, int energi, int armor, int serangan)
            : base(nama, energi, armor, serangan)
        {
            _kemampuanList = new List<IKemampuan>
            {
                new Perbaikan(),
                new FlammenWerfer(),
                new PertahananSuper(),
                new SelfDestructed()
            };
        }

        public void GunakanKemampuan(IKemampuan kemampuan, List<Robot> enemies)
        {
            if (kemampuan is SelfDestructed)
            {
                ((SelfDestructed)kemampuan).Gunakan(this, enemies);
            }
            else
            {
                Console.WriteLine($"{Nama} cannot use this ability on multiple targets.");
            }
        }

        public override void GunakanKemampuan(IKemampuan kemampuan, Robot wielder, Robot target)
        {
            if (_kemampuanList.Contains(kemampuan))
            {
                kemampuan.Gunakan(this, target);
            }
            else
            {
                Console.WriteLine($"{Nama} cannot use {kemampuan.Nama}.");

            }
        }


        public override List<IKemampuan> GetKemampuanList()
        {
            return _kemampuanList;
        }
    }

    class Program
    {
        static void UpdateAllCooldowns(List<Robot> robotList)
        {
            foreach (var robot in robotList)
            {
                foreach (var kemampuan in robot.GetKemampuanList())
                {
                    kemampuan.UpdateCooldown();
                }

                BuffManagement.UpdateBuffs(robot);
            }
        }

        static void UpdateAllCooldowns(List<BossRobot> bossRobotList)
        {
            foreach (var boss in bossRobotList)
            {
                foreach (var kemampuan in boss.GetKemampuanList())
                {
                    kemampuan.UpdateCooldown();
                }

                BuffManagement.UpdateBuffs(boss);
            }
        }

        static void DisplayStatus(List<Robot> robots, List<BossRobot> bossRobots, int turn)
        {
            Console.WriteLine("\nTeam Status:");
            for (int i = 0; i < robots.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {robots[i].Nama}");
                Console.WriteLine($"   Energi: {robots[i].Energi}");
            }
            Console.WriteLine("\nEnemy Status:");
            for (int i = 0; i < bossRobots.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {bossRobots[i].Nama}");
                Console.WriteLine($"   Energi: {bossRobots[i].Energi}");
            }
            Console.WriteLine($"\n--- Turn: {turn} ---");
        }
        static void Main(string[] args)
        {
            List<Robot> robots = new List<Robot>
        {
            new NormalRobot("Electric Robot", 100, 15, 30),
            new RobotPanzer("Tiger", 100, 20, 40),
            new OilDrumRobot("FM-0", 80, 10, 30)
        };
            List<BossRobot> bossRobots = new List<BossRobot>
        {
            new BossRobot("Boss Robot A", 300, 15, 40, 10),
            new BossRobot("Boss Robot B", 300, 20, 40, 10)
        };

            int turn = 1;

            while (bossRobots.Exists(boss => boss.Energi > 0) && robots.Exists(robot => robot.Energi > 0))
            {
                DisplayStatus(robots, bossRobots, turn);

                foreach (Robot robot in robots)
                {
                    bool validChoice = false;
                    while (!validChoice)
                    {
                        if (robot.Energi > 0)
                        {
                            Console.WriteLine($"\n{robot.Nama}'s turn:");
                            Console.WriteLine("Choose action:");
                            Console.WriteLine("1. Normal Attack");
                            Console.WriteLine("2. Use Ability");
                            Console.WriteLine("3. Display Buffs");

                            string choice = Console.ReadLine();

                            switch (choice)

                            {
                                case "1":
                                    bool validChoice1 = false;
                                    Console.Clear();
                                    DisplayStatus(robots, bossRobots, turn);
                                    while (!validChoice1)
                                    {
                                        Console.WriteLine("\nChoose to Attack: ");

                                        for (int i = 0; i < bossRobots.Count; i++)
                                        {
                                            Console.WriteLine($"{i + 1}. {bossRobots[i].Nama}");
                                        }
                                        Console.WriteLine($"{bossRobots.Count + 1}. Cancel");

                                        int selectedBoss;
                                        bool validBossInput = int.TryParse(Console.ReadLine(), out selectedBoss);

                                        if (!validBossInput || selectedBoss < 1 || selectedBoss > bossRobots.Count + 1)
                                        {
                                            Console.Clear();
                                            DisplayStatus(robots, bossRobots, turn);
                                            Console.WriteLine("Invalid Choice, Try Again");
                                        }
                                        else if (selectedBoss == bossRobots.Count + 1)
                                        {
                                            Console.Clear();
                                            DisplayStatus(robots, bossRobots, turn);
                                            validChoice1 = true;
                                        }
                                        else
                                        {
                                            Robot targetBoss = bossRobots[selectedBoss - 1];
                                            if (targetBoss.Energi <= 0)
                                            {
                                                Console.Clear();
                                                Console.WriteLine($"{targetBoss.Nama} is already defeated. Choose another target.");

                                                DisplayStatus(robots, bossRobots, turn);
                                            }
                                            else
                                            {
                                                Console.Clear();
                                                robot.Serang(targetBoss);

                                                DisplayStatus(robots, bossRobots, turn);
                                                validChoice1 = true;
                                                validChoice = true;
                                            }
                                        }
                                    }
                                    break;

                                case "2":
                                    bool validChoice2 = false;
                                    Console.Clear();
                                    DisplayStatus(robots, bossRobots, turn);
                                    while (!validChoice2)
                                    {
                                        Console.WriteLine("\nChoose Ability:");

                                        var kemampuanList = robot.GetKemampuanList();

                                        for (int i = 0; i < kemampuanList.Count; i++)
                                        {
                                            int remainingCooldown = kemampuanList[i].getCooldownCounter();
                                            string cooldownText = remainingCooldown > 0
                                                ? $"{remainingCooldown} Turns Remaining"
                                                : "Ready";
                                            Console.WriteLine($"{i + 1}. {kemampuanList[i].Nama} ({cooldownText})");
                                        }
                                        Console.WriteLine($"{kemampuanList.Count + 1}. Cancel");

                                        int selectedAbility;
                                        bool validAbilityInput = int.TryParse(Console.ReadLine(), out selectedAbility);

                                        if (!validAbilityInput || selectedAbility < 1 || selectedAbility > kemampuanList.Count + 1)
                                        {
                                            Console.Clear();
                                            DisplayStatus(robots, bossRobots, turn);
                                            Console.WriteLine("Invalid Choice, Try Again");
                                        }
                                        else if (selectedAbility == kemampuanList.Count + 1)
                                        {
                                            Console.Clear();
                                            DisplayStatus(robots, bossRobots, turn);
                                            validChoice2 = true;
                                        }
                                        else
                                        {
                                            IKemampuan selectedKemampuan = kemampuanList[selectedAbility - 1];

                                            if (selectedKemampuan.SelfTargeting)
                                            {
                                                bool confirmed = false;
                                                while (!confirmed)
                                                {
                                                    Console.WriteLine($"Use {selectedKemampuan.Nama} on {robot.Nama}?");
                                                    Console.WriteLine("1. Confirm          2. Cancel");

                                                    string confirmation = Console.ReadLine();

                                                    if (confirmation == "1")
                                                    {
                                                        Console.Clear();
                                                        selectedKemampuan.Gunakan(robot, robot);
                                                        Console.WriteLine($"{robot.Nama} Uses {selectedKemampuan.Nama}.");
                                                        DisplayStatus(robots, bossRobots, turn);
                                                        confirmed = true;
                                                        validChoice2 = true;
                                                        validChoice = true;
                                                    }
                                                    else if (confirmation == "2")
                                                    {
                                                        Console.Clear();
                                                        DisplayStatus(robots, bossRobots, turn);
                                                        confirmed = true;
                                                    }
                                                    else
                                                    {
                                                        Console.Clear();
                                                        DisplayStatus(robots, bossRobots, turn);
                                                        Console.WriteLine("Invalid Choice, Try Again");
                                                    }
                                                }
                                            }
                                            else if (selectedKemampuan.AoETarget)
                                            {
                                                bool confirmed1 = false;
                                                while (!confirmed1)
                                                {
                                                    Console.WriteLine($"Use {selectedKemampuan.Nama} on The Enemies?");
                                                    Console.WriteLine("1. Confirm          2. Cancel");

                                                    string confirmation = Console.ReadLine();
                                                    if (confirmation == "1")
                                                    {
                                                        Console.Clear();

                                                        List<Robot> enemies = bossRobots.Cast<Robot>().ToList();
                                                        selectedKemampuan.Gunakan(robot, enemies);
                                                        DisplayStatus(robots, bossRobots, turn);

                                                        confirmed1 = true;
                                                        validChoice2 = true;
                                                        validChoice = true;
                                                    }
                                                    else if (confirmation == "2")
                                                    {
                                                        Console.Clear();
                                                        DisplayStatus(robots, bossRobots, turn);
                                                        confirmed1 = true;
                                                    }
                                                    else
                                                    {
                                                        Console.Clear();
                                                        DisplayStatus(robots, bossRobots, turn);
                                                        Console.WriteLine("Invalid Choice, Try Again");
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                bool validTarget = false;
                                                Console.Clear();
                                                DisplayStatus(robots, bossRobots, turn);
                                                while (!validTarget)
                                                {
                                                    Console.WriteLine("\nChoose to Attack: ");

                                                    for (int i = 0; i < bossRobots.Count; i++)
                                                    {
                                                        Console.WriteLine($"{i + 1}. {bossRobots[i].Nama}");
                                                    }
                                                    Console.WriteLine($"{bossRobots.Count + 1}. Cancel");

                                                    int selectedBoss;
                                                    bool validBossInput = int.TryParse(Console.ReadLine(), out selectedBoss);

                                                    if (!validBossInput || selectedBoss < 1 || selectedBoss > bossRobots.Count + 1)
                                                    {
                                                        Console.Clear();
                                                        DisplayStatus(robots, bossRobots, turn);
                                                        Console.WriteLine("Invalid Choice, Try Again");
                                                    }
                                                    else if (selectedBoss == bossRobots.Count + 1)
                                                    {
                                                        Console.Clear();
                                                        DisplayStatus(robots, bossRobots, turn);
                                                        validTarget = true;
                                                    }
                                                    else
                                                    {
                                                        Robot targetBoss = bossRobots[selectedBoss - 1];

                                                        if (targetBoss.Energi <= 0)
                                                        {
                                                            Console.Clear();
                                                            DisplayStatus(robots, bossRobots, turn);
                                                            Console.WriteLine($"{targetBoss.Nama} is already defeated. Choose another target.");
                                                        }
                                                        else
                                                        {
                                                            if (selectedKemampuan.getCooldownCounter() > 0)
                                                            {
                                                                Console.Clear();
                                                                Console.WriteLine($"{selectedKemampuan.Nama} is on Cooldown");
                                                                DisplayStatus(robots, bossRobots, turn);
                                                                validTarget = true;
                                                            }
                                                            else
                                                            {
                                                                Console.Clear();
                                                                robot.GunakanKemampuan(selectedKemampuan, robot, targetBoss);
                                                                DisplayStatus(robots, bossRobots, turn);
                                                                validTarget = true;
                                                                validChoice2 = true;
                                                                validChoice = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case "3":
                                    bool validChoice3 = false;
                                    while (!validChoice3)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\nTeam Buff Status:");
                                        for (int i = 0; i < robots.Count; i++)
                                        {
                                            Console.WriteLine($"{i + 1}. {robots[i].Nama}");
                                            BuffManagement.DisplayBuffs(robots[i]);
                                        }

                                        Console.WriteLine("\nEnemy Buff Status:");
                                        for (int i = 0; i < bossRobots.Count; i++)
                                        {
                                            Console.WriteLine($"{i + 1}. {bossRobots[i].Nama}");
                                            BuffManagement.DisplayBuffs(bossRobots[i]);
                                        }

                                        Console.WriteLine("\nPress any key to continue...");
                                        Console.ReadKey();
                                        validChoice3 = true;
                                    }
                                    Console.Clear();
                                    DisplayStatus(robots, bossRobots, turn);
                                    break;

                                default:
                                    Console.Clear();
                                    DisplayStatus(robots, bossRobots, turn);
                                    Console.WriteLine("Invalid Choice, Try Again!");
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{robot.Nama} is defeated. Skipping turn.");
                            validChoice = true;
                        }
                    }
                }

                Console.WriteLine("\nEnemies Turn to Attack");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();

                Random random = new Random();

                foreach (BossRobot boss in bossRobots)
                {
                    if (boss.Energi > 0)
                    {
                        List<string> bossAction = new List<string> { "Attack", "Ability" };

                        string action = bossAction[random.Next(bossAction.Count)];

                        switch (action)
                        {
                            case "Attack":
                                List<Robot> aliveRobots = robots.Where(robot => robot.Energi > 0).ToList();
                                if (aliveRobots.Count > 0)
                                {
                                    Robot target = aliveRobots[random.Next(aliveRobots.Count)];
                                    boss.Serang(target);
                                }
                                break;

                            case "Ability":
                                List<IKemampuan> availableAbilities = boss.GetKemampuanList().Where(kemampuan => kemampuan.getCooldownCounter() == 0).ToList();
                                if (availableAbilities.Count > 0)
                                {
                                    IKemampuan selectedAbility = availableAbilities[random.Next(availableAbilities.Count)];

                                    if (selectedAbility.SelfTargeting)
                                    {
                                        boss.GunakanKemampuan(selectedAbility, boss, boss);
                                    }
                                    else if (selectedAbility.AoETarget)
                                    {
                                        foreach (Robot target in robots.Where(r => r.Energi > 0))
                                        {
                                            boss.GunakanKemampuan(selectedAbility, boss, target);
                                        }
                                    }
                                    else
                                    {
                                        List<Robot> targets = robots.Where(robot => robot.Energi > 0).ToList();
                                        if (targets.Count > 0)
                                        {
                                            Robot target = targets[random.Next(targets.Count)];
                                            boss.GunakanKemampuan(selectedAbility, boss, target);
                                        }
                                        else
                                        {
                                            Console.WriteLine("No available robots to use ability on.");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"{boss.Nama} has no abilities available to use.");
                                }
                                break;

                            default:
                                Console.WriteLine("Boss did not take any action.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{boss.Nama} is defeated. Skipping turn.");
                    }
                }
                turn++;
                UpdateAllCooldowns(robots);
                UpdateAllCooldowns(bossRobots);
            }

            if (!robots.Exists(robot => robot.Energi > 0))
            {
                Console.WriteLine("You Lose! All Your Robots Have Been Defeated.");
            }
            else if (!bossRobots.Exists(boss => boss.Energi > 0))
            {
                Console.WriteLine("You Win! All Bosses Have Been Defeated.");
            }
        }
    }
}