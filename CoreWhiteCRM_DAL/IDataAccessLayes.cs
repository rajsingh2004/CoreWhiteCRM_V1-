using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreWhiteCRM_Common.Enums;
using CoreWhiteCRM_Common.Utility;

namespace CoreWhiteCRM_DAL
{
    public interface IDataAccessLayes
    {
        Response GetDataSet();

        Response GetDataTable();

        Response ExecuteScalar();

        Response ExecuteNonQuery();
    }
}
