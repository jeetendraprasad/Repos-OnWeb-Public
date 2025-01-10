using BlazorWasmGames2.Pages;
using System.Diagnostics;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorWasmGames2.Code
{
    internal class SudokuGame
    {
        readonly int _rowsBlockMinVal = 2;
        readonly int _rowsBlockMaxVal = 3;
        readonly int _rowsBlockStartVal = 2;
        readonly int _colsBlockMinVal = 2;
        readonly int _colsBlockMaxVal = 3;
        readonly int _colsBlockStartVal = 2;

        Dictionary<string, SudokuCellInfo> _positions = [];

        Integer1 _rowsBlock, _colsBlock;

        const string _blockIdPrefix = "B[{0},{1}]";                // block names are B[{0-based rows},{0-based cols}]
        const string _cellIdPrefix = "{0}:C[{1},{2}]";             // cell names are BLOCK_ID:[{0-based rows},{0-based cols}]

        public SudokuGame()
        {
            _rowsBlock = new(_rowsBlockStartVal, _rowsBlockMinVal, _rowsBlockMaxVal);
            _colsBlock = new(_colsBlockStartVal, _colsBlockMinVal, _colsBlockMaxVal);

            Init();
        }

        public Dictionary<string, SudokuCellInfo> GetPositions()
        {
            return _positions;
        }

        void Init()
        {
            _rowsBlock = new(_rowsBlockStartVal, _rowsBlockMinVal, _rowsBlockMaxVal);
            _colsBlock = new(_colsBlockStartVal, _colsBlockMinVal, _colsBlockMaxVal);

            _positions = GetSuHoriFull().Flatten().Select(x => new KeyValuePair<string, SudokuCellInfo>(x,
                new SudokuCellInfo(cellId: x, positionType: SudokuPositionTypeEnum.None, maxCellValue: _rowsBlock.ValAsInt * _colsBlock.ValAsInt, 0)
            ))
                .ToDictionary(t => t.Key, t => t.Value);
        }


        List<List<string>> GetSuHoriFull()
        {
            List<List<string>> retVal = [];

            for (int i = 0; i < _rowsBlock.ValAsInt * _colsBlock.ValAsInt; i++)
            {
                //if(i == 1)
                {
                    List<string> list = GetSuHori(i);
                    retVal.Add(list);
                }
            }

            return retVal;
        }

        // Get Solving Unit - Horigontal list
        List<string> GetSuHori(int rowNoSoduku)
        {
            List<string> retVal = [];

            int p = (int)(rowNoSoduku / _colsBlock.ValAsInt);
            int r = (int)(rowNoSoduku % _colsBlock.ValAsInt);

            for (int j = 0; j < _rowsBlock.ValAsInt * _colsBlock.ValAsInt; j++)
            {
                int q = (int)(j / _rowsBlock.ValAsInt);
                int s = (int)(j % _rowsBlock.ValAsInt);

                string blockId = GetBlockId(p, q);

                retVal.Add(string.Format(_cellIdPrefix, blockId, r, s));
            }

            return retVal;
        }

        public static string GetBlockId(int x, int y) => string.Format(_blockIdPrefix, x, y);

        public int RowsBlockMinVal { get => _rowsBlockMinVal; }
        public int RowsBlockMaxVal { get => _rowsBlockMaxVal; }
        public int ColsBlockMinVal { get => _colsBlockMinVal; }
        public int ColsBlockMaxVal { get => _colsBlockMaxVal; }

        public void ReInit(int rowsBlock, int colsBlock)
        {
            _rowsBlock.ValAsInt = rowsBlock;
            _colsBlock.ValAsInt = colsBlock;

            _positions = GetSuHoriFull().Flatten().Select(x => new KeyValuePair<string, SudokuCellInfo>(x,
                new SudokuCellInfo(cellId: x, positionType: SudokuPositionTypeEnum.None, maxCellValue: _rowsBlock.ValAsInt * _colsBlock.ValAsInt, 0)
            ))
                .ToDictionary(t => t.Key, t => t.Value);
        }

        public int GetRowsBlock()
        {
            return _rowsBlock.ValAsInt;
        }

        public int GetColsBlock()
        {
            return _colsBlock.ValAsInt;
        }
    }

    

    internal class Integer1
    {
        readonly int _minValue = int.MinValue;
        readonly int _maxValue = int.MaxValue;

        int _value = int.MinValue;

        public Integer1(int value, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            SetValue(value);
        }

        private int GetValue() => _value;
        private int SetValue(int value)
        {
            Debug.Assert(_minValue <= _maxValue);

            if (value < _minValue)
                _value = _minValue;
            else if (value > _maxValue)
                _value = _maxValue;
            else
                _value = value;

            return _value;
        }

        public int ValAsInt
        {
            get => GetValue();
            set => SetValue(value);
        }
    }

    internal class SudokuCellInfo
    {
        string _cellId = "";
        SudokuPositionTypeEnum _positionType;
        Integer1 _cellValueField1;
        readonly List<int> _hints = [];

        public SudokuCellInfo(string cellId, SudokuPositionTypeEnum positionType, int maxCellValue, int cellValue)
        {
            _cellId = cellId;
            _positionType = positionType;
            _cellValueField1 = new(cellValue, 0, maxCellValue)
            {
                ValAsInt = cellValue
            };

            _hints = Enumerable.Range(1, maxCellValue).ToList();
        }

        public int CellValue
        {
            get => _cellValueField1.ValAsInt;
        }
    }

    internal enum SudokuPositionTypeEnum
    {
        None,
        Manual,
        Solve,
    }
}
