//
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
using static System.Threading.Thread;

namespace DeckOfCards
{
    class Blackjack
    {
        //((ArrayList)playerHand[0][0])[1]: the second card in the first hand of the first player
        //(ArrayList)playerHand[1][0]: the first hand of the second player
        //playerHand[index]: a player

        //ERRORS:   When someone gets a natural, they must be exempt from the turn
        static ArrayList preDeck;
        static Queue<int> deck;
        static Random rand;
        static int[] playersMoney;
        static int[] playersBet;
        static int cutLocation;
        static ArrayList dealerHand;
        static ArrayList[] playersHand;
        static bool[] playersBust;
        static bool dealerBust;
        static string[] playersName;
        static int playerCount;
        static ArrayList indicesOfPlayers;
        static ArrayList[] playersTotal;
        static bool[] playersNatural;
        static bool dealerHasNatural;
        static int dealerTotal;

        public static void PrepareBlackjack(ArrayList[] standardDecks)
        {
            preDeck = new ArrayList();
            deck = new Queue<int>();
            rand = new Random();
            bool isValid = false;
            foreach (ArrayList d in standardDecks)
            {
                for (int count = 0; count < 52; ++count)
                    preDeck.Add(d[count]);
            }
            ShuffleDeck();
            for (int count = 0; count < preDeck.Count; ++count)
                deck.Enqueue(Convert.ToInt32(preDeck[count]));
            dealerHand = new ArrayList();
            do
            {
                playerCount = RetrieveInput("How many players will be playing?");
                if (playerCount < 1)
                {
                    WriteLine("Error - you cannot have less than one player. Please press ENTER and try again.");
                    ReadKey();
                }
                else
                {
                    isValid = true;
                }
            } while (isValid == false);
            playersHand = new ArrayList[playerCount];
            playersMoney = new int[playerCount];
            playersBet = new int[playerCount];
            playersBust = new bool[playerCount];
            playersName = new string[playerCount];
            indicesOfPlayers = new ArrayList(playerCount);
            playersTotal = new ArrayList[playerCount];
            playersNatural = new bool[playerCount];
            Clear();
            for (int count = 0; count < playerCount; ++count)
            {
                WriteLine($"Player {count + 1}, please enter your name.");
                Write(">> ");
                playersName[count] = ReadLine();
                playersMoney[count] = 1500;
                playersHand[count] = new ArrayList();
                playersBust[count] = false;
                indicesOfPlayers.Add(count);
                playersTotal[count] = new ArrayList();
                playersNatural[count] = false;
            }
            int randomPlayer = rand.Next(0, playerCount);
            isValid = false;
            do
            {
                cutLocation = RetrieveInput($"{playersName[randomPlayer]}, where would you like to cut the deck? " +
                    "\nEnter the approximate number of cards that will not be used (typically between 60 and 75 cards).");
                if (cutLocation >= 270)
                {
                    WriteLine("Error - this location is too close to the top of the deck, or goes beyond the number of cards.\n" +
                        "Please press ENTER, and choose a lower number.");
                    ReadKey();
                }
                else if (cutLocation < 0)
                {
                    WriteLine("Error - invalid location. \nPlease press ENTER and try again.");
                }
                else
                {
                    cutLocation += rand.Next(-4, 5);
                    isValid = true;
                }
            } while (isValid == false);

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
            do
            {
                Clear();
                dealerTotal = 0;
                for (int count = 0; count < playerCount; ++count)
                {
                    PlaceBets(count);
                }
                if (deck.Count <= cutLocation)
                {
                    gameEnded = true;
                }
                else if (indicesOfPlayers.Count == 0)
                {
                    gameEnded = true;
                }
                if (gameEnded == false)
                {
                    dealerHand.Clear();
                    dealerTotal = 0;
                    dealerBust = false;
                    ArrayList[] hand = new ArrayList[indicesOfPlayers.Count];
                    foreach (int index in indicesOfPlayers)
                    {
                        playersMoney[index] -= playersBet[index];
                        playersHand[index].Clear();
                        playersTotal[index].Clear();
                        playersBust[index] = false;
                    }
                    for (int count = 0; count < indicesOfPlayers.Count; ++count)
                    {
                        hand[count] = new ArrayList();
                    }
                    for (int count = 0; count < 2; ++count)
                    {
                        for (int count2 = 0; count2 < hand.Length; ++count2)
                        {
                            hand[count2].Add(deck.Dequeue());
                        }
                        dealerHand.Add(deck.Dequeue());
                    }
                    int handCount = 0;
                    foreach (int index in indicesOfPlayers)
                    {
                        playersHand[index].Add(hand[handCount]);
                        ++handCount;
                    }
                    CheckForNaturals();
                    foreach (int index in indicesOfPlayers)
                    {
                        if (playersNatural[index] == false && dealerHasNatural == false)
                            PlayerTurn(index);
                    }
                    if (dealerHasNatural == false)
                        DealerTurn(out dealerTotal);
                    foreach (int index in indicesOfPlayers)
                    {
                        if (playersNatural[index] == false && dealerHasNatural == false)
                        {
                            for (int handNum = 0; handNum < playersHand[index].Count; ++handNum)
                            {
                                playersTotal[index].Add(CalculateTotal((ArrayList)playersHand[index][handNum]));
                                ValidateBust(Convert.ToInt32(playersTotal[index][handNum]), out playersBust[index]);
                                if (Convert.ToInt32(playersTotal[index][handNum]) > dealerTotal)
                                {
                                    if (playersBust[index])
                                    {
                                        WriteLine($"{playersName[index]}, you lost against the dealer.");
                                    }
                                    else
                                    {
                                        WriteLine($"{playersName[index]}, you beat the dealer! Your payoff is 1.5x the amount of your bet.");
                                        playersMoney[index] += (int)(playersBet[index] * 1.5);
                                    }
                                }
                                else if (Convert.ToInt32(playersTotal[index][handNum]) == dealerTotal)
                                {
                                    if (playersBust[index])
                                    {
                                        WriteLine($"{playersName[index]}, you lost against the dealer.");
                                    }
                                    else
                                    {
                                        WriteLine($"Stand-off. {playersName[index]}, you receive your initial bet.");
                                        playersMoney[index] += playersBet[index];
                                    }
                                }
                                else if (Convert.ToInt32(playersTotal[index][handNum]) < dealerTotal)
                                {
                                    if (playersBust[index])
                                    {
                                        WriteLine($"{playersName[index]}, you lost against the dealer.");
                                    }
                                    else if (dealerBust)
                                    {
                                        WriteLine($"{playersName[index]}, you beat the dealer! Your payoff is 1.5x the amount of your bet.");
                                        playersMoney[index] += (int)(playersBet[index] * 1.5);
                                    }
                                    else
                                    {
                                        WriteLine($"{playersName[index]}, you lost against the dealer.");
                                    }
                                }
                                WriteLine("Please press ENTER to continue.");
                                ReadKey();
                            }
                        }
                    }
                }
            } while (gameEnded == false);
            Clear();
            string path = $"Blackjack Leaderboards.txt";
            DisplayLeaderboards(path);
        }

        public static void PlaceBets(int index)
        {
            bool isValid = false;
            do
            {
                Clear();
                playersBet[index] = RetrieveInput($"Your money: {playersMoney[index]:c2}\n" + $"{playersName[index]}, how much money would you like to bet? (Minimum: $2, Maximum: $500, Quit Playing: $0)");
                if (playersBet[index] < 2 || playersBet[index] > 500)
                {
                    if (playersBet[index] == 0)
                    {
                        WriteLine($"Thanks for playing, {playersName[index]}!");
                        WriteLine($"Your money: {playersMoney[index]:c2}");
                        if (playersMoney[index] >= 1500)
                        {
                            WriteLine($"Your taking: {playersMoney[index] - 1500:c2}");
                        }
                        else
                        {
                            WriteLine($"Your losses: {-1 * (playersMoney[index] - 1500):c2}");
                        }
                        WriteLine("Please press ENTER to continue.");
                        indicesOfPlayers.Remove(index);
                        string path = $"Blackjack Leaderboards.txt";
                        AddRecordToLeaderboards(path, index);
                        isValid = true;
                    }
                    else
                    {
                        WriteLine("Error - the bet is outside the acceptable limits. Please press ENTER and try again.");
                    }
                    ReadKey();
                }
                else if (playersBet[index] > playersMoney[index])
                {
                    WriteLine("Error - your bet is more than how much money you have left. Please press ENTER and try again.");
                    ReadKey();
                }
                else
                {
                    isValid = true;
                }
            } while (isValid == false);
        }

        public static void CheckForNaturals()
        {
            dealerTotal = CalculateTotal(dealerHand);
            dealerHasNatural = dealerTotal == 21;
            foreach (int index in indicesOfPlayers)
            {
                playersNatural[index] = CalculateTotal((ArrayList)playersHand[index][0]) == 21;
                if (playersNatural[index])
                {
                    WriteLine($"{playersName[index]}, you have a blackjack!");
                    Write($"{playersName[index]}'s cards: ");
                    DisplayCards((ArrayList)playersHand[index][0]);
                    WriteLine();
                    if (dealerHasNatural)
                    {
                        WriteLine($"Stand-off - the dealer also has a blackjack. {playersName[index]}, you may take back your bet. Press ENTER to continue.");
                        Write($"Dealer's cards: ");
                        DisplayCards(dealerHand);
                        playersMoney[index] += playersBet[index];
                    }
                    else
                    {
                        WriteLine("You now collect 1.5x the amount of your bet! Press ENTER to continue.");
                        playersMoney[index] += (int)(playersBet[index] * 1.5);
                    }
                    ReadKey();
                }
                else if (dealerHasNatural)
                {
                    playersNatural[index] = false;
                    WriteLine($"The dealer has a blackjack, and you do not. The dealer now collects your bet, {playersName[index]}. Press ENTER to continue.");
                    Write($"Dealer's cards: ");
                    DisplayCards(dealerHand);
                    ReadKey();
                }
            }
        }

        public static void PlayerTurn(int index)
        {
            bool stand;
            bool bust;
            int choice;
            ArrayList[] hands;
            bool canSplit;
            bool canDoubleDown;
            bool willDoubleDown = false;
            bool pairOfAces;
            int total;
            string promptMessage;
            ValidateSplitting(index, out canSplit, out pairOfAces);
            ValidateDoublingDown(index, CalculateTotal((ArrayList)playersHand[index][0]), out canDoubleDown);
            if (canSplit)
            {
                promptMessage = $"You got a pair! {playersName[index]}, would you like your hand to be split into two separate hands? (0:Yes, 1:No) ";
                promptMessage += "\nYour cards: ";
                foreach (object card in (ArrayList)playersHand[index][0])
                {
                    int testVal = Convert.ToInt32(card);
                    if (testVal == 1)
                        promptMessage += "A ";
                    else if (testVal == 11)
                        promptMessage += "J ";
                    else if (testVal == 12)
                        promptMessage += "Q ";
                    else if (testVal == 13)
                        promptMessage += "K ";
                    else
                        promptMessage += card + " ";
                }
                if (pairOfAces)
                {
                    promptMessage += "\nIt was a pair of aces! Only ONE card would be dealt to each hand if you split.";
                }
                choice = RetrieveInput(promptMessage);
                if (choice == 0)
                {
                    hands = new ArrayList[2];
                    hands[0] = new ArrayList();
                    hands[1] = new ArrayList();
                    hands[0].Add(((ArrayList)playersHand[index][0])[0]);
                    hands[1].Add(((ArrayList)playersHand[index][0])[1]);
                    playersMoney[index] -= playersBet[index];
                }
                else if (choice == 1)
                {
                    hands = new ArrayList[1];
                    hands[0] = new ArrayList();
                    hands[0].Add(((ArrayList)playersHand[index][0])[0]);
                    hands[0].Add(((ArrayList)playersHand[index][0])[1]);
                }
                else
                {
                    WriteLine("Error - could not recognize choice. Continuing without splitting.");
                    hands = new ArrayList[1];
                    hands[0] = new ArrayList();
                    hands[0].Add(((ArrayList)playersHand[index][0])[0]);
                    hands[0].Add(((ArrayList)playersHand[index][0])[1]);
                }
            }
            else
            {
                hands = new ArrayList[1];
                hands[0] = new ArrayList();
                hands[0].Add(((ArrayList)playersHand[index][0])[0]);
                hands[0].Add(((ArrayList)playersHand[index][0])[1]);
            }
            if (canDoubleDown)
            {
                promptMessage = $"{playersName[index]}, your hand's value is between 9 and 11.";
                promptMessage += "\nWould you like to double down? (0:Yes, 1:No)";
                promptMessage += "\nYour cards: ";
                foreach (object card in (ArrayList)playersHand[index][0])
                {
                    int testVal = Convert.ToInt32(card);
                    if (testVal == 1)
                        promptMessage += "A ";
                    else if (testVal == 11)
                        promptMessage += "J ";
                    else if (testVal == 12)
                        promptMessage += "Q ";
                    else if (testVal == 13)
                        promptMessage += "K ";
                    else
                        promptMessage += card + " ";
                }
                choice = RetrieveInput(promptMessage);
                if (choice == 0)
                {
                    WriteLine("You have chosen to double down. Your bet has doubled, and you will be given a facedown card.\n" +
                        "This card will be revealed once all bets have been collected.");
                    for (int count = 0; count < hands.Length; ++count)
                    {
                        hands[count].Add(deck.Dequeue());
                    }
                    playersMoney[index] -= playersBet[index];
                    playersBet[index] += playersBet[index];
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

            playersHand[index].Clear();
            //For each hand...
            for (int hand = 0; hand < hands.Length; ++hand)
            {
                total = CalculateTotal(hands[hand]);
                stand = false;
                bust = false;
                do
                {
                    Clear();
                    WriteLine($"{playersName[index]}'s Turn");
                    WriteLine($"Your money: {playersMoney[index]:c2}");
                    WriteLine($"Your bet: {playersBet[index]:c2}");
                    Write($"Dealer's Face-up Card: ");
                    DisplayCard(dealerHand[0]);
                    Write("Your cards: ");
                    DisplayCards(hands[hand]);
                    if (pairOfAces)
                    {
                        hands[hand].Add(deck.Dequeue());
                        total = CalculateTotal(hands[hand]);
                        if (total == 21)
                        {
                            WriteLine("You got a blackjack on one of your aces - the payoff is equal to the bet.");
                            playersMoney[index] += playersBet[index];
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
                            Write("Card drawn: ");
                            DisplayCard(hands[hand][hands[hand].Count - 1]);
                            WriteLine($"Your new total is {total}.");
                            if (total > 21)
                            {
                                WriteLine("Bust! Your bet is collected by the dealer.");
                                bust = true;
                                playersBust[index] = true;
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
                        WriteLine("Please press ENTER to continue.");
                        ReadKey();
                    }
                } while (bust == false && stand == false);
                if (pairOfAces)
                {
                    total = CalculateTotal(hands[hand]);
                    if (total != 21)
                        playersHand[index].Add(hands[hand]);
                }
                else
                    playersHand[index].Add(hands[hand]);
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
                Write("Dealer's cards: ");
                DisplayCards(dealerHand);
                WriteLine($"Dealer's total value in hand: {total}.");
                foreach (int index in indicesOfPlayers)
                {
                    WriteLine($"{playersName[index]}'s money: {playersMoney[index]:c2}");
                    WriteLine($"{playersName[index]}'s bet: {playersBet[index]:c2}");
                    foreach (ArrayList hand in playersHand[index])
                    {
                        Write($"{playersName[index]}'s cards: ");
                        DisplayCards(hand);
                        WriteLine($"{playersName[index]}'s total value in hand: {CalculateTotal(hand)}");
                    }
                }
                if (total < 17)
                {
                    WriteLine("\nDealer will hit.");
                    Sleep(1500);
                    dealerHand.Add(deck.Dequeue());
                    Write($"Card drawn: ");
                    DisplayCard(dealerHand[dealerHand.Count - 1]);
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
                    Sleep(1500);
                }
                else
                {
                    WriteLine("\nDealer will stand.");
                    Sleep(1500);
                    stand = true;
                }
            } while (bust == false && stand == false);
        }

        public static int CalculateTotal(ArrayList hand)
        {
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

        public static void ValidateSplitting(int index, out bool canSplit, out bool pairOfAces)
        {
            if (Convert.ToInt32(((ArrayList)playersHand[index][0])[0]) == Convert.ToInt32(((ArrayList)playersHand[index][0])[1]))
            {
                if (playersBet[index] <= playersMoney[index])
                {
                    canSplit = true;
                }
                else
                {
                    canSplit = false;
                }
                if ((canSplit == true) && (Convert.ToInt32(((ArrayList)playersHand[index][0])[0]) == 1))
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

        public static void ValidateDoublingDown(int index, int total, out bool canDoubleDown)
        {
            if (total >= 9 && total <= 11)
            {
                if (playersBet[index] <= playersMoney[index])
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

        public static void ValidateBust(int total, out bool isBust)
        {
            if (total > 21)
            {
                isBust = true;
            }
            else
            {
                isBust = false;
            }
        }

        public static void AddRecordToLeaderboards(string path, int index)
        {
            ArrayList leaderboardRecords = new ArrayList();
            string newRecord = $"{playersName[index]}\t\t{playersMoney[index]}";
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
                        //For the last characters of this line
                        for (int c = lineChars.Length - 5; c < lineChars.Length; ++c)
                        {
                            if (char.IsNumber(lineChars[c]))
                            {
                                score += lineChars[c].ToString();
                            }
                        }
                        if (int.Parse(score) == playersMoney[index] || int.Parse(score) < playersMoney[index])
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

        public static int RetrieveInput(string promptMessage)
        {
            int input;
            bool isValid = false;

            do
            {
                Clear();
                WriteLine(promptMessage);
                Write(">> ");
                if (int.TryParse(ReadLine(), out input) == false)
                {
                    WriteLine("Error - incorrect format. Press ENTER to try again.");
                    ReadKey();
                }
                else
                {
                    isValid = true;
                }
            } while (isValid == false);
            return input;
        }

        public static void DisplayCards(ArrayList player)
        {
            foreach (object card in player)
            {
                int testVal = Convert.ToInt32(card);
                if (testVal == 1)
                    Write("A ");
                else if (testVal == 11)
                    Write("J ");
                else if (testVal == 12)
                    Write("Q ");
                else if (testVal == 13)
                    Write("K ");
                else
                    Write(card + " ");
            }
            WriteLine();
        }

        public static void DisplayCard(object card)
        {
            int testVal = Convert.ToInt32(card);
            if (testVal == 1)
                WriteLine("A");
            else if (testVal == 11)
                WriteLine("J");
            else if (testVal == 12)
                WriteLine("Q");
            else if (testVal == 13)
                WriteLine("K");
            else
                WriteLine(testVal);
        }
    }
}
