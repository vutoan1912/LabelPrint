using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Business
{
    static class Auth
    {
        private static bool _auth = false;
        private static string _token = "";
        public static bool Authenticated
        {
            get { return _auth; }

            set { _auth = value; }
        }

        public static bool Login(string Username, string PrinterId)
        {
            String timeStamp = Common.GetTimestamp(new DateTime());

            return true;
        }

        public static string Token
        {
            get { return _token; }
            set
            {
                _token = value;
                if (ValueChanged != null) ValueChanged(value);
            }
        }
        public static event TokenValueChangedEventHandler ValueChanged;

    }
    delegate void TokenValueChangedEventHandler(string value);
}
