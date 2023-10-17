using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SOCE.Library.UI
{
    public class MyCustomComboBox : ComboBox
    {
        private int caretPosition;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.DropDownOpened += MyCustomComboBox_DropDownOpened;
            //var element = GetTemplateChild("PART_EditableTextBox");
            //if (element != null)
            //{
            //    var textBox = (TextBox)element;
            //    textBox.Drop += OnDropSelectionChanged;
            //}
        }

        private void MyCustomComboBox_DropDownOpened(object sender, EventArgs e)
        {
            //ComboBox combo = (ComboBox)object;
            var element = GetTemplateChild("PART_EditableTextBox");
            if (element != null)
            {
                TextBox txt = (TextBox)element;
                //txt.SelectionLength = 0;
                if (base.IsDropDownOpen && txt.SelectionLength > 0)
                {
                    caretPosition = txt.SelectionLength; // caretPosition must be set to TextBox's SelectionLength
                    txt.CaretIndex = caretPosition;
                }
                if (txt.SelectionLength == 0 && txt.CaretIndex != 0)
                {
                    caretPosition = txt.CaretIndex;
                }
            }
            //throw new NotImplementedException();
        }

        //private void OnDropSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    TextBox txt = (TextBox)sender;
        //    txt.SelectionLength = 0;
        //    //if (base.IsDropDownOpen && txt.SelectionLength > 0)
        //    //{
        //    //    txt.Sele
        //    //    caretPosition = txt.SelectionLength; // caretPosition must be set to TextBox's SelectionLength
        //    //    txt.CaretIndex = caretPosition;
        //    //}
        //    //if (txt.SelectionLength == 0 && txt.CaretIndex != 0)
        //    //{
        //    //    caretPosition = txt.CaretIndex;
        //    //}
        //}

    }
}
