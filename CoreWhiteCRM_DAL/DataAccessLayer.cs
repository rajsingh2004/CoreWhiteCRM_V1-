using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using System.Diagnostics;
using CoreWhiteCRM_Common.Enums;
using CoreWhiteCRM_Common.Utility;
using System.Configuration;

namespace CoreWhiteCRM_DAL
{
    public class DAO<T> : IDataAccessLayes
    {
        T _ObjT;
        Response _ObjResponse;
        string _ConnectionString = ConfigurationManager.ConnectionStrings["CoreWhiteConnection"].ConnectionString;
        EnumDBCommandType _DBCommandType = EnumDBCommandType.StoredProcedure;
        List<PropertyInfo> lstProperties;
        List<PropertyInfo> lstCustomAttributeProperties;

        public DAO(T ObjT)
        {
            this._ObjT = ObjT;

            GetProperties();
            GetCustomAttributeProperties();

            this._ObjResponse = new Response();
        }


        public DAO(T ObjT, EnumDBCommandType DBCommandType)
        {
            this._ObjT = ObjT;
            GetProperties();
            GetCustomAttributeProperties();

            this._ObjResponse = new Response();
            this._DBCommandType = DBCommandType;
        }


        public Response GetDataSet()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_ConnectionString))
                using (SqlCommand cmd = new SqlCommand())
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    cmd.CommandType = (CommandType)_DBCommandType;
                    cmd.Connection = con;
                    da.SelectCommand = cmd;

                    _ObjResponse = SetCommandParameters(cmd);
                    if (_ObjResponse.ResponseCode == EnumResponseCode.Error)
                        return _ObjResponse;

                    _ObjResponse.DataSet =new DataSet();
                    da.Fill(_ObjResponse.DataSet);
                    if (_ObjResponse.DataSet == null)
                    {
                        _ObjResponse.ResponseCode = EnumResponseCode.Fail;
                    }
                    else
                    {
                        SetOutputParameters(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                _ObjResponse.ResponseMessage = ex.Message;
                _ObjResponse.ResponseCode = EnumResponseCode.Error;
            }
            
            return _ObjResponse;
        }


        public Response GetDataTable()
        {
            Response ObjResponse = GetDataSet();
            if(ObjResponse.ResponseCode == EnumResponseCode.Success)
            {
                ObjResponse.DataTable = ObjResponse.DataSet.Tables[0];
            }

            return ObjResponse;
        }


        public Response ExecuteScalar()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_ConnectionString))
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = (CommandType)_DBCommandType;
                    cmd.Connection = con;
                    
                    _ObjResponse = SetCommandParameters(cmd);
                    if (_ObjResponse.ResponseCode == EnumResponseCode.Error)
                        return _ObjResponse;

                    cmd.Connection.Open();
                    _ObjResponse.ScalarResult = cmd.ExecuteScalar();
                    if (_ObjResponse.ScalarResult == null)
                    {
                        _ObjResponse.ResponseCode = EnumResponseCode.Fail;
                    }
                    else
                    {
                        SetOutputParameters(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                _ObjResponse.ResponseMessage = ex.Message;
                _ObjResponse.ResponseCode = EnumResponseCode.Error;
            }

            return _ObjResponse;
        }
       

        public Response ExecuteNonQuery()
        {
            try
            {
                using(SqlConnection con = new SqlConnection())
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = (CommandType)_DBCommandType;
                    cmd.Connection = con;

                    _ObjResponse = SetCommandParameters(cmd);
                    if (_ObjResponse.ResponseCode == EnumResponseCode.Error)
                        return _ObjResponse;

                    _ObjResponse.NoOfRowsEffected = cmd.ExecuteNonQuery();
                    if (_ObjResponse.NoOfRowsEffected == 0)
                    {
                        _ObjResponse.ResponseCode = EnumResponseCode.Fail;
                    }
                    else
                    {
                        SetOutputParameters(cmd);
                    }
                }
            }
            catch(Exception ex)
            {
                _ObjResponse.ResponseMessage = ex.Message;
                _ObjResponse.ResponseCode = EnumResponseCode.Error;
            }

            return _ObjResponse;
        }


        private Response SetCommandParameters(SqlCommand cmd)
        {
            try
            {
                string ProcedureName = GetProperties().Where(x => x.Name == "ProcedureName").Select(y => y.GetValue(_ObjT, null)).First().ToString();
                cmd.CommandText = ProcedureName;

                foreach (PropertyInfo ObjPropertyInfo in GetCustomAttributeProperties())
                {
                    DBHelpAttribute ObjHelpAttribute = ObjPropertyInfo.GetCustomAttribute(typeof(DBHelpAttribute)) as DBHelpAttribute;
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@" + ObjHelpAttribute.ParameterName,
                        Value = ObjPropertyInfo.GetValue(_ObjT),
                        Direction = (ParameterDirection)ObjHelpAttribute.DirectionType,
                        Size = ObjHelpAttribute.Size
                    });
                }
            }
            catch (Exception ex)
            {
                _ObjResponse.ResponseMessage = ex.Message;
                _ObjResponse.ResponseCode = EnumResponseCode.Error;
            }

            return _ObjResponse;
        }


        private void SetOutputParameters(SqlCommand cmd)
        {

            IEnumerable<PropertyInfo> ArrayOfProperties = GetCustomAttributeProperties().Where(p => (p.GetCustomAttribute(typeof(DBHelpAttribute)) as DBHelpAttribute).DirectionType == EnumDirectionType.Output
                                || (p.GetCustomAttribute(typeof(DBHelpAttribute)) as DBHelpAttribute).DirectionType == EnumDirectionType.InputOutput);

            if (ArrayOfProperties.Count() > 0)
            {
                foreach (PropertyInfo pi in ArrayOfProperties)
                {
                    _ObjT.GetType().GetProperty(pi.Name)
                                   .SetValue(_ObjT, cmd.Parameters["@" + (pi.GetCustomAttribute(typeof(DBHelpAttribute)) as DBHelpAttribute).ParameterName].Value);
                }
            }

        }


        private List<PropertyInfo> GetProperties()
        {
            if (lstProperties == null)
            {
                lstProperties = new List<PropertyInfo>();
                lstProperties = _ObjT.GetType().GetProperties().ToList();
            }

            return lstProperties;
        }


        private List<PropertyInfo> GetCustomAttributeProperties()
        {
            if (lstCustomAttributeProperties == null)
            {
                lstCustomAttributeProperties = new List<PropertyInfo>();
                lstCustomAttributeProperties = GetProperties().Where(p => p.GetCustomAttribute(typeof(DBHelpAttribute)) != null).ToList();
            }

            return lstCustomAttributeProperties;
        }
    }


    public class DataAccessLayer
    {
        Response _ObjResponse;
        string _ConnectionString { get; set; }

        public DataAccessLayer()
        {
            this._ObjResponse = new Response();
        }


        public Response GetDataTable(string SqlStatement)
        {
            Response ObjResponse = GetDataSet(SqlStatement);
            if (ObjResponse.ResponseCode == EnumResponseCode.Success)
            {
                ObjResponse.DataTable = new DataTable();
                ObjResponse.DataTable = ObjResponse.DataSet.Tables[0];
            }

            return ObjResponse;
        }


        public Response GetDataSet(string SqlStatement)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_ConnectionString))
                using (SqlCommand cmd = new SqlCommand())
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.CommandText = SqlStatement;
                    da.SelectCommand = cmd;

                    _ObjResponse.DataSet = new DataSet();
                    da.Fill(_ObjResponse.DataSet);
                    if (_ObjResponse.DataSet == null)
                        _ObjResponse.ResponseCode = EnumResponseCode.Fail;
                }

                return _ObjResponse;
            }
            catch (Exception ex)
            {
                _ObjResponse.ResponseMessage = ex.Message;
                _ObjResponse.ResponseCode = EnumResponseCode.Error;
            }


            return _ObjResponse;
        }


        public Response ExecuteScalar(string SqlStatement)
        {
            return ExecuteScalarByExecutingSqlStatement(SqlStatement);
        }


        private Response ExecuteScalarByExecutingSqlStatement(string SqlStatement)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_ConnectionString))
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = con;
                    cmd.CommandText = SqlStatement;

                    cmd.Connection.Open();
                    _ObjResponse.ScalarResult = cmd.ExecuteScalar();
                    if (_ObjResponse.ScalarResult == null)
                        _ObjResponse.ResponseCode = EnumResponseCode.Fail;
                }

            }
            catch (Exception ex)
            {
                _ObjResponse.ResponseMessage = ex.Message;
                _ObjResponse.ResponseCode = EnumResponseCode.Error;
            }

            return _ObjResponse;
        }

    }
}
