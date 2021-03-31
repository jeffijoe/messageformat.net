using System;
using System.Collections.Generic;
using System.Linq;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing
{
    public class RuleParser
    {
        private string _ruleText;
        private int _position;

        public RuleParser(string ruleText)
        {
            _ruleText = ruleText;
        }

        public OrCondition[] ParseRuleContent()
        {
            var conditions = new List<OrCondition>();

            while (!IsEnd)
            {
                if (PeekCurrentChar == '@')
                {
                    return conditions.ToArray();
                }

                var condition = ParseOrCondition();
                conditions.Add(condition);

                AdvanceWhitespace();

                var character = ConsumeChar();
                var characterNext = ConsumeChar();

                if (character == '@')
                {
                    return conditions.ToArray();
                }
                else if (character == 'o' && characterNext == 'r')
                {
                    continue;
                }
                else
                {
                    throw new InvalidCharacterException(character);
                }
            }

            return conditions.ToArray();
        }

        private char PeekCurrentChar =>
            _position < _ruleText.Length
            ? _ruleText[_position]
            : '0';

        private char PeekNextChar =>
            _position + 1 < _ruleText.Length
            ? _ruleText[_position + 1]
            : '0';

        private char PeekAt(int delta)
        {
            if (_position + delta > _ruleText.Length)
                return '0';

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
                return '0';

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
                'v' => OperandSymbol.VisibleFractionDigitNumber,
                'n' => OperandSymbol.AbsoluteValue,
                'i' => OperandSymbol.IntegerDigits,
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

        private IRightOperand[] ParseRightOperand()
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

            return numbers.ToArray();
        }

        private OrCondition ParseOrCondition()
        {
            var andConditions = new List<Operation>();
            while (!IsEnd)
            {
                var operation = ParseAndCondition();
                andConditions.Add(operation);

                AdvanceWhitespace();

                if (PeekCurrentChar == 'a')
                {
                    var andWord = ConsumeCharacters(3);
                    ReadOnlySpan<char> andWordExpected = stackalloc char[3]
                    {
                        'a',
                        'n',
                        'd'
                    };


                    if (andWord.SequenceEqual(andWordExpected))
                    {
                        continue;
                    }
                    else
                    {
                        throw new InvalidCharacterException(andWord[0]);
                    }
                }
                else
                {
                    return new OrCondition(andConditions.ToArray());
                }
            }

            return new OrCondition(andConditions.ToArray());
        }

        private int ParseNumber()
        {
            int numbersCount = 0;
            while (!IsEnd && char.IsNumber(PeekAt(numbersCount)))
            {
                numbersCount++;
            }

            var numberSpan = ConsumeCharacters(numbersCount);
            
            var number = int.Parse(new string(numberSpan.ToArray()));

            return number;
        }
    }
}
