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
                //displaying manus and asking for input
                StartManus();
                int inp = OptionPrompt(maxInp:options.Length+1);

                //starting the game
                if (inp == 1)
                {
                    //preapring the game
                    Normalize(); //setting default game properties
                    CreateCards(); //creating all the cards
                    PlayerPrompt(); //asking players data

                    //adding reference card
                    Card cardX = null;
                    while (cardX==null || cardX.Number > 9)
                        cardX = (Card)DrawCard();
                    CardsOnTable.Add(cardX);

                    PlayerTurn(0); //staring 1st player turn
                }

                //program termination process
                else if (inp == 2)
                {
                    //if quiting is confirmed
                    if (Confirm("Are sure about quiting?"))
                    {
                        Msg.Info("Program is being terminated....");
                        Environment.Exit(0);
                    }

                    //if quiting is not confirmed
                    else
                    {
                        //too many invalid inputs
                        if (invalidInputCount > 3)
                            Environment.Exit(0);

                        else
                        {
                            invalidInputCount = 0;
                            Console.Write("\n\n");
                        }
                    }
                }

                //hidden feature: custom testing
                else if (inp == 3)
                {
                    Normalize();
                    CreateCards();
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

                    p1.deck.Add(GameData.Cards[1]);
                    p1.deck.Add(GameData.Cards[2]);
                    p1.deck.Add(GameData.Cards[3]);
                    p3.deck.Add(GameData.Cards[4]);
                    p3.deck.Add(GameData.Cards[5]);
                    p2.deck.Add(GameData.Cards[6]);
                    p2.deck.Add(GameData.Cards[7]);
                    p2.deck.Add(GameData.Cards[8]);

                    GameData.Players.Add(p1);
                    GameData.Players.Add(p2);
                    GameData.Players.Add(p3);

                    CardsOnTable.Add(Cards[0]);
                    PlayerTurn(0);
                }

                //too many invalid inputs
                else if (inp == -1)
                    Environment.Exit(0);
            }
        }
    }
}
