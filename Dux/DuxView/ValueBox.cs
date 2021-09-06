using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Limcap.Duxtools {

	public class ValueBox : StackPanel {

		public event Action<ValueBox> OnFocus;
		public event Action<ValueBox> OnValueChange;




		public ValueBox( char type, string value ) {
			base.Children.Add( CreateChildComponent( type, value ) );
		}




		public new Control Children => base.Children.Count > 0 ? (Control) base.Children[0] : null;




		private Control CreateChildComponent( char boxKind, string initialValue ) {
			// cria o componente
			Control comp;
			if (boxKind == '#') {
				comp = new CheckBox() { IsChecked = StringAsBool( initialValue ) };
			}
			else if (boxKind == '*') {
				comp = new PasswordBox() { Password = initialValue };
			}
			else {
				comp = new TextBox() { Text = initialValue };
				(comp as TextBox).VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
				(comp as TextBox).CaretIndex = int.MaxValue;
				if (initialValue.Contains( "\n" )) (comp as TextBox).AcceptsReturn = true;
			}

			// define as ações de alteração
			void onChangeEvent( object o, RoutedEventArgs a ) => OnValueChange?.Invoke( this );
			if (comp is CheckBox c) c.Click += onChangeEvent;
			else if (comp is TextBox t) t.TextChanged += onChangeEvent;
			else if (comp is PasswordBox p) p.PasswordChanged += onChangeEvent;


			// define ação de obter foco.
			void onGotFocus( object o, RoutedEventArgs a ) => OnFocus?.Invoke( this );
			if (comp is CheckBox c2) c2.Click += onGotFocus;
			else comp.GotFocus += onGotFocus;

			return comp;
		}




		public string Key { get; set; }

		public string Value {
			get {
				return
				Children is TextBox e ? e.Text :
				Children is CheckBox c ? c.IsChecked.ToString() :
				Children is PasswordBox p ? p.Password :
				Children.ToString();
			}
			set {
				if (Children is TextBox t) t.Text = value;
				else if (Children is CheckBox c) c.IsChecked = StringAsBool( value );//value.Trim() != "0" && value.Trim().ToLower() != "false";
				else if (Children is PasswordBox p) p.Password = value;
			}
		}

		public new Brush Background {
			get => Children.Background;
			set => Children.Background = value;
		}




		public new void Focus() => base.Children[0].Focus();
		public TextBox AsTextBox => Children is TextBox e ? e : null;
		public CheckBox AsCheckBox => Children is CheckBox e ? e : null;
		public PasswordBox AsPasswordBox => Children is PasswordBox e ? e : null;

		private bool StringAsBool( string value ) => !(value is null || value.Trim( ' ', '\t', '\n', '\r', '0' ) == "" || value.Trim().ToLower() == "false");
	}
}
