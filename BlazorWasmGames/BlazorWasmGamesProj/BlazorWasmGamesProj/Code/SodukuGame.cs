
namespace BlazorWasmGamesProj.Code
{


    public class SodukuGame
    {
        int _rowsBlock = 0, _colsBlock = 0;

        const string _blockIdPrefix = "B[{0},{1}]";                // block names are B[{0-based rows},{0-based cols}]
        const string _cellIdPrefix = "{0}:C[{1},{2}]";             // cell names are BLOCK_ID:[{0-based rows},{0-based cols}]

        private List<string> _suBlockFullFlattened = new();
        private List<string> _suHoriFullFlattened = new();
        private List<string> _suVertFullFlattened = new();

        //private Dictionary<string, int> moves = new();
        private List<SodukuGameMove> moves = new();

        public List<SodukuGameMove> Moves
        {
            get
            {
                return moves;
            }
        }
        public Dictionary<string, string> Positions
        {
            get
            {
                Dictionary<string, string> posns = SuHoriFullFlattened.ToDictionary(item => item, item => "0");
                foreach (SodukuGameMove item in moves)
                {
                    posns[item.CellId] = item.CellValue;
                }

                return posns;
            }
        }

        public SodukuGameState GameState { get; private set; } = SodukuGameState.CalculationPending;

        public SodukuGame(int rowsBlock, int colsBlock)
        {
            ReInit(rowsBlock, colsBlock);
        }

        public void ReInit(int rowsBlock, int colsBlock)
        {
            _rowsBlock = rowsBlock;
            _colsBlock = colsBlock;

            GameState = SodukuGameState.CalculationPending;
        }

        public void Recalculate()
        {
            _suBlockFullFlattened = SuBlockFull.Flatten();
            _suHoriFullFlattened = SuHoriFull.Flatten();
            _suVertFullFlattened = SuVertFull.Flatten();

            //moves = new Dictionary<string, int>(_rowsBlock * _colsBlock);
            //moves = SuHoriFullFlattened.ToDictionary( item => item, item => 0);

            GameState = SodukuGameState.CalculationDone_Or_EditModeBegin;
        }

        public bool AddMove(string cellId, string cellValue)
        {
            moves.Add(new SodukuGameMove() { CellId = cellId, CellValue = cellValue});
            return true;
        }

        void Solve()
        {
            CheckHori();
        }

        private void CheckHori()
        {
            foreach (KeyValuePair<string, string> elem in Positions)
            {
                Console.WriteLine("{0} and {1}", elem.Key, elem.Value);
            }

            Console.WriteLine("Moves : " + string.Join(',', Moves));
        }

        public void SaveNSolve()
        {
            this.GameState = SodukuGameState.EditModeDone_Or_SolvingBegin;

            Solve();

            this.GameState = SodukuGameState.SolvingDone;
        }

        public static string GetBlockId(int x, int y) => string.Format(_blockIdPrefix, x, y);

        public static string GetCellId(string blockId, int x, int y) => string.Format(_cellIdPrefix, blockId, x, y);


        public List<string> SuBlockFullFlattened
        {
            get
            {
                return _suBlockFullFlattened;
            }
            private set
            {
                _suBlockFullFlattened = value;
            }
        }
        List<List<string>> SuBlockFull
        {
            get
            {
                List<List<string>> retVal = new();

                for (int i = 0; i < _rowsBlock; i++)
                    for (int j = 0; j < _colsBlock; j++)
                    {
                        List<string> list = GetSuBlock(i, j);
                        retVal.Add(list);
                    }

                return retVal;
            }
        }
        // Get Solving Unit - for a block
        List<string> GetSuBlock(int rowNoBlock, int colNoBlock)
        {
            List<string> retVal = new List<string>();

            string blockId = string.Format(_blockIdPrefix, rowNoBlock, colNoBlock);

            for (int i = 0; i < _rowsBlock; i++)
                for (int j = 0; j < _colsBlock; j++)
                {
                    string id = string.Format(_cellIdPrefix, blockId, j, i);
                    Console.WriteLine($"ID = {id}");
                    retVal.Add(id);
                }


            return retVal;
        }

        public List<string> SuHoriFullFlattened
        {
            get
            {
                return _suHoriFullFlattened;
            }
            private set
            {
                _suHoriFullFlattened = value;
            }
        }
        List<List<string>> SuHoriFull
        {
            get
            {
                List<List<string>> retVal = new();

                for (int i = 0; i < _rowsBlock * _colsBlock; i++)
                {
                    //if(i == 1)
                    {
                        List<string> list = GetSuHori(i);
                        retVal.Add(list);
                    }
                }

                return retVal;
            }
        }
        // Get Solving Unit - Horigontal list
        List<string> GetSuHori(int rowNoSoduku)
        {
            List<string> retVal = new List<string>();

            int p = (int)(rowNoSoduku / _colsBlock);
            int r = (int)(rowNoSoduku % _colsBlock);

            for (int j = 0; j < _rowsBlock * _colsBlock; j++)
            {
                int q = (int)(j / _rowsBlock);
                int s = (int)(j % _rowsBlock);

                string blockId = GetBlockId(p, q);

                retVal.Add(string.Format(_cellIdPrefix, blockId, r, s));
            }

            return retVal;
        }

        public List<string> SuVertFullFlattened
        {
            get
            {
                return _suVertFullFlattened;
            }
            private set
            {
                _suVertFullFlattened = value;
            }
        }
        List<List<string>> SuVertFull
        {
            get
            {
                List<List<string>> retVal = new();

                for (int i = 0; i < _rowsBlock * _colsBlock; i++)
                {
                    //if(i == 1)
                    {
                        List<string> list = GetSuVert(i);
                        retVal.Add(list);
                    }
                }

                return retVal;
            }
        }
        // Get Solving Unit - Vertical list
        List<string> GetSuVert(int colNoSoduku)
        {
            List<string> retVal = new List<string>();

            int q = (int)(colNoSoduku / _rowsBlock);
            int s = (colNoSoduku % _rowsBlock);

            for (int i = 0; i < _rowsBlock * _colsBlock; i++)
            {
                int p = (int)(i / _colsBlock);
                int r = (int)(i % _colsBlock);

                string blockId = GetBlockId(p, q);

                retVal.Add(string.Format(_cellIdPrefix, blockId, r, s));
            }

            return retVal;
        }
    }

    public enum SodukuGameState
    {
        CalculationPending                  = 0,
        CalculationDone_Or_EditModeBegin    = 1,
        EditModeDone_Or_SolvingBegin        = 2,
        SolvingDone                         = 3,
    }

    public class SodukuGameMove
    {
        public string CellId { get; set; } = "";
        public string CellValue { get; set; } = "";

        public SodukuGameMoveMode MoveMode { get; set; } = SodukuGameMoveMode.Manual;

        public override string ToString()
        {
            string retVal = "";

            retVal += $"CellId    = {CellId}";
            retVal += " ";
            retVal += $"CellValue = {CellValue}";


            return retVal;
        }
    }

    public enum SodukuGameMoveMode
    {
        Manual,
        Automatic,
    }
}
