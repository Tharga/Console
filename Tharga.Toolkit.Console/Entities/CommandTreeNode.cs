using System.Collections.Generic;

namespace Tharga.Toolkit.Console.Entities
{
    public class CommandTreeNode
    {
        public string Name { get; }
        public List<CommandTreeNode> Subs { get; }

        public CommandTreeNode(string name, List<CommandTreeNode> subs = null)
        {
            Name = name;
            Subs = subs;
        }
    }
}