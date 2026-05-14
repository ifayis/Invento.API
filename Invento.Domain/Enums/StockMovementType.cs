using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Domain.Enums
{
    public enum StockMovementType
    {
        Purchase = 1,
        Sale = 2,
        Return = 3,
        Damage = 4,
        Adjustment = 5,
        StockIn = 1,
        StockOut = 2
    }
}
