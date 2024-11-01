using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SharpGDX.Input
{
    public interface TextInputWrapper
    {

        string getText();

        int getSelectionStart();

        int getSelectionEnd();

        void setText(string text);

        void setPosition(int position);

        bool shouldClose();
    }
}
