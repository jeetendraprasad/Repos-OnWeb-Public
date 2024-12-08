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

        List<List<int[]>> winningCombinations = new()
        {
            // 00 01 02
            // 10 11 12
            // 20 21 22
            new List<int[]>() { new int[] {0, 0}, new int[] {0, 1}, new int[] {0, 2} },
            new List<int[]>() { new int[] {1, 0}, new int[] {1, 1}, new int[] {1, 2} },
            new List<int[]>() { new int[] {2, 0}, new int[] {2, 1}, new int[] {2, 2} },

            new List<int[]>() { new int[] {0, 0}, new int[] {1, 0}, new int[] {2, 0} },
            new List<int[]>() { new int[] {0, 1}, new int[] {1, 1}, new int[] {2, 1} },
            new List<int[]>() { new int[] {0, 2}, new int[] {1, 2}, new int[] {2, 2} },

            new List<int[]>() { new int[] {0, 0}, new int[] {0, 1}, new int[] {0, 2} },
            new List<int[]>() { new int[] {1, 0}, new int[] {1, 1}, new int[] {1, 2} },
            new List<int[]>() { new int[] {2, 0}, new int[] {2, 1}, new int[] {2, 2} },

            new List<int[]>() { new int[] {0, 2}, new int[] {1, 1}, new int[] {2, 2} },
            new List<int[]>() { new int[] {0, 2}, new int[] {1, 1}, new int[] {2, 0} },
        };
        #endregion

        public Board()
        {
            
        }

        private async Task MakeMove(int row, int col)
        {
            board[row, col] = player;
            player = player == 'x' ? 'o' : 'x';

            foreach(var item in winningCombinations) { }
        }
    }
}
