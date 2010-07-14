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


/* <id Name="sones GraphDB – DBBackwardEdge" />
 * <copyright file="DBBackwardEdge.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>DBBackwardEdge contains all BackwardEdgdes of an particular DBObject.<summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.ObjectManagement
{

    public class BackwardEdgeStream : ADictionaryObject<EdgeKey, EdgeTypeSetOfReferences>
    {

        #region CTors

        public BackwardEdgeStream()
        {

            // Members of APandoraStructure
            _StructureVersion = 1;

            // Members of APandoraObject
            _ObjectStream = DBConstants.DBBACKWARDEDGESTREAM;

            // Object specific data...

            // Set ObjectUUID
            if (ObjectUUID.Length == 0)
                ObjectUUID = base.ObjectUUID;
        }

        public BackwardEdgeStream(ObjectLocation myObjectLocation)
            : this()
        {

            if (myObjectLocation == null || myObjectLocation.Length < FSPathConstants.PathDelimiter.Length)
                throw new ArgumentNullException("Invalid ObjectLocation!");

            ObjectLocation = myObjectLocation;

        }

        #endregion

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new BackwardEdgeStream();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion
        
        #region (de)serialize overrides

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            base.Serialize(ref mySerializationWriter);
        }

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {
            base.Deserialize(ref mySerializationReader);
        }

        #endregion

        public Boolean ContainsBackwardEdge(EdgeKey myEdgeKey)
        {
            return base.ContainsKey(myEdgeKey);
        }

        /// <summary>
        /// Adds a BackwardEdge without flushing them!!!
        /// </summary>
        /// <param name="myEdgeKey"></param>
        /// <param name="myObjectUUID"></param>
        public void AddBackwardEdge(EdgeKey myEdgeKey, ObjectUUID myObjectUUID, DBObjectManager objectManager)
        {
            if (!base.ContainsKey(myEdgeKey))
            {

                base.Add(myEdgeKey, new EdgeTypeSetOfReferences());
            }

            base[myEdgeKey].Add(myObjectUUID);

            isDirty = true;
        }

        /// <summary>
        /// Adds some BackwardEdges without flushing them!!!
        /// </summary>
        /// <param name="myEdgeKey"></param>
        /// <param name="myObjectUUID"></param>
        public void AddBackwardEdge(EdgeKey myEdgeKey, IEnumerable<ObjectUUID> myObjectUUIDs, DBObjectManager objectManager)
        {
            if (!base.ContainsKey(myEdgeKey))
            {
                base.Add(myEdgeKey, new EdgeTypeSetOfReferences(myObjectUUIDs));
            }
            else
            {
                base[myEdgeKey].AddRange(myObjectUUIDs);
            }

            isDirty = true;
        }

        /// <summary>
        /// Removes a BackwardEdge without flushing them!!!
        /// </summary>
        /// <param name="myEdgeKey"></param>
        /// <param name="myObjectUUID"></param>
        /// <returns></returns>
        public Boolean RemoveBackwardEdge(EdgeKey myEdgeKey, ObjectUUID myObjectUUID)
        {

            if (!base.ContainsKey(myEdgeKey) || !base[myEdgeKey].RemoveUUID(myObjectUUID))
                return false;

            if (base[myEdgeKey].Count() == 0)
            {
                if (base.Remove(myEdgeKey) == false)
                    return false;
            }

            isDirty = true;

            return true;

        }

        /// <summary>
        /// Returns all BackwardEdges for a particular Type und Attribute.
        /// Throws KeyNotFoundException for not found Type und Attribute combination
        /// </summary>
        /// <param name="myEdgeKey"></param>
        /// <returns></returns>
        public IEnumerable<ObjectUUID> GetBackwardEdgeUUIDs(EdgeKey myEdgeKey)
        {
            return base[myEdgeKey].GetAllUUIDs();
        }

        public EdgeTypeSetOfReferences GetBackwardEdges(EdgeKey myEdgeKey)
        {
            return base[myEdgeKey];
        }

        #region IEnumerable<KeyValuePair<EdgeKey,EdgeTypeSetOfReferences>> Members

        public new IEnumerator<KeyValuePair<EdgeKey, EdgeTypeSetOfReferences>> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        #endregion


    }
}