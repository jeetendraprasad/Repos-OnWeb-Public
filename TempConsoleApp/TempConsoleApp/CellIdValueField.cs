using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempConsoleApp
{
    internal class CellIdValueField(int size)
    {
        //int _size = size;

        public int Size { get => _inputVal.Length; }

        public void Init(int size)
        {
            _inputVal = new string[size];
        }

        private string[] _inputVal = new string[size];

        //public string this[int index]
        //{
        //    get => Convert.ToInt32(_inputVal[index]) < 10 ? "10" : _inputVal[index];
        //    set => _inputVal[index] = Convert.ToInt32(value) < 10 ? "10" : Convert.ToInt32(value) > 500 ? "500" : value;
        //}

        public string this[int index]
        {
            get
            {
                int retVal;

                Console.WriteLine($"Index = {index} and _inputVal Size = {_inputVal.Length}");

                if (index < 0 || index >= size)
                {
                    return "";
                }
                else if (string.IsNullOrEmpty(_inputVal[index]))
                {
                    return "";
                }
                else if (int.TryParse(_inputVal[index], out retVal) == false)
                {
                    return "";
                }
                else
                {
                    if (retVal > 16)
                        retVal = 16;
                    else if (retVal < 1)
                        retVal = 0;
                }
                return retVal.ToString(CultureInfo.InvariantCulture);
            }
            set
            {

                int retVal = 0;

                Console.WriteLine($"Index = {index} and _inputVal Size = {_inputVal.Length}");

                if (index < 0 || index >= size)
                {
                    _inputVal[index] = "";
                }
                else if (string.IsNullOrEmpty(value))
                {
                    _inputVal[index] = "";
                }
                else if (int.TryParse(value, out retVal) == false)
                {
                    _inputVal[index] = "";
                }
                else
                {
                    if (retVal > 16)
                        retVal = 16;
                    else if (retVal < 1)
                        retVal = 0;
                }

                _inputVal[index] = retVal.ToString(CultureInfo.InvariantCulture); ;
            }
        }
    }
}
