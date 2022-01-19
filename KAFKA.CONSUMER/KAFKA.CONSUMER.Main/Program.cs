using Confluent.Kafka;
using KAFKA.CONSUMER.Ioc;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace KAFKA.CONSUMER.Main
{
    class Program
    {
        #region Dados do Consumer
        private readonly static string _NomeTopico = "testes_topico";
        private readonly static string _GroupId = "kafka-dotnet-getting-started";
        private readonly static string _BootstrapServer = "pkc-ymrq7.us-east-2.aws.confluent.cloud:9092";
        private readonly static string _Usuario = "2JTKE5CLJPKY2RMP";
        private readonly static string _Senha = "UyQPz/nnAIURcmgw+smAanlcC6EobOSpOUmyLZ7cXZ/vgsIRf8I/8UuvBhlMcqXJ";
        #endregion

        static void Main(string[] args)
        {
            var controleDependencia = ControleInjecaoDependencia.MyServiceProvider();

            #region Configuracao do Consumer
            var ConsumerConfiguration = new ConsumerConfig
            {
                GroupId = _GroupId,
                BootstrapServers = _BootstrapServer,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                SslCertificateLocation = "C:\\Users\\ADM\\Desktop\\DEV\\Kafka\\CONSUMER\\KAFKA.CONSUMER\\cacert.pem",
                SaslUsername = _Usuario,
                SaslPassword = _Senha,
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl
            };
            #endregion

            #region Evita que o processo pare
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // Impede que o processo termine
                cts.Cancel();
            };
            #endregion

            using (var consumer = new ConsumerBuilder<string, string>(ConsumerConfiguration).Build())
            {
                consumer.Subscribe(_NomeTopico);

                try
                {
                    while (true)
                    {
                        Console.WriteLine("Lendo Fila...");

                        var mensagem = consumer.Consume(cts.Token);

                        // Caso tenha mensagem
                        if (!mensagem.Message.Equals(null))
                        {
                            Console.WriteLine($"Mensagem encontrada {mensagem.Message.Value}");
                            try
                            {
                                var JsonMensagem = JsonConvert.DeserializeObject<Dominios.DadosKafka>(mensagem.Message.Value);

                                var RealizaQueryInsert = controleDependencia.Insert(JsonMensagem);

                                if (RealizaQueryInsert > 0)
                                    consumer.Commit();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                continue;
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl + c foi precionado
                    return;
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}
