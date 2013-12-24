using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Text;

namespace Db_fpay.Classes
{
    class DBConnect
    {
        private MySqlConnection connection;
        private string username;
        private string password;
        private string server;
        private string database;
        public DBConnect()
        {
            Initialize(); 
        }
        public void Initialize()
        {
            server = "localhost";
            username = "root";
            password = "root";
            database = "fpay";
            string newconnection = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(newconnection);
        }
        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Database connection is lost.");
                        break;
                    case 1045:
                        MessageBox.Show("Username/password combination is wrong.");
                        break;
                }
                return false;
            }
            
        }
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
                
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Connection cannot closed.");
                return false;
            }
        }
        public MySqlConnection getConnection()
        {
            return this.connection;
        }
    }
}
