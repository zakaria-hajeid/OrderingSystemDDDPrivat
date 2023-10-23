using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Dtos.CreateOrderDtos;

public record CreateOrderDto(string UserId, string UserName);    /*{
    
   /* public string UserId { get; init ; }


    public string UserName { get; init ; }


    public string City { get; init ; }


    public string Street { get; init ; }
    public  string State { get; init ; }




    public string Country { get; init ; }


    public string ZipCode { get; init ; }


    public string CardNumber { get; init ; }


    public string CardHolderName { get; init ; }


    public DateTime CardExpiration { get; init ; }


    public string CardSecurityNumber { get; init ; }


    public int CardTypeId
    {
        get; init ;
    }
    public IEnumerable<OrderItemDto> orderItemDtos { get; init; }
   
}
}*/
