using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;

using System;
using System.Collections.Generic;
using System.Text;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration
{
    public class RuleGenerator
    {
        private PluralRule _rule;
        private int _conditionNumber;
        private List<OperandSymbol> _initializedSymbols;
        private int _innerIndent;

        public RuleGenerator(PluralRule rule)
        {
            _rule = rule;
            _conditionNumber = 0;
            HasNext = true;
            _initializedSymbols = new List<OperandSymbol>();
            _innerIndent = 0;
        }

        public bool HasNext { get; private set; }

        public void WriteNext(StringBuilder builder, int indent)
        {
            if(_conditionNumber > _rule.Conditions.Length - 1)
            {
                if(_rule.Conditions.Length > 0)
                    WriteLine(builder, string.Empty, indent);

                WriteLine(builder, "return \"other\";", indent);
                HasNext = false;
                return;
            }

            var condition = _rule.Conditions[_conditionNumber];
            foreach (var operand in GetAllLeftOperands(condition.OrConditions))
            {
                if(!_initializedSymbols.Contains(operand))
                {
                    var line = InitializeValue(operand);
                    WriteLine(builder, line, indent);
                    _initializedSymbols.Add(operand);
                }
            }

            WriteLine(builder, string.Empty, indent);

            if(condition.OrConditions.Length > 0)
            {
                builder.Append("if (");

                for (int orIdx = 0; orIdx < condition.OrConditions.Length; orIdx++)
                {
                    OrCondition orCondition = condition.OrConditions[orIdx];
                    var orIsLast = orIdx == condition.OrConditions.Length - 1;

                    WriteOrCondition(builder, orCondition);

                    if (!orIsLast)
                    {
                        builder.Append(" || ");
                    }
                }

                builder.AppendLine(")");

                _innerIndent += 4;
                WriteLine(builder, $"return \"{condition.Count}\";", indent);
                _innerIndent -= 4;
            }
            else
            {
                throw new InvalidOperationException("Expected to have at least one or condition, but got none");
            }


            _conditionNumber++;
        }

        private void WriteOrCondition(StringBuilder builder, OrCondition orCondition)
        {
            for (int andIdx = 0; andIdx < orCondition.AndConditions.Length; andIdx++)
            {
                var andIsLast = andIdx == orCondition.AndConditions.Length - 1;
                Operation andCondition = orCondition.AndConditions[andIdx];
                builder.Append('(');
                var csharpOperator = andCondition.Relation == Relation.Equals ? "==" : "!=";

                for (int innerOrIdx = 0; innerOrIdx < andCondition.OperandRight.Length; innerOrIdx++)
                {
                    var isLast = innerOrIdx == andCondition.OperandRight.Length - 1;

                    int number = andCondition.OperandRight[innerOrIdx];
                    if (andCondition.OperandLeft is VariableOperand op)
                    {
                        var variable = OperandToVariable(op.Operand);
                        builder.Append($"{variable} {csharpOperator} {number}");
                    }

                    if (!isLast)
                    {
                        builder.Append(" || ");
                    }
                }
                builder.Append(')');

                if (!andIsLast)
                {
                    builder.Append(" && ");
                }
            }
        }

        private char OperandToVariable(OperandSymbol operand)
        {
            return operand switch
            {
                OperandSymbol.AbsoluteValue => 'n',
                OperandSymbol.VisibleFractionDigitNumber => 'v',
                _ => throw new InvalidOperationException($"Unknown variable {operand}")
            };
        }

        private string InitializeValue(OperandSymbol operand)
        {
            return operand switch
            {
                OperandSymbol.AbsoluteValue => "var n = Math.Abs(value);",
                OperandSymbol.VisibleFractionDigitNumber => "var v = (int)value == value ? 0 : 1;",
                var otherSymbol => throw new InvalidOperationException($"Unknown operand symbol {otherSymbol}")
            };
        }

        private IEnumerable<OperandSymbol> GetAllLeftOperands(OrCondition[] conditions)
        {
            foreach (var condition in conditions)
            {
                foreach(var operation in condition.AndConditions)
                {
                    var operand = operation.OperandLeft switch
                    {
                        VariableOperand op => op.Operand,
                        ModuloOperand op => op.Operand,
                        var op => throw new InvalidOperationException($"Unexpected operand {op.GetType()}")
                    };

                    yield return operand;
                }
            }
        }

        private void WriteLine(StringBuilder builder, string value, int indent)
        {
            builder.Append(' ', indent + _innerIndent);
            builder.AppendLine(value);
        }
    }
}
