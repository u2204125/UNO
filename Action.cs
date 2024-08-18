using System;
using System.Collections;
using static UNO.GameData;
using static UNO.Inputs;
using static UNO.Display;

namespace UNO
{
    internal class Action
    {
        //setting the default game properties
        static public void Normalize()
        {
            totalPlayers = 0;
            tempTotalPlayers = 0;
            invalidInputCount = 0;
            extraCardDrawn = 0;
            reversed = false;
            skipped = false;
            fineCard = 0;
            requiredCard = -1;
            choosenColor = -1;
            Cards = new ArrayList();
            CardsOnTable = new ArrayList();
            Players = new ArrayList();
        }

        //creating the mother-deck
        static public void CreateCards()
        {
            int cards = 0;
            for (int k = 0; k < 2; k++)
            {
                for (int i = 1; i <= 4; i++)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        Cards.Add(new Card(i, j, cards++, false)); //i=color, j=number
                                                                   //10=+2, 11=skip, 12=reverse
                    }
                }
            }

            for (int k = 0; k < 4; k++)
            {
                Cards.Add(new Card(0, 13, cards++, false)); //color change
                Cards.Add(new Card(0, 14, cards++, false)); //+4
            }
        }

        //creating players
        static public void PlayerPrompt()
        {
            //too many invalid attempts
            if (invalidInputCount > 3)
                Msg.TooManyAttempts();

            else
            {
                try
                {
                    Console.Write($"How many players are going to play(max {maxPlayers}): ");
                    totalPlayers = int.Parse(Console.ReadLine());

                    //for invalid input
                    if (totalPlayers <= 1 || totalPlayers > maxPlayers)
                        throw new Exception();

                    // for valid input
                    else
                    {
                        //setting game properties
                        invalidInputCount = 0;
                        tempTotalPlayers = totalPlayers;

                        // asking individual's name and setting properties
                        for (int i = 0; i < totalPlayers; i++)
                        {
                            Player playerX = new Player();
                            while (invalidInputCount <= 3)
                            {
                                if(computerMode && (i != 0))
                                {
                                    playerX.name = $"Computer{i}";
                                    playerX.isBot = true;
                                }
                                else
                                {
                                    Console.Write($"\nName of Player{i + 1}: ");
                                    playerX.name = Console.ReadLine();

                                    if (playerX.name == "")
                                        playerX.name = $"Player{i + 1}";
                                }

                                //assigning cards to the player's deck
                                playerX.deck = new ArrayList();
                                for (int j = 0; j < 5; j++)
                                {
                                    playerX.deck.Add(DrawCard());
                                }
                                break;
                            }
                            Players.Add(playerX);

                            //too many invalid attempts
                            if (invalidInputCount > 3) break;
                        }

                        Console.Clear();
                        Msg.Success("All the players added successfully. Loading game....");
                    }
                }

                // for invalid input
                catch (Exception)
                {
                    Msg.Error($"Input must be within the range of '2~{maxPlayers}'");
                    invalidInputCount++;
                    PlayerPrompt();
                }
            }
        }

        //drawing a random card from the mother-deck
        static public Card DrawCard()
        {
            Random rnd = new Random();
            while (true)
            {
                int randIndex = rnd.Next(Cards.Count);
                Card card = (Card)Cards[randIndex];
                
                //only draw the card that is not on players' deck
                if (!card.OnPlayerDeck)
                {
                    card.OnPlayerDeck = true;
                    Cards[randIndex] = card;
                    return card;
                }
            }
        }

        //clearing the table
        static public void ClearTable()
        {
            //if no +2/+4 card is played
            if (requiredCard == -1)
            {
                foreach (Card tableCard in CardsOnTable)
                {
                    tableCard.OnPlayerDeck = false;
                    Cards[tableCard.Index] = tableCard;
                }
                CardsOnTable.Clear();
            }

            //if any +2/+4 card is played
            else
            {
                for (int j = CardsOnTable.Count; j > 0; j--)
                {
                    int x;
                    if (requiredCard == 10) x = (fineCard / 2) - 1;
                    else x = (fineCard / 4) - 1;

                    if ((CardsOnTable.Count - j) != x) continue;

                    CardsOnTable.RemoveAt(j - 1);
                }
            }
        }

        //making player turn
        static public void PlayerTurn(int playerIndex)
        {
            Player playerX = (Player)Players[playerIndex]; //selecting current player

            DisplayTableCards();

            //displaying if a player had changed the color
            if (choosenColor != -1)
            {
                string color = "";
                switch (choosenColor)
                {
                    case 1:
                        color = "Green";
                        break;
                    case 2:
                        color = "Yellow";
                        break;
                    case 3:
                        color = "Red";
                        break;
                    case 4:
                        color = "Blue";
                        break;
                }
                Console.WriteLine("Choosen color: {0}", color);
            }

            Msg.PlayerTurn(playerX.name);

            //displaying player's deck
            int i = 0;
            foreach (Card cardX in playerX.deck)
            {
                DisplayCard(cardX.Color, cardX.Number, ++i);
            }
            Console.Write("\n");

            Card lastCardOnTable = null;

            //taking input of the player's turn
            string inpMsg = "";
            if (requiredCard == -1)
            {
                inpMsg = "Enter your card number(input '0' ";
                inpMsg += (extraCardDrawn == 1) ? "to pass" : "to draw a card";
            }
            else
            {
                string reqCard = requiredCard == 10 ? "+2" : "+4";
                inpMsg = $"Enter a card number of {reqCard}(input '0' to pass the turn and take cards";
            }
            inpMsg += ")";
            int cardIndex = OptionPrompt(minInp: 0, maxInp: playerX.deck.Count, inpMsg: inpMsg);

            //if player doesn't have a matching card
            if (cardIndex == 0)
            {
                //if no +2/+4 card is played
                if (requiredCard == -1)
                {
                    //haven't drawn a card yet
                    if (extraCardDrawn == 0)
                    {
                        playerX.deck.Add(DrawCard());
                        extraCardDrawn++;
                        Players[playerIndex] = playerX; //saving player's state
                        PlayerTurn(playerIndex);
                    }

                    //already have drawn a card
                    else
                    {
                        extraCardDrawn = 0;
                        ChangePlayerTurn(playerIndex);
                    }
                }

                //if any +2/+4 card is played
                else
                {
                    // assigning extra cards to the player's deck
                    for (int j = 0; j < fineCard; j++)
                        playerX.deck.Add(DrawCard());

                    //resetting game properties
                    fineCard = 0;
                    requiredCard = -1;
                    lastCardOnTable = (Card)CardsOnTable[CardsOnTable.Count - 1];

                    //removing cards from the table amd moving to the mother-deck
                    foreach (Card tableCard in CardsOnTable)
                    {
                        tableCard.OnPlayerDeck = false;
                        Cards[tableCard.Index] = tableCard;
                    }
                    CardsOnTable.Clear();
                    CardsOnTable.Add(lastCardOnTable);

                    Players[playerIndex] = playerX; //saving player's state
                    ChangePlayerTurn(playerIndex);
                }
            }

            //if player has a matching card
            else
            {
                //if the turn is not valid
                if (!validTurn(playerIndex, cardIndex))
                {
                    Msg.Error("Invalid move. Try again");

                    invalidInputCount++;
                    if (invalidInputCount > 3)
                    {
                        Msg.TooManyAttempts();
                        Environment.Exit(0);
                    }
                    else
                        PlayerTurn(playerIndex);
                }

                //if the turn is valid
                else
                {
                    //resetting game properties
                    invalidInputCount = 0;
                    extraCardDrawn = 0;
                }
            }

            SpecialCardAction((Card)playerX.deck[cardIndex - 1], false);

            //----removing cards from the table and moving to the mother-deck---
            ClearTable();
            CardsOnTable.Add(playerX.deck[cardIndex - 1]);
            playerX.deck.RemoveAt(cardIndex - 1);
            Players[playerIndex] = playerX; //saving player's state

            //resetting game properties(color change)
            lastCardOnTable = (Card)CardsOnTable[CardsOnTable.Count - 1];
            if ((lastCardOnTable.Number != 13) && (lastCardOnTable.Number != 14))
                choosenColor = -1;

            UnoCall(playerIndex, false);
            WinCheck(playerIndex, false);
            ChangePlayerTurn(playerIndex);
        }

        //making computer turn
        static public void ComputerTurn(int playerIndex)
        {
            Player compX = (Player)Players[playerIndex]; //selecting current computer

            Card lastCardOnTable = null;
            
            //checking for any valid card
            int i = 0;
            foreach (Card cardX in compX.deck)
            {
                if(validTurn(playerIndex, ++i)){
                    DisplayCard(cardX.Color, cardX.Number, -1);
                    break;
                }
                else{
                    if(i == compX.deck.Count)
                        i = -1;
                }
            }
            
            // if no valid card is found
            if (i == -1)
            {
                //if no +2/+4 card is played
                if (requiredCard == -1)
                {
                    //haven't drawn a card yet
                    if (extraCardDrawn == 0)
                    {
                        compX.deck.Add(DrawCard());
                        extraCardDrawn++;
                        Players[playerIndex] = compX; //saving player's state
                        ComputerTurn(playerIndex);
                    }

                    //already have drawn a card
                    else
                        extraCardDrawn = 0;
                }

                //if any +2/+4 card is played
                else
                {
                    // assigning extra cards to the player's deck
                    for (int j = 0; j < fineCard; j++)
                        compX.deck.Add(DrawCard());

                    //resetting game properties
                    fineCard = 0;
                    requiredCard = -1;
                    lastCardOnTable = (Card)CardsOnTable[CardsOnTable.Count - 1];

                    //removing cards from the table amd moving to the mother-deck
                    foreach (Card tableCard in CardsOnTable)
                    {
                        tableCard.OnPlayerDeck = false;
                        Cards[tableCard.Index] = tableCard;
                    }
                    CardsOnTable.Clear();
                    CardsOnTable.Add(lastCardOnTable);

                    Players[playerIndex] = compX; //saving player's state
                    Msg.ComputerTurn(compX.name);
                    ChangePlayerTurn(playerIndex);
                }
            }

            // if valid card is found
            else
            {
                extraCardDrawn = 0;
                SpecialCardAction((Card)compX.deck[i - 1], true);

                //----removing cards from the table and moving to the mother-deck---
                ClearTable();
                CardsOnTable.Add(compX.deck[i - 1]);
                compX.deck.RemoveAt(i - 1);
                Players[playerIndex] = compX; //saving player's state
            }

            //resetting game properties(color change)
            lastCardOnTable = (Card)CardsOnTable[CardsOnTable.Count - 1];
            if ((lastCardOnTable.Number != 13) && (lastCardOnTable.Number != 14))
                choosenColor = -1;

            Msg.ComputerTurn(compX.name);
            UnoCall(playerIndex, true);
            WinCheck(playerIndex, true);
            ChangePlayerTurn(playerIndex);
        }

        //passing player turn
        static private void ChangePlayerTurn(int playerIndex)
        {
            //if only reverse card has been played
            if (reversed & !skipped)
                playerIndex = (playerIndex == 0) ? (totalPlayers - 1) : (playerIndex - 1);

            //if skip card is played
            else if (skipped)
            {
                //if reverse card has been played
                if (reversed)
                    playerIndex = ((playerIndex - 2) < 0) ? (playerIndex - 2 + totalPlayers) : (playerIndex-2);

                //if reverse card has not been played
                else
                    playerIndex = ((playerIndex + 2) >= totalPlayers) ? (playerIndex + 2 - totalPlayers) : (playerIndex+2);

                skipped = false; //resetting game property
            }

            //usual player shifting
            else
            {
                if (playerIndex > (totalPlayers - 1)) playerIndex = 0;
                else if (playerIndex < (totalPlayers - 1)) playerIndex++;

                //codition for (playerIndex == (totalPlayers - 1))
                else
                {
                    if (totalPlayers == tempTotalPlayers) playerIndex = 0;
                }
            }

            if(tempTotalPlayers != totalPlayers) tempTotalPlayers = totalPlayers;

            if (computerMode)
            {
                if (playerIndex == 0)
                    PlayerTurn(playerIndex);
                else
                    ComputerTurn(playerIndex);
            }
            else
            {
                Msg.Info("\aSwitch your position with the next player and hit enter....");
                Console.ReadLine();
                PlayerTurn(playerIndex);
            }
        }

        //checking the validity of the turn
        static private bool validTurn(int playerIndex, int cardIndex)
        {
            Player playerX = (Player)Players[playerIndex];
            Card choosenCard = (Card)playerX.deck[cardIndex - 1];

            //if +2 or +4 card is played
            if (requiredCard != -1)
                return (choosenCard.Number == requiredCard);

            //if color change card has been played
            else if (choosenColor != -1)
                return ((choosenCard.Color == choosenColor) || (choosenCard.Number == 13) || (choosenCard.Number == 14));

            Card lastCardOnTable = (Card)CardsOnTable[CardsOnTable.Count - 1];
            bool colorMatched = (choosenCard.Color == lastCardOnTable.Color);
            bool numberMatched = (choosenCard.Number == lastCardOnTable.Number) || (choosenCard.Number == 13) || (choosenCard.Number == 14);
            if (colorMatched || numberMatched)
                return true;

            return false;
        }

        //taking actions for special cards
        static private void SpecialCardAction(Card cardX, bool isPlayerBot)
        {
            //+2 card
            if (cardX.Number == 10)
            {
                fineCard += 2;
                requiredCard = 10;
            }

            //skip card
            if (cardX.Number == 11)
                skipped = true;

            //reverse card
            if (cardX.Number == 12)
                reversed = !reversed;

            //color change
            if (cardX.Number == 13)
                ColorChoice(isPlayerBot);

            //+4 card
            if (cardX.Number == 14)
            {
                fineCard += 4;
                requiredCard = 14;
                ColorChoice(isPlayerBot);
            }
        }

        //asking for color choice
        static private void ColorChoice(bool isPlayerBot)
        {
            if (computerMode && isPlayerBot)
            {
                Random rnd = new Random();
                int x = rnd.Next(4);
                choosenColor = (x == 0) ? 1 : x;
            }
            else
            {
                string inpMsg = "1| Green\n";
                inpMsg += "2| Yellow\n";
                inpMsg += "3| Red\n";
                inpMsg += "4| Blue\n";
                inpMsg += "Enter a color number";
                int color = OptionPrompt(minInp: 1, maxInp: 4, inpMsg: inpMsg);

                if (color == -1) Environment.Exit(0);
                else choosenColor = color;
            }
        }

        //checking players win situation
        static private void WinCheck(int playerIndex, bool isPlayerBot)
        {
            Player playerX = (Player)Players[playerIndex];
            bool isDeckEmpty = (playerX.deck.Count == 0);
            if (isDeckEmpty)
            {
                Msg.WinMsg(playerX.name);
                Players.RemoveAt(playerIndex); //removing player from the game
                totalPlayers--;
            }

            if ((totalPlayers == 1) || (computerMode && !isPlayerBot && isDeckEmpty))
            {
                Msg.GameOver();
                Console.WriteLine("Loading menus......\n\n");
                Program.Main();
            }
        }

        //checking for uno call
        static private void UnoCall(int playerIndex, bool isPlayerBot)
        {
            Player playerX = (Player)Players[playerIndex];
            if(playerX.deck.Count == 1)
            {
                string inp;
                if (computerMode && isPlayerBot)
                    inp = "0";
                else
                {
                    Console.Write("Press 0 to call for UNO: ");
                    inp = Console.ReadLine();
                }

                //if called UNO
                if (inp == "0")
                {
                    Card lastCard = (Card)playerX.deck[playerX.deck.Count - 1];

                    //if the last card is not a number card
                    if(lastCard.Number > 9)
                    {
                        playerX.deck.Add(DrawCard());
                        Players[playerIndex] = playerX; //saving player's state
                        Msg.Info($"{playerX.name}'s last card is not a number card. Penalty 1 card");
                    }

                    //if the last card is a number card
                    else
                    {
                        Msg.Info($"{playerX.name} has called for UNO");
                    }
                }

                //if didn't call UNO
                else
                {
                    playerX.deck.Add(DrawCard());
                    playerX.deck.Add(DrawCard());
                    Players[playerIndex] = playerX; //saving player's state
                    Msg.Info($"{playerX.name} didn't call for UNO. Penalty 2 card");
                }
            }
        }
    }
}