using Braintree;
using Microsoft.Extensions.Options;
using Rocky_Utility.Configurations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocky_Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        public BrainTreeSettings _brainTreeSettings { get; set; }

        private IBraintreeGateway brainTreeGateWay { get; set; }
        public BrainTreeGate(IOptions<BrainTreeSettings> brainTreeSettings)
        {
            _brainTreeSettings = brainTreeSettings.Value;
        }


        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(
                _brainTreeSettings.Environment,
                _brainTreeSettings.MerchantId,
                _brainTreeSettings.PublicKey,
                _brainTreeSettings.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (brainTreeGateWay == null)
            {
                brainTreeGateWay = CreateGateway();
            }
            return brainTreeGateWay;
        }
    }
}
