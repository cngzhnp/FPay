using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Db_fpay.Classes
{
    class Event
    {
        private string event_id;
        private string event_name;
        private string event_msg;
        private DBConnect db;
        private MySqlCommand cmd;
        private List<Event> retVal;
        public Event()
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
        public List<Event> Select(List<string> tableEvent, string query)
        {
            retVal = new List<Event>();
            if (db.OpenConnection() == true)
            {
                cmd = new MySqlCommand(query, db.getConnection());
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    Event e = new Event();
                    for (int i = 0; i < tableEvent.Count; i++)
                    {
                        if (tableEvent[i] == "event_id")
                            e.set_event_id(Convert.ToString(dataReader["event_id"] + ""));
                        if (tableEvent[i] == "event_msg")
                            e.set_event_msg(dataReader["event_msg"] + "");
                        if (tableEvent[i] == "event_name")
                            e.set_event_name(dataReader["event_name"] + "");
                    }
                    retVal.Add(e);
                }
                dataReader.Close();
                db.CloseConnection();
            }
            return retVal;
        }
        public string get_event_id()
        {
            return event_id;
        }
        public void set_event_id(string _event_id)
        {
            this.event_id = _event_id;
        }
        public string get_event_name()
        {
            return event_name;
        }
        public void set_event_name(string _event_name)
        {
            this.event_name = _event_name;
        }
        public string get_event_msg()
        {
            return event_msg;
        }
        public void set_event_msg(string _event_msg)
        {
            this.event_msg = _event_msg;
        }
        public List<Event> get_list_event()
        {
            return retVal;
        }
    }
}
