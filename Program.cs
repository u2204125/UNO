using System;
using System.Collections;
using static UNO.GameData;
using static UNO.Inputs;
using static UNO.Display;
using static UNO.Action;

namespace UNO
{
    internal class Program
    {
        public static void Main()
        {
            while (true)
            {
                StartManus();
                int inp = OptionPrompt(maxInp:options.Length+1);

                //playing with players
                if (inp == 1)
                {
                    //------preapring the game------
                    Normalize(); //setting default game properties
                    CreateCards(); //creating all the cards
                    computerMode = false;
                    PlayerPrompt(); //asking players data

                    //adding reference card
                    Card cardX = null;
                    while (cardX == null || cardX.Number > 9)
                        cardX = (Card)DrawCard();
                    CardsOnTable.Add(cardX);

                    PlayerTurn(0); //staring 1st player turn
                }

                //playing with computers
                else if (inp == 2)
                {
                    //------preapring the game------
                    Normalize(); //setting default game properties
                    CreateCards(); //creating all the cards
                    computerMode = true;
                    PlayerPrompt(); //asking players data

                    //adding reference card
                    Card cardX = null;
                    while (cardX == null || cardX.Number > 9)
                        cardX = (Card)DrawCard();
                    CardsOnTable.Add(cardX);

                    PlayerTurn(0); //staring 1st player turn
                }

                //program termination process
                else if (inp == 3)
                {
                    //if quiting is confirmed
                    if (Confirm("Are sure about quiting?"))
                    {
                        Msg.Info("Program is being terminated....");
                        Environment.Exit(0);
                    }

                    //if quiting is not confirmed
                    else
                        Console.Clear();
                }

                //hidden feature: custom testing
                else if (inp == (options.Length+1))
                {
                    CustomTestManus();
                    int inp2 = OptionPrompt(maxInp: 4);

                    //showing all the cards
                    if(inp2 == 1)
                    {
                        Normalize();
                        CreateCards();
                        int i = 0;
                        foreach(Card cardX in Cards)
                        {
                            DisplayCard(cardX.Color, cardX.Number, ++i);
                        }
                        Console.Write("Press enter to continue to start menu.....");
                        Console.ReadLine();
                        Console.Clear();
                    }

                    //playing with players
                    else if (inp2 == 2)
                    {
                        Normalize();
                        CreateCards();
                        computerMode = false;
                        totalPlayers = 3;
                        tempTotalPlayers = 3;

                        Player p1 = new Player();
                        Player p2 = new Player();
                        Player p3 = new Player();

                        p1.name = "a";
                        p2.name = "b";
                        p3.name = "c";

                        p1.deck = new ArrayList();
                        p2.deck = new ArrayList();
                        p3.deck = new ArrayList();

                        p1.deck.Add(GameData.Cards[106]);
                        p1.deck.Add(GameData.Cards[2]);
                        p1.deck.Add(GameData.Cards[3]);
                        p3.deck.Add(GameData.Cards[4]);
                        p3.deck.Add(GameData.Cards[107]);
                        p2.deck.Add(GameData.Cards[6]);
                        p2.deck.Add(GameData.Cards[7]);
                        p2.deck.Add(GameData.Cards[109]);

                        GameData.Players.Add(p1);
                        GameData.Players.Add(p2);
                        GameData.Players.Add(p3);

                        CardsOnTable.Add(Cards[0]);
                        PlayerTurn(0);
                    }

                    //playing with computers
                    else if (inp2 == 3)
                    {
                        Normalize();
                        CreateCards();
                        computerMode = true;
                        totalPlayers = 3;
                        tempTotalPlayers = 3;

                        Player p1 = new Player();
                        Player c1 = new Player();
                        Player c2 = new Player();

                        p1.name = "p1";
                        c1.name = "c1";
                        c2.name = "c2";

                        p1.deck = new ArrayList();
                        c1.deck = new ArrayList();
                        c2.deck = new ArrayList();

                        p1.deck.Add(GameData.Cards[1]);
                        p1.deck.Add(GameData.Cards[2]);
                        p1.deck.Add(GameData.Cards[3]);
                        c2.deck.Add(GameData.Cards[14]);
                        c2.deck.Add(GameData.Cards[25]);
                        c1.deck.Add(GameData.Cards[15]);
                        c1.deck.Add(GameData.Cards[27]);
                        c1.deck.Add(GameData.Cards[106]);

                        GameData.Players.Add(p1);
                        GameData.Players.Add(c1);
                        GameData.Players.Add(c2);

                        CardsOnTable.Add(Cards[0]);
                        PlayerTurn(0);
                    }

                    //back to start menu
                    else if (inp2 == 4)
                    {
                        invalidInputCount = 0;
                        Console.Clear();
                        continue;
                    }
                }
            }
        }
    }
}
