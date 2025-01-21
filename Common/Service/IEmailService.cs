using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public interface IEmailService
    {
        Task<bool> SendEmail(Product product);
    }
}
