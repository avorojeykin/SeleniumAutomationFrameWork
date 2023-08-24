using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface IConfigurationProcessor
    {
        ProcessResponse Response { get; set; }
        void Run(string _fileDirectory);
    }
}
