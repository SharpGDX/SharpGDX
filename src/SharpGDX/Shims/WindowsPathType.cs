using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGDX.Shims
{
    enum WindowsPathType
    {
        ABSOLUTE,                   //  C:\foo
        UNC,                        //  \\server\share\foo
        RELATIVE,                   //  foo
        DIRECTORY_RELATIVE,         //  \foo
        DRIVE_RELATIVE              //  C:foo
    }
}
