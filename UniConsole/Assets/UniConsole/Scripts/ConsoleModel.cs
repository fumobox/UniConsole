using UniRx;
using System.Collections.Generic;
using System.Linq;

namespace UniConsole
{

    public class ConsoleModel
    {

        List<string> _lines;

        int _fontSize;

        int _maxLines;

        Dictionary<string, Command> _commands;

        public ReactiveProperty<bool> Visible { get; private set;}

        public ReactiveProperty<string> Text { get; private set; }

        public Subject<Command> CommandStream { get; private set; }

        public ConsoleModel(int fontSize = 30, int maxLines = 256)
        {
            _fontSize = fontSize;
            _lines = new List<string>();
            _maxLines = maxLines;
            _commands = new Dictionary<string, Command>();

            Visible = new ReactiveProperty<bool>();
            Text = new ReactiveProperty<string>();
            CommandStream = new Subject<Command>();
        }

        public List<string> Lines
        {
            get
            {
                return _lines;
            }
        }

        public int MaxLines
        {
            get
            {
                return _maxLines;
            }
        }

        public int FontSize
        {
            get
            {
                return _fontSize;
            }
        }

        public void AddCommand(Command command)
        {
            _commands.Add(command.Name, command);
        }

        public void WriteLine(string line, string prefix = null, bool updateView = true)
        {
            if (string.IsNullOrEmpty(prefix))
                _lines.Add(line);
            else
                _lines.Add(prefix + line);

            var command = ParseCommand(line);
            if (command != null)
                CommandStream.OnNext(command);

            if (_lines.Count > _maxLines)
            {
                _lines = _lines.GetRange(_lines.Count - _maxLines, _maxLines);
            }

            if(updateView)
            {
                Text.Value = _lines.DefaultIfEmpty("").Aggregate((a, b) => a + System.Environment.NewLine + b);
            }
        }

        public void Clear()
        {
            _lines.Clear();
            Text.Value = "";
        }

        public Command ParseCommand(string line)
        {
            if (string.IsNullOrEmpty(line))
                return null;

            var array = line.Split(new char[] { ' ' });
            if (array.Length == 1)
            {
                var command = new Command();
                command.Name = array[0].ToLower();
                return command;
            }
            else if (array.Length > 1)
            {
                var command = new Command();
                command.Name = array[0].ToLower();
                var options = array.Skip(1).Take(array.Length - 1).ToArray();

                for (int i = 0; i < options.Length; i++)
                {
                    var arr2 = options[i].Split(new char[] { '=' });
                    if (arr2.Length == 2)
                    {
                        command.Options.Add(arr2[0].Trim(), arr2[1].Trim());
                    }
                }

                return command;
            }
            else
            {
                return null;
            }
        }

    }


}
