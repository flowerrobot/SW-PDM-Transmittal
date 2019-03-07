namespace TransmittalManager
{
    public interface ICloseable
    {
        bool Close();
        bool DialogResult { get; set; }
    }
}
