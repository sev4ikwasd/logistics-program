using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Prism.Mvvm;

namespace LogisticsProgram
{
    public abstract class BaseViewModel : BindableBase, INotifyDataErrorInfo
    {
        readonly Dictionary<string, PropertyWithErrorsList> propErrors = new Dictionary<string, PropertyWithErrorsList>();
        
        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
            if (args.PropertyName != "HasErrors")
            {
                Validate();
            }
        }

        protected abstract void Validate();
        
        protected void ValidateProperty(string propertyName, object property, Func<PropertyWithErrorsList, PropertyWithErrorsList> check)
        {
            if (propErrors.TryGetValue(propertyName, out var propertyWithErrorsList) == false)
                propertyWithErrorsList = new PropertyWithErrorsList(property, new List<string>());
            else
            {
                propertyWithErrorsList.ListErrors.Clear();
                propertyWithErrorsList.Property = property;
            }

            propertyWithErrorsList = check(propertyWithErrorsList);
            propErrors[propertyName] = propertyWithErrorsList;
            
            var propErrorsCount = propErrors.Values.FirstOrDefault(r =>r.ListErrors.Count > 0);
            HasErrors = propErrorsCount != null;

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            
        }

        public IEnumerable GetErrors(string propertyName)
        {
            PropertyWithErrorsList propertyWithErrorsList;
            if (propertyName == null) return null;
            propErrors.TryGetValue (propertyName, out propertyWithErrorsList);
            return propertyWithErrorsList?.ListErrors;

        }

        private bool hasErrors = false;
        public bool HasErrors
        {
            get => hasErrors;
            set => SetProperty(ref hasErrors, value);
        }
        
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public class PropertyWithErrorsList
        {
            public object Property { get; set; }
            public List<string> ListErrors { get; set; }

            public PropertyWithErrorsList(object property, List<string> listErrors)
            {
                Property = property;
                ListErrors = listErrors;
            }

            public PropertyWithErrorsList()
            {
            }
        }
    }
}