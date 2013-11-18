using System.ComponentModel;

namespace Gimela.Data.Mapping.Internal {
    public abstract class ProxyBase {
		protected void NotifyPropertyChanged(PropertyChangedEventHandler handler, string method) {
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(method));
			}
		}
	}
}