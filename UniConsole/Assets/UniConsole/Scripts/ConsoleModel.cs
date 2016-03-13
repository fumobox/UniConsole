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

        public ReactiveProperty<bool> Visible { get; private set;}

        public ReactiveProperty<string> Text { get; private set; }

        public Subject<Command> CommandStream { get; private set; }

        public ConsoleModel(int fontSize = 30, int maxLines = 256)
        {
            _fontSize = fontSize;
            _lines = new List<string>();
            _maxLines = maxLines;

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

        public void WriteLine(string line, bool updateView = true)
        {
            WriteLine(new List<string> { line }, updateView);
        }

        public void WriteLine(List<string> lines, bool updateView = true)
        {
            if (lines.Count == 0)
                return;

            foreach(var line in lines)
                _lines.Add(line);

            if (_lines.Count > _maxLines)
                _lines = _lines.GetRange(_lines.Count - _maxLines, _maxLines);

            if(updateView)
                Text.Value = _lines.DefaultIfEmpty("").Aggregate((a, b) => a + System.Environment.NewLine + b);
        }

        public void Clear()
        {
            _lines.Clear();
            Text.Value = "";
        }

        /// <summary>
        /// Parse command.
        /// </summary>
        /// <remarks>
        /// [Example]
        /// open temp.text
        /// create width=32 height = 16
        /// </remarks>
        public void ParseCommand(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            Command command = null;

            var array = line.Split(new char[] { ' ' });
            if (array.Length == 1)
            {
                command = new Command();
                command.Name = array[0].ToLower();
            }
            else if (array.Length > 1)
            {
                command = new Command();
                command.Name = array[0].ToLower();
                var options = array.Skip(1).Take(array.Length - 1).ToArray();

                for (int i = 0; i < options.Length; i++)
                {
                    var optionString = options[i];
                    if (optionString.Contains("="))
                    {
                        var arr2 = optionString.Split(new char[] { '=' });
                        if (arr2.Length == 2)
                        {
                            command.Options.Add(arr2[0].Trim(), arr2[1].Trim());
                        }
                    }
                    else
                    {
                        command.Options.Add("argument" + i, optionString.Trim());
                    }
                }
            }

            if(command != null)
                CommandStream.OnNext(command);
        }

    }


}
