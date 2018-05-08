using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Librarian_UI.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Notifies the UI about changes in the model
        /// </summary>
        /// <param name="propertyname">Changed property name</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
