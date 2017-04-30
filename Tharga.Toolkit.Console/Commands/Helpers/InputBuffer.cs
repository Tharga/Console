using System;
using System.Collections.Generic;
using System.Linq;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Commands.Helpers
{
    internal class InputBuffer
    {
        public event EventHandler<InputBufferChangedEventArgs> InputBufferChangedEvent;

        protected virtual void InvokeInputBufferChangedEvent()
        {
            var handler = InputBufferChangedEvent;
            handler?.Invoke(this, new InputBufferChangedEventArgs());
        }

        private readonly List<char> _inputBuffer = new List<char>();

        public int Length => _inputBuffer.Count;
        public bool IsEmpty => !_inputBuffer.Any();

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

            InvokeInputBufferChangedEvent();
        }

        public void Add(string value)
        {
            foreach (var chr in value.ToCharArray())
            {
                _inputBuffer.Add(chr);
            }

            InvokeInputBufferChangedEvent();
        }

        public void RemoveAt(int index)
        {
            _inputBuffer.RemoveAt(index);
            InvokeInputBufferChangedEvent();
        }

        public void Clear()
        {
            _inputBuffer.Clear();
            InvokeInputBufferChangedEvent();
        }

        public override string ToString()
        {
            return new string(_inputBuffer.ToArray());
        }
    }
}