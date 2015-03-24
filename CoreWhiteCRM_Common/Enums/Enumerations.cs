using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWhiteCRM_Common.Enums
{
    public enum EnumResponseCode
    {
        Success = 1,
        Fail = 2,
        Error = 3
    }

    public enum EnumDirectionType
    {
        Input = 1,
        Output = 2,
        InputOutput = 3,
        ReturnValue = 6
    }

    public enum EnumDBCommandType
    {
        // Summary:
        //     An SQL text command. (Default.)
        Text = 1,
        //
        // Summary:
        //     The name of a stored procedure.
        StoredProcedure = 4,
        //
        // Summary:
        //     The name of a table.
        TableDirect = 512,
    }
}
