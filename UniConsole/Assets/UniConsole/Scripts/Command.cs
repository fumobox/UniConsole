using System.Collections.Generic;
using System.Linq;

namespace UniConsole
{

    public class Command
    {
        public string Name { get; set; }

        public Dictionary<string, string> Options { get; private set; }

        public Command()
        {
            Options = new Dictionary<string, string>();
        }

        public bool Equals(string name)
        {
            return Name == name.ToLower();
        }

        public bool HasOptions
        {
            get
            {
                return Options.Count != 0;
            }
        }

        public override string ToString()
        {
            return Name + " " + Options.Select(kv => kv.Key + "=" + kv.Value).DefaultIfEmpty("").Aggregate((a, b) => a + " " + b);
        }

    }

}