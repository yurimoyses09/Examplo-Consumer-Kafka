using KAFKA.CONSUMER.Aplicacao.ComandosSql;
using KAFKA.CONSUMER.Aplicacao.Interfaces;
using KAFKA.CONSUMER.Data.Data;
using KAFKA.CONSUMER.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KAFKA.CONSUMER.Ioc
{
    public class ControleInjecaoDependencia
    {
        public static ComandosSql MyServiceProvider() 
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IComandosSql, ComandosSql>();
            services.AddTransient<IAcessoBancoDados, AcessoBancoDados>();
            services.AddSingleton<ComandosSql>();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            ComandosSql comandosSql = serviceProvider.GetService<ComandosSql>();

            return comandosSql;
        }
    }
}
