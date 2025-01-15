using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plot.Skia
{
    internal interface IUserActionResponse
    {
        bool Execute(Figure figure, IUserAction userInput);
    }
}
