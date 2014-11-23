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
    public class ButtonX : Button
    {

        #region Fields
        
        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground",
            typeof(Brush),
            typeof(ButtonX),
            new PropertyMetadata(new SolidColorBrush(Colors.White), null));

        public static readonly DependencyProperty PressedForegroundProperty =
            DependencyProperty.Register("PressedForeground",
            typeof(Brush),
            typeof(ButtonX),
            new PropertyMetadata(new SolidColorBrush(Colors.Black), null));

        public static readonly DependencyProperty PressedBorderBrushProperty =
            DependencyProperty.Register("PressedBorderBrush",
            typeof(Brush),
            typeof(ButtonX),
            new PropertyMetadata(new SolidColorBrush(Colors.Black), null));

        public static readonly DependencyProperty InvisibleMarginProperty =
            DependencyProperty.Register("InvisibleMargin",
            typeof(Thickness),
            typeof(ButtonX),
            new PropertyMetadata(new Thickness(12), null));

        #endregion

        #region Properties

        public Brush PressedBackground
        {
            set { SetValue(PressedBackgroundProperty, value); }
            get { return (Brush)GetValue(PressedBackgroundProperty); }
        }

        public Brush PressedForeground
        {
            set { SetValue(PressedForegroundProperty, value); }
            get { return (Brush)GetValue(PressedForegroundProperty); }
        }

        public Brush PressedBorderBrush
        {
            set { SetValue(PressedBorderBrushProperty, value); }
            get { return (Brush)GetValue(PressedBorderBrushProperty); }
        }

        public Thickness InvisibleMargin
        {
            set { SetValue(InvisibleMarginProperty, value); }
            get { return (Thickness)GetValue(InvisibleMarginProperty); }
        }
        #endregion

        #region Construct
        
        public ButtonX()
            : base()
            {
                this.DefaultStyleKey = typeof(ButtonX);
            }

        #endregion

    }
}
