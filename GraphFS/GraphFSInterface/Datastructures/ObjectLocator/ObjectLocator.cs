/*
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


/*
 * GraphFSInterface - ObjectLocator
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;

using sones.Lib;
using sones.Lib.Serializer;
using sones.Lib.DataStructures;
using sones.Lib.XML;
using sones.Lib.NewFastSerializer;
using sones.StorageEngines;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Exceptions;
using sones.GraphFS.Objects;
using sones.GraphFS.InternalObjects;
using sones.Lib;

#endregion

namespace sones.GraphFS.DataStructures
{

    /// <summary>
    /// The ObjectLocator describes where to find the different streams,
    /// editions, revisions and copies of an object.
    /// </summary>

    public class ObjectLocator : AFSStructure, IGraphFSDictionary<String, ObjectStream>, IDirectoryListing, IXMLExport
    {


        #region Data

        private Dictionary<String, ObjectStream> _ObjectStreams;

        #endregion

        #region Constructors

        #region ObjectLocator()

        /// <summary>
        /// This will create an empty ObjectLocator
        /// </summary>
        public ObjectLocator()
        {

            // Members of APandoraStructure
            _StructureVersion           = 1;

            // ObjectLocator specific Data
            _ObjectStreams          = new Dictionary<String, ObjectStream>();

        }

        #endregion

        #region ObjectLocator(myObjectLocation, myINode)

        /// <summary>
        /// This will create an empty ObjectLocator based on the information within the INode
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        /// <param name="myINode">The associated INode</param>
        public ObjectLocator(ObjectLocation myObjectLocation, INode myINode)
            : this()
        {

            ObjectUUID                           = myINode.ObjectUUID;
            _INodeReference                      = myINode;
            _ObjectLocation                      = myObjectLocation;
            _ObjectPath                          = _ObjectLocation.Path;
            _ObjectName                          = _ObjectLocation.Name;
            
        }

        #endregion

        #region ObjectLocator(myObjectLocation, myObjectUUID)

        /// <summary>
        /// This will create an empty ObjectLocator based on the information within the INode
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        /// <param name="myINode">The associated INode</param>
        public ObjectLocator(ObjectLocation myObjectLocation, ObjectUUID myObjectUUID)
            : this()
        {

            ObjectUUID                           = myObjectUUID;
            _INodeReference                      = null;
            _ObjectLocation                      = myObjectLocation;
            _ObjectPath                          = _ObjectLocation.Path;
            _ObjectName                          = _ObjectLocation.Name;
            
        }

        #endregion

        #region ObjectLocator(myObjectLocation, myINode, myObjectStream, myObjectType, MinNumberOfRevisions, MaxNumberOfRevisions, myMinRevisionDelta, MaxRevisionAge, MinNumberOfCopies, MaxNumberOfCopies)

        /// <summary>
        /// This will create an object locator locating the given object streams at the
        /// actual revision time.
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        /// <param name="myINode">The associated INode</param>
        /// <param name="myObjectStream">the Name of the object stream</param>
        /// <param name="myObjectType">the type of the object stream</param>
        /// <param name="myObjectEdition">the edition Name of the stream</param>
        /// <param name="MinNumberOfRevisions">minimum number of object revisions</param>
        /// <param name="MaxNumberOfRevisions">maximum number of object revisions</param>
        /// <param name="myMinRevisionDelta">minimum timespan between two revisions</param>
        /// <param name="myMaxRevisionAge">maximum timespan to keep old revisions</param>
        /// <param name="MinNumberOfCopies">minimum number of object copies</param>
        /// <param name="MaxNumberOfCopies">maximum number of object copies</param>
        public ObjectLocator(ObjectLocation myObjectLocation, INode myINode, String myObjectStream, String myObjectEdition, UInt64 myMinNumberOfRevisions, UInt64 myMaxNumberOfRevisions, UInt64 myMinRevisionDelta, UInt64 myMaxRevisionAge, UInt64 myMinNumberOfCopies, UInt64 myMaxNumberOfCopies)
            : this (myObjectLocation, myINode)
        {

            _ObjectStreams          = new Dictionary<String, ObjectStream>();

            var _ObjectRevision     = new ObjectEdition()
                                        {
                                            MinNumberOfRevisions    = myMinNumberOfRevisions,
                                            MaxNumberOfRevisions    = myMaxNumberOfRevisions,
                                            MinRevisionDelta        = myMinRevisionDelta,
                                            MaxRevisionAge          = myMaxRevisionAge
                                        };

            var _ObjectEdition      = new ObjectStream(myObjectStream, myObjectEdition, _ObjectRevision);
            
            _ObjectStreams.Add(myObjectStream, _ObjectEdition);

        }

        #endregion

        #region ObjectLocator(myObjectLocation, myINode, myObjectStream, MinNumberOfRevisions, MaxNumberOfRevisions, myMinRevisionDelta, myMaxRevisionAge, MinNumberOfCopies, MaxNumberOfCopies, myListOfObjectStreams)

        /// <summary>
        /// This will create an object locator locating the given object streams at the
        /// actual revision time.
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        /// <param name="myINode">The associated INode</param>
        /// <param name="myObjectStream">the Name of the object stream</param>
        /// <param name="myObjectType">the type of the object stream</param>
        /// <param name="myObjectEdition">the edition Name of the stream</param>
        /// <param name="myForestUUID">the UUID of the ForestDirectory to make RevisionID unique</param>
        /// <param name="MinNumberOfRevisions">minimum number of object revisions</param>
        /// <param name="MaxNumberOfRevisions">maximum number of object revisions</param>
        /// <param name="myMinRevisionDelta">minimum timespan between two revisions</param>
        /// <param name="myMaxRevisionAge">maximum timespan to keep old revisions</param>
        /// <param name="MinNumberOfCopies">minimum number of object copies</param>
        /// <param name="MaxNumberOfCopies">maximum number of object copies</param>
        /// <param name="myListOfObjectStreams">a list of object streams for storing the given object</param>
        public ObjectLocator(ObjectLocation myObjectLocation, INode myINode, String myObjectStream, String myObjectEdition, UUID myForestUUID, UInt64 myMinNumberOfRevisions, UInt64 myMaxNumberOfRevisions, UInt64 myMinRevisionDelta, UInt64 myMaxRevisionAge, UInt64 myMinNumberOfCopies, UInt64 myMaxNumberOfCopies, List<ObjectDatastream> myListOfObjectStreams)
            : this (myObjectLocation, myINode)
        {

            _ObjectStreams   = new Dictionary<String, ObjectStream>();

            var _ObjectRevision = new ObjectEdition()
                                    {
                                        MinNumberOfRevisions    = myMinNumberOfRevisions,
                                        MaxNumberOfRevisions    = myMaxNumberOfRevisions,
                                        MinRevisionDelta        = myMinRevisionDelta,
                                        MaxRevisionAge          = myMaxRevisionAge
                                    };

            var _ObjectCopy     = new ObjectRevision(myObjectStream)
                                    {
                                        MinNumberOfCopies       = myMinNumberOfCopies,
                                        MaxNumberOfCopies       = myMaxNumberOfCopies
                                    };

            _ObjectCopy.Set(myListOfObjectStreams);
            //HACK: ParentRevisions set to 0!
            var tmpID = new RevisionID((UInt64) myINode.ModificationTime.Ticks, myForestUUID);
            _ObjectRevision.Add(tmpID , _ObjectCopy);

            var _ObjectEdition   = new ObjectStream(myObjectStream, myObjectEdition, _ObjectRevision);

            _ObjectStreams.Add(myObjectStream, _ObjectEdition); 

        }

        #endregion

        #region ObjectLocator(myObjectLocation, myINode, myObjectStream, myObjectType, myRevisionID, MinNumberOfRevisions, MaxNumberOfRevisions, myMinRevisionDelta, myMaxRevisionAge, MinNumberOfCopies, MaxNumberOfCopies, myListOfObjectStreams)

        /// <summary>
        /// This will create an object locator locating the given object streams at the
        /// actual revision time.
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        /// <param name="myINode">The associated INode</param>
        /// <param name="myObjectStream">the Name of the object stream</param>
        /// <param name="myObjectType">the type of the object stream</param>
        /// <param name="myObjectEdition">the object edition Name</param>
        /// <param name="myRevisionTimestamp"></param>
        /// <param name="myForestUUID">the UUID of the ForestDirectory to make RevisionID unique</param>
        /// <param name="MinNumberOfRevisions">minimum number of object revisions</param>
        /// <param name="MaxNumberOfRevisions">maximum number of object revisions</param>
        /// <param name="myMinRevisionDelta">minimum timespan between two revisions</param>
        /// <param name="myMaxRevisionAge">maximum timespan to keep old revisions</param>
        /// <param name="MinNumberOfCopies">minimum number of object copies</param>
        /// <param name="MaxNumberOfCopies">maximum number of object copies</param>
        /// <param name="myListOfObjectStreams">a list of object streams for storing the given object</param>
        public ObjectLocator(ObjectLocation myObjectLocation, INode myINode, String myObjectStream, String myObjectEdition, UInt64 myRevisionTimestamp, UUID myForestUUID, UInt64 myMinNumberOfRevisions, UInt64 myMaxNumberOfRevisions, UInt64 myMinRevisionDelta, UInt64 myMaxRevisionAge, UInt64 myMinNumberOfCopies, UInt64 myMaxNumberOfCopies, List<ObjectDatastream> myListOfObjectStreams)
            : this (myObjectLocation, myINode)
        {

                _ObjectStreams   = new Dictionary<String, ObjectStream>();

            var _ObjectRevision = new ObjectEdition()
                                    {
                                        MinNumberOfRevisions    = myMinNumberOfRevisions,
                                        MaxNumberOfRevisions    = myMaxNumberOfRevisions,
                                        MinRevisionDelta        = myMinRevisionDelta,
                                        MaxRevisionAge          = myMaxRevisionAge
                                    };

            var _ObjectCopy     = new ObjectRevision(myObjectStream)
                                    {
                                        MinNumberOfCopies       = myMinNumberOfCopies,
                                        MaxNumberOfCopies       = myMaxNumberOfCopies
                                    };


            _ObjectCopy.Set(myListOfObjectStreams);
            //HACK: ParentRevisions set to 0!
            var tmpID = new RevisionID((UInt64) myINode.ModificationTime.Ticks, myForestUUID);
            _ObjectRevision.Add(tmpID , _ObjectCopy);

            var _ObjectEdition   = new ObjectStream(myObjectStream, myObjectEdition, _ObjectRevision);

            _ObjectStreams.Add(myObjectStream, _ObjectEdition); 


        }

        #endregion

        #region ObjectLocator(myObjectLocation, mySerializedData)

        /// <summary>
        /// A constructor of the ObjectLocator used for fast deserializing
        /// </summary>
        /// <param name="myObjectLocation">the location of this object (ObjectPath and ObjectName) of the requested file within the file system</param>
        /// <param name="mySerializedData">An array of bytes containing a serialized ObjectLocator</param>
        public ObjectLocator(ObjectLocation myObjectLocation, Byte[] mySerializedData)
        {
            Deserialize(mySerializedData, null, null);
            ObjectLocation = myObjectLocation;
            _isNew = false;
        }

        #endregion

        #endregion


        #region APandoraObject Members

        #region SerializeInnerObject(ref mySerializationWriter)

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {

            try
            {

                #region Write ObjectStreams

                // Write the number of ObjectStreams
                mySerializationWriter.WriteObject((UInt32) _ObjectStreams.Count);

                foreach (var __KV_String_ObjectStream in _ObjectStreams)
                {

                    // Write the name of the ObjectStream
                    mySerializationWriter.WriteObject(__KV_String_ObjectStream.Key);

                    var ___ObjectStream = __KV_String_ObjectStream.Value;
                    
                    #region Write ObjectEditions

                    // Write number of ObjectEditions
                    mySerializationWriter.WriteObject((UInt32)___ObjectStream.Count);

                    // Write name of the DefaultEdition
                    mySerializationWriter.WriteObject(___ObjectStream.DefaultEditionName);

                    foreach (var __KV_String_ObjectEdition in ___ObjectStream)
                    {
                        // Write ObjectEditions myLogin
                        mySerializationWriter.WriteObject(__KV_String_ObjectEdition.Key);

                        // Write IsDeleted
                        mySerializationWriter.WriteObject(__KV_String_ObjectEdition.Value.IsDeleted);

                        mySerializationWriter.WriteObject((UInt64)__KV_String_ObjectEdition.Value.MinNumberOfRevisions);
                        mySerializationWriter.WriteObject((UInt64)__KV_String_ObjectEdition.Value.MaxNumberOfRevisions);
                        mySerializationWriter.WriteObject((UInt64)__KV_String_ObjectEdition.Value.MinRevisionDelta);
                        mySerializationWriter.WriteObject((UInt64)__KV_String_ObjectEdition.Value.MaxRevisionAge);

                        // Write number of ObjectRevisions
                        mySerializationWriter.WriteObject((UInt32)__KV_String_ObjectEdition.Value.Count);
                        
                        #region ObjectRevisions

                        foreach (var __KV_RevisionID_ObjectRevision in __KV_String_ObjectEdition.Value)
                        {

                            #region Write RevisionID

                            mySerializationWriter.WriteObject((UInt64)__KV_RevisionID_ObjectRevision.Key.Timestamp);
                            __KV_RevisionID_ObjectRevision.Key.UUID.Serialize(ref mySerializationWriter);

                            #endregion

                            #region Write Parent revisions

                            var ___ParentRevisionIDs = __KV_RevisionID_ObjectRevision.Value.ParentRevisionIDs;

                            // Write number of Parent revisions
                            mySerializationWriter.WriteObject((UInt16) ___ParentRevisionIDs.Count);

                            foreach (var parentRevision in ___ParentRevisionIDs)
                            {
                                mySerializationWriter.WriteObject((UInt64) parentRevision.Timestamp);
                                parentRevision.UUID.Serialize(ref mySerializationWriter);
                            }

                            #endregion

                            // Write Max, Min NumberOfCopies
                            mySerializationWriter.WriteObject((UInt64) __KV_RevisionID_ObjectRevision.Value.MaxNumberOfCopies);
                            mySerializationWriter.WriteObject((UInt64) __KV_RevisionID_ObjectRevision.Value.MinNumberOfCopies);

                            #region Write ObjectDatastream

                            // Write number of ObjectCopies
                            mySerializationWriter.WriteObject((UInt32) __KV_RevisionID_ObjectRevision.Value.Count);

                            for (var __ObjectCopyCounter = 0; __ObjectCopyCounter < __KV_RevisionID_ObjectRevision.Value.Count; __ObjectCopyCounter++)
                            {

                                var ObjectStream = __KV_RevisionID_ObjectRevision.Value[__ObjectCopyCounter];

                                mySerializationWriter.WriteObject((UInt16) ObjectStream.Compression.Algorithm);
                                mySerializationWriter.WriteObject((UInt16) ObjectStream.ForwardErrorCorrection.Algorithm);
                                mySerializationWriter.WriteObject(ObjectStream.IntegrityCheckValue);
                                //ObjectStream.ObjectUUID.Serialize(ref mySerializationWriter);
                                mySerializationWriter.WriteObject((UInt16) ObjectStream.Redundancy.Algorithm);
                                mySerializationWriter.WriteObject(ObjectStream.ReservedLength);
                                mySerializationWriter.WriteObject(ObjectStream.Blocksize);

                                #region AvailableStorageIDs

                                mySerializationWriter.WriteObject((UInt16)ObjectStream.AvailableStorageUUIDs.Count);

                                foreach (var StorageUUID in ObjectStream.AvailableStorageUUIDs)
                                    StorageUUID.Serialize(ref mySerializationWriter);

                                #endregion

                                // TODO: Need the ObjectStream.BlockIntegrityArrays need to be Serialized?
                                //mySerializationWriter.WriteObject(ObjectStream.BlockIntegrityArrays.Serialize());

                                #region Write ObjectExtent

                                // Write number of ObjectExtents
                                mySerializationWriter.WriteObject((UInt32) ObjectStream.Count);

                                for (var actObjectExtent = 0UL; actObjectExtent < ObjectStream.Count; actObjectExtent++)
                                {
                                    var ObjectExtent = ObjectStream[actObjectExtent];
                                    mySerializationWriter.WriteObject(ObjectExtent.Length);
                                    mySerializationWriter.WriteObject(ObjectExtent.LogicalPosition);
                                    mySerializationWriter.WriteObject(ObjectExtent.StorageUUID);
                                    //ObjectExtent.StorageUUID.Serialize(ref mySerializationWriter);
                                    mySerializationWriter.WriteObject(ObjectExtent.PhysicalPosition);
                                    mySerializationWriter.WriteObject(ObjectExtent.NextExtent.StorageUUID);
                                    //ObjectExtent.NextExtent.StorageUUID.Serialize(ref mySerializationWriter);
                                    mySerializationWriter.WriteObject(ObjectExtent.NextExtent.Position);
                                }

                                #endregion

                            }

                            #endregion
                            
                        }

                        #endregion

                    }

                    #endregion

                }

                #endregion

            }

            catch (Exception e)
            {
                throw new GraphFSException("The ObjectLocator could not be deserialized!\n\n" + e);
            }

        }

        #endregion

        #region DeserializeInnerObject(ref mySerializationReader)

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {

            try
            {

                #region Read ObjectStreams

                _ObjectStreams = new Dictionary<String, ObjectStream>();

                var __NumberOfObjectStreams = (UInt32) mySerializationReader.ReadObject();

                for (var __ObjectStreamCounter = 0; __ObjectStreamCounter < __NumberOfObjectStreams; __ObjectStreamCounter++)
                {

                    var __ObjectStreamName = (String) mySerializationReader.ReadObject();
                    var __ObjectStream     = new ObjectStream(__ObjectStreamName);

                    #region Read ObjectEditions

                    var __NumberOfObjectEditions = (UInt32) mySerializationReader.ReadObject();
                    var __DefaultEditionName     = (String) mySerializationReader.ReadObject();

                    // Set DefaultEdition within the actual ObjectStream object
                    __ObjectStream.SetAsDefaultEdition(__DefaultEditionName);

                    for (var __ObjectEditionCounter = 0; __ObjectEditionCounter < __NumberOfObjectEditions; __ObjectEditionCounter++)
                    {

                        var ObjectEdition_Name                 = (String)  mySerializationReader.ReadObject();
                        var ObjectEdition_IsDeleted            = (Boolean) mySerializationReader.ReadObject();
                        var ObjectEdition_MinNumberOfRevisions = (UInt64)  mySerializationReader.ReadObject();
                        var ObjectEdition_MaxNumberOfRevisions = (UInt64)  mySerializationReader.ReadObject();
                        var ObjectEdition_MinRevisionDelta     = (UInt64)  mySerializationReader.ReadObject();
                        var ObjectEdition_MaxRevisionAge       = (UInt64)  mySerializationReader.ReadObject();

                        var __ObjectEdition = new ObjectEdition(ObjectEdition_Name)
                        {
                            IsDeleted            = ObjectEdition_IsDeleted,
                            MinNumberOfRevisions = ObjectEdition_MinNumberOfRevisions,
                            MaxNumberOfRevisions = ObjectEdition_MaxNumberOfRevisions,
                            MinRevisionDelta     = ObjectEdition_MinRevisionDelta,
                            MaxRevisionAge       = ObjectEdition_MaxRevisionAge
                        };

                        #region ObjectRevisions

                        var NumberOfObjectRevisions = (UInt32) mySerializationReader.ReadObject();

                        for (var __ObjectRevisionCounter = 0; __ObjectRevisionCounter < NumberOfObjectRevisions; __ObjectRevisionCounter++)
                        {

                            // Read actual ObjectRevisionTime
                            var _RevisionID = new RevisionID(
                                                (UInt64) mySerializationReader.ReadObject(),
                                                new UUID((Byte[]) mySerializationReader.ReadObject())
                                              );

                            #region Parent revisions

                            var parents = new HashSet<RevisionID>();
                            var numberOfParentRevisions = (UInt16) mySerializationReader.ReadObject();

                            for(var i=0; i<numberOfParentRevisions; i++)
                            {
                                var r = new RevisionID((UInt64) mySerializationReader.ReadObject(), new UUID((Byte[])mySerializationReader.ReadObject()));
                                parents.Add(r);
                            }

                            #endregion

                            // Write Max, Min NumberOfCopies
                            var ObjectRevision_MaxNumberOfCopies = (UInt64) mySerializationReader.ReadObject();
                            var ObjectRevision_MinNumberOfCopies = (UInt64) mySerializationReader.ReadObject();

                            // Read number of ObjectStreams (aka copies)
                            var NumberOfObjectStreamCopies       = (UInt32) mySerializationReader.ReadObject();

                            var __ObjectRevision = new ObjectRevision(__ObjectStreamName)
                            {
                                MinNumberOfCopies = ObjectRevision_MinNumberOfCopies,
                                MaxNumberOfCopies = ObjectRevision_MaxNumberOfCopies
                            };

                            __ObjectRevision.Set(new List<ObjectDatastream>());

                            #region Read ObjectStreamCopy

                            for (var iObjectStreamCopy = 0; iObjectStreamCopy < NumberOfObjectStreamCopies; iObjectStreamCopy++)
                            {

                                var __ObjectDatastream = new ObjectDatastream();

                                __ObjectDatastream.Compression                        = new ObjectCompression();
                                __ObjectDatastream.Compression.Algorithm              = (CompressionTypes) mySerializationReader.ReadObject();
                                __ObjectDatastream.ForwardErrorCorrection             = new ObjectFEC();
                                __ObjectDatastream.ForwardErrorCorrection.Algorithm   = (ForwardErrorCorrectionTypes) mySerializationReader.ReadObject();
                                __ObjectDatastream.IntegrityCheckValue                = (Byte[]) mySerializationReader.ReadObject();
                                //__ObjectDatastream.ObjectUUID                         = new ObjectUUID();
                                //__ObjectDatastream.ObjectUUID.Deserialize(ref mySerializationReader);
                                __ObjectDatastream.Redundancy                         = new ObjectRedundancy();
                                __ObjectDatastream.Redundancy.Algorithm               = (RedundancyTypes) mySerializationReader.ReadObject();
                                __ObjectDatastream.ReservedLength                     = (UInt64) mySerializationReader.ReadObject();
                                __ObjectDatastream.Blocksize                          = (UInt64) mySerializationReader.ReadObject();

                                #region AvailableStorageIDs

                                var NumberOfObjectStream_AvailableStorageIDs = (UInt16) mySerializationReader.ReadObject();

                                __ObjectDatastream.AvailableStorageUUIDs = new List<StorageUUID>();

                                for (var iObjectStream_AvailableStorageID = 0; iObjectStream_AvailableStorageID < NumberOfObjectStream_AvailableStorageIDs; iObjectStream_AvailableStorageID++)
                                {
                                    var AvailableStorageID = new StorageUUID();
                                    AvailableStorageID.Deserialize(ref mySerializationReader);
                                    __ObjectDatastream.AvailableStorageUUIDs.Add(AvailableStorageID);
                                }

                                #endregion

                                // TODO: Need the ObjectStream.BlockIntegrityArrays need to be Serialized?
                                /*
                                Byte[] SerializedObjectStream_BlockIntegrityArrays = (Byte[])reader.ReadObject();
                                ObjectStream.BlockIntegrityArrays = new BlockIntegrity(SerializedObjectStream_BlockIntegrityArrays);
                                */

                                #region Read ObjectExtent

                                // Read number of ObjectExtents
                                var NumberOfObjectStream_Extents = (UInt32) mySerializationReader.ReadObject();

                                for (var iObjectExtent = 0; iObjectExtent < NumberOfObjectStream_Extents; iObjectExtent++)
                                {

                                    var ObjectExtent                    = new ObjectExtent();
                                    ObjectExtent.Length                 = (UInt64) mySerializationReader.ReadObject();
                                    ObjectExtent.LogicalPosition        = (UInt64) mySerializationReader.ReadObject();
                                    ObjectExtent.StorageUUID            = (StorageUUID) mySerializationReader.ReadObject();
                                    ObjectExtent.PhysicalPosition       = (UInt64) mySerializationReader.ReadObject();

                                    ObjectExtent.NextExtent             = new ExtendedPosition(
                                                                                           (StorageUUID) mySerializationReader.ReadObject(),
                                                                                           (UInt64) mySerializationReader.ReadObject());

                                    __ObjectDatastream.Add(ObjectExtent);

                                }

                                #endregion

                                // Add ObjectStream to ObjectCopies
                                __ObjectRevision.Add(__ObjectDatastream);

                            }

                            #endregion

                            // Add ObjectCopies to ObjectRevisions
                            __ObjectRevision.ParentRevisionIDs = parents;
                            __ObjectEdition.Add(_RevisionID, __ObjectRevision);

                        }

                        #endregion

                        // Add ObjectRevisions to Edition
                        __ObjectStream.Add(ObjectEdition_Name, __ObjectEdition);

                    }

                    #endregion

                    // Add ObjectEditions to _ObjectStreamTypes
                    _ObjectStreams.Add(__ObjectStreamName, __ObjectStream);

                }

                #endregion

            }

            catch (Exception e)
            {
                throw new Exception("ObjectLocator could not be deserialized!\n\n" + e);
            }

        }

        #endregion

        #endregion


        #region ObjectLocator-specific methods

        #region CopyTo(myObjectLocator)

        /// <summary>
        /// Copies all attributes to <paramref name="myObjectLocator"/>. This will keep all references on <paramref name="myObjectLocator"/>
        /// but set all data inside.
        /// </summary>
        /// <param name="myObjectLocator"></param>
        public void CopyTo(ObjectLocator myObjectLocator)
        {
            myObjectLocator.ObjectLocation          = _ObjectLocation;
            myObjectLocator.INodeReference          = _INodeReference;
            myObjectLocator.ObjectLocatorReference  = _ObjectLocatorReference;
            myObjectLocator.PreallocationTickets    = PreallocationTickets;
            myObjectLocator._StructureVersion       = _StructureVersion;
            myObjectLocator.ObjectUUID              = ObjectUUID;
            myObjectLocator._ObjectStreams          = _ObjectStreams;
        }

        #endregion

        #region Clone()

        public override AFSStructure Clone()
        {

            var __ObjectLocatorCopy = new ObjectLocator();
            __ObjectLocatorCopy.ObjectLocation          = _ObjectLocation;
            __ObjectLocatorCopy.INodeReference          = _INodeReference;
            __ObjectLocatorCopy.ObjectLocatorReference  = _ObjectLocatorReference;

            /*
            if (PreallocationTickets != null)
            {
                objectLocatorCopy.PreallocationTickets = new List<byte[]>();
                foreach (Byte[] bytes in PreallocationTickets)
                    objectLocatorCopy.PreallocationTickets.Add((Byte[])bytes.Clone());
                //objectLocatorCopy.PreallocationTickets = PreallocationTickets.Clone<Byte[]>();
            }
            else
                objectLocatorCopy.PreallocationTickets = null;
            */
            __ObjectLocatorCopy._StructureVersion       = _StructureVersion;
            __ObjectLocatorCopy.ObjectUUID              = ObjectUUID;
            __ObjectLocatorCopy._ObjectStreams          = new Dictionary<String, ObjectStream>();

            foreach (var __ObjectStreamsEnumerator in _ObjectStreams)
            {

                var __ObjectStream = new ObjectStream();

                foreach (var __ObjectEditionsEnumerator in __ObjectStreamsEnumerator.Value)
                {
                    var __ObjectEdition     = new ObjectEdition()
                                                {
                                                    MinNumberOfRevisions    = __ObjectEditionsEnumerator.Value.MinNumberOfRevisions,
                                                    MaxNumberOfRevisions    = __ObjectEditionsEnumerator.Value.MaxNumberOfRevisions,
                                                    MinRevisionDelta        = __ObjectEditionsEnumerator.Value.MinRevisionDelta,
                                                    MaxRevisionAge          = __ObjectEditionsEnumerator.Value.MaxRevisionAge
                                                };

                    foreach (var __ObjectRevisionsEnumerator in __ObjectEditionsEnumerator.Value)
                    {

                        var __ObjectRevision = new ObjectRevision(__ObjectRevisionsEnumerator.Value.ObjectStream)
                            {
                                MinNumberOfCopies = __ObjectRevisionsEnumerator.Value.MinNumberOfCopies,
                                MaxNumberOfCopies = __ObjectRevisionsEnumerator.Value.MaxNumberOfCopies
                            };

                        __ObjectRevision.CacheUUID          = __ObjectRevisionsEnumerator.Value.CacheUUID;
                        __ObjectRevision.ParentRevisionIDs  = __ObjectRevisionsEnumerator.Value.ParentRevisionIDs;

                        #region Copy ObjectDatastreams

                        var __ObjectDatastreams = new List<ObjectDatastream>();

                        foreach (var __ObjectDatastream in __ObjectRevisionsEnumerator.Value)
                        {

                            var _NewObjectDatastream = new ObjectDatastream();

                            _NewObjectDatastream.AvailableStorageUUIDs = new List<StorageUUID>();
                            #if(__MonoCS__)
                            foreach (var _StorageUUID in __ObjectDatastream.AvailableStorageUUIDs)
                                _NewObjectDatastream.AvailableStorageUUIDs.Add((StorageUUID)_StorageUUID.Clone());
                            #else
                            foreach (var _StorageUUID in __ObjectDatastream.AvailableStorageUUIDs)
                                _NewObjectDatastream.AvailableStorageUUIDs.Add(new StorageUUID(_StorageUUID));
                            #endif

//                            newStream.BlockIntegrityArrays = copyStream.BlockIntegrityArrays;
                            _NewObjectDatastream.Compression               = new ObjectCompression();
                            _NewObjectDatastream.Compression.Algorithm     = __ObjectDatastream.Compression.Algorithm;
                            _NewObjectDatastream.ForwardErrorCorrection    = new ObjectFEC();
                            _NewObjectDatastream.ForwardErrorCorrection.Algorithm = __ObjectDatastream.ForwardErrorCorrection.Algorithm;
                            _NewObjectDatastream.IntegrityCheckValue       = __ObjectDatastream.IntegrityCheckValue;
                            //_NewObjectDatastream.ObjectUUID                = __ObjectDatastream.ObjectUUID;
                            _NewObjectDatastream.Redundancy                = new ObjectRedundancy();
                            _NewObjectDatastream.Redundancy.Algorithm      = __ObjectDatastream.Redundancy.Algorithm;
                            _NewObjectDatastream.ReservedLength            = __ObjectDatastream.ReservedLength;
                            //newStream.StreamLength              = copyStream.StreamLength;


                            #region Copy ObjectExtents

                            //newStream.Extents = copyStream.Extents.Clone<ObjectExtent>();

                            foreach (var __ObjectExtent in __ObjectDatastream)
                            {

                                var _NewObjectExtent                   = new ObjectExtent();
                                _NewObjectExtent.Length                = __ObjectExtent.Length;
                                _NewObjectExtent.LogicalPosition       = __ObjectExtent.LogicalPosition;
                                _NewObjectExtent.StorageUUID             = __ObjectExtent.StorageUUID;
                                _NewObjectExtent.PhysicalPosition      = __ObjectExtent.PhysicalPosition;
                                
                                _NewObjectExtent.NextExtent            = new ExtendedPosition(__ObjectExtent.NextExtent.StorageUUID, __ObjectExtent.NextExtent.Position);
                                
                                _NewObjectDatastream.Add(_NewObjectExtent);

                            }

                            __ObjectDatastreams.Add(_NewObjectDatastream);

                            #endregion

                        }

                        __ObjectRevision.Set(__ObjectDatastreams);

                        #endregion

                        __ObjectEdition.Add(__ObjectRevisionsEnumerator.Key, __ObjectRevision);

                    }
                    __ObjectStream.Add(__ObjectEditionsEnumerator.Key, __ObjectEdition);
                }
                __ObjectLocatorCopy._ObjectStreams.Add(__ObjectStreamsEnumerator.Key, __ObjectStream);
            }
            
            return __ObjectLocatorCopy;

        }

        #endregion

        #endregion


        #region IGraphFSDictionary<String, ObjectEditions> Members

        #region Add(myObjectStream, myObjectEditions)

        /// <summary>
        /// Adds the given ObjectStream name and ObjectEditions object to the list of
        /// streams.
        /// </summary>
        /// <param name="myObjectStream">the name of the ObjectStream</param>
        /// <param name="myObjectEditions">the myObjectEditions object</param>
        public Boolean Add(String myObjectStream, ObjectStream myObjectEditions)
        {

            try
            {

                _ObjectStreams.Add(myObjectStream, myObjectEditions);

                isDirty = true;

                return true;

            }

            catch (Exception e)
            {
                return false;
            }

        }

        #endregion

        #region Add(myKeyValuePair)

        public Boolean Add(KeyValuePair<String, ObjectStream> myKeyValuePair)
        {
            return Add(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion


        #region this[myObjectStream]

        public ObjectStream this[String myObjectStream]
        {

            get
            {

                ObjectStream _ObjectEditions;

                if (_ObjectStreams.TryGetValue(myObjectStream, out _ObjectEditions))
                    return _ObjectEditions;

                return null;

            }

            set
            {

                if (_ObjectStreams.ContainsKey(myObjectStream))
                    _ObjectStreams[myObjectStream] = value;

                else Add(myObjectStream, value);

                isDirty = true;

            }

        }

        #endregion


        #region ContainsKey(myObjectStream)

        public Boolean ContainsKey(String myObjectStream)
        {

            ObjectStream _ObjectEditions;

            if (_ObjectStreams.TryGetValue(myObjectStream, out _ObjectEditions))
                return true;

            return false;

        }

        #endregion

        #region Contains(myKeyValuePair)

        public Boolean Contains(KeyValuePair<String, ObjectStream> myKeyValuePair)
        {

            ObjectStream _ObjectEditions;

            if (_ObjectStreams.TryGetValue(myKeyValuePair.Key, out _ObjectEditions))
                if (myKeyValuePair.Value.Equals(_ObjectEditions))
                    return true;

            return false;

        }

        #endregion

        #region Keys

        public IEnumerable<String> Keys
        {
            get
            {
                return from _Items in _ObjectStreams select _Items.Key;
            }
        }

        #endregion

        #region Values

        public IEnumerable<ObjectStream> Values
        {
            get
            {
                return from _Items in _ObjectStreams select _Items.Value;
            }
        }

        #endregion

        #region Count

        public UInt64 Count
        {
            get
            {
                return _ObjectStreams.ULongCount();
            }
        }

        #endregion


        #region Remove(myObjectStream)

        public Boolean Remove(String myObjectStream)
        {

            try
            {

                if (_ObjectStreams.Remove(myObjectStream))
                {
                    isDirty = true;
                    return true;
                }

                return false;

            }

            catch (Exception e)
            {
                return false;
            }

        }

        #endregion

        #region Remove(myKeyValuePair)

        public Boolean Remove(KeyValuePair<String, ObjectStream> myKeyValuePair)
        {

            ObjectStream _ObjectEditions;

            if (_ObjectStreams.TryGetValue(myKeyValuePair.Key, out _ObjectEditions))
                if (myKeyValuePair.Value.Equals(_ObjectEditions))
                    return _ObjectStreams.Remove(myKeyValuePair.Key);

            return false;

        }

        #endregion

        #region Clear()

        public void Clear()
        {
            _ObjectStreams.Clear();
        }

        #endregion

        #endregion

        #region IEnumerable<KeyValuePair<String, ObjectEditions>> Members

        public IEnumerator<KeyValuePair<String, ObjectStream>> GetEnumerator()
        {
            return _ObjectStreams.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _ObjectStreams.GetEnumerator();
        }

        #endregion

        #region IObjectLocation Members

        #region ObjectPath

        [NonSerialized]
        protected String _ObjectPath;

        /// <summary>
        /// Stores the ObjectPath of this APandoraObject. Changing this
        /// property will automagically change the myObjectLocation property.
        /// </summary>
        [NotIFastSerialized]
        public String ObjectPath
        {

            get
            {
                return _ObjectPath;
            }

            set
            {

                if (value == null || value.Length < FSPathConstants.PathDelimiter.Length)
                    throw new ArgumentNullException("Invalid ObjectPath!");

                _ObjectPath      = value;
                _ObjectLocation  = new ObjectLocation(DirectoryHelper.Combine(_ObjectPath, _ObjectName));

            }

        }

        #endregion

        #region ObjectName

        [NonSerialized]
        protected String _ObjectName;

        /// <summary>
        /// Stores the ObjectName of this APandoraObject. Changing this
        /// property will automagically change the myObjectLocation property.
        /// </summary>
        [NotIFastSerialized]
        public String ObjectName
        {

            get
            {
                return _ObjectName;
            }

            set
            {

                if (value == null || value.Length < FSPathConstants.PathDelimiter.Length)
                    throw new ArgumentNullException("Invalid ObjectName!");

                _ObjectName      = value;
                _ObjectLocation = new ObjectLocation(DirectoryHelper.Combine(_ObjectPath, _ObjectName));

            }

        }

        #endregion

        #region ObjectLocation

        [NonSerialized]
        protected ObjectLocation _ObjectLocation;

        /// <summary>
        /// Stores the complete ObjectLocation (ObjectPath and ObjectName) of
        /// this APandoraObject. Changing this property will automagically
        /// change the ObjectPath and ObjectName property.
        /// </summary>
        [NotIFastSerialized]
        public ObjectLocation ObjectLocation
        {

            get
            {
                return _ObjectLocation;
            }

            set
            {

                if (value == null || value.Length < FSPathConstants.PathDelimiter.Length)
                    throw new ArgumentNullException("Invalid ObjectLocation!");

                _ObjectLocation  = value;
                _ObjectPath      = _ObjectLocation.Path;
                _ObjectName      = _ObjectLocation.Name;

                isDirty          = true;

            }

        }

        #endregion

        #endregion

        #region IDirectoryListing Members

        #region IPandoraFSReference

        private IGraphFS _IPandoraFSReference;

        public IGraphFS IGraphFSReference
        {

            get
            {
                return _IPandoraFSReference;
            }

            set
            {
                _IPandoraFSReference = value;
            }

        }

        #endregion

        #region ObjectExists(myObjectName)

        public Trinary ObjectExists(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.DotLink))
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.DotDotLink))
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.ObjectLocatorUUID))
                return Trinary.TRUE;

            foreach (String _ObjectStreamType in _ObjectStreams.Keys)
                if (myObjectName.Equals(_ObjectStreamType))
                    return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region ObjectStreamExists(myObjectName, myObjectStream)

        public Trinary ObjectStreamExists(String myObjectName, String myObjectStream)
        {

            if (myObjectName.Equals(FSConstants.DotLink) && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.DotDotLink) && myObjectStream == FSConstants.VIRTUALDIRECTORY)
                return Trinary.TRUE;

            if (myObjectName.Equals(FSConstants.ObjectLocatorUUID) && myObjectStream == FSConstants.INLINEDATA)
                return Trinary.TRUE;

            foreach (String _DirectoryEntry in _ObjectStreams.Keys)
                if (myObjectName.Equals(_DirectoryEntry))
                    return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region GetObjectStreamsList(myObjectName)

        public IEnumerable<String> GetObjectStreamsList(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.DotLink))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(FSConstants.DotDotLink))
                return new List<String> { FSConstants.VIRTUALDIRECTORY };

            if (myObjectName.Equals(FSConstants.ObjectLocatorUUID))
                return new List<String> { FSConstants.INLINEDATA };

            foreach (String _DirectoryEntry in _ObjectStreams.Keys)
                if (myObjectName.Equals(_DirectoryEntry))
                    return new List<String> { FSConstants.VIRTUALDIRECTORY };

            return new List<String>();

        }

        #endregion

        #region GetObjectINodePositions(String myObjectName)

        public IEnumerable<ExtendedPosition> GetObjectINodePositions(String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetInlineData(myObjectName)

        public Byte[] GetInlineData(String myObjectName)
        {

            if (myObjectName.Equals(FSConstants.ObjectLocatorUUID))
                return Encoding.UTF8.GetBytes(ObjectUUID.ToHexString(SeperatorTypes.COLON));

            return new Byte[0];

        }

        #endregion

        #region hasInlineData(myObjectName)

        public Trinary hasInlineData(String myObjectName)
        {
            return ObjectStreamExists(myObjectName, FSConstants.INLINEDATA);
        }

        #endregion

        #region GetSymlink(myObjectName)

        public ObjectLocation GetSymlink(String myObjectName)
        {
            return null;
        }

        #endregion

        #region isSymlink(myObjectName)

        public Trinary isSymlink(String myObjectName)
        {
            return Trinary.FALSE;
        }

        #endregion

        #region GetDirectoryListing()

        public IEnumerable<String> GetDirectoryListing()
        {

            var _DirectoryListing = new List<String>();

            _DirectoryListing.Add(".");
            _DirectoryListing.Add("..");

            _DirectoryListing.Add(FSConstants.ObjectLocatorUUID);

            foreach (String _DirectoryEntry in _ObjectStreams.Keys)
                _DirectoryListing.Add(_DirectoryEntry);

            return _DirectoryListing;

        }

        #endregion

        #region GetDirectoryListing(myFunc)

        public IEnumerable<String> GetDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetDirectoryListing(myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

        public IEnumerable<String> GetDirectoryListing(String[] myName, string[] myIgnoreName, string[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetExtendedDirectoryListing()

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing()
        {

            var _Output = new List<DirectoryEntryInformation>();
            var _OutputParameter = new DirectoryEntryInformation();

            _OutputParameter.Name         = ".";
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _Output.Add(_OutputParameter);

            _OutputParameter.Name         = "..";
            _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
            _Output.Add(_OutputParameter);

            _OutputParameter.Name        = FSConstants.ObjectLocatorUUID;
            _OutputParameter.Streams = new HashSet<String> { FSConstants.INLINEDATA };
            _Output.Add(_OutputParameter);

            foreach (var _DirectoryEntry in _ObjectStreams.Keys)
            {
                _OutputParameter.Name        = _DirectoryEntry;
                _OutputParameter.Streams = new HashSet<String> { FSConstants.VIRTUALDIRECTORY };
                _Output.Add(_OutputParameter);
            }

            return _Output;

        }

        #endregion

        #region GetExtendedDirectoryListing(myFunc)

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(Func<KeyValuePair<String, DirectoryEntry>, Boolean> myFunc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GetExtendedDirectoryListing(myName, myIgnoreName, myRegExpr, myObjectStreams, myIgnoreObjectStreams)

        public IEnumerable<DirectoryEntryInformation> GetExtendedDirectoryListing(String[] myName, string[] myIgnoreName, string[] myRegExpr, List<String> myObjectStreams, List<String> myIgnoreObjectStreams)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DirCount()

        public UInt64 DirCount
        {
            get
            {
                return (UInt64)GetDirectoryListing().LongCount();
            }
        }

        #endregion


        public NHIDirectoryObject NotificationHandling
        {
            get { throw new NotImplementedException(); }
        }

        public void SubscribeNotification(NHIDirectoryObject myNotificationHandling)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeNotification(NHIDirectoryObject myNotificationHandling)
        {
            throw new NotImplementedException();
        }


        #region GetDirectoryEntry(myObjectName)

        public DirectoryEntry GetDirectoryEntry(String myObjectName)
        {
            
            foreach (String _DirectoryEntry in _ObjectStreams.Keys)
                if (_DirectoryEntry.Equals(myObjectName))
                    return new DirectoryEntry { Virtual = new HashSet<String> { FSConstants.VIRTUALDIRECTORY } };

            return null;

        }

        #endregion

        #endregion

        #region IXMLExport Members

        #region ToXML()

        public XDocument ToXML()
        {

            var XML = new XDocument(

                new XElement("ObjectLocator",
                    new XAttribute("Version", _StructureVersion),
                    new XAttribute("ObjectUUID", ObjectUUID.ToHexString(SeperatorTypes.COLON)),

                new XElement("ObjectStreams",

                    from _ObjectStream in _ObjectStreams select
                    new XElement("ObjectStream",
                        new XAttribute("Name", _ObjectStream.Key),
                        new XAttribute("Type", "tobedone!"),

                        new XElement("ObjectEditions",
                            new XAttribute("DefaultEdition", _ObjectStream.Value.DefaultEditionName),

                            from _ObjectEdition in _ObjectStream.Value select
                            new XElement("ObjectEdition",
                                new XAttribute("Name", _ObjectEdition.Key),
                                new XAttribute("IsDeleted", _ObjectEdition.Value.IsDeleted),

                                new XElement("ObjectRevisions",
                                    new XAttribute("MinNumberOfRevisions", _ObjectEdition.Value.MinNumberOfRevisions),
                                    new XAttribute("MaxNumberOfRevisions", _ObjectEdition.Value.MaxNumberOfRevisions),
                                    new XAttribute("MinRevisionDelta", _ObjectEdition.Value.MinRevisionDelta),
                                    new XAttribute("MaxRevisionAge", _ObjectEdition.Value.MaxRevisionAge),

                                    from _ObjectRevisionsEnumerator in _ObjectEdition.Value select new XElement("ObjectRevision",
                                        new XAttribute("RevisionID", _ObjectRevisionsEnumerator.Key),

                                        new XElement("ParentRevisions",
                                        from _ParentRevisionID in _ObjectRevisionsEnumerator.Value.ParentRevisionIDs select
                                            new XElement("ParentRevision",
                                                new XAttribute("RevisionID", _ParentRevisionID)

                                                
                                                
                                                ))))))))));

            return XML;



//                            XMLString.AppendLine("<ParentRevisions>".SpacingLeft(16));

//                            if (ObjectEditionsEnumerator.Value._ParentRevisions.Count > 0 && ObjectEditionsEnumerator.Value._ParentRevisions.ContainsKey(ObjectRevisionsEnumerator.Key))
//                                foreach (RevisionID _ParentRevision in ObjectEditionsEnumerator.Value._ParentRevisions[ObjectRevisionsEnumerator.Key])
//                                    XMLString.AppendFormat("<ParentRevision Timestamp=\"{0}\" />".SpacingLeft(18), _ParentRevision.ToString()); XMLString.AppendLine();

//                            XMLString.AppendLine("</ParentRevisions>".SpacingLeft(16));

//                            XMLString.AppendFormat("<ObjectCopies MinNumberOfCopies=\"{0}\" MaxNumberOfCopies=\"{1}\" CacheUUID=\"{2}\">".SpacingLeft(16), ObjectRevisionsEnumerator.Value.MinNumberOfCopies, ObjectRevisionsEnumerator.Value.MaxNumberOfCopies, ObjectRevisionsEnumerator.Value.CacheUUID); XMLString.AppendLine();

//                            #region Write ObjectStreams

////                            if (ObjectRevisionsEnumerator.Value.Co != null)
//                                foreach (ObjectStream ObjectStream in ObjectRevisionsEnumerator.Value)
//                                {

//                                    XMLString.Append("<ObjectStream".SpacingLeft(18));
//                                    XMLString.AppendFormat(" Algorithm=\"{0}\"",              ObjectStream.Compression);
//                                    XMLString.AppendFormat(" ForwardErrorCorrection=\"{0}\"", ObjectStream.ForwardErrorCorrection);
//                                    XMLString.AppendFormat(" IntegrityCheckValue=\"{0}\"",    ByteArrayHelper.ByteArrayToFormatedString(ObjectStream.IntegrityCheckValue));
//                                    XMLString.AppendFormat(" ObjectUUID=\"{0}\"",             ObjectStream.ObjectUUID);
//                                    XMLString.AppendFormat(" Redundancy=\"{0}\"",             ObjectStream.Redundancy);
//                                    XMLString.AppendFormat(" ReservedLength=\"{0}\"",         ObjectStream.ReservedLength);
//                                    XMLString.AppendFormat(" StreamLength=\"{0}\"",           ObjectStream.StreamLength);
//                                    XMLString.AppendLine(">");

//                                    #region Write AccessRights

//                                    if (ObjectStream.AccessRights != null)
//                                        if (ObjectStream.AccessRights.Count > 0)
//                                        {

//                                            XMLString.AppendLine("<AccessRights>".SpacingLeft(20));

//                                            foreach (AccessRight _AccessRight in ObjectStream.AccessRights)
//                                            {
//                                                XMLString.AppendFormat("<AccessRight AccessFlags=\"{0}\" EncryptionParameters=\"{1}\" UserID=\"{2}\" />".SpacingLeft(20), _AccessRight.AccessFlags.ToString(), _AccessRight.EncryptionParameters.ToString(), _AccessRight.UserID); XMLString.AppendLine();
//                                            }

//                                            XMLString.AppendLine("</AccessRights>".SpacingLeft(20));

//                                        }

//                                        else XMLString.AppendLine("<AccessRights />".SpacingLeft(20));

//                                    else XMLString.AppendLine("<AccessRights />".SpacingLeft(20));

//                                    #endregion

//                                    #region AvailableStorageIDs

//                                    if (ObjectStream.AvailableStorageIDs != null)
//                                        if (ObjectStream.AvailableStorageIDs.Count > 0)
//                                        {


//                                            XMLString.AppendLine("<AvailableStorageIDs>".SpacingLeft(20));

//                                            foreach (UInt64 _StorageID in ObjectStream.AvailableStorageIDs)
//                                            {
//                                                XMLString.AppendFormat("<AvailableStorageID StorageID=\"{0}\">".SpacingLeft(20), _StorageID);
//                                                XMLString.AppendLine();
//                                            }

//                                            XMLString.AppendLine("</AvailableStorageIDs>".SpacingLeft(20));

//                                        }

//                                        else XMLString.AppendLine("<AvailableStorageIDs />".SpacingLeft(20));

//                                    else XMLString.AppendLine("<AvailableStorageIDs />".SpacingLeft(20));

//                                    #endregion

//                                    #region BlockIntegrityArrays

//                                    //if (ObjectStream.BlockIntegrityArrays != null)
//                                        //if (ObjectStream.BlockIntegrityArrays.Count > 0)
//                                        //{


//                                        //    XMLString.AppendLine("<BlockIntegrityArrays>".SpacingLeft(20));

//                                        //    foreach (BlockIntegrity _BlockIntegrity in ObjectStream.BlockIntegrityArrays)
//                                        //    {
//                                        //        XMLString.AppendFormat("<BlockIntegrity XXX=\"{0}\">".SpacingLeft(20), _BlockIntegrity);
//                                        //        XMLString.AppendLine();
//                                        //    }

//                                        //    XMLString.AppendLine("</BlockIntegrityArrays>".SpacingLeft(20));

//                                        //}

//                                        //else XMLString.AppendLine("<BlockIntegrityArrays />".SpacingLeft(20));

//                                    //else
//                                    XMLString.AppendLine("<BlockIntegrityArrays />".SpacingLeft(20));

//                                    #endregion

//                                    #region Write ObjectExtent

//                                    XMLString.AppendLine("<Extents>".SpacingLeft(20));

//                                    if (ObjectStream.Extents != null)
//                                        foreach (ObjectExtent ObjectExtent in ObjectStream.Extents)
//                                        {
//                                            XMLString.Append("<Extent".SpacingLeft(22));
//                                            XMLString.AppendFormat(" Length=\"{0}\"",               ObjectExtent.Length);
//                                            XMLString.AppendFormat(" LogicalPosition=\"{0}\"",      ObjectExtent.LogicalPosition);
//                                            XMLString.AppendFormat(" StorageID=\"{0}\"",            ObjectExtent.StorageID);
//                                            XMLString.AppendFormat(" PhysicalPosition=\"{0}\"",     ObjectExtent.PhysicalPosition);
//                                            XMLString.AppendFormat(" NextExtent_StorageID=\"{0}\"", ObjectExtent.NextExtent.StorageID);
//                                            XMLString.AppendFormat(" NextExtent_Position=\"{0}\"",  ObjectExtent.NextExtent.Position);
//                                            XMLString.AppendLine(" />");
//                                        }

//                                    XMLString.AppendLine("</Extents>".SpacingLeft(20));

//                                    #endregion

//                                    XMLString.AppendLine("</ObjectStream>".SpacingLeft(18));

//                                }

//                        #endregion

//                        XMLString.AppendLine("</ObjectCopies>".SpacingLeft(16));
//                        XMLString.AppendLine("</ObjectRevisions>".SpacingLeft(14));

//                    }

//                    #endregion

//                    XMLString.AppendLine("</ObjectRevisions>".SpacingLeft(12));
//                    XMLString.AppendLine("</ObjectEditions>".SpacingLeft(10));

//                }

//                #endregion

//                XMLString.AppendLine("</ObjectEditions>".SpacingLeft(8));
//                XMLString.AppendLine("</ObjectStream>".SpacingLeft(6));

//            }

//            XMLString.AppendLine("</ObjectStreamTypes>".SpacingLeft(4));
//            XMLString.AppendLine("</ObjectLocator>".SpacingLeft(2));

//            return XMLString.ToString();

        }

        #endregion

        #endregion

        
        #region ToString()

        public override String ToString()
        {

            var _ReturnValue        = "[";
            var _ObjectStreamList   = new List<String>(_ObjectStreams.Keys);

            for (int i=0; i < _ObjectStreamList.Count-1; i++)
                _ReturnValue += _ObjectStreamList[i] + ", ";

            _ReturnValue += _ObjectStreamList[_ObjectStreams.Keys.Count-1] + "]";

            return _ReturnValue;

        }

        #endregion

    }

}