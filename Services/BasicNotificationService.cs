using Finportal.Data;
using Finportal.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Threading.Tasks;

namespace Finportal.Services
{
    public class BasicNotificationService : INotificationService
    {
        public ApplicationsDbContext _dbContext { get; }
        public IEmailSender _emailSender { get; }

        public BasicNotificationService(ApplicationsDbContext context, IEmailSender emailSender)
        {
            _dbContext = context;
            _emailSender = emailSender;
        }
        public async Task NotifyOverdraftAsync(Transaction transaction, BankAccount account, decimal oldBalance)
        {
            if (account.CurrentBalance < 0 && oldBalance > 0)
            {
                var user = (await _dbContext.Users.FindAsync(transaction.FPUserId));
                //Step 1: Create a new Notificatoin record
                var notification = new Notification
                {
                    Created = DateTime.Now,
                    HouseholdId = account.HouseholdId,
                    IsRead = false,
                    Subject = $"Your {account.Name} account has been overdrafted.",
                    Body = $"{user.FullName} overdrafted the {account.Name} {account.Type} Account on {transaction.Created:MMM dd, yyyy} by {Math.Abs(account.CurrentBalance):#.00} as the result of recording a transaction in the amonut of {transaction.Amount:#.00}"
                };
                
                _dbContext.Add(notification);
                await _dbContext.SaveChangesAsync();
                await _emailSender.SendEmailAsync(user.Email, notification.Subject, notification.Body);
            }
        }
    }
}
