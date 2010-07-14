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


/* PandoraFS CLI - DS
 * Achim Friedland, 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;

using sones.Lib;
using sones.Lib.Frameworks.CLIrony.Compiler;
using sones.Lib.CLI;
using sones.Lib.DataStructures;
using sones.GraphFS.Session;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.GraphFS.Connectors.GraphFSCLI
{

    /// <summary>
    /// Shows the size of a disc
    /// </summary>

    public class FSCLI_DS : AllBasicFSCLICommands
    {

        #region Constructor

        public FSCLI_DS()
        {

            // Command name and description
            InitCommand("DS",
                        "Shows the size of a disc",
                        "Shows the size of a disc");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal | CLICommandSymbolTerminal + stringLiteralPVFS);

        }

        #endregion

        #region Execute Command

        public override void Execute(ref object myIGraphFSSession, ref object myIPandoraDBSession, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            _CancelCommand = false;
            var _IGraphFSSession = myIGraphFSSession as IGraphFSSession;

            if (_IGraphFSSession == null)
            {
                WriteLine("No file system mounted...");
                return;
            }

            WriteLine(Environment.NewLine + "Total disc size of the following devices:" + Environment.NewLine);

            var Size = _IGraphFSSession.GetNumberOfBytes(new ObjectLocation(FSPathConstants.PathDelimiter));
            WriteLine("{0,-12} {1,15} Bytes", FSPathConstants.PathDelimiter, Size.ToByteFormattedString(2));

            foreach (var _Mountpoint in _IGraphFSSession.GetChildFileSystemMountpoints(true))
            {
                Size = _IGraphFSSession.GetNumberOfBytes(new ObjectLocation(_Mountpoint));
                WriteLine("{0,-12} {1,15} ", _Mountpoint, Size.ToByteFormattedString(2));
            }

            WriteLine(Environment.NewLine);

        }

        #endregion

    }

}