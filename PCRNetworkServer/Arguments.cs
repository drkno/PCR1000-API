using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PCRNetworkServer
{
    public static class Arguments
    {
        private static Dictionary<string, string> _args = new Dictionary<string, string>();

        public static void LoadArguments(IEnumerable<string> args)
        {
            if (_args == null)
            {
                _args = new Dictionary<string, string>();
            }

            var spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var remover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;

            foreach (var parts in args.Select(txt => spliter.Split(txt, 3)))
            {
                switch (parts.Length)
                {
                    case 1:
                        {
                            if (parameter != null)
                            {
                                if (!_args.ContainsKey(parameter))
                                {
                                    parts[0] = remover.Replace(parts[0], "$1");
                                    _args.Add(parameter, parts[0]);
                                }
                                parameter = null;
                            }
                            break;
                        }
                    case 2:
                        {
                            if (parameter != null)
                            {
                                if (!_args.ContainsKey(parameter))
                                {
                                    _args.Add(parameter, "true");
                                }
                            }
                            parameter = parts[1];
                            break;
                        }
                    case 3:
                        {
                            if (parameter != null)
                            {
                                if (!_args.ContainsKey(parameter))
                                {
                                    _args.Add(parameter, "true");
                                }
                            }

                            parameter = parts[1];

                            if (!_args.ContainsKey(parameter))
                            {
                                parts[2] = remover.Replace(parts[2], "$1");
                                _args.Add(parameter, parts[2]);
                            }

                            parameter = null;
                            break;
                        }
                }
            }

            if (parameter != null && !_args.ContainsKey(parameter))
            {
                _args.Add(parameter, "true");
            }
        }

        public static string GetArgument(string arg)
        {
            try
            {
                return _args[arg];
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static bool GetArgumentBool(string arg)
        {
            var r = GetArgument(arg);
            bool t;
            if (!bool.TryParse(r, out t))
            {
                return false;
            }
            return t;
        }
    }
}
