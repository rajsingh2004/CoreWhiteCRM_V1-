using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using CoreWhiteCRM_Common.Enums;

namespace CoreWhiteCRM_Common.Utility
{
    public class Response 
    {
        private EnumResponseCode _ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public EnumResponseCode ResponseCode 
        { 
            get
            {
                if ((int)this._ResponseCode == 0)
                    return EnumResponseCode.Success;
                else
                    return this._ResponseCode;
            }
            set
            {
                this._ResponseCode = value;
            }
        }
        
        public DataTable DataTable { get; set; }
        
        public DataSet DataSet { get; set; }
        
        public int NoOfRowsEffected { get; set; }
        
        public object ScalarResult { get; set; }
    }
}
