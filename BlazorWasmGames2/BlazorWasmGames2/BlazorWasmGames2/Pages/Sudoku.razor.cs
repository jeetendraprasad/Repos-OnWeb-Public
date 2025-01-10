using BlazorWasmGames2.Code;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorWasmGames2.Pages
{
    public partial class Sudoku
    {
        SudokuGame _sudokuGame = new();
        readonly SudokuUi _sudokuUi = new();

        bool _render = true;

        protected override bool ShouldRender() => _render;

        private void OnChangeGrid()
        {
            _render = false;

            _sudokuGame.Resize(_sudokuUi.RowsBlock, _sudokuUi.ColsBlock);
            _sudokuUi.GridSize = _sudokuUi.RowsBlock * _sudokuUi.ColsBlock;

            _render = true;
        }

        async Task OnChangeGridSize(int value, string controlId)
        {
            _render = false;

            if ("rows_size" == controlId)
                _sudokuUi.SetRowsBlock (value);
            else if ("cols_size" == controlId)
                _sudokuUi.SetColsBlock(value);
            else { ; }

            //UpdateUISizeBindings();
            //_sudokuUi.GridSize = _sudokuUi.RowsBlock * _sudokuUi.ColsBlock;
            Integer1 rowsBlock1 = new(0, _sudokuGame.RowsBlockMinVal, _sudokuGame.RowsBlockMaxVal);
            Integer1 colsBlock1 = new(0, _sudokuGame.ColsBlockMinVal, _sudokuGame.ColsBlockMaxVal);

            rowsBlock1.ValAsInt = _sudokuUi.RowsBlock;
            colsBlock1.ValAsInt = _sudokuUi.ColsBlock;

            _sudokuUi.SetRowsBlock (rowsBlock1.ValAsInt);
            _sudokuUi.SetColsBlock (colsBlock1.ValAsInt);

            Console.WriteLine(JsonSerializer.Serialize(_sudokuUi));

            _render = true;
            await Task.FromResult(0);
        }

        protected override async Task OnInitializedAsync()
        {
            _sudokuGame = new SudokuGame();
            //_sudokuUi = _sudokuGame.GetSudokuUi();
            UpdateUISizeBindings();
            _sudokuUi.GridSize = _sudokuUi.RowsBlock * _sudokuUi.ColsBlock;

            await Task.FromResult(0);
        }

        void UpdateUISizeBindings()
        {
            _sudokuUi.SetRowsBlock(_sudokuGame.GetRowsBlock());
            _sudokuUi.SetColsBlock(_sudokuGame.GetColsBlock());
        }

    }

    internal class SudokuUi
    {
        int _rowsBlock, _colsBlock;

        public int RowsBlock
        {
            get{ return _rowsBlock; }
        }
        public int ColsBlock
        {
            get { return _colsBlock; }
        }

        public void SetRowsBlock(int value)
        {
            GridUpdated = true;
            _rowsBlock = value;
        }

        public void SetColsBlock(int value)
        {
            GridUpdated = true;
            _colsBlock = value;
        }

        public bool GridUpdated { get; private set; } = false;

        public int GridSize { get; set; } = 1;
    }
}
