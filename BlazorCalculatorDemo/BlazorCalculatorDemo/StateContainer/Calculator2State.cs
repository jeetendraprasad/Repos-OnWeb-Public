namespace BlazorCalculatorDemo.StateContainer
{
    public class Calculator2State
    {
        public Calculator2StateFixed Fixed { get; private set; } = new();

        public CalculatorStateVolatile Volatile { get; set; } = new();
    }
}
