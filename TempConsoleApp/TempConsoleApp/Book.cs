using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempConsoleApp
{
    internal class Book
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "Name";
        private int _qty = 0;
        public int Qty
        {
            get
            {
                return _qty > 0 ? 2 : _qty;
            }
            set
            {
                if (value > 0)
                    _qty = 2;
                else
                    _qty = value;
            }
            //            get;
            //            set;
        }
    }
}
