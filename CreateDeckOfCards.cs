using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static System.Random;
using System.Collections;
using static System.Threading.Thread;
using DeckOfCards;
namespace CardGames
{
    class CreateDeckOfCards
    {
        static ArrayList deckOfCardsNumbers = new ArrayList();
        static ArrayList deckOfCardsSuits = new ArrayList();
        static void Main(string[] args)
        {
            int choice = 1;
            CreateDeck();
            ShuffleDeck();
            string title = "\n\tCARD GAMES INTERFACE V 1.7.1\n________________________________________";
            string choices = title +
                    "\nChoose a game! Enter your choice below, then press ENTER." +
                    "\n1:War" +
                    "\n2:Go Fish" +
                    "\n3:View Go Fish Leaderboards" +
                    "\n4:Blackjack" +
                    "\n5:View Blackjack Leaderboards" +
                    "\n0:Quit";

            WriteLine(title);
            Sleep(1000);
            DisplayWelcomeScreen3();
            Clear();
            do
            {
                Clear();
                ShuffleDeck();
                WriteLine(choices);
                Write("\nChoice >> ");
                if (int.TryParse(ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 0:
                            Clear();
                            WriteLine(title);
                            Sleep(500);
                            DisplayExitScreen1();
                            choice = -1;
                            break;
                        case 1:
                            Clear();
                            ShuffleDeck();
                            War.PlayWar(deckOfCardsNumbers);
                            break;
                        case 2:
                            Clear();
                            ShuffleDeck();
                            GoFishV2_1.PlayGoFish(deckOfCardsNumbers);
                            break;
                        case 3:
                            Clear();
                            GoFishV2_1.DisplayAllLeaderboards();
                            break;
                        case 4:
                            Clear();
                            ArrayList[] standardDecks = new ArrayList[6];
                            for (int count = 0; count < standardDecks.Length; ++count)
                            {
                                standardDecks[count] = new ArrayList();
                                ShuffleDeck();
                                for (int card = 0; card < 52; ++card)
                                    standardDecks[count].Add(deckOfCardsNumbers[card]);
                            }
                            Blackjack.PrepareBlackjack(standardDecks);
                            break;
                        case 5:
                            Clear();
                            BlackjackSolo.DisplayLeaderboards();
                            break;
                        default:
                            WriteLine("Error - this choice is not available. Press ENTER to continue.");
                            ReadKey();
                            break;
                    }
                }
                else
                {
                    WriteLine("Error - the choice was in the incorrect format. Press ENTER to continue.");
                    ReadKey();
                }
            } while (choice != -1);
        }

        public static void CreateDeck()
        {
            int[] numbers = new int[52];
            string[] suits = new string[52];
            int val = 1;
            for (int count = 0; count < 52; ++count)
            {
                if (val > 13)
                    val = 1;
                numbers[count] = val;
                ++val;
            }

            for (int count = 0; count < 13; ++count)
                suits[count] = "Spades";

            for (int count = 13; count < 26; ++count)
                suits[count] = "Clubs";

            for (int count = 26; count < 39; ++count)
                suits[count] = "Hearts";

            for (int count = 39; count < 52; ++count)
                suits[count] = "Diamonds";

            deckOfCardsNumbers.AddRange(numbers);
            deckOfCardsSuits.AddRange(suits);
        }

        public static void DisplayDeck()
        {
            for (int count = 0; count < 52; ++count)
            {
                WriteLine($"{deckOfCardsNumbers[count]} of {deckOfCardsSuits[count]}");
            }
            ReadKey();
        }

        public static void ShuffleDeck()
        {
            int tempNumber;
            string tempSuit;
            int val;
            Random rand = new Random();
            for (int countCycle = 0; countCycle < 100; ++countCycle)
            {
                for (int count = 0; count < 52; ++count)
                {
                    val = rand.Next(0, 52);
                    tempNumber = Convert.ToInt32(deckOfCardsNumbers[count]);
                    tempSuit = deckOfCardsSuits[count].ToString();
                    deckOfCardsNumbers[count] = deckOfCardsNumbers[val];
                    deckOfCardsSuits[count] = deckOfCardsSuits[val];
                    deckOfCardsNumbers[val] = tempNumber;
                    deckOfCardsSuits[val] = tempSuit;
                }
            }
        }

        public static void DisplayWelcomeScreen1()
        {
            WriteLine("\nWelcome!\t\t\tWelcome!");
            Sleep(500);
            WriteLine("\n\tWelcome!\tWelcome!");
            Sleep(500);
            WriteLine("\n\t\tWelcome!");
            Sleep(500);
            WriteLine("\n\tWelcome!\tWelcome!");
            Sleep(500);
            WriteLine("\nWelcome!\t\t\tWelcome!");
            Sleep(500);
        }

        public static void DisplayWelcomeScreen2()
        {
            WriteLine("\n                     Welcome!\n");
            Sleep(1000);
            WriteLine("________00000000000___________000000000000_________");
            Sleep(100);
            WriteLine("______00000000_____00000___000000_____0000000______");
            Sleep(100);
            WriteLine("____0000000_____________000______________00000_____");
            Sleep(100);
            WriteLine("___0000000_______________0_________________0000____");
            Sleep(100);
            WriteLine("__000000____________________________________0000___");
            Sleep(100);
            WriteLine("__00000_____________________________________ 0000__");
            Sleep(100);
            WriteLine("_00000______________________________________00000__");
            Sleep(100);
            WriteLine("_00000_____________________________________000000__");
            Sleep(100);
            WriteLine("__000000_________________________________0000000___");
            Sleep(100);
            WriteLine("___0000000______________________________0000000____");
            Sleep(100);
            WriteLine("_____000000____________________________000000______");
            Sleep(100);
            WriteLine("_______000000________________________000000________");
            Sleep(100);
            WriteLine("__________00000_____________________0000___________");
            Sleep(100);
            WriteLine("_____________0000_________________0000_____________");
            Sleep(100);
            WriteLine("_______________0000_____________000________________");
            Sleep(100);
            WriteLine("_________________000_________000___________________");
            Sleep(100);
            WriteLine("_________________ __000_____00_____________________");
            Sleep(100);
            WriteLine("______________________00__00_______________________");
            Sleep(100);
            WriteLine("________________________00_________________________");
            Sleep(1000);
        }

        public static void DisplayWelcomeScreen3()
        {
            WriteLine("\n                     Welcome!\n");
            Sleep(1000);
            WriteLine("________________________00_________________________");
            Sleep(100);
            WriteLine("______________________00__00_______________________");
            Sleep(100);
            WriteLine("_________________ __000_____00_____________________");
            Sleep(100);
            WriteLine("_________________000_________000___________________");
            Sleep(100);
            WriteLine("_______________0000_____________000________________");
            Sleep(100);
            WriteLine("_____________0000_________________0000_____________");
            Sleep(100);
            WriteLine("__________00000_____________________0000___________");
            Sleep(100);
            WriteLine("_______000000________________________000000________");
            Sleep(100);
            WriteLine("_____000000____________________________000000______");
            Sleep(100);
            WriteLine("___0000000______________________________0000000____");
            Sleep(100);
            WriteLine("__000000_________________________________0000000___");
            Sleep(100);
            WriteLine("_00000_____________________________________000000__");
            Sleep(100);
            WriteLine("_00000______________________________________00000__");
            Sleep(100);
            WriteLine("__00000_____________________________________ 0000__");
            Sleep(100);
            WriteLine("__000000____________________________________0000___");
            Sleep(100);
            WriteLine("___0000000_______________0_________________0000____");
            Sleep(100);
            WriteLine("____0000000_____________000______________00000_____");
            Sleep(100);
            WriteLine("______00000000_____00000___000000_____0000000______");
            Sleep(100);
            WriteLine("________00000000000___________000000000000_________");
            Sleep(1000);
        }

        public static void DisplayExitScreen1()
        {
            WriteLine("\nGoodbye!\t\t\tGoodbye!");
            Sleep(500);
            WriteLine("\n\tGoodbye!\tGoodbye!");
            Sleep(500);
            WriteLine("\n\t\tGoodbye!");
            Sleep(500);
            WriteLine("\n\tGoodbye!\tGoodbye!");
            Sleep(500);
            WriteLine("\nGoodbye!\t\t\tGoodbye!");
            Sleep(500);
        }
    }
}

