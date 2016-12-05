using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace jTech.Wpf.Mvvm
{
    public abstract class NotifyPropertyChangedViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool OnChange<T>(ref T field, T value, Expression<Func<T>> selectorExpression)
        {
            if (AreEqual(field, value)) return false;

            field = value;
            NotifyPropertyChanged(selectorExpression);
            return true;
        }

        public bool OnChange<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (AreEqual(field, value)) return false;

            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        public void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unary = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unary.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            NotifyPropertyChanged(memberExpression.Member.Name);
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool AreEqual<T>(T first, T second)
        {
            return EqualityComparer<T>.Default.Equals(first, second);
        }

    }
}
