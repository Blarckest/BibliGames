﻿using Modele;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DataManager
{
    public abstract class Saver
    {
        protected string Folder;
        protected Saver(string folder)
        {
            this.Folder = folder;
            Directory.CreateDirectory(folder);
        }
        public abstract void Save(Data manager);
        public abstract void Save(IList<Element> elements, IList<string> additionalFolder);
    }
}
