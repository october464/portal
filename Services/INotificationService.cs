using Finportal.Models;
using System.Threading.Tasks;
namespace Finportal.Services
{
    public interface INotificationService
    {
        //subject: Your account has been overdrafted
        //Body:overdraft Wells Fargo Checking Account on the 9th Dec. by $40.50  as the result of recording a transaction 

        public Task NotifyOverdraftAsync(Transaction transaction, BankAccount account, decimal oldBalance );
    }
}
