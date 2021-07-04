using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ajuda_
{
    class Globais
    {
        public static string ComputeSha256Hash(string rawData) {
            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static int removerTipo;
        public static int tipoPERFIL;
        public static int idPerfil;
        public static int idPerfilUTIL;
        public static int idPaciente;
        public static int idLoggedFunc=0;
        public static int maxMarcacao;
        public static int job;
        public static string loggedId="";
        public static Boolean is2Authenticator = false;
        public static Boolean Admin = false;
        public static bool funcON;
        public static string code;
        public static string Email;
        public static string Nome;
        public static string Token = "";
       // public static string Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2MjI4MTMwNjEsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzcxIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNzEifQ.JQgc6hLXETC7PUdZTWugNKLpPzKY2DCqPjojLhm5kPA";
        public static string baseURL = "http://www.ajudamais.somee.com/api/v1/";



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
