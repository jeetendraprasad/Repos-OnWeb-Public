using BlazorWasmGamesProj.Code;
using System.Diagnostics;
using System.Timers;

namespace BlazorWasmGamesProj.Pages
{
    public partial class Soduku
    {
        int rowsBlock = 2;
        int colsBlock = 2;

        bool editMode = true;

        //int GetRowsBlock() { return rowsBlock; }

        public Soduku()
        {
            sodukuGame = new(rowsBlock, colsBlock, sodukuSizeInPx);
        }


        SodukuGame sodukuGame;



        int sodukuSizeInPx = 900;

        System.Timers.Timer timer = new System.Timers.Timer();

        bool debuggingOn = false;
        int index = 0;
        string highlightCell = "";



        string blockIdPrefix = "B[{0},{1}]";                // block names are B[{0-based rows},{0-based cols}]
        string cellIdPrefix = "{0}:C[{1},{2}]";             // cell names are BLOCK_ID:[{0-based rows},{0-based cols}]


        string GetSodukuStyle() => $"width: {sodukuSizeInPx}px;height: {sodukuSizeInPx}px;";
        string GetBlockRowStyle() => $"width: {sodukuSizeInPx / colsBlock}px;height: {sodukuSizeInPx / rowsBlock}px;float:left;";
        string GetBlockStyle(int colorIndex) => $"width: {sodukuSizeInPx / colsBlock}px;height: {sodukuSizeInPx / rowsBlock}px;float:left;background-color:red;";
        string GetBlockId(int x, int y) => string.Format(blockIdPrefix, x, y);
        string GetCellId(string blockId, int x, int y) => string.Format(cellIdPrefix, blockId, x, y);
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

        List<string> SuVertFullFlattened
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

                for (int i = 0; i < rowsBlock * colsBlock; i++)
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

        List<string> SuHoriFullFlattened
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

                for (int i = 0; i < rowsBlock * colsBlock; i++)
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

            int p = (int)(rowNoSoduku / colsBlock);
            int r = (int)(rowNoSoduku % colsBlock);

            for (int j = 0; j < rowsBlock * colsBlock; j++)
            {
                int q = (int)(j / rowsBlock);
                int s = (int)(j % rowsBlock);

                string blockId = GetBlockId(p, q);

                retVal.Add(string.Format(cellIdPrefix, blockId, r, s));
            }

            return retVal;
        }

        // Get Solving Unit - Vertical list
        List<string> GetSuVert(int colNoSoduku)
        {
            List<string> retVal = new List<string>();

            int q = (int)(colNoSoduku / rowsBlock);
            int s = (colNoSoduku % rowsBlock);

            for (int i = 0; i < rowsBlock * colsBlock; i++)
            {
                int p = (int)(i / colsBlock);
                int r = (int)(i % colsBlock);

                string blockId = GetBlockId(p, q);

                retVal.Add(string.Format(cellIdPrefix, blockId, r, s));
            }

            return retVal;
        }

        List<string> SuBlockFullFlattened
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

                for (int i = 0; i < rowsBlock; i++)
                    for (int j = 0; j < colsBlock; j++)
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

            string blockId = string.Format(blockIdPrefix, rowNoBlock, colNoBlock);

            for (int i = 0; i < rowsBlock; i++)
                for (int j = 0; j < colsBlock; j++)
                {
                    string id = string.Format(cellIdPrefix, blockId, j, i);
                    Console.WriteLine($"ID = {id}");
                    retVal.Add(id);
                }


            return retVal;
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
            List<string> cells = SuBlockFullFlattened;
            await Task.FromResult(0);
            //await Task.Delay(1);
        }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        void InitTimer()
        {
            if (!timer.Enabled)
            {
                timer = new System.Timers.Timer();
                timer.Interval = 1000;
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
                List<string> cell = SuBlockFullFlattened; // SuVertFullFlattened; // SuHoriFullFlattened; // SuBlockFullFlattened

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
    }
}
