using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace jTech.Wpf.AttachedBehaviours
{
    public class KeyBindingsAttachedBehaviours : DependencyObject
    {
        public static readonly DependencyProperty OwnerWindowKeyBindingsProperty = DependencyProperty.RegisterAttached(
            "OwnerWindowKeyBindings",
            typeof(InputBindingCollection),
            typeof(KeyBindingsAttachedBehaviours),
            new UIPropertyMetadata(new PropertyChangedCallback(OnOwnerWindowKeyBindingsChanged)));

        private static void OnOwnerWindowKeyBindingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var subject = sender as FrameworkElement;
                if (subject == null) return;

                var ownerWindow = Window.GetWindow(subject);

                if (ownerWindow == null)
                {
                    subject.Loaded += LateRegister;
                }
                else
                {
                    var oldBindings = e.OldValue as InputBindingCollection;
                    if (oldBindings != null)
                    {
                        UnRegisterBindings(ownerWindow, oldBindings);
                    }

                    var newBindings = e.NewValue as InputBindingCollection;
                    if (newBindings != null)
                    {
                        RegisterBindings(ownerWindow, newBindings);
                    }
                }
            }
            catch (Exception)
            {
                // TODO
            }
        }

        private static void LateRegister(object sender, RoutedEventArgs args)
        {
            try
            {
                var subject = sender as FrameworkElement;
                if (subject == null) return;

                subject.Loaded -= LateRegister;

                var ownerWindow = Window.GetWindow(subject);

                var inputBindingCollection = subject.GetValue(OwnerWindowKeyBindingsProperty) as InputBindingCollection;
                if (inputBindingCollection == null) return;

                RegisterBindings(ownerWindow, inputBindingCollection);
            }
            catch (Exception)
            {
                // TODO:
            }
        }

        private static void RegisterBindings(Window window, InputBindingCollection inputBindingCollection)
        {
            foreach (var keyBinding in inputBindingCollection.OfType<KeyBinding>())
            {
                window.InputBindings.Add(keyBinding);
            }
        }

        private static void UnRegisterBindings(Window window, InputBindingCollection inputBindingCollection)
        {
            foreach (var keyBinding in inputBindingCollection.OfType<KeyBinding>())
            {
                window.InputBindings.Remove(keyBinding);
            }
        }

        public static InputBindingCollection GetOwnerWindowKeyBindings(DependencyObject obj)
        {
            return (InputBindingCollection) obj.GetValue(OwnerWindowKeyBindingsProperty);
        }

        public static void SetOwnerWindowKeyBindings(DependencyObject obj, InputBindingCollection value)
        {
            obj.SetValue(OwnerWindowKeyBindingsProperty, value);
        }

        public static readonly DependencyProperty SuppressKeyBindingsProperty = DependencyProperty.RegisterAttached(
            "SuppressKeyBindings",
            typeof(InputBindingCollection),
            typeof(KeyBindingsAttachedBehaviours),
            new UIPropertyMetadata(new PropertyChangedCallback(OnSuppressKeyBindingsChanged)));

        private static Dictionary<FrameworkElement, KeyEventHandler>
            SuppressKeyBindingsChangedSubscriptions = new Dictionary<FrameworkElement, KeyEventHandler>();

        private static void OnSuppressKeyBindingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var subject = sender as FrameworkElement;
                if (subject == null) return;

                KeyEventHandler subscription;
                if (SuppressKeyBindingsChangedSubscriptions.TryGetValue(subject, out subscription))
                {
                    // If we already have this element registered, have the key bindings been cleared?
                    if (e.NewValue == null)
                    {
                        // Yes? Unsubscribe and remove item from our collection.
                        subject.KeyUp -= subscription;
                        SuppressKeyBindingsChangedSubscriptions.Remove(subject);
                    }

                    // Either way, nothing more to do here.
                    return;
                }

                // Subscribe to key up events.
                subscription = new KeyEventHandler(OnKeyUp);
                subject.KeyUp += subscription;
            }
            catch (Exception)
            {
                // TODO
            }
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {

        }

        public static InputBindingCollection GetSuppressKeyBindings(DependencyObject obj)
        {
            return (InputBindingCollection)obj.GetValue(SuppressKeyBindingsProperty);
        }

        public static void SetSuppressKeyBindings(DependencyObject obj, InputBindingCollection value)
        {
            obj.SetValue(SuppressKeyBindingsProperty, value);
        }


    }
}
