using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Domain.Entities
{
    public class Product : Base
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int quantity { get; set; }
    }
}
