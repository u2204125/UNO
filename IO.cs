using System;
using static UNO.GameData;
using static UNO.Display;

namespace UNO
{
    public class Inputs
    {
        //option choice input prompt
        static public int OptionPrompt(int maxInp, int minInp = 1, string inpMsg = "Enter your choice", string errMsg = "")
        {
            while (true)
            {
                int inp;
                try
                {
                    Console.Write($"{inpMsg}: ");
                    inp = int.Parse(Console.ReadLine());

                    //throwing exception for any wrong input
                    if (inp < minInp || inp > maxInp)
                        throw new Exception();

                    Console.Clear();
                    invalidInputCount = 0;
                    return inp;
                }
                
                //for invalid input
                catch (Exception)
                {
                    Msg.Error(errMsg == "" ? $"Input must be within the range of '{minInp}~{maxInp}'" : errMsg);
                    invalidInputCount++;

                    //too many invalid attempts
                    if (invalidInputCount > 3)
                    {
                        Msg.TooManyAttempts(203);
                        break;
                    }
                }
            }

            return -1;
        }

        //asking confirmation
        static public bool Confirm(string msg)
        {
            Console.Write($"{msg}(y/n): ");
            string inp = Console.ReadLine();

            // if input is valid
            if (inp.ToLower() == "y" || inp.ToLower() == "n")
            {
                invalidInputCount = 0;
                return inp.ToLower() == "y" ? true : false;
            }

            // if input is not valid
            else
            {
                Msg.Error($"Input must be 'y' or 'n' ");
                invalidInputCount++;

                //too many invalid attempts
                if (invalidInputCount > 3)
                {
                    Msg.TooManyAttempts();
                    return false;
                }

                Confirm(msg);
                return false;
            }
        }
    }

    internal class Display
    {

        //Displaying the starting menus
        static public void StartManus()
        {
            //printing the logo
            Console.WriteLine("----------------------------------");
            Console.WriteLine("|               UNO              |");
            Console.WriteLine("----------------------------------");

            //printing the options
            int i = 0;
            foreach(string option in options)
            {
                Console.WriteLine($"{++i}| {option}");
            }
            Console.Write("\n\n");
        }

        //card display format
        static public void DisplayCard(int colour, int num, int i = -1)
        {
            string color = "", number = "";

            //setting color name
            switch (colour)
            {
                case 0:
                    color = "Black ";
                    break;
                case 1:
                    color = "Green ";
                    break;
                case 2:
                    color = "Yellow";
                    break;
                case 3:
                    color = "Red   ";
                    break;
                case 4:
                    color = "Blue  ";
                    break;
            }

            //setting card number
            switch (num)
            {
                case 10:
                    number = "     +2      ";
                    break;
                case 11:
                    number = "    SKIP     ";
                    break;
                case 12:
                    number = "   REVERSE   ";
                    break;
                case 13:
                    number = "COLOR_CHANGE ";
                    break;
                case 14:
                    number = "     +4      ";
                    break;
                default:
                    number = $"      {num}      ";
                    break;
            }

            //---formating starts here---
            Console.Write("\n");

            // if index is not provided(for displaying table cards)
            if (i == -1)
            {
                for (int j = 0; j < 29; j++) Console.Write("-");
                Console.WriteLine($"\n|    {color} ; {number} |");
                for (int j = 0; j < 29; j++) Console.Write("-");
            }

            // if index is provided(for displaying player cards)
            else
            {
                if (i > 9)
                {
                    for (int j = 0; j < 30; j++) Console.Write("-");
                    Console.WriteLine($"\n| {i}. {color} ; {number} |");
                    for (int j = 0; j < 30; j++) Console.Write("-");
                }
                else
                {
                    for (int j = 0; j < 29; j++) Console.Write("-");
                    Console.WriteLine($"\n| {i}. {color} ; {number} |");
                    for (int j = 0; j < 29; j++) Console.Write("-");
                }
            }

            Console.WriteLine();
        }

        //displaying cards on the table
        static public void DisplayTableCards()
        {
            Console.Write("Cards on the table:\n");
            int i = 0;
            foreach (Card card in CardsOnTable)
            {
                DisplayCard(card.Color, card.Number);
            }
        }

        //all pop-up messages are here
        public class Msg
        {
            static public void Error(string msg, int code = 101)
            {
                Console.WriteLine($"\n\tError-{code}: {msg}\n");
            }

            static public void Warning(string msg)
            {
                Console.WriteLine($"\n\tWarning: {msg}\n");
            }

            static public void Info(string msg)
            {
                Console.WriteLine($"\n\tInfo: {msg}\n");
            }

            static public void Success(string msg)
            {
                Console.WriteLine($"\n\tSuccess: {msg}\n");
            }

            static public void TooManyAttempts(int code=201)
            {
                Error("Too many invalid attempts. Terminating program.....", code);
            }

            static public void PlayerTurn(string name)
            {
                Console.WriteLine($"\n\t!! {name}'s turn !!\n");
            }

            static public void WinMsg(string name)
            {
                Console.WriteLine($"\n\t******* {name} has won! *******\n");
            }

            static public void GameOver()
            {
                Console.WriteLine($"\n\t!!*******!! Game Over !!*******!!\n");
            }
        }
    }
}
