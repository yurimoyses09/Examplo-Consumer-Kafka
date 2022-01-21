using KAFKA.CONSUMER.Aplicacao.Interfaces;
using KAFKA.CONSUMER.Data.Interfaces;
using KAFKA.CONSUMER.Dominios;
using System;
using System.Data.SqlClient;

namespace KAFKA.CONSUMER.Aplicacao.ComandosSql
{
    public class ComandosSql : IComandosSql
    {
        #region Construtor
        private readonly IAcessoBancoDados _acessoBancoDados;
        public ComandosSql(IAcessoBancoDados acessoBancoDados)
        {
            _acessoBancoDados = acessoBancoDados;
        }
        #endregion

        #region Geradores de comandos sql
        public string GeraComandoSqlInsert(DadosKafka dados)
        {
            return string.Format(@"
                        INSERT INTO tb_mensagem_kafka 
                        (
                            mensagem_nome,
                            mensagem_cpf,
                            mensagem_idade,
                            mensagem_status_civil
                        ) 
                        VALUES (
                            '{0}', 
                            '{1}', 
                            '{2}', 
                             {3}
                        )",
                        dados.data.nome,
                        dados.data.cpf,
                        dados.data.idade,
                        dados.data.status_civil);
        }
        #endregion

        #region Executa queries
        public int Insert(DadosKafka dadosMensagem)
        {
            int linhasAfetadas = 0;

            using (SqlConnection connection = new SqlConnection(_acessoBancoDados.ObtemStringDeConexao()))
            {
                try
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                        _acessoBancoDados.AbreConexao(connection);

                    SqlCommand command = new SqlCommand(GeraComandoSqlInsert(dadosMensagem), connection);

                    linhasAfetadas = command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    _acessoBancoDados.FechaConexao(connection);
                }
            }

            return linhasAfetadas;
        }
        #endregion
    }
}
