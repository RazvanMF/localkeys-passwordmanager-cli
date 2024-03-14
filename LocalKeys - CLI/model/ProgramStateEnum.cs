using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalKeys___CLI.model
{
    internal enum ProgramState : int
    {
        Fatal = -5585,

        LoginSuccess = 1,
        LoginFailure = -1,
        RegistrationSuccess = 2,
        RegistrationFailure = -2,

        DisplaySuccess = 3,
        DisplayFailure = -3,
        AddSuccess = 4,
        AddFailure = -4,
        DeleteSuccess = 5,
        DeleteFailure = -5,
        UpdateSuccess = 6,
        UpdateFailure = -6,

        Logout = 10,
    }
}
