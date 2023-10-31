using MySqlConnector;
using System.Text.RegularExpressions;

public class DBManager
{
    public static MySqlConnection mysql = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataBase">数据库名称</param>
    /// <param name="server">IP</param>
    /// <param name="port">端口</param>
    /// <param name="name">用户名</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    public static bool Connect(string dataBase,string server,int port,string name,string password)
    {
        mysql.ConnectionString = string.Format("database={0};server={1};port={2};name={3};passwork={4};", dataBase, server, port, name, password);
        try
        {
            mysql.Open();
            return true;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    //是否是安全字符串
    //sql语句在结尾加入/等危险字符可以实现删除库操作
    public static bool IsSafeString(string str)
    {
        if(string.IsNullOrEmpty(str))
            return false;
        return !Regex.IsMatch(str, @"[-|;|,|.|\/|\(|\)|\{\|}|%|@|\*|!|\'|]");//不允许包含这些字符
    }

    //账户是否存在
    public static bool IsAccountExist(string user)
    {
        if(!IsSafeString(user))
            return false;

        try
        {
            string str = string.Format("select * from account where name = '{0}'", user);
            MySqlCommand cmd = new MySqlCommand(str,mysql);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool has = reader.HasRows;
            reader.Close();
            return has;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

    }
    //注册
    public static bool Register(string user,string password)
    {
        if(!IsSafeString(user)||!IsSafeString(password))
            return false;
        if (IsAccountExist(user))
            return false;

        try
        {
            string str = string.Format("insert into account set name='{0}',password='{1}'", user, password);
            MySqlCommand cmd = new MySqlCommand(str, mysql);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Close();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
    //登录
    public static bool Login(string user, string password)
    {
        if (!IsSafeString(user) || !IsSafeString(password))
            return false;
        try
        {
            string str = string.Format("select * from account where name = '{0}' and password='{1}'", user, password);
            MySqlCommand cmd = new MySqlCommand(str, mysql);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool has = reader.HasRows;
            reader.Close();
            return has;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}