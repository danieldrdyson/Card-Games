//Daniel Dyson
//This program receives the shuffled deck of cards from the CreateDeckOfCards class, and proceeds to start the game of Go Fish.
//Player 1 is the user, and more players are computer generated, where their choices of whom to steal cards from and what cards to steal are completely random,
//with the addition of some abilities to hold "memory" of which players have what cards in common.
//The player with the most 4-of-a-kinds at the end of the game is the winner!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Collections;
using System.IO;
using static System.Threading.Thread;

namespace DeckOfCards
{
    class GoFishV2
    {
        //ERRORS:   when a player loses a certain card, then their memory for attempting to steal that card should be wiped
        //          there is an unhandled exception involving one of the queues being emptied, possible trying to dequeue when you cannot (line 801?)
        //          when a four of a kind has been made, the memories should be wiped of that card


        //Defines each player's hand of cards, what their choices are for stealing cards from, and the number of 4-of-a-kinds each player
        //currently has during the game. The deck of cards to "go fish" from has also been defined and instantiated
        static ArrayList player1Hand;
        static ArrayList playersToChooseFrom1;
        static int player1Points;
        static ArrayList[] botsHand;
        static ArrayList[] botsPlayersToChoose;
        static int[] botsPoints;
        static Queue<int> deck = new Queue<int>();
        static Queue<int>[] botsMemoryPlayer;
        static Queue<int>[] botsMemoryCard;
        static string[] botsNames;

        //These variables are used as checks to see whether the player has followed the rules correctly, when the turns and game have ended,
        //and allows the other "players" to make choices
        static bool turnEnded;
        static bool playerSelected;
        static int playerNumber;
        static int cardNumber;
        static bool gameEnded;
        static int randomChoice;
        static Random rand;
        static string errorPlayerNumberToStealFrom = 
            "Error - enter the player number that you would like to steal cards from: ";
        static string errorCardNumberToSteal = 
            "Error - enter the card number you would like to steal: ";
        static string errorDoesNotHaveCardNumber = 
            "You cannot steal a card that you do not already have: ";
        static string name;
        static int numberOfPlayers;
        static double WIN_MULTIPLIER = 2.307692307692308;//30 divided by 13
        static int MEMORY_CAPACITY = 3;
        static bool player1StillInGame;
        static string[] potentialNames = { "Alex", "Brandon", "Caroline", "Demi", "Elliot", "Fred", "George", "Hannah", "Juliet", "Kelly", "Luna", "Mary",
            "Ned", "Oscar", "Penelope", "Quasimodo", "Rachel", "Sarah", "Timmy", "Ursinia", "Vanessa", "William", "X-Ray", "Yanessa", "Zanessa" };

        public static void PlayGoFish(ArrayList deckOfCards, int numOfPlayers)
        {
            numberOfPlayers = numOfPlayers;
            player1StillInGame = true;
            WriteLine("Player 1, please enter your name, then press ENTER.");
            Write(">> ");
            name = ReadLine();
            Clear();

            int numOfBots = numOfPlayers - 1;

            for (int count = 0; count < deckOfCards.Count; ++count)
                deck.Enqueue(Convert.ToInt32(deckOfCards[count]));

            player1Hand = new ArrayList();
            player1Points = 0;
            playersToChooseFrom1 = new ArrayList();

            botsHand = new ArrayList[numOfBots];
            botsPlayersToChoose = new ArrayList[numOfBots];
            botsMemoryPlayer = new Queue<int>[numOfBots];
            botsMemoryCard = new Queue<int>[numOfBots];

            for (int bot = 0; bot < numOfBots; ++bot)
            {
                botsHand[bot] = new ArrayList();
                botsPlayersToChoose[bot] = new ArrayList();
                botsMemoryPlayer[bot] = new Queue<int>();
                botsMemoryCard[bot] = new Queue<int>();
                playersToChooseFrom1.Add(bot + 2);
                for (int player = -1; player < botsHand.Length; ++player)
                    if (bot != player)
                    {
                        botsPlayersToChoose[bot].Add(player + 2);
                    }
            }

            botsPoints = new int[numOfBots];

            //bot=0:Player 2
            //bot=1:Player 3...

            rand = new Random();
            botsNames = new string[numOfBots];
            ArrayList names = new ArrayList();
            for (int count = 0; count < potentialNames.Length; ++count)
            {
                names.Add(potentialNames[count]);
            }
            for (int name = 0; name < numberOfPlayers - 1; ++name)
            {
                botsNames[name] = Convert.ToString(names[rand.Next(0, names.Count)]);
                names.Remove(botsNames[name]);
            }
            randomChoice = -1;

            DealCards(deckOfCards, numOfBots);
            PlayCards();
        }

        public static void DealCards(ArrayList deckOfCards, int numOfBots)
        {
            if (numOfBots == 1)
            {
                for (int count = 0; count < 7; ++count)
                {
                    player1Hand.Add(deck.Dequeue());
                }

                for (int countPlayers = 0; countPlayers < numOfBots; ++countPlayers)
                {
                    for (int count = 0; count < 7; ++count)
                    {
                        botsHand[countPlayers].Add(deck.Dequeue());
                    }
                }
            }
            else
            {
                for (int count = 0; count < 5; ++count)
                {
                    player1Hand.Add(deck.Dequeue());
                }

                for (int countPlayers = 0; countPlayers < numOfBots; ++countPlayers)
                {
                    for (int count = 0; count < 5; ++count)
                    {
                        botsHand[countPlayers].Add(deck.Dequeue());
                    }
                }
            }
        }

        public static void PlayCards()
        {
            gameEnded = false;
            CheckFor4OfAKinds();
            ReadKey();

            while (!gameEnded)
            {
                turnEnded = false;
                playerSelected = false;
                cardNumber = 0;
                Player1Turn();
                turnEnded = false;
                for (int bot = 0; bot < numberOfPlayers - 1; ++bot)
                {
                    //0 = Player 2
                    BotsTurn(bot);
                    turnEnded = false;
                }
            }
            Clear();
            bool player1Wins = true;
            for (int bot = 0; bot < botsHand.Length; ++bot)
            {
                if (player1Points < botsPoints[bot])
                {
                    player1Wins = false;
                }
            }

            if (player1Wins)
            {
                WriteLine($"{name} Wins!!!");
            }
            else
                WriteLine($"{name} Loses!!!");

            WriteLine();
            CheckFor4OfAKinds();
            if (player1Wins)
                player1Points = (int)(player1Points * WIN_MULTIPLIER);
            WriteLine("Press ENTER to continue");
            ReadKey();

            string path = $"Go Fish Leaderboards ({numberOfPlayers} Players).txt";
            ArrayList leaderboardRecords = new ArrayList();
            leaderboardRecords.Clear();
            string newRecord = $"{name}\t\t{player1Points}";
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
                        if (int.Parse(score) == player1Points || int.Parse(score) < player1Points)
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

            Clear();
            WriteLine($"YOUR SCORE: {player1Points}\n");
            DisplayLeaderboards(path);
            ReadKey();
        }

        public static void CheckFor4OfAKinds()
        {
            for (int count = 0; count < player1Hand.Count; ++count)
            {
                int card = Convert.ToInt32(player1Hand[count]);
                int frequency = 0;
                for (int x = 0; x < player1Hand.Count; ++x)
                {
                    if (card == Convert.ToInt32(player1Hand[x]))
                    {
                        ++frequency;
                    }
                }
                if (frequency == 4)
                {
                    ++player1Points;
                    for (int x = 0; x < 4; ++x)
                        player1Hand.Remove(card);
                    /*
                                        for (int bot = 0; bot < numberOfPlayers - 1; ++bot)
                                        {
                                            RemoveBotMemory();
                                        }*/
                    WriteLine($"{name} got a 4-of-a-kind!");
                    WriteLine();
                }
            }

            //botsHand.Length: number of bots
            //numberOfPlayers-1: number of bots
            //botsHand[#].Count: number of cards for a particular bot
            //botsHand[#][#]: the card number for a particular bot
            for (int bot = 0; bot < botsHand.Length; ++bot)
            {
                for (int count = 0; count < botsHand[bot].Count; ++count)
                {
                    int card = Convert.ToInt32(botsHand[bot][count]);
                    int frequency = 0;
                    for (int x = 0; x < botsHand[bot].Count; ++x)
                    {
                        if (card == Convert.ToInt32(botsHand[bot][x]))
                        {
                            ++frequency;
                        }
                    }
                    if (frequency == 4)
                    {
                        ++botsPoints[bot];
                        for (int x = 0; x < 4; ++x)
                            botsHand[bot].Remove(card);

                        for (int botNum = 0; botNum < numberOfPlayers - 1; ++botNum)
                        {
                            ArrayList memoryPlayer = new ArrayList();
                            ArrayList memoryCard = new ArrayList();
                            int countProperty = botsMemoryCard[botNum].Count;
                            for (int index = 0; index < countProperty; ++index)
                            {
                                memoryPlayer.Add(botsMemoryPlayer[botNum].Dequeue());
                                memoryCard.Add(botsMemoryCard[botNum].Dequeue());
                            }
                            for (int index = countProperty - 1; index >= 0; --index)
                            {
                                if (Convert.ToInt32(memoryCard[index]) == card)
                                {
                                    memoryPlayer.RemoveAt(index);
                                    memoryCard.RemoveAt(index);
                                }
                            }
                            for (int index = 0; index < memoryPlayer.Count; ++index)
                            {
                                botsMemoryPlayer[botNum].Enqueue(Convert.ToInt32(memoryPlayer[index]));
                                botsMemoryCard[botNum].Enqueue(Convert.ToInt32(memoryCard[index]));
                            }
                        }

                        WriteLine($"{botsNames[bot]} got a 4-of-a-kind!");
                        WriteLine();
                    }
                }
            }

            WriteLine($"{name}'s 4-of-a-kinds: {player1Points}");
            for (int bot = 0; bot < botsHand.Length; ++bot)
            {
                WriteLine($"{botsNames[bot]}'s 4-of-a-kinds: {botsPoints[bot]}");
            }

            WriteLine();
        }

        public static void Player1Turn()
        {
            if (player1Hand.Count == 0)
            {
                if (deck.Count == 0)
                {
                    Player1OutOfGame();
                    turnEnded = true;
                }
                else
                {
                    for (int draw = 0; draw < 5; ++draw)
                    {
                        if (deck.Count > 0)
                        {
                            player1Hand.Add(deck.Dequeue());
                        }
                    }
                }
            }
            while (!turnEnded)
            {
                Clear();
                if (player1Hand.Count == 0)
                {
                    if (deck.Count == 0)
                    {
                        Player1OutOfGame();
                    }
                    turnEnded = true;
                }
                else
                {
                    WriteLine("Your cards:");
                    foreach (object val in player1Hand)
                        Write($"{val} ");

                    WriteLine();
                    WriteLine($"{name}, choose a player to steal from.");
                    int names = 0;
                    foreach (object val in playersToChooseFrom1)
                    {
                        Write($"{botsNames[names]} ({val}), ");
                        ++names;
                    }
                    Write(">> ");

                    bool hasCard = false;
                    playerSelected = false;

                    while (!playerSelected)
                    {
                        while (!int.TryParse(ReadLine(), out playerNumber))
                        {
                            WriteLine(errorPlayerNumberToStealFrom);
                            Write("Players: ");
                            int namess = 0;
                            foreach (object val in playersToChooseFrom1)
                            {
                                Write($"{botsNames[namess]} ({val}), ");
                                ++namess;
                            }
                            Write(">> ");
                        }
                        if (playersToChooseFrom1.Count > 0)
                        {
                            for (int count = 0; count < playersToChooseFrom1.Count; ++count)
                            {
                                if (playerNumber == Convert.ToInt32(playersToChooseFrom1[count]))
                                    playerSelected = true;
                            }
                        }
                        else
                        {
                            playerSelected = true;
                        }
                        if (!playerSelected)
                        {
                            WriteLine(errorPlayerNumberToStealFrom);
                            Write("Players: ");
                            int namess = 0;
                            foreach (object val in playersToChooseFrom1)
                            {
                                Write($"{botsNames[namess]} ({val}), ");
                                ++namess;
                            }
                            Write(">> ");
                        }
                    }

                    WriteLine($"{name}, what card(s) would you like to steal from {botsNames[playerNumber - 2]}?");
                    Write(">> ");

                    while (!hasCard)
                    {
                        while (!int.TryParse(ReadLine(), out cardNumber))
                            Write(errorCardNumberToSteal);
                        WriteLine();
                        for (int count = 0; count < player1Hand.Count; ++count)
                        {
                            if (cardNumber == Convert.ToInt32(player1Hand[count]))
                                hasCard = true;
                        }
                        if (hasCard)
                        {
                            int cardsStolen = 0;

                            for (int bot = 0; bot < numberOfPlayers - 1; ++bot)
                            {
                                if (playerNumber == bot + 2)
                                {
                                    for (int cycle = 0; cycle < 3; ++cycle)
                                    {
                                        for (int count = 0; count < botsHand[bot].Count; ++count)
                                        {
                                            if (cardNumber == Convert.ToInt32(botsHand[bot][count]))
                                            {
                                                player1Hand.Add(cardNumber);
                                                botsHand[bot].Remove(cardNumber);
                                                ++cardsStolen;
                                            }
                                        }
                                    }
                                }
                            }

                            if (cardsStolen == 0)
                            {
                                WriteLine($"Go fish, {name}!");
                                if (deck.Count > 0)
                                {
                                    player1Hand.Add(deck.Dequeue());
                                }
                                else
                                {
                                    WriteLine("There are no more cards in the deck.");
                                }
                                turnEnded = true;
                            }
                            else
                            {
                                RemoveBotMemory(playerNumber - 2, cardNumber);

                                if (cardsStolen == 1)
                                    WriteLine($"{name} stole 1 card from {botsNames[playerNumber - 2]}!");
                                else
                                    WriteLine($"{name} stole {cardsStolen} cards from {botsNames[playerNumber - 2]}!");
                            }
                            WriteLine();
                            CheckFor4OfAKinds();
                            if (player1Points + botsPoints.Sum() == 13)
                            {
                                gameEnded = true;
                                turnEnded = true;
                            }
                        }
                        else
                        {
                            Write(errorDoesNotHaveCardNumber);
                        }
                    }
                    ReadKey();
                }
                InsertBotMemory(-1);
            }
        }

        public static void BotsTurn(int bot)//0 = Player 2
        {
            //If the bot has no cards in hand...
            if (botsHand[bot].Count == 0)
            {
                //...and there are no cards in the deck, the bot is out of the game
                if (deck.Count == 0)
                {
                    BotOutOfGame(bot);
                    turnEnded = true;
                }
                //...but there are cards in the deck, the bot draws one card
                else
                {
                    for (int draw = 0; draw < 5; ++draw)
                    {
                        if (deck.Count > 0)
                        {
                            botsHand[bot].Add(deck.Dequeue());
                        }
                    }
                }
            }
            cardNumber = -1;
            playerNumber = 0;
            randomChoice = -1;
            ArrayList players = new ArrayList();
            ArrayList cards = new ArrayList();
            //Beginning of turn
            while (!turnEnded)
            {
                Clear();
                int iteration = 0;
                //If the bot has no cards in hand...
                if (botsHand[bot].Count == 0)
                {
                    //...and there are no more cards in the deck, the bot is out of the game
                    if (deck.Count == 0)
                    {
                        BotOutOfGame(bot);
                    }
                    //...but there are cards in the deck, the bots turn has ended
                    turnEnded = true;
                }
                //The bot has cards in hand
                else
                {
                    //If the bot remembers a player having a card that bot also has, then the bot will immediately ask that player for the card.
                    if (botsMemoryPlayer[bot].Count > 0)
                    {
                        ///have a check to see if the player number from memory matches with actual options for player choices
                        ///error here - bots tend to select themselves
                        playerNumber = botsMemoryPlayer[bot].Dequeue();
                        cardNumber = botsMemoryCard[bot].Dequeue();
                    }
                    //The bot does not remember any player having a card in common, and will choose a player and card number randomly
                    else
                    {
                        randomChoice = rand.Next(0, botsPlayersToChoose[bot].Count);
                        //If this is the bot's first attempt at stealing cards within this turn, then the bot may do so at random.
                        if (players.Count == 0)
                        {
                            playerNumber = Convert.ToInt32(botsPlayersToChoose[bot][randomChoice]);
                            randomChoice = rand.Next(0, botsHand[bot].Count);
                            cardNumber = Convert.ToInt32(botsHand[bot][randomChoice]);
                        }
                        //If this is not the bot's first attempt at stealing cards in this turn, 
                        //the bot knows to not call on the same players for the same cards within a turn
                        else
                        {
                            playerNumber = Convert.ToInt32(botsPlayersToChoose[bot][randomChoice]);
                            randomChoice = rand.Next(0, botsHand[bot].Count);
                            cardNumber = Convert.ToInt32(botsHand[bot][randomChoice]);
                            bool restartLoop = true;
                            while (restartLoop)
                            {
                                for (int count = 0; count < players.Count; ++count)
                                {
                                    if (playerNumber == Convert.ToInt32(players[count]) && cardNumber == Convert.ToInt32(cards[count]))
                                    {
                                        randomChoice = rand.Next(0, botsHand[bot].Count);
                                        cardNumber = Convert.ToInt32(botsHand[bot][randomChoice]);
                                        restartLoop = true;
                                        continue;
                                    }
                                    else
                                        restartLoop = false;
                                }
                                ++iteration;
                                //If the bot cannot make a rational decision in 100 tries, 
                                //then the program moves on with the last random choice of player and card
                                if (iteration > 100)
                                    break;
                            }
                        }
                    }

                    players.Add(playerNumber);
                    cards.Add(cardNumber);
                    WriteLine("Your cards:");
                    foreach (object val in player1Hand)
                        Write($"{val} ");

                    WriteLine();
                    WriteLine($"{botsNames[bot]}, choose a player to steal from.");
                    //If the bot chooses Player 1, substitue the "1" with Player 1's name.
                    if (playerNumber == 1)
                        WriteLine($">> {name}");
                    //If the bot chose another bot, then just display name as "Player #."
                    else
                        WriteLine($">> {botsNames[playerNumber - 2]}");
                    if (player1StillInGame)
                        Sleep(2000);
                    if (playerNumber == 1)
                        WriteLine($"{botsNames[bot]}, what card(s) would you like to steal from {name}?");
                    else
                        WriteLine($"{botsNames[bot]}, what card(s) would you like to steal from {botsNames[playerNumber - 2]}?");
                    WriteLine($">> {cardNumber}");
                    if (player1StillInGame)
                        Sleep(2000);
                    WriteLine();

                    int cardsStolen = 0;
                    //If the bot chose to steal cards from Player 1, then check for any cards in common, and give the bot the cards from Player 1.
                    if (playerNumber == 1)
                    {
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {
                            for (int count = 0; count < player1Hand.Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(player1Hand[count]))
                                {
                                    botsHand[bot].Add(cardNumber);
                                    player1Hand.Remove(cardNumber);
                                    ++cardsStolen;
                                }
                            }
                        }
                        //If the bot stole the rest of Player 1's cards...
                        if (player1Hand.Count == 0)
                        {
                            //...and there are no more cards in the deck, Player 1 is out of the game.
                            if (deck.Count == 0)
                            {
                                Player1OutOfGame();
                            }
                        }
                    }
                    else
                    {
                        //playerNumber-2==botNum
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {

                            for (int count = 0; count < botsHand[playerNumber - 2].Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(botsHand[playerNumber - 2][count]))
                                {
                                    //More efficient way of adding and removing cards???
                                    botsHand[bot].Add(cardNumber);
                                    botsHand[playerNumber - 2].Remove(cardNumber);
                                    ++cardsStolen;
                                }

                                RemoveBotMemory(playerNumber - 2, cardNumber);
                            }
                        }
                    }

                    if (cardsStolen == 0)
                    {
                        WriteLine($"Go fish, {botsNames[bot]}!");
                        if (deck.Count > 0)
                        {
                            botsHand[bot].Add(deck.Dequeue());
                        }
                        else
                        {
                            WriteLine("There are no more cards in the deck.");
                        }
                        turnEnded = true;
                    }
                    else
                    {
                        if (playerNumber == 1)
                        {
                            if (cardsStolen == 1)
                                WriteLine($"{botsNames[bot]} stole 1 card from {name}!");
                            else
                                WriteLine($"{botsNames[bot]} stole {cardsStolen} cards from {name}!");
                        }
                        else
                        {
                            if (cardsStolen == 1)
                                WriteLine($"{botsNames[bot]} stole 1 card from {botsNames[playerNumber - 2]}!");
                            else
                                WriteLine($"{botsNames[bot]} stole {cardsStolen} cards from {botsNames[playerNumber - 2]}!");
                        }
                    }
                    WriteLine();
                    CheckFor4OfAKinds();
                    if (player1StillInGame)
                        Sleep(4000);
                    if (player1Points + botsPoints.Sum() == 13)
                    {
                        gameEnded = true;
                        turnEnded = true;
                    }
                }
                InsertBotMemory(bot);//0 = Player 2
            }
        }

        public static void DisplayLeaderboards(string path)
        {
            WriteLine($"LEADERBOARD ({numberOfPlayers} Players)\n");
            try
            {
                StreamReader read = new StreamReader(path);
                while (!read.EndOfStream)
                {
                    WriteLine(read.ReadLine());
                }
                read.Close();
            }
            catch (FileNotFoundException)
            {
                WriteLine("Error - no leaderboard stats exist yet for this game");
            }
            catch (Exception)
            {
                WriteLine("Error - could not load leaderboard stats");
            }
        }

        public static void DisplayAllLeaderboards()
        {
            for (int x = 2; x < 8; ++x)
            {
                string path = $"Go Fish Leaderboards ({x} Players).txt";
                WriteLine($"LEADERBOARD ({x} Players)\n");
                try
                {
                    StreamReader read = new StreamReader(path);
                    while (!read.EndOfStream)
                    {
                        WriteLine(read.ReadLine());
                    }
                    read.Close();
                }
                catch (FileNotFoundException)
                {
                    WriteLine("Error - no leaderboard stats exist yet for this game");
                }
                catch (Exception)
                {
                    WriteLine("Error - could not load leaderboard stats");
                }
                WriteLine();
            }
        }

        public static void Player1OutOfGame()
        {
            for (int bot = 0; bot < botsHand.Length; ++bot)
            {
                botsPlayersToChoose[bot].Remove(1);

                ArrayList memoryPlayer = new ArrayList();
                ArrayList memoryCard = new ArrayList();
                int countProperty = botsMemoryCard[bot].Count;
                for (int index = 0; index < countProperty; ++index)
                {
                    memoryPlayer.Add(botsMemoryPlayer[bot].Dequeue());
                    memoryCard.Add(botsMemoryCard[bot].Dequeue());
                }
                for (int index = 0; index < countProperty; ++index)
                {
                    ArrayList indexToRemove = new ArrayList();
                    if (Convert.ToInt32(memoryPlayer[index]) == bot + 2)
                    {
                        indexToRemove.Add(memoryPlayer.IndexOf(bot + 2));
                    }
                    for (int a = indexToRemove.Count - 1; a > -1; --a)
                    {
                        memoryPlayer.Remove(bot + 2);
                        memoryCard.RemoveAt(Convert.ToInt32(indexToRemove[a]));
                        --countProperty;
                    }

                }

                for (int index = 0; index < countProperty; ++index)
                {
                    botsMemoryPlayer[bot].Enqueue(Convert.ToInt32(memoryPlayer[index]));
                    botsMemoryCard[bot].Enqueue(Convert.ToInt32(memoryCard[index]));
                }
                player1StillInGame = false;
            }
        }

        public static void BotOutOfGame(int bot)
        {
            playersToChooseFrom1.Remove(bot + 2);
            for (int botNum = 0; botNum < botsHand.Length; ++botNum)
            {
                botsPlayersToChoose[botNum].Remove(bot + 2);

                ArrayList memoryPlayer = new ArrayList();
                ArrayList memoryCard = new ArrayList();
                int countProperty = botsMemoryCard[bot].Count;
                for (int index = 0; index < countProperty; ++index)
                {
                    memoryPlayer.Add(botsMemoryPlayer[botNum].Dequeue());
                    memoryCard.Add(botsMemoryCard[botNum].Dequeue());
                }
                for (int index = 0; index < countProperty; ++index)
                {
                    ArrayList indexToRemove = new ArrayList();
                    if (Convert.ToInt32(memoryPlayer[index]) == bot + 2)
                    {
                        indexToRemove.Add(memoryPlayer.IndexOf(bot + 2));
                    }
                    for (int a = indexToRemove.Count - 1; a > -1; --a)
                    {
                        memoryPlayer.Remove(bot + 2);
                        memoryCard.RemoveAt(Convert.ToInt32(indexToRemove[a]));
                        --countProperty;
                    }

                }

                for (int index = 0; index < countProperty; ++index)
                {
                    botsMemoryPlayer[botNum].Enqueue(Convert.ToInt32(memoryPlayer[index]));
                    botsMemoryCard[botNum].Enqueue(Convert.ToInt32(memoryCard[index]));
                }
            }
        }

        public static void InsertBotMemory(int currentBot)
        {
            for (int bot = 0; bot < numberOfPlayers - 1; ++bot)
            {
                if (currentBot != bot)
                {
                    ArrayList players = new ArrayList();
                    ArrayList cards = new ArrayList();
                    while (botsMemoryPlayer[bot].Count > 0)
                    {
                        players.Add(botsMemoryPlayer[bot].Dequeue());
                        cards.Add(botsMemoryCard[bot].Dequeue());
                    }
                    for (int card = 0; card < botsHand[bot].Count; ++card)
                    {
                        if (cardNumber == Convert.ToInt32(botsHand[bot][card]))
                        {
                            int remember = rand.Next(0, 2);
                            if (remember == 1)
                            {
                                bool noDuplicates = true;
                                for (int count = 0; count < players.Count; ++count)
                                {
                                    if (Convert.ToInt32(players[count]) == (currentBot + 2) && Convert.ToInt32(cards[count]) == cardNumber)
                                    {
                                        noDuplicates = false;
                                    }
                                }
                                for (int count = 0; count < players.Count; ++count)
                                {
                                    botsMemoryPlayer[bot].Enqueue(Convert.ToInt32(players[count]));
                                    botsMemoryCard[bot].Enqueue(Convert.ToInt32(cards[count]));
                                }
                                if (noDuplicates)
                                {
                                    botsMemoryPlayer[bot].Enqueue(currentBot + 2);
                                    botsMemoryCard[bot].Enqueue(cardNumber);
                                }
                                if (botsMemoryPlayer[bot].Count > MEMORY_CAPACITY)
                                {
                                    botsMemoryPlayer[bot].Dequeue();
                                    botsMemoryCard[bot].Dequeue();
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void RemoveBotMemory(int botLoser, int cardNumber)
        {//Bots' memories should only have things removed if:
         //someone makes a four of a kind, and a bot was going to ask for that card
         //the bot has had cards stolen from them, and they were going to ask for that card from a player

            //For each card in the loser's hand
            ArrayList memoryPlayers = new ArrayList();
            ArrayList memoryCards = new ArrayList();
            while (botsMemoryPlayer[botLoser].Count > 0)
            {
                memoryPlayers.Add(botsMemoryPlayer[botLoser].Dequeue());
                memoryCards.Add(botsMemoryCard[botLoser].Dequeue());
            }
            int index = memoryCards.IndexOf(cardNumber);
            while (index != -1)
            {
                memoryPlayers.RemoveAt(index);
                memoryCards.RemoveAt(index);
                index = memoryCards.IndexOf(cardNumber);
            }
            for (int count = 0; count < memoryPlayers.Count; ++count)
            {
                botsMemoryPlayer[botLoser].Enqueue(Convert.ToInt32(memoryPlayers[count]));
                botsMemoryCard[botLoser].Enqueue(Convert.ToInt32(memoryCards[count]));
            }
        }
    }
}