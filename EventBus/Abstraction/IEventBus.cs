﻿using EventBus.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Abstraction
{
    public interface IEventBus
    {
        Task Publish(IntegrationEvent @event);

    }
}
