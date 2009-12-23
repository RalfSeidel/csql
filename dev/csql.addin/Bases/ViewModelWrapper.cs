using System;
using System.Collections.Generic;
using System.Text;

namespace csql.addin.Bases
{
    public class ViewModelWrapper<T>  : ViewModel
    {
        public T Value 
        {
            get { return temporaryValue; }
            set 
            { 
                if (! this.temporaryValue.Equals(value)) 
                    OnHasChanged(); 
                this.temporaryValue = value; 
            }
        }

        public bool HasChanges { get { return (! temporaryValue.Equals(initialValue)); } }

        T temporaryValue;
        T initialValue;

        public ViewModelWrapper(T value)
        {
            this.temporaryValue = value;
            this.initialValue = value;
        }

        public void AcceptChanges()
        {
            this.initialValue = temporaryValue;
            ForceRaisPropertyChanged();
        }

        public void ResetChanges()
        {
            this.temporaryValue = initialValue;
            ForceRaisPropertyChanged();
        }

        public void ForceRaisPropertyChanged()
        {
            RaisePropertyChanged("Value");
        }

        public delegate void HasChangedEventHandler();
        public event HasChangedEventHandler HasChanged;

        public void OnHasChanged()
        {
            if (HasChanged != null) HasChanged();
        }
    }
}
