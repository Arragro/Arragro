using Arragro.Common.Logging;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Log4Net.Tests
{
    class Program
    {
        static void RaiseError()
        {
            var log = LogManager.GetLogger("Test Logger");
            log.Error(new Exception("Test Error").ToString());
        }

        static void Main(string[] args)
        {
            var appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            LogManager.LogFactory = new Arragro.Log4Net.Log4NetFactory(false);

            var hierarchy = (Hierarchy)log4net.LogManager.GetRepository();
            
            var memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);
            Log4NetBuilder.Setup(appPath);

            var log = LogManager.GetLogger("Test Logger");

            log.Debug("Test 1");
            log.Debug("Test 2");

            log.Info("Test 1");
            log.Info("Test 2");

            log.Error(new Exception("Test Error").ToString());
            RaiseError();

            //Console.WriteLine(JsonConvert.SerializeObject(memory.GetEvents()));

            Console.ReadKey();
        }
    }
}
