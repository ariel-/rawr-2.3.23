using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Rawr
{
    public static class JsonParser
    {
        private enum Token
        {
            None,
            CurlyOpen,
            CurlyClose,
            SquareOpen,
            SquareClose,
            Comma,
            String,
            Number,
            Colon,
            False,
            True,
            Null,
        }

        private static Token Peek(string json, ref int index)
        {
            ConsumeWhitespace(json, ref index);

            if (index >= json.Length)
            {
                return Token.None;
            }

            switch (json[index])
            {
                case '{':
                    return Token.CurlyOpen;
                case '}':
                    return Token.CurlyClose;
                case '[':
                    return Token.SquareOpen;
                case ']':
                    return Token.SquareClose;
                case ',':
                    return Token.Comma;
                case '"':
                    return Token.String;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return Token.Number;
                case ':':
                    return Token.Colon;
                case 't':
                case 'T':
                    return Token.True;
                case 'f':
                case 'F':
                    return Token.False;
                case 'n':
                case 'N':
                    return Token.Null;
                default:
                    return Token.None;
            }
        }

        private static object ParseValue(string json, ref int index)
        {
            switch (Peek(json, ref index))
            {
                case Token.CurlyOpen:
                    return ParseObject(json, ref index);
                case Token.String:
                    return ParseString(json, ref index);
                case Token.SquareOpen:
                    return ParseArray(json, ref index);
                case Token.Number:
                    return ParseNumber(json, ref index);
                case Token.True:
                    return ParseTrue(json, ref index);
                case Token.False:
                    return ParseFalse(json, ref index);
                case Token.Null:
                    return ParseNull(json, ref index);
                default:
                    throw new ArgumentException("Unexpected token in JSON at index " + index);
            }
        }

        private static object ParseNull(string json, ref int index)
        {
            ConsumeChar(json, ref index, 'n', 'N');
            ConsumeChar(json, ref index, 'u', 'U');
            ConsumeChar(json, ref index, 'l', 'L');
            ConsumeChar(json, ref index, 'l', 'L');
            return false;
        }

        private static bool ParseFalse(string json, ref int index)
        {
            ConsumeChar(json, ref index, 'f', 'F');
            ConsumeChar(json, ref index, 'a', 'A');
            ConsumeChar(json, ref index, 'l', 'L');
            ConsumeChar(json, ref index, 's', 'S');
            ConsumeChar(json, ref index, 'e', 'E');
            return false;
        }

        private static bool ParseTrue(string json, ref int index)
        {
            ConsumeChar(json, ref index, 't', 'T');
            ConsumeChar(json, ref index, 'r', 'R');
            ConsumeChar(json, ref index, 'u', 'U');
            ConsumeChar(json, ref index, 'e', 'E');
            return true;
        }

        private static object ParseNumber(string json, ref int index)
        {
            int startindex = index;
            for (; index < json.Length; index++)
            {
                if ("0123456789+-.eE".IndexOf(json[index]) == -1)
                {
                    break;
                }
            }
            string num = json.Substring(startindex, index - startindex);
            if (num.Contains("."))
            {
                return double.Parse(num, CultureInfo.InvariantCulture);
            }
            else
            {
                return int.Parse(num, CultureInfo.InvariantCulture);
            }
        }

        private static object[] ParseArray(string json, ref int index)
        {
            List<object> arr = new List<object>();

            ConsumeChar(json, ref index, '[');

            switch (Peek(json, ref index))
            {
                case Token.SquareClose:
                    ConsumeChar(json, ref index, ']');
                    return arr.ToArray();
            }

            do
            {
                object value = ParseValue(json, ref index);
                arr.Add(value);

                switch (Peek(json, ref index))
                {
                    case Token.SquareClose:
                        ConsumeChar(json, ref index, ']');
                        return arr.ToArray();
                    case Token.Comma:
                        ConsumeChar(json, ref index, ',');
                        break;
                    default:
                        throw new ArgumentException("Unexpected token in JSON at index " + index);
                }
            } while (true);
        }

        private static string ParseString(string json, ref int index)
        {            
            ConsumeChar(json, ref index, '"');

            int start = index;
            StringBuilder sb = new StringBuilder();

            while (index < json.Length)
            {
                switch (json[index])
                {
                    case '"':
                        index++;
                        return sb.ToString();
                    case '\\':
                        index++;
                        switch (json[index])
                        {
                            case '"':
                                index++;
                                sb.Append('"');
                                break;
                            case '\\':
                                index++;
                                sb.Append('\\');
                                break;
                            case '/':
                                index++;
                                sb.Append('/');
                                break;
                            case 'b':
                                index++;
                                sb.Append('\b');
                                break;
                            case 'f':
                                index++;
                                sb.Append('\f');
                                break;
                            case 'n':
                                index++;
                                sb.Append('\n');
                                break;
                            case 'r':
                                index++;
                                sb.Append('\r');
                                break;
                            case 't':
                                index++;
                                sb.Append('\t');
                                break;
                            case 'u':
                                sb.Append(Char.ConvertFromUtf32(int.Parse(json.Substring(index, 4), NumberStyles.HexNumber)));
                                index += 4;
                                break;
                            default:
                                throw new ArgumentException("Unexpected escape sequence in JSON at index " + start);
                        }
                        break;
                    default:
                        sb.Append(json[index]);
                        index++;
                        break;
                }
            }
            throw new ArgumentException("Unterminated string in JSON at index " + start);
        }

        private static void ConsumeChar(string json, ref int index, params char[] validChars)
        {
            if (index >= json.Length || Array.IndexOf(validChars, json[index]) == -1)
            {
                throw new ArgumentException("Unexpected token in JSON at index " + index);
            }
            index++;
        }

        private static void ConsumeWhitespace(string json, ref int index)
        {
            while (index < json.Length && char.IsWhiteSpace(json[index]))
            {
                index++;
            }
        }

        private static Dictionary<string, object> ParseObject(string json, ref int index)
        {
            ConsumeChar(json, ref index, '{');

            return ParseObjectBody(json, ref index, false);
        }

        private static Dictionary<string, object> ParseObjectBody(string json, ref int index, bool noneTerminated)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();

            switch (Peek(json, ref index))
            {
                case Token.CurlyClose:
                    ConsumeChar(json, ref index, '}');
                    return obj;
                case Token.None:
                    if (!noneTerminated)
                    {
                        throw new ArgumentException("Unexpected token in JSON at index " + index);
                    }
                    return obj;
                case Token.String:
                    // OK
                    break;
                default:
                    throw new ArgumentException("Unexpected token in JSON at index " + index);
            }

            do
            {
                string key = ParseString(json, ref index);
                ConsumeWhitespace(json, ref index);
                ConsumeChar(json, ref index, ':');
                ConsumeWhitespace(json, ref index);
                object value = ParseValue(json, ref index);
                obj[key] = value;

                switch (Peek(json, ref index))
                {
                    case Token.CurlyClose:
                        ConsumeChar(json, ref index, '}');
                        return obj;
                    case Token.None:
                        if (!noneTerminated)
                        {
                            throw new ArgumentException("Unexpected token in JSON at index " + index);
                        }
                        return obj;
                    case Token.Comma:
                        ConsumeChar(json, ref index, ',');
                        break;
                    default:
                        throw new ArgumentException("Unexpected token in JSON at index " + index);
                }
            } while (true);
        }

        public static Dictionary<string, object> Parse(string json)
        {
            int index = 0;
            return ParseObjectBody(json, ref index, true);
        }

        public static Dictionary<string, object> Merge(Dictionary<string, object> obj1, Dictionary<string, object> obj2)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>(obj1);
            foreach (var kvp in obj2)
            {
                obj[kvp.Key] = kvp.Value;
            }
            return obj;
        }
    }
}
