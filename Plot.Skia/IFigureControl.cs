namespace Plot.Skia
{
    public interface IFigureControl
    {
        Figure Figure { get; }

        float DisplayScale { get; }

        void Refresh();

        void Reset();
        void Reset(Figure figure, bool disposeOldFigure);

        float DetectDisplayScale();

        void SetCursor(CursorType cursorType);

    }
}
