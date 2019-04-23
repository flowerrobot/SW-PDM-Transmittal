using DockingAdapterMVVM;

namespace TestApp
{
    public class NameViewModel :IDockElement
    {
        public string Name { get; set; } = "My Name is";
        public string Header { get; set; } = "I Give Names out";
        public DockState State { get; set; } = DockState.Document;
    }
}
