//Daniel Dyson
//This program retrieves a shuffled deck of cards from the CreateDeckOfCards class, and proceeds to start the card game, War.
//The user can be either Player 1 or 2, but both players have an equal chance of winning.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using static System.Console;
using System.IO;

namespace DeckOfCards
{
    class War
    {
        static Queue<int> player1 = new Queue<int>();
        static Queue<int> player2 = new Queue<int>();
        static Queue<int> awardDeck = new Queue<int>();

        public static void PlayWar(ArrayList deckOfCardsNumbers)
        {
            DealCards(deckOfCardsNumbers);
            PlayCards();
        }

        public static void DealCards(ArrayList deckOfCardsNumbers)
        {
            for (int count = 0; count < 26; ++count)
            {
                player1.Enqueue(Convert.ToInt32(deckOfCardsNumbers[count]));
            }

            for (int count = 26; count < 52; ++count)
            {
                player2.Enqueue(Convert.ToInt32(deckOfCardsNumbers[count]));
            }
        }

        public static void PlayCards()
        {
            bool endOfGame = false;
            int numOfRounds = 1;
            int player1val;
            int player2val;
            StreamWriter write;
            int gameNum = 1;
            while (File.Exists($"War Game #{gameNum}.txt"))
            {
                ++gameNum;
            }
            write = new StreamWriter($"War Game #{gameNum}.txt");
            while (endOfGame == false)
            {
                player1val = player1.Dequeue();
                player2val = player2.Dequeue();
                WriteLine($"Player 1: {player1val}\t Player 2: {player2val}");
                write.WriteLine($"Player 1: {player1val}\t Player 2: {player2val}");
                if (player1val > player2val)
                {
                    WriteLine("Player 1 wins this round");
                    write.WriteLine("Player 1 wins this round");
                    ReadKey();
                    player1.Enqueue(player1val);
                    player1.Enqueue(player2val);
                }
                else if (player1val < player2val)
                {
                    WriteLine("Player 2 wins this round");
                    write.WriteLine("Player 2 wins this round");
                    ReadKey();
                    player2.Enqueue(player2val);
                    player2.Enqueue(player1val);
                }
                else if (player1val == player2val)
                {
                    WriteLine("This round is a tie. Both players keep going until one of them wins");
                    write.WriteLine("This round is a tie. Both players keep going until one of them wins");
                    ReadKey();
                    while (player1val == player2val)
                    {
                        try
                        {
                            awardDeck.Enqueue(player1val);
                            awardDeck.Enqueue(player2val);
                            player1val = player1.Dequeue();
                            player2val = player2.Dequeue();
                            WriteLine($"Player 1: {player1val}\t Player 2: {player2val}");
                            write.WriteLine($"Player 1: {player1val}\t Player 2: {player2val}");
                            if (player1val > player2val)
                            {
                                WriteLine("Player 1 breaks the tie and takes all of the cards");
                                write.WriteLine("Player 1 breaks the tie and takes all of the cards");
                                ReadKey();
                                while (awardDeck.Count > 0)
                                {
                                    player1.Enqueue(awardDeck.Dequeue());
                                }
                            }
                            else if (player1val < player2val)
                            {
                                WriteLine("Player 2 breaks the tie and takes all of the cards");
                                write.WriteLine("Player 2 breaks the tie and takes all of the cards");
                                ReadKey();
                                while (awardDeck.Count > 0)
                                {
                                    player2.Enqueue(awardDeck.Dequeue());
                                }
                            }
                        }
                        catch
                        {
                            WriteLine("A player has run out of cards!");
                            write.WriteLine("A player has run out of cards!");
                            ReadKey();
                        }
                    }
                }
                if (player1.Count == 0)
                {
                    endOfGame = true;
                }
                else if (player2.Count == 0)
                {
                    endOfGame = true;
                }
                else if (numOfRounds == 26)
                {
                    endOfGame = true;
                }
                ++numOfRounds;
            }

            if (player1.Count == 0)
            {
                WriteLine("Player 2 Wins!!!");
                write.WriteLine("Player 2 Wins!!!");
            }
            else if (player2.Count == 0)
            {
                WriteLine("Player 1 Wins!!!");
                write.WriteLine("Player 1 Wins!!!");
            }
            else if (player1.Count > player2.Count)
            {
                WriteLine("Player 1 Wins!!!");
                write.WriteLine("Player 1 Wins!!!");
            }
            else if (player1.Count < player2.Count)
            {
                WriteLine("Player 2 Wins!!!");
                write.WriteLine("Player 2 Wins!!!");
            }
            write.Close();
            ReadKey();
        }
    }
}