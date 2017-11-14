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
    class GoFishV2_1
    {
        //ERRORS:   

        //Defines each player's hand of cards, what their choices are for stealing cards from, and the number of 4-of-a-kinds each player
        //currently has during the game. The deck of cards to "go fish" from has also been defined and instantiated
        static Queue<int> deck = new Queue<int>();
        static ArrayList[] playersHand;
        static ArrayList[] playersPlayersToChoose;
        static int[] playersPoints;
        static Queue<int>[] playersMemoryPlayer;
        static Queue<int>[] playersMemoryCard;
        static string[] playersNames;
        static ArrayList[] playersAlreadyChosenPlayers;
        static ArrayList[] playersAlreadyChosenCards;
        //These variables are used as checks to see whether the player has followed the rules correctly, when the turns and game have ended,
        //and allows the other "players" to make choices
        static bool turnEnded;
        static bool playerSelected;
        static int playerNumber;
        static int cardNumber;
        static bool gameEnded;
        static int randomChoice;
        static Random rand;
        static string errorPlayerNumberToStealFrom = "Error - enter the player number that you would like to steal cards from: ";
        static string errorCardNumberToSteal = "Error - enter the card number you would like to steal: ";
        static string errorDoesNotHaveCardNumber = "Error - you cannot steal a card that you do not already have: ";
        static string errorPlayersOutsideLimits = "Error - choice is outside acceptable limits. Press ENTER and try again.";
        static string errorInvalidFormat = "Error - the choice is not in the correct format. Press ENTER and try again.";
        static string errorDifficultyDoesNotExist = "Error - this difficulty does not exist. Press ENTER and try again.";
        static int numberOfPlayers;
        static double WIN_MULTIPLIER = 2.307692307692308;//30 divided by 13
        static int MEMORY_CAPACITY;
        static bool player1StillInGame;
        static string[] potentialNames = { "Alex", "Brandon", "Caroline", "Demi", "Elliot", "Fred", "George", "Hannah", "Juliet", "Kelly", "Luna", "Mary",
            "Ned", "Oscar", "Penelope", "Quasimodo", "Rachel", "Sarah", "Timmy", "Ursinia", "Vanessa", "William", "X-Ray", "Yanessa", "Zanessa" };
        static int difficultySetting;

        public static void PlayGoFish(ArrayList deckOfCards)
        {
            player1StillInGame = true;
            bool isValid = false;

            do
            {
                WriteLine("Enter the number of players, INCLUDING YOU, there will be (2-10 players). Then, press ENTER to continue.");
                Write(">> ");
                if (int.TryParse(ReadLine(), out numberOfPlayers))
                {
                    if (numberOfPlayers <= 10 && numberOfPlayers >= 2)
                    {
                        isValid = true;
                    }
                    else
                    {
                        WriteLine(errorPlayersOutsideLimits);
                        ReadKey();
                        Clear();
                    }
                }
                else
                {
                    WriteLine(errorInvalidFormat);
                    ReadKey();
                    Clear();
                }
            }
            while (isValid == false);

            Clear();
            isValid = false;
            while (isValid == false)
            {
                WriteLine("Please choose a difficulty (0:Easy, 1:Medium, 2:Hard, 3:Insane), then press ENTER to continue.");
                Write(">> ");
                if (int.TryParse(ReadLine(), out difficultySetting))
                {
                    if (difficultySetting >= 0 && difficultySetting <= 3)
                        isValid = true;

                    else
                    {
                        WriteLine(errorDifficultyDoesNotExist);
                        ReadKey();
                        Clear();
                    }
                }
                else
                {
                    WriteLine(errorInvalidFormat);
                    ReadKey();
                    Clear();
                }
            }
            if (difficultySetting == 0)
                MEMORY_CAPACITY = 2;
            else if (difficultySetting == 1)
                MEMORY_CAPACITY = 3;
            else if (difficultySetting == 2)
                MEMORY_CAPACITY = 4;
            else
                MEMORY_CAPACITY = 100;

            Clear();
            WriteLine("Player 1, please enter your name, then press ENTER to begin playing.");
            Write(">> ");
            playersNames = new string[numberOfPlayers];
            playersNames[0] = ReadLine();
            Clear();

            for (int count = 0; count < deckOfCards.Count; ++count)
                deck.Enqueue(Convert.ToInt32(deckOfCards[count]));

            playersHand = new ArrayList[numberOfPlayers];
            playersPlayersToChoose = new ArrayList[numberOfPlayers];
            playersMemoryPlayer = new Queue<int>[numberOfPlayers];
            playersMemoryCard = new Queue<int>[numberOfPlayers];
            playersAlreadyChosenPlayers = new ArrayList[numberOfPlayers];
            playersAlreadyChosenCards = new ArrayList[numberOfPlayers];

            for (int player = 0; player < numberOfPlayers; ++player)
            {
                playersHand[player] = new ArrayList();
                playersPlayersToChoose[player] = new ArrayList();
                playersMemoryPlayer[player] = new Queue<int>();
                playersMemoryCard[player] = new Queue<int>();
                playersAlreadyChosenPlayers[player] = new ArrayList();
                playersAlreadyChosenCards[player] = new ArrayList();
                for (int opponent = 0; opponent < numberOfPlayers; ++opponent)
                    if (player != opponent)
                    {
                        playersPlayersToChoose[player].Add(opponent + 1);
                    }
            }

            playersPoints = new int[numberOfPlayers];

            //bot=0:Player 1
            //bot=1:Player 2...

            rand = new Random();
            ArrayList names = new ArrayList();
            for (int count = 0; count < potentialNames.Length; ++count)
            {
                names.Add(potentialNames[count]);
            }
            for (int name = 1; name < numberOfPlayers; ++name)
            {
                playersNames[name] = Convert.ToString(names[rand.Next(0, names.Count)]);
                names.Remove(playersNames[name]);
            }
            randomChoice = -1;

            DealCards(deckOfCards, numberOfPlayers);
            PlayCards();
        }

        public static void DealCards(ArrayList deckOfCards, int numOfPlayers)
        {
            if (numOfPlayers == 2)
            {
                for (int countCard = 0; countCard < 7; ++countCard)
                {
                    for (int countPlayer = 0; countPlayer < numberOfPlayers; ++countPlayer)
                        playersHand[countPlayer].Add(deck.Dequeue());
                }
            }
            else
            {
                for (int countCard = 0; countCard < 5; ++countCard)
                {
                    for (int countPlayer = 0; countPlayer < numberOfPlayers; ++countPlayer)
                        playersHand[countPlayer].Add(deck.Dequeue());
                }
            }
        }

        public static void PlayCards()
        {
            playersHand[0].Sort();
            gameEnded = false;
            int startingPlayer = rand.Next(0, numberOfPlayers);
            while (!gameEnded)
            {
                turnEnded = false;
                playerSelected = false;
                cardNumber = 0;
                turnEnded = false;
                for (int player = startingPlayer; player < numberOfPlayers; ++player)
                {
                    //0 = Player 1
                    PlayersTurn(player);
                    turnEnded = false;
                }
                startingPlayer = 0;
            }
            Clear();
            bool player1Wins = true;
            for (int bot = 1; bot < numberOfPlayers; ++bot)
            {
                if (playersPoints[0] < playersPoints[bot])
                    player1Wins = false;
            }

            if (player1Wins)
            {
                WriteLine($"{playersNames[0]} Wins!!!");
            }
            else
                WriteLine($"{playersNames[0]} Loses!!!");

            WriteLine();
            CheckFor4OfAKinds();
            if (player1Wins)
                playersPoints[0] = (int)(playersPoints[0] * WIN_MULTIPLIER);
            WriteLine("Press ENTER to continue");
            ReadKey();
            string path;
            if (difficultySetting == 0)
                path = $"Go Fish Leaderboards (Easy, {numberOfPlayers} Players).txt";
            else if (difficultySetting == 1)
                path = $"Go Fish Leaderboards (Medium, {numberOfPlayers} Players).txt";
            else if (difficultySetting == 2)
                path = $"Go Fish Leaderboards (Hard, {numberOfPlayers} Players).txt";
            else
                path = $"Go Fish Leaderboards (Insane, {numberOfPlayers} Players).txt";

            AddRecordToLeaderboards(path);
            Clear();
            WriteLine($"YOUR SCORE: {playersPoints[0]}\n");
            DisplayLeaderboards(path);
        }

        public static void CheckFor4OfAKinds()
        {
            //numberOfPlayers-1: number of bots
            //players_Hand[#].Count: number of cards for a particular player
            //players_Hand[#][#]: the card number for a particular player
            for (int player = 0; player < numberOfPlayers; ++player)
            {
                for (int count = 0; count < playersHand[player].Count; ++count)
                {
                    int card = Convert.ToInt32(playersHand[player][count]);
                    int frequency = 0;
                    for (int x = 0; x < playersHand[player].Count; ++x)
                    {
                        if (card == Convert.ToInt32(playersHand[player][x]))
                        {
                            ++frequency;
                        }
                    }
                    if (frequency == 4)
                    {
                        ++playersPoints[player];
                        for (int x = 0; x < 4; ++x)
                            playersHand[player].Remove(card);

                        for (int botNum = 1; botNum < numberOfPlayers; ++botNum)
                            RemoveBotMemory(botNum, card);

                        WriteLine($"{playersNames[player]} got a 4-of-a-kind!");
                        WriteLine();
                    }
                }
            }
            for (int player = 0; player < numberOfPlayers; ++player)
            {
                WriteLine($"{playersNames[player]}'s 4-of-a-kinds: {playersPoints[player]}");
            }
            WriteLine();
        }

        public static void PlayersTurn(int player)//0 = Player 1
        {
            //int player: 1 = Player 2
            //If the player has no cards in hand...
            if (playersHand[player].Count == 0)
            {
                //...and there are no cards in the deck, the player is out of the game
                if (deck.Count == 0)
                {
                    PlayerOutOfGame(player);
                    turnEnded = true;
                }
                //...but there are cards in the deck, the player draws five cards
                else
                {
                    for (int draw = 0; draw < 5; ++draw)
                    {
                        if (deck.Count > 0)
                        {
                            playersHand[player].Add(deck.Dequeue());
                        }
                    }
                }
            }
            cardNumber = -1;
            playerNumber = 0;
            randomChoice = -1;
            //Beginning of turn
            while (!turnEnded)
            {
                playersHand[0].Sort();
                Clear();
                int iteration = 0;
                //If the player has no cards in hand...
                if (playersHand[player].Count == 0)
                {
                    //...and there are no more cards in the deck, the player is out of the game
                    if (deck.Count == 0)
                        PlayerOutOfGame(player);
                    //...but there are cards in the deck, the player's turn has ended
                    turnEnded = true;
                }
                //The player has cards in hand
                else
                {
                    //If it is player 1's turn, allow player 1 to select the opponent he will attempt to steal from.
                    if (player == 0)
                    {
                        //Display the choices for the player
                        CheckFor4OfAKinds();
                        WriteLine("Your cards:");
                        DisplayCards();
                        WriteLine();
                        WriteLine($"{playersNames[player]}, choose a player to steal from.");

                        DisplayPlayerChoices();
                        Write(">> ");

                        playerSelected = false;

                        while (!playerSelected)
                        {
                            while (!int.TryParse(ReadLine(), out playerNumber))
                            {
                                WriteLine(errorPlayerNumberToStealFrom);
                                DisplayPlayerChoices();
                                Write(">> ");
                            }
                            if (playersPlayersToChoose[0].Count > 0)
                            {
                                for (int count = 0; count < playersPlayersToChoose[0].Count; ++count)
                                {
                                    if (playerNumber == Convert.ToInt32(playersPlayersToChoose[0][count]))
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
                                DisplayPlayerChoices();
                                Write(">> ");
                            }
                        }
                    }
                    //If the player is a bot, use either the bot's memory or a random number generator to make the bot's choices.
                    else
                    {
                        WriteLine("Your cards:");
                        DisplayCards();

                        WriteLine();
                        WriteLine($"{playersNames[player]}, choose a player to steal from.");
                        //If the bot remembers a player having a card that bot also has, then the bot will immediately ask that player for the card.
                        if (playersMemoryPlayer[player].Count > 0)
                        {
                            playerNumber = playersMemoryPlayer[player].Dequeue();
                            cardNumber = playersMemoryCard[player].Dequeue();
                        }
                        //The bot does not remember any player having a card in common, and will choose a player and card number randomly
                        else
                        {
                            randomChoice = rand.Next(0, playersPlayersToChoose[player].Count);
                            //If this is the bot's first attempt at stealing cards within this turn, then the bot may do so at random.
                            if (playersAlreadyChosenPlayers[player].Count == 0)
                            {
                                playerNumber = Convert.ToInt32(playersPlayersToChoose[player][randomChoice]);
                                randomChoice = rand.Next(0, playersHand[player].Count);
                                cardNumber = Convert.ToInt32(playersHand[player][randomChoice]);
                            }
                            //If this is not the bot's first attempt at stealing cards in this turn, 
                            //the bot knows to not call on the same players for the same cards within a turn
                            else
                            {
                                playerNumber = Convert.ToInt32(playersPlayersToChoose[player][randomChoice]);
                                randomChoice = rand.Next(0, playersHand[player].Count);
                                cardNumber = Convert.ToInt32(playersHand[player][randomChoice]);
                                bool restartLoop = true;
                                while (restartLoop)
                                {
                                    for (int count = 0; count < playersAlreadyChosenPlayers[player].Count; ++count)
                                    {
                                        if (playerNumber == Convert.ToInt32(playersAlreadyChosenPlayers[player][count]) && cardNumber == Convert.ToInt32(playersAlreadyChosenCards[player][count]))
                                        {
                                            randomChoice = rand.Next(0, playersPlayersToChoose[player].Count);
                                            playerNumber = Convert.ToInt32(playersPlayersToChoose[player][randomChoice]);
                                            randomChoice = rand.Next(0, playersHand[player].Count);
                                            cardNumber = Convert.ToInt32(playersHand[player][randomChoice]);
                                            restartLoop = true;
                                            break;
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
                        playersAlreadyChosenPlayers[player].Add(playerNumber);
                        playersAlreadyChosenCards[player].Add(cardNumber);
                        WriteLine($">> {playersNames[playerNumber - 1]}");
                    }

                    //If the player is player 1, allow the player to choose the card they would like to try and steal

                    if (player == 0)
                    {
                        WriteLine($"{playersNames[player]}, what card(s) would you like to steal from {playersNames[playerNumber - 1]}?");
                        Write(">> ");
                        bool hasCard = false;
                        while (!hasCard)
                        {
                            while (!int.TryParse(ReadLine(), out cardNumber))
                            {
                                Write(errorCardNumberToSteal);
                                DisplayCards();
                                Write(">> ");
                            }
                            for (int count = 0; count < playersHand[0].Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(playersHand[0][count]))
                                    hasCard = true;
                            }
                            if (hasCard == false)
                            {
                                Write(errorDoesNotHaveCardNumber);
                                Write(" ");
                                DisplayCards();
                                Write(">> ");
                            }
                        }
                        WriteLine();
                    }
                    else
                    {
                        if (player1StillInGame)
                            Sleep(2000);
                        WriteLine($"{playersNames[player]}, what card(s) would you like to steal from {playersNames[playerNumber - 1]}?");
                        WriteLine($">> {cardNumber}");
                        if (player1StillInGame)
                            Sleep(2000);
                        WriteLine();
                    }

                    int cardsStolen = 0;

                    for (int count = playersHand[playerNumber - 1].Count - 1; count >= 0; --count)
                    {
                        if (cardNumber == Convert.ToInt32(playersHand[playerNumber - 1][count]))
                        {
                            playersHand[player].Add(cardNumber);
                            playersHand[playerNumber - 1].Remove(cardNumber);
                            ++cardsStolen;
                        }
                    }

                    //If the player stole the rest of the loser's cards...
                    if (playersHand[playerNumber - 1].Count == 0)
                    {
                        //...and there are no more cards in the deck, Player 1 is out of the game.
                        if (deck.Count == 0)
                        {
                            PlayerOutOfGame(playerNumber - 1);
                        }
                    }

                    if (cardsStolen == 0)
                    {
                        WriteLine($"Go fish, {playersNames[player]}!");
                        if (deck.Count > 0)
                        {
                            playersHand[player].Add(deck.Dequeue());
                        }
                        else
                        {
                            WriteLine("There are no more cards in the deck.");
                        }
                        turnEnded = true;
                    }
                    else
                    {
                        RemoveBotMemory(playerNumber - 1, cardNumber);
                        if (cardsStolen == 1)
                            WriteLine($"{playersNames[player]} stole 1 card from {playersNames[playerNumber - 1]}!");
                        else
                            WriteLine($"{playersNames[player]} stole {cardsStolen} cards from {playersNames[playerNumber - 1]}!");
                    }
                    WriteLine();
                    CheckFor4OfAKinds();
                    if (player == 0)
                    {
                        ReadKey();
                    }
                    else
                    {
                        if (player1StillInGame)
                            Sleep(4000);
                    }
                    if (playersPoints.Sum() == 13)
                    {
                        gameEnded = true;
                        turnEnded = true;
                    }
                }
                InsertBotMemory(player); //0 = Player 1
            }
            if (deck.Count > 0)
            {
                playersAlreadyChosenPlayers[player].Clear();
                playersAlreadyChosenCards[player].Clear();
            }
            else
            {
                int remember;
                if (difficultySetting == 0)
                {
                    remember = rand.Next(0, 4);
                    if (remember != 0)
                    {
                        playersAlreadyChosenPlayers[player].Clear();
                        playersAlreadyChosenCards[player].Clear();
                    }
                }
                else if (difficultySetting == 1)
                {
                    remember = rand.Next(0, 2);
                    if (remember != 0)
                    {
                        playersAlreadyChosenPlayers[player].Clear();
                        playersAlreadyChosenCards[player].Clear();
                    }
                }
                else if (difficultySetting == 2)
                {
                    remember = rand.Next(0, 4);
                    if (remember == 0)
                    {
                        playersAlreadyChosenPlayers[player].Clear();
                        playersAlreadyChosenCards[player].Clear();
                    }
                }
            }
        }

        public static void AddRecordToLeaderboards(string path)
        {
            ArrayList leaderboardRecords = new ArrayList();
            string newRecord = $"{playersNames[0]}\t\t{playersPoints[0]}";
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
                        if (int.Parse(score) == playersPoints[0] || int.Parse(score) < playersPoints[0])
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
            if (difficultySetting == 0)
                WriteLine($"LEADERBOARD (Easy, {numberOfPlayers} Players)\n");
            else if (difficultySetting == 1)
                WriteLine($"LEADERBOARD (Medium, {numberOfPlayers} Players)\n");
            else if (difficultySetting == 2)
                WriteLine($"LEADERBOARD (Hard, {numberOfPlayers} Players)\n");
            else
                WriteLine($"LEADERBOARD (Insane, {numberOfPlayers} Players)\n");
            try
            {
                StreamReader read = new StreamReader(path);
                while (!read.EndOfStream)
                    WriteLine(read.ReadLine());
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
            ReadKey();
        }

        public static void DisplayAllLeaderboards()
        {
            string messagePressEnter = "Press ENTER to continue.";
            WriteLine("\tGO FISH\n");
            for (int x = 2; x < 11; ++x)
            {
                string path = $"Go Fish Leaderboards (Easy, {x} Players).txt";
                WriteLine($"LEADERBOARD (Easy, {x} Players)\n");
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
            WriteLine(messagePressEnter);
            ReadKey();
            Clear();
            WriteLine("\tGO FISH\n");
            for (int x = 2; x < 11; ++x)
            {
                string path = $"Go Fish Leaderboards (Medium, {x} Players).txt";
                WriteLine($"LEADERBOARD (Medium, {x} Players)\n");
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
            WriteLine(messagePressEnter);
            ReadKey();
            Clear();
            WriteLine("\tGO FISH\n");
            for (int x = 2; x < 11; ++x)
            {
                string path = $"Go Fish Leaderboards (Hard, {x} Players).txt";
                WriteLine($"LEADERBOARD (Hard, {x} Players)\n");
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
            WriteLine(messagePressEnter);
            ReadKey();
            Clear();
            WriteLine("\tGO FISH\n");
            for (int x = 2; x < 11; ++x)
            {
                string path = $"Go Fish Leaderboards (Insane, {x} Players).txt";
                WriteLine($"LEADERBOARD (Insane, {x} Players)\n");
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
            WriteLine(messagePressEnter);
            ReadKey();
            Clear();
        }

        public static void DisplayCards()
        {
            foreach (object val in playersHand[0])
                Write($"{val} ");
        }

        public static void DisplayPlayerChoices()
        {
            Write("Players: ");
            for (int player = 0; player < playersPlayersToChoose[0].Count; ++player)
            {
                int tempIndex = Convert.ToInt32(playersPlayersToChoose[0][player]) - 1;
                Write($"{tempIndex + 1}:{playersNames[tempIndex]} ");
            }
        }

        public static void PlayerOutOfGame(int player)
        {
            for (int botNum = 0; botNum < numberOfPlayers; ++botNum)
            {
                playersPlayersToChoose[botNum].Remove(player + 1);

                ArrayList memoryPlayer = new ArrayList();
                ArrayList memoryCard = new ArrayList();
                int countProperty = playersMemoryCard[botNum].Count;
                while (playersMemoryPlayer[botNum].Count > 0)
                {
                    memoryPlayer.Add(playersMemoryPlayer[botNum].Dequeue());
                    memoryCard.Add(playersMemoryCard[botNum].Dequeue());
                }
                for (int index = 0; index < countProperty; ++index)
                {
                    ArrayList indexToRemove = new ArrayList();
                    if (Convert.ToInt32(memoryPlayer[index]) == player + 1)
                    {
                        indexToRemove.Add(memoryPlayer.IndexOf(player + 1));
                    }
                    for (int a = indexToRemove.Count - 1; a > -1; --a)
                    {
                        memoryPlayer.Remove(player + 1);
                        memoryCard.RemoveAt(Convert.ToInt32(indexToRemove[a]));
                        --countProperty;
                    }

                }
                for (int index = 0; index < countProperty; ++index)
                {
                    playersMemoryPlayer[botNum].Enqueue(Convert.ToInt32(memoryPlayer[index]));
                    playersMemoryCard[botNum].Enqueue(Convert.ToInt32(memoryCard[index]));
                }
            }
            if (player == 0)
                player1StillInGame = false;
        }

        /// <summary>
        /// Gives the bots in the game a chance of remembering which player has a certain card that they possess.
        /// </summary>
        /// <param name="currentBot">This bot will not have its memory added to because it is the player that is currently making its play.</param>
        public static void InsertBotMemory(int currentBot)
        {
            for (int bot = 1; bot < numberOfPlayers; ++bot)//For each bot...
            {
                if (currentBot != bot)//The bot must not be the same bot who is currently on their turn.
                {
                    ArrayList players = new ArrayList();//instantiates two array lists - one for the player, and one for the card the player has.
                    ArrayList cards = new ArrayList();
                    while (playersMemoryPlayer[bot].Count > 0)//empties bot's memory, which is in the queue, into the array lists.
                    {
                        players.Add(playersMemoryPlayer[bot].Dequeue());
                        cards.Add(playersMemoryCard[bot].Dequeue());
                    }
                    for (int card = 0; card < playersHand[bot].Count; ++card)//for each card in the bot's hand...
                    {
                        bool doesRemember = false;
                        if (cardNumber == Convert.ToInt32(playersHand[bot][card]))//if the card number that was in the play 
                                                                                  //equals the card number in the bot's hand, then the bot is given a chance to remember that card and player.
                        {
                            int remember;
                            doesRemember = false;
                            if (difficultySetting == 0)
                            {
                                remember = rand.Next(0, 4);
                                if (remember == 1)
                                    doesRemember = true;
                            }
                            else if (difficultySetting == 1)
                            {
                                remember = rand.Next(0, 2);
                                if (remember == 1)
                                    doesRemember = true;
                            }
                            else if (difficultySetting == 2)
                            {
                                remember = rand.Next(0, 4);
                                if (remember != 1)
                                    doesRemember = true;
                            }
                            else
                                doesRemember = true;
                        }
                        if (doesRemember)
                        {
                            bool noDuplicates = true;
                            for (int count = 0; count < players.Count; ++count)//for each piece of memory...
                            {
                                //if the memory already exists, then there is a duplicate
                                if (Convert.ToInt32(players[count]) == (currentBot + 1) && Convert.ToInt32(cards[count]) == cardNumber)
                                    noDuplicates = false;
                            }
                            if (noDuplicates)//if there are no duplicates, add the memory
                            {
                                players.Add(currentBot + 1);
                                cards.Add(cardNumber);
                            }
                        }
                    }
                    for (int count = players.Count - 1; count >= 0; --count)//for each piece of memory, enter the memories back into the queues.
                    {
                        playersMemoryPlayer[bot].Enqueue(Convert.ToInt32(players[count]));
                        playersMemoryCard[bot].Enqueue(Convert.ToInt32(cards[count]));
                    }
                    if (playersMemoryPlayer[bot].Count > MEMORY_CAPACITY)
                    {
                        playersMemoryPlayer[bot].Dequeue();
                        playersMemoryCard[bot].Dequeue();
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
            //Empty the bot's memory into temporary storage
            while (playersMemoryPlayer[botLoser].Count > 0)
            {
                memoryPlayers.Add(playersMemoryPlayer[botLoser].Dequeue());
                memoryCards.Add(playersMemoryCard[botLoser].Dequeue());
            }
            int index = memoryCards.IndexOf(cardNumber);
            //Gets rid of memory based off of the presence of the card number
            while (index != -1)
            {
                memoryPlayers.RemoveAt(index);
                memoryCards.RemoveAt(index);
                index = memoryCards.IndexOf(cardNumber);
            }
            //Fills the bot's memory again
            for (int count = 0; count < memoryPlayers.Count; ++count)
            {
                playersMemoryPlayer[botLoser].Enqueue(Convert.ToInt32(memoryPlayers[count]));
                playersMemoryCard[botLoser].Enqueue(Convert.ToInt32(memoryCards[count]));
            }
        }
    }
}