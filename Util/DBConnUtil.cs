using Microsoft.Data.SqlClient;

namespace YourNamespace.util;
public class DBConnUtil
{
    public static SqlConnection GetDBConn(string propertyFileName)
    {
        string connString = DBPropertyUtil.GetConnectionString(propertyFileName);
        SqlConnection conn = new SqlConnection(connString);
        conn.Open();
        return conn;
    }
}