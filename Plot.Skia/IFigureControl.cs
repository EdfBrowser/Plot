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

        // TODO: 是否让用户自己来管理
        void SetCursor(CursorType cursorType);

    }
}
