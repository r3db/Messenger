using System;
using System.Collections.Generic;
using System.Text;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class UserTypingMessageResponse : AbstractMessageResponse
    {
        public int Length { get; private set; }
        public string TypingUser { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; }

        public UserTypingMessageResponse(Version mimeTypeVersion, string contentType, string typingUser, Dictionary<string, string> parameters, int length)
            : base(mimeTypeVersion, contentType)
        {
            parameters.NotNull();

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", "less than 0");
            }

            typingUser.NotEmpty();

            this.Parameters = parameters;
            this.Length = length;
            this.TypingUser = typingUser;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in this.Parameters)
            {
                sb.AppendFormat("{0}: {1}{2}", kvp.Key, kvp.Value, Environment.NewLine);
            }
            if (sb.Length != 0)
            {
                sb.Remove(sb.Length - 2, 2);
            }

            return string.Format("User {0} is typing...", this.TypingUser);
            //return string.Format("MSG Hotmail Hotmail {0}{1}{2}{3}{4}", Length, Environment.NewLine, base.ToString(), Environment.NewLine, sb);
        }
    }
}