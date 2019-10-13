using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;

//using NLog.Targets;


public interface ServiceEntry {
    //void Initial();
    void Execute(bool debug);
    void Control(bool running);
}

