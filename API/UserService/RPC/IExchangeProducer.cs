using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.RPC
{
    public interface IExchangeProducer
    {
        public void SendProductMessage<T>(T message, string queueName);
    }
}
