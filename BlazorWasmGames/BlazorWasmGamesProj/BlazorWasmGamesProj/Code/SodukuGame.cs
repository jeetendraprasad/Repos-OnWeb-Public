namespace BlazorWasmGamesProj.Code
{
    public class SodukuGame
    {
        int _rowsBlock = 0, _colsBlock = 0;

        const string _blockIdPrefix = "B[{0},{1}]";                // block names are B[{0-based rows},{0-based cols}]
        const string _cellIdPrefix = "{0}:C[{1},{2}]";             // cell names are BLOCK_ID:[{0-based rows},{0-based cols}]

        public SodukuGame(int rowsBlock, int colsBlock)
        {
            ReInit(rowsBlock, colsBlock);
        }

        public void ReInit(int rowsBlock, int colsBlock)
        {
            _rowsBlock = rowsBlock;
            _colsBlock = colsBlock;
        }

        public static string GetBlockId(int x, int y) => string.Format(_blockIdPrefix, x, y);

        public static string GetCellId(string blockId, int x, int y) => string.Format(_cellIdPrefix, blockId, x, y);


        public List<string> SuBlockFullFlattened
        {
            get
            {
                List<string> list = SuBlockFull.Flatten();
                Console.WriteLine(string.Join("    ", list));
                return list;
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
                List<string> list = SuHoriFull.Flatten();
                Console.WriteLine(string.Join("    ", list));
                return list;
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
                List<string> list = SuVertFull.Flatten();
                Console.WriteLine(string.Join("    ", list));
                return list;
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
}
