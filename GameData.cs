using System.Collections;

namespace UNO
{
    internal class GameData
    {
        //starting menu options
        static public string[] options = {
            "Start game",
            "Exit"
        };

        //game properties
        static public int maxPlayers = 5;
        static public int totalPlayers, tempTotalPlayers;
        static public int invalidInputCount;
        static public int extraCardDrawn;
        static public bool reversed;
        static public bool skipped;
        static public int fineCard;
        static public int requiredCard;
        static public int choosenColor;
        static public ArrayList Cards;
        static public ArrayList CardsOnTable;

        //template of a card
        public class Card
        {
            public int Color;
            public int Number;
            public int Index;
            public bool OnPlayerDeck;
            public Card(int color, int number, int index, bool onPlayerDeck)
            {
                Color = color;
                Number = number;
                Index = index;
                OnPlayerDeck = onPlayerDeck;
            }
        }

        //player's template
        public class Player
        {
            public string name;
            public ArrayList deck;
        }
        static public ArrayList Players;
    }
}
