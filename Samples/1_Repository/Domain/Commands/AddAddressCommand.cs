using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Commands
{
    public sealed class AddAddressCommand
    {
        public string City { get; set; }

        public string Street { get; set; }
    }
}
