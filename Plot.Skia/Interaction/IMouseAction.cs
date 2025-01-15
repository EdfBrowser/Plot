namespace Plot.Skia
{
    internal interface IMouseAction : IUserAction
    {
        PointF Point { get; }
    }
}
