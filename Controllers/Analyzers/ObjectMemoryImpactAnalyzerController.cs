﻿using kedi.engine.Services;
using kedi.engine.Services.Analyze;
using kedi.engine.Services.Analyzers;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace kedi.engine.Controllers.Analyzers
{
    public class ObjectMemoryImpactAnalyzerController : ApiController
    {
        ObjectMemoryImpactAnalyzer analyzer = new ObjectMemoryImpactAnalyzer();

        [Route("api/analyzers/exceptiogn-analyzer/{sessionId}")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]string sessionId)
        {
            return Ok(analyzer.Analyze(sessionId));
        }

    }
}



