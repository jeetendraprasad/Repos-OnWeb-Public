﻿namespace BlazorWasmGamesProj.Code
{
    public class ExtensionMethods
    {
    }

    public static class ListExtension
    {
        /// <summary>
        /// Creates a single flat list from a list of lists input. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inListOfLists"></param>
        /// <returns>A single flatten list from a list of lists input</returns>
        public static List<T> Flatten<T>(this List<List<T>> inListOfLists)
        {
            List<T> outList = new List<T>();
            foreach (List<T> list in inListOfLists)
            {
                outList.AddRange(list);
            }
            return outList;
        }
    }
}
