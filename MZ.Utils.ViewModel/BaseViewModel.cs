using MZ.Utils.ViewModel.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MZ.Utils.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public bool IsBusy { get => GetValue<bool>(); set => SetValue(value); }

        public BaseViewModel()
        {
            //Run the InitializeAsync task in a separate thread.
            if (this is IInitializeAsync init)
                Task.Run(async () => await init.InitializeAsync());
        }

        #region Property Utilities
        private IDictionary<string, object?> _values { get; } = new Dictionary<string, object?>();

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Get the backed value of the property.
        /// </summary>
        /// <typeparam name="T">Typo to cast stored data (from obj to T)</typeparam>
        /// <param name="propertyName">Usually null, but will get that key from the backed store.</param>
        /// <returns>Value if the value exists and is of type T, otherwise default of T</returns>
        public T? GetValue<T>([CallerMemberName] string? propertyName = null)
            => _values.ContainsKey(propertyName!) && _values[propertyName!] is T value ? value : default;

        /// <summary>
        /// SetValue stores the value given into the backing store.
        /// </summary>
        /// <typeparam name="T">Type of value (Detected by the type of value)</typeparam>
        /// <param name="value">The value to store</param>
        /// <param name="propertyName">The propertyname of the value to store</param>
        public void SetValue<T>(T? value, [CallerMemberName] string? propertyName = null)
        {
            //If doesn't have the value or if the value has actually changed, then trigger the property changed event.
            if (!_values.ContainsKey(propertyName!) ||
               (_values[propertyName!] is T data && !EqualityComparer<T>.Default.Equals(data, value)) ||
               (_values[propertyName!] == null && value != null))
            {
                _values[propertyName!] = value!;
                RaisePropertyChanged(propertyName!);
            }
        }

        /// <summary>
        /// Raises properties changed and related properties up to <see cref="BaseViewModelExtensions"/>.MaxRelationDepth relations deep.
        /// </summary>
        /// <param name="propertyName">Property Name to trigger the event</param>
        /// <param name="depth">Current depth of the Relationship</param>
        public void RaisePropertyChanged([CallerMemberName] string? propertyName = null, int depth = 0)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName!));

            if (depth < BaseViewModelExtensions.MaxRelationDepth)
            {
                this.RaiseRelated(depth + 1, propertyName!);
            }
        }

        #endregion
    }
}