using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit.Console.Entities
{
    public class CommandTreeNode<T>
    {
        private readonly CommandTreeNode<T> _parent;
        public T Key { get; }
        public string Value { get; }
        public List<CommandTreeNode<T>> Subs { get; }

        public string FullValuePath => _parent?._parent == null ? Value : $"{_parent.FullValuePath} {Value}";

        private CommandTreeNode(CommandTreeNode<T> parent, T key, string value, IEnumerable<CommandTreeNode<T>> subs)
        {
            _parent = parent;
            Key = key;
            Value = value;
            Subs = subs?.Select(x => new CommandTreeNode<T>(this, x.Key, x.Value, x.Subs)).ToList() ?? new List<CommandTreeNode<T>>();
        }

        public CommandTreeNode(IEnumerable<CommandTreeNode<T>> subs)
            : this(null, default(T), "root", subs)
        {
        }

        public CommandTreeNode(T key, string value, IEnumerable<CommandTreeNode<T>> subs = null)
            : this(null, key, value, subs)
        {
        }

        public CommandTreeNode<T> Select(string entry)
        {
            return FindNode(entry, Subs);
        }

        private CommandTreeNode<T> FindNode(string entry, List<CommandTreeNode<T>> nodes)
        {
            var segments = entry.Split(' ');
            foreach (var segment in segments)
            {
                var exactHit = nodes.FirstOrDefault(x => x.Value.Equals(segment, StringComparison.InvariantCultureIgnoreCase));
                var hits = nodes.Where(x => x.Value.StartsWith(segment, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                if (hits.Length == 1 && string.Equals(hits.First().Value, segment, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (string.Equals(entry, segment, StringComparison.InvariantCultureIgnoreCase))
                        return hits.First();

                    var subEntry = entry.Substring(segment.Length + 1);
                    return FindNode(subEntry, hits.First().Subs);
                }
                else if (segments.Length > 1 && exactHit != null)
                {
                    var subEntry = entry.Substring(segment.Length + 1);
                    return FindNode(subEntry, hits.First().Subs);
                }

                if (hits.Length >= 1)
                {
                    return hits.First();
                }
            }

            return nodes.FirstOrDefault();
        }

        public CommandTreeNode<T> Previous(CommandTreeNode<T> selection)
        {
            var pLoc = FindParentLocation(selection, Subs);
            var arr = pLoc?.Item1.Subs.ToArray();
            return arr?[pLoc.Item2 - 1 < 0 ? arr.Length - 1 : pLoc.Item2 - 1];
        }

        public CommandTreeNode<T> Next(CommandTreeNode<T> selection)
        {
            var pLoc = FindParentLocation(selection, Subs);
            var arr = pLoc?.Item1.Subs.ToArray();
            return arr?[pLoc.Item2 + 1 >= arr.Length ? 0 : pLoc.Item2 + 1];
        }

        private Tuple<CommandTreeNode<T>, int> FindParentLocation(CommandTreeNode<T> selection, List<CommandTreeNode<T>> nodes)
        {
            var ns = nodes.ToArray();
            for (var i = 0; i < ns.Length; i++)
            {
                if (ReferenceEquals(ns[i], selection))
                {
                    return new Tuple<CommandTreeNode<T>, int>(ns[i]._parent, i);
                }

                var p = FindParentLocation(selection, ns[i].Subs);
                if (p != null)
                {
                    return p;
                }
            }
            return null;
        }
    }
}