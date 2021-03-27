using FluentNHibernate.Cfg;
using log4net;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WebApplication.Session
{
    /// <summary>
    /// SessionProvider NHibernate Helper
    /// </summary>
    public static class SessionProvider 
    {
        #region Constructors

        static SessionProvider()
        {
            ISessionFactory = CreateSessionFactory();
        }

    
        #endregion

        #region Public Properties

        public static string DefaultHibernateCfgFileName { get; set; } = "hibernate.cfg.xml";


        public static ISessionFactory ISessionFactory { get; private set; }


        #endregion

        #region Private Methods

        private static ISessionFactory CreateSessionFactory()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo("log4net.config"));

            ISessionFactory iSessionFactory = null;
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
 
                Configuration Configuration = new Configuration();

                Configuration.BeforeBindMapping += OnBeforeBindMapping;

                Configuration.Configure(GetDefaultConfigurationFilePath());

                //FluentNHibernate Configuration API for configuring NHibernate
                Configuration = Fluently.Configure(Configuration)
                                .Mappings(model =>
                                {
                                    model.FluentMappings
                                                 .AddFromAssembly(typeof(SessionProvider).Assembly);
                                    
                                    //model.FluentMappings.Add<MemberConditionFilter>();
                                })
                            .BuildConfiguration();

                iSessionFactory = Configuration.BuildSessionFactory();

               
                stopwatch.Stop();
            }
            catch(Exception ex)
            {
                throw;
            }

            return iSessionFactory;
        }

        private static void OnBeforeBindMapping(object sender, BindMappingEventArgs bindMappingEventArgs)
        {
            // Force using the fully qualified type name instead of just the class name.
            // This will get rid of any duplicate mapping/class name issues.
            bindMappingEventArgs.Mapping.autoimport = false;
        }

        public static string GetExecutionPath()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Note RelativeSearchPath can be null even if the doc say something else; don't remove the check
            var searchPath = AppDomain.CurrentDomain.RelativeSearchPath ?? string.Empty;

            string relativeSearchPath = searchPath.Split(';').First();
            return Path.Combine(baseDirectory, relativeSearchPath);
        }

        private static string GetDefaultConfigurationFilePath()
        {
            return Path.Combine(GetExecutionPath(), DefaultHibernateCfgFileName);
        }

        #endregion Private Methods
    }
}
