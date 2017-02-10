#region

using System;
using System.Collections.Generic;

#endregion

namespace Rnwood.SmtpServer
{
    public class ParameterParser
    {
        private readonly List<Parameter> _parameters = new List<Parameter>();

        public ParameterParser(params string[] arguments)
        {
            Parse(arguments);
        }

        public Parameter[] Parameters => _parameters.ToArray();

        private void Parse(string[] tokens)
        {
            foreach (var token in tokens)
            {
                var tokenParts = token.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                var key = tokenParts[0];
                var value = tokenParts.Length > 1 ? tokenParts[1] : null;
                _parameters.Add(new Parameter(key, value));
            }
        }
    }
}