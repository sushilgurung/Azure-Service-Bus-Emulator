using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public class EmailService : IEmailService
    {

        public async Task<bool> SendEmail(Product product)
        {
            return await Task.FromResult(true);
        }
    }
}
