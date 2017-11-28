using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Contracts;

namespace Messenger.API.Parser
{
    public sealed class CommandParser
    {
        private readonly StringReader reader;
        public int Position { get; private set; }
        public int Length { get; private set; }

        public CommandParser(string s)
        {
            s.NotEmpty();

            this.reader = new StringReader(s);
            this.Position = 0;
            this.Length = s.Length;
        }

        #region Properties

        public bool EndOfStream
        {
            get { return this.Position + 1 >= this.Length; }
        }

        #endregion

        public void MoveNext(int n)
        {
            this.ReadBlock(n);
            this.Position += n;
        }

        public void MoveNext()
        {
            this.MoveNext(1);
        }

        #region Read Methods

        public string ReadBlock(int n)
        { 
            char[] buffer = new char[n];
            this.reader.ReadBlock(buffer, 0, n);
            this.Position += n;
            return new string(buffer);
        }

        public T ReadNumericType<T>(bool ignoreWhiteSpaces, Func<string, T> callback)
        {
            if (ignoreWhiteSpaces)
            {
                while (char.IsWhiteSpace((char)this.reader.Peek()))
                {
                    this.reader.Read();
                    this.Position++;
                }
            }

            StringBuilder sb = new StringBuilder();

            while (this.reader.Peek() >= 48 && this.reader.Peek() <= 57)
            {
                sb.Append((char)this.reader.Read());
                this.Position++;
            }

            return callback(sb.ToString());
        }

        public T ReadNumericType<T>(Func<string, T> callback)
        {
            return this.ReadNumericType<T>(true, callback);
        }

        #region Read Integers

        public int ReadInt32()
        { 
            return this.ReadNumericType(c => int.Parse(c));
        }

        public uint ReadIntU32()
        {
            return this.ReadNumericType(c => uint.Parse(c));
        }

        public long ReadInt64()
        {
            return this.ReadNumericType(c => long.Parse(c));
        }

        public ulong ReadUInt64()
        {
            return this.ReadNumericType(c => ulong.Parse(c));
        }

        #endregion

        public string ReadToken(bool stopOnWhiteSpaces)
        {
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                char c = (char)this.reader.Peek();
                if ((int)c == -1 || char.IsControl(c) || (stopOnWhiteSpaces && /*char.IsWhiteSpace(c)*/ c == (char)32))
                {
                    break;
                }

                sb.Append((char)this.reader.Read());
                this.Position++;
            }

            return sb.ToString();

        }

        public string ReadToken()
        {
            return this.ReadToken(true);
        }

        public IEnumerable<string> ReadTokens(bool stopOnWhiteSpaces)
        {
            List<string> tokens = new List<string>(22);
            while (this.EndOfStream == false)
            {
                string value = this.ReadToken(stopOnWhiteSpaces);

                if (char.IsControl((char)this.reader.Peek()))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        tokens.Add(value);
                    }
                    break;
                }
                if (!string.IsNullOrEmpty(value))
                {
                    tokens.Add(value);
                }
                if (stopOnWhiteSpaces)
                {
                    while (!char.IsControl((char)this.reader.Peek()) && /*char.IsWhiteSpace((char)reader.Peek())*/ this.reader.Peek() == 32)
                    {
                        this.reader.Read();
                        this.Position++;
                    }
                }
            }

            return tokens;

        }

        public IEnumerable<string> ReadTokens()
        {
            return this.ReadTokens(true);
        }

        #endregion

    }
}