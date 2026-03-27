using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FristGrpc.Tests.Unit.Helpers
{
    public class TestServerCallContext : ServerCallContext
    {
        public static TestServerCallContext Create(
        Metadata? headers = null,
        CancellationToken cancellationToken = default)
        {
            return new TestServerCallContext(
                headers ?? new Metadata(),
                cancellationToken == default ? CancellationToken.None : cancellationToken);
        }

        private readonly Metadata requestHeaders;
        private readonly CancellationToken cancellationToken;

        private TestServerCallContext(Metadata requestHeaders, CancellationToken cancellationToken)
        {
            this.requestHeaders = requestHeaders;
            this.cancellationToken = cancellationToken;
        }
        protected override string MethodCore => "MethodCore";

        protected override string HostCore => "HostCore";

        protected override string PeerCore => "PeerCore";

        protected override DateTime DeadlineCore { get; }

        protected override Metadata RequestHeadersCore => throw new NotImplementedException();

        protected override CancellationToken CancellationTokenCore => throw new NotImplementedException();

        protected override Metadata ResponseTrailersCore => throw new NotImplementedException();

        protected override Status StatusCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected override WriteOptions? WriteOptionsCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected override AuthContext AuthContextCore => throw new NotImplementedException();

        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
        {
            throw new NotImplementedException();
        }

        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
        {
            throw new NotImplementedException();
        }
    }
}
