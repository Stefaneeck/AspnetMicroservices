using Ordering.Application.Models;
using System.Threading.Tasks;

namespace Ordering.Application.Contracts.Infrastructure
{
    public interface IEmailService
    {
        //Any email service can be implemented in the infrastructure layer without affecting the application layer
        Task<bool> SendMail(Email email);
        Task SendEmail(Email email);
    }
}
