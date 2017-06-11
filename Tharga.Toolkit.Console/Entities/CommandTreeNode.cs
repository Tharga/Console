using System.Collections.Generic;

namespace Tharga.Toolkit.Console.Entities
{
    public class CommandTreeNode<T>
    {
        public T Key { get; }
        public string Value { get; }
        public List<CommandTreeNode<T>> Subs { get; }

        public CommandTreeNode(T key, string value, List<CommandTreeNode<T>> subs = null)
        {
            Key = key;
            Value = value;
            Subs = subs;
        }
    }
}