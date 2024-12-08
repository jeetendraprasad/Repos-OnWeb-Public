using Microsoft.JSInterop;
using System.Data;

namespace TicTacToeGame.Pages
{
    public partial class Board
    {
        #region [Game tools]
        char[,] board =
        {
            {' ', ' ', ' ' },
            {' ', ' ', ' ' },
            {' ', ' ', ' ' }
        };

        char player = 'x';
        #endregion

        public Board()
        {
            
        }

        private async Task MakeMove(int row, int col)
        {
            board[row, col] = player;

            if(TestWin(player))
            {
                var a = await JS.InvokeAsync<bool>("confirm", player + " wins");
            }

            if(NoMoreMoves())
            {
                await JS.InvokeAsync<bool>("confirm", "GAME ENDS");

                ResetGame();
            }

            player = player == 'x' ? 'o' : 'x';
        }

        private bool TestWin(char player)
        {
            for (int row = 0; row < 3; row++)
            {
                if (CustomArray<char>.GetRow(board, row).Count(item => item == player) == 3)
                    return true;
            }

            for (int col = 0; col < 3; col++)
            {
                if (CustomArray<char>.GetColumn(board, col).Count(item => item == player) == 3)
                    return true;
            }

            return false;
        }

        private bool NoMoreMoves()
        {
            char[] baData = board.Cast<char>().ToArray();
            return !baData.Any(item => item == ' ');
        }

        void ResetGame()
        {
            char[,] boardBlank =
            {
                {' ', ' ', ' ' },
                {' ', ' ', ' ' },
                {' ', ' ', ' ' }
            };

            board = boardBlank;
        }
    }

    public static class CustomArray<T>
    {
        public static T[] GetColumn(T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public static T[] GetRow(T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }
    }
}
