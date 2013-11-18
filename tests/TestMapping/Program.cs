using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.Mapping;

namespace TestMapping
{
  class Program
  {
    static void Main(string[] args)
    {
      Mapper.Reset();
      Mapper.Initialize(cfg =>
      {
        cfg.CreateMap<OrderDto, Order>()
          .ForMember(dest => dest.Id, opt => opt.Ignore());
      });

      Mapper.AssertConfigurationIsValid();

      var orderDto = new OrderDto { Amount = 50m };
      var order = new Order { Id = 4 };

      Mapper.Map(orderDto, order);
      Console.WriteLine(order.Amount);

      Console.ReadKey();
    }
  }

  public class OrderDto
  {
    public decimal Amount { get; set; }
  }

  public class Order
  {
    public int Id { get; set; }
    public decimal Amount { get; set; }
  }
}
