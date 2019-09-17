﻿//
// Copyright 2018-2019 Dynatrace LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Dynatrace.OpenKit.API;
using Dynatrace.OpenKit.Protocol;

namespace Dynatrace.OpenKit.Core.Objects
{

    /// <summary>
    ///  Abstract base class implementing the <see cref="IAction"/> interface
    /// </summary>
    public abstract class BaseAction : OpenKitComposite, IAction
    {
        /// <summary>
        /// The web request tracer being returned once this action was closed.
        /// </summary>
        private static readonly IWebRequestTracer NullWebRequestTracer = new NullWebRequestTracer();

        /// <summary>
        /// the parent object of this <see cref="IAction"/>
        /// </summary>
        private readonly OpenKitComposite parent;

        /// <summary>
        /// object for synchronization
        /// </summary>
        protected readonly object LockObject = new object();

        internal BaseAction(ILogger logger,
            OpenKitComposite parent,
            string name,
            Beacon beacon)
        {
            Logger = logger;
            this.parent = parent;

            Id = beacon.NextId;
            Name = name;

            StartTime = beacon.CurrentTimestamp;
            StartSequenceNo = beacon.NextSequenceNumber;

            Beacon = beacon;
        }

        /// <summary>
        /// Unique identifier of this <see cref="IAction"/>.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Name of this <see cref="IAction"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Unique identifier of the parent <see cref="IAction"/>.
        /// </summary>
        public int ParentId => parent.ActionId;

        /// <summary>
        /// Returns the time when this <see cref="IAction"/> was started.
        /// </summary>
        public long StartTime { get; }

        /// <summary>
        /// Returns the time when this <see cref="IAction"/> was ended or <code>-1</code> if the action was not ended yet.
        /// </summary>
        public long EndTime { get; private set; } = -1;

        /// <summary>
        /// Returns the start sequence number of this <see cref="IAction"/>.
        /// </summary>
        public int StartSequenceNo { get; }

        /// <summary>
        /// Returns the end sequence number of this <see cref="IAction"/>.
        /// </summary>
        public int EndSequenceNo { get; private set; } = -1;

        /// <summary>
        /// Indicates if this action was <see cref="LeaveAction">left</see>
        /// </summary>
        internal bool IsActionLeft => EndTime != -1L;

        /// <summary>
        /// Beacon for sending the data
        /// </summary>
        protected Beacon Beacon { get; }

        /// <summary>
        /// Logger for tracing log messages
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Returns the parent <see cref="IAction"/> which might be <code>null</code> in case the parent is not an
        /// implementor of the <see cref="IAction"/> interface.
        /// </summary>
        internal abstract IAction ParentAction { get; }

        #region IAction implementation

        public IAction ReportEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Logger.Warn($"{this} ReportEvent: eventName must not be null or empty");
                return this;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} ReportEvent({eventName})");
            }

            lock (LockObject)
            {
                if (!IsActionLeft)
                {
                    Beacon.ReportEvent(Id, eventName);
                }
            }

            return this;
        }

        public IAction ReportValue(string valueName, string value)
        {
            if (string.IsNullOrEmpty(valueName))
            {
                Logger.Warn($"{this} ReportValue (string): valueName must not be null or empty");
                return this;
            }

            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} ReportValue({valueName}, {value})");
            }

            lock (LockObject)
            {
                if (!IsActionLeft)
                {
                    Beacon.ReportValue(Id, valueName, value);
                }
            }

            return this;
    }

        public IAction ReportValue(string valueName, double value)
        {
            if (string.IsNullOrEmpty(valueName))
            {
                Logger.Warn($"{this} ReportValue (double): valueName must not be null or empty");
                return this;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} ReportValue(double) ({valueName}, {value})");
            }

            lock (LockObject)
            {
                if (!IsActionLeft)
                {
                    Beacon.ReportValue(Id, valueName, value);
                }
            }

            return this;
        }

        public IAction ReportValue(string valueName, int value)
        {
            if (string.IsNullOrEmpty(valueName))
            {
                Logger.Warn($"{this} ReportValue (int): valueName must not be null or empty");
                return this;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} ReportValue(int) ({valueName}, {value})");
            }

            lock (LockObject)
            {
                if (!IsActionLeft)
                {
                    Beacon.ReportValue(Id, valueName, value);
                }
            }

            return this;
        }

        public IAction ReportError(string errorName, int errorCode, string reason)
        {
            if (string.IsNullOrEmpty(errorName))
            {
                Logger.Warn($"{this} ReportError: errorName must not be null or empty");
                return this;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} ReportError({errorName}, {errorCode}, {reason})");
            }

            lock (LockObject)
            {
                if (!IsActionLeft)
                {
                    Beacon.ReportError(Id, errorName, errorCode, reason);
                }
            }

            return this;
        }

        public IWebRequestTracer TraceWebRequest(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Logger.Warn($"{this} TraceWebRequest (String): url must not be null or empty");
                return NullWebRequestTracer;
            }
            if (!WebRequestTracer.IsValidUrlScheme(url))
            {
                Logger.Warn($"{this} TraceWebRequest (String): url \"{url}\" does not have a valid scheme");
                return NullWebRequestTracer;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} TraceWebRequest (${url})");
            }

            lock (LockObject)
            {
                if (!IsActionLeft)
                {
                    var tracer = new WebRequestTracer(Logger, this, Beacon, url);
                    StoreChildInList(tracer);

                    return tracer;
                }
            }

            return NullWebRequestTracer;
        }

        public IAction LeaveAction()
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} LeaveAction({Name})");
            }

            lock (LockObject)
            {
                if (IsActionLeft)
                {
                    // LeaveAction was previously called
                    return ParentAction;
                }

                EndTime = Beacon.CurrentTimestamp;
                EndSequenceNo = Beacon.NextSequenceNumber;

                Beacon.AddAction(this);
            }

            var childObjects = GetCopyOfChildObjects();
            foreach (var childObject in childObjects)
            {
                childObject.Dispose();
            }

            // detach from parent
            parent.OnChildClosed(this);

            return ParentAction;
        }

        #endregion

        #region OpenKitComposite implementation

        public override int ActionId => Id;

        internal override void OnChildClosed(IOpenKitObject childObject)
        {
            lock (LockObject)
            {
                RemoveChildFromList(childObject);
            }
        }

        #endregion

        public override void Dispose()
        {
            LeaveAction();
        }
    }
}