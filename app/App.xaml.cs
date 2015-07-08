/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using log4net;
using System.IO;
using log4net.Config;
using System.Windows.Markup;
using System.Globalization;
using thialgou.lib;
using thialgou.appl.lib;

namespace thialgou.appl
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ILog LOG;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                String log4net = String.Format(thialgou.appl.Properties.Resources.log4net_xml, Engine.ApplicationDataPath);
                using (Stream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(log4net)))
                {
                    XmlConfigurator.Configure(stream);
                    LOG = LogManager.GetLogger(typeof(App));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.Shutdown();
            }

            LOG.Debug("====================================================");
            LOG.Debug(String.Format("======Starting Thialgou v{0} ====", System.Reflection.Assembly.GetEntryAssembly().GetName().Version));
            LOG.Debug("====================================================");

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            try
            {
                /* Register H.264 main instance */
                org.doubango.thialgou.commonWRAP.CommonMain.setMainH264(thialgou.lib.h264.MainInstH264.GetInst(this.Dispatcher));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //Engine.Stop();
        }
    }
}
