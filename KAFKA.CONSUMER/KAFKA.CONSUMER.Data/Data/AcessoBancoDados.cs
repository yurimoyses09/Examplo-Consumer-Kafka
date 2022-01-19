using KAFKA.CONSUMER.Data.Interfaces;
using System.Data.SqlClient;

namespace KAFKA.CONSUMER.Data.Data
{
    public class AcessoBancoDados : IAcessoBancoDados
    {
        public void AbreConexao(SqlConnection connection)
        {
            connection.Open();
        }

        public void FechaConexao(SqlConnection connection)
        {
            connection.Close();
        }

        public string ObtemStringDeConexao()
        {
            return @"Data Source =DESKTOP-DH4FP6N; Initial Catalog = db_dados_kafka; Integrated Security = SSPI";
        }
    }
}
