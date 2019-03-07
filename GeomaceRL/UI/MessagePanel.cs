using BearLib;
using System;
using System.Collections.Generic;

namespace GeomaceRL.UI
{
    // Accepts messages from various systems and displays it in the message console. Also keeps a 
    // rolling history of messages that can be displayed.
    public class MessagePanel
    {
        private readonly int _maxSize;
        private readonly IList<string> _messages;

        public MessagePanel(int maxSize)
        {
            _maxSize = maxSize;
            _messages = new List<string>();
        }

        // Place a new message onto the message log if its MessageLevel is lower than the currently
        // set level.
        public void AddMessage(string text)
        {
            _messages.Add(text);

            if (_messages.Count > _maxSize)
                _messages.RemoveAt(0);
        }

        // Modify the last message by adding additional text.
        public void AppendMessage(string text)
        {
            int prev = _messages.Count - 1;
            _messages[prev] += " " + text;
        }

        public void Clear()
        {
            _messages.Clear();
        }

        public void Draw(LayerInfo layer)
        {
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopLeftChar = '┼',
                TopRightChar = '┤',
                TopChar = '─',
                LeftChar = '│',
                RightChar = '│'
            });

            layer.Print(-1, $"{Constants.HEADER_LEFT}[color=white]Messages[/color]{Constants.HEADER_RIGHT}",
                System.Drawing.ContentAlignment.TopCenter);

            Terminal.Color(Colors.Text);
            // draw messages
            int maxCount = Math.Min(_messages.Count, layer.Height);
            int yPos = layer.Height - 1;

            for (int i = 0; i < maxCount; i++)
            {
                layer.Print(yPos, _messages[_messages.Count - i - 1]);
                yPos--;
            }
        }
    }
}
