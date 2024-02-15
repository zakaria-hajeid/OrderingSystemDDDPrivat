namespace IntegrationEventLogEF.Repository
{
    public sealed class IIntegrationOutboxRepository
    {
        private readonly DbConnection _dbConnection;

        private readonly IntegrationEventLogContext _integrationEventLogContext;
        public IIntegrationOutboxRepository(DbConnection dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            /// <summary>
            ///بتقدر تعملو انجيكت عادي فقط بدون كل هاذ الفاكتوري لانو البروجيت الرئيسي عاملو انجيكت ومعطي الكونيكشن سترنج عادي مشان يضيف التيبل عندو بالداتا بيس نفسها واساسا هاي كلاس لايبراري الها ريفارنس من البروجيكت الاساسي
            /// </summary>
            _integrationEventLogContext = new IntegrationEventLogContext(
         new DbContextOptionsBuilder<IntegrationEventLogContext>()
             .UseSqlServer(_dbConnection)
             .Options);
        }

        public async Task<IEnumerable<IntegrationEventOutbox>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            var tid = transactionId.ToString();
            var result = await _integrationEventLogContext.IntegrationEventLogs
                               .Where(e => e.TransactionId == tid && e.State == EventStateEnum.NotPublished)
                               .ToListAsync();

          
            List<Type> _eventTypes = Assembly.Load(result.Select(X=>X.eventAssymblyName).FirstOrDefault()!) //call the application service 
                                             .GetTypes()
                                             .Where(t => typeof(IntegrationEvent).IsAssignableFrom(t)
                                                         && !t.IsInterface && !t.IsAbstract)
                                             .ToList();

            if (result.Any())
            {
                //DESERILIZE THE CONTENT STRIN IN SPEACIFIC
                //IntegrationEvent CLASS AND ASSIGN IN IntegrationEvent PROPERTY
                return result.OrderBy(o => o.CreationTime)
                             .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
            }

            return new List<IntegrationEventOutbox>();
        }
        public async Task<IntegrationEventOutbox> RetrieveEventById(Guid eventId)
        {
            IntegrationEventOutbox eventLogEntry = _integrationEventLogContext.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
            return eventLogEntry;
        }
        public async Task SaveChangesAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            _integrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            await _integrationEventLogContext.SaveChangesAsync();
        }
        public async Task ADDIntegrationEvent(IntegrationEventOutbox integrationEventOutbox)
        {
            await _integrationEventLogContext.IntegrationEventLogs.AddAsync(integrationEventOutbox);
        }

        public async Task UpdateIntegrationEvent(IntegrationEventOutbox integrationEventOutbox)
        {
            _integrationEventLogContext.IntegrationEventLogs.Update(integrationEventOutbox);
            await _integrationEventLogContext.SaveChangesAsync();

        }
        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _integrationEventLogContext.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

            return _integrationEventLogContext.SaveChangesAsync();
        }
        public void DisposeContext()
        {
            _integrationEventLogContext?.Dispose();
        }
        public async Task<IEnumerable<IntegrationEventOutbox>> RetrieveFailedPublishEvent()
        {
            var result = await _integrationEventLogContext.IntegrationEventLogs
                               .Where(e => e.State == EventStateEnum.PublishedFailed)
                               .ToListAsync();
            if (!result.Any())
            {
                return Enumerable.Empty<IntegrationEventOutbox>();
            }

            List<Type> _eventTypes = Assembly.Load(result.Select(X => X.eventAssymblyName).FirstOrDefault()!) //call the application service 
                                             .GetTypes()
                                             .Where(t => typeof(IntegrationEvent).IsAssignableFrom(t)
                                                         && !t.IsInterface && !t.IsAbstract)
                                             .ToList();

            if (result.Any())
            {
                //DESERILIZE THE CONTENT STRIN IN SPEACIFIC
                //IntegrationEvent CLASS AND ASSIGN IN IntegrationEvent PROPERTY
                return result.OrderBy(o => o.CreationTime)
                             .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
            }

            return new List<IntegrationEventOutbox>();
        }


    }
}
