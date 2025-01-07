using BlazorWasmGames2.Pages;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlazorWasmGames2.Code
{
    internal class SudokuGame
    {
        Integer1 _rowsBlock = new(2, 2, 3), _colsBlock = new(2, 2, 3);

        const string _blockIdPrefix = "B[{0},{1}]";                // block names are B[{0-based rows},{0-based cols}]
        const string _cellIdPrefix = "{0}:C[{1},{2}]";             // cell names are BLOCK_ID:[{0-based rows},{0-based cols}]

        public SudokuGame()
        {
            //Init();
        }

        public void Resize(int rowsBlock, int colsBlock)
        {
            _rowsBlock.ValAsInt = rowsBlock;
            _colsBlock.ValAsInt = colsBlock;
        }

        public SudokuUi GetSudokuUi()
        {
            SudokuUi sudokuUi = new SudokuUi();

            sudokuUi.RowsBlock = _rowsBlock.ValAsInt;
            sudokuUi.ColsBlock = _colsBlock.ValAsInt;

            return sudokuUi;
        }
    }

    

    internal class Integer1
    {
        int _minValue = int.MinValue;
        int _maxValue = int.MaxValue;

        int _value = int.MinValue;

        public Integer1(int value, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            SetValue(value);
        }
        //public static implicit operator int(Integer1 d) => d.GetValue();
        //public static explicit operator Integer1(int b) => new Integer1(b);

        private int GetValue() => _value;
        private int SetValue(int value)
        {
            if (value < _minValue)
                _value = _minValue;
            if (value > _maxValue)
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

    internal class CellIdValueField1(int maxValue)
    {
        readonly int _maxValue = maxValue;
        string _value = "";

        public int ValAsInt
        {
            get => GenericMethods.StrToIntDef(Val, default);
            set => _value = value.ToString(CultureInfo.InvariantCulture);
        }

        string Val
        {
            get
            {
                int retVal;

                if (string.IsNullOrEmpty(_value))
                {
                    return "";
                }
                else if (int.TryParse(_value, out retVal) == false)
                {
                    return "";
                }
                else
                {
                    if (retVal < 1)
                        retVal = 0;
                    else if (retVal > _maxValue)
                        retVal = _maxValue;
                }

                string result;
                if (retVal != 0)
                    result = retVal.ToString(CultureInfo.InvariantCulture);
                else
                    result = "";

                return result;
            }
            set
            {

                int retVal = 0;

                if (string.IsNullOrEmpty(value))
                {
                    _value = "";
                }
                else if (int.TryParse(value, out retVal) == false)
                {
                    _value = "";
                }
                else
                {
                    if (retVal < 1)
                        retVal = 0;
                    else if (retVal > _maxValue)
                        retVal = _maxValue;
                }
                string result;

                if (retVal != 0)
                    result = retVal.ToString(CultureInfo.InvariantCulture);
                else
                    result = "";

                _value = result;
            }
        }
    }

    internal class SudokuCellInfo
    {
        string _cellId = "";
        SudokuPositionTypeEnum _positionType;
        CellIdValueField1 _cellValueField1;
        readonly List<int> _hints = [];

        public SudokuCellInfo(string cellId, SudokuPositionTypeEnum positionType, int maxCellValue, int cellValue)
        {
            _cellId = cellId;
            _positionType = positionType;
            _cellValueField1 = new(maxCellValue)
            {
                ValAsInt = cellValue
            };

            _hints = Enumerable.Range(1, maxCellValue).ToList();
        }
    }

    internal enum SudokuPositionTypeEnum
    {
        None,
        Manual,
        Solve,
    }
}
