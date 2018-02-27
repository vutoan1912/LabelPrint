using EasyHttp.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Business
{
    public class HTTP
    {
        private static HTTP _instance;

        private HTTP()
        {

        }

        public static HTTP Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HTTP();
                }
                return _instance;
            }
        }

        public HttpResponse Post(string url, Object param)
        {
            string full_url = Config.API_URL + url;
            var http = new HttpClient();
            http.Request.Accept = HttpContentTypes.ApplicationJson;
            http.Request.ContentType = HttpContentTypes.ApplicationXWwwFormUrlEncoded;
            http.Request.AddExtraHeader("Authorization", Config.API_KEY);
            var response = http.Post(full_url, param, HttpContentTypes.ApplicationJson);
            return response;
        }

        public HttpResponse Get(string url, Object param = null)
        {
            string full_url = Config.API_URL + url;
            var http = new HttpClient();
            http.Request.Accept = HttpContentTypes.ApplicationJson;
            http.Request.AddExtraHeader("Authorization", Config.API_KEY);
            var response = http.Get(full_url, param);
            return response;
        }
    }

    class ApiResponse
    {
        private bool _status = true;
        private String message = "Good";
        private dynamic _dynamicBody;
        private string _raw_text;

        public dynamic DynamicBody
        {
            get { return _dynamicBody; }
            set { _dynamicBody = value; }
        }

        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public string RawText
        {
            get { return _raw_text; }
            set { _raw_text = value; }
        }
    }
}
