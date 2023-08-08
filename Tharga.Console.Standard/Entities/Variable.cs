namespace Tharga.Console.Entities
{
    internal class Variable
    {
        public Variable(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }
    }
}