using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ajuda_
{
    class Globais
    {
        public static string job;
        public static string loggedId="";
        public static Boolean is2Authenticator = false;
        public static string code;
        public static string Email;
        public static string Token = "";
        public static string baseURL = "https://localhost:44378/api/v1/";



        public class Filter
        {
            public string Field { get; set; }
            public string Operator { get; set; }
            public object Value { get; set; }
            public string Logic { get; set; }
            public IEnumerable<Filter> Filters { get; set; }
        }

        public class Sort
        {
            public string Field { get; set; }
            public string Dir { get; set; }
        }

        public class FilterDTO
        {
            public int Offset { get; set; }
            public int Limit { get; set; }
            public IEnumerable<Sort> Sort { get; set; }
            public Filter Filter { get; set; }
        }

    }
}
