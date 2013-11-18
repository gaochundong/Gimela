using System.Globalization;

namespace Gimela.Rukbat.MPS.BusinessEntities
{
  public class PublishedDestination
  {
    public PublishedDestination(int port)
    {
      Port = port;
    }

    public string Id { get { return string.Format(CultureInfo.InvariantCulture, "{0}", Port); } }

    public int Port { get; private set; }

    public override bool Equals(object obj)
    {
      bool result = false;

      if (obj != null)
      {
        PublishedDestination other = obj as PublishedDestination;
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
