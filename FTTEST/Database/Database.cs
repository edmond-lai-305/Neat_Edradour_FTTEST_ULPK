using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FTTEST.Database
{
    public class Database
    {
        public delegate void ErrorsHandler(ErrorsEvent errors);
       
    }
    public class ErrorsEvent : EventArgs
    {
        public readonly string Message;

        public ErrorsEvent(string msg)
        {
            this.Message = msg;
        }
    }
}
