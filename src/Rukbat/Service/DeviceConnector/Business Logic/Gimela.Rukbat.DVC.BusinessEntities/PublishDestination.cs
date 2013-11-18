using System.Globalization;

namespace Gimela.Rukbat.DVC.BusinessEntities
{
  public class PublishDestination
  {
    public PublishDestination(string address, int port)
    {
      Address = address;
      Port = port;
    }

    public string Id { get { return string.Format(CultureInfo.InvariantCulture, "dest://{0}:{1}", Address, Port); } }

    public string Address { get; private set; }

    public int Port { get; private set; }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null)
      {
        PublishDestination other = obj as PublishDestination;
        if (other != null && this.Id == other.Id)
        {
          result = true;
        }
      }

      return result;
    }

    public override int GetHashCode()
    {
      return this.Id.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "{0}", Id);
    }
  }
}
