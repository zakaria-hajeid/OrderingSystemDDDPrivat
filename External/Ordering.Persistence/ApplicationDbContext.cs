using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Prematives;
using Ordering.Persistence.EntityConfigratios;
using Ordering.Persistence.Extinsions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Persistence
{
    public sealed class ApplicationDbContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "ordering";

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;

        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentMethod> Payments { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }

        private IDbContextTransaction _currentTransaction;
        private readonly IMediator _mediator;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;


        bool IUnitOfWork.HasActiveTransaction { get => _currentTransaction != null; }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return _currentTransaction;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {

                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch 
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CardTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BuyerEntityTypeConfiguration());

        }
        public async Task PublishEventAsyncAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this);
            /*int c = await base.SaveChangesAsync(cancellationToken);
            return c;*/
        }

        public IExecutionStrategy CreateExecutionStrategy()
        {
            return Database.CreateExecutionStrategy();
        }

        public DbConnection DbConnection()
        {
            return Database.GetDbConnection();

        }
    }
}

