namespace Plot.Skia
{
    internal interface IUserActionResponse
    {
        bool Execute(Figure figure, IUserAction userInput);
        void Reset(Figure figure);
    }
}
