using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PasswordKeeper.Apps.Wpf.Features
{
    public class ValidatableViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> errors;
        private readonly Dictionary<string, List<(Func<bool> predicate, string message, bool stopOnFail)>> rules;

        public ValidatableViewModel()
        {
            errors = new Dictionary<string, List<string>>();
            rules = new Dictionary<string, List<(Func<bool> predicate, string message, bool stopOnFail)>>();
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => errors.SelectMany(x => x.Value).Any();

        public IEnumerable GetErrors(string propertyName)
        {
            errors.TryGetValue(propertyName, out List<string> propertyErrors);
            return propertyErrors;
        }

        protected void AddError(string propertyName, string message)
        {
            errors[propertyName] = new List<string> { message };
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void AddRule(string propertyName, Func<bool> predicate, string message, bool stopOnFail = false)
        {
            var rule = (predicate, message, stopOnFail);
            if (!rules.TryGetValue(propertyName, out List<(Func<bool> predicate, string message, bool stopOnFail)> propertyRules))
            {
                rules.Add(propertyName, new List<(Func<bool>, string, bool)> { rule });
            }
            else
            {
                propertyRules.Add(rule);
            }
        }

        protected void Validate(string propertyName)
        {
            if (!rules.TryGetValue(propertyName, out List<(Func<bool> predicate, string message, bool stopOnFail)> propertyRules))
            {
                return;
            }

            Validate(propertyName, propertyRules);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void ValidateAll()
        {
            errors.Clear();
            foreach (var propertyRules in rules)
            {
                Validate(propertyRules.Key, propertyRules.Value);
            }

            foreach (var propertyErrors in errors)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyErrors.Key));
            }
        }

        private void Validate(string propertyName, List<(Func<bool> predicate, string message, bool stopOnFail)> propertyRules)
        {
            errors[propertyName] = new List<string>();

            foreach (var (predicate, message, stopOnError) in propertyRules)
            {
                if (!predicate())
                {
                    errors[propertyName].Add(message);

                    if (stopOnError)
                    {
                        break;
                    }
                }
            }
        }
    }
}