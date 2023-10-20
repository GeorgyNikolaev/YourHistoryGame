using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace YourHistory
{
    class Game
    {
        static void Main()
        {
            StartGame();

            //Игра
            void StartGame()
            {
                Console.WriteLine();
                // Создание игрока
                Player player = new Player();

                // Введение
                Console.WriteLine("Добро пожаловать новичок!");
                Console.ReadKey();

                Console.WriteLine("Давай создадим тебе профиль.\n");
                Console.ReadKey();
                
                // Создание профиля
                Console.Write("Какое вас зовут? ");

                player.CreatePlayer();
                player.name = Convert.ToString(Console.ReadLine());

                Console.WriteLine("Отличное имя!\n");
                Console.ReadKey();

                Console.WriteLine("Характеристики мы не выбираем, так что вот, что ты имеешь:");
                player.ShowCharacteristics();
                Console.WriteLine();
                Console.ReadKey();

                Console.WriteLine("Хмм...");
                Console.ReadKey();

                Identification(player);
                Console.WriteLine();
                Console.ReadKey();

                Console.WriteLine("Ладно, мы справились.");

                // Сюжет
                Move(player);
            }

            // Выбор, куда игрок хочет сходить
            void Move(Player player)
            {
                Console.WriteLine();
                while (true)
                {
                    Console.WriteLine("куда хочешь сходить?");
                    Console.WriteLine("Магазин || Дом || Бар || Банк || Coming sone");
                    string answer = Convert.ToString(Console.ReadLine());
                    switch (answer)
                    {
                        case "Магазин":
                            Shop(player);
                            continue;
                        case "Дом":
                            House(player);
                            continue;
                        case "Бар":
                            Bar(player);
                            continue;
                        case "Банк":
                            Bank(player);
                            continue;
                        case "Break": 
                            break;
                        default:
                            Console.Write("Не верно введённые символы,попробуй ещё раз");
                            continue;
                    }
                    break;
                }
                
            }

            // Бар
            void Bar(Player player)
            {
                for (int i = 0; i < 30; i++) Console.Write("-");
                Console.WriteLine("\nВнимание, {0}, в баре", player.name);
            }

            // Дом
            void House(Player player)
            {
                for (int i = 0; i < 30; i++) Console.Write("-");
                Console.WriteLine("\nВнимание, {0}, ты дома", player.name);
            }

            // Магазин
            void Shop(Player player)
            {
                int wallet = player.wallet;
                int total = 0;// Итоговая сумма корзины
                Random random = new Random();
                var basket = new List<Store> { };// Корзина игрока
                string answer = null;

                var assortments = new List<Store> // Составления ассортимента магазина
                {
                    new (){ Name = "Колбаса", Id = 0, Quantity = random.Next(1, 6), Price = 130},
                    new (){ Name = "Хлеб", Id = 1, Quantity = random.Next(5, 15), Price = 30},
                    new (){ Name = "Молоко", Id = 2, Quantity = random.Next(2, 8), Price = 60},
                    new (){ Name = "Сыр", Id = 3, Quantity = random.Next(0, 4), Price = 120},
                };

                // Приветствие
                Console.Clear();
                for (int i = 0; i < 30; i++) Console.Write("-");
                Console.WriteLine("\nВнимание, {0}, ты в магазине.\n", player.name);
                Console.WriteLine("Твой баланс: {0}руб.\n", wallet);
                Console.WriteLine("Сегодня в наличии:");

                // Вывод ассортимента магазина на экран
                ShowAssortments(assortments);

                // Инструкция по добавлению товара в корзину
                Console.WriteLine("\nДля того, чтобы добавить что-то в корзину, напиши '+', а если убрать '-'.");
                Console.WriteLine("Затем напиши Id и кол-во желаемого.");
                Console.WriteLine("Если захочешь остановиться, напиши 'Stop'.\n");

                // Набор продуктов в корзину
                EditBasket(ref basket,ref assortments);
                total = CalculationTotal(basket, assortments);
                Console.Clear();
                Console.WriteLine("Окей, давай я покажу, что ты взял.");

                //Вывод корзины на экран
                ShowBasket(basket, assortments, total);
                Console.ReadKey();

                // Проверка корзины и в случаи необходимости удаление товаров
                while (total > wallet)
                {
                    Console.Clear();
                    Console.WriteLine("У вас не хватает денежных средств попробуйте отредактировать корзину\n");
                    Console.WriteLine("Ассортимент на данный момент:");
                    ShowAssortments(assortments);

                    // Инструкция по удалению товара из корзины
                    Console.WriteLine("\nЧтобы убрать товар из корзины введите '-' и '+', чтобы добавить.\n");
                    Console.WriteLine("Если захочешь остановиться, напиши 'Stop'.\n");

                    EditBasket(ref basket, ref assortments);
                    total = CalculationTotal(basket, assortments);
                    ShowBasket(basket, assortments, total);
                }
                if (total != 0)
                {
                    player.wallet = wallet - total;
                    Console.WriteLine("\nУра, Оплата прошла успешно, твой баланс: {0}руб", player.wallet);
                }
                else Console.WriteLine("\nОчень жаль, что ты ничего не взял.");

                Console.WriteLine("Ждём тебя снова!");
            }

            // Подсчет корзины
            int CalculationTotal(List<Store> basket, List<Store> assortments)
            {
                int total = 0;

                foreach (Store product in basket)
                {
                    total += assortments[product.Id].Price * product.Quantity;
                }

                return total;
            }

            // Вывод ассортимента магазина на экран
            void ShowAssortments(List<Store> assortments)
            {
                Console.WriteLine();
                for (int i = 0; i < 42; i++) Console.Write("-");
                foreach (Store assortment in assortments)
                {
                    Console.Write("\n||  {0}({1}шт) - {2}руб (Id {3})",
                        assortment.Name,
                        assortment.Quantity,
                        assortment.Price,
                        assortment.Id);
                    Console.SetCursorPosition(40, Console.CursorTop);
                    Console.Write("||");
                }
                Console.WriteLine();
                for (int i = 0; i < 42; i++) Console.Write("-");
                Console.WriteLine();
            }

            //Вывод корзины на экран
            void ShowBasket(List<Store> basket, List<Store> assortments,int total)
            {
                Console.WriteLine();
                for (int i = 0; i < 42; i++) Console.Write("-");
                foreach (Store product in basket)
                {
                    Console.Write("\n||  {0}({1}шт)", assortments[product.Id].Name, product.Quantity);
                    Console.SetCursorPosition(40, Console.CursorTop);
                    Console.Write("||");
                }
                Console.WriteLine();
                for (int i = 0; i < 42; i++) Console.Write("-");
                Console.Write("\n||  Итого: {0}", total);
                Console.SetCursorPosition(40, Console.CursorTop);
                Console.Write("||");
                Console.WriteLine();
                for (int i = 0; i < 42; i++) Console.Write("-");
                Console.WriteLine();
            }

            // Создание корзины
            void EditBasket(ref List<Store> basket,ref List<Store> assortments)
            {
                string answer = null; // Ответ "+" или "-" на добавления товара в корзину
                int id = 0; // Id товара
                int quality = 0;// Кол-во товара

                while (true)
                {
                    answer = Convert.ToString(Console.ReadLine());
                    switch (answer)
                    {
                        case "+":
                            Console.Write("Id = ");
                            id = Convert.ToInt32(Console.ReadLine());
                            // Проверка на наличие такого Id в магазине
                            if (id > assortments.LongCount() - 1)
                            {
                                Console.WriteLine("Такого Id в магазине нет, попробуйте снова"); Console.WriteLine(); continue;
                            }

                            // Постановка курсора на нужную позицию
                            Console.CursorTop--;
                            Console.CursorLeft = id.ToString().Length + 5;
                            Console.Write(", кол-во = ");
                            quality = Convert.ToInt32(Console.ReadLine());

                            // Проверка на наличие такого кол-ва товара в магазине
                            if (quality > assortments[id].Quantity)
                            {
                                Console.WriteLine("Столько в магазине не осталось, попробуйте снова"); Console.WriteLine(); continue;
                            }

                            // Проверка на уже существующие товары в корзине с таким же Id
                            if (basket.Exists(x => x.Id == id)) basket.Find(x => x.Id == id).Quantity += quality;
                            else basket.Add(new() { Id = id, Quantity = quality });

                            assortments[id].Quantity -= quality;

                            Console.WriteLine("Добавил!\n");
                            continue;
                        case "-":
                            Console.Write("Id = ");
                            id = Convert.ToInt32(Console.ReadLine());
                            // Проверка на наличие такого Id в корзине
                            if (!basket.Exists(x => x.Id == id))
                            {
                                Console.WriteLine("Такого Id в корзине нет, попробуйте снова"); continue;
                            }
                            // Постановка курсора на нужную позицию
                            Console.CursorTop--;
                            Console.CursorLeft = id.ToString().Length + 5;
                            Console.Write(", кол-во = ");
                            quality = Convert.ToInt32(Console.ReadLine());

                            // Проверка на наличие такого кол-ва товара в корзине
                            if (quality > basket.Find(x => x.Id == id).Quantity)
                            {
                                Console.WriteLine("Столько в корзине нет, попробуйте снова"); continue;
                            }

                            // Проверка на уже существующие товары в корзине с таким же Id
                            basket.Find(x => x.Id == id).Quantity -= quality;

                            assortments[id].Quantity += quality;

                            Console.WriteLine("Убрал!\n");
                            continue;
                        case "Stop":
                            break;
                        default: Console.WriteLine("Вы ввели неправильный символ, попробуйте снова"); continue;
                    }
                    break;
                }
            }

            // Идентификация профиля
            void Identification(Player player)
            {
                int health = player.health;
                int power = player.power;
                int endurancewer = player.endurance;
                int wallet = player.wallet;

                if (health < 75) Console.Write("Ты инвалид, ");
                else if (health < 90) Console.Write("Ты не здоровый, ");
                else if (health < 100) Console.Write("Ты мужик, ");
                else if (health < 150) Console.Write("Ты здоровяк, ");
                else if (health < 200) Console.Write("Ты читер, за такое банят.");

                if (power < 400) Console.Write("тюбик, ");
                else if (power < 700) Console.Write("хилый, ");
                else if (power < 1_000) Console.Write("средней силы, ");
                else if (power < 1_500) Console.Write("могучий, ");
                else if (power < 2_000) Console.Write("Ты читер, за такое банят.");

                if (endurancewer < 300) Console.Write("курильщик, ");
                else if (endurancewer < 500) Console.Write("не выносливый, ");
                else if (endurancewer < 700) Console.Write("выносливый, ");
                else if (endurancewer < 1000) Console.Write("стойкий, ");
                else if (endurancewer < 2000) Console.Write("Ты читер, за такое банят.");

                if (wallet < 5_000) Console.Write("бедный.");
                else if (wallet < 20_000) Console.Write("крестьянин.");
                else if (wallet < 50_000) Console.Write("купец.");
                else if (wallet < 100_000) Console.Write("боярин.");
                else if (wallet < 1_000_000) Console.Write("Ты читер, за такое банят.");

                Console.WriteLine();
            }

            // Банк
            void Bank(Player player)
            {
                Bank account = new Bank();

                Console.WriteLine("Добро пожаловать! давайте сделаем вам банковские счёт!");
                Console.Write("Давайте пополним ваш счет, напишите сумму для пополнения: ");

                account.Pay(Convert.ToInt32(Console.ReadLine()));

            }
        }
    }

    // Система магазина
    class Store
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }

    // Банковская система
    class Bank
    {
        int bankAccount = 0;
        public string name = null;
        public void CreateAccount ()
        {
            Console.WriteLine("Вы создали счет! Рады вас видеть {0}!", name);
        }

        public void Pay(int pay)
        {
            bankAccount += pay;
            Console.WriteLine("Спасибо, что пользуетесь услугами нашего банка");
            Console.WriteLine("Ваш баланс составляет {0}руб.", bankAccount);
        }
    }

    // Игрок
    class Player
    {
        public string name = null;
        public int health = 0;
        public int power = 0;
        public int endurance = 0;
        public int wallet = 0;

        Random random = new Random();
        public void CreatePlayer()
        {
            health = random.Next(60, 100);
            power = random.Next(100, 1000);
            endurance = random.Next(100, 1000);
            wallet = random.Next(1_000, 100_000);
        }

        public void ShowCharacteristics()
        {
            Console.WriteLine("Имя: {0}", name);
            Console.WriteLine("Здоровье: {0}", health);
            Console.WriteLine("Сила: {0}", power);
            Console.WriteLine("Выносливость: {0}", endurance);
            Console.WriteLine("Баланс: {0}", wallet);
        }
    }
}