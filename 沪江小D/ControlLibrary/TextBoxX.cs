using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace System.Windows.Controls
{

    [StyleTypedProperty(Property = "HintTextStyle", StyleTargetType = typeof(TextBlock)),
    TemplatePart(Name = "HintText", Type = typeof(TextBlock)),
    TemplateVisualState(Name = "HintTextVisible", GroupName = "HintTextStates"),
    TemplateVisualState(Name = "HintTextHidden", GroupName = "HintTextStates")]
    public class TextBoxX : TextBox
    {
        #region Fields

        private bool itsIsFocused = false;

        #endregion

        #region Properties and DependencyProperties
        
        public static readonly DependencyProperty HintTextProperty = DependencyProperty.Register(
            "HintText",
            typeof(string),
            typeof(TextBoxX),
            new PropertyMetadata(""));

        public static readonly DependencyProperty HintTextForegroundProperty = DependencyProperty.Register(
            "HintTextForeground",
            typeof(Brush),
            typeof(TextBoxX),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public static readonly DependencyProperty HintTextStyleProperty = DependencyProperty.Register(
            "HintTextStyle",
            typeof(Style),
            typeof(TextBoxX),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ActionIconProperty =
            DependencyProperty.Register("ActionIcon", 
            typeof(ImageSource), 
            typeof(TextBoxX), 
            new PropertyMetadata(null));


        public string HintText {
            get { return (string)this.GetValue(HintTextProperty); }
            set { this.SetValue(HintTextProperty, value); }
        }

        public Brush HintTextForeground {
            get { return (Brush)this.GetValue(HintTextForegroundProperty); }
            set { this.SetValue(HintTextForegroundProperty, value); }
        }

        public Style HintTextStyle {
            get { return (Style)this.GetValue(HintTextStyleProperty); }
            set { this.SetValue(HintTextStyleProperty, value); }
        }

        public ImageSource ActionIcon
        {
            get { return base.GetValue(ActionIconProperty) as ImageSource; }
            set { base.SetValue(ActionIconProperty, value); }
        }

        #endregion

        public TextBoxX()
            : base() {
            this.DefaultStyleKey = typeof(TextBoxX);

            this.GotFocus += new RoutedEventHandler(HintTextBox_GotFocus);
            this.LostFocus += new RoutedEventHandler(HintTextBox_LostFocus);
            this.Loaded += new RoutedEventHandler(HintTextBox_Loaded);
            this.TextChanged += new TextChangedEventHandler(HintTextBox_TextChanged);
        }

        #region Events
        
        private void HintTextBox_Loaded(object sender, RoutedEventArgs e) {
            this.GoToVisualState(true);
        }

        private void HintTextBox_GotFocus(object sender, RoutedEventArgs e) {
            this.itsIsFocused = true;
            this.GoToVisualState(false);
        }

        private void HintTextBox_LostFocus(object sender, RoutedEventArgs e) {
            this.itsIsFocused = false;
            this.GoToVisualState(true);
        }

        private void HintTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (!this.itsIsFocused) {
                this.GoToVisualState(false);
            }
        }

        #endregion

        #region Method
        
        private void GoToVisualState(bool theIsHintDisplayed) {
            if (theIsHintDisplayed && (this.Text == null || (this.Text != null && this.Text.Length == 0))) 
            {
                VisualStateManager.GoToState(this, "HintTextVisible", true);
            } else 
            {
                VisualStateManager.GoToState(this, "HintTextHidden", true);
            }
        }

        #endregion
    }
}
