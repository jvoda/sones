﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class UniqueID
    {
        private long _id = Int64.MinValue;

        public long GetNextID()
        {
            long result = Interlocked.Increment(ref _id);
            return result;
        }
    }
}