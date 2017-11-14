//
//This program receives the shuffled deck of cards from the CreateDeckOfCards class, and proceeds to start the game of Go Fish.
//Player 1 is the user, and Players 2, 3, and 4 are computer generated, where their choices of whom to steal cards from and what cards to steal are completely random
//The player with the most 4-of-a-kinds at the end of the game is the winner!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Collections;
using System.IO;

namespace DeckOfCards
{
    class GoFish
    {
        //Defines each player's hand of cards, what their choices are for stealing cards from, and the number of 4-of-a-kinds each player
        //currently has during the game. The deck of cards to "go fish" from has also been defined and instantiated
        static ArrayList player1Hand;
        static ArrayList playersToChooseFrom1;
        static int player1Points;
        static ArrayList player2Hand;
        static ArrayList playersToChooseFrom2;
        static int player2Points;
        static ArrayList player3Hand;
        static ArrayList playersToChooseFrom3;
        static int player3Points;
        static ArrayList player4Hand;
        static ArrayList playersToChooseFrom4;
        static int player4Points;
        static Queue<int> deck = new Queue<int>();

        //These variables are used as checks to see whether the player has followed the rules correctly, when the turns and game have ended,
        //and allows the other "players" to make choices
        static bool turnEnded;
        static bool playerSelected;
        static int playerNumber;
        static int cardNumber;
        static bool gameEnded;
        static int randomChoice;
        static Random rand;
        static string errorPlayerNumberToStealFrom = "Error - enter the player number that you would like to steal cards from";
        static string errorCardNumberToSteal = "Error - enter the card number you would like to steal >> ";
        static string errorDoesNotHaveCardNumber = "You cannot steal a card that you do not already have >> ";

        public static void PlayGoFish(ArrayList deckOfCards)
        {
            player1Hand = new ArrayList();
            player1Points = 0;
            player2Hand = new ArrayList();
            player2Points = 0;
            player3Hand = new ArrayList();
            player3Points = 0;
            player4Hand = new ArrayList();
            player4Points = 0;
            playersToChooseFrom1 = new ArrayList();
            playersToChooseFrom1.Add(2);
            playersToChooseFrom1.Add(3);
            playersToChooseFrom1.Add(4);
            playersToChooseFrom2 = new ArrayList();
            playersToChooseFrom2.Add(1);
            playersToChooseFrom2.Add(3);
            playersToChooseFrom2.Add(4);
            playersToChooseFrom3 = new ArrayList();
            playersToChooseFrom3.Add(1);
            playersToChooseFrom3.Add(2);
            playersToChooseFrom3.Add(4);
            playersToChooseFrom4 = new ArrayList();
            playersToChooseFrom4.Add(1);
            playersToChooseFrom4.Add(2);
            playersToChooseFrom4.Add(3);
            rand = new Random();
            randomChoice = -1;
            DealCards(deckOfCards);
            PlayCards();
        }

        public static void DealCards(ArrayList deckOfCards)
        {
            for (int count = 0; count < 7; ++count)
            {
                player1Hand.Add(deckOfCards[count]);
            }
            for (int count = 7; count < 14; ++count)
            {
                player2Hand.Add(deckOfCards[count]);
            }
            for (int count = 14; count < 21; ++count)
            {
                player3Hand.Add(deckOfCards[count]);
            }
            for (int count = 21; count < 28; ++count)
            {
                player4Hand.Add(deckOfCards[count]);
            }
            for (int count = 28; count < 52; ++count)
            {
                deck.Enqueue(Convert.ToInt32(deckOfCards[count]));
            }
        }

        public static void PlayCards()
        {
            //StreamWriter write;
            int gameNumber = 1;
            while (File.Exists($"Go Fish #{gameNumber}.txt"))
            {
                ++gameNumber;
            }

            gameEnded = false;
            CheckFor4OfAKinds();
            ReadKey();

            while (!gameEnded)
            {
                //write = new StreamWriter($"Go Fish #{gameNumber}.txt", append: true);
                turnEnded = false;
                playerSelected = false;
                cardNumber = 0;

                Player1Turn();

                turnEnded = false;
                Player2Turn();

                turnEnded = false;
                Player3Turn();

                turnEnded = false;

                Player4Turn();
                Clear();
                //write.Close();

            }
            if (player1Points > player2Points && player1Points > player3Points && player1Points > player4Points)
                WriteLine("Player 1 Wins!!!");
            else if (player2Points > player1Points && player2Points > player3Points && player2Points > player4Points)
                WriteLine("Player 2 Wins!!!");
            else if (player3Points > player1Points && player3Points > player2Points && player3Points > player4Points)
                WriteLine("Player 3 Wins!!!");
            else if (player4Points > player1Points && player4Points > player2Points && player4Points > player3Points)
                WriteLine("Player 4 Wins!!!");
            else
                WriteLine("Tie Game!!!");

            WriteLine();
            CheckFor4OfAKinds();
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
                    {
                        player1Hand.Remove(card);
                    }
                    WriteLine("Player 1 got a 4-of-a-kind!");
                    WriteLine();
                }
            }


            for (int count = 0; count < player2Hand.Count; ++count)
            {
                int card = Convert.ToInt32(player2Hand[count]);
                int frequency = 0;
                for (int x = 0; x < player2Hand.Count; ++x)
                {
                    if (card == Convert.ToInt32(player2Hand[x]))
                    {
                        ++frequency;
                    }
                }
                if (frequency == 4)
                {
                    ++player2Points;
                    for (int x = 0; x < 4; ++x)
                    {
                        player2Hand.Remove(card);
                    }
                    WriteLine("Player 2 got a 4-of-a-kind!");
                    WriteLine();
                }
            }


            for (int count = 0; count < player3Hand.Count; ++count)
            {
                int card = Convert.ToInt32(player3Hand[count]);
                int frequency = 0;
                for (int x = 0; x < player3Hand.Count; ++x)
                {
                    if (card == Convert.ToInt32(player3Hand[x]))
                    {
                        ++frequency;
                    }
                }
                if (frequency == 4)
                {
                    ++player3Points;
                    for (int x = 0; x < 4; ++x)
                    {
                        player3Hand.Remove(card);
                    }
                    WriteLine("Player 3 got a 4-of-a-kind!");
                    WriteLine();
                }
            }


            for (int count = 0; count < player4Hand.Count; ++count)
            {
                int card = Convert.ToInt32(player4Hand[count]);
                int frequency = 0;
                for (int x = 0; x < player4Hand.Count; ++x)
                {
                    if (card == Convert.ToInt32(player4Hand[x]))
                    {
                        ++frequency;
                    }
                }
                if (frequency == 4)
                {
                    ++player4Points;
                    for (int x = 0; x < 4; ++x)
                    {
                        player4Hand.Remove(card);
                    }
                    WriteLine("Player 4 got a 4-of-a-kind!");
                    WriteLine();
                }
            }

            WriteLine($"Player 1's 4-of-a-kinds: {player1Points}");
            WriteLine($"Player 2's 4-of-a-kinds: {player2Points}");
            WriteLine($"Player 3's 4-of-a-kinds: {player3Points}");
            WriteLine($"Player 4's 4-of-a-kinds: {player4Points}");

            WriteLine();
        }

        public static void Player1Turn()
        {
            if (player1Hand.Count == 0)
            {
                turnEnded = true;
                playersToChooseFrom2.Remove(1);
                playersToChooseFrom3.Remove(1);
                playersToChooseFrom4.Remove(1);
            }
            while (!turnEnded)
            {
                Clear();
                if (player1Hand.Count == 0)
                    turnEnded = true;
                else
                {
                    WriteLine("Your cards:");
                    foreach (object val in player1Hand)
                    {
                        Write($"{val} ");
                        //write.Write($"{val} ");
                    }
                    WriteLine();
                    //write.WriteLine();
                    bool hasCard = false;
                    WriteLine("Player 1, choose a player to steal from");
                    playerSelected = false;
                    Write(">> ");
                    while (!playerSelected)
                    {
                        while (!int.TryParse(ReadLine(), out playerNumber))
                        {
                            WriteLine(errorPlayerNumberToStealFrom);
                            Write("Players: ");
                            foreach (object val in playersToChooseFrom1)
                                Write($"{val} ");
                            Write(">> ");
                        }
                        for (int count = 0; count < playersToChooseFrom1.Count; ++count)
                        {
                            if (playerNumber == Convert.ToInt32(playersToChooseFrom1[count]))
                                playerSelected = true;
                        }
                        if (!playerSelected)
                        {
                            WriteLine(errorPlayerNumberToStealFrom);
                            Write("Players: ");
                            foreach (object val in playersToChooseFrom1)
                                Write($"{val} ");
                            Write(">> ");
                        }
                    }
                    WriteLine();
                    WriteLine($"Player 1, what cards would you like to steal from Player {playerNumber}?");
                    WriteLine();
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
                            ArrayList ind = new ArrayList();
                            int cardsStolen = 0;
                            if (playerNumber == 2)
                            {
                                for (int cycle = 0; cycle < 3; ++cycle)
                                {
                                    for (int count = 0; count < player2Hand.Count; ++count)
                                    {
                                        if (cardNumber == Convert.ToInt32(player2Hand[count]))
                                        {
                                            player1Hand.Add(cardNumber);
                                            player2Hand.Remove(cardNumber);
                                            ++cardsStolen;
                                        }
                                    }
                                }
                            }
                            else if (playerNumber == 3)
                            {
                                for (int cycle = 0; cycle < 3; ++cycle)
                                {
                                    for (int count = 0; count < player3Hand.Count; ++count)
                                    {
                                        if (cardNumber == Convert.ToInt32(player3Hand[count]))
                                        {
                                            player1Hand.Add(cardNumber);
                                            player3Hand.Remove(cardNumber);
                                            ++cardsStolen;
                                        }
                                    }
                                }
                            }
                            else if (playerNumber == 4)
                            {
                                for (int cycle = 0; cycle < 3; ++cycle)
                                {
                                    for (int count = 0; count < player4Hand.Count; ++count)
                                    {
                                        if (cardNumber == Convert.ToInt32(player4Hand[count]))
                                        {
                                            player1Hand.Add(cardNumber);
                                            player4Hand.Remove(cardNumber);
                                            ++cardsStolen;
                                        }
                                    }
                                }
                            }
                            if (cardsStolen == 0)
                            {
                                WriteLine("Go fish!");
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
                                WriteLine($"Player 1 stole {cardsStolen} cards from Player {playerNumber}!");
                            }
                            WriteLine();
                            CheckFor4OfAKinds();
                            if (player1Points + player2Points + player3Points + player4Points == 13)
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
            }
        }

        public static void Player2Turn()
        {
            if (player2Hand.Count == 0)
            {
                turnEnded = true;
                playersToChooseFrom1.Remove(2);
                playersToChooseFrom3.Remove(2);
                playersToChooseFrom4.Remove(2);
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
                if (player2Hand.Count == 0)
                    turnEnded = true;
                else
                {
                    randomChoice = rand.Next(0, playersToChooseFrom2.Count);

                    if (players.Count == 0)
                    {
                        playerNumber = Convert.ToInt32(playersToChooseFrom2[randomChoice]);
                        randomChoice = rand.Next(0, player2Hand.Count);
                        cardNumber = Convert.ToInt32(player2Hand[randomChoice]);
                    }
                    else
                    {
                        playerNumber = Convert.ToInt32(playersToChooseFrom2[randomChoice]);
                        randomChoice = rand.Next(0, player2Hand.Count);
                        cardNumber = Convert.ToInt32(player2Hand[randomChoice]);
                        bool restartLoop = true;
                        while (restartLoop)
                        {
                            for (int count = 0; count < players.Count; ++count)
                            {
                                if (playerNumber == Convert.ToInt32(players[count]) && cardNumber == Convert.ToInt32(cards[count]))
                                {
                                    randomChoice = rand.Next(0, player2Hand.Count);
                                    cardNumber = Convert.ToInt32(player2Hand[randomChoice]);
                                    restartLoop = true;
                                    continue;
                                }
                                else
                                    restartLoop = false;
                            }
                            ++iteration;
                            if (iteration > 100)
                                continue;
                        }
                    }

                    players.Add(playerNumber);
                    cards.Add(cardNumber);
                    WriteLine("Player 2, choose a player to steal from.");
                    WriteLine($">> {playerNumber}");
                    WriteLine($"Player 2, what cards would you like to steal from Player {playerNumber}?");
                    WriteLine($">> {cardNumber}");
                    WriteLine();
                    ReadKey();

                    int cardsStolen = 0;
                    if (playerNumber == 1)
                    {
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {
                            for (int count = 0; count < player1Hand.Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(player1Hand[count]))
                                {
                                    player2Hand.Add(cardNumber);
                                    player1Hand.Remove(cardNumber);
                                    ++cardsStolen;
                                }
                            }
                        }
                    }
                    else if (playerNumber == 3)
                    {
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {
                            for (int count = 0; count < player3Hand.Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(player3Hand[count]))
                                {
                                    player2Hand.Add(cardNumber);
                                    player3Hand.Remove(cardNumber);
                                    ++cardsStolen;
                                }
                            }
                        }
                    }
                    else if (playerNumber == 4)
                    {
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {
                            for (int count = 0; count < player4Hand.Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(player4Hand[count]))
                                {
                                    player2Hand.Add(cardNumber);
                                    player4Hand.Remove(cardNumber);
                                    ++cardsStolen;
                                }
                            }
                        }
                    }
                    if (cardsStolen == 0)
                    {
                        WriteLine("Go fish!");
                        if (deck.Count > 0)
                        {
                            player2Hand.Add(deck.Dequeue());
                        }
                        else
                        {
                            WriteLine("There are no more cards in the deck.");
                        }
                        turnEnded = true;
                    }
                    else
                    {
                        WriteLine($"Player 2 stole {cardsStolen} cards from Player {playerNumber}!");
                    }
                    WriteLine();
                    CheckFor4OfAKinds();
                    if (player1Points + player2Points + player3Points + player4Points == 13)
                    {
                        gameEnded = true;
                        turnEnded = true;
                    }
                    ReadKey();
                }

            }
            players.Clear();
            cards.Clear();
        }

        public static void Player3Turn()
        {
            if (player3Hand.Count == 0)
            {
                turnEnded = true;
                playersToChooseFrom1.Remove(3);
                playersToChooseFrom2.Remove(3);
                playersToChooseFrom4.Remove(3);
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
                if (player3Hand.Count == 0)
                    turnEnded = true;
                else
                {
                    randomChoice = rand.Next(0, playersToChooseFrom3.Count);

                    if (players.Count == 0)
                    {
                        playerNumber = Convert.ToInt32(playersToChooseFrom3[randomChoice]);
                        randomChoice = rand.Next(0, player3Hand.Count);
                        cardNumber = Convert.ToInt32(player3Hand[randomChoice]);
                    }
                    else
                    {
                        playerNumber = Convert.ToInt32(playersToChooseFrom3[randomChoice]);
                        randomChoice = rand.Next(0, player3Hand.Count);
                        cardNumber = Convert.ToInt32(player3Hand[randomChoice]);
                        bool restartLoop = true;
                        while (restartLoop)
                        {
                            for (int count = 0; count < players.Count; ++count)
                            {
                                if (playerNumber == Convert.ToInt32(players[count]) && cardNumber == Convert.ToInt32(cards[count]))
                                {
                                    randomChoice = rand.Next(0, player3Hand.Count);
                                    cardNumber = Convert.ToInt32(player3Hand[randomChoice]);
                                    restartLoop = true;
                                    continue;
                                }
                                else
                                    restartLoop = false;
                            }
                            ++iteration;
                            if (iteration > 100)
                                continue;
                        }
                    }

                    players.Add(playerNumber);
                    cards.Add(cardNumber);
                    WriteLine("Player 3, choose a player to steal from.");
                    WriteLine($">> {playerNumber}");
                    WriteLine($"Player 3, what cards would you like to steal from Player {playerNumber}?");
                    WriteLine($">> {cardNumber}");
                    WriteLine();
                    ReadKey();

                    int cardsStolen = 0;
                    if (playerNumber == 1)
                    {
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {
                            for (int count = 0; count < player1Hand.Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(player1Hand[count]))
                                {
                                    player3Hand.Add(cardNumber);
                                    player1Hand.Remove(cardNumber);
                                    ++cardsStolen;
                                }
                            }
                        }
                    }
                    else if (playerNumber == 2)
                    {
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {
                            for (int count = 0; count < player2Hand.Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(player2Hand[count]))
                                {
                                    player3Hand.Add(cardNumber);
                                    player2Hand.Remove(cardNumber);
                                    ++cardsStolen;
                                }
                            }
                        }
                    }
                    else if (playerNumber == 4)
                    {
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {
                            for (int count = 0; count < player4Hand.Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(player4Hand[count]))
                                {
                                    player3Hand.Add(cardNumber);
                                    player4Hand.Remove(cardNumber);
                                    ++cardsStolen;
                                }
                            }
                        }
                    }
                    if (cardsStolen == 0)
                    {
                        WriteLine("Go fish!");
                        if (deck.Count > 0)
                        {
                            player3Hand.Add(deck.Dequeue());
                        }
                        else
                        {
                            WriteLine("There are no more cards in the deck.");
                        }
                        turnEnded = true;
                    }
                    else
                    {
                        WriteLine($"Player 3 stole {cardsStolen} cards from Player {playerNumber}!");
                    }
                    WriteLine();
                    CheckFor4OfAKinds();
                    if (player1Points + player2Points + player3Points + player4Points == 13)
                    {
                        gameEnded = true;
                        turnEnded = true;
                    }
                    ReadKey();
                }
            }
            players.Clear();
            cards.Clear();
        }

        public static void Player4Turn()
        {
            if (player4Hand.Count == 0)
            {
                turnEnded = true;
                playersToChooseFrom1.Remove(4);
                playersToChooseFrom2.Remove(4);
                playersToChooseFrom3.Remove(4);
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
                if (player4Hand.Count == 0)
                    turnEnded = true;
                else
                {
                    randomChoice = rand.Next(0, playersToChooseFrom4.Count);

                    if (players.Count == 0)
                    {
                        playerNumber = Convert.ToInt32(playersToChooseFrom4[randomChoice]);
                        randomChoice = rand.Next(0, player4Hand.Count);
                        cardNumber = Convert.ToInt32(player4Hand[randomChoice]);
                    }
                    else
                    {
                        playerNumber = Convert.ToInt32(playersToChooseFrom4[randomChoice]);
                        randomChoice = rand.Next(0, player4Hand.Count);
                        cardNumber = Convert.ToInt32(player4Hand[randomChoice]);
                        bool restartLoop = true;
                        while (restartLoop)
                        {
                            for (int count = 0; count < players.Count; ++count)
                            {
                                if (playerNumber == Convert.ToInt32(players[count]) && cardNumber == Convert.ToInt32(cards[count]))
                                {
                                    randomChoice = rand.Next(0, player4Hand.Count);
                                    cardNumber = Convert.ToInt32(player4Hand[randomChoice]);
                                    restartLoop = true;
                                    continue;
                                }
                                else
                                    restartLoop = false;
                            }
                            ++iteration;
                            if (iteration > 100)
                                continue;
                        }
                    }

                    players.Add(playerNumber);
                    cards.Add(cardNumber);
                    WriteLine("Player 4, choose a player to steal from.");
                    WriteLine($">> {playerNumber}");
                    WriteLine($"Player 4, what cards would you like to steal from Player {playerNumber}?");
                    WriteLine($">> {cardNumber}");
                    WriteLine();
                    ReadKey();

                    int cardsStolen = 0;
                    if (playerNumber == 1)
                    {
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {
                            for (int count = 0; count < player1Hand.Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(player1Hand[count]))
                                {
                                    player4Hand.Add(cardNumber);
                                    player1Hand.Remove(cardNumber);
                                    ++cardsStolen;
                                }
                            }
                        }
                    }
                    else if (playerNumber == 2)
                    {
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {
                            for (int count = 0; count < player2Hand.Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(player2Hand[count]))
                                {
                                    player4Hand.Add(cardNumber);
                                    player2Hand.Remove(cardNumber);
                                    ++cardsStolen;
                                }
                            }
                        }
                    }
                    else if (playerNumber == 3)
                    {
                        for (int cycle = 0; cycle < 3; ++cycle)
                        {
                            for (int count = 0; count < player3Hand.Count; ++count)
                            {
                                if (cardNumber == Convert.ToInt32(player3Hand[count]))
                                {
                                    player4Hand.Add(cardNumber);
                                    player3Hand.Remove(cardNumber);
                                    ++cardsStolen;
                                }
                            }
                        }
                    }
                    if (cardsStolen == 0)
                    {
                        WriteLine("Go fish!");
                        if (deck.Count > 0)
                        {
                            player4Hand.Add(deck.Dequeue());
                        }
                        else
                        {
                            WriteLine("There are no more cards in the deck.");
                        }
                        turnEnded = true;
                    }
                    else
                    {
                        WriteLine($"Player 4 stole {cardsStolen} cards from Player {playerNumber}!");
                    }
                    WriteLine();
                    CheckFor4OfAKinds();
                    if (player1Points + player2Points + player3Points + player4Points == 13)
                    {
                        gameEnded = true;
                        turnEnded = true;
                    }
                    ReadKey();
                }
            }
            players.Clear();
            cards.Clear();
        }
    }
}
