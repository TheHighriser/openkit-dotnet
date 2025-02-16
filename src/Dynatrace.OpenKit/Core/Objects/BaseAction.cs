﻿//
// Copyright 2018-2021 Dynatrace LLC
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
using Dynatrace.OpenKit.Util;
using System;

namespace Dynatrace.OpenKit.Core.Objects
{

    /// <summary>
    ///  Abstract base class implementing the <see cref="IAction"/> interface
    /// </summary>
    public abstract class BaseAction : OpenKitComposite, IActionInternals, ICancelableOpenKitObject
    {
        /// <summary>
        /// Indicates whether this action was already <see cref="LeaveAction">left</see> or not.
        /// </summary>
        private bool isActionLeft;

        /// <summary>
        /// the parent object of this <see cref="IAction"/>
        /// </summary>
        private readonly IOpenKitComposite parent;

        #region Constructor

        /// <summary>
        /// object for synchronization
        /// </summary>
        protected readonly object LockObject = new object();

        internal BaseAction(ILogger logger,
            IOpenKitComposite parent,
            string name,
            IBeacon beacon)
        {
            Logger = logger;
            this.parent = parent;

            Id = beacon.NextId;
            Name = name;

            StartTime = beacon.CurrentTimestamp;
            StartSequenceNo = beacon.NextSequenceNumber;

            Beacon = beacon;
        }

        #endregion

        /// <summary>
        /// Accessor for simplified explicit access to internal <see cref="IOpenKitComposite"/>.
        /// </summary>
        private protected IOpenKitComposite ThisComposite => this;

        /// <summary>
        /// Accessor for simplified explicit access to internal <see cref="IActionInternals"/>.
        /// </summary>
        private protected IActionInternals ThisAction => this;

        #region IActionInternals implementation

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
        bool IActionInternals.IsActionLeft => isActionLeft;

        IAction IActionInternals.ParentAction => ParentAction;

        #endregion

        /// <summary>
        /// Returns the parent <see cref="IAction"/> which might be <code>null</code> in case the parent is not an
        /// implementor of the <see cref="IAction"/> interface.
        /// </summary>
        internal abstract IAction ParentAction { get; }

        /// <summary>
        /// Beacon for sending the data
        /// </summary>
        private protected IBeacon Beacon { get; }

        /// <summary>
        /// Logger for tracing log messages
        /// </summary>
        protected ILogger Logger { get; }


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
                if (!ThisAction.IsActionLeft)
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
                Logger.Debug($"{this} ReportValue (string) ({valueName}, {value})");
            }

            lock (LockObject)
            {
                if (!ThisAction.IsActionLeft)
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
                Logger.Debug($"{this} ReportValue (double) ({valueName}, {value.ToInvariantString()})");
            }

            lock (LockObject)
            {
                if (!ThisAction.IsActionLeft)
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
                Logger.Debug($"{this} ReportValue (int) ({valueName}, {value.ToInvariantString()})");
            }

            lock (LockObject)
            {
                if (!ThisAction.IsActionLeft)
                {
                    Beacon.ReportValue(Id, valueName, value);
                }
            }

            return this;
        }

        public IAction ReportValue(string valueName, long value)
        {
            if (string.IsNullOrEmpty(valueName))
            {
                Logger.Warn($"{this} ReportValue (long): valueName must not be null or empty");
                return this;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} ReportValue (long) ({valueName}, {value.ToInvariantString()})");
            }

            lock (LockObject)
            {
                if (!ThisAction.IsActionLeft)
                {
                    Beacon.ReportValue(Id, valueName, value);
                }
            }

            return this;
        }

        public IAction ReportError(string errorName, int errorCode)
        {
            if (string.IsNullOrEmpty(errorName))
            {
                Logger.Warn($"{this} ReportError: errorName must not be null or empty");
                return this;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} ReportError({errorName}, {errorCode.ToInvariantString()})");
            }

            lock (LockObject)
            {
                if (!ThisAction.IsActionLeft)
                {
                    Beacon.ReportError(Id, errorName, errorCode);
                }
            }

            return this;
        }

        public IAction ReportError(string errorName, string causeName, string causeDescription, string causeStackTrace)
        {
            if (string.IsNullOrEmpty(errorName))
            {
                Logger.Warn($"{this} ReportError: errorName must not be null or empty");
                return this;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} ReportError({errorName}, {causeName}, {causeDescription}, {causeStackTrace})");
            }

            lock (LockObject)
            {
                if (!ThisAction.IsActionLeft)
                {
                    Beacon.ReportError(Id, errorName, causeName, causeDescription, causeStackTrace);
                }
            }

            return this;
        }

        public IAction ReportError(string errorName, Exception exception)
        {
            if (string.IsNullOrEmpty(errorName))
            {
                Logger.Warn($"{this} ReportError: errorName must not be null or empty");
                return this;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} ReportError({errorName}, {exception})");
            }

            lock (LockObject)
            {
                if (!ThisAction.IsActionLeft)
                {
                    Beacon.ReportError(Id, errorName, exception);
                }
            }

            return this;
        }

        public IWebRequestTracer TraceWebRequest(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Logger.Warn($"{this} TraceWebRequest (String): url must not be null or empty");
                return NullWebRequestTracer.Instance;
            }
            if (!WebRequestTracer.IsValidUrlScheme(url))
            {
                Logger.Warn($"{this} TraceWebRequest (String): url \"{url}\" does not have a valid scheme");
                return NullWebRequestTracer.Instance;
            }
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} TraceWebRequest(${url})");
            }

            lock (LockObject)
            {
                if (!ThisAction.IsActionLeft)
                {
                    var tracer = new WebRequestTracer(Logger, this, Beacon, url);
                    ThisComposite.StoreChildInList(tracer);

                    return tracer;
                }
            }

            return NullWebRequestTracer.Instance;
        }

        public IAction LeaveAction()
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} LeaveAction({Name})");
            }

            return DoLeaveAction(false);
        }

        public IAction CancelAction()
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug($"{this} CancelAction({Name})");
            }

            return DoLeaveAction(true);
        }

        private IAction DoLeaveAction(bool discardData)
        {
            lock (LockObject)
            {
                if (ThisAction.IsActionLeft)
                {
                    // LeaveAction was previously called
                    return ParentAction;
                }

                isActionLeft = true;
            }

            var childObjects = ThisComposite.GetCopyOfChildObjects();
            foreach (var childObject in childObjects)
            {
                if (discardData)
                {
                    if (childObject is ICancelableOpenKitObject cancelableObject)
                    {
                        cancelableObject.Cancel();
                    }
                    else
                    {
                        Logger.Warn(childObject.ToString() + " is not cancelable - falling back to Dispose() instead");
                        childObject.Dispose();
                    }
                }
                else
                {
                    childObject.Dispose();
                }
            }

            EndTime = Beacon.CurrentTimestamp;
            EndSequenceNo = Beacon.NextSequenceNumber;

            if (!discardData)
            {
                Beacon.AddAction(this);
            }

            // detach from parent
            parent.OnChildClosed(this);

            return ParentAction;
        }

        public TimeSpan Duration
        {
            get
            {
                lock(LockObject)
                {
                    var durationInMilliseconds = ThisAction.IsActionLeft
                        ? EndTime - StartTime
                        : Beacon.CurrentTimestamp - StartTime;

                    return TimeSpan.FromMilliseconds(durationInMilliseconds);
                }
            }
        }

        #endregion

        #region OpenKitComposite implementation

        public override int ActionId => Id;

        private protected override void OnChildClosed(IOpenKitObject childObject)
        {
            lock (LockObject)
            {
                ThisComposite.RemoveChildFromList(childObject);
            }
        }

        #endregion

        #region ICancelableOpenKitObject implementation

        void ICancelableOpenKitObject.Cancel()
        {
            CancelAction();
        }

        #endregion

        public override void Dispose()
        {
            LeaveAction();
        }
    }
}
