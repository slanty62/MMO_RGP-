using System;
using System.Collections.Generic;
using System.Linq;

namespace RoguelikeGame
{
    // Базовый класс для всех персонажей
    public abstract class Character
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        public bool IsAlive => Health > 0;

        public Character(string name, int maxHealth)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }

        public virtual void Heal(int amount)
        {
            Health += amount;
            if (Health > MaxHealth) Health = MaxHealth;
        }
    }

    // Класс оружия
    public class Weapon
    {
        public string Name { get; private set; }
        public int MinDamage { get; private set; }
        public int MaxDamage { get; private set; }
        public string Description { get; private set; }

        public Weapon(string name, int minDamage, int maxDamage, string description = "")
        {
            Name = name;
            MinDamage = minDamage;
            MaxDamage = maxDamage;
            Description = description;
        }

        public int GetDamage()
        {
            Random random = new Random();
            return random.Next(MinDamage, MaxDamage + 1);
        }

        public override string ToString()
        {
            return $"{Name} ({MinDamage}-{MaxDamage} урона)";
        }
    }

    // Класс игрока
    public class Player : Character
    {
        public int Score { get; private set; }
        public Weapon CurrentWeapon { get; private set; }
        public int EnemiesDefeated { get; private set; }

        public Player(string name) : base(name, 100)
        {
            Score = 0;
            EnemiesDefeated = 0;
        }

        public void SetWeapon(Weapon weapon)
        {
            CurrentWeapon = weapon;
        }

        public int Attack()
        {
            if (CurrentWeapon == null)
                return 5; // Базовый урон без оружия

            return CurrentWeapon.GetDamage();
        }

        public void AddScore(int points)
        {
            Score += points;
            EnemiesDefeated++;
        }

        public void ShowStatus()
        {
            Console.WriteLine($"=== {Name} ===");
            Console.WriteLine($"Здоровье: {Health}/{MaxHealth}");
            Console.WriteLine($"Оружие: {(CurrentWeapon != null ? CurrentWeapon.ToString() : "Нет оружия")}");
            Console.WriteLine($"Очки: {Score}");
            Console.WriteLine($"Побеждено врагов: {EnemiesDefeated}");
            Console.WriteLine();
        }
    }

    // Класс врага
    public class Enemy : Character
    {
        public Weapon Weapon { get; private set; }
        public int ExperienceValue { get; private set; }

        public Enemy(string name, int maxHealth, Weapon weapon, int expValue)
            : base(name, maxHealth)
        {
            Weapon = weapon;
            ExperienceValue = expValue;
        }

        public int Attack()
        {
            return Weapon?.GetDamage() ?? 5;
        }

        public override string ToString()
        {
            return $"{Name} (Здоровье: {Health}/{MaxHealth}, Оружие: {Weapon.Name})";
        }
    }

    // Рофл враги
    public static class EnemyFactory
    {
        private static readonly List<(string name, int health, string weaponName, int minDmg, int maxDmg, int exp)>
            EnemyTemplates = new List<(string, int, string, int, int, int)>
        {
            ("Голодный таксист", 30, "Счетчик ярости", 5, 10, 10),
            ("Офисный планктон", 25, "Степлер возмездия", 8, 12, 15),
            ("Сонный бариста", 40, "Кипящее молоко", 10, 16, 20),
            ("Тролль из интернета", 20, "Ядовитый комментарий", 15, 25, 30),
            ("Бегущий за автобусом", 35, "Портфель хаоса", 12, 18, 25),
            ("Кот-захватчик дивана", 50, "Мурчание разрушения", 8, 15, 35),
            ("Сосед с перфоратором", 45, "Дрель бессонницы", 18, 28, 40),
            ("Продавец ненужного", 30, "Навязчивая реклама", 5, 20, 20),
            ("Гоблин-минималист", 25, "Пустота", 1, 50, 45),
            ("Утка с претензиями", 15, "Громкое кряканье", 10, 30, 50)
        };

        public static Enemy CreateRandomEnemy()
        {
            Random random = new Random();
            var template = EnemyTemplates[random.Next(EnemyTemplates.Count)];

            var weapon = new Weapon(template.weaponName, template.minDmg, template.maxDmg);
            return new Enemy(template.name, template.health, weapon, template.exp);
        }
    }

    // Рофл оружия
    public static class WeaponFactory
    {
        private static readonly List<Weapon> Weapons = new List<Weapon>
        {
            new Weapon("Банан самонаведения", 12, 22, "Спелый банан, который всегда попадает в цель"),
            new Weapon("Лук-порей", 8, 18, "Стреляет слезоточивыми стрелами"),
            new Weapon("Посох чихания", 15, 25, "Заставляет врагов чихать до потери сознания"),
            new Weapon("Сковорода предков", 20, 30, "Нагревается в самый неподходящий момент"),
            new Weapon("Носок смерти", 25, 35, "Пахнет так, что враги падают замертво"),
            new Weapon("Резиновый цыпленок", 10, 15, "Пищит при ударе, сбивая противника с толку"),
            new Weapon("Ведро правды", 18, 28, "Заставляет врагов признаться во всех грехах"),
            new Weapon("Лазерный пук", 30, 40, "Тихий, но смертоносный"),
            new Weapon("Тапок богов", 22, 32, "Летит с материнской скоростью"),
            new Weapon("Кот-метатель", 5, 50, "Непредсказуемый урон: то поцарапает, то уснет")
        };

        public static Weapon GetRandomWeapon()
        {
            Random random = new Random();
            return Weapons[random.Next(Weapons.Count)];
        }
    }

    // Основной класс игры
    public class Game
    {
        private Player player;
        private Random random;

        // Рофл фразы для атак
        private string[] playerAttackPhrases = {
            "Вы бьете врага с криком 'Это спарта!'",
            "Ваша атака сопровождается эпичным саундтреком",
            "Вы вспоминаниете совет тренера и бьете точнее",
            "Враг отвлекается на пролетающую птицу, и вы пользуетесь моментом",
            "Вы атакуете с мемной мощью 2010 года",
            "Удар такой силы, что у врага слетают штаны",
            "Вы бьете врага, цитируя Шекспира"
        };

        private string[] enemyAttackPhrases = {
            "Враг атакует, бормоча что-то о повышении квартплаты",
            "Атака сопровождается громким чихом",
            "Враг бьет вас, параллельно листая ленту соцсетей",
            "Атака такая же неожиданная, как опечатка в важном документе",
            "Враг атакует с криком 'За квартиру!'",
            "Удар наносится с утренним недовольством",
            "Атака пахнет вчерашним борщом"
        };

        public Game()
        {
            random = new Random();
        }

        public void Start()
        {
            Console.WriteLine("=== ДОБРО ПОЖАЛОВАТЬ В MMO_RPG: ВЕРСИЯ 'под снюсом' ===");
            Console.Write("Введите имя вашего персонажа: ");
            string playerName = Console.ReadLine();

            player = new Player(string.IsNullOrEmpty(playerName) ? "Неуклюжий герой" : playerName);

            // Смешное стартовое оружие
            player.SetWeapon(new Weapon("Веник бабушки", 3, 8, "Бьет больно, но больше пыли поднимает"));

            string[] startPhrases = {
                $"\nИтак, {player.Name}, ваш путь начинается с веника и надежды...",
                $"\n{player.Name}, они говорили, что вы не сможете. Докажите, что они были правы!",
                $"\nПриготовьтесь, {player.Name}! Впереди слава, очки и много смешных смертей.",
                $"\n{player.Name}, мир ждет своего героя. Ну или хотя бы кого-то с веником."
            };

            Console.WriteLine(startPhrases[random.Next(startPhrases.Length)]);
            Console.WriteLine();

            GameLoop();
        }

        private void GameLoop()
        {
            while (player.IsAlive)
            {
                player.ShowStatus();

                // Случайное событие
                int eventType = random.Next(1, 5); // Теперь 4 типа событий

                switch (eventType)
                {
                    case 1: // Бой с врагом
                        CombatEncounter();
                        break;
                    case 2: // Находка оружия
                        WeaponFindEncounter();
                        break;
                    case 3: // Лечение
                        HealingEncounter();
                        break;
                    case 4: // Специальное событие
                        SpecialEncounter();
                        break;
                }

                if (player.IsAlive)
                {
                    Console.WriteLine("Нажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            GameOver();
        }

        private void CombatEncounter()
        {
            Enemy enemy = EnemyFactory.CreateRandomEnemy();
            Console.WriteLine($"💀 Вы встретили {enemy.Name}!");
            Console.WriteLine($"{enemy}\n");

            while (enemy.IsAlive && player.IsAlive)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Атаковать");
                Console.WriteLine("2. Попытаться убежать");

                string input = Console.ReadLine();

                if (input == "1")
                {
                    // Атака игрока с рофл фразой
                    Console.WriteLine(playerAttackPhrases[random.Next(playerAttackPhrases.Length)]);
                    int playerDamage = player.Attack();
                    enemy.TakeDamage(playerDamage);
                    Console.WriteLine($"⚔️ Вы наносите {playerDamage} урона!");

                    if (!enemy.IsAlive)
                    {
                        Console.WriteLine($"🎉 Вы победили {enemy.Name}!");
                        player.AddScore(enemy.ExperienceValue);
                        Console.WriteLine($"➕ Получено очков: {enemy.ExperienceValue}");

                        // Рофл сообщение о победе
                        string[] victoryPhrases = {
                            "Враг плачет, вспоминая свою несложившуюся карьеру клоуна",
                            "Победа! Теперь можно и поспать",
                            "Враг обещает рассказать об этом в своих мемуарах",
                            "Вы победили! Наверное..."
                        };
                        Console.WriteLine(victoryPhrases[random.Next(victoryPhrases.Length)]);
                        break;
                    }

                    // Атака врага с рофл фразой
                    Console.WriteLine(enemyAttackPhrases[random.Next(enemyAttackPhrases.Length)]);
                    int enemyDamage = enemy.Attack();
                    player.TakeDamage(enemyDamage);
                    Console.WriteLine($"💥 {enemy.Name} наносит {enemyDamage} урона!");

                    if (!player.IsAlive)
                    {
                        string[] deathPhrases = {
                            "Вы падаете с мыслью 'а ведь я так и не допил тот кофе...'",
                            "Мир медленно уплывает куда-то вдаль. Очень медленно.",
                            "Главное - умереть с улыбкой! У вас получилось.",
                            "Вы проиграли, но зато выглядели стильно"
                        };
                        Console.WriteLine(deathPhrases[random.Next(deathPhrases.Length)]);
                        break;
                    }

                    Console.WriteLine($"\nВаше здоровье: {player.Health}/{player.MaxHealth}");
                    Console.WriteLine($"Здоровье {enemy.Name}: {enemy.Health}/{enemy.MaxHealth}\n");
                }
                else if (input == "2")
                {
                    if (random.Next(0, 2) == 0) // 50% шанс убежать
                    {
                        string[] escapePhrases = {
                            "🏃 Вы успешно сбежали, притворившись кустом!",
                            "🏃 Вы убегаете, оставляя за собой шлейф достоинства",
                            "🏃 Побег удался! Враг теперь ищет кого-то другого",
                            "🏃 Вы сбежали! На время..."
                        };
                        Console.WriteLine(escapePhrases[random.Next(escapePhrases.Length)]);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("❌ Вам не удалось сбежать!");
                        Console.WriteLine(enemyAttackPhrases[random.Next(enemyAttackPhrases.Length)]);
                        int enemyDamage = enemy.Attack();
                        player.TakeDamage(enemyDamage);
                        Console.WriteLine($"💥 {enemy.Name} атакует вас по пятой точкой и наносит {enemyDamage} урона!");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Неверный ввод! Попробуйте снова.");
                }
            }
        }

        private void WeaponFindEncounter()
        {
            Weapon newWeapon = WeaponFactory.GetRandomWeapon();
            Console.WriteLine($"🎁 Вы нашли новое оружие: {newWeapon}");
            if (!string.IsNullOrEmpty(newWeapon.Description))
            {
                Console.WriteLine($"📝 {newWeapon.Description}");
            }

            Console.WriteLine("Выберите действие:");
            Console.WriteLine($"1. Взять {newWeapon.Name}");
            Console.WriteLine($"2. Оставить текущее оружие ({player.CurrentWeapon})");

            string input = Console.ReadLine();
            if (input == "1")
            {
                player.SetWeapon(newWeapon);
                string[] equipPhrases = {
                    $"✅ Вы экипировали {newWeapon.Name}! Теперь вы почти опасны.",
                    $"✅ {newWeapon.Name} теперь ваш! Надеюсь, он умеет мыть посуду.",
                    $"✅ Новое оружие! Пахнет... интересно.",
                    $"✅ Вы взяли {newWeapon.Name}. Выглядит сомнительно, но попробуем."
                };
                Console.WriteLine(equipPhrases[random.Next(equipPhrases.Length)]);
            }
            else
            {
                string[] skipPhrases = {
                    "❌ Вы оставили оружие. Может, зря?",
                    "❌ Оружие осталось лежать. Оно выглядело обиженно.",
                    "❌ Вы прошли мимо. Наверное, у вас уже есть планы получше.",
                    "❌ Оружие не тронуто. Теперь им заинтересуются археологи."
                };
                Console.WriteLine(skipPhrases[random.Next(skipPhrases.Length)]);
            }
        }

        private void HealingEncounter()
        {
            string[] healingItems = {
                "бутерброд с колбасой из прошлого века",
                "банка соленых огурцов",
                "чашка кофе от начальника",
                "таблетка от жадности",
                "компот из детства",
                "пирожок с тайной начинкой",
                "энергетик сомнительного качества",
                "бабушкино варенье",
                "забытая шоколадка в кармане",
                "суп, который 'сам себя съел'"
            };

            int healAmount = random.Next(10, 30);
            string item = healingItems[random.Next(healingItems.Length)];

            Console.WriteLine($"💚 Вы нашли {item}! Восстановлено {healAmount} здоровья.");
            player.Heal(healAmount);
            Console.WriteLine($"Ваше здоровье: {player.Health}/{player.MaxHealth}");
        }

        private void SpecialEncounter()
        {
            string[] specialEvents = {
                "Вы встретили танцующего медведя. Он научил вас новым движениям (+5 к морали)",
                "На вас с дерева упал кокос. К счастью, он был пустой",
                "Вы нашли карту сокровищ... на обратной стороне реклама пиццерии",
                "Гном предложил вам сделку: ваша душа в обмен на скидку 10%",
                "Вы попали под дождь из лягушек. Одна из них оказалась принцессой (но вы ее съели)",
                "Бродячий музыкант спел вам песню. Она была настолько плоха, что враги вокруг разбежались",
                "Вы нашли забытый диплом. Теперь вы официально Герой",
                "Проезжающий автомобиль обрызгал вас грязью. Зато красиво!",
                "Вы обнаружили, что ваш меч заточен с одной стороны. Это прогресс!",
                "Незнакомец подарил вам носок. Говорит, это семейная реликвия"
            };

            string eventText = specialEvents[random.Next(specialEvents.Length)];
            Console.WriteLine($"🎭 {eventText}");

            // Случайный бонус
            if (random.Next(0, 2) == 0)
            {
                int bonus = random.Next(5, 20);
                player.Heal(bonus);
                Console.WriteLine($"💚 Получен бонус к здоровью! +{bonus} HP");
            }
        }

        private void GameOver()
        {
            Console.Clear();
            Console.WriteLine("=== GAME OVER ===");
            Console.WriteLine($"Игрок: {player.Name}");
            Console.WriteLine($"Финальный счет: {player.Score} очков");
            Console.WriteLine($"Побеждено врагов: {player.EnemiesDefeated}");

            string[] gameOverPhrases = {
                "Не расстраивайтесь! Даже великие герои иногда проигрывают... часто... очень часто.",
                "Зато вы хорошо выглядели в процессе!",
                "Это был славный бой! Ну, не очень славный, но был же!",
                "Главное - участие. И ваше участие закончилось.",
                "Не везет в игре - повезет в любви! Надеемся...",
                "Вы проиграли, но зато сколько анекдотов можно будет рассказать!"
            };

            Console.WriteLine($"\n{gameOverPhrases[random.Next(gameOverPhrases.Length)]}");
            Console.WriteLine("\nСпасибо за игру!");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }

    // Главный класс программы
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "MMO_RPG под снюсом";

            while (true)
            {
                Console.Clear();
                Game game = new Game();
                game.Start();

                Console.Clear();
                Console.WriteLine("Хотите сыграть еще раз? (y/n)");
                string playAgain = Console.ReadLine();

                if (playAgain?.ToLower() != "y")
                {
                    Console.WriteLine("Возвращайтесь, когда наскучат победы в других играх!");
                    break;
                }
            }
        }
    }
}