﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loggly.Config;

namespace Loggly.Example
{
    class LogglyExample
    {
        readonly ILogglyClient _loggly = new LogglyClient();

        /// <summary>
        /// Calling result on the task forces it to be synchronous
        /// </summary>
        public ResponseCode SendPlainMessageSynchronous()
        {
            var logEvent = new LogglyEvent();
            logEvent.Data.Add("message", "Synchronous message at {0} using {1}", DateTime.Now, LogglyConfig.Instance.Transport.LogTransport);
            var r = _loggly.Log(logEvent).Result;
            return r.Code;
        }

        public void SendAsync()
        {
            var logEvent = new LogglyEvent();
            logEvent.Data.Add("message", "Asynchronous message at {0} using {1}", DateTime.Now, LogglyConfig.Instance.Transport.LogTransport);
            _loggly.Log(logEvent).ConfigureAwait(false);
        }

        public void SendWithAttributes()
        {
            var logEvent = new LogglyEvent();

            logEvent.Data.Add("message", "Message with attributes");
            logEvent.Data.Add("context", new LogObject());

            _loggly.Log(logEvent);
        }

        public async Task<LogResponse> SendCustomObjectAsync()
        {
            var logEvent = new LogglyEvent();
            logEvent.Data = new LogObject();
            return await _loggly.Log(logEvent).ConfigureAwait(false);
        }

        public async void SendWithSpecificTransport(LogTransport transport)
        {
            var priorTransport = LogglyConfig.Instance.Transport;

            var newTransport = new TransportConfiguration {LogTransport = transport};
            LogglyConfig.Instance.Transport = newTransport.GetCoercedToValidConfig();

            var logEvent = new LogglyEvent();
            logEvent.Data.Add("message", "Log event sent with forced transport={0}", transport);
            await _loggly.Log(logEvent);

            LogglyConfig.Instance.Transport = priorTransport;
        }
    }
}
