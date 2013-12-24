using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Db_fpay.Classes
{
    class Server
    {
        private string server_id;
        private string server_name;
        private string server_ip;
        private string server_info;
        private MySqlCommand cmd;
        private DBConnect db = null;
        private List<Server> retVal;
        public Server()
        {
            db = new DBConnect();
        }
        public void InsDel(string query)
        {
            if (db.OpenConnection() == true)
            {
                cmd = new MySqlCommand(query, db.getConnection());
                cmd.ExecuteNonQuery();
                db.CloseConnection();
            }
        }
        public void Update(string query)
        {
            if (db.OpenConnection() == true)
            {
                cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = db.getConnection();
                cmd.ExecuteNonQuery();
                db.CloseConnection();
            }
        }
        public List<Server> Select(List<string> tableServer, string query)
        {
            retVal = new List<Server>();
            if (db.OpenConnection() == true)
            {
                cmd = new MySqlCommand(query, db.getConnection());
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    Server s = new Server();
                    for (int i = 0; i < tableServer.Count; i++)
                    {
                        if (tableServer[i] == "server_id")
                            s.set_server_id(Convert.ToString(dataReader["server_id"] + ""));
                        if (tableServer[i] == "server_ip")
                            s.set_server_ip(dataReader["server_ip"] + "");
                        if (tableServer[i] == "server_name")
                            s.set_server_name(dataReader["server_name"] + "");
                    }
                    retVal.Add(s);
                }
                dataReader.Close();
                db.CloseConnection();
            }            
            return retVal;
        }
        public string get_server_id()
        {
            return server_id;
        }
        public void set_server_id(string _server_id)
        {
            this.server_id = _server_id;
        }
        public string get_server_name()
        {
            return server_name;
        }
        public void set_server_name(string _server_name)
        {
            this.server_name = _server_name;
        }
        public string get_server_ip()
        {
            return server_ip;
        }
        public void set_server_ip(string _server_ip)
        {
            this.server_ip = _server_ip;
        }
        public string get_server_info()
        {
            return server_info;
        }
        public void set_server_info(string _server_info)
        {
            this.server_info = _server_info;
        }
        public List<Server> get_list_server()
        {
            return retVal;
        }
    }
}
