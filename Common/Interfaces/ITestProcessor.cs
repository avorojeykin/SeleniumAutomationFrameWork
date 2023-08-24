using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface ITestProcessor
    {
        ProcessResponse Response { get; set; }
        void Run();
    }
}
