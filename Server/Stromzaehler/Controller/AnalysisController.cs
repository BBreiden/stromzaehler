
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stromzaehler.Analysis;
using Stromzaehler.Models;

namespace Stromzaehler.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        public AnalysisController(IBlinkData data) 
        {
            Data = data;
        }

        public IBlinkData Data { get; }

        //[HttpGet]
        //public IEnumerable<Blink> GetFilled() 
        //{
        //    new BlinkAnalysis(Data).FillGaps().ToArray();       
        //}
    }
}