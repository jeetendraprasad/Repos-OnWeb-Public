using BlazorWasmGamesProj.Code;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;
using System.Text.Json;
using System.Timers;
using static System.Reflection.Metadata.BlobBuilder;

namespace BlazorWasmGamesProj.Pages
{
    public partial class Soduku
    {
        int rowsBlock = 2;
        int colsBlock = 2;

        //bool editMode = true;

        public Soduku()
        {
            _sodukuGame = new(rowsBlock, colsBlock);
            //_cellIdValueField = new(rowsBlock * colsBlock * rowsBlock * colsBlock, rowsBlock * colsBlock);
            //_sodukuGame._positions = [];
        }


        SodukuGame _sodukuGame;

        //CellIdValueField _cellIdValueField;



        int sodukuSizeInPx = 900;

        private async Task ValidateAndSodukuResize()
        {
            rowsBlock = rowsBlock < 2 ? 2 : rowsBlock;
            colsBlock = colsBlock < 2 ? 2 : colsBlock;

            rowsBlock = rowsBlock > 3 ? 3 : rowsBlock;
            colsBlock = colsBlock > 3 ? 3 : colsBlock;

            _sodukuGame.ReInit(rowsBlock, colsBlock);

            //_cellIdValueField.Init(rowsBlock * colsBlock * rowsBlock * colsBlock, rowsBlock * colsBlock);
            //_sodukuGame._positions = [];

            _sodukuGame.Recalculate();

            await Task.FromResult(0);
        }

        System.Timers.Timer timer = new System.Timers.Timer();

        bool debuggingOn = false;
        int index = 0;
        string highlightCell = "";


        string GetSodukuStyle() => $"width: {sodukuSizeInPx}px;height: {sodukuSizeInPx}px;";
        string GetBlockRowStyle() => $"width: {sodukuSizeInPx / colsBlock}px;height: {sodukuSizeInPx / rowsBlock}px;float:left;";
        string GetBlockStyle(int colorIndex) => $"width: {sodukuSizeInPx / colsBlock}px;height: {sodukuSizeInPx / rowsBlock}px;float:left;background-color:red;";
        //string GetBlockId(int x, int y) => string.Format(blockIdPrefix, x, y);
        //string GetCellId(string blockId, int x, int y) => string.Format(cellIdPrefix, blockId, x, y);
        string GetCellStyle(string? cellId = null)
        {
            string style = $"width: {sodukuSizeInPx / rowsBlock / colsBlock}px;height: {sodukuSizeInPx / rowsBlock / colsBlock}px;float:left;border: solid;"; // display:flex; flex-direction: column;font-size: 2em;

            if (cellId != null)
            {
                int x, y;
                GetBlockRowAndBlockColFromCellId(cellId, out x, out y);
                string color = GetBlockColor(x, y);
                style += $"background-color:{color};";
            }

            return style;
        }

        void GetBlockRowAndBlockColFromCellId(string cellId, out int row, out int col)
        {
            row = col = -1;

            // cellIdPrefix = "{0}:C[{1},{2}]"
            string[]? splitted = cellId.Split('[', ',', ']');

            int.TryParse(splitted[1], out row);
            int.TryParse(splitted[2], out col);
        }

        string GetBlockColor(int row, int col)
        {
            int ind = rowsBlock * row + col;
            List<string> basicHtmlColors = new List<string>() { "silver", "gray", "red", "yellow", "lime", "aqua", "teal", "olive", "fuchsia", "purple", "green", "maroon", "blue", "navy", "white", "black", };
            return basicHtmlColors[ind % basicHtmlColors.Count];
        }

        async Task Debug1()
        {
            index = 0;
            debuggingOn = true;
            InitTimer();
            await Task.FromResult(0);
            //DeInitTimer();
        }

        async Task Debug2()
        {
            Console.WriteLine(JsonSerializer.Serialize(_sodukuGame?.Positions ?? []));

            string key = _sodukuGame?.Positions?.Keys?.FirstOrDefault() ?? "";
            Console.WriteLine(_sodukuGame?.Positions?[key] ?? new());
            //foreach (KeyValuePair<string, string> elem in _sodukuGame.Positions)
            //{
            //    Console.WriteLine("{0} and {1}", elem.Key, elem.Value);
            //}

            //Console.WriteLine("Moves : " + string.Join(',', _sodukuGame.Moves));

            await Task.FromResult(0);
            //await Task.Delay(1);
        }

        async Task UpdatePosition(ChangeEventArgs e, string cellId, string myValue, KeyValuePair<string, SodukuCellInfo> item)
        {
            string varupdatedValue = e?.Value?.ToString() ?? "";

            _sodukuGame.Positions[cellId].CellValue.Val = varupdatedValue;

            if (varupdatedValue.Length < 2)
            {
                    item.Value.CellValue.Val = varupdatedValue;
            }
            else
            {
                string tempValue = item.Value.CellValue.Val;
                item.Value.CellValue.Val = "";
                await Task.Yield();
                item.Value.CellValue.Val = tempValue;
            }
        }

        //async Task AddMove(ChangeEventArgs args, string cellId, int index)
        //{
        //    _cellIdValueField[index] = args?.Value?.ToString() ?? "";

        //    _sodukuGame.AddMove(cellId, _cellIdValueField[index]);

        //    Console.WriteLine($"cellId = {cellId} and index = {index}"); 

        //    await Task.FromResult(0);
        //}

        protected override Task OnInitializedAsync()
        {
            //_cellIdValueField.Init(rowsBlock * colsBlock * rowsBlock * colsBlock, rowsBlock * colsBlock);
            _sodukuGame.ReInit(rowsBlock, colsBlock);
            //_sodukuGame._positions = [];
            _sodukuGame.Recalculate();
            return base.OnInitializedAsync();
        }

        void InitTimer()
        {
            if (!timer.Enabled)
            {
                timer = new System.Timers.Timer();
                timer.Interval = 500;
                timer.Elapsed += OnTimerElapsed;
                timer.Enabled = true;
            }
        }

        void DeInitTimer()
        {
            if (timer.Enabled)
            {
                timer.Stop();
                timer.Elapsed -= OnTimerElapsed;
                timer.Enabled = false;
                timer.Dispose();
            }

        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            //Console.WriteLine(DateTime.Now);

            //timer.Stop();

            if (debuggingOn)
            {
                List<string> cell = _sodukuGame.SuBlockFullFlattened; // SuVertFullFlattened; // SuHoriFullFlattened; // SuBlockFullFlattened

                if (index > cell.Count)
                {
                    debuggingOn = false;
                    DeInitTimer();
                    index = 0;
                }
                else if (index <= cell.Count && index >= 0)
                {
                    highlightCell = cell[index++];
                }
                else
                {
                    // wrong: 
                    Debug.Assert(false);
                }

            }

            InvokeAsync(
                () =>
                {
                    StateHasChanged();
                }
            );
        }

        public void Dispose()
        {
            timer.Dispose();
        }

        void ResetPositions()
        {

        }

        async Task SaveNSolve()
        {
            ResetPositions();
            _sodukuGame.SaveNSolve();

            await Task.FromResult(0);
        }
    }
}
