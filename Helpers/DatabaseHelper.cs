using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Helpers
{
    public static class DatabaseConfig
    {
        public static string ConnectionString =>
            "Server=localhost\\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;";
    }
}
