﻿using Ordering.Application.Behaviors;
using Ordering.Application;
using MediatR.NotificationPublishers;
using FluentValidation;
using Service.Common.Extinsions;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;

namespace OrderingSystemDDD.Configration
{
    public class ApplicationServiceInstalle : IServiceInstaller
    {
        public void Instal(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
            cfg.RegisterServicesFromAssemblyContaining(typeof(AssemblyReference));
             //The area the use the request and request handler
              //cfg.NotificationPublisher = new ForeachAwaitPublisher();each notification handler execute one by one
             cfg.NotificationPublisher = new TaskWhenAllPublisher(); //each notification handler execute in parallel way
                                                                     //Note :The fist pipline add is first invoke sequentially 
                cfg.AddOpenBehavior(typeof(IdempotencyCommandBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });

            services.AddValidatorsFromAssembly(AssemblyReference.assembly);

            services.AddEventBusSharedServices(configuration);
        }
    }
}
