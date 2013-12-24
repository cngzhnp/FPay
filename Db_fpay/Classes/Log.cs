using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using MySql.Data;

namespace Db_fpay.Classes
{
    class Log
    {
        private string log_id;
        private string server_id;
        private string client_id;
        private string event_id;
        private string log_info;
        private string log_time;
        private DBConnect db;
        private MySqlCommand cmd;
        public List<Log> retVal;
        public Log()
        {
           db  = new DBConnect();
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
        public List<Log> Select(List<string>tableLog, string query)
        {
            retVal = new List<Log>();
            if (db.OpenConnection() == true)
            {
                cmd = new MySqlCommand(query, db.getConnection());
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    Log l = new Log();
                    for (int i = 0; i < tableLog.Count; i++)
                    {
                        if (tableLog[i] == "client_id")
                            l.set_client_id(Convert.ToString(dataReader["client_id"] + ""));
                        if (tableLog[i] == "event_id")
                            l.set_event_id(Convert.ToString(dataReader["event_id"] + ""));
                        if (tableLog[i] == "log_id")
                            l.set_log_id(Convert.ToString(dataReader["log_id"] + ""));
                        if (tableLog[i] == "log_info")
                            l.set_log_info(dataReader["log_info"] + "");
                        if (tableLog[i] == "log_time")
                            l.set_log_time(Convert.ToString(dataReader["log_time"] + ""));
                        if (tableLog[i] == "server_id")
                            l.set_server_id(Convert.ToString(dataReader["server_id"] + ""));
                    }
                    retVal.Add(l);
                }
                dataReader.Close();
                db.CloseConnection();
            }
            return retVal;
        }
        public string get_log_id()
        {
            return log_id;
        }
        public void set_log_id(string _log_id)
        {
            this.log_id = _log_id;
        }
        public string get_server_id()
        {
            return server_id;
        }
        public void set_server_id(string _server_id)
        {
            this.server_id = _server_id; 
        }
        public string get_client_id()
        {
            return client_id; 
        }
        public void set_client_id(string _client_id)
        {
            this.client_id = _client_id;
        }
        public string get_event_id()
        {
            return event_id;
        }
        public void set_event_id(string _event_id)
        {
            this.event_id = _event_id;
        }
        public string get_log_info()
        {
            return log_info;
        }
        public void set_log_info(string _log_info)
        {
            this.log_info = _log_info;
        }
        public string get_log_time()
        {
            return log_time;
        }
        public void set_log_time(string _log_time)
        {
            this.log_time = _log_time;
        }
    }
}
