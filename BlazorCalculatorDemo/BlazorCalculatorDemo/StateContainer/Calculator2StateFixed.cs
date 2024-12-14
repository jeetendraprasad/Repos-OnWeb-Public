namespace BlazorCalculatorDemo.StateContainer
{
    public class Calculator2StateFixed
    {
        public enum ButtonTypes
        {
            Digit,             // All things that are part of numbers. 0 and 1 to 9 and dot(.)
            Equal,             // equal button
            Clear,              // clear button
        }

        public class ButtonInformation
        {
            public ButtonTypes Type { get; set; }
            public string Text { get; set; } = "";
        }
        public ButtonInformation[,] ButtonsInfo { get; } = {
            { new() { Text = "1", Type = ButtonTypes.Digit }, new() { Text = "2", Type = ButtonTypes.Digit }, },
            { new() { Text = "=", Type = ButtonTypes.Equal }, new() { Text = "C", Type = ButtonTypes.Clear }, },
        };

    }
}
