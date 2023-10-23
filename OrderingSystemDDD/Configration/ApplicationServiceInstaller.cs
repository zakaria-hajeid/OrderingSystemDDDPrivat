using Ordering.Application.Behaviors;
using Ordering.Application;
using MediatR.NotificationPublishers;
using FluentValidation;
using Service.Common.Extinsions;

namespace OrderingSystemDDD.Configration
{
    public class ApplicationServiceInstalle : IServiceInstaller
    {
        public void Instal(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(AssemblyReference.assembly.GetType());// regesterfrom application Dll not from program 
                //The area the use the request and request handler
                // cfg.NotificationPublisher = new ForeachAwaitPublisher(); each notification handler execute one by one
                //cfg.NotificationPublisher = new TaskWhenAllPublisher(); each notification handler execute in parallel way
                //cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
               // cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });

            services.AddValidatorsFromAssembly(AssemblyReference.assembly);

            services.AddSharedServices(configuration);
        }
    }
}
