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
        private readonly static string _NomeTopico = "SEU TOPICO";
        private readonly static string _GroupId = "SEU GROUPID";
        private readonly static string _BootstrapServer = "SEU SERVIDOR";
        private readonly static string _Usuario = "SEU USUARIO";
        private readonly static string _Senha = "SUA SENHA";
        private readonly static string _DiretorioCertificado = "DIRETORIO DO CERTIFICADO";
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
                SslCertificateLocation = _DiretorioCertificado,
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

            try
            {
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
                                    Console.WriteLine("Deserealizando mensagem");
                                    var JsonMensagem = JsonConvert.DeserializeObject<Dominios.DadosKafka>(mensagem.Message.Value);

                                    Console.WriteLine("Realizando insert dos dados no sql");
                                    var RealizaQueryInsert = controleDependencia.Insert(JsonMensagem);

                                    if (RealizaQueryInsert > 0)
                                    {
                                        Console.WriteLine("Mensagem gravada no sql com sucesso. Linhas afetadas {0}", RealizaQueryInsert);
                                        Console.WriteLine("========================================================");
                                        consumer.Commit();
                                    }
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
                        Console.WriteLine("Operação foi cancelada");
                        return;
                    }
                    finally
                    {
                        consumer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
            }

        }
    }
}
