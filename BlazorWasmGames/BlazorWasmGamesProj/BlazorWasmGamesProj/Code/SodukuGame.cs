namespace BlazorWasmGamesProj.Code
{
    public class SodukuGame
    {
        int _rowsBlock = 0, _colsBlock = 0;
        int _sodukuSizeInPx;

        const string _blockIdPrefix = "B[{0},{1}]";                // block names are B[{0-based rows},{0-based cols}]
        const string _cellIdPrefix = "{0}:C[{1},{2}]";             // cell names are BLOCK_ID:[{0-based rows},{0-based cols}]

        public SodukuGame()
        {

        }

        public SodukuGame(int rowsBlock, int colsBlock, int sodukuSizeInPx = 900)
        {
            Init(rowsBlock, colsBlock, sodukuSizeInPx);
        }

        private void Init(int rowsBlock, int colsBlock, int sodukuSizeInPx)
        {
            _rowsBlock = rowsBlock;
            _colsBlock = colsBlock;
            _sodukuSizeInPx = sodukuSizeInPx;
        }
    }
}
