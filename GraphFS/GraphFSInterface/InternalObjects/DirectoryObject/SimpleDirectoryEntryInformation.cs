﻿/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* PandoraFS - SimpleDirectoryEntryInformation
 * (c) Daniel Kirstenpfad, 2009
 *  
 * Actually only used to get rid of eventually existing Non Serializable data structures
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

#region Usings

using System;
using System.Collections; 
using System.Collections.Generic;


#endregion

namespace sones.GraphFS.InternalObjects
{
  
    /// <summary>
    /// An directory entry information is an object which holds all
    /// information on an object within an extended directory listing.
    /// </summary>

    
    public struct SimpleDirectoryEntryInformation
    {

        public String                   Name;
        public Int64                    Timestamp;
        public UInt64                   Size;
        public List<String>             StreamTypes;

    }

}