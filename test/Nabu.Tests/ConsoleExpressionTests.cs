using System.Linq;
using Iago.ConsoleWriter;
using NFluent;
using Xunit;

namespace Nabu.Tests
{
  
}

namespace Iago.ConsoleWriter
{
    using System.Text.RegularExpressions;
    using System.Collections.Generic;
    using System.Collections;
    using System;
    using System.Text;

    public interface IConsoleExpressionOutputAdapter
    {
        string Adapt(ConsoleTokens expression);
    }

    public class StringOutputAdapter : IConsoleExpressionOutputAdapter
    {
        public string Adapt(ConsoleTokens expression)
        {
            var buffer = new StringBuilder();
            foreach (var item in expression)
            {
                buffer.Append(item.State);
            }
            return buffer.ToString();
        }
    }

    public class AnsiConsoleOutputAdapter : IConsoleExpressionOutputAdapter
    {
        public string Adapt(ConsoleTokens expression)
        {
            var buffer = new StringBuilder();
            foreach (var item in expression)
            {
                if(item.Action == "red")
                {
                    buffer.Append($"\x1b[31m{item.State}\x1b[39m");
                } 
                else if (item.Action == "normal")
                {
                    buffer.Append($"\x1b[39m{item.State}");
                }
                else {
                    buffer.Append(item.State);
                }
            }
            return buffer.ToString();
        }
    }
    public class ConsoleStringToken
    {
        public string Action { get; }
        public string State { get; }
        public ConsoleStringToken(string action, string state)
        {
            Action = action;
            State = state;
        }

        public override string ToString()
        {
            return $"{Action}('{State}')";
        }
    }

    
    public class ConsoleTokens : IEnumerable<ConsoleStringToken>
    {
        private List<ConsoleStringToken> tokens;

        public ConsoleTokens(IEnumerable<ConsoleStringToken> tokens)
        {
            this.tokens = new List<ConsoleStringToken>(tokens);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<ConsoleStringToken> GetEnumerator()
        {
            return tokens.GetEnumerator();
        }

        public string Print(IConsoleExpressionOutputAdapter adapter)
        {
            return adapter.Adapt(this);
        }
    }

    public class BadConsoleWriteTokenException : Exception
    {
        public BadConsoleWriteTokenException(string message):base(message)
        {
        }
    }

    public class ConsoleHelper
    {
        public static Func<string> TokenizePattern {get;set;} =
            ()=>@"#(?<action>\w+)`(?<content>[^`]*)`";

        public static ConsoleTokens Tokenize(string input, string pattern = null)
        {
            string currentPattern = pattern ?? TokenizePattern();
            if(!currentPattern.Contains("action")
                || !currentPattern.Contains("content"))
                {
                    throw new BadConsoleWriteTokenException(
                        "your pattern does not contain action, and content groups");
                }
            var rex = new Regex(currentPattern);
            Match match = rex.Match(input);
            var tokens = new List<ConsoleStringToken>();
            var bufferInput = input;
            var lastIndexEnd = 0;

            while(match.Success)
            {
                //match.GetType().GetProperties().ToList().ForEach(Console.WriteLine);
                if(match.Index != lastIndexEnd)
                {
                    tokens.Add(new ConsoleStringToken("normal",input.Substring(lastIndexEnd,(match.Index-lastIndexEnd))));
                }


                var actionName = match.Groups["action"].Value;
                var content = match.Groups["content"].Value;

                if(!string.IsNullOrEmpty(actionName))
                {
                    tokens.Add(new ConsoleStringToken(actionName,content));
                }
                lastIndexEnd = match.Index + match.Length;
                if(lastIndexEnd>input.Length) lastIndexEnd = input.Length;
                match = match.NextMatch();
            }
            return new ConsoleTokens(tokens);
        }
    }

}

