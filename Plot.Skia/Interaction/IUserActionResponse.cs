namespace Plot.Skia
{
    internal interface IUserActionResponse
    {
        CursorType CursorType { get; }

        bool Execute(Figure figure, IUserAction userInput);
        void Reset(Figure figure);
    }
}
