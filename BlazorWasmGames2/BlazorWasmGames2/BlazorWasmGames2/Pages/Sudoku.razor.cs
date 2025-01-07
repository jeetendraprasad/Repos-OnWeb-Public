using BlazorWasmGames2.Code;
using Microsoft.AspNetCore.Components;

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

            //_sudokuGame.Resize();

            _render = true;

            this.StateHasChanged();

            await Task.FromResult(0);
        }

        protected override async Task OnInitializedAsync()
        {
            _sudokuGame = new SudokuGame();
            _sudokuUi = _sudokuGame.GetSudokuUi();

            await Task.FromResult(0);
        }

        void UpdateUIBindings()
        {
        }

    }

    internal class SudokuUi
    {
        public int RowsBlock { get; set; }
        public int ColsBlock { get; set; }

        public int GridSize { get; set; } = 1;
    }
}
