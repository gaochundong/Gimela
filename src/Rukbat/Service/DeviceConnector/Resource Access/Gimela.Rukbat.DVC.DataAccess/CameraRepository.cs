using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gimela.Data.Repository;
using Gimela.Infrastructure.ResourceLocation;
using Gimela.Rukbat.DVC.DataAccess.Models;

namespace Gimela.Rukbat.DVC.DataAccess
{
  public class CameraRepository : RepositoryBase<Camera>, ICameraRepository
  {
    public override int Count()
    {
      return Locator.Get<IDatabase>().Instance.GetCollection<Camera>(Name).Count();
    }

    public override Camera Find(string id)
    {
      return Locator.Get<IDatabase>().Instance.GetCollection<Camera>(Name).FindOneById(id);
    }

    public override IEnumerable<Camera> FindAll()
    {
      return Locator.Get<IDatabase>().Instance.GetCollection<Camera>(Name).FindAll();
    }

    public override void Save(Camera entity)
    {
      Locator.Get<IDatabase>().Instance.GetCollection<Camera>(Name).Save(entity);
    }

    public override void Remove(string id)
    {
      Locator.Get<IDatabase>().Instance.GetCollection<Camera>(Name).Remove(id);
    }

    public override void RemoveAll()
    {
      Locator.Get<IDatabase>().Instance.GetCollection<Camera>(Name).RemoveAll();
    }
  }
}
