using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit.Console.Command.Base
{
    internal class InputBuffer
    {
        private readonly List<char> _inputBuffer = new List<char>();

        public int Length { get { return _inputBuffer.Count; } }
        public bool IsEmpty { get { return !_inputBuffer.Any(); } }

        public char? LastOrDefault()
        {
            return _inputBuffer.LastOrDefault();
        }

        public void Insert(int index, string input)
        {
            foreach (var chr in input.ToCharArray())
            {
                _inputBuffer.Insert(index++, chr);
            }
        }

        public void RemoveAt(int index)
        {
            _inputBuffer.RemoveAt(index);
        }

        public void Clear()
        {
            _inputBuffer.Clear();
        }

        public override string ToString()
        {
            return new string(_inputBuffer.ToArray());
        }
    }
}