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


/* <id name="sones GraphDB – Version Information" />
 * <copyright file="Version.cs"
 *            company="sones GmbH">
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary></summary>
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace sones.GraphDB
{
    /// <summary>
    /// The version information of this Pandora Database build
    /// </summary>
    public static class Version
    {
        public static UInt16 VersionMajor = 4;
        public static UInt16 VersionMinor = 1;
        public static UInt16 BuildNumber = 1;

        public static String VersionString
        { 
            get
            {
                return "PandoraDatabase Revision " + VersionMajor + "." + VersionMinor + " (Build " + BuildNumber + ")";
            }
        }
    }

}