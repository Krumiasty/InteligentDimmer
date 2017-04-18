using InteligentDimmer.Infrastructure;

namespace InteligentDimmer.ViewModel
{
    public class ViewModelLocator
    {
        public ConnectionViewModel ConnectionViewModel => GetViewModel<ConnectionViewModel>();
        public ControlViewModel ControlViewModel => GetViewModel<ControlViewModel>();

        public static T GetViewModel<T>()
        {
            return ObjectContainer.Instance.GetInstance<T>();
        }
    }
}
