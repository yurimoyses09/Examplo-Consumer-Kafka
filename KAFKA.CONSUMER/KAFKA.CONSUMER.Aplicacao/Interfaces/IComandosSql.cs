using KAFKA.CONSUMER.Dominios;

namespace KAFKA.CONSUMER.Aplicacao.Interfaces
{
    public interface IComandosSql
    {
        public int Insert(DadosKafka dadosMensagem);
        public string GeraComandoSqlInsert(DadosKafka dados);

    }
}
