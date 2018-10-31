using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs4.Common.Classes;

namespace Subs4.DbfLib
{
    public class DbfDAL : IDisposable
    {
        private const string CONNECTION_STRING_FMT = "Driver={{Microsoft dBASE Driver (*.dbf)}};DriverID=277;Dbq={0}";

        private OdbcConnection _connection;
        private string _directoryName;
        private string _tableName;

        public void Connect(string directoryName)
        {
            _directoryName = directoryName;

            _connection = new OdbcConnection(string.Format(CONNECTION_STRING_FMT, _directoryName));
            _connection.Open();

            CreateTable();
        }

        public void Disconnect()
        {
            _connection.Close();
        }

        public void Dispose()
        {
            if (_connection == null) return;

            if (_connection.State == ConnectionState.Closed) return;

            _connection.Close();
        }

        private void CreateTable()
        {
            _tableName = string.Format("{0:yyMM}{1}{2}.DBF", DateTime.Today, "04", "28");
            var filename = Path.Combine(_directoryName, _tableName);

            using (var fs = File.Create(filename))
            {
                var original = Properties.Resources.original;
                fs.Write(original, 0, original.Length);
            }
        }

        public void AddPersonBenefits(Person person)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = string.Format("INSERT INTO [{0}]", _tableName) +
                                  " (pfr, fm, im, ot, dtr, indx, nsp, ulc, dom, korp, kvar, komnata, cat_id,  pr, org_id, usluga, summa)" +
                                  " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                // parameters should follow in strictly this order
                cmd.Parameters.AddWithValue("@pfr", person.SNILS);
                cmd.Parameters.AddWithValue("@fm", person.LastName);
                cmd.Parameters.AddWithValue("@im", person.FirstName);
                cmd.Parameters.AddWithValue("@ot", person.MiddleName);
                cmd.Parameters.AddWithValue("@dtr", person.DOB.ToShortDateString());
                cmd.Parameters.AddWithValue("@indx", person.Address.ZipCode);
                cmd.Parameters.AddWithValue("@nsp", person.Address.City);
                cmd.Parameters.AddWithValue("@ulc", person.Address.Street);
                cmd.Parameters.AddWithValue("@dom", person.Address.House);
                cmd.Parameters.AddWithValue("@korp", person.Address.Building);
                cmd.Parameters.AddWithValue("@kvar", person.Address.Flat);
                cmd.Parameters.AddWithValue("@komnata", person.Address.Room.ToString());
                cmd.Parameters.AddWithValue("@catid", "");
                cmd.Parameters.AddWithValue("@pr", string.Empty);
                cmd.Parameters.AddWithValue("@orgid", "28");

                cmd.Parameters.AddWithValue("@usluga", "00");
                cmd.Parameters.AddWithValue("@summa", 0.0);

                foreach (var benefit in person.Benefits)
                {
                    cmd.Parameters["@catid"].Value = benefit.CategoryCode;
                    cmd.Parameters["@usluga"].Value = benefit.ServiceGroupCode;
                    cmd.Parameters["@summa"].Value = benefit.Value;

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
