﻿using System;
using System.Diagnostics;
using System.IO.Packaging;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Boogie;
using Microsoft.Dafny;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using System.Text.RegularExpressions;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Collections.Generic;

namespace DafnyLanguageServer
{
    public class CounterExampleParams : IRequest<CounterExampleResults>
    {
        public string DafnyFile { get; set; }
    }

    public class CounterExampleResult
    {
        public int Line { get; set; }
        public int Col { get; set; }
        public Dictionary<string, string> Variables { get; } = new Dictionary<string, string>();

    }

    public class CounterExampleResults
    {
        public List<CounterExampleResult> CounterExamples { get; } = new List<CounterExampleResult>();

    }


    [Serial, Method("counterExample")]
    public interface ICounterExample : IJsonRpcRequestHandler<CounterExampleParams, CounterExampleResults>
    {
    }

    public class CounterExampleHandler : ICounterExample
    {

        private readonly BufferManager _bufferManager;

        public CounterExampleHandler(BufferManager b)
        {
            _bufferManager = b;
        }

        public async Task<CounterExampleResults> Handle(CounterExampleParams request, CancellationToken cancellationToken)
        {
            string programSource = _bufferManager.GetTextFromBuffer(new Uri(request.DafnyFile));
            var service = new CounterExampleService(request.DafnyFile, programSource);
            return await service.ProvideCounterExamples();
        }

    }
}