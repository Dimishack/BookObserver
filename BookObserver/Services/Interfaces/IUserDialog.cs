using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookObserver.Services.Interfaces
{
    internal interface IUserDialog
    {
        void ShowInformation(string message, string caption);
    }
}
