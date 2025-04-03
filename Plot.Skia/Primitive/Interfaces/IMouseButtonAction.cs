namespace Plot.Skia
{
    internal interface IMouseButtonAction : IMouseAction
    {
        string ButtonName { get; }
        bool Pressed { get; }
    }
}
