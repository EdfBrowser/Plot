namespace Plot.Skia.Interaction
{
    internal interface IMouseButtonAction : IMouseAction
    {
        string ButtonName { get; }
        bool Pressed { get; }
    }
}
