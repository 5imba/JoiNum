public class ValueChangedEventArgs : System.EventArgs
{
    public int value { get; set; }

    public ValueChangedEventArgs(int value)
    {
        this.value = value;
    }
}
