//Daniel Dyson
//7/2/17
//Blackjack (bicyclecards.com)
//Ace can be 1 or 11
//Face cards are 10 (11-13)
//Limits on betting
//Cut, so that 60-75 cards will not be used
//Place bets now
//All players (including dealer) are given one face-up card. This is repeated, except that the dealer's second card is facedown.
//Check for naturals (ace and ten card). The dealer may check for a natural if the face up card is an ace or ten.
//Player's turn: stand or hit
//Split pairs
//Double down
//Insurance
/*
Insurance
When the dealer's face-up card is an ace, any of the players 
may make a side bet of up to half the original bet that the 
dealer's face-down card is a ten-card, and thus a blackjack for the 
house. Once all such side bets are placed, the dealer looks at his 
hole card. If it is a ten-card, it is turned up, and those players 
who have made the insurance bet win and are paid double the amount 
of their half-bet - a 2 to 1 payoff. When a blackjack occurs for 
the dealer, of course, the hand is over, and the players' main 
bets are collected - unless a player also has blackjack, in which 
case it is a stand-off. Insurance is invariably not a good proposition 
for the player, unless he is quite sure that there are an unusually 
high number of ten-cards still left undealt.

More players???
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Collections;
using static System.Random;
using System.IO;

namespace DeckOfCards
{
    class BlackjackSolo
    {
        //((ArrayList)playerHand[0])[1]: the second card in the first hand
        //(ArrayList)playerHand[0]: the first hand
        //playerHand: a player
        static ArrayList preDeck;
        static Queue<int> deck;
        static Random rand;
        static int playerMoney;
        static int playerBet;
        static int cutLocation;
        static ArrayList dealerHand;
        static ArrayList playerHand;
        static bool proceedToPlayer;
        static bool playerBust;
        static bool dealerBust;
        static string playerName;

        public static void PrepareBlackjack(ArrayList[] standardDecks)
        {
            preDeck = new ArrayList();
            deck = new Queue<int>();
            rand = new Random();
            playerMoney = 1500;
            foreach (ArrayList d in standardDecks)
            {
                for (int count = 0; count < 52; ++count)
                    preDeck.Add(d[count]);
            }
            ShuffleDeck();
            for (int count = 0; count < preDeck.Count; ++count)
                deck.Enqueue(Convert.ToInt32(preDeck[count]));
            bool isValid = false;
            do
            {
                Clear();
                WriteLine("Please enter your name.");
                Write(">> ");
                playerName = ReadLine();
                Clear();
                WriteLine("Where would you like to cut the deck? \nEnter the approximate number of cards that will not be used (typically between 60 and 75 cards).");
                Write(">> ");
                if (int.TryParse(ReadLine(), out cutLocation) == false)
                {
                    WriteLine("Error - incorrect format. Press ENTER to try again.");
                    ReadKey();
                }
                else if (cutLocation >= 270)
                {
                    WriteLine("Error - this location is too close to the top, or goes beyond the number of cards.\n" +
                        "Please press ENTER, and choose a lower number.");
                    ReadKey();
                }
                else
                {
                    isValid = true;
                    cutLocation += rand.Next(-4, 5);
                }
            } while (isValid == false);
            dealerHand = new ArrayList();
            playerHand = new ArrayList();
            PlayBlackjack();
        }

        public static void ShuffleDeck()
        {
            int tempNumber;
            int val;
            for (int countCycle = 0; countCycle < 100; ++countCycle)
            {
                for (int count = 0; count < preDeck.Count; ++count)
                {
                    val = rand.Next(0, 52);
                    tempNumber = Convert.ToInt32(preDeck[count]);
                    preDeck[count] = preDeck[val];
                    preDeck[val] = tempNumber;
                }
            }
        }

        public static void PlayBlackjack()
        {
            bool gameEnded = false;
            ArrayList playerTotal = new ArrayList();
            int dealerTotal = 0;
            do
            {
                Clear();
                if (deck.Count <= cutLocation)
                {
                    gameEnded = true;
                }
                else if (playerMoney <= 0)
                {
                    gameEnded = true;
                }
                else
                {
                    bool isValid = false;
                    do
                    {
                        Clear();
                        WriteLine($"Your money: {playerMoney:c2}");
                        WriteLine("How much money would you like to bet? (Minimum: $2, Maximum: $500, Quit Playing: $0)");
                        Write(">> ");
                        if (int.TryParse(ReadLine(), out playerBet) == false)
                        {
                            WriteLine("Error - incorrect format. Please press ENTER and try again.");
                            ReadKey();
                        }
                        else if (playerBet < 2 || playerBet > 500)
                        {
                            if (playerBet == 0)
                            {
                                WriteLine("Thanks for playing! Please press ENTER to continue.");
                                gameEnded = true;
                                isValid = true;
                            }
                            else
                            {
                                WriteLine("Error - the bet is outside the acceptable limits. Please press ENTER and try again.");
                            }
                            ReadKey();
                        }
                        else if (playerBet > playerMoney)
                        {
                            WriteLine("Error - your bet is more than how much money you have left. Please press ENTER and try again.");
                            ReadKey();
                        }
                        else
                        {
                            isValid = true;
                        }
                    } while (isValid == false);
                    if (gameEnded == false)
                    {
                        playerMoney -= playerBet;
                        playerHand.Clear();
                        dealerHand.Clear();
                        playerTotal.Clear();
                        dealerTotal = 0;
                        playerBust = false;
                        dealerBust = false;
                        proceedToPlayer = false;
                        ArrayList hand = new ArrayList();
                        for (int count = 0; count < 2; ++count)
                        {
                            hand.Add(deck.Dequeue());
                            dealerHand.Add(deck.Dequeue());
                        }
                        playerHand.Add(hand);
                        CheckForNaturals();
                        if (proceedToPlayer)
                        {
                            PlayerTurn();
                            DealerTurn(out dealerTotal);
                            for (int handNum = 0; handNum < playerHand.Count; ++handNum)
                            {
                                playerTotal.Add(CalculateTotal((ArrayList)playerHand[handNum]));
                                if (Convert.ToInt32(playerTotal[handNum]) > dealerTotal)
                                {
                                    if (playerBust)
                                    {
                                        WriteLine("You lost against the dealer.");
                                    }
                                    else
                                    {
                                        WriteLine("You beat the dealer! Your payoff is 1.5x the amount of your bet.");
                                        playerMoney += (int)(playerBet * 1.5);
                                    }

                                }
                                else if (Convert.ToInt32(playerTotal[handNum]) == dealerTotal)
                                {
                                    if (playerBust)
                                    {
                                        WriteLine("You lost against the dealer.");
                                    }
                                    else
                                    {
                                        WriteLine("Stand-off. You receive your initial bet.");
                                        playerMoney += playerBet;
                                    }
                                }
                                else if (Convert.ToInt32(playerTotal[handNum]) < dealerTotal)
                                {
                                    if (playerBust)
                                    {
                                        WriteLine("You lost against the dealer.");
                                    }
                                    else if (dealerBust)
                                    {
                                        WriteLine("You beat the dealer! Your payoff is 1.5x the amount of your bet.");
                                        playerMoney += (int)(playerBet * 1.5);
                                    }
                                    else
                                    {
                                        WriteLine("You lost against the dealer.");
                                    }
                                }
                                ReadKey();
                            }
                        }
                    }
                }
            } while (gameEnded == false);
            Clear();
            string path = $"Blackjack Leaderboards.txt";
            AddRecordToLeaderboards(path);
            WriteLine($"Your money: {playerMoney:c2}");
            if (playerMoney >= 1500)
            {
                WriteLine($"Your taking: {playerMoney - 1500:c2}");
            }
            else
            {
                WriteLine($"Your losses: {-1 * (playerMoney - 1500):c2}");
            }
            WriteLine();
            DisplayLeaderboards(path);
        }

        public static void CheckForNaturals()
        {
            bool dealerHasNatural = dealerHand.Contains(1) && (dealerHand.Contains(10) || dealerHand.Contains(11) || dealerHand.Contains(12) || dealerHand.Contains(13));
            bool playerHasNatural = ((ArrayList)playerHand[0]).Contains(1) && (((ArrayList)playerHand[0]).Contains(10) || ((ArrayList)playerHand[0]).Contains(11) || ((ArrayList)playerHand[0]).Contains(12) || ((ArrayList)playerHand[0]).Contains(13));

            if (playerHasNatural)
            {
                //Your cards:
                WriteLine("You have a blackjack!");
                WriteLine($"Your cards: {((ArrayList)playerHand[0])[0]} {((ArrayList)playerHand[0])[1]}");
                WriteLine();
                if (dealerHasNatural)
                {
                    //Dealer's cards:
                    WriteLine("Stand-off - the dealer also has a blackjack. You may take back your bet.");
                    WriteLine($"Dealer's cards: {dealerHand[0]} {dealerHand[1]}");
                    playerMoney += playerBet;
                }
                else
                {
                    WriteLine("You now collect 1.5x the amount of your bet!");
                    playerMoney += (int)(playerBet * 1.5);
                }
                ReadKey();
            }
            else if (dealerHasNatural)
            {
                //Dealer's cards
                WriteLine("The dealer has a blackjack, and you do not. The dealer now collects your bet.");
                WriteLine($"Dealer's cards: {dealerHand[0]} {dealerHand[1]}");
                ReadKey();
            }
            else
            {
                proceedToPlayer = true;
            }
        }

        public static void PlayerTurn()
        {
            /*
             * Doubling Down
             * Another option open to the player is doubling his bet when the original two cards dealt total 
             * 9, 10, or 11. When the player's turn comes, he places a bet equal to the original bet, and the 
             * dealer gives him just one card, which is placed face down and is not turned up until the bets
             * are settled at the end of the hand. With two fives, the player may split a pair, double down,
             * or just play the hand in the regular way. Note that the dealer does not have the option of 
             * splitting or doubling down.
             */
            bool stand;
            bool bust;
            int choice;
            ArrayList[] hands;
            bool canSplit;
            bool canDoubleDown;
            bool willDoubleDown = false;
            bool pairOfAces;
            int total;
            ValidateSplitting(out canSplit, out pairOfAces);
            ValidateDoublingDown(CalculateTotal((ArrayList)playerHand[0]), out canDoubleDown);
            if (canSplit)
            {
                WriteLine("You got a pair! Would you like your hand to be split into two separate hands? (0:Yes, 1:No)");
                Write("Your cards: ");
                foreach (object card in (ArrayList)playerHand[0])
                {
                    Write(card + " ");
                }
                WriteLine();
                Write(">> ");
                if (pairOfAces)
                {
                    WriteLine("It was a pair of aces! Only ONE card would be dealt to each hand.");
                }
                if (int.TryParse(ReadLine(), out choice) == false)
                {
                    WriteLine("Error - could not recognize choice. Continuing without splitting.");
                    hands = new ArrayList[1];
                    hands[0] = new ArrayList();
                    hands[0].Add(((ArrayList)playerHand[0])[0]);
                    hands[0].Add(((ArrayList)playerHand[0])[1]);
                }
                else if (choice == 0)
                {
                    hands = new ArrayList[2];
                    hands[0] = new ArrayList();
                    hands[1] = new ArrayList();
                    hands[0].Add(((ArrayList)playerHand[0])[0]);
                    hands[1].Add(((ArrayList)playerHand[0])[1]);
                    playerMoney -= playerBet;
                }
                else if (choice == 1)
                {
                    hands = new ArrayList[1];
                    hands[0] = new ArrayList();
                    hands[0].Add(((ArrayList)playerHand[0])[0]);
                    hands[0].Add(((ArrayList)playerHand[0])[1]);
                }
                else
                {
                    WriteLine("Error - could not recognize choice. Continuing without splitting.");
                    hands = new ArrayList[1];
                    hands[0] = new ArrayList();
                    hands[0].Add(((ArrayList)playerHand[0])[0]);
                    hands[0].Add(((ArrayList)playerHand[0])[1]);
                }
            }
            else
            {
                hands = new ArrayList[1];
                hands[0] = new ArrayList();
                hands[0].Add(((ArrayList)playerHand[0])[0]);
                hands[0].Add(((ArrayList)playerHand[0])[1]);
            }
            if (canDoubleDown)
            {
                WriteLine("Your hand's value is between 9 and 11. Would you like to double down? (0:Yes, 1:No)");
                Write("Your cards: ");
                foreach (object card in (ArrayList)playerHand[0])
                {
                    Write(card + " ");
                }
                WriteLine();
                Write(">> ");
                if (int.TryParse(ReadLine(), out choice) == false)
                {
                    WriteLine("Error - could not recognize choice. Continuing without doubling down.");
                    willDoubleDown = false;
                }
                else if (choice == 0)
                {
                    WriteLine("You have chosen to double down. Your bet has doubled, and you will be given a facedown card.\n" +
                        "This card will be revealed once all bets have been collected.");
                    for (int count = 0; count < hands.Length; ++count)
                    {
                        hands[count].Add(deck.Dequeue());
                    }
                    playerMoney -= playerBet;
                    playerBet += playerBet;
                    willDoubleDown = true;
                }
                else if (choice == 1)
                {
                    WriteLine("You have chosen not to double down.");
                    willDoubleDown = false;
                }
                else
                {
                    WriteLine("Error - could not recognize choice. Continuing without doubling down.");
                    willDoubleDown = false;
                }
            }

            playerHand.Clear();
            //For each hand...
            for (int hand = 0; hand < hands.Length; ++hand)
            {
                total = CalculateTotal(hands[hand]);
                stand = false;
                bust = false;
                do
                {
                    Clear();
                    WriteLine($"Your money: {playerMoney:c2}");
                    WriteLine($"Your bet: {playerBet:c2}");
                    WriteLine($"Dealer's Face-up Card: {dealerHand[0]}");
                    Write("Your cards: ");
                    foreach (object card in hands[hand])
                    {
                        Write(card + " ");
                    }
                    WriteLine();
                    if (pairOfAces)
                    {
                        hands[hand].Add(deck.Dequeue());
                        total = CalculateTotal(hands[hand]);
                        if (total == 21)
                        {
                            WriteLine("You got a blackjack on one of your aces - the payoff is equal to the bet.");
                            ReadKey();
                        }
                        stand = true;
                    }
                    else if (willDoubleDown)
                    {
                        stand = true;
                    }
                    else
                    {
                        Write($"Your total value in hand: {total}.\n" +
                            "Please choose whether to:\n" +
                            "0:Hit, or\n" +
                            "1:Stand.\n" +
                            ">> ");
                        if (int.TryParse(ReadLine(), out choice) == false)
                        {
                            WriteLine("Error - choice in incorrect format. Please press ENTER and try again.");
                        }
                        else if (choice == 0)
                        {
                            hands[hand].Add(deck.Dequeue());
                            total = CalculateTotal(hands[hand]);
                            Write("Your cards: ");
                            foreach (object card in hands[hand])
                            {
                                Write(card + " ");
                            }
                            WriteLine();
                            WriteLine($"Your new total is {total}.");
                            if (total > 21)
                            {
                                WriteLine("Bust! Your bet is collected by the dealer.");
                                bust = true;
                                playerBust = true;
                            }
                            else if (total == 21)
                            {
                                WriteLine("Great job!");
                                stand = true;
                            }
                        }
                        else if (choice == 1)
                        {
                            WriteLine("You chose to stand.");
                            stand = true;
                        }
                        else
                        {
                            WriteLine("Error - choice not recognized. Please press ENTER and try again.");
                        }
                        ReadKey();
                    }
                } while (bust == false && stand == false);
                playerHand.Add(hands[hand]);
            }
        }

        public static void DealerTurn(out int total)
        {
            bool stand = false;
            bool bust = false;
            do
            {
                Clear();
                total = CalculateTotal(dealerHand);
                WriteLine($"Your money: {playerMoney:c2}");
                WriteLine($"Your bet: {playerBet:c2}");
                Write("Dealer's cards: ");
                foreach (object card in dealerHand)
                {
                    Write(card + " ");
                }
                WriteLine();
                WriteLine($"Dealer's total value in hand: {total}.");
                foreach (ArrayList hand in playerHand)
                {
                    Write("Your cards: ");
                    foreach (object card in hand)
                    {
                        Write(card + " ");
                    }
                    WriteLine($"\nYour total value in hand: {CalculateTotal(hand)}");
                }
                if (total < 17)
                {
                    WriteLine("\nDealer will hit.");
                    if (playerBust == false)
                        ReadKey();
                    dealerHand.Add(deck.Dequeue());
                    WriteLine($"Card drawn: {dealerHand[dealerHand.Count - 1]}");
                    total = CalculateTotal(dealerHand);
                    WriteLine($"The dealer's new total is {total}.");
                    if (total > 21)
                    {
                        WriteLine("The dealer went bust!");
                        bust = true;
                        dealerBust = true;
                    }
                    else if (total == 21)
                    {
                        WriteLine("The dealer got 21!");
                        stand = true;
                    }
                    if (playerBust == false)
                        ReadKey();
                }
                else
                {
                    WriteLine("\nDealer will stand.");
                    if (playerBust == false)
                        ReadKey();
                    stand = true;
                }
            } while (bust == false && stand == false);
        }

        public static int CalculateTotal(ArrayList hand)
        {
            hand.Sort();
            int numOfAces = 0;
            int total = 0;
            for (int count = hand.Count - 1; count >= 0; --count)
            {
                if (Convert.ToInt32(hand[count]) == 1)
                {
                    ++numOfAces;
                }
            }
            if (numOfAces == 1)
            {
                total += 11;
                for (int count = 1; count < hand.Count; ++count)
                {
                    int testVal = Convert.ToInt32(hand[count]);
                    if (testVal == 11 || testVal == 12 || testVal == 13)
                    {
                        total += 10;
                    }
                    else
                    {
                        total += testVal;
                    }
                }
                if (total > 21)
                {
                    total = 0;
                    for (int count = 0; count < hand.Count; ++count)
                    {
                        int testVal = Convert.ToInt32(hand[count]);
                        if (testVal == 11 || testVal == 12 || testVal == 13)
                        {
                            total += 10;
                        }
                        else
                        {
                            total += testVal;
                        }
                    }
                }
            }
            else
            {
                for (int count = 0; count < hand.Count; ++count)
                {
                    int testVal = Convert.ToInt32(hand[count]);
                    if (testVal == 11 || testVal == 12 || testVal == 13)
                    {
                        total += 10;
                    }
                    else
                    {
                        total += testVal;
                    }
                }
            }
            return total;
        }

        public static void ValidateSplitting(out bool canSplit, out bool pairOfAces)
        {
            if (Convert.ToInt32(((ArrayList)playerHand[0])[0]) == Convert.ToInt32(((ArrayList)playerHand[0])[1]))
            {
                if (playerBet <= playerMoney)
                {
                    canSplit = true;
                }
                else
                {
                    canSplit = false;
                }
                if ((canSplit == true) && (Convert.ToInt32(((ArrayList)playerHand[0])[0]) == 1))
                {
                    pairOfAces = true;
                }
                else
                {
                    pairOfAces = false;
                }
            }
            else
            {
                canSplit = false;
                pairOfAces = false;
            }
        }

        public static void ValidateDoublingDown(int total, out bool canDoubleDown)
        {
            if (total >= 9 && total <= 11)
            {
                if (playerBet <= playerMoney)
                {
                    canDoubleDown = true;
                }
                else
                {
                    canDoubleDown = false;
                }
            }
            else
            {
                canDoubleDown = false;
            }
        }

        public static void AddRecordToLeaderboards(string path)
        {
            ArrayList leaderboardRecords = new ArrayList();
            string newRecord = $"{playerName}\t\t{playerMoney}";
            try
            {
                if (File.Exists(path))
                {
                    StreamReader read = new StreamReader(path);
                    while (!read.EndOfStream)
                    {
                        leaderboardRecords.Add(read.ReadLine());
                    }
                    read.Close();
                    string score;
                    bool canBreak = false;
                    bool lowestScore = true;
                    //For all the lines
                    for (int records = 0; records < leaderboardRecords.Count; ++records)
                    {
                        score = null;
                        string line = leaderboardRecords[records].ToString();
                        char[] lineChars = line.ToCharArray();

                        for (int c = lineChars.Length - 3; c < lineChars.Length; ++c)
                        {
                            if (char.IsNumber(lineChars[c]))
                            {
                                score += lineChars[c].ToString();
                            }
                        }
                        if (int.Parse(score) == playerMoney || int.Parse(score) < playerMoney)
                        {
                            //add this score to the place before this record
                            leaderboardRecords.Insert(records, newRecord);
                            lowestScore = false;
                            canBreak = true;
                        }
                        if (canBreak)
                            break;
                    }
                    if (lowestScore)
                    {
                        leaderboardRecords.Add(newRecord);
                    }

                    StreamWriter write = new StreamWriter(path);
                    for (int recordsAgain = 0; recordsAgain < leaderboardRecords.Count; ++recordsAgain)
                    {
                        write.WriteLine(leaderboardRecords[recordsAgain]);
                    }
                    write.Close();
                }
                else
                {
                    StreamWriter write = new StreamWriter(path);
                    write.WriteLine(newRecord);
                    write.Close();
                }
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }
        }

        public static void DisplayLeaderboards(string path)
        {
            WriteLine("BLACKJACK LEADERBOARDS - Total Money Left\n");
            try
            {
                StreamReader read = new StreamReader(path);
                while (!read.EndOfStream)
                    WriteLine(read.ReadLine());
                read.Close();
            }
            catch (FileNotFoundException)
            {
                WriteLine("Error - no leaderboard stats exist yet for this game.");
            }
            catch (Exception)
            {
                WriteLine("Error - could not load leaderboard stats.");
            }
            WriteLine("\nPlease press ENTER to continue.");
            ReadKey();
        }

        public static void DisplayLeaderboards()
        {
            string path = $"Blackjack Leaderboards.txt";
            WriteLine("BLACKJACK LEADERBOARDS - Total Money Left\n");
            try
            {
                StreamReader read = new StreamReader(path);
                while (!read.EndOfStream)
                    WriteLine(read.ReadLine());
                read.Close();
            }
            catch (FileNotFoundException)
            {
                WriteLine("Error - no leaderboard stats exist yet for this game.");
            }
            catch (Exception)
            {
                WriteLine("Error - could not load leaderboard stats.");
            }
            WriteLine("\nPlease press ENTER to continue.");
            ReadKey();
        }
    }
}