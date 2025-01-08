using BlazorWasmGames2.Code;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace BlazorWasmGames2.Pages
{
    public partial class Sudoku
    {
        SudokuGame _sudokuGame = new SudokuGame();
        SudokuUi _sudokuUi = new SudokuUi();

        bool _render = true;

        protected override bool ShouldRender() => _render;

        async Task OnChangeGridSize(ChangeEventArgs e, string controlId)
        {
            _render = false;

            Console.WriteLine($" inside OnChangeGridSize with param controlId = {controlId}");

            string varUpdatedValue = e.Value?.ToString() ?? "";

            Console.WriteLine($"varupdatedValue  = {varUpdatedValue}");

            if ("rows_size" == controlId)
                _sudokuUi.RowsBlock = GenericMethods.StrToIntDef(varUpdatedValue, 0);
            else if ("cols_size" == controlId)
                _sudokuUi.ColsBlock = GenericMethods.StrToIntDef(varUpdatedValue, 0);
            else
                ;



            //UpdateUISizeBindings();
            //_sudokuUi.GridSize = _sudokuUi.RowsBlock * _sudokuUi.ColsBlock;
            Integer1 rowsBlock1 = new(0, _sudokuGame.RowsBlockMinVal, _sudokuGame.RowsBlockMaxVal);
            Integer1 colsBlock1 = new(0, _sudokuGame.ColsBlockMinVal, _sudokuGame.ColsBlockMaxVal);

            rowsBlock1.ValAsInt = _sudokuUi.RowsBlock;
            colsBlock1.ValAsInt = _sudokuUi.ColsBlock;

            _sudokuUi.RowsBlock = rowsBlock1.ValAsInt;
            _sudokuUi.ColsBlock = colsBlock1.ValAsInt;

            Console.WriteLine(JsonSerializer.Serialize(_sudokuUi));

            _render = true;

            await Task.Delay(1);
            this.StateHasChanged();
            await Task.Delay(1);
            this.StateHasChanged();

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
            _sudokuUi.RowsBlock = _sudokuGame.GetRowsBlock();
            _sudokuUi.ColsBlock = _sudokuGame.GetColsBlock();
        }

    }

    internal class SudokuUi
    {
        public int RowsBlock { get; set; }
        public int ColsBlock { get; set; }

        public int GridSize { get; set; } = 1;
    }
}
