using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIDA64Reader.Utils
{
    public static class Extension
    {
        public static void ForEach<T>(this T[] array,Action<T> action)
        {
            array.ForEach(item => {
                action.Invoke(item);
            });
        }
    }
}
