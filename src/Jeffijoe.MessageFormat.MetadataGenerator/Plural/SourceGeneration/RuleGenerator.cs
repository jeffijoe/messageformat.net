using System;
using System.Collections.Generic;
using System.Text;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration
{
    public class RuleGenerator
    {
        private PluralRule _rule;
        private List<OperandSymbol> _initializedSymbols;
        private int _innerIndent;

        public RuleGenerator(PluralRule rule)
        {
            _rule = rule;
            _initializedSymbols = new List<OperandSymbol>();
            _innerIndent = 0;
        }

        public void WriteTo(StringBuilder builder, int indent)
        {
            foreach(var condition in _rule.Conditions)
            {
                WriteNext(condition, builder, indent);
            }

            WriteOther(builder, indent);
        }

        private void WriteOther(StringBuilder builder, int indent)
        {
            if (_rule.Conditions.Length > 0)
                WriteLine(builder, string.Empty, indent);

            WriteLine(builder, "return \"other\";", indent);
        }

        private void WriteNext(Condition condition, StringBuilder builder, int indent)
        {
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
                builder.Append(' ', _innerIndent + indent);
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
        }

        private void WriteOrCondition(StringBuilder builder, OrCondition orCondition)
        {
            for (int andIdx = 0; andIdx < orCondition.AndConditions.Length; andIdx++)
            {
                var andIsLast = andIdx == orCondition.AndConditions.Length - 1;
                Operation andCondition = orCondition.AndConditions[andIdx];
                builder.Append('(');

                for (int innerOrIdx = 0; innerOrIdx < andCondition.OperandRight.Length; innerOrIdx++)
                {
                    var isLast = innerOrIdx == andCondition.OperandRight.Length - 1;

                    var leftVariable = andCondition.OperandLeft switch
                    {
                        VariableOperand op => OperandToVariable(op.Operand).ToString(),
                        ModuloOperand op => $"{OperandToVariable(op.Operand)} % {op.ModValue}",
                        var otherOp => throw new InvalidOperationException($"Unknown operation {otherOp.GetType()}")
                    };

                    var line = andCondition.OperandRight[innerOrIdx] switch
                    {
                        RangeOperand range => andCondition.Relation == Relation.Equals
                            ? $"{leftVariable} >= {range.Start} && {leftVariable} <= {range.End}"
                            : $"({leftVariable} < {range.Start} || {leftVariable} > {range.End})",
                        NumberOperand number => andCondition.Relation == Relation.Equals
                            ? $"{leftVariable} == {number.Number}"
                            : $"{leftVariable} != {number.Number}",
                        var otherOperand => throw new InvalidOperationException($"Unknown right operand {otherOperand.GetType()}")
                    };
                    
                    builder.Append(line);

                    if (!isLast)
                    {
                        builder.Append(andCondition.Relation == Relation.Equals ? " || " : " && ");
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
                OperandSymbol.IntegerDigits => 'i',
                _ => throw new InvalidOperationException($"Unknown variable {operand}")
            };
        }

        private string InitializeValue(OperandSymbol operand)
        {
            return operand switch
            {
                OperandSymbol.AbsoluteValue => "var n = Math.Abs(value);",
                OperandSymbol.VisibleFractionDigitNumber => "var v = (int)value == value ? 0 : 1;",
                OperandSymbol.IntegerDigits => "var i = (int)value;",
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
