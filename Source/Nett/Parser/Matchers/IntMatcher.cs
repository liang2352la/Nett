﻿using System;
using System.Text;

namespace Nett.Parser.Matchers
{
    internal sealed class IntMatcher : MatcherBase
    {
        private bool hasPos = false;
        private bool hasSign = false;
        internal override Token? Match(LookaheadBuffer<char> cs)
        {
            this.hasPos = cs.PeekIs('+');
            this.hasSign = this.hasPos || cs.PeekIs('-');

            if (this.hasSign || cs.PeekInRange('0', '9'))
            {
                StringBuilder sb = new StringBuilder(16);
                sb.Append(cs.Consume());

                while (!cs.End && (cs.PeekInRange('0', '9') || cs.PeekIs('_')))
                {
                    sb.Append(cs.Consume());
                }

                if (cs.PeekIs('-') && sb.Length == 4 && !this.hasSign)
                {
                    var dtm = new DateTimeMatcher(sb);
                    return dtm.Match(cs);
                }
                else if (cs.PeekIs('.') && cs.LaIs(3, ':') && !this.hasPos)
                {
                    var tsm = new TimespanMatcher(sb);
                    return tsm.Match(cs);
                }

                if (cs.End || cs.PeekIsWhitespace())
                {
                    return new Token(TokenType.Integer, sb.ToString());
                }
                else
                {
                    if (cs.PeekIs('E') || cs.PeekIs('e') || cs.PeekIs('.'))
                    {
                        var matcher = new FloatMatcher(sb);
                        return matcher.Match(cs);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }
            else
            {
                return new Token?();
            }
        }
    }
}