
using System.Data.SqlClient;

namespace KAFKA.CONSUMER.Data.Interfaces
{
    public interface IAcessoBancoDados
    {
        public string ObtemStringDeConexao();
        public void AbreConexao(SqlConnection connection); 
        public void FechaConexao(SqlConnection connection); 
    }
}
