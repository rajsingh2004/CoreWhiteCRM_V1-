using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CoreWhiteCRM_Common.Enums;

namespace CoreWhiteCRM_Common.Utility
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DBHelpAttribute : Attribute
    {
        public string ParameterName { get; set; }

        private EnumDirectionType _DirectionType { get; set; }

        public EnumDirectionType DirectionType
        {
            get
            {
                if (this._DirectionType == 0)
                    return EnumDirectionType.Input;
                else
                    return this._DirectionType;
            }
            set
            {
                this._DirectionType = value;
            }
        }

        private int _Size { get; set; }

        public int Size
        {
            get
            {
                if (this._Size == 0)
                    return 1000;
                else
                    return this._Size;
            }
            set
            {
                this._Size = value;
            }
        }
    }

}
