﻿using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using Xunit;

namespace Utility.OperationResult.Tests
{
    /// <summary>
    ///     Tests the <see cref="Result"/> class.
    /// </summary>
    public class ResultTests
    {
        /// <summary>
        ///     The xLogger instance to use for the tests.
        /// </summary>
        private static Logger logger = (Logger)LogManager.GetCurrentClassLogger(typeof(Logger));

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResultTests"/> class.
        /// </summary>
        public ResultTests()
        {
            // configure the logger with a debugger target
            LoggingConfiguration config = new LoggingConfiguration();
            DebuggerTarget debug = new DebuggerTarget();
            config.AddTarget("debug", debug);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, debug));
        }

        /// <summary>
        ///     Tests the constructor of <see cref="Result"/>.
        /// </summary>
        [Fact]
        public void Constructor()
        {
            Result testblank = new Result();

            Assert.Equal(ResultCode.Success, testblank.ResultCode);

            Result test = new Result(ResultCode.Warning);

            Assert.Equal(ResultCode.Warning, test.ResultCode);
        }

        /// <summary>
        ///     Tests the implicit boolean operator for <see cref="Result"/>.
        /// </summary>
        [Fact]
        public void ImplicitOperator()
        {
            Result test = new Result();

            Assert.Equal(true, test);

            Assert.Equal(test, test.AddError("test"));

            Assert.Equal(false, test);
        }

        /// <summary>
        ///     Tests <see cref="Result.AddInfo(string)"/>.
        /// </summary>
        [Fact]
        public void AddInfo()
        {
            Result test = new Result();

            Assert.Equal(test, test.AddInfo("test"));

            Assert.Equal("test", test.GetLastInfo());
            Assert.Equal(1, test.Messages.Count);
            Assert.Equal(MessageType.Info, test.Messages[0].Type);
            Assert.Equal(ResultCode.Success, test.ResultCode);
        }

        /// <summary>
        ///     Tests <see cref="Result.AddWarning(string)"/>.
        /// </summary>
        [Fact]
        public void AddWarning()
        {
            Result test = new Result();

            Assert.Equal(test, test.AddWarning("test"));

            Assert.Equal("test", test.GetLastWarning());
            Assert.Equal(1, test.Messages.Count);
            Assert.Equal(MessageType.Warning, test.Messages[0].Type);
            Assert.Equal(ResultCode.Warning, test.ResultCode);
        }

        /// <summary>
        ///     Tests <see cref="Result.AddError(string)"/>.
        /// </summary>
        [Fact]
        public void AddError()
        {
            Result test = new Result();

            Assert.Equal(test, test.AddError("test"));

            Assert.Equal("test", test.GetLastError());
            Assert.Equal(1, test.Messages.Count);
            Assert.Equal(MessageType.Error, test.Messages[0].Type);
            Assert.Equal(ResultCode.Failure, test.ResultCode);
        }

        /// <summary>
        ///     Tests <see cref="Result.RemoveMessages(MessageType)"/>.
        /// </summary>
        [Fact]
        public void RemoveMessages()
        {
            Result test = new Result();

            test.AddError("one");
            test.AddError("two");

            Assert.Equal(2, test.Messages.Count);

            Assert.Equal(test, test.RemoveMessages(MessageType.Any));

            Assert.Equal(0, test.Messages.Count);

            test.AddError("three");
            test.AddWarning("four");
            test.AddInfo("five");

            Assert.Equal(3, test.Messages.Count);

            Assert.Equal(test, test.RemoveMessages(MessageType.Warning));

            Assert.Equal(2, test.Messages.Count);
            Assert.Empty(test.Messages.Where(t => t.Type == MessageType.Warning));
        }

        /// <summary>
        ///     Tests <see cref="Result.SetResultCode(ResultCode)"/>.
        /// </summary>
        [Fact]
        public void SetResultCode()
        {
            Result test = new Result();

            Assert.Equal(ResultCode.Success, test.ResultCode);

            Assert.Equal(test, test.SetResultCode(ResultCode.Failure));

            Assert.Equal(ResultCode.Failure, test.ResultCode);
        }

        /// <summary>
        ///     Tests <see cref="Result.LogResult(Logger, string)"/> overloads.
        /// </summary>
        [Fact]
        public void LogResult()
        {
            Result test = new Result();

            Assert.Equal(test, test.LogResult(logger));
            Assert.Equal(test, test.LogResult(logger.Info));

            test.AddInfo("test");
            test.AddWarning("test");

            Assert.Equal(test, test.LogResult(logger));

            test.AddError("test");

            Assert.Equal(test, test.LogResult(logger));
        }

        /// <summary>
        ///     Tests
        ///     <see cref="Result.LogAllMessages(System.Action{string}, MessageType, string, string)"/> overloads.
        /// </summary>
        [Fact]
        public void LogAllMessages()
        {
            Result test = new Result();
            test.AddInfo("test");

            Assert.Equal(test, test.LogAllMessages(logger.Info, "header", "footer"));

            Assert.Equal(test, test.LogAllMessages(logger.Info, MessageType.Info));
        }

        /// <summary>
        ///     Tests <see cref="Result.Incorporate(Result)"/>.
        /// </summary>
        [Fact]
        public void Incorporate()
        {
            Result test = new Result();
            test.AddError("test");

            Result testTwo = new Result();

            testTwo.Incorporate(test);

            Assert.Equal(test.ResultCode, testTwo.ResultCode);
            Assert.Equal(test.Messages, testTwo.Messages);
        }

        [Fact]
        public void GetLastNoMessages()
        {
            Result test = new Result();

            string error = test.GetLastError();
            string warning = test.GetLastWarning();
            string info = test.GetLastInfo();
        }
    }
}