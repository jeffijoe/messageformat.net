using System;
using System.Collections.Generic;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing
{
    public class RuleParser
    {
        private readonly string _ruleText;
        private int _position;

        public RuleParser(string ruleText)
        {
            _ruleText = ruleText;
        }

        public IReadOnlyList<OrCondition> ParseRuleContent()
        {
            var conditions = new List<OrCondition>();

            while (!IsEnd)
            {
                if (PeekCurrentChar == '@')
                {
                    return conditions;
                }

                var condition = ParseOrCondition();
                conditions.Add(condition);

                AdvanceWhitespace();
                
                if (IsEnd)
                {
                    return conditions;
                }

                var character = ConsumeChar();

                // This is where the samples start, we don't care about any of those.
                if (character == '@')
                {
                    return conditions;
                }

                // We expect the next token to be "or"
                var characterNext = ConsumeChar();
                if (character == 'o' && characterNext == 'r')
                {
                    continue;
                }

                throw new InvalidCharacterException(character);
            }

            return conditions;
        }

        private static readonly char NullCharacter = '\0';

        private char PeekCurrentChar =>
            _position < _ruleText.Length
            ? _ruleText[_position]
            : NullCharacter;

        private char PeekNextChar =>
            _position + 1 < _ruleText.Length
            ? _ruleText[_position + 1]
            : NullCharacter;

        private char PeekAt(int delta)
        {
            if (_position + delta >= _ruleText.Length)
                return NullCharacter;

            return _ruleText[_position + delta];
        }

        private ReadOnlySpan<char> ConsumeCharacters(int count)
        {
            if (_position + count > _ruleText.Length)
            {
                var characters = _ruleText.AsSpan(_position, _ruleText.Length - _position);
                _position = _ruleText.Length;
                return characters;
            }
            else
            {
                var characters = _ruleText.AsSpan(_position, count);
                _position += count;
                return characters;
            }
        }

        private char ConsumeChar()
        {
            if (IsEnd)
                return NullCharacter;

            var character = PeekCurrentChar;
            _position++;
            return character;
        }

        private bool IsEnd => _position >= _ruleText.Length;

        private void AdvanceWhitespace()
        {
            while (!IsEnd && char.IsWhiteSpace(PeekCurrentChar))
            {
                ConsumeChar();
            }
        }

        private ILeftOperand ParseLeftOperand()
        {
            var operandSymbol = ConsumeChar() switch
            {
                'n' => OperandSymbol.AbsoluteValue,
                'i' => OperandSymbol.IntegerDigits,
                'v' => OperandSymbol.VisibleFractionDigitNumber,
                'w' => OperandSymbol.VisibleFractionDigitNumberWithoutTrailingZeroes,
                'f' => OperandSymbol.VisibleFractionDigits,
                't' => OperandSymbol.VisibleFractionDigitsWithoutTrailingZeroes,
                'c' => OperandSymbol.ExponentC,
                'e' => OperandSymbol.ExponentE,
                var otherCharacter => throw new InvalidCharacterException(otherCharacter)
            };

            AdvanceWhitespace();

            if(PeekCurrentChar == '%')
            {
                ConsumeChar();
                AdvanceWhitespace();

                var number = ParseNumber();

                return new ModuloOperand(operandSymbol, number);
            }

            return new VariableOperand(operandSymbol);
        }

        private Operation ParseAndCondition()
        {
            AdvanceWhitespace();
            var leftOperand = ParseLeftOperand();
            

            AdvanceWhitespace();
            var firstRelationCharacter = ConsumeChar();
            var relation = firstRelationCharacter switch
            {
                '=' => Relation.Equals,
                '!' when ConsumeChar() == '='
                    => Relation.NotEquals,
                var otherCharacter => throw new InvalidCharacterException(otherCharacter)
            };

            AdvanceWhitespace();
            var rightOperand = ParseRightOperand();
            return new Operation(leftOperand, relation, rightOperand);
        }

        private IReadOnlyList<IRightOperand> ParseRightOperand()
        {
            var numbers = new List<IRightOperand>();

            while (!IsEnd) 
            {
                AdvanceWhitespace();

                var number = ParseNumber();
                if (PeekCurrentChar == '.')
                {
                    if (PeekNextChar == '.')
                    {
                        ConsumeCharacters(2);
                        AdvanceWhitespace();

                        var nextNumber = ParseNumber();
                        numbers.Add(new RangeOperand(number, nextNumber));
                    }
                    else
                    {
                        throw new InvalidCharacterException(PeekCurrentChar);
                    }
                }
                else
                {
                    numbers.Add(new NumberOperand(number));
                }

                if (PeekCurrentChar == ',')
                {
                    ConsumeChar();
                    AdvanceWhitespace();
                    continue;
                }
                else
                {
                    break;
                }
            }

            return numbers;
        }

        private OrCondition ParseOrCondition()
        {
            var andWordSpan = "and".AsSpan();

            var andConditions = new List<Operation>();
            while (!IsEnd)
            {
                var operation = ParseAndCondition();
                andConditions.Add(operation);

                AdvanceWhitespace();

                if (PeekCurrentChar == 'a')
                {
                    var andWord = ConsumeCharacters(3);


                    if (andWord.SequenceEqual(andWordSpan))
                    {
                        continue;
                    }

                    throw new InvalidCharacterException(andWord[0]);
                }

                return new OrCondition(andConditions);
            }

            return new OrCondition(andConditions);
        }

        private int ParseNumber()
        {
            int numbersCount = 0;
            while (!IsEnd && char.IsNumber(PeekAt(numbersCount)))
            {
                numbersCount++;
            }

            var numberSpan = ConsumeCharacters(numbersCount);
            
            var number = int.Parse(numberSpan.ToString());

            return number;
        }
    }
}
