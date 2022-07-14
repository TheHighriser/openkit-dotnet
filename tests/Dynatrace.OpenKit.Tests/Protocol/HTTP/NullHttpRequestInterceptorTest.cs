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

using Dynatrace.OpenKit.API.HTTP;
using NSubstitute;
using NUnit.Framework;

namespace Dynatrace.OpenKit.Protocol.HTTP
{
    public class NullHttpRequestInterceptorTest
    {
        private IHttpRequest mockHttpRequest;

        [SetUp]
        public void SetUp()
        {
            mockHttpRequest = Substitute.For<IHttpRequest>();
        }

        [Test]
        public void InterceptDoesNotInteractWithHttpRequest()
        {
            // given
            var target = NullHttpRequestInterceptor.Instance;

            // when
            target.Intercept(mockHttpRequest);

            // then
            Assert.That(mockHttpRequest.ReceivedCalls, Is.Empty);
        }
    }
}