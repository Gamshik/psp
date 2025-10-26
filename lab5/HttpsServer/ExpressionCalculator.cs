using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HttpsServer;

public static class ExpressionCalculator
{
    private static readonly Dictionary<string, int> OperatorPrecedence = new()
    {
        { "+", 1 },
        { "-", 1 },
        { "*", 2 },
        { "/", 2 }
    };

    public static double Evaluate(string expression)
    {
        var tokens = Tokenize(expression);

        var rpnTokens = ToReversePolishNotation(tokens);

        return EvaluateRpn(rpnTokens);
    }

    private static List<string> Tokenize(string expression)
    {
        string cleanExpression = expression.Replace(" ", "");

        var tokens = new List<string>();

        for (int i = 0; i < cleanExpression.Length; i++)
        {
            char c = cleanExpression[i];

            if (char.IsDigit(c) || c == '.')
            {
                string num = "";
                while (i < cleanExpression.Length && (char.IsDigit(cleanExpression[i]) || cleanExpression[i] == '.'))
                {
                    num += cleanExpression[i];
                    i++;
                }
                tokens.Add(num);
                i--; 
            }
            else if (c is '+' or '*' or '/' or '(' or ')')
            {
                tokens.Add(c.ToString());
            }
            else if (c == '-')
            {
                bool isUnary = (tokens.Count == 0 || tokens.Last() == "(")
                               && i + 1 < cleanExpression.Length
                               && (char.IsDigit(cleanExpression[i + 1]) || cleanExpression[i + 1] == '.');

                if (isUnary)
                {
                    string num = "-";
                    i++;
                    while (i < cleanExpression.Length && (char.IsDigit(cleanExpression[i]) || cleanExpression[i] == '.'))
                    {
                        num += cleanExpression[i];
                        i++;
                    }
                    tokens.Add(num);
                    i--; 
                }
                else
                {
                    tokens.Add("-");
                }
            }
            else
            {
                throw new ArgumentException($"Недопустимый символ в выражении: '{c}'");
            }
        }
        return tokens;
    }
    private static List<string> ToReversePolishNotation(List<string> tokens)
    {
        var output = new List<string>();
        var operators = new Stack<string>();

        foreach (var token in tokens)
        {
            if (double.TryParse(token, out _))
            {
                output.Add(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }

                if (operators.Count == 0 || operators.Pop() != "(")
                {
                    throw new ArgumentException("Пропущена открывающая скобка.");
                }
            }
            else if (OperatorPrecedence.ContainsKey(token))
            {
                while (operators.Count > 0 && operators.Peek() != "(" &&
                       ((OperatorPrecedence[operators.Peek()] > OperatorPrecedence[token]) ||
                        (OperatorPrecedence[operators.Peek()] == OperatorPrecedence[token])))
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
            else
            {
                throw new ArgumentException($"Недопустимый оператор: {token}");
            }
        }

        while (operators.Count > 0)
        {
            var op = operators.Pop();
            if (op == "(" || op == ")")
            {
                throw new ArgumentException("Неверное расположение скобок.");
            }
            output.Add(op);
        }

        return output;
    }

    private static double EvaluateRpn(List<string> rpnTokens)
    {
        var stack = new Stack<double>();

        foreach (var token in rpnTokens)
        {
            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
            {
                stack.Push(number);
            }
            else if (OperatorPrecedence.ContainsKey(token))
            {
                if (stack.Count < 2)
                {
                    throw new ArgumentException("Недостаточно операндов для оператора.");
                }
                double operand2 = stack.Pop();
                double operand1 = stack.Pop();
                double result = token switch
                {
                    "+" => operand1 + operand2,
                    "-" => operand1 - operand2,
                    "*" => operand1 * operand2,
                    "/" => operand1 / operand2,
                    _ => throw new ArgumentException($"Неизвестный оператор: {token}")
                };
                stack.Push(result);
            }
        }

        if (stack.Count != 1)
        {
            throw new ArgumentException("Выражение имеет неверный формат.");
        }

        return stack.Pop();
    }
}