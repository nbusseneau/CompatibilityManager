using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace CompatibilityManager.ViewModels
{
    public class FlagChanged : PubSubEvent { }
    public class RemoveCommandIssued : PubSubEvent<AdditionalFlagViewModel> { }

    public class AdditionalFlagViewModel : BindableBase
    {
        private EventAggregator eventAggregator;

        private string flag = string.Empty;
        public string Flag
        {
            get => this.flag;
            set => SetProperty(ref this.flag, value, this.eventAggregator.GetEvent<FlagChanged>().Publish);
        }

        public DelegateCommand RemoveCommand { get; protected set; }

        public AdditionalFlagViewModel(EventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.RemoveCommand = new DelegateCommand(this.Remove);
        }

        public AdditionalFlagViewModel(EventAggregator eventAggregator, string flag) : this(eventAggregator)
        {
            this.flag = flag;
        }

        private void Remove()
        {
            this.eventAggregator.GetEvent<RemoveCommandIssued>().Publish(this);
        }
    }
}
