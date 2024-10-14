using System;
using System.Text;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.SourceGeneration;

public class RuleGenerator
{
    private readonly PluralRule _rule;
    private int _innerIndent;

    public RuleGenerator(PluralRule rule)
    {
        _rule = rule;
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
        WriteLine(builder, "return \"other\";", indent);
    }

    private void WriteNext(Condition condition, StringBuilder builder, int indent)
    {
        if(condition.OrConditions.Count > 0)
        {
            builder.Append(' ', _innerIndent + indent);
            builder.Append("if (");

            for (int orIdx = 0; orIdx < condition.OrConditions.Count; orIdx++)
            {
                OrCondition orCondition = condition.OrConditions[orIdx];
                var orIsLast = orIdx == condition.OrConditions.Count - 1;

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

            WriteLine(builder, string.Empty, indent);
        }
        else
        {
            throw new InvalidOperationException("Expected to have at least one or condition, but got none");
        }
    }

    private void WriteOrCondition(StringBuilder builder, OrCondition orCondition)
    {
        for (int andIdx = 0; andIdx < orCondition.AndConditions.Count; andIdx++)
        {
            var andIsLast = andIdx == orCondition.AndConditions.Count - 1;
            Operation andCondition = orCondition.AndConditions[andIdx];
            builder.Append('(');

            for (int innerOrIdx = 0; innerOrIdx < andCondition.OperandRight.Count; innerOrIdx++)
            {
                var isLast = innerOrIdx == andCondition.OperandRight.Count - 1;

                var leftVariable = andCondition.OperandLeft switch
                {
                    VariableOperand op => $"context.{OperandToVariable(op.Operand)}",
                    ModuloOperand op => $"context.{OperandToVariable(op.Operand)} % {op.ModValue}",
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
            OperandSymbol.AbsoluteValue => 'N',
            OperandSymbol.IntegerDigits => 'I',
            OperandSymbol.VisibleFractionDigitNumber => 'V',
            OperandSymbol.VisibleFractionDigitNumberWithoutTrailingZeroes => 'W',
            OperandSymbol.VisibleFractionDigits => 'F',
            OperandSymbol.VisibleFractionDigitsWithoutTrailingZeroes => 'T',
            OperandSymbol.ExponentC => 'C',
            OperandSymbol.ExponentE => 'E',
            _ => throw new InvalidOperationException($"Unknown variable {operand}")
        };
    }

    private void WriteLine(StringBuilder builder, string value, int indent)
    {
        builder.Append(' ', indent + _innerIndent);
        builder.AppendLine(value);
    }
}