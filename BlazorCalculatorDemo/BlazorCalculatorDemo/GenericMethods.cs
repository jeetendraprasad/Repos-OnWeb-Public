﻿namespace BlazorCalculatorDemo
{
    public static class GenericMethods
    {
        public static String Eval(String expression)
        {
            string retVal = "";

            System.Data.DataTable table = new System.Data.DataTable();

            try
            {
                object solution = table.Compute(expression, String.Empty);

                retVal = Convert.ToString(Convert.ToDouble(solution));

            }
            catch (Exception)
            {
                retVal = "ERROR";
            }

            return retVal;
        }
    }
}
