namespace KAFKA.CONSUMER.Dominios
{
    public class DadosKafka
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string nome { get; set; }
        public string cpf { get; set; }
        public string idade { get; set; }
        public int status_civil { get; set; }
    }
}
