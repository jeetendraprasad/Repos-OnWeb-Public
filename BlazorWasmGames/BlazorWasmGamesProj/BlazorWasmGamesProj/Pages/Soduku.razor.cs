using BlazorWasmGamesProj.Code;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Data.Common;
using System.Diagnostics;
using System.Text.Json;
using System.Timers;
using static System.Reflection.Metadata.BlobBuilder;

namespace BlazorWasmGamesProj.Pages
{
    public partial class Soduku : IDisposable
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


        readonly SodukuGame _sodukuGame;

        //CellIdValueField _cellIdValueField;



        readonly int sodukuSizeInPx = 900;

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

        System.Timers.Timer? timer = new();

        bool debuggingOn = false;
        int index = 0;
        string highlightCell = "";


        async Task AddPosition(ChangeEventArgs e, KeyValuePair<string, SodukuCellInfo> item)
        {
            string varupdatedValue = e.Value?.ToString() ?? "";

            item.Value.CellValueVal = varupdatedValue;

            item.Value.SetPositionType(PositionTypeEnum.Manual);

            await Task.FromResult(0);
        }


        string GetSodukuStyle() => $"width: {sodukuSizeInPx}px;height: {sodukuSizeInPx}px;";
        string GetCellStyle(string? cellId = null)
        {
            string style = $"width: {sodukuSizeInPx / rowsBlock / colsBlock}px;height: {sodukuSizeInPx / rowsBlock / colsBlock}px;float:left;border: solid;"; // display:flex; flex-direction: column;font-size: 2em;

            if (cellId != null)
            {
                GetBlockRowAndBlockColFromCellId(cellId, out int x, out int y);
                string color = GetBlockColor(x, y);
                style += $"background-color:{color};";
            }

            return style;
        }

        private static void GetBlockRowAndBlockColFromCellId(string cellId, out int row, out int col)
        {
            // cellIdPrefix = "{0}:C[{1},{2}]"
            string[]? splitted = cellId.Split('[', ',', ']');
            _ = int.TryParse(splitted[1], out row);
            _ = int.TryParse(splitted[2], out col);
        }

        string GetBlockColor(int row, int col)
        {
            int ind = rowsBlock * row + col;
            List<string> basicHtmlColors = ["silver", "gray", "red", "yellow", "lime", "aqua", "teal", "olive", "fuchsia", "purple", "green", "maroon", "blue", "navy", "white", "black",];
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

            //foreach (KeyValuePair<string, string> elem in _sodukuGame.Positions)
            //{
            //    Console.WriteLine("{0} and {1}", elem.Key, elem.Value);
            //}

            //Console.WriteLine("Moves : " + string.Join(',', _sodukuGame.Moves));

            await Task.FromResult(0);
            //await Task.Delay(1);
        }

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
            if (timer != null && !timer.Enabled)
            {
                timer = new System.Timers.Timer
                {
                    Interval = 500
                };
                timer.Elapsed += OnTimerElapsed;
                timer.Enabled = true;
            }
        }

        void DeInitTimer()
        {
            if (timer != null && timer.Enabled)
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

        async Task SaveNSolve()
        {
            _sodukuGame.SaveNSolve();

            await Task.FromResult(0);
        }

        public void Dispose()
        {
            timer?.Dispose();

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
            }
        }
    }
}
