using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit.Console.Command.Base
{
    internal class InputBuffer
    {
        private readonly List<char> _inputBuffer = new List<char>();

        public int Length { get { return this._inputBuffer.Count; } }
        public bool IsEmpty { get { return !this._inputBuffer.Any(); } }

        public char? LastOrDefault()
        {
            return this._inputBuffer.LastOrDefault();
        }

        public void Insert(int index, char keyChar)
        {
            this._inputBuffer.Insert(index, keyChar);
        }

        public void RemoveAt(int index)
        {
            _inputBuffer.RemoveAt(index);
        }

        public void Clear()
        {
            this._inputBuffer.Clear();
        }

        public override string ToString()
        {
            return new string(this._inputBuffer.ToArray());
        }
    }
}