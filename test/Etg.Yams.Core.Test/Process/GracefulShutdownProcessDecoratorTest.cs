﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Etg.Yams.Process;
using Etg.Yams.Process.Ipc;
using Etg.Yams.Test.stubs;
using Xunit;

namespace Etg.Yams.Test.Process
{
    public class GracefulShutdownProcessDecoratorTest
    {
        [Fact]
        public async Task TestThatExitedEventIsNotFiredOnGracefulShutdown()
        {
            var yamsConfig = new YamsConfigBuilder("clusterId", "1", "instanceId", "C:\\Foo").Build();
            var process = new StubIProcess().HasExited_Get(() => true);
            IIpcConnection connection = new StubIIpcConnection()
                .SendMessage(message =>
                {
                    process.Exited_Raise(process, new ProcessExitedArgs(process, ""));
                    return Task.CompletedTask;
                })
                .Disconnect(() => Task.CompletedTask);
            GracefulShutdownProcessDecorator decorator = new GracefulShutdownProcessDecorator(yamsConfig, process, connection);
            bool hasExitedFired = false;
            decorator.Exited += (sender, args) =>
            {
                hasExitedFired = true;
            };

            await decorator.Close();
            Assert.False(hasExitedFired);
        }
    }
}
