using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Db_fpay.Classes
{
    class Client
    {
        private string client_id = null;
        private string client_name = null;
        private string client_ip = null;
        private string server_id = null;
        private string client_info = null;
        private DBConnect db;
        private MySqlCommand cmd;
        public List<Client> retVal;
        public Client()
        {
            db = new DBConnect();
        }
        public void InsDel(string query)
        {
            if (db.OpenConnection() == true)
            {
                cmd = new MySqlCommand(query, db.getConnection());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Calisti!");
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
        public List<Client> Select(List<string>tableClient, string query )
        {
            retVal = new List<Client>();
            if (db.OpenConnection() == true)
            {
                cmd = new MySqlCommand(query, db.getConnection());
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    Client cli = new Client();
                    for (int i = 0; i < tableClient.Count; i++)
                    {
                        if (tableClient[i] == "client_info")
                            cli.set_client_info(dataReader["client_info"].ToString());
                        if (tableClient[i] == "client_id")
                            cli.set_client_id((dataReader["client_id"]).ToString());
                        if (tableClient[i] == "client_name")
                            cli.set_client_name(dataReader["client_name"].ToString());
                        if (tableClient[i] == "server_id")
                            cli.set_server_id((dataReader["server_id"].ToString()));
                        if (tableClient[i] == "client_ip")
                            cli.set_client_ip(dataReader["client_ip"].ToString());
                    }
                    retVal.Add(cli);
                }
                dataReader.Close();
                db.CloseConnection();
            }
            return retVal;
        }
        public string get_client_id()
        {
            return client_id;
        }
        public void set_client_id(string _client_id)
        {
            this.client_id = _client_id;
        }
        public string get_client_name()
        {
            return client_name;
        }
        public void set_client_name(string _client_name)
        {
            this.client_name = _client_name;
        }
        public string get_client_ip()
        {
            return client_ip;
        }
        public void set_client_ip(string _client_ip)
        {
            this.client_ip = _client_ip;
        }
        public string get_server_id()
        {
            return server_id;
        }
        public void set_server_id(string _server_id)
        {
            this.server_id = _server_id;
        }
        public string get_client_info()
        {
            return client_info;
        }
        public void set_client_info(string _client_info)
        {
            this.client_info = _client_info;
        }
    }
}
