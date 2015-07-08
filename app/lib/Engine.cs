/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib;
using log4net;
using System.IO;
using System.Windows.Forms;
using thialgou.lib.model;
using thialgou.lib.CommonTypes;

namespace thialgou.appl.lib
{
    internal class Engine : _Engine
    {
        private static ILog LOG = LogManager.GetLogger(typeof(Engine));
        private const String MULI_INSTANCE_FILE = "./.multiinstance";
        private static Boolean sInitialized;
        static String sApplicationDataPath;

        protected Engine(MediaType mediaType, ContainerType inputFileContainerType, EncodingType inputFileEncondingType, String inputFilePath, String outputFilePath)
            : base(mediaType, inputFileContainerType, inputFileEncondingType, inputFilePath, outputFilePath)
        {
            if (!sInitialized)
            {
                // Native initialization here
                sInitialized = true;
            }
        }

        public static Engine CreateRawH264(String inputFilePath, String outputFilePath)
        {
            return new Engine(MediaType.Video, ContainerType.Raw, EncodingType.H264, inputFilePath, outputFilePath);
        }        

        public override Boolean MultiInstance
        {
            get
            {
                return System.IO.File.Exists(Engine.MULI_INSTANCE_FILE);
            }
        }

        static internal String ApplicationDataPath
        {
            get
            {
                if (sApplicationDataPath == null)
                {
                    if (System.IO.File.Exists(Engine.MULI_INSTANCE_FILE))
                    {
                        sApplicationDataPath = System.IO.Path.Combine(Application.StartupPath, "User");
                    }
                    else
                    {
                        String applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        sApplicationDataPath = Path.Combine(applicationData, "Doubango\\Thialgou");
                    }
                    Directory.CreateDirectory(sApplicationDataPath);
                }
                return sApplicationDataPath;
            }
        }
    }
}
