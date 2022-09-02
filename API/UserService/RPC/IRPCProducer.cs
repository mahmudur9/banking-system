using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountService.RPC
{
    public interface IRPCProducer
    {
        public void SendProductMessage<T>(T message, string queueName);
    }
}
