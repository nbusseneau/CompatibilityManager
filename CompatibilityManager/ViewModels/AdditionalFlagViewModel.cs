using Prism.Events;
using Prism.Mvvm;

namespace CompatibilityManager.ViewModels
{
    public class FlagChanged : PubSubEvent { }

    public class AdditionalFlagViewModel : BindableBase
    {
        private EventAggregator eventAggregator;

        private string flag; 
        public string Flag
        {
            get => this.flag;
            set => SetProperty(ref this.flag, value, this.eventAggregator.GetEvent<FlagChanged>().Publish);
        }

        public AdditionalFlagViewModel(EventAggregator eventAggregator, string flag)
        {
            this.eventAggregator = eventAggregator;
            this.flag = flag;
        }
    }
}
