using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.Repository;
using Gimela.Rukbat.DVC.DataAccess.Models;
using Gimela.Infrastructure.ResourceLocation;

namespace Gimela.Rukbat.DVC.DataAccess
{
  public class PublishedCameraRepository : RepositoryBase<PublishedCamera>, IPublishedCameraRepository
  {
    public override int Count()
    {
      return Locator.Get<IDatabase>().Instance.GetCollection<PublishedCamera>(Name).Count();
    }

    public override PublishedCamera Find(string id)
    {
      return Locator.Get<IDatabase>().Instance.GetCollection<PublishedCamera>(Name).FindOneById(id);
    }

    public override IEnumerable<PublishedCamera> FindAll()
    {
      return Locator.Get<IDatabase>().Instance.GetCollection<PublishedCamera>(Name).FindAll();
    }

    public override void Save(PublishedCamera entity)
    {
      Locator.Get<IDatabase>().Instance.GetCollection<PublishedCamera>(Name).Save(entity);
    }

    public override void Remove(string id)
    {
      Locator.Get<IDatabase>().Instance.GetCollection<PublishedCamera>(Name).Remove(id);
    }

    public override void RemoveAll()
    {
      Locator.Get<IDatabase>().Instance.GetCollection<PublishedCamera>(Name).RemoveAll();
    }
  }
}
