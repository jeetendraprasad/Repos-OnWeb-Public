﻿namespace BlazorWasmGames2.Code
{
    public class GenericMethods
    {
        public static int StrToIntDef(string s, int @default) =>
            int.TryParse(s, out int result) ? result : @default;
    }
}
