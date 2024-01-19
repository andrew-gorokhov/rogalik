namespace rogalik
{
    using System;
    using System.Collections.Generic;

    class Program
    {
        static void Main()
        {
            Game game = new Game();
            game.Start();
        }
    }

    class Player
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Points { get; set; }
        public Aid Medkit { get; set; }
        public Weapon Weapon { get; set; }

        public Player(string name, int maxHealth)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = MaxHealth;
            Points = 0;
            Medkit = new Aid("Аптечка", 20);
            Weapon = new Sword("Меч", 15, 80);
        }

        public void Heal()
        {
            Health += Medkit.HealingPower;
            if (Health > MaxHealth)
                Health = MaxHealth;
            Console.WriteLine($"{Name} исцелился. Здоровье: {Health}/{MaxHealth}");
        }

        public int TakeDamage(int damage)
        {
            // Добавим шанс уклонения в 20%
            Random random = new Random();
            bool dodge = random.Next(1, 101) <= 20;

            if (!dodge)
            {
                Health -= damage;
                if (Health < 0)
                    Health = 0;
                //Console.WriteLine($"{Name} получил урон {damage}. Здоровье: {Health}/{MaxHealth}");
                return damage;
            }
            else
            {
               // Console.WriteLine($"{Name} уклонился от атаки!");
                return 0;
            }
        }

        public bool Attack(Enemy enemy)
        {
            int damage = enemy.TakeDamage(Weapon.Damage);
            if (damage > 0)
            {
                Console.WriteLine($"{Name} атаковал {enemy.Name} с помощью {Weapon.Name}. {enemy.Name} получил урон {damage}.");
            }
            else
            {
                Console.WriteLine($"{enemy.Name} уклонился от атаки {Name}!");
            }

            return damage > 0;
        }

        public void ReplaceWeapon(Weapon newWeapon)
        {
            Console.WriteLine($"{Name} заменил своё оружие на {newWeapon.Name}.");
            Weapon = newWeapon;
        }

        public bool AskToReplaceWeapon(Weapon enemyWeapon)
        {
            Console.WriteLine($"Вы победили противника и увидели его оружие: {enemyWeapon.Name}.");
            Console.WriteLine("Хотите заменить своё оружие на это?");
            Console.WriteLine("1. Да");
            Console.WriteLine("2. Нет");

            string choice = Console.ReadLine();
            return choice == "1";
        }
    }

    class Sword : Weapon
    {
        public Sword(string name, int damage, int durability) : base(name, damage, durability)
        {
        }
    }

    class Bow : Weapon
    {
        public Bow(string name, int damage, int durability) : base(name, damage, durability)
        {
        }
    }

    class Enemy
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public Weapon Weapon { get; set; }

        public Enemy(string name, int maxHealth)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = MaxHealth;
            Weapon = new Weapon("Кинжал", 5, 50);
        }

        public int TakeDamage(int damage)
        {
            // Добавим шанс уклонения в 20%
            Random random = new Random();
            bool dodge = random.Next(1, 101) <= 20;

            if (!dodge)
            {
                Health -= damage;
                if (Health < 0)
                    Health = 0;
               Console.WriteLine($"{Name} получил урон {damage}. Здоровье: {Health}/{MaxHealth}");
            }
            else
            {
               // Console.WriteLine($"{Name} уклонился от атаки!");
            }

            return dodge ? 0 : damage;
        }

        public void Attack(Player player)
        {
            int damage = Weapon.Damage;
            int hitDamage = player.TakeDamage(damage);
            if (hitDamage > 0)
            {
                Console.WriteLine($"{Name} атаковал {player.Name}! {player.Name} получил урон {hitDamage}. Здоровье: {player.Health}/{player.MaxHealth}");
            }
            else
            {
                Console.WriteLine($"{player.Name} уклонился от атаки {Name}!");
            }
        }

        public Weapon DropWeapon()
        {
            Console.WriteLine($"{Name} уронил своё оружие.");
            return Weapon;
        }
    }

    class Aid
    {
        public string Name { get; set; }
        public int HealingPower { get; set; }

        public Aid(string name, int healingPower)
        {
            Name = name;
            HealingPower = healingPower;
        }
    }

    class Weapon
    {
        public string Name { get; set; }
        public int Damage { get; set; }
        public int Durability { get; set; }

        public Weapon(string name, int damage, int durability)
        {
            Name = name;
            Damage = damage;
            Durability = durability;
        }

        public void ReduceDurability()
        {
            Durability--;
            if (Durability < 0)
                Durability = 0;
            Console.WriteLine($"Прочность {Name}: {Durability}");
        }
    }

    class Game
    {
        private Player player;

        public void Start()
        {
            Console.WriteLine("Введите свое имя:");
            string playerName = Console.ReadLine();

            player = new Player(playerName, 100);

            while (player.Health > 0)
            {
                Enemy enemy = GenerateRandomEnemy();
                Console.WriteLine($"Вы встретили врага: {enemy.Name}. Здоровье: {enemy.Health}/{enemy.MaxHealth}");

                while (enemy.Health > 0)
                {
                    Console.WriteLine("Выберите действие:");
                    Console.WriteLine("1. Атаковать");
                    Console.WriteLine("2. Исцелиться");

                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            if (player.Attack(enemy))
                            {
                                enemy.Attack(player);
                            }
                            break;

                        case "2":
                            player.Heal();
                            break;

                        default:
                            Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                            break;
                    }

                    if (enemy.Health <= 0)
                    {
                        Console.WriteLine($"Вы победили {enemy.Name}! Вы получили 10 очков.");
                        player.Points += 10;

                        Weapon enemyWeapon = enemy.DropWeapon();
                        if (player.AskToReplaceWeapon(enemyWeapon))
                        {
                            player.ReplaceWeapon(enemyWeapon);
                        }
                    }

                    if (player.Health <= 0)
                    {
                        Console.WriteLine("Игра окончена. Вы побеждены.");
                        break;
                    }
                }
            }

            Console.WriteLine($"Ваш итоговый счет: {player.Points}");
        }

        

        private Enemy GenerateRandomEnemy()
        {
            Random random = new Random();
            string[] enemyNames = { "Гоблин", "Орк", "Тролль", "Скелет", "Бес", "Зомби", "Кобольд" };
            string randomEnemyName = enemyNames[random.Next(enemyNames.Length)];
            int randomEnemyHealth = random.Next(50, 100);

            // Добавим случайный выбор оружия для врага
            Weapon randomEnemyWeapon;
            int weaponType = random.Next(1, 5);
            switch (weaponType)
            {
                case 1:
                    randomEnemyWeapon = new Weapon("Кинжал", 5, 50);
                    break;

                case 2:
                    randomEnemyWeapon = new Sword("Меч", 15, 80);
                    break;

                case 3:
                    randomEnemyWeapon = new Bow("Лук", 12, 60);
                    break;
                case 4:
                    randomEnemyWeapon = new Sword("Катана", 10, 60);
                    break;

                default:
                    randomEnemyWeapon = new Weapon("Кинжал", 5, 50);
                    break;
            }

            Enemy enemy = new Enemy(randomEnemyName, randomEnemyHealth);
            enemy.Weapon = randomEnemyWeapon;

            return enemy;
        }
    }

}